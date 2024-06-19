using UnityEngine;
using System.Collections;

public static class Utils
{
    public static Quaternion GetRotation(Matrix4x4 matrix)
    {
        var qw = Mathf.Sqrt(1f + matrix.m00 + matrix.m11 + matrix.m22) / 2;
        var w = 4 * qw;
        var qx = (matrix.m21 - matrix.m12) / w;
        var qy = (matrix.m02 - matrix.m20) / w;
        var qz = (matrix.m10 - matrix.m01) / w;
        if (float.IsNaN(qw) || float.IsInfinity(qw))
            qw = 1;
        if (float.IsNaN(qx) || float.IsInfinity(qx))
            qx = 1;
        if (float.IsNaN(qy) || float.IsInfinity(qy))
            qy = 1;
        if (float.IsNaN(qz) || float.IsInfinity(qz))
            qz = 1;

        
        return new Quaternion(qx, qy, qz, qw);
    }

    public static Vector3 GetPosition(Matrix4x4 matrix)
    {
        return matrix.GetColumn(3);
    }

    public static Vector3 GetScale(Matrix4x4 m)
    {
        var x = Mathf.Sqrt(m.m00 * m.m00 + m.m01 * m.m01 + m.m02 * m.m02);
        var y = Mathf.Sqrt(m.m10 * m.m10 + m.m11 * m.m11 + m.m12 * m.m12);
        var z = Mathf.Sqrt(m.m20 * m.m20 + m.m21 * m.m21 + m.m22 * m.m22);

        return new Vector3(x, y, z);
    }

    public static Vector3 MultiplyVector3(Vector3 a, Vector3 b)
    {
        Vector3 result = new Vector3();
        result.x = a.x * b.x;
        result.y = a.y * b.y;
        result.z = a.z * b.z;
        return result;
    }
}
