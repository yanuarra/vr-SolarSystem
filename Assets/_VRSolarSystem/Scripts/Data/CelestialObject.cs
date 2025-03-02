using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Planet", menuName = "ScriptableObjects/Create Planet Data", order = 1)]
public class CelestialObject : ScriptableObject
{
    public string planetName;
    public List<string> planetNameAlt = new List<string>();
    [TextArea(5, 10)]
    public string planetDesc;
    public Sprite planetThumbnail;
    public List<CelestialObject> moons = new List<CelestialObject>();
    public float orbitDistance;
    public float AphelionInAU;
    public float PerihelionInAU;
    public float planetDiameter;
    public float revolutionTime;
    public float revolutionTimeHours;
    public float rotationTime;
    public float axialTilt;
    public float orbitalInclination;
}
