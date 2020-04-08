namespace ringriker_trn
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.lb_info = new System.Windows.Forms.Label();
            this.bt_about = new System.Windows.Forms.Button();
            this.lb_trn_status = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lb_info
            // 
            this.lb_info.AutoSize = true;
            this.lb_info.Location = new System.Drawing.Point(12, 9);
            this.lb_info.Name = "lb_info";
            this.lb_info.Size = new System.Drawing.Size(86, 13);
            this.lb_info.TabIndex = 0;
            this.lb_info.Text = "Trainer loading...";
            // 
            // bt_about
            // 
            this.bt_about.Location = new System.Drawing.Point(324, 227);
            this.bt_about.Name = "bt_about";
            this.bt_about.Size = new System.Drawing.Size(96, 24);
            this.bt_about.TabIndex = 1;
            this.bt_about.Text = "About";
            this.bt_about.UseVisualStyleBackColor = true;
            this.bt_about.Click += new System.EventHandler(this.bt_about_Click);
            // 
            // lb_trn_status
            // 
            this.lb_trn_status.AutoSize = true;
            this.lb_trn_status.Location = new System.Drawing.Point(12, 241);
            this.lb_trn_status.Name = "lb_trn_status";
            this.lb_trn_status.Size = new System.Drawing.Size(0, 13);
            this.lb_trn_status.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(432, 263);
            this.Controls.Add(this.lb_trn_status);
            this.Controls.Add(this.bt_about);
            this.Controls.Add(this.lb_info);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ringriker Trainer";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lb_info;
        private System.Windows.Forms.Button bt_about;
        private System.Windows.Forms.Label lb_trn_status;



    }
}

