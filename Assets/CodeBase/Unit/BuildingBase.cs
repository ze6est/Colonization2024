using UnityEngine;

[RequireComponent(typeof(Unit))]
public class BuildingBase : MonoBehaviour
{
    private MovementToTarget _movement;
    private FlagInstaller _flagInstaller;
    private BaseUnits _baseUnits;
    private Coroutine _moveToTarget;
    private float _minDistanceToTargetForAction;

    private void Awake()
    {
        _movement = GetComponent<MovementToTarget>();
        _baseUnits = Resources.Load<BaseUnits>(PrefabsPath.BaseUnits);
    }

    public void SendUnit(FlagInstaller flagInstaller, float minDistanceToTargetForAction)
    {
        _flagInstaller = flagInstaller;        
        _minDistanceToTargetForAction = minDistanceToTargetForAction;
        _movement.TargetReached += OnTargetReached;
        _flagInstaller.PositionChanged += OnPositionChanged;

        _moveToTarget = StartCoroutine(_movement.MoveToTarget(flagInstaller.FlagPosition, _minDistanceToTargetForAction));        
    }    

    private void OnPositionChanged(Vector3 newPosition)
    {
        StopCoroutine(_moveToTarget);
        _moveToTarget = StartCoroutine(_movement.MoveToTarget(newPosition, _minDistanceToTargetForAction));
    }

    private void OnTargetReached()
    {
        Destroy(_flagInstaller.Flag.gameObject);
        SpawnBase(_baseUnits, _flagInstaller.FlagPosition);

        _movement.TargetReached -= OnTargetReached;
    }

    private void SpawnBase(BaseUnits baseUnits, Vector3 flagPosition)
    {
        _flagInstaller.PositionChanged -= OnPositionChanged;
        BaseUnits newBase = Instantiate(baseUnits, flagPosition, Quaternion.identity);

        UnitSpawner unitSpawner = newBase.GetComponent<UnitSpawner>();
        SenderConstructionBaseUnits senderConstructionBaseUnits = newBase.GetComponent<SenderConstructionBaseUnits>();
        FlagSpawner flagSpawner = newBase.GetComponent<FlagSpawner>();

        Unit unit = GetComponent<Unit>();

        MaxSpawnPointPosition maxSpawnPointPosition = _flagInstaller.GetComponent<MaxSpawnPointPosition>();

        newBase.Construct(_flagInstaller, unit);
        unitSpawner.Construct(maxSpawnPointPosition);
        senderConstructionBaseUnits.Construct(_flagInstaller);
        _flagInstaller.Construct(flagSpawner);
    }
}