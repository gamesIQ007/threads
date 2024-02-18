using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TextWebRequest : MonoBehaviour
{
    [SerializeField] private Text text;

    public async void LoadText()
    {
        const string url = "https://www.ya.ru/";
        text.text = "Load...";

        UnityWebRequest webRequest = UnityWebRequest.Get(url);

        await webRequest.SendWebRequest();

        string result = webRequest.downloadHandler.text;

        text.text = result;

        webRequest.Dispose();
    }
}
