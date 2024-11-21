# :high_brightness: LogiWiz - Logitech G13, G15, and G510 Applet :high_brightness:
## Control Philips Wiz Bulbs :bulb: with your keyboard!
This is an applet for the Logitech G13, G15, and G510 keyboards used to control Philips Wiz smart bulbs. This even works with the Logitech Z-10 speakers.

![C#](https://img.shields.io/badge/C%23-6600cc.svg)
[![GPL-3.0](https://img.shields.io/badge/License-GPL%203.0-yellow.svg)](https://www.gnu.org/licenses/gpl-3.0.en.html)
[![Open Source Friendly](https://img.shields.io/badge/Open%20Source%20Friendly-violet.svg)](https://open-source.org/)
![GitHub commit activity](https://img.shields.io/github/commit-activity/m/OLeDouxEt/LogiWiz_G15_LCD_Applet)


## Table of Contents

* [Getting Started](#getting-started)
* [Features](#features)
* [Usage](#usage)
* [License](#license)

## Getting Started

To get started with this project, follow these steps:

1. Have one of these Logitech keyboards: G13, G15, and G510. The Logitech Z-10 Speakers can also be used.
2. Install Logitech Gaming Software. (Tested with Version 9.02 and 9.04)
3. Install Microsoft Visual Redistributable 2012 (VC++ 11.0) if not already installed
4. Download the zip file under releases
5. Unzip and open the folder
6. Run the ".exe"

## Features

* Adjust bulb brightness
* Lower or raise bulb temperature
* Turn blubs on and off
* Chnage to one of the pre-configured scenes
* Cycle through list of bulbs and select one to adjust

## Usage
Once downloaded and unzipped, you can launch "LogiWiz.exe" and a file named "Config.txt" will be created in the same folder as the ".exe".
Populate this file with the local IP addresses of the bulbs by entering each IP on a new line. After this, press the button below "Bulb" on the display to select the bulb you would like to adjust.

### Example Usage
<!--To use this project, simply run the application and follow the instructions in the console.-->
Cycle through list of bulbs and select one to adjust

Brightness:

![LogiWiz_Bright](https://github.com/user-attachments/assets/90d963c1-4da9-44c7-a9d4-5b815d631af8)

- Use the plus and minus buttons to adjust brightness level by 10 unints between 10 and 100.

Power Toggle:

![LogiWiz_On_Off](https://github.com/user-attachments/assets/08fcb0ce-d3e1-4ad8-aeee-f5d327e7bedc)

- On and off buttons allow you to change the bulb's power state.

Temperature:

![LogiWiz_Temp](https://github.com/user-attachments/assets/8810126a-c9ae-4cb8-a311-92e1093225db)

- Make a bulb's light warmer or cooler by increments of 500 between 2200k and 6000k.

Scenes:

![LogiWiz_Scene](https://github.com/user-attachments/assets/e1788db3-a571-499e-936c-1bb8a0ef649a)

- Rotate between the pre-defined, static and dynamic, color scenes of the bulb as follows:
  * 1: "Ocean"
  * 2: "Romance"
  * 3: "Sunset"
  * 4: "Party"
  * 5: "Fireplace"
  * 6: "Cozy"
  * 7: "Forest"
  * 8: "Pastel Colors"
  * 9: "Wake up"
  * 10: "Bedtime"
  * 11: "Warm White"
  * 12: "Daylight"
  * 13: "Cool white"
  * 14: "Night light"
  * 15: "Focus"
  * 16: "Relax"
  * 17: "True colors"
  * 18: "TV time"
  * 19: "Plantgrowth"
  * 20: "Spring"
  * 21: "Summer"
  * 22: "Fall"
  * 23: "Deepdive"
  * 24: "Jungle"
  * 25: "Mojito"
  * 26: "Club"
  * 27: "Christmas"
  * 28: "Halloween"
  * 29: "Candlelight"
  * 30: "Golden white"
  * 31: "Pulse"
  * 32: "Steampunk"
  * 33: "Diwali"
  * 34: "White"
  * 35: "Alarm"


## License

[GPL-3.0](https://github.com/OLeDouxEt/LogiWiz_G15_LCD_Applet/blob/main/LICENSE)
