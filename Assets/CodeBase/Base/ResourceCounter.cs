using UnityEngine;

[RequireComponent(typeof(SenderForResources))]
public class ResourceCounter : MonoBehaviour
{
    [SerializeField] private int _countResourcesAtStart;

    ResourceCounterView _view;
    private SenderForResources _senderForResources;    
    private int _countCollectedResources;

    private void Awake()
    {
        _senderForResources = GetComponent<SenderForResources>();
        _view = GetComponentInChildren<ResourceCounterView>();
        _countCollectedResources += _countResourcesAtStart;
    }

    private void OnEnable()
    {
        _senderForResources.ResourceCollected += OnResourceCollected;        
    }    

    private void Start()
    {
        RefreshText();
    }

    private void OnDisable()
    {
        _senderForResources.ResourceCollected -= OnResourceCollected;        
    }

    public bool TryReduceResources(int count)
    {
        if(count < 0)
            return false;

        if(count > _countCollectedResources)
            return false;

        _countCollectedResources -= count;
        RefreshText();
        return true;
    }

    private void OnResourceCollected()
    {
        ChangeCountResource(1);
    }    

    private void ChangeCountResource(int count)
    {
        _countCollectedResources += count;

        RefreshText();            
    }

    private void RefreshText()
    {
        _view.RefreshText(_countCollectedResources);
    }
}