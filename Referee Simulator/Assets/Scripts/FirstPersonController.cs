using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class FirstPersonController : MonoBehaviour
{
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    private Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    private CharacterController _characterController;
    private Vector3 _moveDirection = Vector3.zero;
    private float _rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    private AnimatorController _animatorController;

    private Ball _ball;
    
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        _animatorController = GetComponent<AnimatorController>();
        _ball = null;
    }

    void Update()
    {
        
        MovementAndRotation();
            
        // Lock cursor
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        
        
        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = true;
    }

    private void MovementAndRotation()
    {
        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = _moveDirection.y;
        _moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && _characterController.isGrounded)
        {
            _moveDirection.y = jumpSpeed;
        }
        else
        {
            _moveDirection.y = movementDirectionY;
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!_characterController.isGrounded)
        {
            _moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        _moveDirection *= Time.deltaTime;
        _characterController.Move(_moveDirection);

        // Player and Camera rotation
        if (canMove)
        {
            _rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            _rotationX = Mathf.Clamp(_rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        
        if (_moveDirection.x != 0 || _moveDirection.z != 0)
        {
            _animatorController.SetParameter("running", true);
            if (_ball != null)
            {
                _ball.StartBallRotation();
            }
               
        }
        else
        {
            _animatorController.SetParameter("running", false);
            if (_ball != null)
            {
                _ball.SetBallRotation(false);
            }
        }

    }

    public Vector3 GetMoveDirection()
    {
        return _moveDirection;
    }

    public void SetBall(Ball ball)
    {
        _ball = ball;
    }

}
