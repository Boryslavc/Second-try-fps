using UnityEngine;
using UnityEngine.Events;

public class MovingStructure : MonoBehaviour
{
    [SerializeField] private MovingStructureNode currentNode;
    [SerializeField][Range(0, 0.5f)] private float maxDeviation = 0.2f;

    private Vector3 _currentRealDirection;

    private MovingStructureNode _nextNode;
    private bool _inTravel;
    private float _allDistance;
    private float _completedPath;

    public UnityAction onNodeReached;

    private void Start()
    {
        transform.position = currentNode.transform.position;

        _currentRealDirection = Vector3.zero;

        _nextNode = null;
        _inTravel = false;
        _completedPath = 0;
        _allDistance = 0;
    }

    public void Move(Vector3 normalizedDiraction, float distance)
    {
        if (!_inTravel)
        {
            _currentRealDirection = FindRealDirection(normalizedDiraction, out _nextNode);

            if (_nextNode == null)
                return;
            else
            {
                _inTravel = true;
                _allDistance = Vector3.Distance(currentNode.transform.position, _nextNode.transform.position);
            }
        }

        //  если смотрим в противоположном направлении
        if (Vector3.Dot(_currentRealDirection, normalizedDiraction) < -(1.0f - maxDeviation))
            distance *= -1;
        else if (1.0f - Vector3.Dot(_currentRealDirection, normalizedDiraction) > maxDeviation)
            return;

        _completedPath += distance;

        if (_completedPath >= _allDistance)
            _completedPath = _allDistance;
        else if (_completedPath <= 0.0f)
            _completedPath = 0.0f;
        else
            transform.position += _currentRealDirection * distance;

        if (_completedPath == 0)
        {
            transform.position = currentNode.transform.position;

            _currentRealDirection = Vector3.zero;

            _nextNode = null;
            _inTravel = false;
            _completedPath = 0;
            _allDistance = 0;

            onNodeReached?.Invoke();
        }
        else if (_completedPath == _allDistance)
        {
            currentNode = _nextNode;
            transform.position = currentNode.transform.position;

            _currentRealDirection = Vector3.zero;

            _nextNode = null;
            _inTravel = false;
            _completedPath = 0;
            _allDistance = 0;

            onNodeReached?.Invoke();
        }
    }

    private Vector3 FindRealDirection(Vector3 normalizedDiraction, out MovingStructureNode nextNode)
    {
        Vector3 realDirection = Vector3.zero;
        float maxDot = -1f;
        float currentDot;

        nextNode = null;

        for (int i = 0; i < currentNode.forwardVectors.Count; i++)
        {
            currentDot = Vector3.Dot(normalizedDiraction, currentNode.forwardVectors[i]);

            if (currentDot > maxDot)
            {
                maxDot = currentDot;
                nextNode = currentNode.GetConnectedNodes()[i];
                realDirection = currentNode.forwardVectors[i];
            }
        }

        if (1.0f - maxDot <= maxDeviation)
            return realDirection;
        else
        {
            nextNode = null;
            return Vector3.zero;
        }
    }    
}