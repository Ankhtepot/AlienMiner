using UnityEngine;

namespace Utilities.Extensions
{
    public static class Vector3Extensions
    {
        public static Vector3Int ToVector3Int(this Vector3 source)
        {
            return new Vector3Int(
                Mathf.RoundToInt(source.x),
                Mathf.RoundToInt(source.y),
                Mathf.RoundToInt(source.z)
                );
        }

        public static Vector2Int ToVector2IntXZ(this Vector3 source)
        {
            return new Vector2Int(
                Mathf.RoundToInt(source.x),
                Mathf.RoundToInt(source.z));
        }
    }
}