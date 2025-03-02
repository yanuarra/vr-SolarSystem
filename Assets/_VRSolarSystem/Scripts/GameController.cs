using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    //[SerializeField] private CanvasGroup rec;
    [SerializeField] private XROrigin _origin;
    [SerializeField] private Transform _originUIOffset;
    [SerializeField] private ScreenFader _screenFader;
    [SerializeField] private UIDisplayHandler _display;
    [SerializeField] private Transform _teleportPoint;
    [SerializeField] private Transform _UIPoint;
    [SerializeField] private Transform _UIPointPlayer;
    private Planet _curplanet;
    [SerializeField] private List<Planet> _colPlanet = new List<Planet>();
    [SerializeField] private List<PopUpNameObj> _colPopUp = new List<PopUpNameObj>();
    [SerializeField] private List<OrbitRenderer> _colOrbit = new List<OrbitRenderer>();
    [SerializeField] private List<Button> _colSelector = new List<Button>();
    [SerializeField] private GameObject _selectorPanel;
    [SerializeField] private GameObject _selectorPrefab;
    [SerializeField] private GameObject _popUpNamePrefab;
    [SerializeField] private GameObject _UI;
    [SerializeField] private PlanetObj _planetObj;
    [SerializeField] private LitePlanetObj _liteplanetObj;
    [SerializeField] private GameObject _light;
    [SerializeField] private GameObject recticle;
    private Transform _oriTransform;
    private Camera cam;
    [SerializeField] private XRInteractorLineVisual[] rayRender;
    [SerializeField] private XRRayInteractor[] rayInteract;
    [SerializeField] private float UIOffsetDistance;

    bool _isFocusOnPlanet = false;
    bool _isMoving = false;

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
        StartCoroutine(Initialize());
        cam = Camera.main;
    }

    public void TogglePlanetInteractable(bool state)
    {
        foreach (var item in _colPlanet)
        {
            PlanetInteraction pi = item.gameObject.GetComponent<PlanetInteraction>();
            pi.ToggleInteractable(state);
        }
    }

    private IEnumerator Initialize()
    {
        yield return new WaitForEndOfFrame();
        if (_screenFader == null)
            _screenFader = FindAnyObjectByType<ScreenFader>();
        if (_planetObj == null)
            _planetObj = FindAnyObjectByType<PlanetObj>();
        if (_liteplanetObj == null)
            _liteplanetObj = FindAnyObjectByType<LitePlanetObj>();
        PopulateSelector();
        GameObject oriGO = new GameObject("origin");
        oriGO.transform.position = _origin.transform.position;
        _oriTransform = oriGO.transform;
        foreach (var item in _colPlanet)
        {
            yield return item.Init();
        }
        foreach (var item in _colPlanet)
        {
            SpawnPopUpPlanetName(item);
        }
        ToggleOrbitRender(false);
        TogglePlanetInteractable(false);
        rayRender = _origin.GetComponentsInChildren<XRInteractorLineVisual>();
        rayInteract = _origin.GetComponentsInChildren<XRRayInteractor>();
    }

    void ToggleXRLineRenderer(bool state)
    {
        if (rayRender == null) return;
        if (rayRender.Length <= 0)
            rayRender = _origin.GetComponentsInChildren<XRInteractorLineVisual>();
        foreach (var item in rayRender)
        {
            item.enabled = state;
        }
    }

    void ToggleXRRay(bool state)
    {
        if (rayInteract == null) return;
        if (rayInteract.Length <= 0)
            rayInteract = _origin.GetComponentsInChildren<XRRayInteractor>();
        foreach (var item in rayInteract)
        {
            item.enabled = state;
        }
    }

    private IEnumerator ResetPlanet()
    {
        yield return new WaitForEndOfFrame();
        _isFocusOnPlanet = false;
        //_liteplanetObj.overlayPlanetObj.SetActive(false);
        AudioHandler.Instance.StopVoPlanet();
        ToggleXRLineRenderer(false);
        ToggleXRRay(false);
        if (UIDisplayHandler.Instance._stateWorldUI)
            UIDisplayHandler.Instance.TogglePlanetInfoOverlay();
        if (UIDisplayHandler.Instance._stateHandUI)
            UIDisplayHandler.Instance.TogglePlanetListOverlay();
        //move
        _curplanet = null;
        yield return StartCoroutine(DoMovement());
        yield return new WaitUntil(() => _isMoving == false);
        foreach (var item in _colPlanet)
        {
            item.gameObject.SetActive(true);
        }
        foreach (var pop in _colPopUp)
        {
            pop.TogglePopUp(false);
        }
        ToggleXRLineRenderer(true);
        ToggleXRRay(true);
        TogglePlanetInteractable(true);
        RecenterUI();
    }

    public void Reset()
    {
        StartCoroutine(ResetPlanet());
    }

    private void SpawnPopUpPlanetName(Planet planet)
    {
        GameObject popUp = Instantiate(_popUpNamePrefab, planet.transform);
        float minDistance = Vector3.Distance(_origin.transform.position, GlobalProperties.sun.transform.position);
        float maxDistance = Vector3.Distance(_origin.transform.position, _colPlanet.LastOrDefault()._planet.gameObject.transform.position);
        PopUpNameObj popUpNameObj = popUp.GetComponent<PopUpNameObj>();
        popUpNameObj.Init(planet, minDistance, maxDistance);
        _colPopUp.Add(popUpNameObj);
        popUpNameObj.TogglePopUp(false);
    }

    public int GetPlanetIndex(Planet planet)
    {
        return _colPlanet.IndexOf(planet);
    }

    private List<float> _diameters = new List<float>();
    public float[] GetPlanetLowestHighestDiameter()
    {
        foreach (var item in _colPlanet)
        {
            if (_diameters.Contains(item._planetdata.planetDiameter)) continue;
            if (item._planetdata.planetName != "Sun") _diameters.Add(item._planetdata.planetDiameter);
        }
        float[] lowToHighest = new float[2];
        lowToHighest[0] = Mathf.Min(_diameters.ToArray());
        lowToHighest[1] = Mathf.Max(_diameters.ToArray());
        return lowToHighest;
    }

    public void PopulateSelector()
    {
        CreateLine();
        foreach (var item in _colPlanet)
        {
            GameObject a = Instantiate(_selectorPrefab, _selectorPanel.transform);
            Button b = a.GetComponent<Button>();
            b.onClick.AddListener(delegate { StartCoroutine( OnPlanetSelected(item)); });
            TMP_Text text = b.GetComponentInChildren<TMP_Text>();
            text.text = item.name.ToUpper();
            a.name = "Selector_" + item.name;
            CreateLine();
            _colSelector.Add(b);

            //PlanetInteraction pi = item.GetComponent(typeof(PlanetInteraction)) as PlanetInteraction;
            PlanetInteraction pi = item.gameObject.GetComponent<PlanetInteraction>();
            if (pi)
            {
                pi.InitInteraction(item, recticle, delegate { SelectPlanet(item); });
            }
            else { Debug.Log("asdsad"); };
        }
        Canvas.ForceUpdateCanvases();
        //_selectorPanel.SetActive(false);
    }

    public void ToggleSelectorInteractable(bool state)
    {
        foreach (var item in _colSelector)
        {
            item.interactable = state;
        }
    }

    private void CreateLine()
    {
        GameObject line = new GameObject("Line");
        Image img = line.AddComponent<Image>();
        line.transform.SetParent(_selectorPanel.transform, false);
        img.color = UnityEngine.Color.white;
        line.GetComponent<RectTransform>().sizeDelta = new Vector2(5f, 2f);
    }

    public void SelectPlanet(Planet planet)
    {
        StartCoroutine(OnPlanetSelected(planet));
    }

    private IEnumerator OnPlanetSelected(Planet planet)
    {
        if (_curplanet == planet)
            yield break;
        yield return new WaitForEndOfFrame();
        if (_curplanet != null)
            yield return StartCoroutine(ResetPlanet());
        _curplanet = planet;
        _curplanet._planetMotion.isRotation = true;
        ToggleSelectorInteractable(false);
        if (UIDisplayHandler.Instance._stateHandUI)
            UIDisplayHandler.Instance.TogglePlanetListOverlay();
        int ind = _colPlanet.IndexOf(planet);
        ind++;
        ToggleXRLineRenderer(false);
        ToggleXRRay(false);
        ToggleOrbitRender(false);
        FocusOnPlanet(planet);
        StartCoroutine(DoRotateLights());
        yield return StartCoroutine(DoMovement());
        yield return new WaitUntil(() => _isMoving == false);
        if (!UIDisplayHandler.Instance._stateWorldUI)
            UIDisplayHandler.Instance.TogglePlanetInfoOverlay();
        _liteplanetObj.SelectPlanet(ind.ToString());
        ToggleXRLineRenderer(true);
        ToggleXRRay(true);
        ToggleSelectorInteractable(true);
    }

    public void RecenterUI()
    {
        var direction = _curplanet != null ? (_curplanet._planet.transform.position - cam.transform.position).normalized:
            cam.transform.forward;
            //(GlobalProperties.sun.transform.position - cam.transform.position).normalized;
        var targetRotation = Quaternion.LookRotation(direction, transform.up);
        _UIPointPlayer.transform.rotation = new Quaternion(0, targetRotation.y, 0, targetRotation.w);
    }

    public void FocusOnPlanet(Planet planet)
    {
        _isFocusOnPlanet = true; 
        foreach (var item in _colPlanet)
        {
            if (planet == item)
            {
                //item.gameObject.SetActive(true);
                continue;
            }
            //item.gameObject.SetActive(false);
        }
        foreach (var pop in _colPopUp)
        {
            pop.TogglePopUp(false);
        }
    }

    public void DisableAll()
    {
        foreach (var item in _colPlanet)
        {
            item.gameObject.SetActive(false);
            item.ToggleOrbit(false);
        }
    }

    public void ToggleOrbitRender(bool state)
    {
        if (_isFocusOnPlanet) return;
        foreach (var item in _colPlanet)
        {
            item.ToggleOrbit(state);
        }
    }

    public void TogglePopUp(bool state)
    {
        if (_isFocusOnPlanet) return;
        foreach (var item in _colPopUp)
        {
            item.TogglePopUp(state);
        }
    }

    private IEnumerator DoMovement()
    {
        yield return new WaitForEndOfFrame();
        _isMoving = true;
        if (_curplanet != null)
        {
            //_origin.transform.LookAt(_curplanet._planet.transform, transform.up);
            //_origin.transform.rotation = new Quaternion(0, _origin.transform.rotation.y, 0, _origin.transform.rotation.w);
            Vector3 camForwardFlat = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z).normalized;
            Vector3 _originForwardFlat = new Vector3(_origin.transform.position.x, 0, _origin.transform.position.z).normalized;
            Vector3 targetDir = _curplanet._planet.transform.position - cam.transform.position;
            //Vector3 _originForwardFlat = new Vector3(targetDir.x, 0, targetDir.z).normalized;
            //float angle = Vector3.SignedAngle(camForwardFlat, _originForwardFlat, Vector3.up);
            //Vector3 targetAngle = new Vector3(0, angle, 0);
            //_origin.transform.eulerAngles += targetAngle;

            //Rotate Slowly
            Vector3 desiredForwardFlat = new Vector3(_curplanet._planet.transform.position.x, 0, _curplanet._planet.transform.position.z).normalized;
            //float angle = Vector3.SignedAngle(camForwardFlat, desiredForwardFlat, Vector3.up);
            float angle = Vector3.SignedAngle(camForwardFlat, targetDir.normalized, Vector3.up);
            float angle2 = Vector3.SignedAngle(camForwardFlat, desiredForwardFlat, Vector3.up);
            float angle3 = Vector3.SignedAngle(_originForwardFlat, camForwardFlat, Vector3.up);
            Debug.Log(_curplanet._planetdata.planetName + " " + angle + " " + angle2 + " " + angle3);
            //Quaternion toRotation = Quaternion.LookRotation(targetDir, transform.up);
            //Debug.Log(_curplanet._planetdata.planetName + " " + toRotation);
            //_origin.transform.rotation = Quaternion.Lerp(cam.transform.rotation, toRotation, 1);
            //Vector3 lookDirection = lookAtTarget.position - mainCamera.position;
            //lookDirection.Normalize();
            //float target = _origin.transform.eulerAngles.y + angle3;
            Vector3 targetAngle = new Vector3(0, angle, 0);
            //Vector3 targetAngle2 = new Vector3(0, angle3, 0);
            Vector3 targetEuler = _origin.transform.eulerAngles + targetAngle;
            //LeanTween.rotateY(_origin.gameObject, targetAngle.y, 3f)/*.setEaseOutExpo()*/;
            //Vector3 targetAngle = new Vector3(0, angle, 0);
            //_origin.transform.eulerAngles += targetAngle;

            //Move
            Vector3 targetPos = _curplanet.curTeleportPoint.position;
            LeanTween.move(_origin.gameObject, targetPos, 5f)/*.setEaseOutExpo()*/
                .setOnComplete(() => { RecenterUI(); _isMoving = false; });
            LeanTween.value(_origin.transform.eulerAngles.y, targetEuler.y, 3f)
                .setOnUpdate((float val) => _origin.transform.eulerAngles = new Vector3(0, val, 0));
        }
        else
        {
            //_origin.transform.LookAt(_oriTransform, transform.up);
            //_origin.transform.rotation = new Quaternion(0, _origin.transform.rotation.y, 0, _origin.transform.rotation.w);
            Vector3 targetPos = _oriTransform.position;

            //Reset view
            //Vector3 camForwardFlat = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z).normalized;
            //Vector3 _originForwardFlat = new Vector3(_origin.transform.forward.x, 0, _origin.transform.forward.z).normalized;
            //float angle = Vector3.SignedAngle(camForwardFlat, _originForwardFlat, Vector3.up);
            //Vector3 targetAngle = new Vector3(0, angle, 0);
            //_origin.transform.eulerAngles += targetAngle;

            LeanTween.move(_origin.gameObject, targetPos, 5f)/*.setEaseOutExpo()*/
                .setOnComplete(() => { RecenterUI(); _isMoving = false; });
            yield return new WaitForSeconds(3f);

            //if (isResetting) { 
            //    //Reset view
            //    _origin.transform.LookAt(GlobalProperties.sun.transform, transform.up);
            //    _origin.transform.rotation = new Quaternion(0, _origin.transform.rotation.y, 0, _origin.transform.rotation.w);
            //    camForwardFlat = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z).normalized;
            //    _originForwardFlat = new Vector3(_origin.transform.forward.x, 0, _origin.transform.forward.z).normalized;
            //    angle = Vector3.SignedAngle(camForwardFlat, _originForwardFlat, Vector3.up);
            //    targetAngle = new Vector3(0, angle, 0);
            //    _origin.transform.eulerAngles += targetAngle;
            //}
        }
    }

    private IEnumerator DoRotateLights()
    {
        yield return new WaitForEndOfFrame();
        if (_light != null)
        {
            Vector3 lightForwardFlat = new Vector3(_light.transform.forward.x, 0, _light.transform.forward.z).normalized;
            Vector3 planetForwardFlat = new Vector3(_curplanet._planet.transform.position.x, 0, _curplanet._planet.transform.position.z).normalized;
            Vector3 targetDir = _curplanet._planet.transform.position - _light.transform.position;
            float angle = Vector3.SignedAngle(lightForwardFlat, targetDir.normalized, Vector3.up);
            Vector3 targetAngle = new Vector3(0, angle, 0);
            Vector3 targetEuler = _light.transform.eulerAngles + targetAngle;
            LeanTween.value(_light.transform.eulerAngles.y, targetEuler.y, 3f).setOnUpdate((float val) => _light.transform.eulerAngles = new Vector3(0, val, 0));
        }
    }

    private IEnumerator DoZoomIn()
    {
        yield return new WaitForEndOfFrame();
        if (_curplanet != null)
        {
            _origin.transform.LookAt(_curplanet._planet.transform, transform.up);
            _origin.transform.rotation = new Quaternion(0, _origin.transform.rotation.y, 0, _origin.transform.rotation.w);
            Vector3 targetPos = _origin.transform.position + 
                (/*(_curplanet._planet.transform.position - _origin.transform.position).normalized*/
                _origin.transform.forward * 
                Vector3.Distance(_curplanet._planet.transform.position, _curplanet.curTeleportPoint.position));
            Vector3 camForwardFlat = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z).normalized;
            Vector3 _originForwardFlat = new Vector3(_origin.transform.forward.x, 0, _origin.transform.forward.z).normalized;
            float angle = Vector3.SignedAngle(camForwardFlat, _originForwardFlat, Vector3.up);
            Vector3 targetAngle = new Vector3(0, angle, 0);
            _origin.transform.eulerAngles += targetAngle;
            LeanTween.move(_curplanet._planet, targetPos, 3f).setEaseInOutExpo().setOnComplete(() => { RecenterUI(); });
            yield return new WaitForSeconds(3f);
        }
    }
}