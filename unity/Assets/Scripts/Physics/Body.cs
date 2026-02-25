using UnityEngine;
using System.Collections.Generic;

public class Body : MonoBehaviour
{
    [Header("Physics")]
    [Range(0, 1)] public float dragCoefficient = 1f;
    [SerializeField] private new BoxCollider collider;
    [SerializeField] private new Rigidbody rigidbody;
    [Header("Property")]
    public Vector3 linearVelocity => rigidbody.linearVelocity;
    public Vector3 angularVelocity => rigidbody.angularVelocity;
    public Vector3 acceleration;

    [field: SerializeField] public float volume { get; private set; }
    [field: SerializeField] public float density { get; private set; }
    public float height => transform.position.y;

    private ComponentBody[] components;

    public float mass
    {
        get
        {
            return rigidbody.mass;
        }
        set
        {
            rigidbody.mass = value;
        }
    }

    public Vector3 size => collider.size;
    public Vector3 center => collider.center;
    public Vector3 worldSpaceCenter => rigidbody.worldCenterOfMass;
    private void OnEnable()
    {
        RecalculateLocalProperty();
        components = GetComponentsInChildren<ComponentBody>();
        for (int i = 0; i < components.Length; i++)
            components[i].OnInit(this);
    }

    private void OnDisable()
    {
        for (int i = 0; i < components.Length; i++)
            components[i].OnShutdown(this);
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

        AddForce(EnviromentSettings.GetDensity(height) * -EnviromentSettings.Gravity * volume);
        AddForce(EnviromentSettings.Gravity * mass);

        Vector3 v_ot = linearVelocity - (EnviromentSettings.WindDirection * Vector3.forward * EnviromentSettings.WindSpeed);
        AddForce(-0.5f * dragCoefficient
            * EnviromentSettings.GetDensity(height)
            * GeometryUtils.ComputeBoxProjectedArea(size, transform.rotation, EnviromentSettings.WindDirection)
            * v_ot.sqrMagnitude
            * v_ot.normalized);

        for (int i = 0; i < components.Length; i++)
            components[i].OnUpdate(this);

        // Финальное ускорение
        acceleration = (linearVelocity - saveLinearVelocity) / Time.fixedDeltaTime;
    }

    public void AddForce(Vector3 force, ForceMode mode = ForceMode.Force) => rigidbody.AddForce(force, mode);

    public void AddTorque(Vector3 torque, ForceMode mode = ForceMode.Force) => rigidbody.AddTorque(torque, mode);

    public void AddForceAtPosition(Vector3 force, Vector3 position, ForceMode mode = ForceMode.Force) => rigidbody.AddForceAtPosition(force,position, mode);
}