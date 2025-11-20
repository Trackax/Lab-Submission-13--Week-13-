using System;
using System.Globalization;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SceneWeatherController : MonoBehaviour
{
    private WeatherManager weatherManager = new WeatherManager();
    public Material skyboxMaterial; // Drag your skybox material here in the Inspector
    public Light sunLight; // Drag your directional light here

    // UI elements
    public TextMeshProUGUI cityInputField;
    public Button updateWeatherButton;

    void Start()
    {
        // Initial fetch
        StartCoroutine(weatherManager.GetWeatherXML(OnXMLDataLoaded));

        // Setup UI listeners
        if (updateWeatherButton != null)
        {
            updateWeatherButton.onClick.AddListener(OnCityUpdateClicked);
        }
    }

    private void OnCityUpdateClicked()
    {
        if (cityInputField != null && !string.IsNullOrEmpty(cityInputField.text))
        {
            weatherManager.SetCity(cityInputField.text);
            StartCoroutine(weatherManager.GetWeatherXML(OnXMLDataLoaded));
        }
    }

    public void OnXMLDataLoaded(string xmlData)
    {
        Debug.Log("Received data: " + xmlData);
        ParseXmlData(xmlData);
    }

    private void ParseXmlData(string xmlData)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlData);

        // Extract weather description
        XmlNode weatherNode = xmlDoc.SelectSingleNode("//weather");
        string weatherValue = weatherNode.Attributes["value"].Value;

        // Extract temperature (in Kelvin)
        XmlNode tempNode = xmlDoc.SelectSingleNode("//temperature");
        float tempKelvin = float.Parse(tempNode.Attributes["value"].Value, CultureInfo.InvariantCulture);

        // Extract sunrise and sunset times (ISO 8601 format)
        XmlNode sunNode = xmlDoc.SelectSingleNode("//sun");
        DateTime sunrise = DateTime.Parse(sunNode.Attributes["rise"].Value, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
        DateTime sunset = DateTime.Parse(sunNode.Attributes["set"].Value, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

        UpdateScene(weatherValue, tempKelvin, sunrise, sunset);
    }

    private void UpdateScene(string weatherCondition, float tempKelvin, DateTime sunrise, DateTime sunset)
    {
        // Change skybox (based on condition, simple example)
        if (weatherCondition.Contains("rain") || weatherCondition.Contains("cloud"))
        {
            // Assign a cloudy/rainy skybox material
            // skyboxMaterial = Resources.Load<Material>("CloudySkybox"); 
        }
        else
        {
            // Assign a clear skybox material
            // skyboxMaterial = Resources.Load<Material>("ClearSkybox");
        }
        // RenderSettings.skybox = skyboxMaterial;

        // Change sun intensity and color based on temperature
        // Lower temperature (colder) might have a cooler sun color
        // Example: map tempKelvin (e.g., 273K to 310K) to a color gradient
        float tempCelsius = tempKelvin - 273.15f;
        sunLight.color = Color.Lerp(Color.blue, Color.red, Mathf.InverseLerp(0, 40, tempCelsius));
        sunLight.intensity = Mathf.Lerp(0.5f, 1.5f, Mathf.InverseLerp(0, 40, tempCelsius));

        // Day/night cycle based on real time and city's sunrise/sunset
        DateTime currentTimeUTC = DateTime.UtcNow;
        // Calculate a time percentage between sunrise and sunset for scene rotation/lighting
        // This part needs more complex logic to simulate full day/night cycle based on specific times
        // A simple approach is to use the current system time to determine the rotation of the directional light or the skybox
    }
}