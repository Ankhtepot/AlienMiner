using UnityEngine;

namespace Utilities.Extensions
{
    public static class Vector3IntExtensions
    {
        public static Vector2Int ToVector2Int(this Vector3Int source)
        {
            return new Vector2Int(source.x, source.z);
        }
    }
}