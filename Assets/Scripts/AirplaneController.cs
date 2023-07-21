using UnityEngine;

public class AirplaneController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float minSpeed = 4;
    [SerializeField] private float maxSpeed = 12;

    [SerializeField] private float turnSpeed;
    [SerializeField] private float smoothRollTime;
    [SerializeField] private float rollAngle;
    [SerializeField] private float smoothPitchTime;
    [SerializeField] private float maxPitchAngle;
    [SerializeField] private float turnSmoothTime;
    
    [Header("Camera FOV")]
    [SerializeField] private float fovSlow = 60;
    [SerializeField] private float fovFast = 75;
    [SerializeField] private float fovSmoothTime = 0.25f;

    [Header("Elevation")]
    private float currentElevation;
    [SerializeField] private float minElevation = 10.2f;
    [SerializeField] private float maxElevation = 11;
    
    [Header("References")]
    [SerializeField] private Camera cam;
    [SerializeField] private Transform leftFlap;
    [SerializeField] private Transform rightFlap;
    [SerializeField] private GameObject lights;

    private float smoothedTurnSpeed;
    private float turnSmoothV;
    private float pitchSmoothV;
    private float rollSmoothV;

    private float worldRadius = 10;
    
    private float baseTargetSpeed;
    private float currentSpeed;
    private float smoothFovVelocity;
    
    private float turnInput;
    private float currentPitchAngle;
    private float currentRollAngle;
    private float pitchInput;

    private bool lastButtonState;

    private float SpeedT => (currentSpeed - minSpeed) / (maxSpeed - minSpeed);
    
    private void Start()
    {
        baseTargetSpeed = Mathf.Lerp(minSpeed, maxSpeed, 0.35f);
        currentSpeed = baseTargetSpeed;
    }

    private void Update()
    {
        HandleInput();
        HandleMovement();
    }

    public void UpdateMovementInput(Vector2 moveInput, float speed)
    {
        turnInput = moveInput.x;
        pitchInput = moveInput.y;

        baseTargetSpeed = minSpeed + (maxSpeed - minSpeed) * speed;
    }
    
    private void HandleInput()
    {
        UpdateMovementInput(ArduinoBridge.Instance.Joystick, ArduinoBridge.Instance.Acceleration);
        if(ArduinoBridge.Instance.Button && !lastButtonState)
            lights.SetActive(!lights.activeSelf);
        lastButtonState = ArduinoBridge.Instance.Button;
    }

    private void HandleMovement()
    {
        smoothedTurnSpeed = Mathf.SmoothDamp(smoothedTurnSpeed, turnInput * turnSpeed, ref turnSmoothV, turnSmoothTime);

        currentSpeed = baseTargetSpeed;

        float forwardSpeed = Mathf.Cos(Mathf.Abs(currentPitchAngle) * Mathf.Deg2Rad) * currentSpeed;
        float verticalVelocity = -Mathf.Sin(currentPitchAngle * Mathf.Deg2Rad) * currentSpeed * 100;
        
        currentElevation += verticalVelocity * Time.deltaTime;
        currentElevation = Mathf.Clamp(currentElevation, minElevation, maxElevation);

        UpdatePosition(forwardSpeed);
        UpdateRotation(smoothedTurnSpeed * Time.deltaTime);

        float targetPitch = pitchInput * maxPitchAngle;

        float distanceToPitchLimit = (targetPitch > 0) ? currentElevation - minElevation : maxElevation - currentElevation;
        float pitchLimitSmoothDistance = 3;
        targetPitch *= Mathf.Clamp01(distanceToPitchLimit / pitchLimitSmoothDistance);

        currentPitchAngle = Mathf.SmoothDampAngle(currentPitchAngle, targetPitch, ref pitchSmoothV, smoothPitchTime);

        float targetRoll = turnInput * -rollAngle;
        currentRollAngle = Mathf.SmoothDampAngle(currentRollAngle, targetRoll, ref rollSmoothV, smoothRollTime);
    }

    private void UpdatePosition(float forwardSpeed)
    {
        Vector3 newPos = transform.position + transform.forward * (forwardSpeed * Time.deltaTime);
        newPos = newPos.normalized * (worldRadius + currentElevation);
        transform.position = newPos;
    }

    private void UpdateRotation(float turnAmount)
    {
        Vector3 gravityUp = transform.position.normalized;
        transform.RotateAround(transform.position, gravityUp, turnAmount);
        transform.LookAt((transform.position + transform.forward * 10).normalized * (worldRadius + currentElevation), gravityUp);
        transform.rotation = Quaternion.FromToRotation(transform.up, gravityUp) * transform.rotation;
        transform.RotateAround(transform.position, transform.forward, currentRollAngle * 2.5f);
        transform.RotateAround(transform.position, transform.right, currentPitchAngle * 15f);
        
        leftFlap.localRotation = Quaternion.Euler(currentPitchAngle * 100, 0, 0);
        rightFlap.localRotation = Quaternion.Euler(currentPitchAngle * 100, 0, 0);
    }
    
    private void LateUpdate()
    {
        UpdateView();
        
        cam.fieldOfView = Mathf.SmoothDamp(cam.fieldOfView, CalculateFOV(), ref smoothFovVelocity, fovSmoothTime);
    }

    private void UpdateView()
    {
        
    }

    private float CalculateFOV()
    {
        return Mathf.Lerp(fovSlow, fovFast, SpeedT);
    }
}
