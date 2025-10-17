using UnityEngine;

public class RoadSpawner : ArcGISFeatureLayerComponent
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(nameof(GetFeatures));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
