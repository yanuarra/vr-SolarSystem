using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ScaleBasedOnDistance : MonoBehaviour
{
    Transform cam;
    float FixedSize = 0.00001f;
    [SerializeField] float minScale = 0.00025f;
    [SerializeField] float maxScale = 1f;
    [SerializeField] float _minDistance = .001f;
    [SerializeField] float _maxDistance = 50f;
    private void Start()
    {
        cam = Camera.main.transform;
    }

    private void Update()
    {
        Scale();
    }

    void Scale()
    {
        var distance = Vector3.Distance(cam.position, transform.position);
        var size = distance * FixedSize * Camera.main.fieldOfView;
        var scale = Mathf.Lerp(minScale, maxScale, Mathf.InverseLerp(_minDistance, _maxDistance, distance));
        transform.localScale = new Vector3(scale, scale, scale);
        transform.localScale = Vector3.one * size;
    }
}
