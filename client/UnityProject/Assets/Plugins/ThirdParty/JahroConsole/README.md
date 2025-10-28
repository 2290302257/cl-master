#Jahro

Jahro Console is a brand new plugin for Unity 3D. It The Jahro strives to provide an amazing development experience by an opportunity to use the console for executing commands in a Unity Project directly or in a build of an app on a target device (after integration and building).


## Requirements for UnityEditor
The following Unity 3D software versions make business with Jahro: 2019.3.0f5 and higher.

## Requirements for Mobile version
Specially designed for the mobile app debugging process, Jahro brings all its capabilities with the application version on a mobile device.
Jahro is compatible with the following software:
* Android v.5 and higher;
* iOS v.10 and higher.

## Installation 
The plugin installation process is comfortable and simple as :
* Open Unity Editor.
* Go to Window menu -> Package manager.
* Select My Assets from the package scope drop-down menu.
* Find the Jahro package (or type Jahro in the search field) to install from the [list of packages](https://docs.unity3d.com/Packages/com.unity.package-manager-ui@2.0/manual/index.html#PackManLists)
* Once the package information appears in the details panel, select the required version from the drop-down menu (or just select the latest one).
* Click Download and Install buttons. 
* Once the process is finished, the new package is ready to use.
More information about package installation is available at Unity official docs: https://docs.unity3d.com/Manual/upm-ui-update.html 


## Deactivation
> “Note”: 
> By default, Jahro Console is enabled in Dev and Prod builds. To be careful in delivering builds to non-Jahro experienced users, it can be deactivated from a build.
Taking mom-style care about the project's data, Jahro deactivation will not affect the application code and the performance of a build. 
For safe deactivation:
* Open Unity Editor.
* Go to Tools menu -> Jahro Settings.
* Remove the checkmark next to Jahro Enabled.
A disabled built-in package can be re-enabled by setting the checkmark next to Jahro Enabled. In the case of the plugin’s reactivation, Jahro code lines become available in a project again.

## Settings
> “Note”: 
>Settings menu is not available in Mobile version.
Jahro is customization friendly and flexible.
To configure the plugin:
* Go to the Tools menu of the Unity main window.
* Select Jahro Setting.
* General settings include:
* Enable Jahro - adding or removing the checkmark next to this option Activates/Deactivates Jahro console.
* Show UI Launch Button - adding or removing the checkmark next to this option enables/disables Launch Button.
* Launch by keyboard shortcuts - adding or removing the checkmark next to this option allows using some shortcuts for managing console.
> “Note”: 
> It’s not possible to add custom shortcuts at the moment.
* Launch by Tap Area - adding or removing the checkmark next to this option allows using fast four taps launch on Mobile version.
Assemblies section allows selecting assemblies that contain Jahro.

## Removal
> “Note”: 
> Removal can bring harm and real troubles to the existing commands and API callbacks. Using Deactivation instead of Removal to save a build’s performance, time, and nerves.
The plugin removal process includes the following steps:
* Open Unity Editor.
* Go to Window menu -> Package manager.
* Select Jahro console from the list.
* Press the Remove button in the lower-left corner of the window.

## Commands
It’s possible to create custom commands for Jahro Console or apply the existing ones.
* Open C# script you use for commands creation in Project.
* Add JahroConsole namespace “using JahroConsole” to connect it to Jahro.
* Add the required command to the document using the following attributes:
* JahroCommand - Used to mark a command that will be used as a Command in Visual mode.
* methodName - Used to identify the command.
* methodDescription - Adds a description to a command.
* groupName - Assigns a command to a group it should be related to.
* Once a command is added, it will be possible to use it in both Console and Visual modes.

## Supported parameters:
* int
* bool
* string
* float
* double
* Vector2
* Vector3
* enum
* and arrays:
* int[]
* float[]
* bool[]
* string[]
* float[]
* double[]
* Vector3[]
* Vector2[]
...and any combination of primitive types described above.
Supported returned value:
* void
* string
If you need information about supported commands, please use  “Help” command.

