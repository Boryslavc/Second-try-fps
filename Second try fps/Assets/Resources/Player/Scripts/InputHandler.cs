using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [Header("Input Action Asset Reference")]
    [SerializeField] private InputActionAsset _inputActions;

    [Header("Input Map Names References")]
    [SerializeField] private string _mapName = "BasicMovement";

    [Header("Movement Action Names")]
    [SerializeField] private string _moveActionName = "Move";
    [SerializeField] private string _lookActionName = "Look";
    [SerializeField] private string _sprintActionName = "Sprint";
    [SerializeField] private string _jumpActionName = "Jump";

    [Header("Shooting Action Names")]
    [SerializeField] private string _shootActionName = "Shoot";
    [SerializeField] private string _aimActionName = "Aim";
    [SerializeField] private string _changeWeaponName = "ChangeWeapon";


    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction sprintAction;
    private InputAction jumpAction;

    private InputAction shootAction;
    private InputAction aimAction;
    private InputAction changeWeapon;


    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool JumpBool { get; private set; }
    public bool SprintBool { get; private set; }
    public bool ShootBool { get;private set; }
    public bool AimBool { get; private set; }
    public int ChangeWeaponInt { get; private set; }


    private void Awake()
    {
        AssignActions();

        InitializeActions();
    }

    private void AssignActions()
    {
        var inputMap = _inputActions.FindActionMap(_mapName);

        moveAction = inputMap.FindAction(_moveActionName);
        lookAction = inputMap.FindAction(_lookActionName);
        jumpAction = inputMap.FindAction(_jumpActionName);
        sprintAction = inputMap.FindAction(_sprintActionName);

        shootAction = inputMap.FindAction(_shootActionName);
        aimAction = inputMap.FindAction(_aimActionName);
        changeWeapon = inputMap.FindAction(_changeWeaponName);
    }

    private void InitializeActions()
    {
        moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => MoveInput = Vector2.zero;


        lookAction.performed += context => LookInput = context.ReadValue<Vector2>();
        lookAction.canceled += context => LookInput = Vector2.zero;

        jumpAction.performed += context => JumpBool = true;
        jumpAction.canceled += context => JumpBool = false;

        sprintAction.performed += context => SprintBool = true;
        sprintAction.canceled += context => SprintBool = false;

        shootAction.performed += context => ShootBool = true;
        shootAction.canceled += context => ShootBool = false;

        aimAction.performed += context => AimBool = true;
        aimAction.canceled += context => AimBool = false;

        changeWeapon.performed += context => 
                        ChangeWeaponInt = Mathf.Clamp((int)context.ReadValue<float>(),-1,1);
        changeWeapon.canceled += context => ChangeWeaponInt = 0;
    }

    private void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
        jumpAction.Enable();
        sprintAction.Enable();

        shootAction.Enable();
        aimAction.Enable();
        changeWeapon.Enable();
    }


    private void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
        jumpAction.Disable();
        sprintAction.Disable();

        shootAction.Disable();
        aimAction.Disable();
        changeWeapon.Disable();
    }
}
