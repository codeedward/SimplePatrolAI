using UnityEngine;
public static class Vector2Extensions{
      public static Vector3 GetMe3(this Vector2 me){
        return new Vector3(me.x, 0, me.y);
    }
}
