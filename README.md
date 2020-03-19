# VirtualFlip
VirtualFlip small tool to allow basic functionality like tap and swipe on 4G flip phone via a PC UI

-Only works with one device connected at a time. Make sure ADB is installed globally or is stored in the same directory as the executable.
You can run the following Powershell Script to ensure ADB is up and running on your device:

if(!(Get-Command adb -ErrorAction SilentlyContinue)){if(!(Get-Command choco -ErrorAction SilentlyContinue)){iex((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))}choco install adb --force -y}
