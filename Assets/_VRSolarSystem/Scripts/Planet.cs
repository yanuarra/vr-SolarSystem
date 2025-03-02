using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

public class Planet : MonoBehaviour
{
    public CelestialObject _planetdata;
    public PlanetMotion _planetMotion;
    public PlanetInteraction _planetInteraction;
    public OrbitRenderer _orbitRenderer;
    [SerializeField] GameObject _axialPivot;
    public GameObject _planet;
    [SerializeField] List<GameObject> _moons = new List<GameObject>();
    float _scale = 1.0f;
    public Transform curTeleportPoint;
    public Transform curUIPoint;
    [SerializeField] Vector3 originPlanetPos;
    [SerializeField] bool isRealScale = false;

    public IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();
        if (_planetMotion == null) 
            _planetMotion = GetComponent<PlanetMotion>();
        _planetMotion.isRevolution = false;
        if (_planet == null) 
            _planet = GetComponentInChildren<MeshRenderer>().gameObject;
        if (_planetdata != null)
            yield return _planetMotion.InitProperties(_planetdata, _planet, isRealScale);
        if (_planetInteraction == null) 
            _planetInteraction = GetComponent<PlanetInteraction>();
        if (_orbitRenderer == null) 
            _orbitRenderer = GetComponentInChildren<OrbitRenderer>();

        SetPlanetScale();
        SetAxialTilt();
        SetPoints();

        //_planetMotion.isRevolution = true;
        RevealNightEarthTexture rev = GetComponentInChildren<RevealNightEarthTexture>();
        if (rev != null) rev.Init(10f, _planet);
        if (_orbitRenderer != null) 
            _orbitRenderer.Init(this, isRealScale);

        originPlanetPos = _planet.transform.position;
    }

    public void ToggleOrbit(bool state)
    {
        if (_orbitRenderer != null)
            _orbitRenderer.ToggleRenderer(state);
    }

    public void SetPoints()
    {
        GameObject telePoint = new GameObject("Teleport Point");
        telePoint.transform.SetParent(gameObject.transform, false);
        telePoint.transform.rotation = Quaternion.identity;

        Vector3 direction = Vector3.zero;
        float scaler = 0f;
        float yOffset = 0f;
        float UIOffset = 1f;
        float UIScaler = 0f;
        if (this == GlobalProperties.sun)
        {
            direction = (transform.position - Vector3.forward).normalized;
            scaler = 2.2f;
            UIScaler = .2f;
            UIOffset = 1.65f;
            yOffset = -1f;
        }
        else if (_planetdata.planetName == "Saturn")
        {
            direction = (Vector3.zero - _planet.transform.position).normalized;
            //direction = _planet.transform.right;
            yOffset = 1f;
            UIScaler = .3f;
            scaler = 3f;
            UIOffset = 3f;
        }
        else
        {
            direction = (Vector3.zero - _planet.transform.position).normalized;
            scaler = 2.1f;
            UIScaler = .25f;
            UIOffset = 2.1f;
            yOffset = -1f;
        }
        telePoint.transform.position = _planet.transform.position;
        telePoint.transform.position = telePoint.transform.position + direction * _planet.transform.localScale.x * scaler;
        telePoint.transform.RotateAround(_planet.transform.position, transform.up, -30f);
        telePoint.transform.LookAt(_planet.transform);
        curTeleportPoint = telePoint.transform;

        //UIScaler = _planet == GlobalProperties.sun? 1f: .35f;
        GameObject UIPoint = new GameObject("UI Point");
        UIPoint.transform.SetParent(gameObject.transform, false);
        UIPoint.transform.rotation = Quaternion.identity;
        UIPoint.transform.position = _planet.transform.position;
        UIPoint.transform.localPosition -= Vector3.right * _planet.transform.localScale.x * UIOffset;
        UIPoint.transform.localPosition += Vector3.forward;

        float uiscale = _planet.transform.localScale.x * UIScaler;
        UIPoint.transform.localScale = new Vector3(uiscale, uiscale, uiscale);
        curUIPoint = UIPoint.transform;
    }

    public void SetAxialTilt()
    {
        _axialPivot = new GameObject("Axial Tilt " + _planetdata.name);
        _axialPivot.transform.position = Vector3.zero;
        _axialPivot.transform.rotation = Quaternion.identity;
        transform.parent = _axialPivot.transform;
        _axialPivot.transform.Rotate(-Vector3.forward * _planetdata.orbitalInclination);
    }

    public void SetPlanetScale()
    {
        if (!isRealScale)
        {
            if (this == GlobalProperties.sun) return;
            float[] lowtohi = GameController.Instance.GetPlanetLowestHighestDiameter();
            //float scal = Mathf.Clamp(_scale, 4f, 15f);
            float diff = Mathf.InverseLerp(lowtohi[0], lowtohi[1], _planetdata.planetDiameter);
            Debug.Log(diff);
            float scal2 = Mathf.Lerp(9f, 22f, diff);
            _scale = scal2;
        }
        else
        {
            _scale = _planetdata.planetDiameter / GlobalProperties.EarthDiameter * GlobalProperties.globalScaleMultiplier;
        }
        _planet.transform.localScale = new Vector3(_scale, _scale, _scale);
        foreach (var item in _moons)
        {
            item.transform.localScale *= _scale;
        }
    }

    public void ResetPlanetPos()
    {
        LeanTween.move(_planet, originPlanetPos, 3f).setEaseInOutExpo();
    }
}
