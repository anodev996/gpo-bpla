using UnityEngine;
using System.Collections.Generic;
public class Body : MonoBehaviour
{
    [Header("Box parameters")]
    public Vector3 size = Vector3.one;       
    public Vector3 center = Vector3.zero;

    [Header("Physics")]
    public float mass = 1f;
    [Range(0,1)]public float dragCoefficient = 1f;

    [Header("Property")]
    public Vector3 linearVelocity;   
    public Vector3 angularVelocity;
    public Vector3 acceleration;

    [Header("Collision settings")]
    public LayerMask collisionMask = -1;           
    public int maxIterations = 5;                   
    public float skinWidth = 0.01f;                  
    [Range(0f, 1f)] public float bounciness = 0f;

    [field: SerializeField] public float volume { get; private set; }
    [field: SerializeField] public float density { get; private set; }
    public float height => transform.position.y;

    private void OnEnable()
    {
        RecalculateLocalProperty();
    }
    private void OnValidate()
    {
        if (mass <= 0)
            mass = 0.001f;
        RecalculateLocalProperty();
    }
    public void RecalculateLocalProperty()
    {
        volume = size.x * size.y * size.z;
        density = mass / volume;
    }

    private void FixedUpdate()
    {
        Vector3 saveLinearVelocity = linearVelocity;
        //F = pgv
        AddForce(EnviromentSettings.GetDensity(height) * -EnviromentSettings.Gravity * volume);

        //F = mg
        AddForce(EnviromentSettings.Gravity * mass);

        ///<summary>
        ///F = (1/2) * Cx * ρ * S * V²
        ///Где:
        ///F — сила сопротивления воздуха(в Ньютонах).
        ///Cx — коэффициент лобового сопротивления(аэродинамический коэффициент).Это безразмерная величина, которая зависит только от формы объекта и его ориентации в потоке .
        ///ρ(ро) — плотность воздуха(в кг/ м³). При нормальных условиях(на уровне моря, при температуре около + 15°C) она составляет примерно 1,2 — 1,225 кг / м³ .
        ///S — площадь миделева сечения(в м²).Это площадь проекции вашего куба на плоскость, перпендикулярную направлению движения . Для куба, движущегося перпендикулярно своей грани, это будет просто площадь этой грани: S = a², где a — длина ребра куба.
        ///V — скорость движения объекта относительно воздуха(в м / с).Обратите внимание, что скорость берется в квадрате.
        ///<summary>
        Vector3 v_ot = linearVelocity - (EnviromentSettings.WindDirection * Vector3.forward * EnviromentSettings.WindSpeed);
        AddForce(-0.5f * dragCoefficient 
            * EnviromentSettings.GetDensity(height) 
            * GeometryUtils.ComputeBoxProjectedArea(size, transform.rotation, EnviromentSettings.WindDirection) 
            * v_ot.sqrMagnitude 
            * v_ot.normalized);
        
        if (angularVelocity != Vector3.zero)
            transform.Rotate(angularVelocity * Time.fixedDeltaTime, Space.World);

        ResolveOverlaps();

        Vector3 movement = linearVelocity * Time.fixedDeltaTime;

        Vector3 correctedMovement = ResolveCollisions(movement);

        transform.position += correctedMovement;

        if (correctedMovement != movement)
        {
            linearVelocity = correctedMovement / Time.fixedDeltaTime;
        }
        acceleration = (linearVelocity - saveLinearVelocity) / Time.fixedDeltaTime;
    }

    private void ResolveOverlaps()
    {
        Vector3 worldCenter = transform.TransformPoint(center);
        Vector3 halfExtents = size * 0.5f;
        Quaternion rotation = transform.rotation;

        Collider[] overlaps = Physics.OverlapBox(worldCenter, halfExtents, rotation, collisionMask);
        if (overlaps.Length == 0) return;

        Vector3 depenetration = Vector3.zero;
        foreach (var col in overlaps)
        {
            Vector3 closestPoint = col.ClosestPoint(worldCenter);
            Vector3 direction = worldCenter - closestPoint;
            float distance = direction.magnitude;
            if (distance < 0.0001f) continue;

            float overlapDistance = halfExtents.magnitude - distance;
            depenetration += direction.normalized * overlapDistance;
        }

        if (depenetration != Vector3.zero)
        {
            transform.position += depenetration;
        }
    }

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

            if (Physics.BoxCast(worldCenter, halfExtents, moveDir, out RaycastHit hit, rotation, moveDistance, collisionMask))
            {
                float safeDistance = Mathf.Max(0, hit.distance - skinWidth);

                Vector3 moveToContact = moveDir * safeDistance;
                newPosition += moveToContact;
                remainingMovement = moveDir * (moveDistance - safeDistance);

                remainingMovement = Vector3.ProjectOnPlane(remainingMovement, hit.normal);

                if (bounciness > 0)
                {
                    Vector3 incomingVelocity = linearVelocity;
                    Vector3 reflected = Vector3.Reflect(incomingVelocity, hit.normal);
                    linearVelocity = Vector3.Lerp(incomingVelocity, reflected, bounciness);
                }

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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 worldCenter = transform.TransformPoint(center);
        Quaternion rotation = transform.rotation;
        Vector3 halfExtents = size * 0.5f;

        Gizmos.matrix = Matrix4x4.TRS(worldCenter, rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, size);
        Gizmos.matrix = Matrix4x4.identity; 
    }

    public void AddForce(Vector3 force, ForceMode mode = ForceMode.Force)
    {
        force += force;
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
        Vector3 worldCenter = transform.TransformPoint(center);
        Vector3 relativePosition = position - worldCenter;
        Vector3 torque = Vector3.Cross(relativePosition, force);
        AddForce(force, mode);
        AddTorque(torque, mode);
    }

    public void SetLinearVelocity(Vector3 newVelocity) => linearVelocity = newVelocity;

    public void SetAngularVelocity(Vector3 newAngularVelocity) => angularVelocity = newAngularVelocity;
}
