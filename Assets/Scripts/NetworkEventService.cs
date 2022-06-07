using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class NetworkEventService : MonoBehaviour, INetworkEventService
{
    private List<Event> cachedEvents = new List<Event>();

    private bool isColdDown;
    private bool isSendingData;
    private float cooldownTimeLeft;

    private void Start()
    {
        EventsSaveSystem.Init();

        if (EventsSaveSystem.ContainsAnyFiles())
        {
            var jsonEventsFiles = EventsSaveSystem.Load();

            string jsonEvents = string.Join(",", jsonEventsFiles);

            StartCoroutine(SendDataToAnalytics(jsonEvents));

            EventsSaveSystem.RemoveAll();
        }

        isColdDown = false;

        //TrackEvent("startGame", "player 1, exp 0");
        //TrackEvent("startGame", "player 2, exp 1");
    }

    private void Update()
    {
        cooldownTimeLeft -= Time.deltaTime;

        if (isColdDown && !isSendingData && cooldownTimeLeft <= 0f)
        {
            isColdDown = false;

            string jsonEvents = JsonConvert.SerializeObject(new Events(cachedEvents));
            Debug.Log(jsonEvents);

            StartCoroutine(SendDataToAnalytics(jsonEvents));
        }
    }

    public void TrackEvent(string type, string data)
    {
        cachedEvents.Add(new Event(type, data));

        if (!isColdDown)
        {
            isColdDown = true;
            cooldownTimeLeft = NetworkConstants.COOLDOWN_BEFORE_SEND;
        }
    }

    private IEnumerator SendDataToAnalytics(string data)
    {
        isSendingData = true;

        var www = UnityWebRequest.Post(NetworkConstants.SERVER_URL, data);

        yield return www.SendWebRequest();

        Debug.Log(www.result);

        if (www.result != UnityWebRequest.Result.Success)
        {
            EventsSaveSystem.Save(data);
        }

        isSendingData = false;
        cooldownTimeLeft = NetworkConstants.COOLDOWN_BEFORE_SEND;
        cachedEvents = null;

        www.Dispose();
    }
}
