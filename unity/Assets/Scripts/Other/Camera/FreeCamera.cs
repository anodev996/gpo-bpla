using UnityEngine;

public class FreeCamera : MonoBehaviour,IMoveTo
{
    [Header("Movement Settings")]
    [SerializeField] private float panSpeed = 20f;
    [SerializeField] private float orbitSpeed = 5f;
    [SerializeField] private float zoomSpeed = 30f;
    [SerializeField] private float shiftMultiplier = 2f;

    private float yaw = 0f;
    private float pitch = 0f;

    // Поля для плавного перемещения
    private bool isMoving = false;
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private float moveDuration;
    private float moveTimer;
    private Vector3 startPosition;
    private Quaternion startRotation;

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

        // Если камера перемещается плавно — обновляем интерполяцию и не обрабатываем ввод
        if (isMoving)
        {
            moveTimer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(moveTimer / moveDuration);

            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

            if (t >= 1f)
            {
                isMoving = false;
                Vector3 finalAngles = targetRotation.eulerAngles;
                yaw = finalAngles.y;
                pitch = finalAngles.x;
            }
            return;
        }

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

    /// <summary>
    /// Плавно перемещает камеру в заданную позицию и поворот за указанное время.
    /// Во время перемещения обычное управление блокируется.
    /// </summary>
    /// <param name="targetPos">Целевая позиция</param>
    /// <param name="targetRot">Целевой поворот</param>
    /// <param name="duration">Длительность анимации в секундах</param>
    public void SmoothMoveTo(Vector3 targetPos, Quaternion targetRot, float duration)
    {
        // Прерываем текущее перемещение, если оно было
        isMoving = true;
        targetPosition = targetPos;
        targetRotation = targetRot;
        moveDuration = Mathf.Max(duration, 0.01f); // Защита от нулевой длительности
        moveTimer = 0f;
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    public void MoveAndRotateTo(Vector3 position, Quaternion rotation) => SmoothMoveTo(position, rotation, 0.5f);
}