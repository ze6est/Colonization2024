using System;
using UnityEngine;

public class Ground : MonoBehaviour
{
    [SerializeField] private float _deathZone = 2f;

    private Mesh _planeMesh;
    private float _extremePointX;
    private float _extremePointZ;

    private void Awake()
    {
        _planeMesh = gameObject.GetComponent<MeshFilter>().mesh;
        Bounds bounds = _planeMesh.bounds;
        _extremePointX = gameObject.transform.localScale.x * bounds.size.x / 2 - _deathZone;
        _extremePointZ = gameObject.transform.localScale.z * bounds.size.z / 2 - _deathZone;
    }

    public bool TrySetPosition(out Vector3 position, float flagRadius, LayerMask _interferencesMask)
    {
        Vector3 cursorPosition = GetCursorPosition();

        float positionX;
        float positionZ;

        if (Math.Abs(cursorPosition.x) > _extremePointX)
        {
            positionX = _extremePointX;

            if (cursorPosition.x < 0)
                positionX = -positionX;
        }
        else
            positionX = cursorPosition.x;

        if (Math.Abs(cursorPosition.z) > _extremePointZ)
        {
            positionZ = _extremePointZ;

            if (cursorPosition.z < 0)
                positionZ = -positionZ;
        }
        else
            positionZ = cursorPosition.z;

        position = new Vector3(positionX, 0, positionZ);

        if (SpawnPointInstaller.CheckPositionToFree(position, flagRadius, _interferencesMask))
        {
            position = Vector3.zero;
            return false;
        }
        else
        {
            return true;
        }
    }

    private Vector3 GetCursorPosition()
    {
        Vector3 cursorPositionOnPlane = Vector3.zero;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        float distance;

        if (plane.Raycast(ray, out distance))
        {
            cursorPositionOnPlane = ray.GetPoint(distance);
        }

        return cursorPositionOnPlane;
    }
}