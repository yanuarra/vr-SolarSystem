using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CustomButtonProperties))]
public class CustomButton : Button, IPointerEnterHandler, IPointerDownHandler, IPointerClickHandler, IPointerExitHandler
{
    private Color defaultColor;
    public CustomButtonProperties properties;

    public void SetProperties(CustomButtonProperties _properties)
    {
        properties = _properties;
        switch (properties.type)
        {
            case CustomButtonProperties.Type.primary:
                defaultColor = properties.primaryColor;
                break;
            case CustomButtonProperties.Type.secondary:
                defaultColor = properties.secondaryColor;
                break;
        }
        if (interactable)
        {
            properties.SetColor(defaultColor, properties.enableColorContent);
        }
        else
        {
            properties.SetColor(properties.disableColor, properties.disableColorContent);
        }
    }

    public void ChangeColor(CustomButtonProperties.Type tye)
    {
        properties.type = tye;
        SetProperties(properties);
    }

    public void SetPressedState()
    {
        DoStateTransition(SelectionState.Pressed, true);
    }

    public void SetSelectedState()
    {
        this.Select();
        DoStateTransition(SelectionState.Selected, true);
    }

    public void SetNormalState()
    {
        DoStateTransition(SelectionState.Normal, true);
    }

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        if (properties == null)
            return;
        switch (state)
        {
            case SelectionState.Normal:
                properties.SetColor(defaultColor, properties.enableColorContent);
                break;
            case SelectionState.Highlighted:
                properties.SetColor(properties.highlightColor, properties.highlightColorContent);
                break;
            case SelectionState.Pressed:
                properties.SetColor(properties.pressedColor, properties.enableColorContent);
                properties.ToggleIcon(true);
                break;
            case SelectionState.Selected:
                properties.SetColor(properties.selectedColor, properties.enableColorContent);
                properties.ToggleIcon(true);
                break;
            case SelectionState.Disabled:
                properties.SetColor(properties.disableColor, properties.disableColorContent);
                properties.ToggleIcon(false);
                break;
            default:
                break;
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        //AudioHandler.Instance.PlayButtonClicked();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        AudioHandler.Instance.PlayButtonHighlight();
        base.OnPointerEnter(eventData);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        AudioHandler.Instance.PlayButtonClicked();
        base.OnPointerClick(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
    }
}
