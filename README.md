# Ring Riker game trainer

## Info
Maybe the worst game to make trainer for.
With CheatEngine you can find the values, but you can't
intercept anything in the game. Pointer scan results
became useless after 2-3 restarts, code noping doesn't work
because the program code is allocated to random places.
To fix this the program scans every 0.2 seconds for a certain
assembly code, if it's ok, I use code injection on the
HP value getter and use that memory address.

## Usage
1. Start ringriker_trn.exe
2. Start the game
3. Press any of the hotkeys in-game

D0, D1, ... = Your normal number keys on the keyboard (not NUMPAD)
