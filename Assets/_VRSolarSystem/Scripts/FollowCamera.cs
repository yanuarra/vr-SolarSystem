using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Quaternion rotationMultiplier = new Quaternion(1, 1, 1, 1);
    public float distMoveParameter = 1f;
    private Quaternion targetRotation;
    public float distanceFromCamera = 0.8f;
    private Vector3 targetPosition;
    private bool foundTarget;

    [SerializeField] private float yDistance;
    [SerializeField] private float angleFactor;
    [SerializeField] private float moveSpeed;

    [SerializeField] private float targetAngleFactor = 20;
    [SerializeField] private float targetDistance = 1;
    [SerializeField] private bool keepFollowingPlayer;

    private float _distanceFromTargetPosition;
    private float _angleBetweenTargetAndSelf;
    private Camera _mainCam;
    private bool _moveCheck = false;
    [SerializeField] GameObject _cameraFront;

    private bool _isMoving = false;
    private GameObject _q;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FindingTarget());
    }

    // Update is called once per frame
    void Update()
    {
        if (foundTarget)
        {
            _distanceFromTargetPosition = Vector3.Distance(transform.position, targetPosition);
            _angleBetweenTargetAndSelf = Quaternion.Angle(targetRotation, transform.rotation);

            if (!_moveCheck && (_angleBetweenTargetAndSelf > angleFactor ||
                 _distanceFromTargetPosition > distMoveParameter))
            {
                _moveCheck = true;
            }
            else if (_moveCheck)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, moveSpeed * Time.deltaTime);
                //Debug.Log("rotating");
                transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                //Debug.Log("moving");
                //transform.DOMove(targetPosition, moveSpeed * Time.deltaTime);
                if (_angleBetweenTargetAndSelf < targetAngleFactor ||
                    _distanceFromTargetPosition < targetDistance)
                {
                    _moveCheck = false;
                }
            }
            if (!keepFollowingPlayer)
                return;

            FollowPlayer();
        }
    }

    IEnumerator FindingTarget()
    {
        foundTarget = false;
        _mainCam = Camera.main;
        while (_mainCam == null)
        {
            yield return new WaitForSeconds(1);
            _mainCam = Camera.main;
        }
        if (_cameraFront==null)
            _cameraFront = FindObjectOfType<ScreenFader>().transform.Find("UIOffset").gameObject;
        _q = new GameObject("canvasIndicator");
        if (keepFollowingPlayer)
            FindTarget();
    }

    public void FollowPlayer()
    {
        if (_cameraFront == null)
            _cameraFront = FindObjectOfType<ScreenFader>().transform.Find("UIOffset").gameObject;
        FindTarget();
        Debug.Log("follow");
    }

    private void FindTarget()
    {
        Vector3 pos = _cameraFront.transform.position + _cameraFront.transform.forward * distanceFromCamera;
        _q.transform.position = new Vector3(pos.x, _mainCam.transform.position.y + yDistance, pos.z);
        foundTarget = true;
        //position
        targetPosition = _q.transform.position;
        //rotation
        _q.transform.LookAt(_mainCam.transform);
        targetRotation = _q.transform.rotation * rotationMultiplier;
    }
}
