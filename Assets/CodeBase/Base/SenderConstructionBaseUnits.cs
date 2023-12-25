using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ResourceCounter))]
public class SenderConstructionBaseUnits : MonoBehaviour
{
    [SerializeField] private FlagInstaller _flagInstaller;
    [SerializeField] private float _offsetToTargetForAction;
    [SerializeField] private int _costSendingUnitToBuildBase = 5;

    private Unit _currentFreeUnit;
    private BaseUnits _baseUnits;
    private ResourceCounter _counter;
    private float _reduce = 0.5f;

    public event UnityAction UnitForBuildingBaseSent;    

    public void Construct(FlagInstaller flagInstaller)
    {
        _flagInstaller = flagInstaller;
    }

    private void Awake()
    {
        _baseUnits = GetComponent<BaseUnits>();
        _counter = GetComponent<ResourceCounter>();
    }

    public void Enable()
    {
        StartCoroutine(SendUnitConstructionBaseUnits());
    }

    private IEnumerator SendUnitConstructionBaseUnits()
    {
        WaitForSeconds wait = new WaitForSeconds(_reduce);

        while (!_counter.TryReduceResources(_costSendingUnitToBuildBase))
            yield return wait;

        yield return StartCoroutine(FindFreeUnit());        

        float minDistanceToTargetForAction = _currentFreeUnit.Radius + _flagInstaller.Flag.Radius + _offsetToTargetForAction;        

        BuildingBase buildingBase = _currentFreeUnit.GetComponent<BuildingBase>();
        buildingBase.SendUnit(_flagInstaller, minDistanceToTargetForAction);        

        UnitForBuildingBaseSent?.Invoke();
    }    

    private IEnumerator FindFreeUnit()
    {
        while (!_baseUnits.TryGetFreeUnit(out _currentFreeUnit, false))
        {
            yield return null;
        }
    }    
}