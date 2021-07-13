using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] Transform playerBody;
    [SerializeField] Transform rotationCenter;
    [SerializeField] float movementSpeed = 100;
    [SerializeField] float yawnSpeed = 1f;
    [SerializeField] float rotateToDirectionSpeed = 0.3f;
    [SerializeField] float yawnReverseSpeed = 2f;
    [SerializeField] float maxYawnAngle = 10f;

    private Vector3 lastSetMovementDirection = Vector3Extensions.Zero;
    private Vector3 targetDirection = Vector3Extensions.Zero;
    private float yawnAngle = 0;
    private float movementSpeedScaled = 0;
    [SerializeField] private bool shouldRotate = false;
    

    void Start()
    {
        this.AutoCheck();
    }

    void Update()
    {
        if(GameManager.IsMovementEnabled)
        {
            ProcessInput();
        }
    }

    private void ProcessInput()
    {
        movementSpeedScaled = movementSpeed * Time.deltaTime;
        Vector3 newMoveVector = Vector3Extensions.Zero;
        bool isMoving = false;

        if (Input.GetKey(KeyCode.D))
        {
            newMoveVector += Vector3Extensions.Right;
        }
        if (Input.GetKey(KeyCode.A))
        {
            newMoveVector += Vector3Extensions.Left;
        }
        if (Input.GetKey(KeyCode.W))
        {
            newMoveVector += Vector3Extensions.Forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            newMoveVector += Vector3Extensions.Back;
        }

        if (newMoveVector != Vector3Extensions.Zero)
        {
            shouldRotate = true;
            isMoving = true;
            targetDirection = newMoveVector * movementSpeedScaled;
            lastSetMovementDirection = newMoveVector;
        }

        SetYawn(isMoving);

        RotateToDirection();

        player.transform.position += targetDirection;

        Vector3 playerBodyRotation = new Vector3(yawnAngle, playerBody.rotation.eulerAngles.y, 0);
        playerBody.rotation = Quaternion.Euler(playerBodyRotation);
    }

    private void SetYawn(bool isMoving)
    {
        if (isMoving)
        {
            if (yawnAngle < maxYawnAngle)
            {
                yawnAngle += yawnSpeed * Time.deltaTime;
            }
        }
        else
        {
            targetDirection = Vector3Extensions.Zero;

            if (yawnAngle < 0f)
            {
                yawnAngle = 0f;
            }

            if (yawnAngle > 0.1f)
            {
                yawnAngle -= yawnReverseSpeed * Time.deltaTime;
            }
        }
    }

    private void RotateToDirection()
    {
        if (!shouldRotate) return;

        if ((playerBody.transform.forward - lastSetMovementDirection).magnitude > 0.01f)
        {
            playerBody.transform.forward = Vector3.Slerp(playerBody.transform.forward, lastSetMovementDirection, rotateToDirectionSpeed * Time.deltaTime);
        }
        else
        {
            //Debug.LogError("Switchin rotation to OFF.");
            shouldRotate = false;
        }
    }
}
