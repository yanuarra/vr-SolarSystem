using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitRenderer : MonoBehaviour
{
    [SerializeField] Planet planet;
    [SerializeField] LineRenderer line;
    [SerializeField] MeshRenderer meshRenderer;
    int lineRendererLength = 100;
    Transform dummy;
    Transform center;
    Vector3 _curPos;
    Vector3 _startPos;
    float min, max;
    float _fixedDistance = 9f;
    [SerializeField] GameObject _parent;

    public void Init(Planet _planet, bool isRealScale)
    {
        center = GlobalProperties.sun.transform;
        planet = _planet;
        //dummy = new GameObject().transform;
        line = GetComponent<LineRenderer>();
        line.positionCount = lineRendererLength;
        _startPos = transform.position;
        min = planet._planetdata.PerihelionInAU * GlobalProperties.globalDistanceMultiplier;
        max = planet._planetdata.AphelionInAU * GlobalProperties.globalDistanceMultiplier;
        planet.transform.position = Vector3.forward + center.position;
        int index = GameController.Instance.GetPlanetIndex(_planet);
        float distanceToSun = isRealScale ? EllipticalOrbit() : GlobalProperties.sun._planet.transform.localScale.x / 5 + (index * _fixedDistance);
        if (isRealScale)
        {
            //Oval
            transform.position = (transform.position - center.position).normalized * EllipticalOrbit() * GlobalProperties.globalDistanceMultiplier /*+ center.position*/;
        }
        else
        {
            //Circle
            transform.position = (transform.position - center.position).normalized * distanceToSun * GlobalProperties.globalDistanceMultiplier;
        }

        float angle = 360f/lineRendererLength;
        for (int i = 0; i < lineRendererLength; i++)
        {
            //line.SetPosition(i, transform.position + PO.ParametricOrbit(2 * Mathf.PI / (lineRendererLength - 1) * i));
            //float angle = Mathf.Lerp(0, 360f, Mathf.InverseLerp(0, lineRendererLength - 1, i));
            if (i != 0) 
                transform.RotateAround(center.position, Vector3.up, angle);
            //transform.position = (transform.position - center.position).normalized * EllipticalOrbit() * GlobalProperties.globalDistanceMultiplier /*+ center.position*/;
            line.SetPosition(i, transform.position);
        }

        line.positionCount = lineRendererLength + 1;
        line.SetPosition(lineRendererLength , line.GetPosition(0));
        line.GetComponent<Renderer>().enabled = true;
        line.useWorldSpace = false;
        //BakeLineDebuger(gameObject);

        _parent = new GameObject("Orbit");
        _parent.transform.position = Vector3.zero;
        _parent.transform.rotation = Quaternion.identity;
        gameObject.transform.SetParent(_parent.transform);
        gameObject.transform.position = Vector3.zero;
        _parent.transform.SetParent(planet.transform);
        gameObject.transform.rotation = Quaternion.identity;
        _parent.transform.Rotate(-Vector3.forward * _planet._planetdata.orbitalInclination);

        _planet._orbitRenderer = this;
        Debug.Log(_planet._orbitRenderer);
    }

    public void ToggleRenderer(bool state)
    {
        float target = state ? 1 : 0;
        if (line != null)
        {
            Color end = line.endColor;
            Color start = line.startColor;
            float cur = line.startColor.a;
            LeanTween.value(line.gameObject, cur, target, .5f).setOnUpdate((float val) =>
            {
                Color newC = new Color(1f, 1f, 1f, val);
                line.startColor = newC;
                line.endColor = newC;
            });
        }
        //meshRenderer.enabled = state;
    }

    float EllipticalOrbit()
    {
        _curPos = transform.position;
        Vector2 side12 = new Vector2(_startPos.x, _startPos.z) - new Vector2(center.position.x, center.position.z);
        Vector2 side22 = new Vector2(_curPos.x, _curPos.z) - new Vector2(center.position.x, center.position.z);
        float degree = Vector2.SignedAngle(side12, side22);
        float rad = degree * Mathf.Deg2Rad;
        float curDistance = Mathf.Lerp(min, max, Mathf.InverseLerp(0, Mathf.PI, Mathf.Abs(rad)));
        return curDistance;
    }

    public Material s_matDebug;
    public void BakeLineDebuger(GameObject lineObj)
    {
        var lineRenderer = lineObj.GetComponent<LineRenderer>();
        var meshFilter = lineObj.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        lineRenderer.BakeMesh(mesh);
        meshFilter.sharedMesh = mesh;

        meshRenderer = lineObj.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = s_matDebug;

        GameObject.Destroy(lineRenderer);
    }
}
