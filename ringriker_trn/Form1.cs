using System;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Media;

namespace ringriker_trn
{
    public partial class Form1 : Form
    {
        class HotkeyItem
        {
            public uint offset;
            public Keys key;
            public string name;
            public bool cantoggle;
            public bool toggled;

            public HotkeyItem(uint offset, Keys key, string name)
            {
                this.offset = offset;
                this.key = key;
                this.name = name;
            }
        }

        HotkeyItem[] hotkeys =
        {
            new HotkeyItem(0,Keys.D1, "Power-Jump"),
            new HotkeyItem(1,Keys.D2, "Power-Run"),
            new HotkeyItem(2,Keys.D3, "Fast-Attack"),
            new HotkeyItem(3,Keys.D4, "Divine Protection"),
            new HotkeyItem(4,Keys.D5, "Health Regeneration"),
            new HotkeyItem(5,Keys.D6, "Lock Pick"),
            new HotkeyItem(10,Keys.D7, "Unlimited Health") { cantoggle = true },
            new HotkeyItem(11,Keys.D8, "Unlimited Energy") { cantoggle = true },
            new HotkeyItem(18,Keys.D9, "Unlimited Wolf HP") { cantoggle = true },
            new HotkeyItem(16,Keys.D0, "Wolf Call"),
            new HotkeyItem(45,Keys.F1, "Chest key"),
            new HotkeyItem(70,Keys.F2, "Adjustments/Experience points")
        };

        const uint JMP_LEN = 5;

        byte[] hook_jump =
        {
            0xE9, 0x00, 0x00, 0x00, 0x00,       //jmp newcode
            0x90                                //nop
        };

        byte[] hook_inject =
        {
            0x89, 0x1D, 0x00, 0x00, 0x00, 0x00, //mov [newcode+11], ebx
            0x8B, 0x1B,                         //mov ebx,[ebx]
            0x53,                               //push ebx
            0xD9, 0x04, 0x24,                   //fld dword ptr [esp]
            0xE9, 0x00, 0x00, 0x00, 0x00,       //jmp originalcode
            0x00, 0x00, 0x00, 0x00              //HP address (intercepted from EBX register)
        };

        uint inj_addr = 0;
        uint player_addr = 0;

        MemoryEdit.Memory mem;
        KeyHook.GlobalKeyboardHook gkh;
        bool trn_seek = true;
        bool trn_active = false;
        bool trn_running = true;

        //Seek and constant memory check, injection
        Thread thd_main;
        Thread thd_seek;

        //Activate sounds
        SoundPlayer snd_ac;
        SoundPlayer snd_de;

        public Form1()
        {
            InitializeComponent();
            GenerateHelpText();
            lb_trn_status.Text = "Waiting for game...";
            thd_seek = new Thread(TrainerSeek_Thd);
            thd_seek.Start();
            snd_ac = new SoundPlayer(Properties.Resources.activate);
            snd_de = new SoundPlayer(Properties.Resources.deactivate);
        }

        void InitTrainer()
        {
            EnableHooks();
            InterceptAddress();
            thd_main = new Thread(Trainer_Thd);
            thd_main.Start();
            trn_active = true;
        }

        void EnableHooks()
        {
            mem = new MemoryEdit.Memory();
            if (!mem.Attach("RingRiker", 0x001F0FFF))
            {
                MessageBox.Show("Failed to init trainer.\nTrainer closing...", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }
            gkh = new KeyHook.GlobalKeyboardHook();
            if (!gkh.Hook())
            {
                MessageBox.Show("Failed to init trainer hotkey handler.\nTrainer closing...", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }
            gkh.KeyUp += gkh_KeyUp;
        }

        void InterceptAddress()
        {
            try
            {
                uint base_addr = 0;
                //Check 3 addresses in integer (assembly check)
                for (uint ptr = 0x02000CE5; ptr < 0x03000CE5; ptr += 0x00001000)
                {
                    bool chk = true;
                    uint tmp = ptr;
                    uint code = 0;
                    chk &= mem.ReadUInt32(tmp) == 0x3D811E89;
                    tmp -= 0xB493;
                    chk &= mem.ReadUInt32(tmp) == 0xD9531B8B;
                    if (chk)
                    {
                        tmp -= 0x38;
                        code = mem.ReadUInt32(tmp);
                        chk &= code == 0xD9531B8B;
                        if (chk)
                        {
                            base_addr = tmp;
                            break;
                        }
                        else if ((code & 0x000000E9) == 0x000000E9)
                        {
                            base_addr = tmp;
                            tmp = (uint)(mem.ReadUInt32(tmp + 1) + tmp + JMP_LEN);
                            inj_addr = tmp;
                            break;
                        }
                    }
                }
                //Not in-game yet?
                if (base_addr == 0)
                {
                    return;
                }
                if (inj_addr == 0)
                {
                    inj_addr = (uint)mem.Allocate(0x20);
                }
                //base_addr
                //jmp newmem
                UpdatePointer(GetJumpAddr(base_addr, inj_addr, 0), 1, hook_jump);
                //hook_inject
                //mov [newcode+11], ebx
                UpdatePointer(inj_addr + 17, 2, hook_inject);
                //jmp originalcode
                UpdatePointer(GetJumpAddrReturn(inj_addr, base_addr, 17), 13, hook_inject);
                //Inject interceptor
                mem.WriteByte(inj_addr, hook_inject, hook_inject.Length);
                //Redirect original code
                mem.WriteByte(base_addr, hook_jump, hook_jump.Length);
                uint ptr_player = 0;
                do
                {
                    Thread.Sleep(100);
                    ptr_player = mem.ReadUInt32(inj_addr + 17);
                }
                while (ptr_player == 0);
                player_addr = ptr_player - 0x28;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occurred while injecting: " + ex.Message + "\nTrainer closing...",
                Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                Environment.Exit(0);
            }
        }

        void UpdatePointer(uint ptr, uint offset, byte[] arr)
        {
            Array.Copy(BitConverter.GetBytes(ptr), 0, arr, offset, 4);
        }

        uint GetJumpAddr(uint source, uint target, uint offset)
        {
            return target - (source + JMP_LEN);
        }

        uint GetJumpAddrReturn(uint source, uint target, uint offset)
        {
            return (target + JMP_LEN) - (source + offset);
        }

        void GenerateHelpText()
        {
            lb_info.Text = string.Empty;
            foreach (HotkeyItem h in hotkeys)
            {
                lb_info.Text += h.name + " - CTRL + " + h.key + "\n";
            }
        }

        void gkh_KeyUp(object sender, KeyEventArgs e)
        {
            //If game is not focused don't check hotkeys
            if (!mem.IsFocused() || player_addr == 0)
            {
                return;
            }
            if (e.Control)
            {
                foreach (HotkeyItem h in hotkeys)
                {
                    if (h.key == e.KeyCode)
                    {
                        mem.WriteByte(player_addr + h.offset * 4, BitConverter.GetBytes(1000f), 4);
                        if (h.toggled) snd_de.Play();
                        else snd_ac.Play();
                        if (h.cantoggle)
                        {
                            h.toggled = !h.toggled;
                        }
                        break;
                    }
                }
            }
        }

        void bt_about_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This program was made in Visual C# 2008 Express\nBy Kurtis (2020)",
                Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            trn_running = false;
            trn_seek = false;
            Environment.Exit(0);
        }

        void Trainer_Thd()
        {
            while (trn_running)
            {
                if (trn_active)
                {
                    InterceptAddress();
                    foreach (HotkeyItem h in hotkeys)
                    {
                        if (h.toggled)
                        {
                            mem.WriteByte(player_addr + h.offset * 4, BitConverter.GetBytes(1000f), 4);
                        }
                    }
                }
                Thread.Sleep(200);
            }
        }

        void TrainerSeek_Thd()
        {
            while (trn_seek)
            {
                if (Process.GetProcessesByName("RingRiker").Length > 0)
                {
                    trn_seek = false;
                    Invoke(new MethodInvoker(() =>
                    {
                        lb_trn_status.Text = "Active";
                        InitTrainer();
                    }));
                }
                Thread.Sleep(1000);
            }
        }
    }
}