# FreeMove
[![license](https://img.shields.io/github/license/ImDema/FreeMove.svg)](https://github.com/imDema/FreeMove/blob/master/LICENSE.txt)

Move directories freely without breaking installations or shortcuts

You can use this tool to move programs that install on C:\ by default to another drive to save space on your main drive

### How It works
1. The files are moved to the new location
2. A [symbolic link](https://www.wikiwand.com/en/NTFS_junction_point) is created from the old location redirecting to the new one. Any program trying to access a file in the old location will automatically be redirected to its new location

## Download
[![Github All Releases](https://img.shields.io/github/downloads/imDema/FreeMove/total.svg)](https://github.com/imDema/FreeMove/releases/latest)

#### From GitHub

[Download the latest release](https://github.com/imDema/FreeMove/releases/latest)

#### From Scoop

```
scoop install freemove
```

### Usage

Run the executable and use the GUI

> Note: this program requires administrator privileges for its core functionality

## Recommendations
You should not move important system directories as they can break core functionalities like Windows Update and Windows Store Apps.

`C:\Users` - `C:\Documents and Settings` - `C:\Program Files` - `C:\Program Files (x86)` should not be moved. If you wish to do It anyway do it at your own risk. To move a directory back refer to the last part of the readme.

That said, moving directories contained in the previously mentioned directories should not cause any problem. So you are free to move `C:\Program Files\HugeProgramIDontWantOnMySSD` without any problem.

## Screenshots
![Screenshot](http://i.imgur.com/fW6ZEg3.png)

## Uninstalling moved programs
Uninstall the program just as you would normally without deleting the junction. The uninstaller will work normally leaving an empty directory in the location you moved the program to, and the directory link in the original location, both of which you can then delete manually

## Moving back a program
Delete the junction in the old location (this won't delete the content) and move the directory back to its original position

## Contributing

The project is currently being maintained for fixes, but no new features are currently in development or planned.

I wrote the tool alone and I am currently the only developer, at the moment I'm pursuing my PhD and I'm too busy with other projects to work on new features.

I'll keep watching the project and managing possible contributions, so, if you are interested in contributing, leave an issue or comment on an open issue and let me know!
