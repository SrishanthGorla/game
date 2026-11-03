using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 3f, -7f);
    public float smoothSpeed = 8f;

    [Header("Dynamic Nitro Zoom")]
    public float normalFOV = 60f;
    public float nitroFOV = 75f;
    public float fovZoomSpeed = 4f;

    private Camera cam;
    private CarController car;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (target != null)
        {
            car = target.GetComponent<CarController>();
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Follow car position
        Vector3 targetPosition = target.TransformPoint(offset);
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

        // Keep eyes on the car
        transform.LookAt(target.position + Vector3.up * 1f);

        // Smoothly zoom camera lens in and out
        if (cam != null && car != null)
        {
            float desiredFOV = car.isBoosting ? nitroFOV : normalFOV;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, desiredFOV, fovZoomSpeed * Time.deltaTime);
        }
    }
}