using UnityEngine;
public static class Vector3Extensions{
      public static Vector3 OverrideY(this Vector3 me, float newY){
        return new Vector3(me.x, newY, me.z);
    }
}