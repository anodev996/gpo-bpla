using UnityEngine;

public class Body : MonoBehaviour
{
    [Header("Параметры бокса")]
    public Vector3 size = Vector3.one;       
    public Vector3 center = Vector3.zero;

    [Header("Физические свойства")]
    public float mass = 1f;

    [Header("Состояние")]
    public Vector3 linearVelocity;   
    public Vector3 angularVelocity;

    [Header("Настройки коллизий")]
    public LayerMask collisionMask = -1;           
    public int maxIterations = 5;                   
    public float skinWidth = 0.01f;                  
    [Range(0f, 1f)] public float bounciness = 0f;

    private void FixedUpdate()
    {
        AddForce(Physics.gravity * mass);



        // Вращаем объект (без проверки коллизий при вращении)
        if (angularVelocity != Vector3.zero)
            transform.Rotate(angularVelocity * Time.fixedDeltaTime, Space.World);

        // Выталкиваем из начальных перекрытий (если есть)
        ResolveOverlaps();

        // Вычисляем желаемое линейное перемещение
        Vector3 movement = linearVelocity * Time.fixedDeltaTime;

        // Обрабатываем столкновения и получаем скорректированное перемещение
        Vector3 correctedMovement = ResolveCollisions(movement);

        // Двигаем объект
        transform.position += correctedMovement;

        // Обновляем линейную скорость в соответствии с фактическим перемещением
        if (correctedMovement != movement)
        {
            linearVelocity = correctedMovement / Time.fixedDeltaTime;
        }
    }

    /// <summary>
    /// Выталкивание из перекрытий с другими коллайдерами в начальный момент.
    /// Используется простой алгоритм: смещение по направлению от ближайшей точки.
    /// </summary>
    private void ResolveOverlaps()
    {
        Vector3 worldCenter = transform.TransformPoint(center);
        Vector3 halfExtents = size * 0.5f;
        Quaternion rotation = transform.rotation;

        Collider[] overlaps = Physics.OverlapBox(worldCenter, halfExtents, rotation, collisionMask);
        if (overlaps.Length == 0) return;

        // Находим ближайшую точку на поверхности бокса для разрешения перекрытия
        // В реальном проекте стоит реализовать более аккуратный алгоритм,
        // но для демонстрации используем простое смещение в сторону от центра масс перекрытий.
        Vector3 depenetration = Vector3.zero;
        foreach (var col in overlaps)
        {
            // Ближайшая точка на коллайдере к центру нашего бокса
            Vector3 closestPoint = col.ClosestPoint(worldCenter);
            Vector3 direction = worldCenter - closestPoint;
            float distance = direction.magnitude;
            if (distance < 0.0001f) continue;

            // Требуемое смещение, чтобы устранить перекрытие (зазор skinWidth)
            float overlapDistance = halfExtents.magnitude - distance; // грубое приближение
            depenetration += direction.normalized * overlapDistance;
        }

        if (depenetration != Vector3.zero)
        {
            transform.position += depenetration;
        }
    }

    /// <summary>
    /// Выполняет непрерывную проверку столкновений (BoxCast) и корректирует перемещение
    /// с учётом скольжения вдоль поверхностей.
    /// </summary>
    private Vector3 ResolveCollisions(Vector3 movement)
    {
        Vector3 remainingMovement = movement;
        Vector3 newPosition = transform.position;

        Vector3 worldCenter = transform.TransformPoint(center);
        Vector3 halfExtents = size * 0.5f;
        Quaternion rotation = transform.rotation;

        for (int i = 0; i < maxIterations; i++)
        {
            float moveDistance = remainingMovement.magnitude;
            if (moveDistance < 0.0001f) break;

            Vector3 moveDir = remainingMovement.normalized;

            // Выполняем BoxCast
            if (Physics.BoxCast(worldCenter, halfExtents, moveDir, out RaycastHit hit, rotation, moveDistance, collisionMask))
            {
                float safeDistance = Mathf.Max(0, hit.distance - skinWidth);

                Vector3 moveToContact = moveDir * safeDistance;
                newPosition += moveToContact;
                remainingMovement = moveDir * (moveDistance - safeDistance);

                // Проецируем оставшееся перемещение на плоскость удара
                remainingMovement = Vector3.ProjectOnPlane(remainingMovement, hit.normal);

                // Применяем отскок
                if (bounciness > 0)
                {
                    Vector3 incomingVelocity = linearVelocity;
                    Vector3 reflected = Vector3.Reflect(incomingVelocity, hit.normal);
                    linearVelocity = Vector3.Lerp(incomingVelocity, reflected, bounciness);
                }

                // Обновляем центр и поворот для следующей итерации (позиция уже временно изменена)
                transform.position = newPosition;
                worldCenter = transform.TransformPoint(center);
                rotation = transform.rotation;
            }
            else
            {
                newPosition += remainingMovement;
                break;
            }
        }

        return newPosition - transform.position;
    }

    /// <summary>
    /// Визуализация бокса в редакторе.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 worldCenter = transform.TransformPoint(center);
        Quaternion rotation = transform.rotation;
        Vector3 halfExtents = size * 0.5f;

        // Рисуем каркасный куб
        Gizmos.matrix = Matrix4x4.TRS(worldCenter, rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, size);
        Gizmos.matrix = Matrix4x4.identity; // сбрасываем
    }

    /// <summary>
    /// Добавить линейную силу (учитывает массу).
    /// </summary>
    public void AddForce(Vector3 force, ForceMode mode = ForceMode.Force)
    {
        switch (mode)
        {
            case ForceMode.Force:
                linearVelocity += force / mass * Time.fixedDeltaTime;
                break;
            case ForceMode.Acceleration:
                linearVelocity += force * Time.fixedDeltaTime;
                break;
            case ForceMode.Impulse:
                linearVelocity += force / mass;
                break;
            case ForceMode.VelocityChange:
                linearVelocity += force;
                break;
        }
    }

    /// <summary>
    /// Добавить момент силы (угловое ускорение). Упрощённо (момент инерции = 1).
    /// </summary>
    public void AddTorque(Vector3 torque, ForceMode mode = ForceMode.Force)
    {
        switch (mode)
        {
            case ForceMode.Force:
                angularVelocity += torque / mass * Time.fixedDeltaTime;
                break;
            case ForceMode.Acceleration:
                angularVelocity += torque * Time.fixedDeltaTime;
                break;
            case ForceMode.Impulse:
                angularVelocity += torque / mass;
                break;
            case ForceMode.VelocityChange:
                angularVelocity += torque;
                break;
        }
    }

    public void AddForceAtPosition(Vector3 force, Vector3 position, ForceMode mode = ForceMode.Force)
    {
        // Центр масс (совпадает с центром бокса)
        Vector3 worldCenter = transform.TransformPoint(center);

        // Вектор от центра масс до точки приложения
        Vector3 relativePosition = position - worldCenter;

        // Момент силы
        Vector3 torque = Vector3.Cross(relativePosition, force);

        // Добавляем линейную составляющую
        AddForce(force, mode);

        // Добавляем момент
        AddTorque(torque, mode);
    }

    /// <summary>
    /// Установить линейную скорость.
    /// </summary>
    public void SetLinearVelocity(Vector3 newVelocity) => linearVelocity = newVelocity;

    /// <summary>
    /// Установить угловую скорость.
    /// </summary>
    public void SetAngularVelocity(Vector3 newAngularVelocity) => angularVelocity = newAngularVelocity;
}
