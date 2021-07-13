using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private bool isMovementEnabled;

    public static bool IsMovementEnabled;

    void Start()
    {
        SetMovementEnabled(true);
    }

    void Update()
    {
        
    }

    private void SetMovementEnabled(bool isEnabled)
    {
        isMovementEnabled = isEnabled;
        IsMovementEnabled = isEnabled;
    }
}
