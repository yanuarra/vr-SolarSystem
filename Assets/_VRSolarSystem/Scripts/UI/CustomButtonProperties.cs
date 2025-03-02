using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class CustomButtonProperties : MonoBehaviour
{
    public enum Type {
        primary,
        secondary
    }

    public Type type;
    public Color primaryColor;
    public Color secondaryColor;
    public Color highlightColor;
    public Color pressedColor;
    public Color selectedColor;
    public Color disableColor;
    public Color enableColorContent;
    public Color highlightColorContent;
    public Color disableColorContent;
    [SerializeField]
    private Image background;
    [SerializeField]
    private Image iconButton;
    [SerializeField]
    private TMP_Text textButton;
    private TextMeshProUGUI textMeshProUGUI;
    private CustomButton customButton;
    private CanvasGroup canvasGroup;
    public bool isSmoothTransition = false;

    private void Awake() {
        customButton = this.GetComponent<CustomButton>();
        canvasGroup = this.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = this.AddComponent<CanvasGroup>();
        textMeshProUGUI = textButton.GetComponent<TextMeshProUGUI>();

    }

    private void OnEnable() {
        customButton.SetProperties(this);
    }


    void updateValueExampleCallback(Color val)
    {
        textMeshProUGUI.color = val;
    }

    public void SetColor(Color colorBg, Color colorContent) {
        if (isSmoothTransition)
        {
            if (background != null) 
                LeanTween.color(background.gameObject.GetComponent<RectTransform>(), colorBg, .3f).setEaseOutExpo();
            if (iconButton != null)
                LeanTween.color(iconButton.gameObject.GetComponent<RectTransform>(), colorContent, .3f).setEaseOutExpo();
            if (textButton != null)
                LeanTween.value(this.gameObject, textMeshProUGUI.color, colorContent, .3f).setOnUpdate(updateValueExampleCallback).setEaseOutExpo();
        }
        else
        {
            if (background != null) background.color = colorBg;
            if (iconButton != null) iconButton.color = colorContent;
            if (textButton != null) textButton.color = colorContent;
        }
    }

    public void ToggleIcon(bool _state)
    {
        if (iconButton != null)
            iconButton.gameObject.SetActive(_state);
    }

    public void ResetButton()
    {
        type = Type.primary;
        customButton.SetProperties(this);
    }

    public void ChangeButtonType(Type _type)
    {
        type = _type == Type.primary? Type.primary : Type.secondary;
        customButton.SetProperties(this);
    }
}
