using UnityEngine;

public class FreeCamera : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float panSpeed = 20f;          // Скорость перемещения (pan)
    [SerializeField] private float orbitSpeed = 5f;         // Скорость вращения (orbit)
    [SerializeField] private float zoomSpeed = 30f;         // Скорость приближения (zoom)
    [SerializeField] private float shiftMultiplier = 2f;    // Множитель скорости при зажатом Shift

    private float yaw = 0f;
    private float pitch = 0f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
    }

    void Update()
    {
        if (gameObject.activeSelf == false)
            return;
        float speedFactor = Input.GetKey(KeyCode.LeftShift) ? shiftMultiplier : 1f;

        if (Input.GetMouseButton(2))
        {
            float deltaX = Input.GetAxis("Mouse X");
            float deltaY = Input.GetAxis("Mouse Y");

            Vector3 right = transform.right;
            Vector3 up = transform.up;

            Vector3 panDelta = (right * -deltaX + up * -deltaY) * panSpeed * speedFactor * Time.unscaledDeltaTime;
            transform.position += panDelta;
        }

        if (Input.GetMouseButton(1))
        {
            float deltaX = Input.GetAxis("Mouse X");
            float deltaY = Input.GetAxis("Mouse Y");

            yaw += deltaX * orbitSpeed;
            pitch -= deltaY * orbitSpeed; 

            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            Vector3 zoomDelta = transform.forward * scroll * zoomSpeed * speedFactor * Time.unscaledDeltaTime;
            transform.position += zoomDelta;
        }
    }
}