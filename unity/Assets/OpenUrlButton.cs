using UnityEngine;

public class OpenUrlButton : MonoBehaviour
{
    [Tooltip("Full URL (include http:// or https://)")]
    public string url = "https://example.com";

    // Hook this to a UI Button's OnClick()
    public void OpenLink()
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            Debug.LogWarning("URL is empty.");
            return;
        }
        Application.OpenURL(url);
    }
}
