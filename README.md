# XR Orbit Visualization Toolkit
## About the project
This application uses the [Microsoft HoloLens 2]("https://www.microsoft.com/en-us/hololens/hardware") to display holographic orbital data in your environment. It provides quick and intuitive 3D visualisation of complex orbits, bringing depth-perception into an othwerwise 2D representation.
## Development

Clone this repo to your private GitHub account. When you've made sufficient changes, apply for a [pull request]("https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/proposing-changes-to-your-work-with-pull-requests/about-pull-requests") to apply your changes to the main project repo.

General Microsoft guidance on getting started with the Mixed Reality Toolkit (MRTK) can be found [here]("https://docs.microsoft.com/en-us/windows/mixed-reality/mrtk-unity/?view=mrtkunity-2021-05").

Tools you will need to continue development:
1. [Unity 2019.4.16f1]("https://unity3d.com/get-unity/download/archive") with:
   1.  UWP Export
2. [Visual Studio 2019 (17.8 or higher)]("https://docs.microsoft.com/en-us/windows/mixed-reality/mrtk-unity/?view=mrtkunity-2021-05") with:
   1. Desktop development with C++
   2. Universal Windows Platform (UWP) development
   3. Game development with Unity
   
   Within the UWP workload, make sure the following components are included for installation:
      1. Windows 10 SDK version 10.0.19041.0
      2. USB Device Connectivity
      3. C++ (v142) Universal Windows Platform tools

After you open the project in Unity, open the *File > Build Settings* window, select *Universal Windows Platform* 