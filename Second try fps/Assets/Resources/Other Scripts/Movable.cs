using UnityEngine;
using DG.Tweening;

public class Movable : MonoBehaviour, IShootable
{
    [SerializeField] private Transform[] _possiblePositions;
    [SerializeField] private float _delay;
    [SerializeField] private float _transitionDuration;
    [SerializeField] private float _transitionCoolDown;
    [SerializeField] private float _collisionDamage;
    [Tooltip("Optional gun type, by which transition will be invoked.")]
    [SerializeField] private Gun _gun;
    [Tooltip("Trigger button.")]
    [SerializeField] private ShootableButton button;

    public TweenCallback OnPositionChanged;

    private int currentPositionIndex;
    private float lastMoveTime;
    private bool isMoving;
    private ShootingHandler shooter;

    private void Awake()
    {
        shooter = FindAnyObjectByType<ShootingHandler>();

        if (button != null)
            button.OnBeingShot += () =>
            {
                if(Time.time > lastMoveTime + _transitionCoolDown && !isMoving)
                {
                    isMoving = true;
                    Move();
                }
            };
    }

    public void TakeBullet(float damage)
    {
        if (Time.time > lastMoveTime + _transitionCoolDown && button == null)
        {
            if(_gun == null)
            {
                if(!isMoving)
                {
                    isMoving = true;
                    Invoke(nameof(Move), _delay);
                }
            }
            else if(_gun.GetType() == shooter.GetCurrentGunType())
            {
                if (!isMoving)
                {
                    // to prevent from invoking multiple times if delay is > 0
                    isMoving = true;
                    Invoke(nameof(Move), _delay);
                }
            }
        }
    }

    private void Move()
    {
        var nextPositionIndex = (currentPositionIndex + 1) % _possiblePositions.Length;
        var nextPosition = _possiblePositions[nextPositionIndex].position;
        var nextRotation = _possiblePositions[nextPositionIndex].rotation;
        var nextScale = _possiblePositions[nextPositionIndex].localScale;

        transform.DOMove(nextPosition, _transitionDuration);
        transform.DORotate(nextRotation.eulerAngles, _transitionDuration);
        transform.DOScale(nextScale, _transitionDuration).OnComplete(OnPositionChanged);

        currentPositionIndex = nextPositionIndex;

        lastMoveTime = Time.time;

        isMoving = false;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(_collisionDamage > 0 && isMoving) 
        {
            if (button.transform.TryGetComponent<Health>(out Health health))
                health.TakeDamage(_collisionDamage);
        }    
    }
}
