using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MaxSpawnPointPosition))]
public class ResourceSpawner : MonoBehaviour
{
    [SerializeField] private float _spawnCheckRadiusResource;
    [SerializeField] private LayerMask _interferencesMask;
    [SerializeField] private float _duration;

    private MaxSpawnPointPosition _ground;
    private Resource _resource;    
    private bool _isGameWorked;

    private void Awake()
    {        
        _resource = Resources.Load(PrefabsPath.Resource).GetComponent<Resource>();
        _ground = GetComponent<MaxSpawnPointPosition>();
    }

    private void Start()
    {
        _isGameWorked = true;

        StartCoroutine(SpawnResource());
    }    
    
    private IEnumerator SpawnResource()
    {
        WaitForSeconds waitTime = new WaitForSeconds(_duration);
        
        while (_isGameWorked)
        {
            Vector3 spawnPosition;
            bool isPositionOccupied = SpawnPointInstaller.TrySetPosition(out spawnPosition, _ground.X, -_ground.X, _ground.Z, -_ground.Z,
                _spawnCheckRadiusResource, _interferencesMask);

            if (!isPositionOccupied)
                Instantiate(_resource, spawnPosition, Quaternion.identity);

            yield return waitTime;
        }        
    }    
}