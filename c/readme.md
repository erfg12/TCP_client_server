## Setup Build Environment
- [Windows] Install [MinGW](https://sourceforge.net/projects/mingw), add `C:\MinGW\bin` to Environment Variables PATH.
- [MacOS] https://discussions.apple.com/thread/5250041
- [Ubuntu] In terminal run `sudo apt install build-essential`

## How To Build
- [Windows] Open Command Prompt, navigate to `c/src` directory, type `mingw32-make`
- [MacOS & Ubuntu] Open Terminal, navigate to `c/src` directory, type `make`

## How To Debug
1) Type `gdb client <or> server.exe`, type `start` once prompted with (gdb)
2) If it trips a breakpoint immediately, type `continue`