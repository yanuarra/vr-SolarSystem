using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnBoarding : MonoBehaviour
{
    [SerializeField]
    private GameObject overlayBokeh_onBoarding;

    [SerializeField]
    private GameObject overlay_onBoarding;

    private event Action BackEvent;

    private event Action NextEvent;

    public static OnBoarding Instance { get; private set; }

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
        overlay_onBoarding.SetActive(true);
        //overlayBokeh_onBoarding.SetActive(true);
    }

    public void InitHandler(Action next = null, Action back = null)
    {
        NextEvent = next;
        BackEvent = back;
        PanelHandler.Instance.SetNewCurrentOverlay(overlay_onBoarding);
        PanelHandler.Instance.SetNewCurrentOverlay(overlayBokeh_onBoarding);
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
}