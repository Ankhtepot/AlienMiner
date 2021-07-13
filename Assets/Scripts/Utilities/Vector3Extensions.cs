using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extensions
{
    public static readonly Vector3 Up;
    public static readonly Vector3 Down;
    public static readonly Vector3 Left;
    public static readonly Vector3 Right;
    public static readonly Vector3 Forward;
    public static readonly Vector3 Back;
    public static readonly Vector3 Zero;
    public static readonly Vector3 One;

    static Vector3Extensions() {
        Up = Vector3.up;
        Down = Vector3.down;
        Left = Vector3.left;
        Right = Vector3.right;
        Forward = Vector3.forward;
        Back = Vector3.back;
        Zero = Vector3.zero;
        One = Vector3.one;
    }

}
