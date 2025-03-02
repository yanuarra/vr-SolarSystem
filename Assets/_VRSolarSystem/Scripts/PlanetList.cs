using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "New Planet List", menuName = "Planet List")]
[Serializable]
public class PlanetList : ScriptableObject
{
    public List<Planet> planetList = new List<Planet>();
    public Dictionary<string, TemplatePlanet> templateDictionary = new Dictionary<string, TemplatePlanet>();
    public List<TemplatePlanet> templateList = new List<TemplatePlanet>();

    [Serializable]
    public class Planet
    {
        public string id;
        public int order;
        public string name;
        public string alias;

        [TextArea]
        public string description;

        public Texture planetImage;
        public Texture icon2D;
        public AudioClip planetVO;
        public string colorHex;
        public Sprite planetFull;
        public Sprite planetDesc;
    }

    [Serializable]
    public class TemplatePlanet
    {
        public Sprite buttonImage;
        public Sprite backgroundLeft;
        public Sprite backgroundRight;
        public Sprite lineImage;
    }
}