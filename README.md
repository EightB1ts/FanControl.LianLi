# FanControl.LianLi

An unofficial LianLi plugin for [FanControl](https://github.com/Rem0o/FanControl.Releases).

[![Support](https://img.shields.io/badge/Support-Buy_Me_A_Coffee-yellow?style=for-the-badge&logo=buy%20me%20a%20coffee&color=FFDD00)](https://www.buymeacoffee.com/CameronHalter)

## Device Support

| Device                          | PID          | Status                          | Read Fan RPM | Set Fan RPM |
| ------------------------------- | ------------ | ------------------------------- | ------------ | ----------- |
| LianLi-UNI SL                   | `7750, a100` | Supported                       | ✔️          | ✔️          |
| LianLi-UNI AL                   | `a101`       | Untested                        | ❓           | ❓          |
| LianLi-UNI SL-Infinity          | `a102`       | Supported                       | ✔️           | ✔️         |
| LianLi-UNI SL v2                | `a103, a105` | Untested                        | ❓           | ❓          |
| LianLi-UNI AL v2                | `a104`       | Untested                        | ❓           | ❓          |

## Installation

1. Download latest [release](https://github.com/EightB1ts/FanControl.LianLi/releases). You will have 2 options once you unzip the file:
    - **FanControl.LianLiPlugin.ARGB.dll (recommended)**: This will set LED effects to match the motherboard ARGB header.
    - **FanControl.LianLiPlugin.dll**: This will not configure any LED settings.

    Please review the [Known Issues](https://github.com/EightB1ts/FanControl.LianLi#known-issues) section to determine which version is right for you.

2. Once you determine which version of the plugin you want to use, drag it into FanControl's `Plugins` folder.

## Known Issues

1. Sharing the controllers across multiple pieces of software at the same time will lead to issues. For example, using this plugin along with OpenRGB. If you want to dynamically change the RGB, please use **FanControl.LianLiPlugin.ARGB.dll** and connect the controller to your motherboard's ARGB header.

## Submitting An Issue

When submitting an issue, please include the Name, VID, and PID of your controller. It can be located within Device Manager:

![Device Manager](https://raw.githubusercontent.com/EightB1ts/FanControl.LianLi/main/images/DeviceManager.PNG)

## Screenshots

![Screenshot 1](https://raw.githubusercontent.com/EightB1ts/FanControl.LianLi/main/images/Screenshot1.PNG)


