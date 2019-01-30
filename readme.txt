Lazurite - home automation software.
------------------------------------
- LazuriteDiagram.uml contains class diagram of all non-ui projects;
- Lazurite directory contains all ui and non-ui projects;
- LazuritePlugins contains plugins source for Lazurite:
	- CommonPlugin allows user to get current date and time, kill processes, show some message to user and etc.;
	- ModbusPlugin - is a plugin for controlling devices that work by Modbus protocol. Powered by NModbus;
	- MultimediaKeysPlugin - emulation of keyboard key press;
	- PingPlugin - allows user to ping some devices in network;
	- RunProcessPlugin - start and control Windows processes;
	- VolumePlugin - change volume level of current audio output device / change default output;
	- WakeOnLanPlugin - power on some computers in LAN by Wake-on-Lan protocol;
	- ZWavePlugin allows user to control ZWave devices from complex scenarios and triggers. Powered by OpenZWave;
	- SendingMessagesPlugin - send message to user or users devices;
	- UserGeolocationPlugin - allows to calculate distance between user and place, determine user place and view current user geolocation.
- Libs contains external libraries;
- Releases
	- Plugins folder contains compiled and zipped *.pyp plugins files;
	- PluginsMaterial contains binaries for use in plugin projects;
	- PluginsCreator contains program for ".pyp" (Lazurite plugin file) files creation;
	- ReleaseBinaries contains all Lazurite distrib files;
	- ReleaseInstaller contains installer file;
	- AndroidApp - APK file of Lazurite Android client;
------------------------------------
Features short list:
- Complex scenarios and triggers;
- Control home from android smartphone;
- Create your own plugins;
- Secure connection by SSL and custom encrypting by secret code;
- Manage user rights;
- ZWave and Modbus "from box";
- Geolocation actions;
- Send and recieve messages;
- Statistics maintaining
------------------------------------

Copyright 2018 Novgorodcev Anton

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0
