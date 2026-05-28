
using UnityEngine;

/// <summary>
/// Мусорный класс под небольшую анимацию перемещения
/// </summary>
public class PointInput : MonoBehaviour
{
    [SerializeField] private ControlledBody controlled;
    [SerializeField] private Vector3 point;
    [SerializeField] private float angle;

    public void FixedUpdate()
    {
        Vector3 toPoint = point - controlled.transform.position;

        if (toPoint.sqrMagnitude < 0.01f)
        {
            controlled.direction = Vector3.zero;
        }
        else
        {
            float x = toPoint.x;
            float z = toPoint.z;
            toPoint.x = z;
            toPoint.z = x;
            controlled.direction = Vector3.Lerp(controlled.direction, toPoint.normalized, Time.fixedDeltaTime);
        }
    }
}