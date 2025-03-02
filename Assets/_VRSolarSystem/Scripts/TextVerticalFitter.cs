using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextVerticalFitter : MonoBehaviour
{
    [SerializeField]
    private Text textContent;

    [SerializeField]
    private RectTransform rectContent;

    private float defaultRectHeigth;
    private float textRowHeigth;

    private void Awake()
    {
        defaultRectHeigth = rectContent.sizeDelta.y;
        textRowHeigth = GetTextRectHeight("");
    }

    public void SetText(string message)
    {
        int rowAmount = Mathf.CeilToInt(GetTextRectHeight(message) / textRowHeigth);
        textContent.text = message;
        rectContent.sizeDelta = new Vector2(rectContent.sizeDelta.x, defaultRectHeigth * rowAmount);
    }

    public void RefreshTextRowHeigth()
    {
        textRowHeigth = GetTextRectHeight("");
    }

    private float GetTextRectHeight(string s)
    {
        TextGenerator textGen = new TextGenerator();
        Vector2 contentSize = new Vector2(textContent.rectTransform.rect.size.x, textContent.rectTransform.rect.size.y);
        TextGenerationSettings generationSettings = textContent.GetGenerationSettings(contentSize);
        float height = Mathf.Floor(textGen.GetPreferredHeight(s, generationSettings));
        return height;
    }
}