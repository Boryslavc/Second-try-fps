using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Walking parameters")]
    [SerializeField] private float _walkingSpeed = 3.0f;
    [SerializeField] private float _sprintMultiplier = 2f;

    [Header("Jump parameters")]
    [SerializeField] private float _jumpForce = 4f;
    [SerializeField] private float _gravity = 9.8f;

    [Header("Look sensetivity")]
    [SerializeField] private float _mouseSensetivity = 2f;
    [SerializeField] private float _upAndDownRange = 80f;


    private CharacterController characterController;
    private Camera camera;
    private InputHandler inputHandler;

    private Vector3 currentMoveDirection;
    private float verticalRotation;

    private void Awake()
    {
        camera = Camera.main;
        characterController = GetComponent<CharacterController>();
        inputHandler = GetComponent<InputHandler>();
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        float speed = _walkingSpeed;
        if (inputHandler.SprintBool)
            speed *= _sprintMultiplier;

        Vector3 movingDirection = new Vector3(inputHandler.MoveInput.x, 0, inputHandler.MoveInput.y);
        Vector3 worldDirection = transform.TransformDirection(movingDirection);
        worldDirection.Normalize();

        worldDirection =  HandleSlopeMovement(worldDirection);

        currentMoveDirection.x = worldDirection.x * speed;
        currentMoveDirection.z = worldDirection.z * speed;

        HandleJumping();

        characterController.Move(currentMoveDirection * Time.deltaTime);
    }

    private Vector3 HandleSlopeMovement(Vector3 moveDirection)
    {
        if (characterController.isGrounded)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, Vector3.down, out hit, 2))
            {
                var angle = Vector3.Angle(Vector3.up, hit.normal);

                if (angle > 5)
                {
                    Vector3 projectedVector = Vector3.ProjectOnPlane(moveDirection, hit.normal);
                    return projectedVector;
                }
            }
        }
        return moveDirection;
    }
    private void HandleJumping()
    {
        if (characterController.isGrounded)
        {
            if (inputHandler.JumpBool)
                currentMoveDirection.y = _jumpForce;
        }
        else
        {
            currentMoveDirection.y -= _gravity * Time.deltaTime;
        }
    }
    private void HandleRotation()
    {
        float xRotation = inputHandler.LookInput.x * _mouseSensetivity;
        transform.Rotate(0, xRotation, 0);

        verticalRotation -= inputHandler.LookInput.y * _mouseSensetivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -_upAndDownRange, _upAndDownRange);

        camera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }
}
