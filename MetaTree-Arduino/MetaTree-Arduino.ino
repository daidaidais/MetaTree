#include <SPI.h>         // needed for Arduino versions later than 0018
#include <Ethernet2.h>
#include <EthernetUdp2.h>         // UDP library from: bjoern@cs.stanford.edu 12/30/2008


// Enter a MAC address and IP address for your controller below.
// The IP address will be dependent on your local network:
byte mac[] = {
  0x90, 0xA2, 0xDA, 0x10, 0xAA, 0x74
};
IPAddress ip(131, 113, 137, 92);
unsigned int localPort = 8888;      // local port to listen on

// buffers for receiving and sending data
char packetBuffer[UDP_TX_PACKET_MAX_SIZE];  //buffer to hold incoming packet,

char * id;
char * value;

// Ethernet Shield uses pins 10, 11, 12, 13 !!
const int led_pin1 = 3;
const int led_pin2 = 5;
const int led_pin3 = 6;
const int led_pin4 = 9;

// An EthernetUDP instance to let us send and receive packets over UDP
EthernetUDP Udp;

//  VARIABLES FOR PULSE SENSOR
int pulsePin = 0;                 // Pulse Sensor purple wire connected to analog pin 0
int blinkPin = 13;                // pin to blink led at each beat
int fadePin = 5;                  // pin to do fancy classy fading blink at each beat
int fadeRate = 0;                 // used to fade LED on with PWM on fadePin

// Volatile Variables, used in the interrupt service routine!
volatile int BPM;                   // int that holds raw Analog in 0. updated every 2mS
volatile int Signal;                // holds the incoming raw data
volatile int IBI = 600;             // int that holds the time interval between beats! Must be seeded! 
volatile boolean Pulse = false;     // "True" when User's live heartbeat is detected. "False" when not a "live beat". 
volatile boolean QS = false;        // becomes true when Arduoino finds a beat.

// Regards Serial OutPut  -- Set This Up to your needs
static boolean serialVisual = false;   // Set to 'false' by Default.  Re-set to 'true' to see Arduino Serial Monitor ASCII Visual Pulse

void setup() {

  Serial.begin(115200);
  
  // start the Ethernet and UDP:
  Serial.println("Begin Ethernet");
  Ethernet.begin(mac, ip);
  Serial.println("Begin localport");
  Udp.begin(localPort);

  //FOR PULSE SENSOR
  interruptSetup();                 // sets up to read Pulse Sensor signal every 2mS   
}

void loop() {


  /****** PULSE ******/

   if (QS == true){     //  A Heartbeat Was Found
                       // BPM and IBI have been Determined
                       // Quantified Self "QS" true when arduino finds a heartbeat
     Serial.print("BPM:");              
     Serial.print(BPM);  // for Unity visualization
     Serial.print("\t");
     Serial.println("");

     float treePulse;
     if(BPM < 60) {treePulse = 0;}
      else if(BPM > 80) {treePulse = 255;}
      else {treePulse = (BPM - 60) * 255 / 20;}
      analogWrite(led_pin1, treePulse);
                
      QS = false;                      // reset the Quantified Self flag for next time 
    }
       

  // if there's UDP data available, read a packet
  int packetSize = Udp.parsePacket();
  if (packetSize) {
    //Serial.print("Received packet of size ");
    //Serial.println(packetSize);
    //Serial.print("From ");
    IPAddress remote = Udp.remoteIP();
    for (int i = 0; i < 4; i++) {
      //Serial.print(remote[i], DEC);
      if (i < 3) {
        //Serial.print(".");
      }
    }
    //Serial.print(", port ");
    //Serial.println(Udp.remotePort());

    // read the packet into packetBufffer
    Udp.read(packetBuffer, UDP_TX_PACKET_MAX_SIZE);
    //Serial.println("Contents:");
    //Serial.println(packetBuffer);

    //split packet with comma
    id = strtok(packetBuffer, ',');
    value = strchr(packetBuffer, ',');
    ++value;
    int idInt = atoi(id);
    float valueFloat = atof(value);

      /****** LEAN Y ******/
    
    if(idInt == 3){
      Serial.print("leanY: ");
      Serial.println(valueFloat);
      analogWrite(led_pin3, valueFloat * 255);
    }

      /****** PUPIL ******/
      
    else if(idInt == 5){
      Serial.print("pupil: ");
      Serial.println(valueFloat);
      float treePupil;
      if(valueFloat < 90) {treePupil = 0;}
      else if(valueFloat > 120) {treePupil = 255;}
      else {treePupil = (valueFloat - 90) * 255 / 40;}
      analogWrite(led_pin2, treePupil);
    }
    
  }

}

