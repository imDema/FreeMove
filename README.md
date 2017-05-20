# FreeMove
Move directories (even from one drive to another) freely without breaking installations or shortcuts

If a program installs on C:\ by default or you want to move the installation folder of a program to somewhere else without breaking it you can use this program.
## How It works
1. The files are moved to the new location
2. A directory junction is created from the old location to the new one. This way trying to access a file from its old location will simply redirect to the new one
## Usage
Just run the executable and use the GUI

If you want to move from or to a directory, like those contained in C:\Program Files, which requires administrative privileges, run the program as an administrator or it won't work!

## Recommendations
You should not move important system directories as they can break core functionalities like Windows Update and Windows Store Apps.

`C:\Users` - `C:\Documents and Settings` - `C:\Program Files` - `C:\Program Files (x86)` should not be moved. If you wish to do It anyway do it at your own risk. To move the directory back refer to the last part of the readme.

However moving directories contained in the previously mentioned directories should not cause any problem. So you are free to move `C:\Program Files\HugeProgramIDontWantOnMySSD` without any problem.

## Screenshots
![Screenshot](http://i.imgur.com/svWyDZ6.png)

## Uninstalling moved programs
To uninstall you a program you should proceed as you would do normally without deleting the junction. This way the uninstaller will delete all the files It needs to (even if they are in the new location) and leave the junction file and an empty directory where you moved the program which you can choose to delete manually or not (they take up almost no space)

## Moving back a program
Delete the junction (this won't delete the content) in the old position and move the directory back to its original position
