using UnityEngine;

public class ModeSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject spiritPrefab;

    [SerializeField] private Camera playerCamera;
    private Camera spiritCamera;

    private PlayerMovement playerMovement;
    private ShootingHandler shootingHandler;
    private InputHandler inputHandler;

    private SpiritMovementHandler spiritMovementHandler;

    public bool isSpirit { get; private set; }

    private void OnEnable()
    {
        inputHandler.onModeChanged += OnAttempToChangeMode;
    }

    private void OnDisable()
    {
        inputHandler.onModeChanged -= OnAttempToChangeMode;
    }

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        shootingHandler = GetComponent<ShootingHandler>();
        inputHandler = GetComponent<InputHandler>();
    }

    public void OnAttempToChangeMode()
    {
        if (isSpirit)
            SwitchToNormal();
        else
            SwitchToSpirit();

        Debug.Log(isSpirit);
    }

    private void SwitchToSpirit()
    {
        isSpirit = true;

        playerMovement.enabled = false;
        shootingHandler.enabled = false;

        playerCamera.enabled = false;

        SpawnSpirit();

        spiritCamera.enabled = true;
    }

    private void SwitchToNormal()
    {
        isSpirit = false;

        playerMovement.enabled = true;
        shootingHandler.enabled = true;

        spiritCamera.enabled = false;
        playerCamera.enabled = true;

        KillSpirit();
    }

    private void SpawnSpirit()
    {
        spiritMovementHandler = Instantiate(spiritPrefab, transform.position, Quaternion.identity).GetComponent<SpiritMovementHandler>();
        spiritCamera = spiritMovementHandler.GetCamera();
        spiritMovementHandler.Init(inputHandler);
    }

    private void KillSpirit()
    {
        GameObject.Destroy(spiritMovementHandler.gameObject);
        spiritMovementHandler = null;
        spiritCamera = null;
    }
}
