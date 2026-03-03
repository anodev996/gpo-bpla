using System.Collections.Generic;
using System.IO;
using TriInspector;
using UnityEditor;
using UnityEngine;
using static UnityEditor.IMGUI.Controls.CapsuleBoundsHandle;

public class Quadcopter : ControlledBody
{
    [SerializeField, Group("Main"), Tab("Quadcopter")] protected CubeMatrix<Motor> motors;
    [ShowInInspector, Group("Main"), Tab("Quadcopter"), ReadOnly] protected Matrix2x2 movement;
    [SerializeField, Group("Main"), Tab("Quadcopter")] private Vector3 forwardAxis = Vector3.forward;
    [SerializeField, Group("Main"), Tab("Quadcopter")] private Vector3 rightAxis = Vector3.right;
    [SerializeField] private float maxAcceleration = 8f;

    // PID регуляторы для высоты и рыскания (остаются)
    [field: SerializeField, Group("Main"), Tab("PID")] public PIDController altitudePID { get; private set; }
    [field: SerializeField, Group("Main"), Tab("PID")] public PIDController yawPID { get; private set; }

    [field: SerializeField, Group("Main"), Tab("PID")] public PIDController velPitchPID { get; private set; }
    [field: SerializeField, Group("Main"), Tab("PID")] public PIDController velRollPID { get; private set; }

    [field: SerializeField, Group("Main"), Tab("Quadcopter")] public float maxAngle { get; private set; } = 30f;
    [field: SerializeField, Group("Main"), Tab("Quadcopter")] public float correctionFactor { get; private set; } = 1f;
    [field: SerializeField, Group("Main"), Tab("Quadcopter")] public float damping { get; private set; } = 0.1f;
    protected override void OnEnable()
    {
        base.OnEnable();
        targetPosition = transform.position;
    }
    private Vector3 upAxis => Vector3.Cross(forwardAxis, rightAxis).normalized;
    public float throttle, pitchCorrection, rollCorrection, yawCorrection;
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        UpdateMovement(direction, angle);
        // Применяем нагрузку к моторам
        for (int x = 0; x < 2; x++)
            for (int y = 0; y < 2; y++)
                motors[x, y].Workload = movement[x, y];
    }

    public void TestMotors(float throttle, float pitchCorrection, float rollCorrection, float yawCorrection)
    {
        movement[0, 0] = throttle + pitchCorrection + rollCorrection - yawCorrection; // левый передний
        movement[1, 0] = throttle + pitchCorrection - rollCorrection + yawCorrection; // правый передний
        movement[0, 1] = throttle - pitchCorrection + rollCorrection + yawCorrection; // левый задний
        movement[1, 1] = throttle - pitchCorrection - rollCorrection - yawCorrection; // правый задний
    }
    public Vector3 targetPosition { get; set; }
    public float targetYaw { get; set; }

    /// <summary>
    /// Вычисляет нагрузку на моторы для достижения желаемого движения.
    /// </summary>
    /// <param name="direction">Целевая линейная скорость в мировых координатах.</param>
    /// <param name="angle">Целевой угол рысканья (в градусах).</param>
    private void UpdateMovement(Vector3 direction, float angle)
    {
        float dt = Time.fixedDeltaTime;
        targetPosition += direction * dt;
        targetYaw += angle;
        // --- Текущее состояние ---
        Vector3 currentPos = transform.position;
        float currentYaw = transform.eulerAngles.y;
        Vector3 velocity = linearVelocity;       // мировая линейная скорость
        Vector3 angularVel = angularVelocity;    // мировая угловая скорость

        // --- Ошибки ---
        Vector3 errorPos = targetPosition - currentPos;
        float errorYaw = Mathf.DeltaAngle(currentYaw, targetYaw);

        // Локальные оси дрона (преобразуем мировую скорость в локальную)
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);
        float forwardSpeed = Vector3.Dot(localVelocity, forwardAxis);
        float rightSpeed = Vector3.Dot(localVelocity, rightAxis);
        float upSpeed = velocity.y; // вертикальная скорость остаётся глобальной (если ось Y – вверх)

        // Локальная ошибка позиции
        Vector3 localErrorPos = transform.InverseTransformDirection(errorPos);
        float errorForward = Vector3.Dot(localErrorPos, forwardAxis);
        float errorRight = Vector3.Dot(localErrorPos, rightAxis);
        float errorHeight = errorPos.y;

        // Угловые скорости в локальных осях
        Vector3 localAngularVel = transform.InverseTransformDirection(angularVel);
        float angularPitch = Vector3.Dot(localAngularVel, rightAxis);
        float angularRoll = Vector3.Dot(localAngularVel, forwardAxis);
        float angularYaw = Vector3.Dot(localAngularVel, upAxis);

        // Управление высотой
        throttle = altitudePID.Update(errorHeight, dt) - upSpeed * damping;
        throttle = Mathf.Clamp(throttle, 0f, 1f);
        // Внешний контур по позиции (Пока что удален, я типо хз возможно будем его использовать в автоРежиме)
        // Внутренний контур: ошибка скорости -> желаемый угол наклона
        float errorForwardSpeed = direction.x - forwardSpeed;
        float errorRightSpeed = direction.z - rightSpeed;

        float desiredPitch = velPitchPID.Update(errorForwardSpeed, dt);
        float desiredRoll = velRollPID.Update(errorRightSpeed, dt);

        // Ограничение углов
        desiredPitch = Mathf.Clamp(desiredPitch, -maxAngle, maxAngle);
        desiredRoll = Mathf.Clamp(desiredRoll, -maxAngle, maxAngle);

        // Управление рысканием
        yawCorrection = -yawPID.Update(errorYaw, dt) * correctionFactor + angularYaw * damping;
        yawCorrection = Mathf.Clamp(yawCorrection, -1f, 1f);

        // Преобразование углов в коррекции моторов
        pitchCorrection = desiredPitch * correctionFactor - angularPitch * damping;
        rollCorrection = desiredRoll * correctionFactor + angularRoll * damping;

        // --- Миксер X-конфигурации ---
        movement[0, 0] = throttle + pitchCorrection + rollCorrection - yawCorrection; // левый передний
        movement[1, 0] = throttle + pitchCorrection - rollCorrection + yawCorrection; // правый передний
        movement[0, 1] = throttle - pitchCorrection + rollCorrection + yawCorrection; // левый задний
        movement[1, 1] = throttle - pitchCorrection - rollCorrection - yawCorrection; // правый задний

        movement.Clamp01();
    }
}