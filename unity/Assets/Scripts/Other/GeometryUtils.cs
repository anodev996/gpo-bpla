using UnityEngine;

public static class GeometryUtils
{
    public static Vector3 GetPerpendicularPointOnLine(Vector3 lineA, Vector3 lineB, Vector3 point)
    {
        Vector3 lineVector = lineB - lineA;
        float lineLengthSqr = lineVector.sqrMagnitude;

        if (lineLengthSqr < Mathf.Epsilon)
            return lineA;

        float t = Vector3.Dot(point - lineA, lineVector) / lineLengthSqr;

        return lineA + t * lineVector;
    }

    public static float GetDistanceToLine(Vector3 lineA, Vector3 lineB, Vector3 point)
    {
        Vector3 lineDir = lineB - lineA;
        float lineLengthSqr = lineDir.sqrMagnitude;

        if (lineLengthSqr < Mathf.Epsilon)
            return Vector3.Distance(point, lineA);

        float t = Vector3.Dot(point - lineA, lineDir) / lineLengthSqr;

        Vector3 closestPoint = lineA + t * lineDir;

        return Vector3.Distance(point, closestPoint);
    }

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