using UnityEngine;

public static class GeometryUtils
{
    public static float ComputeBoxProjectedArea(Vector3 boxSize, Quaternion boxRotation, Quaternion windDirection)
    {
        Vector3 windDir = windDirection * Vector3.forward;
        windDir.Normalize();

        Vector3 right = boxRotation * Vector3.right;
        Vector3 up = boxRotation * Vector3.up;
        Vector3 forward = boxRotation * Vector3.forward;

        float areaX = boxSize.y * boxSize.z;
        float areaY = boxSize.x * boxSize.z;
        float areaZ = boxSize.x * boxSize.y;

        float dotX = Mathf.Abs(Vector3.Dot(windDir, right));
        float dotY = Mathf.Abs(Vector3.Dot(windDir, up));
        float dotZ = Mathf.Abs(Vector3.Dot(windDir, forward));

        return areaX * dotX + areaY * dotY + areaZ * dotZ;
    }

    public static float ComputeBoxProjectedArea(Vector3 boxSize, Quaternion boxRotation, Vector3 windDirection) => ComputeBoxProjectedArea(boxSize, boxRotation, Quaternion.Euler(windDirection));
}