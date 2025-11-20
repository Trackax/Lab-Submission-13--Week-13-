using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Xml.Linq;
using System.Linq;

public class WeatherManager
{
    private const string BaseApiUrl = "http://api.openweathermap.org/data/2.5/weather?mode=xml";
    private string apiKey = "014e430148b0a0cccd26324159160984";
    private string city = "Orlando,us";

    public void SetCity(string newCity)
    {
        city = newCity;
    }

    private IEnumerator CallAPI(string url, Action<string> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {request.error}");
            }
            else
            {
                callback(request.downloadHandler.text);
            }
        }
    }

    public IEnumerator GetWeatherXML(Action<string> callback)
    {
        string url = $"{BaseApiUrl}&q={city}&appid={apiKey}";
        return CallAPI(url, callback);
    }
}