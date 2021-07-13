using UnityEngine;

namespace Utilities.Extensions
{
    public static class Vector2IntExtensions
    {
        public static Vector3 ToVector3(this Vector2Int vector)
        {
            return new Vector3(vector.x, 0f, vector.y);
        }
    }
}