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

*28/05/2023* - Introduced Blazor Server Application to act as another client.
There's no inherent difference in flow between the mobile **MAUI Client** and the **Blazor Server** application,
as shown on the [architecture](#thop-architecture) diagram below.

*05/06/2023* - The final grade recieved for this project is **12 (A+)**. The Grade is calculated from 3 sub-grades, which are as follows:
- **Embedded Controller (_IoT_) III:** 12 (A+)
- **App Programming III:** 12 (A+)
- **Serverside Programming III:** 12 (A+)

---

## Table of Contents
- [Projects](#projects)
- [Setup](#setup)
    - [MAUI App](#maui-android-setup)
    - [API](#api-setup)
    - [Blazor Server](#blazor-server-setup)
        - [Remove Auth0](#remove-auth0)
- [THOP Architecture](#thop-architecture)
- [THOP Sensor Circuit](#thop-sensor-circuit)
- [Endpoints](#endpoints)
- [Topics](#topics)
- [Features](#requirements)
    - [Optional Features](#optional-requirements)
- [Standards](#standards)
- [Change Log](#change-log)

---

## Projects
_All projects are prefixed with `Oiski.School.THOP`_

| Project | Platform | Languages | Timeframe | Backend Store |
|---------|----------|----------|-----------|---------------|
| `App` | <img style="vertical-align: bottom;" src="/Diagrams/Images/MAUI_Client.png" width="25px" /> NET MAUI | C#, XAML | _22.05.23_ - _24.05.23_ | `Api` |
| `Api` | <img style="vertical-align: bottom;" src="/Diagrams/Images/RestApi_Client.png" width="25px" /> .NET RESTApi | C# | | InfluxDB |
| `Humidex` | <img style="vertical-align: bottom;" src="/Diagrams/Images/MKR1010_Client.png" width="25px" /> Arduino MKR WiFi 1010 | C++ | | `Api` |
| `Web` | <img style="vertical-align: bottom;" src="/Diagrams/Images/MAUI_Client.png" width="25px" /> .NET Blazor Server | C#, JS, HTML, CSS | | `Api` |

---

## Setup

### MAUI Android Setup
The mobile application requires a **TUNNEL_URL** in [AppConstants](/Oiski.School.THOP.Services/AppConstants.cs) that
targets the `API`. If the tunnel is not present the app will not be able to connect to the `API`.
See [this](https://learn.microsoft.com/da-dk/aspnet/core/test/dev-tunnels?view=aspnetcore-7.0) article for information
on how to establish and use **Dev Tunnels**.

### API Setup
The `API` must run through a [Dev Tunnel](https://learn.microsoft.com/da-dk/aspnet/core/test/dev-tunnels?view=aspnetcore-7.0) as the `MAUI` Android application requires prober certification for it
to be able to connect. This is a limitation from Android (_See [Connect to local web services](https://learn.microsoft.com/en-us/dotnet/maui/data-cloud/local-web-services) for more information_).

#### User Secrets
The following **User Secrets** are required for the API to function as intended:
```json
{
  "HiveMQ": {
    "Broker": "<Broker_Url>",
    "Credentials": {
      "Username": "<Client_Username>",
      "Password": "<Client_Password>"
    }
  },
  "Influx": {
    "BucketName": "<Bucket_Name>",
    "OrgId": "<Organization_Id>",
    "Url": "<Bucket_Url>",
    "Token": "<Bucket_Token>"
  }
}
```

Addtionally the API requires the following [App Settings](/Oiski.School.THOP.Api/appsettings.json):
```json
{
  "Subs": [
    {
      "Topic": "+/+/climate",
      "QoS": "1"
    }
  ],
  "Pubs": {
    "HomeWindow": {
      "Topic": "home/window",
      "QoS": "1"
    }
  }
}
```

#### Blazor Server Setup
The Web application uses **Auth0** to secure (_Although only to demonstrate the flow_) and requires the follwing
**User Secrets** to function as intended:
```json
{
  "Securiy": {
    "Domain": "<Auth0_Domain>",
    "ClientId": "<Auth0_Client_Secret>"
  }
}
```

For more information about how to set up the **Auth0** credentials, see [Creating the Auth0 application](https://auth0.com/blog/what-is-blazor-tutorial-on-building-webapp-with-authentication/#Securing-the-Application-with-Auth0).

Addtionally the web client requires a **TUNNEL_URL** in [AppConstants](/Oiski.School.THOP.Services/AppConstants.cs) that
targets the `API`. If the tunnel is not present the client will not be able to connect to the `API`.
See [this](https://learn.microsoft.com/da-dk/aspnet/core/test/dev-tunnels?view=aspnetcore-7.0) article for information
on how to establish and use **Dev Tunnels**.
The `Blazor Server` application itself should not run through a **Dev Tunnel**, especially if **Auth0** security is
used, as **Dev Tunnels** are not supported by **Auth0**.

#### Remove Auth0
To run the web application without **Auth0** do the following:
1. Go to [Program](/Oiski.School.THOP.Web/Program.cs) and remove line 1: `#define USE_AUTH0`
1. Go to [App](/Oiski.School.THOP.Web/App.razor) and comment out this snippet
    ```html
     <AuthorizeRouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)">
        <Authorizing>
            <p>Determining session state, please wait...</p>
        </Authorizing>
        <NotAuthorized>
            <h1>Sorry</h1>
            <p>You're not authorized to reach this page. You need to log in.</p>
        </NotAuthorized>
    </AuthorizeRouteView>
    ```
    and replace it with this:
    ```html
    <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
    ```
1. Go to [MainLayout](/Oiski.School.THOP.Web/Shared/MainLayout.razor) and remove line 7-8 and 12-13.
   Also remove line 17: `<AccessControl />`
1. Go to [Overview](/Oiski.School.THOP.Web/Pages/OverView.razor) and remove line 3: `@attribute [Authorize]`
1. Go to [Analytics](/Oiski.School.THOP.Web/Pages/Analytics.razor) and remove line 2: `@attribute [Authorize]`

This should remove the **Auth0** security, as there's not much security involved. The main reason for the presence of
**Auth0** was to demonstrate the use of an OIDC flow through the `Blazor Server` application.
This can hardly be called security.

**TL:DR:** If you're super lazy just go to [v1.1.1-Rel](https://github.com/Mike-Mortensen-Portfolio/Oiski.School.THOP/releases/tag/v1.1.1-Rel),
as security has been removed on that tag. Running the web application without Auth0 also removes the need for
the application to **not** run through a **Dev Tunnel**, which means the application can, altough it doesn't have to,
run through a **Dev Tunnel**.

**Keep in mind** that the tag is not maintained and could be several commits behind and
therefore not include potential new features or bug fixes.

<p align="right"><strong><a href="#introduction">^ To Top ^</a></strong></p>

---

## THOP Architecture
![Architecture_Diagram](/Diagrams/Architecture_Diagram.drawio.png)

**The diagram** above demonstrates the communication between the different sub-systems in **THOP** system.
If we read the diagram from the bottom and begin with the `Arduino MKR WiFi 1010` board,
we can see that MQTT messages are sent to the `HiveMQ` MQTT broker.
From there each message is relayed to the `MQTTNET Client`,
which serves as the mediater between the API and the broker,
and is embedded in the `.NET Minimal REST API`. In that sense the `MQTTNET Client`
is a sub-part of the API itself.

When the `.NET Minimal REST API` recives humidex data through the `MQTTNET Client`
the data is pushed to an `InfluxDB` through a seperate pipeline.
This means that no API endpoint is used to act on the data-stream, published by the physical
sensor.

The `.NET Minimal REST API` exposes simple endpoints, which can be interacted with by any outsider,
which, in this case, is a mobile `.NET MAUI` application.
More specifically the API exposes a `GET` endpoint for communicating with `InfluxDB`,
as demonstrated on the diagram above. It also exposes `POST` endpoints for controlling peripheral components
on the `Arduino MKR WiFi 1010`.

*28/05/2023* - The `Blazor Server Application` acts as a client and incorperates a desktop solution
for the **THOP** ecosystem. It supports the exact same dataset as the mobile **MAUI Client**,
which means that the two clients are identical to the extend of data manipulation and display.

<p align="right"><strong><a href="#introduction">^ To Top ^</a></strong></p>

---

## THOP Sensor Circuit
![Circuit_Diagram](/Diagrams/Circuit_Diagram.drawio.png)

**The diagram** above demonstrates how the `Arduino MKR WiFi 1010`
is set up. The circuit includes a **servo** motor that should simulate opening and closing ventilation,
as well as a **DHT11** sensor, which is the peripheral used to collect Humidity and Temperature data.
The API and board also supports control of an LED, however, this is not shown on the circuit,
as the peripheral is not necessary or important in order for the system to serve its purpose.
The endpoints and functionality, however, are implemented in both the API and on the board,
as well as in the mobile application.

<p align="right"><strong><a href="#introduction">^ To Top ^</a></strong></p>

---

## Endpoints
<table>
    <thead>
        <tr>
            <th>Endpoint</th>
            <th>Type</th>
            <th>Request Body</th>
            <th>Response Body</th>
            <th>Description</th>
        </tr>
    </thead>
<tbody>
<tr>
<td>
    thop/ventilation
</td>
<td>
    <code>POST</code>
</td>
<td>

```json
{
    "locationId": "home",
    "deviceId":"ArduinoMKR1010",
    "vents": true
}
```

<td><!--Empty--></td>
<td>
    Turn on the vents controlled by &lt;<i>deviceId</i>&gt; at &lt;<i>locationId</i>&gt;
</td>
</tr>
<tr>
<td>
    thop/humidex
</td>
<td>
    <code>GET</code>
</td>

<td><!--Empty--></td>

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
<tr>
<td>
    thop/light
</td>
<td>
    <code>POST</code>
</td>

<td><!--Empty--></td>

<td>

```json
{
    "locationId": "home",
    "deviceId":"ArduinoMKR1010",
    "lights": true
}
```

<td>
    Switch the lights controlled by &lt;<i>deviceId</i>&gt; at &lt;<i>locationId</i>&gt; on/off
</td>
</tr>
<tr>
<td>
    thop/killhumidex
</td>
<td>
    <code>POST</code>
</td>

<td><!--Empty--></td>

<td>

```json
true
```

</td>
<td>
    Forces the <code>thop/humidex</code> endpoint to return a 500 Internal Server Error. This endpoint will return <strong>true</strong>
    if the kill switch is active; otherwise, if not, <strong>false</strong>
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
    "on": "on"
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

## Requirements
- - [X] Can display actual reading (_latest readings_) for temperature, humidity and the timestamp for when the reading was recorded
- - [X] Can display a graph of the readings
    - - [X] Should be able to pick between latest time, day or week.
- - [X] A button should be able to simulate a opening a window or ventilation (_via MQTT and a servo peripheral_)
- - [X] Mobile app should be structured using MVVM design priciples and Dependency Injection
- - [X] Should be able to handle unstable internet connection

## Optional Requirements
- - [X] Can display latest data if disconnected
- - [X] The ability to switch between telemetry context. Ex. from different parts of a house.
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
    - `Branches` must be all lowercase, seperating words by dashes and named as follows: *[MajorVersion]/[YouInitials]/[FeatureName]*.
      > Example: v0/msm/example-branch.
- **Code**
  - `Namespaces` must be constructed as follows: _Oiski.School.[ProjectName].[FolderName]_.
  - `Fields` must be _private_ or _protected_.
  - `Properties` must be _public_, _protected_ or _internal_.
  - `Interfaces` must have their own subfolder, which should never be included in their `namespace`.

  <p align="right"><strong><a href="#introduction">^ To Top ^</a></strong></p>

---

## Change Log
- **[v1.1.2-Dev](https://github.com/Mike-Mortensen-Portfolio/Oiski.School.THOP/releases/tag/v1.1.2-Dev)**
    - **Added**
        - New App Icon
- **[v1.1.1-Dev](https://github.com/Mike-Mortensen-Portfolio/Oiski.School.THOP/releases/tag/v1.1.1-Dev)**
    - **Added**
        - Auth0 authentication
        - Refactoring and code cleanup
- **[v1.1.0-Dev](https://github.com/Mike-Mortensen-Portfolio/Oiski.School.THOP/releases/tag/v1.1.0-Dev)**
    - **Added**
        - Blazor server project as web solution (_`Oiski.School.THOP.Web`_)
        - Added control view (_/Overview_)
        - Added Graph view (_/Analytics_)
        - Added Peripheral control
- **[v1.0.1-Dev](https://github.com/Mike-Mortensen-Portfolio/Oiski.School.THOP/releases/tag/v1.0.1-Dev)**
    - **Added**
        - Moved all common code into a service project (_`Oiski.School.THOP.Services`_)
        - Added more documentation to the codebase
 - **[v1.0.0-Dev](https://github.com/Mike-Mortensen-Portfolio/Oiski.School.THOP/releases/tag/v1.0.0-Dev)**
   - **Added**
     - API with basic endpoints for controlling peripheral components and query telemetry data
         - MQTTNET Client as a mediater between the HiveMQ broker and the .NET API
     - Android Application to interact with through the API

<p align="right"><strong><a href="#introduction">^ To Top ^</a></strong></p>