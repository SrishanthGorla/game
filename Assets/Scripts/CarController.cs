using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Car Settings")]
    public float maxSpeed = 40f;         
    public float acceleration = 20f;     
    public float turnSpeed = 60f;        
    public float brakeForce = 30f;       
    public float deceleration = 10f;     

    [Header("Nitro Settings")]
    public float nitroMultiplier = 1.6f; 
    public float nitroFuel = 100f;       
    public float nitroDrainRate = 30f;   
    public float nitroRechargeRate = 12f;
    [HideInInspector] public bool isBoosting = false;

    [Header("Audio Settings")]
    public AudioSource engineAudio;
    public float minPitch = 0.8f;        
    public float maxPitch = 2.2f;        

    [Header("Wheel Models")]
    public Transform wheelFL;
    public Transform wheelFR;
    public Transform wheelRL;
    public Transform wheelRR;
    public float maxSteerAngle = 30f;

    [Header("Respawn Settings")]
    public float fallThreshold = -5f; // Triggers respawn when falling below this height
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    private Rigidbody rb;

    private float currentSpeed = 0f;
    private float currentSteerAngle = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (engineAudio == null)
        {
            engineAudio = GetComponent<AudioSource>();
        }

        // Memorize start location on track
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
    }

    void Update()
    {
        // 1. Manual Respawn with 'R' key OR Auto-Respawn when falling into the void
        if (Input.GetKeyDown(KeyCode.R) || transform.position.y < fallThreshold)
        {
            Respawn();
            return;
        }

        float moveInput = Input.GetAxis("Vertical");   
        float turnInput = Input.GetAxis("Horizontal"); 
        bool isBraking = Input.GetKey(KeyCode.Space);  
        bool wantsNitro = Input.GetKey(KeyCode.LeftShift);

        // Nitro Logic
        if (wantsNitro && moveInput > 0 && nitroFuel > 0f)
        {
            isBoosting = true;
            nitroFuel -= nitroDrainRate * Time.deltaTime;
            nitroFuel = Mathf.Clamp(nitroFuel, 0f, 100f);
        }
        else
        {
            isBoosting = false;
            if (!wantsNitro && nitroFuel < 100f)
            {
                nitroFuel += nitroRechargeRate * Time.deltaTime;
                nitroFuel = Mathf.Clamp(nitroFuel, 0f, 100f);
            }
        }

        float targetMaxSpeed = isBoosting ? (maxSpeed * nitroMultiplier) : maxSpeed;
        float currentAccel = isBoosting ? (acceleration * 2.2f) : acceleration;

        // Acceleration and Coasting
        if (moveInput > 0)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetMaxSpeed, currentAccel * Time.deltaTime);
        }
        else if (moveInput < 0)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, -maxSpeed * 0.4f, acceleration * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.deltaTime);
        }

        // Brake
        if (isBraking)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, brakeForce * Time.deltaTime);
        }

        // Move Car
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

        // Turn Car Body
        if (Mathf.Abs(currentSpeed) > 0.1f)
        {
            float direction = currentSpeed > 0 ? 1f : -1f;
            transform.Rotate(Vector3.up * turnInput * turnSpeed * Time.deltaTime * direction);
        }

        // Engine Pitch
        if (engineAudio != null)
        {
            float speedRatio = Mathf.Abs(currentSpeed) / maxSpeed;
            engineAudio.pitch = Mathf.Lerp(minPitch, maxPitch, speedRatio);
        }

        // Animate Wheels
        AnimateWheels(turnInput);
    }

    void AnimateWheels(float turnInput)
    {
        float rollSpeed = currentSpeed * 360f * Time.deltaTime;
        if (wheelFL) wheelFL.Rotate(Vector3.right, rollSpeed, Space.Self);
        if (wheelFR) wheelFR.Rotate(Vector3.right, rollSpeed, Space.Self);
        if (wheelRL) wheelRL.Rotate(Vector3.right, rollSpeed, Space.Self);
        if (wheelRR) wheelRR.Rotate(Vector3.right, rollSpeed, Space.Self);

        currentSteerAngle = Mathf.Lerp(currentSteerAngle, turnInput * maxSteerAngle, 10f * Time.deltaTime);

        if (wheelFL)
        {
            Vector3 localRot = wheelFL.localEulerAngles;
            localRot.y = currentSteerAngle;
            wheelFL.localEulerAngles = localRot;
        }

        if (wheelFR)
        {
            Vector3 localRot = wheelFR.localEulerAngles;
            localRot.y = 180f + currentSteerAngle;
            wheelFR.localEulerAngles = localRot;
        }
    }

    public void Respawn()
    {
        currentSpeed = 0f;
        transform.position = spawnPosition;
        transform.rotation = spawnRotation;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public float GetCurrentSpeedKmh()
    {
        return Mathf.Abs(currentSpeed) * 3.6f;
    }
}       