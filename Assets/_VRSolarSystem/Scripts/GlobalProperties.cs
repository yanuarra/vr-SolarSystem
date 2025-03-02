using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalProperties : MonoBehaviour
{
    public static GlobalProperties Instance;
    public Slider speedSlider;
    public Slider distanceSlider;
    public Slider scaleSlider;
    public InputField multiplierValue;
    public static float globalSpeedMultiplier = .01f;
    public static float globalDistanceMultiplier = 1f;
    public static float globalScaleMultiplier = 5.0f;
    public static Planet sun;

    //constant
    const float ASTRONOMICAL_UNIT = 149597870.7f; //KM
    const float EARTH_YEAR = 365.25f; //DAYS
    const float EARTH_YEAR_IN_HOURS = 8760f; //HOUR
    const float EARTH_DAY_IN_HOURS = 24f; //HOUR
    const float EARTH_DIAMETER = 12756f; //KM
    public static float AstronomicalUnit => ASTRONOMICAL_UNIT;
    public static float EarthRevolutionDays => EARTH_YEAR;
    public static float EarthRevolutionDaysInHours => EARTH_YEAR_IN_HOURS;
    public static float EarthDay => EARTH_DAY_IN_HOURS;
    public static float EarthDiameter => EARTH_DIAMETER;

    private void Awake()
    {
        SetAsSingleton();
        Init();
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

    private void Init()
    {
        speedSlider.onValueChanged.AddListener( delegate { UpdateSpeedMultiplier(speedSlider.value); });
        distanceSlider.onValueChanged.AddListener( delegate { UpdateDistanceMultiplier(distanceSlider.value); });
        scaleSlider.onValueChanged.AddListener( delegate { UpdateScaleMultiplier(scaleSlider.value); });
        speedSlider.value = globalSpeedMultiplier;
        distanceSlider.value = globalDistanceMultiplier;
        scaleSlider.value = globalScaleMultiplier;
        sun = GameObject.FindGameObjectWithTag("Sun").GetComponent<Planet>();
    }

    private void UpdateSpeedMultiplier(float val)
    {
        globalSpeedMultiplier = val;
    }

    private void UpdateDistanceMultiplier(float val)
    {
        globalDistanceMultiplier = val;
    }

    private void UpdateScaleMultiplier(float val)
    {
        globalScaleMultiplier = val;
    }
}
