# VirtualFlip
VirtualFlip small tool to allow basic functionality like tap and swipe on 4G flip phone via a PC UI
	
	VirtualFlip2
	
Get it at https://github.com/sh7411usa/VirtualFlip/releases
or direct download VirtualFlip2 v0.91BETA:
https://github.com/sh7411usa/VirtualFlip/releases/download/0.91BETA/VirtualFlip2.exe
This tool was designed with 4G Flip Phone Management in mind.

The main features include:
A)	A self updating screencast from the device display.
B)	Send Text to the device feature (type on your phone with your computer's keyboard)
C)	TAP, SWIPE, and LONGPRESS support. (use your mouse to manipulate the phone screen)
D)	An emulated (experimental) LG Exalt Keypad. (Currently for Exalt only)
E)	App installation feature. (Get apps from apps4flip)
F)	Screenshot feature. (save a screenshot of your phone to the computer)

Before we explain how to use it, it is necessary to understand a few things.
A)	This app requires a program called ADB in order to run. If you dont know what this is,
	you can make sure that it is installed by downloading this script:
	https://github.com/sh7411usa/VirtualFlip/releases/download/0.91BETA/ensureADB.ps1
	left click it, and choose "run with powershell".
	if prompted about the "execution policy", press [y] and press [enter].
	
B)	There is one more step to setting up ADB, you need to set your phone to allow computers
	in general to connect to it, and then your machine specifically, as well. You need to
	do both.
	For LG Exalt:
	1.)	If you are using an LG Exalt, enter ##PROGRAM220 (thats ##7764726220) on your
		dialpad, and press SEND.
	2.)	Enter 000000 as the service code.
	3.) When the "Verizon Hidden Menu" pops up, choose "Developer Options" from the list.
	4.) Click on #3, USB Debugging and turn it ON.
	
	For Kyocera Dura Max, Kyocera Dura XV, Kyocera Cadence or Orbic 4G Flip phones:
	1.)	Open the Settings app and go to Software Information.
	2.) Click on the build number 7 times, and then go back to Settings. A new option
		called developer options will appear.
	3.) click on Developer Options.
	4.)	find and enable USB Debugging
	
	Some phones require these additional steps:
	1.)	in the Settings app select Security and click on Unknown Sources, and enable
		it. (If you cant't find this option on your phone, then skip it.)
		
	All phone types continue here:
	1.) Your phone is now ready to connect generally with computers. Connect it to your PC.
	2.) A screen should pop up on your phone saying "Allow USB Debugging?", check the box
		and choose "OK".
	7.) Youre all set to use VirtualFlip2
	
C)	Please note that the screen update has a potential delay, and your TAPS, SWIPES, and
	Text are processed irrespective of the picture on the screen. that means that even if
	the picture didn't update yet, your taps and swipes will go through to the device in
	its current state. Be patient with the app and don't overload it while you wait. If
	you try to do something, wait for it to finish before the next click or send text.
	
D)	The application is currently only designed to work with one device attached to your
	PC at a time.
	
Youre all set to open VirtualFlip2!
A)	Send a tap to the device by clicking on the screen.
B)	If you click and hold in one place, a long press will be sent after 2 seconds.
C)	If you click and drag, a swipe will be sent from your starting point to the point where
	you let go, as soon as you let go of the mouse. The force, and route between start point
	and end point do not male a difference, neither does the speed at which you swipe.
D)	you can type text to the device by entering it into the text box and clicking send or
	pressing [enter].
F)	For all of these events, a status is displayed for five seconds at the bottom of the
	screen.
G)	for your convenience, mouse coordinates are provided where ever the mouse moves over
	the display. these can be used in various ADB commands.
H)	The app will automatically detect if it is connected to an LG exalt (as long as the its
	USB) and will enable the R&L Soft keys on the display. other phones can still make use
	of the LG exalt keyboard layout, under menu>flip keypad.
I)	you can save a screenshot in png format using menu>save screenshot
J)	you can (attempt) to install apk files (android app packages) to the device using
	menu>install app

obviously the software is provide as is with no gaurantees, and no responsibility is
taken by the dev for any eventualities arising even indirectly from the use of VirtualFlip2

You can get apps for 4G flip phones from apps4flip.com (unaffiliated). once the app files
are in your download folder, you can easily install and set them up using VirtualFlip2.

Here we give instructions how to install waze from their site using our app. (btw, we take
no responsibility for any apps promoted on any platform - even our own). this guide is for
educational purposes.
1.)	Download the App file (*.apk)
2.) Launch VirtualFlip after following instructions above.
3.) go to menu>install app
4.) find the app you just downloaded, and click ok
5.) be patient. the larger the app, the longer it takes. you will be notified when install
	completes.
6.) the app installs in the background, so you can continue to use virtualflip while it installs
7.) launch the app. you can do this in virtualflip2 under menu>preset Commands>launch waze
8.) on its first launch, the app will need some set up. scroll to the bottom of each page
	and follow the on screen prompts. click on whatever you need to get through. be patient.

Additional questions and suggestions should be addressed to sh7411usa01@gmail.com
please dont harass. enjoy.
