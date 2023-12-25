using UnityEngine;

[RequireComponent(typeof(ClickTracker))]
public class ColorChanger : MonoBehaviour
{
    private const string EmissionColor = "_EmissionColor";
    private Material _material;
    private ClickTracker _clickTracker;

    private void Awake()
    {
        _material = GetComponentInChildren<Renderer>().material;
        _clickTracker = GetComponent<ClickTracker>();
    }

    private void OnEnable()
    {
        _clickTracker.ClickHappened += OnClickHappened;
    }

    private void OnClickHappened()
    {
        _material.SetColor(EmissionColor, Color.white);
        _clickTracker.ClickHappened -= OnClickHappened;
    }    
}