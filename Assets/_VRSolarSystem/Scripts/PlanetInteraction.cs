using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;

public class PlanetInteraction : MonoBehaviour
{
    [SerializeField] SphereCollider _sphereCollider;
    [SerializeField] XRSimpleInteractable interactable;
    [SerializeField] Outline outline;
    [SerializeField] ChangeOutlineHandler[] changes;

    public void InitInteraction(Planet planet, GameObject recticle, Action<Planet> asd)
    {
        //_collider = gameObject.AddComponent<SphereCollider>();
        //_collider.radius = 5f;
        //_collider.isTrigger = true;
        interactable = planet._planet.GetComponent<XRSimpleInteractable>();
        if (interactable == null)
            interactable = planet._planet.AddComponent<XRSimpleInteractable>();

        if (interactable)
        {
            _sphereCollider = planet._planet.GetComponent<SphereCollider>();
            if (_sphereCollider == null) 
                _sphereCollider = planet._planet.AddComponent<SphereCollider>();
            _sphereCollider.radius = 1;
            interactable.colliders.Add(_sphereCollider);
            interactable.customReticle = recticle != null ? recticle : GameObject.Find("Canvas_Recticle");
            interactable.activated.AddListener(delegate { asd(planet); });
        }

        //interactable.onHoverEntered.AddListener(delegate { onHoverEnter(); });
        //interactable.onHoverExited.AddListener(delegate { onHoverExit(); });
        //interactable.onActivate.AddListener(delegate { onActivate(); });

        //changes = GetComponentsInChildren<ChangeOutlineHandler>();
        //foreach (var item in changes)
        //{
        //    item.type = ChangeOutlineHandler.OutlineType.isBlinking;
        //    item.isBlinking = false ;
        //}
    }

    public void ToggleInteractable(bool state) { 
        if (interactable != null) 
            interactable.enabled = state;
        
    }

    public void ToggleOutline(bool state)
    {
        foreach (var item in changes)
        {
            item.type = ChangeOutlineHandler.OutlineType.isBlinking;
            item.isBlinking = state;
        }
    }

    public void ToggleOutlineBlink(bool state)
    {
        foreach (var item in changes)
        {
            if (state)
            {
                item.OutlineIsBlinking();
            }
            else
            {
                item.OutlineNotBlinking();
            }
        }
    }

    void onHoverEnter()
    {
        foreach (var item in changes)
        {
            item.OutlineIsBlinking();
        }
    }

    void onHoverExit()
    {
        foreach (var item in changes)
        {
            item.OutlineIsBlinking();
        }
    }

    void onActivate()
    {
        Debug.Log("Teleport");
    }

}
