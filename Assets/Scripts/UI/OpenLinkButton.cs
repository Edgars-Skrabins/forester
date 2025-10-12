using System;
using UnityEngine;

public class OpenLinkButton : MonoBehaviour
{
    [SerializeField] private string url = "";
    private UnityEngine.UI.Button openLinkButton;

    private void OnEnable()
    {
        if (openLinkButton == null)
        {
            openLinkButton = GetComponent<UnityEngine.UI.Button>();
            if (openLinkButton == null)
            {
                Debug.LogError("OpenLinkButton script must be attached to a GameObject with a Button component.");
                return;
            }
        }
        openLinkButton.onClick.AddListener(OpenLink);
    }
    
    private void OnDisable()
    {
        openLinkButton.onClick.RemoveListener(OpenLink);
    }

    private void OpenLink()
    {
        if (!string.IsNullOrEmpty(url))
        {
            Application.OpenURL(url);
        }
        else
        {
            Debug.LogWarning("URL is empty! Please assign one in the Inspector.");
        }
    }
}