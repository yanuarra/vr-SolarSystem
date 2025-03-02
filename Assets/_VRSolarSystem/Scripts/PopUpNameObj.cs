using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PopUpNameObj : MonoBehaviour
{
    public float FixedSize = .001f;
    public float minScale = 7f;
    public float maxScale = 15f;
    float _minDistance;
    float _maxDistance;
    GameObject popUp;
    Canvas canvas;
    LineRenderer line;
    TMP_Text text;
    Planet item;
    int lineLength = 3;
    List<Vector3> linePoints = new List<Vector3>();
    RectTransform _rectTransform;

    public void Init(Planet planet, float minDistance, float maxDistance)
    {
        Transform cam = Camera.main.transform;
        canvas = gameObject.GetComponentInChildren<Canvas>();
        item = planet;
        popUp = this.gameObject;
        popUp.name = "PopUp_" + item.name;
        popUp.transform.SetParent(item.transform, false);
        popUp.transform.rotation = Quaternion.identity;
        Vector3 side2 = Vector3.up;
        Vector3 side1 = new Vector3(cam.position.x, item._planet.transform.position.y, cam.position.z) - item._planet.transform.position;
        if (item.name == "Uranus")
        {
            popUp.transform.position = item._planet.transform.position -
                Vector3.Cross(side1, side2).normalized * item._planet.transform.localScale.x;
            linePoints.Add(popUp.transform.position);
            popUp.transform.position -= Vector3.Cross(side1, side2).normalized * 15f/*item._planet.transform.localScale.x * 1.5f*/;
        }
        else
        {
            popUp.transform.position = item._planet.transform.position +
                Vector3.Cross(side1, side2).normalized * item._planet.transform.localScale.x;
            linePoints.Add(popUp.transform.position);
            popUp.transform.position += Vector3.Cross(side1, side2).normalized * 15f/*item._planet.transform.localScale.x * 1.5f*/;
        }
        linePoints.Add(popUp.transform.position);
        popUp.transform.position += Vector3.up * 15f/*item._planet.transform.localScale.x * 1.5f*/;

        //Get Bottom Canvas
        _rectTransform = canvas.GetComponent<RectTransform>();
        linePoints.Add(UIExtension.GetPivotInWorldSpace(_rectTransform));
        //Vector2 normalizedPosition;
        //GetNormalizedPosition(out normalizedPosition);
        //GameObject point = new GameObject("pointbotmid");
        //point.transform.SetParent(this.transform);
        //point.transform.position = normalizedPosition;

        var distance = Vector3.Distance(cam.position, item._planet.transform.position);
        var size = distance * FixedSize * Camera.main.fieldOfView;
        var scale = Mathf.Lerp(minScale, maxScale, Mathf.InverseLerp(minDistance, maxDistance, distance));
        popUp.transform.localScale = new Vector3(scale, scale, scale);
        canvas.transform.LookAt(Camera.main.transform);
        line = popUp.GetComponentInChildren<LineRenderer>();
        if (line != null)
        {
            line.positionCount = linePoints.Count;
            for (int i = 0; i < line.positionCount; i++)
            {
                line.SetPosition(i, linePoints[i]);
            }
        }
        line.enabled = true;
        text = popUp.GetComponentInChildren<TMP_Text>();
        if (text != null)
        {
            text.text = item._planetdata.planetName;
        }
    }

    private bool GetNormalizedPosition(out Vector2 normalizedPosition)
    {
        normalizedPosition = default;
        Vector2 botmid = new Vector2(_rectTransform.offsetMax.x / 2, _rectTransform.offsetMin.y);
        if (!RectTransformUtility.ScreenPointToWorldPointInRectangle(_rectTransform, botmid, Camera.main, out var localPosition))
            return false;
        normalizedPosition = Rect.PointToNormalized(_rectTransform.rect, localPosition);
        Debug.Log(localPosition + " " +normalizedPosition);
        return true;
    }

    public void TogglePopUp(bool state)
    {
        float target = state ? 1 : 0;
        if (line != null)
        {
            Color end = line.endColor;
            Color start = line.startColor;
            float cur = line.startColor.a;
            LeanTween.value(line.gameObject, cur, target, .5f).setOnUpdate((float val) =>
            {
                //line.material.SetColor("_Color", new Color(1f, 1f, 1f, valf));
                Color newC = new Color(1f, 1f, 1f, val);
                //start.a = val;
                //end.a = val;
                line.startColor = newC;
                line.endColor = newC;
            });
        }
        CanvasGroup sr = item.GetComponentInChildren<CanvasGroup>();
        if (sr != null)
            LeanTween.alphaCanvas(sr, target, .5f);
    }
}
