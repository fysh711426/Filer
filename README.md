# Filer  

This is a file browser, you can use your mobile device to browse files on your computer on a local area network.  

---  

### Demo  

Open url on mobile.  

![Demo/1662989508113.jpg](Demo/1662989508113.jpg)  

---  

### Get Started  

1. Download and unzip the file.  
[releases/Filer-v1.0.0-win-x64.zip](https://github.com/fysh711426/Filer/releases)  

2. Open and setting `appsettings.json`.  

```C#
// Set server ip and port
// http://{server_ip}:{port}
"Kestrel": {
  "Endpoints": {
    "Http": {
      "Url": "http://192.168.1.100:2048"
    }
  }
}
```

```C#
// Set shared directory
"WorkDirs": [
  {
    "Name": "Shared",
    "Path": "D:\\Shared"
  }
]
```

3. Execute `Filer.exe`.  

![Demo/1662988648589.jpg](Demo/1662988648589.jpg)  

---  

### Add firewall rules  

The firewall blocks the ports of the computer, so we need to set up rules to allow specific IP connections.  

1. Open `Windows Defender` Advanced settings.  

![Demo/1662990495644.jpg](Demo/1662990495644.jpg)  

2. Add input rules.  

![Demo/1662990748973.jpg](Demo/1662990748973.jpg)  

![Demo/1662991121906.jpg](Demo/1662991121906.jpg)  

![Demo/1662990803599.jpg](Demo/1662990803599.jpg)  

![Demo/1662990844820.jpg](Demo/1662990844820.jpg)  

> Specify the IP address between `192.168.1.100` and `192.168.1.110` to connect.  