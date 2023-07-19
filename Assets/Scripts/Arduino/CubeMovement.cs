using UnityEngine;

public class CubeMovement : MonoBehaviour
{
    private bool lastButtonState;

    private bool buttonToggle;
    
    private void Update()
    {
        if (!ArduinoBridge.IsInitialized) return;
        
        transform.position += ArduinoBridge.Instance.Joystick.x * Time.deltaTime * Vector3.right;
        transform.position += ArduinoBridge.Instance.Joystick.y * Time.deltaTime * Vector3.up;
        transform.position += ArduinoBridge.Instance.Joystick.z * Time.deltaTime * Vector3.forward;
        
        var currentButtonState = ArduinoBridge.Instance.Button;
        
        if (currentButtonState != lastButtonState && currentButtonState)
        {
            buttonToggle = !buttonToggle;
        }

        if (buttonToggle) GetComponent<MeshRenderer>().material.color = Color.black;
        else GetComponent<MeshRenderer>().material.color = Color.white;
        
        lastButtonState = currentButtonState;
    }
}
