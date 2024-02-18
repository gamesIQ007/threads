using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ImageWebRequest : MonoBehaviour
{
    [SerializeField] private Text text;
    [SerializeField] private Image image;

    public async void LoadAsync()
    {
        const string url = "https://animekabegami.com/image_wallpaper/1503408491_thumb.jpg";
        text.text = "Load...";

        UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url);

        await webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), 1, 1, SpriteMeshType.FullRect);

            image.sprite = sprite;

            text.text = "Done";
        }
        else
        {
            text.text = webRequest.result.ToString();
        }

        webRequest.Dispose();
    }
}
