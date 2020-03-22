#Ensure that ADB is installed globally on a windows machine by sh7411usa
if (!(Get-Command adb -ErrorAction SilentlyContinue)){
  Set-ExecutionPolicy Bypass -Scope Process -Force;
	if (!(Get-Command choco -ErrorAction SilentlyContinue)){
	  iex (New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1');
  }
	choco install adb --force -y
}
