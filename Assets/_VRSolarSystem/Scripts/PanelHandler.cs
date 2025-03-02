using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelHandler : MonoBehaviour
{
    [SerializeField]
    private Transform userInterface;

    private Transform cam;
    private GameObject currentOverlay;

    public static PanelHandler Instance { get; private set; }

    private void Awake()
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

    public void SetRotationToCam()
    {
        if (!CamExist()) return;
        userInterface.rotation = Quaternion.Euler(new Vector3(
            userInterface.eulerAngles.x, cam.eulerAngles.y, userInterface.eulerAngles.z));
    }

    public void SetNewCurrentOverlay(GameObject overlay)
    {
        if (currentOverlay != null)
        {
            currentOverlay.SetActive(false);
        }
        currentOverlay = overlay;
        ActiveCurrentOverlay();
    }

    public void ActiveCurrentOverlay()
    {
        if (currentOverlay == null) return;
        currentOverlay.SetActive(true);
    }

    public void DeactiveCurrentOverlay()
    {
        if (currentOverlay == null) return;
        currentOverlay.SetActive(false);
    }

    private bool CamExist()
    {
        if (Camera.main == null) return false;
        cam = Camera.main.transform;
        return true;
    }
}