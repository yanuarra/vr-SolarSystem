
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class PlanetMotion : MonoBehaviour
{
    [SerializeField] Transform centerMass;
    [SerializeField] Transform revealingLight;
    [SerializeField] float _revolutionDays = 1.0f;
    [SerializeField] float _planetToSunDistance = 1.0f;
    [SerializeField] float _aphelionDistance = 1.0f;
    [SerializeField] float _perihelionDistance = 1.0f;
    [SerializeField] float _rotationSpeed = 1.0f;
    [SerializeField] float _revolutionSpeed = 1.0f;
    float _fixedDistance = 9f;
    GameObject _planet;
    Vector3 _startPos;
    Vector3 _curPos;
    public bool isRevolution = true;
    public bool isRotation = true;

    public IEnumerator InitProperties(CelestialObject data, GameObject planet, bool isRealScale)
    {
        yield return new WaitForEndOfFrame();
        if (centerMass == null) centerMass = GlobalProperties.sun.transform;
        _planet = planet;
        //_revolutionSpeed = data.revolutionTime / GlobalProperties.EarthRevolutionDays;
        _revolutionSpeed = 360f / data.revolutionTime /*/ GlobalProperties.EarthRevolutionDays*/ ; //(365.25/365.25)
        //_rotationSpeed = GlobalProperties.EarthDay/data.rotationTime;
        _rotationSpeed = 360f / (data.rotationTime / GlobalProperties.EarthDay);

        //DISTANCE
        _aphelionDistance = data.AphelionInAU * GlobalProperties.globalDistanceMultiplier; 
        _perihelionDistance = data.PerihelionInAU * GlobalProperties.globalDistanceMultiplier;
        planet.transform.Rotate(-Vector3.forward * data.axialTilt, Space.Self);
        planet.transform.position = Vector3.forward * _planetToSunDistance + centerMass.position;
        int i = GameController.Instance.GetPlanetIndex(GetComponent<Planet>());
        float distanceToSun = isRealScale? EllipticalOrbit() : GlobalProperties.sun._planet.transform.localScale.x/5 + (i * _fixedDistance);
        planet.transform.position = (planet.transform.position - centerMass.position).normalized * distanceToSun * GlobalProperties.globalDistanceMultiplier;
        _startPos = transform.position;

        if (!isRealScale)
        {
            float angle = 0;
            float[] lowtohi = GameController.Instance.GetPlanetLowestHighestDiameter();
            CelestialObject _planetdata = GetComponent<Planet>()._planetdata;
            //float scal = Mathf.Clamp(_scale, 4f, 15f);
            float diff = Mathf.InverseLerp(lowtohi[0], lowtohi[1], _planetdata.planetDiameter);
            angle = Mathf.Lerp(30, -30, diff);
            //angle = angle > 0f  ? Mathf.Clamp(angle, 5, 30): Mathf.Clamp(angle, -30, -5);
            Debug.Log(_planetdata.planetName + angle);
            transform.RotateAround(centerMass.position, Vector3.up, angle);
        }
        //planet.transform.RotateAround(centerMass.position, Vector3.up, angle);
        if (revealingLight != null)
        {
            revealingLight.transform.LookAt(transform.position);
        }
    }

    void Update()
    {
        //if (planet != null)
        //{
        //    transform.position = planet.position + (transform.position - planet.position).normalized * satelliteOrbitDistance;
        //    transform.RotateAround(planet.position, Vector3.up, rotationAroundPlanetSpeed * Time.deltaTime);
        //}
        //transform.LookAt(sun);
        //transform.Translate(Vector3.right * Time.deltaTime * (defaultEarthYear / rotationAroundSunDays) * (globalValuesScript.globalPlanetRotationAroundSun) * (rotationAroundSunDays / defaultEarthYear));
        if (centerMass == null) return;
        if (_planet == null) return;
        if (isRevolution) { 
            transform.RotateAround(centerMass.position, Vector3.up,
                Time.deltaTime * (_revolutionSpeed) *
                (GlobalProperties.globalSpeedMultiplier /** Time.deltaTime*/));
            //Vector3 delta = centerMass.position - transform.position;
            //delta.y = 0;
            //orbiter.position = transform.position + delta.normalized * TargetDistance;
            //transform.position = (transform.position - centerMass.position).normalized * EllipticalOrbit() * GlobalProperties.globalDistanceMultiplier /*+ centerMass.position*/;
        }
        if (isRotation)
            _planet.transform.Rotate(-Vector3.up * Time.deltaTime * _rotationSpeed * GlobalProperties.globalSpeedMultiplier, Space.Self);
    }

    public void ToggleRevolution(bool _state)
    {
        isRevolution = _state;
    }
    float EllipticalOrbit()
    {
        _curPos = transform.position;
        Vector2 side12 = new Vector2(_startPos.x, _startPos.z) - new Vector2(centerMass.position.x, centerMass.position.z) ;
        Vector2 side22 = new Vector2(_curPos.x, _curPos.z) - new Vector2(centerMass.position.x, centerMass.position.z);
        float degree = Vector2.SignedAngle(side12, side22);
        float rad = degree * Mathf.Deg2Rad;
        Debug.Log(rad);
        float curDistance = Mathf.Lerp(_perihelionDistance, _aphelionDistance, Mathf.InverseLerp(0, Mathf.PI, Mathf.Abs(rad)));
        return curDistance;
    }
}
