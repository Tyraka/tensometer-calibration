#include <String.h>
#include "HX711.h"  //You must have this library in your arduino library folder
#define DOUT 46
#define CLK 44

HX711 scale(DOUT, CLK);
String message = "";
float calibration_factor = -96650; //-106600 worked for my 40Kg max scale setup

void setup()
{
  Serial.begin(9600);
  scale.set_scale();
  scale.tare(); //Reset the scale to 0
}

void sendScaleMeasurments()
{
  Serial.println(scale.get_units(), 3);
  Serial.flush();
}

void serialEvent()
{
  while (Serial.available())
  {
    if ((Serial.available() == 1) && ((char)Serial.peek() == 'a'))
    {
      Serial.println(scale.get_units(), 3);
      Serial.read();
    }
    else
    {
        String recievedFactor = Serial.readString();
        scale.set_scale(recievedFactor.toInt());
    }  
    }  
}



void loop()
{
}
//=============================================================================================
