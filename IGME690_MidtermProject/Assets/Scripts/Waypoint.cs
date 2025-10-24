using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Esri.ArcGISMapsSDK.Components;
using Esri.ArcGISMapsSDK.Utils.GeoCoord;
using Esri.GameEngine.Geometry;

public class Waypoint : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(StartingUp());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private IEnumerator StartingUp()
    {
        yield return new WaitForSeconds(1);
        GetComponent<ArcGISLocationComponent>().enabled = false;
        float randomOffsetX = UnityEngine.Random.Range(10, 15);
        float randomOffsetZ = UnityEngine.Random.Range(10, 15);

        // Randomly flip sign (so offset can go in any direction)
        if (UnityEngine.Random.value > 0.5f) randomOffsetX *= -1;
        if (UnityEngine.Random.value > 0.5f) randomOffsetZ *= -1;

        // Create the offset vector
        Vector3 offset = new Vector3(randomOffsetX, 0f, randomOffsetZ);

        transform.position += offset;
    }
}
