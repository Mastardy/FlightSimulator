using UnityEngine;

public class CubeMovement : MonoBehaviour
{
    private void Update()
    {
        if (!ArduinoBridge.IsInitialized) return;
        
        transform.position += ArduinoBridge.Instance.Joystick.x * Time.deltaTime * Vector3.right;
        transform.position += ArduinoBridge.Instance.Joystick.y * Time.deltaTime * Vector3.up;
    }
}
