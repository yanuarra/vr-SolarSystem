using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class UIDisplayHandler : MonoBehaviour
{
    public static UIDisplayHandler Instance;
    [SerializeField] GameObject planetListOverlay;
    [SerializeField] LitePlanetObj planetInfoOverlay;
    [SerializeField] LazyFollow follow;
    [SerializeField] CanvasFader fader;
    public bool _stateWorldUI = false;
    public bool _stateHandUI = false;

    private void Awake()
    {
        SetAsSingleton();
    }

    private void SetAsSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        DoFadeCanvasGroup(planetListOverlay, _stateHandUI, 0f);
        DoFadeCanvasGroup(planetInfoOverlay.gameObject, _stateWorldUI, 0f);
        planetInfoOverlay.gameObject.SetActive(_stateWorldUI);
        planetListOverlay.SetActive(_stateWorldUI);
        //planetInfoOverlay.overlayPlanetObj.SetActive(_stateWorldUI);
    }

    public void FollowCamera()
    {
        follow.enabled = !follow.isActiveAndEnabled;
    }

    public void TogglePlanetListOverlay(Action act = null)
    {
        if (!planetListOverlay.activeInHierarchy)
            planetListOverlay.SetActive(true);
        _stateHandUI = !_stateHandUI;
        if (_stateHandUI)
            GameController.Instance.RecenterUI();
        DoFadeCanvasGroup(planetListOverlay, _stateHandUI, .5f, act);
    }

    public void TogglePlanetInfoOverlay()
    {
        if (!planetInfoOverlay.gameObject.activeInHierarchy)
            planetInfoOverlay.gameObject.SetActive(true);
        _stateWorldUI = !_stateWorldUI;
        DoFadeCanvasGroup(planetInfoOverlay.gameObject, _stateWorldUI, .5f);
        GameController.Instance.TogglePopUp(_stateWorldUI);
        GameController.Instance.ToggleOrbitRender(_stateWorldUI);
    }

    public void ToggleOrbitRender()
    {
        GameController.Instance.ToggleOrbitRender(_stateWorldUI);
    }

    public void DoFadeCanvasGroup(GameObject go, bool state, float time, Action act = null)
    {
        float target = state ? 1 : 0;
          CanvasGroup sr = go.GetComponentInChildren<CanvasGroup>();
        if (sr != null)
            LeanTween.alphaCanvas(sr, target, time).setOnComplete(act);
    }
}
