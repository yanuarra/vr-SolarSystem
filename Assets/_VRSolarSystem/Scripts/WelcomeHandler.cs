using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WelcomeHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject overlay_Welcome;

    private event Action BackEvent;

    private event Action NextEvent;

    public static WelcomeHandler Instance { get; private set; }

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
        overlay_Welcome.SetActive(false);
    }

    public void InitHandler(Action next = null, Action back = null)
    {
        NextEvent = next;
        BackEvent = back;
        PanelHandler.Instance.SetNewCurrentOverlay(overlay_Welcome);
        BackHandler.Instance.AssignBackEvent(BackEvent);
        Debug.Log("welcome");
    }

    public void Next()
    {
        if (NextEvent != null)
        {
            NextEvent.Invoke();
        }
    }

    public void BackToLogin()
    {
        //
    }
}