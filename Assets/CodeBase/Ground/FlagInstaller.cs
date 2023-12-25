using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ClickTracker))]
[RequireComponent(typeof(Ground))]
public class FlagInstaller : MonoBehaviour
{
    [SerializeField] private FlagSpawner _flagSpawner;
    [SerializeField] private LayerMask _interferencesMask;
    private ClickTracker _clickTracker;
    private Ground _ground;
    private Flag _flag;      
    
    public Flag Flag => _flag;
    public Vector3 FlagPosition { get; private set; }

    public event UnityAction FlagInstalled;
    public event UnityAction<Vector3> PositionChanged;    

    public void Construct(FlagSpawner flagSpawner)
    {
        _flagSpawner = flagSpawner;
    }

    private void Awake()
    {
        _clickTracker = GetComponent<ClickTracker>();
        _ground = GetComponent<Ground>();
    }

    private void OnEnable()
    {
        _clickTracker.ClickHappened += OnClickHappened;
    }

    private void OnDisable()
    {
        _clickTracker.ClickHappened -= OnClickHappened;
    }

    private void OnClickHappened()
    {
        if (_flagSpawner.TrySpawnFlag(out Flag flag))
        {
            _flag = flag;

            if(_ground.TrySetPosition(out Vector3 position, _flag.Radius, _interferencesMask))
            {
                FlagPosition = _flag.gameObject.transform.position = position;
                FlagInstalled?.Invoke();
            }            
        }
        else
        {
            if(_flag != null)
            {
                if (_ground.TrySetPosition(out Vector3 position, _flag.Radius, _interferencesMask))
                {
                    FlagPosition = _flag.gameObject.transform.position = position;
                    PositionChanged?.Invoke(FlagPosition);
                }                   
            }
        }
    }    
}