using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Esri.ArcGISMapsSDK.Components;
using Esri.GameEngine.Geometry;
using Esri.ArcGISMapsSDK.Utils.GeoCoord;
using Unity.Mathematics;
using TMPro;

public class Weather : MonoBehaviour
{
    public bool useSimulatedWeather;
    public TMP_Text weatherText;
    public WeatherController weatherController;
    public ArcGISMapComponent arcGISMap;

    public double latitude = 40.7128;
    public double longitude = -74.0060;

    [Header("OpenWeatherMap")]
    public string apiKey;

    [Header("Historical Test Settings")]
    public int year = 2023;
    public int month = 7;
    public int day = 21;
    public int hour = 15;

    private GameObject rainInstance;

    void Start()
    {
        if (!useSimulatedWeather)
        {
            StartCoroutine(UpdateWeather());
        }

        // Simulate historical heavy rain
        TestingRain();


    }

    double3 GeoToUnityPosition(double lat, double lon, float height)
    {
        ArcGISPoint point = new ArcGISPoint(lon, lat, height, ArcGISSpatialReference.WGS84());
        return arcGISMap.View.GeographicToWorld(point);
    }
    IEnumerator UpdateWeather()
    {
        while (true)
        {
            string url = $"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&appid={apiKey}&units=metric";
            Debug.Log(url);
            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                WeatherData data = JsonUtility.FromJson<WeatherData>(request.downloadHandler.text);
                UpdateRain(data);
                ShowSeaLevel(data);
                weatherText.text = "Temperature : " + data.main.temp;
            }
            else
            {
                Debug.LogWarning("Weather API request failed");
            }

            //Update every 10 minutes
            yield return new WaitForSeconds(600f);
        }
    }

    void UpdateRain(WeatherData data)
    {
        if (rainInstance == null) return;

        var emission = rainInstance.GetComponent<ParticleSystem>().emission;

        if (data.wind != null && data.wind._1h > 0)
        {
            /*Debug.Log("The rainfall is falling!");
            emission.rateOverTime = data.wind._1h * 50f;
            rainInstance.SetActive(true);*/
            weatherController.SetWeatherState(WeatherController.WeatherState.Rain);
        }
        else
        {
            /*Debug.Log("The rain isn't raining!");
            emission.rateOverTime = 0;
            rainInstance.SetActive(false);*/
            weatherController.SetWeatherState(WeatherController.WeatherState.Clear);
        }
    }
    void TestingRain()
    {
        float rainVolume = 10f;

        var emission = rainInstance.GetComponent<ParticleSystem>().emission;
        emission.rateOverTime = rainVolume * 50f;

        rainInstance.SetActive(true);

        Debug.Log($"Simulating heavy rain on {year}-{month}-{day} at {hour}:00 in NYC");
    }
    void ShowSeaLevel(WeatherData data)
    {
        Debug.Log("Sea Level: " + data.main.sea_level);
    }
    [System.Serializable]
    public class WeatherData
    {
        public WeatherMain main;
        public RainData wind;
    }

    [System.Serializable]
    public class RainData
    {
        public float _1h; // rain volume in mm
    }
    [System.Serializable]
    public class WeatherMain
    {
        public float temp;
        public float pressure;
        public float sea_level;
        public float grnd_level;
    }


}