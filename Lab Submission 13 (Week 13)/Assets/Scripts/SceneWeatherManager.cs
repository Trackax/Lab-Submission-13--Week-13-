using System;
using System.Globalization;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class SceneWeatherManager : MonoBehaviour
{
    private WeatherManager weatherManager = new WeatherManager();
    public Material skyboxMaterial;
    public Light sunLight;
    public TextMeshProUGUI cityInputField;
    public Button updateWeatherButton;
    public RawImage internetImageDisplay; // Reference to a RawImage component in UI
    public string[] webImageUrls = { "https://upload.wikimedia.org/wikipedia/commons/thumb/1/15/Cat_August_2010-4.jpg/2560px-Cat_August_2010-4.jpg", "https://upload.wikimedia.org/wikipedia/commons/d/d5/Retriever_in_water.jpg", "https://upload.wikimedia.org/wikipedia/commons/6/68/Sciuridae.jpg" };

    void Start()
    {
        StartCoroutine(weatherManager.GetWeatherXML(OnXMLDataLoaded));
        if (webImageUrls != null && webImageUrls.Length > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, webImageUrls.Length);
            string selectedImageUrl = webImageUrls[randomIndex];
            StartCoroutine(DownloadImage(selectedImageUrl, OnImageDownloaded));
        }
    }

    public void OnCityUpdateClicked()
    {
        if (cityInputField != null && !string.IsNullOrEmpty(cityInputField.text))
        {
            string newCity = cityInputField.text;
            weatherManager.SetCity(newCity);
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
        XmlNode weatherNode = xmlDoc.SelectSingleNode("//weather");
        string weatherValue = weatherNode.Attributes["value"].Value;
        XmlNode tempNode = xmlDoc.SelectSingleNode("//temperature");
        float tempKelvin = float.Parse(tempNode.Attributes["value"].Value, CultureInfo.InvariantCulture);
        XmlNode sunNode = xmlDoc.SelectSingleNode("//sun");
        DateTime sunrise = DateTime.Parse(sunNode.Attributes["rise"].Value, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
        DateTime sunset = DateTime.Parse(sunNode.Attributes["set"].Value, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
        UpdateScene(weatherValue, tempKelvin, sunrise, sunset);
    }

    private void UpdateScene(string weatherCondition, float tempKelvin, DateTime sunrise, DateTime sunset)
    {
        if (weatherCondition.Contains("rain") || weatherCondition.Contains("cloud"))
        {
            skyboxMaterial = Resources.Load<Material>("CloudySkybox");
        }
        else
        {
            skyboxMaterial = Resources.Load<Material>("ClearSkybox");
        }
        RenderSettings.skybox = skyboxMaterial;
        float tempCelsius = tempKelvin - 273.15f;
        sunLight.color = Color.Lerp(Color.blue, Color.red, Mathf.InverseLerp(0, 40, tempCelsius));
        sunLight.intensity = Mathf.Lerp(0.5f, 1.5f, Mathf.InverseLerp(0, 40, tempCelsius));
        DateTime currentTimeUTC = DateTime.UtcNow;
    }

    public IEnumerator DownloadImage(string imageUrl, Action<Texture2D> callback)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Image download error: {request.error}");
        }
        else
        {
            callback(DownloadHandlerTexture.GetContent(request));
        }
    }

    private void OnImageDownloaded(Texture2D texture)
    {
        if (internetImageDisplay != null && texture != null)
        {
            internetImageDisplay.texture = texture;
            internetImageDisplay.rectTransform.sizeDelta = new Vector2(200f, 150f);
        }
    }
}
