using DigitalRuby.RainMaker;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeatherController : MonoBehaviour
{
    public static WeatherController Instance { get; private set; }

    public enum WeatherState
    {
        Clear,
        Sunny,
        Rain
    }

    private WeatherState state;
    [SerializeField] private RealtimeSunController timeController;
    [SerializeField] private BaseRainScript rain;

    [SerializeField, Range(0, 1)]
    private float[] rainChancePerDay;
    private int consecutiveRainHours;
    private int currentDay;
    private bool hasWeatherChanged; // Needed so that the weather doesn't update multiple times at once
    private float skyChangeAmt;
    [SerializeField]
    private TMP_Text weatherText;

    // The current state of the weather. Get-only.
    public WeatherState CurrentState
    {
        get { return state; }
    }
    private void Awake()
    {
        if (Instance == null) { Instance = this; }
    }
    // Start is called before the first frame update
    void Start()
    {
        hasWeatherChanged = false;
        state = WeatherState.Clear;
        //RenderSettings.skybox = new Material(RenderSettings.skybox);
        SetWeatherText();
        skyChangeAmt = 0;
        consecutiveRainHours = -1;
    }

    // Update is called once per frame
    void Update()
    {
        // Update the weather every hour
        if (timeController.currentTime.Minute == 0 && !hasWeatherChanged) // Could be replaced with an event that goes out when a day changes
        {
            // If it's a new day, increment the current day as well
            if (timeController.currentTime.Hour == 0)
                IncrementCurrentDay();
            UpdateWeather();
            //Debug.Log(state.ToString());
        }
        else if (timeController.currentTime.Minute != 0 && hasWeatherChanged)
            hasWeatherChanged = false;

        //UpdateSkyColor();
    }

    private void IncrementCurrentDay()
    {
        currentDay = (currentDay + 1) % rainChancePerDay.Length;
        SetWeatherText();
    }

    private void UpdateWeather()
    {
        hasWeatherChanged = true;
        // If there's not a chance of rain on a given day, set the weather to be clear and return early
        if (rainChancePerDay[currentDay] == 0)
        {
            SetWeatherState(Random.value < 0.5f ? WeatherState.Clear : WeatherState.Sunny);
            return;
        }

        // If it's currently raining, depending on how long it's been raining, force it to rain for longer
        if (state == WeatherState.Rain && rainChancePerDay[currentDay] >= 0.5f)
        {
            int forcedRainHours = Mathf.CeilToInt((rainChancePerDay[currentDay] - 0.4f) * 10);
            if (forcedRainHours < consecutiveRainHours)
            {
                SetWeatherState(WeatherState.Rain);
                return;
            }
        }

        // Otherwise, set the weather based on the chance of rain
        float rand = Random.value;
        if (rand <= rainChancePerDay[currentDay])
            SetWeatherState(WeatherState.Rain);
        else
            SetWeatherState(Random.value < 0.5f ? WeatherState.Clear : WeatherState.Sunny);
    }

    public void SetWeatherState(WeatherState state)
    {
        this.state = state;
        if (state == WeatherState.Clear || state == WeatherState.Sunny)
        {
            rain.RainIntensity = 0;
            consecutiveRainHours = -1;
        }
        else
        {
            float rainChanceToday = rainChancePerDay[currentDay];
            // Scale how much it rains based on the chance of rain
            if (rainChanceToday >= 0.75f)
                rain.RainIntensity = 1;
            else
                rain.RainIntensity = Random.Range(
                    Mathf.Max(rainChanceToday - 0.25f, 0.1f), Mathf.Max(rainChanceToday + 0.25f, 1f));
            consecutiveRainHours++;
        }
    }

    private void SetWeatherText()
    {
        Debug.Log("Setting weather text");
        weatherText.text = "Weather Forecast\n" +
            $"Today: {(int)Mathf.Ceil(rainChancePerDay[currentDay] * 100)}% rain chance\n" +
            $"Tomorrow: {(int)Mathf.Ceil(rainChancePerDay[(currentDay + 1) % rainChancePerDay.Length] * 100)}% rain chance\n" +
            $"In 2 Days: {(int)Mathf.Ceil(rainChancePerDay[(currentDay + 2) % rainChancePerDay.Length] * 100)}% rain chance\n";

    }

    private void UpdateSkyColor()
    {
        float dt = Time.deltaTime;
        if (state == WeatherState.Rain && skyChangeAmt < 1)
            skyChangeAmt += dt;
        else if (state == WeatherState.Clear && skyChangeAmt > 0)
            skyChangeAmt -= dt;

        float skyExposure = Mathf.Lerp(1.3f, 0.5f, skyChangeAmt);
        float skyColorRGB = Mathf.Lerp(0.5f, 0f, skyChangeAmt);
        Color skyColor = new Color(skyColorRGB, skyColorRGB, skyColorRGB);
        RenderSettings.skybox.SetFloat("_Exposure", skyExposure);
        RenderSettings.skybox.SetColor("_SkyTint", skyColor);
    }
}
