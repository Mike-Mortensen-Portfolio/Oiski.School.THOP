# Introduction
This project is written for my 5th year _App Programming III_ course examn.
It's composed of 3 parts; A mobile application, .NET RestApi and an embedded system.
The idea is to have the embedded system read telemetry data and push it to the RestApi
through the **MQTT protocol**, where the Api will store it in an InfluxDB.
The Mobile application would then pull this data and display it using a graph.

## Projects
_All projects are prefixed with `Oiski.School.Telemetry`_

| Project | Platform | Language | Timeframe | Backend Store |
|---------|----------|----------|-----------|---------------|
| `App` | .NET MAUI | C# | _17.05.23_ - _24.05.23_ | `Api` |
| `Api` | .NET RESTApi | C# | | InfluxDB |
| `HiveMQ` | Arduino MKR WiFi 1010 | C++ | | `Api` |


## Endpoints
| Endpoint | Type | Description |
|----------|------|-------------|

## Topics
| Topic | Input | Description | Sample |
|-------|--------|------------|--------|
| home/led | `On`/`Off` | Turn the home led on or off | `{ "led": "on" }` |
| home/servo | `Open`/`Close` | Open or close the home window | `{ "window": "Open" }` |

## Structure
![](imageLink)

## Initial Features
- - [ ] Can display actual reading (_latest readings_) for temperature, humidity and the timestamp for when the reading was recorded
- - [ ] Can display a graph of the readings
    - - [ ] Should be able to pick between latest tim, day week.
- - [ ] A button should be able to simulate a opening a window or ventilation (_via MQTT and a servo peripheral_)
- - [ ] Mobile app should be structured using MVVM design priciples and Dependency Injection
- - [ ] Can display latest data if disconnected
- - [ ] Should be able to handle unstable internet connection

## Optional Features
- - [ ] The ability to switch between telemetry context. Ex. from different parts of a house.
- - [ ] The ability to register an alarm that will push a notification if the temperature exceeds a given limit

## Class Diagrams
<details><summary>Click to show image(s)</summary>
![image](**Insert-Image**)
</details>

# Standards
- **Versioning**
  - Version Template: _[_Major_].[_Minor_].[_Patch_]-[StateMod]_.
  - `Major:`
    - Major Changes
    - Changes to Core structure (_Like an UI Switch_)
  - `Minor:`
    - 100 _Minor_ versions = 1 _Major_ version
    - Features
    - Large Code Refactoring (_Ex. If you create a new file when refactoring_)
  - `Patch:`
    - 100 _Patch_ versions = 1 _Minor_ version
    - Hotfixs
    - Revisions
    - Minor Code Refactoring
  - `StateMod:`
    - Can either be _Dev_ or _Rel_ and defines the state of the version. _Dev_ meaning the version is on the Development branch, while _Rel_ means it's on the Release branch and therefore published.
      > Example: v0.10.3-Dev
  - When a _Major_ version is applied it resets the version count, same goes for a _Minor_ version.
      > Example: v0.55.15-Rel -> v1.0.0-Rel | v0.99.56-Dev -> v0.99.0-Dev
- **Source Control**
    - `Features` must be branched out and developed on an isolated branch and merged back into the `Developer` branch when done.
    - `Branches` must be named as follows: *[MajorVersion]/[YouInitials]/[FeatureName]*.
      > Example: v0/MSM/ExampleBranch.
- **Code**
  - `Namespaces` must be constructed as follows: _Oiski.[ProjectName].[FolderName]_.
  - `Fields` must be _private_ or _protected_.
  - `Properties` must be _public_, _protected_ or _internal_.
  - `Interfaces` must have their own subfolder, which should never be included in their `namespace`.

## Change Log
 - **[v0.0.0](LinkToGitHubTag)**
   - **Added**
     - _List the features added with this version_
   - **Modified**
     - _List the areas that were altered in any way in this version_
   - **Fixed**
     - _List the bugs that were fixed in this version_