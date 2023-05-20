#include <Arduino.h>
#include <WifiNINA.h>
#include <ArduinoMqttClient.h>
#include "arduino_secrets.h"
#include <Servo.h>
#include <DHT.h>
#include <Adafruit_SSD1306.h>
#include <Adafruit_GFX.h>

#define dht_type DHT11
#define SCREEN_WIDTH 128
#define SCREEN_HEIGHT 64
#define OLED_RESET -1
Adafruit_SSD1306 display (SCREEN_WIDTH, SCREEN_HEIGHT, &Wire, OLED_RESET);

//  Prototypes
void onMessage(int messageSize);
void led(String payload);
void sub(String payload);
void unsSub(String payload);
void servoControl(String payload);
void publishHumidity (String topic);
void publishTemperature (String topic);
void publishClimate (String topic);
void writeToOLED (String payload);

//  Networking
WiFiSSLClient wifiClient;
MqttClient mqttClient(wifiClient);

//  WiFi
char ssid[] = SECRET_SSID;
char pass[] = SECRET_PASS;

// pins
const int led_pin = 9;
const int servo_pin = 2;
const int dht_pin = 1;

//  MQTT
const char username[] = SECRET_USERNAME;
const char token[] = SECRET_TOKEN;
const char broker[] = SECRET_BROKER;
const int port = 8883;
const char clientId[] = "Oiski_1010";
const String topics[] = {"home/led", "home/window", "controller/sub", "controller/unsub"};
const String publicTopics[] {"home/hum", "home/temp", "home/climate"};
bool retain = true;

//  Peripherals
Servo servo;
DHT dhtSensor(dht_pin, dht_type);

//  Time control
const long interval = 1000;
unsigned long previousMillis = 0;

void setup()
{
  pinMode(led_pin, OUTPUT);
  servo.attach (servo_pin);
  dhtSensor.begin();
  Serial.begin (9600);
  while (!Serial)
  {
    ; //  Wait for serial to connect
  }

  if (!display.begin (SSD1306_SWITCHCAPVCC, 0x3C))
  {
    Serial.println ("OLED not working...");
  }
  else
  {
    Serial.println("OLED Running");
  }
  
  display.clearDisplay();
  display.setTextSize (2);
  display.setTextColor(WHITE);
  display.setCursor (30, 16);
  display.println ("(^__^)");
  display.display();

  Serial.print ("Connecting to Wifi: ");
  Serial.println (ssid);
  while (WiFi.begin (ssid, pass) != WL_CONNECTED)
  {
    Serial.println ("(^__^)");
    delay (5000);
  }

  Serial.println("WiFi Connected");
  Serial.println();

  mqttClient.setId(clientId);
  mqttClient.setUsernamePassword(username, token);
  mqttClient.setCleanSession(true);

  Serial.print ("Connecting to MQTT broker: ");
  Serial.println (broker);

  String humPayload = "Humidity publisher is MIA";
  mqttClient.beginWill (publicTopics[0], humPayload.length(), retain, 2);
  mqttClient.println (humPayload);
  mqttClient.endWill ();

  String tempPayload = "Temperature publisher is MIA";
  mqttClient.beginWill (publicTopics[1], tempPayload.length(), retain, 2);
  mqttClient.println (tempPayload);
  mqttClient.endWill ();

  if (!mqttClient.connect (broker, port))
  {
    Serial.print ("Something went wrong. Error Code: ");
    Serial.println (mqttClient.connectError());

    while (1);
  }

  Serial.println ("Connected to MQTT Broker");
  Serial.println();
  mqttClient.onMessage (onMessage);

  unsigned int topicsLenght = sizeof(topics)/sizeof(topics[0]);
  Serial.println ("Subscribing to:");
  for (unsigned int i = 0; i < topicsLenght; i++)
  {
    Serial.print (topics[i]);
    Serial.print("----------");
    if (mqttClient.subscribe (topics[i], 1))
    {
      Serial.println ("OK");
    }
    else
    {
      Serial.println ("!!");
    }
  }

  Serial.println ("Subcribbed to all provided topics");

  //  Retain and will message
  unsigned int publishTopicsLenght = sizeof(publicTopics)/sizeof(publicTopics[0]);
  for (unsigned int i = 0; i < publishTopicsLenght; i++)
  {
    Serial.print("Retain for: ");
    Serial.println(publicTopics[i]);
    mqttClient.beginMessage (publicTopics[i], retain);
    mqttClient.print ("You're now subscribbed to: ");
    mqttClient.println (publicTopics[i]);
    mqttClient.endMessage();
  }
  Serial.println ("Retain message for topics set");
  //retain = false;

  Serial.println ("Setup done");
}

void loop()
{
  mqttClient.poll();  //  Heartbeat

  unsigned long currentMillis = millis();

  if (currentMillis - previousMillis >= interval)
  {
    previousMillis = currentMillis;

    // publishHumidity (publicTopics[0]);
    // publishTemperature (publicTopics[1]);
    publishClimate (publicTopics[2]);
  }
}

void onMessage (int messageSize)
{
  String topic = mqttClient.messageTopic();

  Serial.print ("Message Recieved for topic: ");
  Serial.println (topic);

  Serial.print ("Payload: ");
  String payload = mqttClient.readString ();
  Serial.println (payload);

  if (topic == "home/led")
  {
    led(payload);
  }

  if (topic == "controller/sub")
  {
    sub (payload);
  }

  if (topic == "controller/unsub")
  {
    unsSub (payload);
  }

  if (topic == "home/window")
  {
    servoControl(payload);
  }

  // if ()
  // {
  //   writeToOLED(payload);
  // }
}

void led(String payload)
{
  int status = payload.toInt();
  digitalWrite (led_pin, status);
  Serial.print ("Setting LED status: ");
  Serial.println (payload);
}

void sub(String payload)
{
    mqttClient.subscribe (payload);
    Serial.print ("Subscribing to: ");
    Serial.println (payload);
}

void unsSub(String payload)
{
    mqttClient.unsubscribe (payload);
    Serial.print ("Unsubscribing from: ");
    Serial.println (payload);
}

void servoControl(String payload)
{
    int state = payload.toInt();
    servo.write(state);
    Serial.print ("Window status: ");
    Serial.println (payload);
}

void publishHumidity (String topic)
{
  float hum = dhtSensor.readHumidity();

  mqttClient.beginMessage (topic, retain);
  mqttClient.print (hum);
  mqttClient.endMessage();
}

void publishTemperature (String topic)
{
  float temp = dhtSensor.readTemperature();

  mqttClient.beginMessage (topic, retain);
  mqttClient.print (temp);
  mqttClient.endMessage();
}

void publishClimate (String topic)
{
  float temp = dhtSensor.readTemperature();
  float hum = dhtSensor.readHumidity();

  mqttClient.beginMessage (topic, retain);
  mqttClient.print ("{");
  mqttClient.print ("\"Temperature\":");
  mqttClient.print ("\"");
  mqttClient.print (temp);
  mqttClient.print ("\"");
  mqttClient.print (",");
  mqttClient.print ("\"Humidity\":");
  mqttClient.print ("\"");
  mqttClient.print (hum);
  mqttClient.print ("\"");
  mqttClient.print ("}");
  mqttClient.endMessage ();
}