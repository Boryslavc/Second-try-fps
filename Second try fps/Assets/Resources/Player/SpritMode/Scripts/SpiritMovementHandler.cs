using UnityEngine;

public class SpiritMovementHandler : MonoBehaviour
{
    [Header("Walking parameters")]
    [SerializeField] private float _speed = 3.0f;
    [SerializeField] private float _speedMultiplyer = 2.0f;

    [Header("Look sensetivity")]
    [SerializeField] private float _mouseSensetivity = 2f;
    [SerializeField] private float _upAndDownRange = 80f;

    [SerializeField] private Camera camera;

    private InputHandler inputHandler;

    private float verticalRotation;

    public void Init(InputHandler inputHandler)
    {
        this.inputHandler = inputHandler;
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        Vector3 movingDirection = new Vector3();

        movingDirection += camera.transform.right * inputHandler.MoveInput.x;
        movingDirection += camera.transform.forward * inputHandler.MoveInput.y;

        movingDirection += Vector3.up * inputHandler.SpiritFlyFloat;

        transform.position += movingDirection * Time.deltaTime * _speed * (inputHandler.SprintBool ? _speedMultiplyer : 1.0f);
    }

    private void HandleRotation()
    {
        float xRotation = inputHandler.LookInput.x * _mouseSensetivity;
        transform.Rotate(0, xRotation, 0);

        verticalRotation -= inputHandler.LookInput.y * _mouseSensetivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -_upAndDownRange, _upAndDownRange);

        camera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    public Camera GetCamera()
    {
        return camera;
    }

    public InputHandler GetInputHandler()
    {
        return inputHandler;
    }
}
