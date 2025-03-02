using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationHandler : MonoBehaviour
{
    [SerializeField]
    private TextVerticalFitter messageText;

    [SerializeField]
    private TextVerticalFitter titleText;

    [SerializeField]
    private GameObject spaceTitle;

    [SerializeField]
    private GameObject objTitle;

    [SerializeField]
    private GameObject overlay_Confirmation;

    [SerializeField]
    private GameObject buttonPlay;

    [SerializeField]
    private GameObject buttonYesNo;

    private event Action YesEvent;

    private event Action NoEvent;

    public static ConfirmationHandler Instance { get; private set; }

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
        overlay_Confirmation.SetActive(false);
    }

    public void ActivePanel(string message, Action yes = null, Action no = null)
    {
        PanelHandler.Instance.DeactiveCurrentOverlay();
        overlay_Confirmation.SetActive(true);
        spaceTitle.SetActive(false);
        objTitle.SetActive(false);

        buttonPlay.SetActive(false);
        buttonYesNo.SetActive(true);

        YesEvent = yes;
        NoEvent = no;
        messageText.SetText(message);
    }

    public void ActivePanel(string title, string message, Action yes = null, Action no = null)
    {
        PanelHandler.Instance.DeactiveCurrentOverlay();
        overlay_Confirmation.SetActive(true);
        spaceTitle.SetActive(true);
        objTitle.SetActive(true);

        buttonPlay.SetActive(false);
        buttonYesNo.SetActive(true);

        YesEvent = yes;
        NoEvent = no;
        titleText.SetText(title);
        messageText.SetText(message);
    }

    public void ActivePanelPlay(string title, string message, Action yes = null, Action no = null)
    {
        PanelHandler.Instance.DeactiveCurrentOverlay();
        overlay_Confirmation.SetActive(true);
        spaceTitle.SetActive(true);
        objTitle.SetActive(true);

        buttonPlay.SetActive(true);
        buttonYesNo.SetActive(false);

        YesEvent = yes;
        NoEvent = no;
        titleText.SetText(title);
        messageText.SetText(message);
    }

    public void Yes()
    {
        if (YesEvent != null)
        {
            YesEvent.Invoke();
        }
        overlay_Confirmation.SetActive(false);
        Debug.Log("Yes");
    }

    public void No()
    {
        overlay_Confirmation.SetActive(false);
        if (NoEvent != null)
        {
            NoEvent.Invoke();
        }
        if (BackHandler.Instance.IsBackEventNull())
        {
            PanelHandler.Instance.ActiveCurrentOverlay();
        }
        Debug.Log("NO");
    }

    public bool IsOverlayActive()
    {
        return overlay_Confirmation.activeSelf;
    }
}