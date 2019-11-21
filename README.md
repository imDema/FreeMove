# FreeMove
[![license](https://img.shields.io/github/license/ImDema/FreeMove.svg)](https://github.com/imDema/FreeMove/blob/master/LICENSE.txt)

Move directories freely without breaking installations or shortcuts

You can use this tool to move programs that install on C:\ by default to another drive to save space on your main drive

> NOTE: Currently rewriting a lot of old code to make the codebase more robust and maintainable

### How It works
1. The files are moved to the new location
2. A [symbolic link](https://www.wikiwand.com/en/NTFS_junction_point) is created from the old location redirecting to the new one. Any program trying to access a file in the old location will automatically be redirected to its new location

## Download

[![Github All Releases](https://img.shields.io/github/downloads/imDema/FreeMove/total.svg)](https://github.com/imDema/FreeMove/releases/latest)

[Download the latest release](https://github.com/imDema/FreeMove/releases/latest)

### Usage

Run the executable and use the GUI

Depending on your Windows version the program could need to be run as administrator

## Recommendations
You should not move important system directories as they can break core functionalities like Windows Update and Windows Store Apps.

`C:\Users` - `C:\Documents and Settings` - `C:\Program Files` - `C:\Program Files (x86)` should not be moved. If you wish to do It anyway do it at your own risk. To move a directory back refer to the last part of the readme.

That said, moving directories contained in the previously mentioned directories should not cause any problem. So you are free to move `C:\Program Files\HugeProgramIDontWantOnMySSD` without any problem.

## Screenshots
![Screenshot](http://i.imgur.com/fW6ZEg3.png)

## Uninstalling moved programs
Uninstall the program just as you would normally without deleting the junction. The uninstaller will work normally leaving an empty directory in the location you moved the program to, and the directory link in the original location, both of which you can delete manually

## Moving back a program
Delete the junction in the old location (this won't delete the content) and move the directory back to its original position


---------------------------------------------------------------------

Want to offer me a beer?
```
BTC:    326HDAYj5bJ5sDn6RcnHnvNEfDkk466USX
ETH:    0xf48E9A99602D6eCC685E33837A47130D7C5fd65D
NANO:   xrb_3ntghh4bej57is4noa1sxaiefgxinwfqraxm5donp1qdeytnmnoamdq9xfxh
```
