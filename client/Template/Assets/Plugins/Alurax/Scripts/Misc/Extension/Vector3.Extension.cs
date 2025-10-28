using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extension
{
    public const float KFloatEpsilon = 0.001f;

    public static bool Approximately(this Vector3 self, Vector3 target)
    {
        if (Mathf.Abs(self.x - target.x) > KFloatEpsilon)
            return false;
        if (Mathf.Abs(self.y - target.y) > KFloatEpsilon)
            return false;
        if (Mathf.Abs(self.z - target.z) > KFloatEpsilon)
            return false;
        return true;
    }

    public static bool Approximately(this Quaternion self, Quaternion target)
    {
        if (Mathf.Abs(self.x - target.x) > KFloatEpsilon)
            return false;
        if (Mathf.Abs(self.y - target.y) > KFloatEpsilon)
            return false;
        if (Mathf.Abs(self.z - target.z) > KFloatEpsilon)
            return false;
        if (Mathf.Abs(self.w - target.w) > KFloatEpsilon)
            return false;
        return true;
    }
}    


