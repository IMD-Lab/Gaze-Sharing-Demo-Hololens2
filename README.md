# Gaze-Sharing-Demo-Hololens2
Demo for sharing gaze between two Hololens2

***Piano Demo:***

![PianoDemo](https://user-images.githubusercontent.com/39384021/209696668-f7660dfa-1de0-49cf-bed9-c895a6e51bd7.gif)

Full demo video [here](https://www.dropbox.com/s/9ymutfngsqrsz6j/TPP%20AR%20Eyegaze.mp4?dl=0).

## Vuforia Installation
Because vuforia package is too large for git, get [Vuforia-10.7.2](https://www.dropbox.com/sh/k5pv9btk3b5amho/AACw_EzHn0N1oDrP05bOhmvea?dl=0) and place in both Gaze-Sender/Packages and Gaze-Receiver/Packages directory before opening in Unity to install the package smoothly.

## Heatmap
This project uses the Unity3D heatmap implementation of [kDanik](https://github.com/kDanik/heatmap-unity)

![heatmap](https://user-images.githubusercontent.com/39384021/209697388-5d555a87-ca2b-4799-b847-d4d292488b7a.png)

## Calibration
This project uses Vuforia and Anchors to calibrate Sender and Receiver.

1. Download and print the [Vuforia stones image](https://www.dropbox.com/sh/ipjqhoc6k1b58is/AADxfMo4N6SVxsAM0K07dRxba?dl=0). (A3 size, use either the docx or jpg)
2. When in Unity playmode and just want to debug, in AnchorScript.cs attached to Stage, set *CalibOnStart* value to *false*. Otherwise, don't forget to set to *true* before building and deploying in Hololens, else vuforia image tracking will be turned off initially.
3. Trigger an *OnManipulationEnded* event on the **blue cube** (like grabbing then releasing) to place an anchor to where the stones image is located.
4. Do this for both the Sender and Receiver project.


###### ---

**Reminders:**
###### - In Unity Build Settings, switch platform to UWP. 
###### - Open the GazeReceiver and GazeSender scenes respectively (the scene with Mqtt are for smartphone implementation). 
###### - If there is problem with communication between Sender and Receiver, check firewall settings. Also check if in GazeSender, the ip address of the remote host located in OSCTransmitter is set correctly (default to 127.0.0.1 if both Unity projects run on the same PC).
