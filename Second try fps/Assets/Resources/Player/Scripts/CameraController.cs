using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField] private float speed;

    private Camera camera;
    private InputHandler inputHandler;

    private bool isDecreased = false;
    private bool corourineIsPlaying;

    private void Awake()
    {
        camera = GetComponent<Camera>();
        inputHandler = GetComponentInParent<InputHandler>();
    }

    private void Update()
    {
        if(inputHandler.SprintBool && !corourineIsPlaying)
        {
            if(!isDecreased)
                StartCoroutine(nameof(DecreaseFOV));
        }
        else if(!corourineIsPlaying)
        {
            if(isDecreased)
                StartCoroutine(nameof(IncreaseFOV));
        }
    }
    private IEnumerator DecreaseFOV()
    {
        corourineIsPlaying = true;

        float timer = 0;

        float initialFov = 60f;
        float decreased = 45f; 

        while(timer < 1)
        {
            camera.fieldOfView = Mathf.Lerp(initialFov, decreased, timer);
            timer += speed * Time.deltaTime;
            yield return null;
        }

        isDecreased = true;
        corourineIsPlaying = false;
    }

    private IEnumerator IncreaseFOV()
    {
        corourineIsPlaying = true;

        float timer = 0;

        float initialFov = 45f;
        float increased = 60f;

        while (timer < 1)
        {
            camera.fieldOfView = Mathf.Lerp(initialFov, increased, timer);
            timer += speed * Time.deltaTime;
            yield return null;
        }

        isDecreased = false;
        corourineIsPlaying = false;
    }
}
