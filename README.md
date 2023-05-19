<div align="center">
    <img src="/Diagrams/Images/pngwing.com.png" alt="Header_Image" width="300px" height="300px"/>
</div>
<div align="center">
    <strong style="font-size:30px; display:block;">THOP</strong>
    <p style="display:block; font-weight:normal; font-size:20px;">Temperature and Humidity Observation Program </p>
</div>

## Introduction
The <span style="color:#8fc7ae">Temperature and Humidity Observation Program</span> or <span style="color:#8fc7ae">THOP</span> for short, is written for my 5th year _App Programming III_ course examn.
It's composed of 3 parts; A mobile application, .NET RestApi and an embedded system.
The idea is to have the embedded system read humidex data from a sensor and push it to the RestApi
through the **MQTT protocol**. The Api will then store it in an InfluxDB.
The Mobile application will pull this data and display it using a graph (_See [architecture](#thop-architecture)_).

---

## Projects
_All projects are prefixed with `Oiski.School.THOP`_

| Project | Platform | Language | Timeframe | Backend Store |
|---------|----------|----------|-----------|---------------|
| `App` | <img style="vertical-align: bottom;" src="/Diagrams/Images/MAUI_Client.png" width="25px" /> NET MAUI | C# | _17.05.23_ - _24.05.23_ | `Api` |
| `Api` | <img style="vertical-align: bottom;" src="/Diagrams/Images/RestApi_Client.png" width="25px" /> .NET RESTApi | C# | | InfluxDB |
| `Humidex` | <img style="vertical-align: bottom;" src="/Diagrams/Images/MKR1010_Client.png" width="25px" /> Arduino MKR WiFi 1010 | C++ | | `Api` |

---

## THOP Architecture
![Architecture_Diagram](/Diagrams/Architecture_Diagram.drawio.png)

<p align="right"><strong><a href="#introduction">^ To Top ^</a></strong></p>

---

## THOP Sensor Circuit
![Circuit_Diagram](/Diagrams/Circuit_Diagram.drawio.png)

<p align="right"><strong><a href="#introduction">^ To Top ^</a></strong></p>

---

## Endpoints
<table>
    <thead>
        <tr>
            <th>Endpoint</th>
            <th>Type</th>
            <th>Sample</th>
            <th>Description</th>
        </tr>
    </thead>
<tbody>
<tr>
<td>
    telemetry/ventilation
</td>
<td>
    <code>POST</code>
</td>
<td>

```json
{
    "locationId": "home",
    "deviceId":"ArduinoMKR1010",
    "vents":"on"
}
```

<td>
    Turn on the vents controlled by &lt;<i>deviceId</i>&gt; at &lt;<i>locationId</i>&gt;
</td>
</tr>
<tr>
<td>
    telemetry/climate
</td>
<td>
    <code>GET</code>
</td>
<td>

```json
{
    "sensor":"DHT11",
    "locationId":"home",
    "startTime": "2012-04-23T18:25:43.511Z",
    "endTime": "2012-04-23T18:25:43.511Z"
}
```

<td>
    Pull telemetry data from Influx based on the query filter
</td>
</tr>
</tbody>
</table>

<p align="right"><strong><a href="#introduction">^ To Top ^</a></strong></p>

---

## Topics
<table>
    <thead>
        <tr>
            <th>Topic</th>
            <th>Sample</th>
            <th>Description</th>
        </tr>
    </thead>
<tbody>
<tr>
<td>
    &lt;<i>locationId</i>&gt;/&lt;<i>deviceId</i>&gt;
</td>
<td>

```json
{
    "locationId": "home",
    "deviceId":"ArduinoMKR1010",
    "vents":"on"
}
```

<td>
    Publish to this topic to control peripherals controlled by &lt;<i>deviceId</i>&gt;
</tr>
<tr>
<td>
    &lt;<i>locationId</i>&gt;/&lt;<i>deviceId</i>&gt;/climate
</td>
<td>

```json
{
    "locationId": "home",
    "sensor": "DHT11",
    "temperature": 22.5,
    "humidity": 10.2
}
```

</td>
<td>
    The stream of Humidity and Temperature data for the &lt;<i>deviceId</i>&gt; at &lt;<i>locationId</i>&gt;
</td>
</tr>
</tbody>
</table>

<p align="right"><strong><a href="#introduction">^ To Top ^</a></strong></p>

---

## Initial Features
- - [ ] Can display actual reading (_latest readings_) for temperature, humidity and the timestamp for when the reading was recorded
- - [ ] Can display a graph of the readings
    - - [ ] Should be able to pick between latest time, day or week.
- - [ ] A button should be able to simulate a opening a window or ventilation (_via MQTT and a servo peripheral_)
- - [ ] Mobile app should be structured using MVVM design priciples and Dependency Injection
- - [ ] Can display latest data if disconnected
- - [ ] Should be able to handle unstable internet connection

## Optional Features
- - [ ] The ability to switch between telemetry context. Ex. from different parts of a house.
- - [ ] The ability to register an alarm that will push a notification if the temperature exceeds a given limit

<p align="right"><strong><a href="#introduction">^ To Top ^</a></strong></p>

---

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

  <p align="right"><strong><a href="#introduction">^ To Top ^</a></strong></p>

---

## Change Log
 - **[v0.0.0](LinkToGitHubTag)**
   - **Added**
     - _List the features added with this version_
   - **Modified**
     - _List the areas that were altered in any way in this version_
   - **Fixed**
     - _List the bugs that were fixed in this version_

<p align="right"><strong><a href="#introduction">^ To Top ^</a></strong></p>