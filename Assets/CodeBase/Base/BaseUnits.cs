using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(UnitSpawner))]
[RequireComponent(typeof(SenderForResources))]
[RequireComponent(typeof(FlagSpawner))]
[RequireComponent(typeof(SenderConstructionBaseUnits))]
public class BaseUnits : MonoBehaviour
{
    [SerializeField] private FlagInstaller _flagInstaller;

    private UnitSpawner _unitSpawner;
    private SenderForResources _senderForResources;
    private FlagSpawner _flagSpawner;
    private SenderConstructionBaseUnits _senderConstructionBaseUnits;
    [SerializeField] private List<Unit> _freeUnits = new List<Unit>();
    [SerializeField] private List<Unit> _occupiedUnits = new List<Unit>();    

    public void Construct(FlagInstaller flagInstaller, Unit unit)
    {
        _flagInstaller = flagInstaller;
        _freeUnits.Add(unit);
    }

    private void Awake()
    {        
        _unitSpawner = GetComponent<UnitSpawner>();
        _senderForResources = GetComponent<SenderForResources>();
        _flagSpawner = GetComponent<FlagSpawner>();
        _senderConstructionBaseUnits = GetComponent<SenderConstructionBaseUnits>();
    }

    private void OnEnable()
    {
        _unitSpawner.UnitCreated += OnUnitCreated;
        _senderForResources.UnitReleased += OnUnitReleased;
        _flagSpawner.FlagReadyToInstalled += OnFlagReadyToInstalled;
    }    

    private void OnDisable()
    {
        _unitSpawner.UnitCreated -= OnUnitCreated;
        _senderForResources.UnitReleased -= OnUnitReleased;
    }

    public bool TryGetFreeUnit(out Unit _currentFreeUnit, bool isSpawnUnits = true)
    {
        if (_freeUnits.Count == 0)
        {
            _currentFreeUnit = null;
            return false;
        }

        _currentFreeUnit = _freeUnits.First();
        _freeUnits.Remove(_currentFreeUnit);

        if(isSpawnUnits)
        {            
            _occupiedUnits.Add(_currentFreeUnit);
        }

        return true;
    }    

    private void OnUnitCreated(Unit unit)
    {
        _freeUnits.Add(unit);
    }

    private void OnUnitReleased(Unit releasedUnit)
    {
        Unit freeUnit = _occupiedUnits.First(unit => unit.Number == releasedUnit.Number);

        _occupiedUnits.Remove(freeUnit);
        _freeUnits.Add(freeUnit);
    }

    private void OnFlagReadyToInstalled()
    {
        _flagInstaller.FlagInstalled += OnFlagInstalled;
    }

    private void OnFlagInstalled()
    {
        _flagSpawner.FlagReadyToInstalled -= OnFlagReadyToInstalled;
        _flagInstaller.FlagInstalled -= OnFlagInstalled;
        _unitSpawner.enabled = false;
        _senderConstructionBaseUnits.Enable();
        _senderConstructionBaseUnits.UnitForBuildingBaseSent += OnUnitForBuildingBaseSent;
    }

    private void OnUnitForBuildingBaseSent()
    {
        _senderConstructionBaseUnits.UnitForBuildingBaseSent -= OnUnitForBuildingBaseSent;        
        _unitSpawner.enabled = true;
    }
}