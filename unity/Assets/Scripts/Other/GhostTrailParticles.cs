using System.Collections;
using UnityEngine;


public class GhostTrailParticles : MonoBehaviour
{
    [Header("Particle System")]
    public new ParticleSystem particleSystem;   // система частиц для эффекта

    [Header("Пороги")]
    public float angleThreshold = 15f;
    public float angleDelta;
    private Quaternion previousRotation;

    void Start()
    {
        if (particleSystem == null)
            particleSystem = GetComponent<ParticleSystem>();
        
        previousRotation = transform.rotation;
    }

    void Update()
    {
        Vector3 currentPosition = transform.position;
        Quaternion currentRotation = transform.rotation;

        angleDelta += Quaternion.Angle(previousRotation, currentRotation);

        bool thresholdReached = angleDelta >= angleThreshold;

        if (thresholdReached)
        {
            int steps = Mathf.FloorToInt(angleDelta / angleThreshold);

            for (int i = 1; i <= steps; i++)
            {
                float t = (float)i / (steps + 1); 
                Quaternion interpolatedRot = Quaternion.Slerp(previousRotation, currentRotation, t);

                angleDelta -= angleThreshold; 
                EmitParticle(currentPosition, interpolatedRot);
            }
            previousRotation = currentRotation;
        }
    }

    private void EmitParticle(Vector3 position, Quaternion rotation)
    {
        var emitParams = new ParticleSystem.EmitParams();
        emitParams.position = position;
        emitParams.rotation3D = rotation.eulerAngles;

        particleSystem.Emit(emitParams, 1);
    }
}