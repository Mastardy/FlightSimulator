void setup()
{
  Serial.begin(9600);
  pinMode(2, INPUT_PULLUP);
}

void loop()
{
  float x = (analogRead(A0) / 1024.0f) * 2 - 1;
  float y = (analogRead(A1) / 1024.0f) * -2 + 1;
  float z = (analogRead(A2) / 1024.0f) * 2 - 1;
  int b = digitalRead(2);

  if(abs(x) < 0.05f) x = 0.0f;
  if(abs(y) < 0.05f) y = 0.0f;
  if(abs(z) < 0.05f) z = 0.0f;

  String message = String(x, 1) + "," + String(y, 1) + "," + String(z, 1) + "," + b + "\n";

  Serial.print(message);
  Serial.flush();

  delay(20);
}