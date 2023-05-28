#include <Arduino.h>
#include <WifiNINA.h>
#include <ArduinoMqttClient.h>
#include "arduino_secrets.h"
#include <Servo.h>
#include <DHT.h>
#include <ArduinoJson.h>

/////////// Interval //////////
unsigned long lastMillis = 0;
unsigned int intervalInMili = 120000;
unsigned int retryIntervalInMili = 5000;

//  Networking
WiFiSSLClient wifiClient;
MqttClient mqttClient(wifiClient);

////////// WiFi /////////
char ssid[] = SECRET_SSID;
char pass[] = SECRET_PASS;

////////// MQTT //////////
const char username[] = SECRET_USERNAME;
const char token[] = SECRET_TOKEN;
const char broker[] = SECRET_BROKER;
const int port = 8883;
const char locationId[] = "home";
const char clientId[] = "oiski_1010";
const String topics[] = {locationId + String ("/") + clientId};
const String publishTopics[] {locationId + String ("/") + clientId + String ("/climate")};
bool retain = true;

////////// Peripherals //////////
#define dht_type DHT11
const int dht_pin = 1;
DHT dhtSensor(dht_pin, dht_type);
const int led_pin = 9;
const int servo_pin = 2;
Servo servo;

////////// Prototypes //////////
void connectWiFi ();
void connectMQTT ();
void onMessageRecieved (int messageSize);
void publishClimate (String topic);

void setup()
{
  // On board RBG LED
  WiFiDrv::pinMode (25, OUTPUT);  //  Green LED
  WiFiDrv::pinMode (26, OUTPUT);  //  Red LED
  WiFiDrv::pinMode (27, OUTPUT);  //  Blue LED
  dhtSensor.begin();
  pinMode (led_pin, OUTPUT);
  servo.attach (servo_pin);
  Serial.begin (9600);
  // while (!Serial) ; //  Wait for serial to connect

  mqttClient.setId(clientId);
  mqttClient.setUsernamePassword(username, token);
  mqttClient.setCleanSession(true);

   mqttClient.onMessage (onMessageRecieved);

  Serial.println ("Setup done");
}

void loop()
{
  if (WiFi.status () != WL_CONNECTED)
    {
      WiFiDrv::digitalWrite (26, HIGH);   //  Red LED
      connectWiFi();
    }

    if (!mqttClient.connected ())
    {
      WiFiDrv::digitalWrite (26, LOW);  //  Red LED
      WiFiDrv::digitalWrite (27, HIGH); //  Blue LED

      // If MQTT client is disconnected; connect
      connectMQTT ();

      WiFiDrv::digitalWrite (27, LOW);  //  Blue LED
      WiFiDrv::digitalWrite (25, HIGH); //  Green LED
    }

  mqttClient.poll();  //  Heartbeat

  if (millis () - lastMillis > intervalInMili)
  {
    lastMillis = millis ();

    publishClimate(publishTopics[0]);
  }
}

void connectWiFi ()
{
  Serial.print ("Attempting to connect to SSID: ");
  Serial.print (ssid);
  Serial.print (" ");

  while (WiFi.begin (ssid, pass) != WL_CONNECTED)
  {
    Serial.print ("-");
    delay (retryIntervalInMili);
  }

  Serial.println();

  Serial.println ("Connected");
}

void connectMQTT ()
{
  Serial.print ("Attempting to connect to MQTT broker: ");
  Serial.print (broker);
  Serial.print (" ");

  while (!mqttClient.connect (broker, port))
  {
    Serial.print ("-");
    delay (retryIntervalInMili);
  }
  
  Serial.println ();

  Serial.println ("Connected");

  unsigned int topicsLenght = sizeof(topics)/sizeof(topics[0]);
  Serial.println ("Subscribing to:");
  for (unsigned int i = 0; i < topicsLenght; i++)
  {
    Serial.print (topics[i]);
    Serial.print("----------");
    if (mqttClient.subscribe (topics[i], 1))
      Serial.println ("OK");
    else
      Serial.println ("!!");
  }

  Serial.println ("Subcribbed to all provided topics");

  //  Retain and will message
  unsigned int publishTopicsLenght = sizeof(publishTopics)/sizeof(publishTopics[0]);
  for (unsigned int i = 0; i < publishTopicsLenght; i++)
  {
    Serial.print("Retain for: ");
    Serial.println(publishTopics[i]);
    mqttClient.beginMessage (publishTopics[i], retain);
    mqttClient.print ("You're now subscribbed to: ");
    mqttClient.println (publishTopics[i]);
    mqttClient.endMessage();
  }
  Serial.println ("Retain message for topics set");
}

void publishClimate (String topic)
{
  float temp = dhtSensor.readTemperature();
  float hum = dhtSensor.readHumidity();

  StaticJsonDocument<256> doc;
  doc ["locationId"] = locationId;
  doc ["sensor"] = "DHT11";
  doc ["temperature"] = temp;
  doc ["humidity"] = hum;

  mqttClient.beginMessage (topic, (unsigned long) measureJson (doc));
  serializeJson (doc, mqttClient);
  mqttClient.endMessage ();
}

void onMessageRecieved (int messageSize)
{
  //  Recieved message; print topic and contents
  Serial.print ("Message recieved on topic: '");
  Serial.print (mqttClient.messageTopic ());
  Serial.print ("', Size: ");
  Serial.print (messageSize);
  Serial.println (" Bytes:");

  if (!messageSize)
    return;

  String payload = mqttClient.readString();
  payload.toUpperCase();
  Serial.println ("Payload:");
  Serial.println (payload);

  StaticJsonDocument<256> doc;
  deserializeJson (doc, payload);
  Serial.println ("Processing payload");

  if (doc.containsKey ("VENTS"))
  {
    String stateValue = doc["VENTS"];
    int open = stateValue == String ("ON");

    if (open)
      servo.write (180);
    else
      servo.write (0);
  }

  if (doc.containsKey ("LIGHTS"))
  {
    String stateValue = doc["LIGHTS"];
    int on = stateValue == String ("ON");

    if (on)
      digitalWrite (led_pin, HIGH);
    else
      digitalWrite (led_pin, LOW);
  }

  Serial.println ("Processed");
  Serial.println ();
}
