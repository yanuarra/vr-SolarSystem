using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static PlanetList;

public class LitePlanetObj : MonoBehaviour
{
    public PlanetList planetList;
    public GameObject overlayPlanetObj;

    public Image Image_Planet; // Foto Planet pop up di sebelah kiri

    public Image PopUpRight; // BG pop up kanan
    public Image Button_Image;
    public AudioClip planetVO;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 5.0f;
    [SerializeField] private bool fadeIn = false;
    Coroutine fade;

    public void InitObj(PlanetList.Planet planet)
    {
        overlayPlanetObj.SetActive(true);
        Image_Planet.sprite = planet.planetFull;
        PopUpRight.sprite = planet.planetDesc;

        planetVO = planet.planetVO;
        PlayPlanetVO();
        if (ColorUtility.TryParseHtmlString(planet.colorHex, out Color planetColor))
        {
            Button_Image.color = planetColor;
        }
        canvasGroup.alpha = 0f;
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }
        cg.alpha = end;
    }

    public void FadeIn()
    {
        if (fade != null)
            StopCoroutine(fade);
        fade = StartCoroutine(FadeCanvasGroup(canvasGroup, 0, 1, fadeDuration));
        //StartCoroutine(FadeCanvasGroup(canvasGroup, 0, 1, fadeDuration));
    }

    public void FadeOut()
    {
        if (fade != null)
            StopCoroutine(fade);
        fade = StartCoroutine(FadeCanvasGroup(canvasGroup, 1, 0, fadeDuration));
        //StartCoroutine(FadeCanvasGroup(canvasGroup, 1, 0, fadeDuration));
    }

    public void CallImage(PlanetList.TemplatePlanet templatePlanet)
    {
        Button_Image.sprite = templatePlanet.buttonImage;
    }

    public void PlayPlanetVO()
    {
        AudioHandler.Instance.PlayVoPlanet(planetVO);
    }

    public void SelectPlanet(string planetID)
    {
        PlanetList.Planet planet = planetList.planetList.Find(p => p.id == planetID);
        Debug.Log("Call Planet ID = " + planetID);
        if (planet != null)
        {
            InitObj(planet);
        }
    }
}