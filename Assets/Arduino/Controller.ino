void setup()
{
  Serial.begin(9600);
}

void loop()
{
  float x = (analogRead(A0) / 1024.0f) * 2 - 1;
  float y = (analogRead(A1) / 1024.0f) * -2 + 1;

  if(abs(x) < 0.05f) x = 0.0f;
  if(abs(y) < 0.05f) y = 0.0f;

  String message = String(x, 1) + "," + String(y, 1) + "\n";

  Serial.print(message);
  Serial.flush();

  delay(20);
}