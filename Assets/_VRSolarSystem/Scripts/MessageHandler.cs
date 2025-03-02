using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageHandler : MonoBehaviour
{
    [SerializeField]
    private Text messageText;

    [SerializeField]
    private RectTransform messageRect;

    [SerializeField]
    private GameObject overlay_Message;

    private event Action OkEvent;

    private float DefaultRectHeigth;
    private float TextRowHeigth;

    public static MessageHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        DefaultRectHeigth = messageRect.sizeDelta.y;
        TextRowHeigth = GetMessageRectHeight("");
        overlay_Message.SetActive(false);
    }

    public void ActivePanel(string message, Action ok = null)
    {
        PanelHandler.Instance.DeactiveCurrentOverlay();
        int rowAmount = Mathf.CeilToInt(GetMessageRectHeight(message) / TextRowHeigth);
        overlay_Message.SetActive(true);
        OkEvent = ok;
        messageText.text = message;
        messageRect.sizeDelta = new Vector2(messageRect.sizeDelta.x, DefaultRectHeigth * rowAmount);
    }

    public float GetMessageRectHeight(string s)
    {
        TextGenerator textGen = new TextGenerator();
        Vector2 contentSize = new Vector2(messageText.rectTransform.rect.size.x, messageText.rectTransform.rect.size.y);
        TextGenerationSettings generationSettings = messageText.GetGenerationSettings(contentSize);
        float height = Mathf.Floor(textGen.GetPreferredHeight(s, generationSettings));
        return height;
    }

    public void Ok()
    {
        overlay_Message.SetActive(false);
        if (OkEvent != null)
        {
            OkEvent.Invoke();
        }
    }

    public bool IsOverlayActive()
    {
        return overlay_Message.activeSelf;
    }
}