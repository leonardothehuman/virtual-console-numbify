# Virtual Console Numbify
An automated tool to create Wii virtualconsole injections

Created by leonardothehuman (leonardorg)

This is an automation tool that uses several third part tools to automatically replace
Rom files on Nintendo Wii's virtual console's Wad files, allowing you to create your own
Injects.

## License

Virtual Console Numbify is released under the terms of the GNU General Public License v3.

See "GNU-Gpl-3.0.txt" for more information.

## Prerequisites

Autoit3: Needed to automate HowardCtools
Download -> https://www.autoitscript.com/site/autoit/downloads/

ActiveX Control Pad: Needed by HowardCtools
Download -> http://download.microsoft.com/download/activexcontrolpad/install/4.0.0.950/win98mexp/en-us/setuppad.exe

VB6 runtime plus: Needed by HowardCtools
Download -> https://sourceforge.net/projects/vb6extendedruntime/

FM20: Needed by HowardCtools
Download -> https://www.softpedia.com/get/Programming/Other-Programming-Files/Microsoft-Forms.shtml

Net framework 2.0: Needed by LibWiiSharp, Comes preinstalled on the most recent windows versions
(but need to be enabled on control panel), for older versions, it can be downloaded at
https://www.microsoft.com/pt-br/download/details.aspx?id=22

Net framework 4.8: Needed by Virtual Console Numbify itself, Comes preinstalled on the most recent windows versions,
for older versions, it can be downloaded at
https://support.microsoft.com/pt-br/topic/instalador-offline-do-microsoft-net-framework-4-8-para-windows-9d23f658-3b97-68ab-d013-aa3c3e7495e0

common-key.bin: You must extract this file from your own wii and put it into Virtual Console Numbify's directory

## Usage

Just fill all the fields and follow your heart...

## Planned features
- [ ] Flash support

- [ ] PC engine CD support

- [ ] Commodore 64 support

- [ ] MSX support

- [ ] Automatic byteswap on nintendo 64

## Known Bugs
- [x] Prerequisites check don't work on x64 version

- [x] Autoit installation is not checked

- [x] Pc engine save title have one single line

- [x] Neo Geo aes save title can't have more than one line

- [x] Some Master system wad's roms don't have sms extension, so the rom replacement is skipped, one of the base wads affected is Alex Kidd - The Lost Stars (USA) (SMS) (Virtual Console)

## Credits

I only could build this tool, because I was standing on the shoulder of giants, Thanks for all those wondertful people for creating
the tools that Virtual Console Numbify uses:


libwiisharp - leathl

wadunpacker.exe - by BFGR

wadtool.exe - by Calantra and DDF 

bannertool.exe - by Calantra

wadpacker.exe - by KiKe

sha1 - unknown author

everything else from autoinjectwad - Creffca (AKA Avicr)

Devilken's vistual console injector - DevilKen

HowardC_Tools - HowardC

The steps used by Virtual Console Numbify was mostly based on this tutorial:
https://gbatemp.net/threads/how-to-create-professional-virtual-console-injects.439224/