using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector2Extension
{
    public static bool Approximately(this Vector2 self, Vector2 target)
    {
        if (Mathf.Abs(self.x - target.x) > Vector3Extension.KFloatEpsilon)
            return false;
        if (Mathf.Abs(self.y - target.y) > Vector3Extension.KFloatEpsilon)
            return false;
        return true;
    }
}    



