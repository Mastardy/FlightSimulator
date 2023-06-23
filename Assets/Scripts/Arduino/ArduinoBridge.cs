using System.Globalization;
using UnityEngine;
using System.IO.Ports;
using System.Threading;

public class ArduinoBridge : Singleton<ArduinoBridge>
{
    private SerialPort serialPort;
    private Thread readThread;

    private Vector2 joystick;
    public Vector2 Joystick => joystick;
    
    [SerializeField] private string portName = "COM3";
    [SerializeField] private int baudRate = 9600;
    
    protected override void Awake()
    {
        base.Awake();
        serialPort = new SerialPort(portName, baudRate);
    }
    
    private void Start()
    {
        serialPort.Open();
        readThread = new Thread(ReadSerialData);
        readThread.Start();
    }

    private async void ReadSerialData()
    {
        string complete = string.Empty;

        while (serialPort.IsOpen)
        {
            int bytesToRead = serialPort.BytesToRead;
            if (bytesToRead > 0)
            {
                byte[] buffer = new byte[bytesToRead];
                int bytesRead = await serialPort.BaseStream.ReadAsync(buffer, 0, buffer.Length);
                string data = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);
                
                complete += data;

                if (complete.Contains('\n'))
                {
                    complete = complete.Split('\n')[^2];

                    joystick.x = SafeParse(complete.Split(',')[0]);
                    joystick.y = SafeParse(complete.Split(',')[1]);

                    complete = string.Empty;
                }
            }
        }
    }

    private float SafeParse(string input)
    {
        if (string.IsNullOrEmpty(input)) return 0.0f;

        if(float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
            return result;
        
        return 0.0f;
    }
    
    private void OnApplicationQuit()
    {
        serialPort.Close();
        readThread.Join();
    }
}