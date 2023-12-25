using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ClickTracker))]
public class FlagSpawner : MonoBehaviour
{
    private ClickTracker _clickTracker;
    private Flag _flag;
    private bool _isFlagReadyToInstalled;    

    public event UnityAction FlagReadyToInstalled;

    private void Awake()
    {
        _clickTracker = GetComponent<ClickTracker>();
        _flag = Resources.Load<Flag>(PrefabsPath.Flag);
        _isFlagReadyToInstalled = false;
    }

    private void Start()
    {
        _clickTracker.ClickHappened += OnClickHappened;
    }

    public bool TrySpawnFlag(out Flag flag)
    {
        if (_isFlagReadyToInstalled)
        {
            flag = Instantiate(_flag);            
            _isFlagReadyToInstalled = false;
            return true;
        }
        else
        {
            flag = null;
            return false;
        }
    }

    private void OnClickHappened()
    {
        _isFlagReadyToInstalled = true;
        _clickTracker.ClickHappened -= OnClickHappened;
        FlagReadyToInstalled?.Invoke();
    }
}