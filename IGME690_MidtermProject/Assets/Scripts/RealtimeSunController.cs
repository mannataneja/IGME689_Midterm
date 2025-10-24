using Esri.ArcGISMapsSDK.Components;
using Esri.GameEngine.Geometry;
using System;
using UnityEngine;
using TMPro;

public class RealtimeSunController : MonoBehaviour
{
	[SerializeField] private ArcGISMapComponent map;

	[SerializeField] private TMP_Text timeText;
	[SerializeField] private bool useSimulatedTime;
	[SerializeField] private float startHour;
	[SerializeField] private float timeMultiplier;
	private DateTime currentTime;

	private float timePercent;
	private float earthAxialTiltRadians = 23.5f * Mathf.Deg2Rad;
	private float earthRadiansPerHour = 15f * Mathf.Deg2Rad;
	private float earthMinutesPerRadian = 4f / Mathf.Deg2Rad;

    private void Start()
    {
		currentTime = DateTime.Now.Date + TimeSpan.FromHours(startHour);
	}

    private void Update()
	{
        if (useSimulatedTime)
        {
			currentTime = currentTime.AddSeconds(Time.deltaTime * timeMultiplier);

			transform.localRotation = Quaternion.Euler(new Vector3((currentTime.Hour * 360f), 0f, 0f));

			timeText.text = currentTime.ToString("hh:mm");
		}
        else
        {
			// Get the current UTC time and map location
			DateTime nowLocal = DateTime.Now;
			Vector2 coords = new Vector2((float)map.OriginPosition.X, (float)map.OriginPosition.Y) * Mathf.Deg2Rad;

			// Sum up the total hours that it has been within the day as a decimal number
			// This allows for more accurate time conversions + faster real-time updating of the sun
			float hoursLocal = nowLocal.Hour;
			hoursLocal += nowLocal.Minute / 60f;
			hoursLocal += nowLocal.Second / 3600f;
			hoursLocal += nowLocal.Millisecond / 3600000f;

			float localStandardTimeMeridian = earthRadiansPerHour * (TimeZoneInfo.Local.GetUtcOffset(nowLocal).Hours);
			float b = (Mathf.PI * 2f / 365f) * (nowLocal.DayOfYear - 81);
			float equationOfTime = (9.87f * Mathf.Sin(b + b)) - (7.53f * Mathf.Cos(b)) - (1.5f * Mathf.Sin(b));
			float timeCorrectionFactor = earthMinutesPerRadian * (coords.x - localStandardTimeMeridian) + equationOfTime;
			float localSolarTime = hoursLocal + (timeCorrectionFactor / 60f);

			// Calculate the hour angle of the sun, which is the conversion of local solar time to radians moved across the sky
			float hourAngle = earthRadiansPerHour * (localSolarTime - 12);

			// Calculate the declination of the sun
			float declination = earthAxialTiltRadians * Mathf.Sin(b);

			// Calculate the elevation and azimuth angles of the sun in the sky
			float elevation = Mathf.Asin((Mathf.Sin(declination) * Mathf.Sin(coords.y)) + (Mathf.Cos(declination) * Mathf.Cos(coords.y) * Mathf.Cos(hourAngle)));
			float azimuth = Mathf.Acos(((Mathf.Sin(declination) * Mathf.Cos(coords.y)) - (Mathf.Cos(declination) * Mathf.Sin(coords.y) * Mathf.Cos(hourAngle))) / Mathf.Cos(elevation));

			// Adjust the azimuth angle due to the fact that I am using trigonometry functions with multiple solutions (like arcsin)
			if (localSolarTime > 12f)
			{
				azimuth = (Mathf.PI * 2f) - azimuth;
			}

			// Set the rotation of the directional light to match the sun's position in the sky
			transform.rotation = Quaternion.Euler(elevation * Mathf.Rad2Deg, -azimuth * Mathf.Rad2Deg, 0f);

			timeText.text = DateTime.Now.ToString("hh:mm");
		}

	}
}