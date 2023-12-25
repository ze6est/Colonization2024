using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ResourceCounter))]
public class UnitSpawner : MonoBehaviour
{
    [SerializeField] private MaxSpawnPointPosition _maxSpawnPointPosition;
    [SerializeField] private int _countUnitsAtStart = 3;
    [SerializeField] private float _maxSpawnRadius = 1.5f;
    [SerializeField] private float _spawnCheckRadiusUnit = 1f;
    [SerializeField] private int _costUnit = 3;
    [SerializeField] private LayerMask _interferencesMask;

    private ResourceCounter _counter;
    private Unit _unit;
    private float _delay = 1f;    
    private Coroutine _spawnUnitForResourcesJob;
    private bool _isUnitCreated;

    public event UnityAction<Unit> UnitCreated;

    public void Construct(MaxSpawnPointPosition maxSpawnPointPosition)
    {
        _maxSpawnPointPosition = maxSpawnPointPosition;

        Enable();
    }

    private void Awake()
    {
        _unit = Resources.Load(PrefabsPath.Unit).GetComponent<Unit>();
        _counter = GetComponent<ResourceCounter>();
    }

    private void OnEnable()
    {
        _spawnUnitForResourcesJob = StartCoroutine(SpawnUnitForResources());
    }

    private void Start()
    {
        if (_maxSpawnPointPosition != null)
            Enable();
    }

    private void OnDisable()
    {
        StopCoroutine(_spawnUnitForResourcesJob);
    }

    private void Enable()
    {
        StartCoroutine(SpawnUnits());
    }

    private IEnumerator SpawnUnits()
    {
        for (int i = 0; i < _countUnitsAtStart; i++)
        {
            yield return StartCoroutine(SpawnUnit());
        }        
    }

    private IEnumerator SpawnUnit()
    {
        float maxPositionX = transform.position.x + _maxSpawnRadius;
        float minPositionX = transform.position.x - _maxSpawnRadius;
        float maxPositionZ = transform.position.z + _maxSpawnRadius;
        float minPozitionZ = transform.position.z - _maxSpawnRadius;

        float maxSpawnPointPositionX = maxPositionX < _maxSpawnPointPosition.X ? maxPositionX : _maxSpawnPointPosition.X;
        float minSpawnPointPositionX = minPositionX < -_maxSpawnPointPosition.X ? -_maxSpawnPointPosition.X : minPositionX;
        float maxSpawnPointPositionZ = maxPositionZ < _maxSpawnPointPosition.Z ? maxPositionZ : _maxSpawnPointPosition.Z;
        float minSpawnPointPositionZ = minPozitionZ < -_maxSpawnPointPosition.Z ? -_maxSpawnPointPosition.Z : minPozitionZ;

        bool isPositionOccupied = true;

        while (isPositionOccupied)
        {
            Vector3 spawnPosition;
            isPositionOccupied = SpawnPointInstaller.TrySetPosition(out spawnPosition, maxSpawnPointPositionX, minSpawnPointPositionX,
                maxSpawnPointPositionZ, minSpawnPointPositionZ, _spawnCheckRadiusUnit, _interferencesMask);

            if (!isPositionOccupied)
            {
                Unit unit = Instantiate(_unit, spawnPosition, Quaternion.identity);
                _isUnitCreated = true;
                UnitCreated?.Invoke(unit);
            }

            yield return null;
        }        
    }

    private IEnumerator SpawnUnitForResources()
    {
        _isUnitCreated = true;
        bool isUnitsSpawn = true;

        WaitForSeconds duration = new WaitForSeconds(_delay);

        while (isUnitsSpawn)
        {
            yield return duration;
            if (_isUnitCreated)
            {
                if (_counter.TryReduceResources(_costUnit))
                {
                    _isUnitCreated = false;
                    yield return StartCoroutine(SpawnUnit());
                }
            }                
        }        
    }
}