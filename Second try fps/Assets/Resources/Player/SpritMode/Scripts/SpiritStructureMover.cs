using UnityEngine;

public class SpiritStructureMover : MonoBehaviour
{
    [SerializeField] private LayerMask movableStructureLayer;
    [SerializeField] private float reachingDistance = 5.0f;

    private SpiritMovementHandler movementHandler;
    private InputHandler inputHandler;
    private Camera camera;

    private MovingStructure currentStructure;

    private void Start()
    {
        movementHandler = GetComponent<SpiritMovementHandler>();

        camera = movementHandler.GetCamera();
        inputHandler = movementHandler.GetInputHandler();
        currentStructure = null;
    }

    private void Update()
    {
        Transform structureTransform;
        bool isLookingOnMovableStructure = IsLookingOnMovableStructure(out structureTransform);

        if (currentStructure == null && isLookingOnMovableStructure)
            currentStructure = structureTransform.gameObject.GetComponent<MovingStructure>();


        if (isLookingOnMovableStructure && currentStructure.transform.GetInstanceID() != structureTransform.GetInstanceID())
            currentStructure = structureTransform.GetComponent<MovingStructure>();


        if (isLookingOnMovableStructure && inputHandler.ShootBool)
            currentStructure.Move(camera.transform.forward, 0.03f);
    }

    private bool IsLookingOnMovableStructure(out Transform structure)
    {
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        RaycastHit hit;

        Debug.DrawRay(camera.transform.position, camera.transform.forward * 2, Color.blue);

        if (Physics.Raycast(ray, out hit, reachingDistance, movableStructureLayer))
        {
            structure = hit.transform.parent;
            return true;
        }
        else
        {
            structure = null;
            return false;
        }
    }
}
