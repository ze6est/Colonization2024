using UnityEngine;

public static class SpawnPointInstaller
{
    public static bool TrySetPosition(out Vector3 spawnPosition, float maxSpawnPointPositionX, float minSpawnPointPositionX,
        float maxSpawnPointPositionZ, float minSpawnPointPositionZ, float targetRadius, LayerMask interferencesMask)
    {
        float spawnPozitionX = Random.Range(maxSpawnPointPositionX, minSpawnPointPositionX);
        float spawnPozitionZ = Random.Range(maxSpawnPointPositionZ, minSpawnPointPositionZ);

        spawnPosition = new Vector3(spawnPozitionX, 0, spawnPozitionZ);

        return CheckPositionToFree(spawnPosition, targetRadius, interferencesMask);
    }

    public static bool CheckPositionToFree(Vector3 spawnPosition, float targetRadius, LayerMask interferencesMask)
    {
        bool isPositionOccupied = Physics.CheckSphere(spawnPosition, targetRadius, interferencesMask);

        return isPositionOccupied;
    }
}