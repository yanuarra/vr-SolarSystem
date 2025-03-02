using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static PlanetList;

public class PlanetObj : MonoBehaviour
{
    public PlanetList planetList;
    public GameObject overlayPlanetObj;

    public RawImage Image_Planet; // Foto Planet pop up di sebelah kiri
    public TextMeshProUGUI Text_PlanetName; // nama Planet di pop up sebelah kiri
    public TextMeshProUGUI Text_PlanetAlias; // nama planet di pop up sebelah kanan
    public TextMeshProUGUI Text_PlanetDescription; // deskripsi planet di pop up sebelah kanan
    public Text Text_Button;
    public Image Alias_Image;
    public Image Line_Image;
    public Image Name_Image;
    public Image Button_Image;
    public RawImage Image_PlanetIcon;
    public Image Alias_Background_Image;
    public Image PopUpLeft; // BG pop up kiri
    public Image PopUpRight; // BG pop up kanan
    public AudioClip planetVO;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 5.0f;
    [SerializeField] private bool fadeIn = false;

    public void InitObj(PlanetList.Planet planet)
    {
        if (fadeIn)
        {
            FadeIn();
            Debug.Log("Fade In Started");
        }
        else
        {
            FadeOut();
            Debug.Log("Fade Out Started");
        }
        overlayPlanetObj.SetActive(true);
        Image_Planet.texture = planet.planetImage;
        Text_PlanetName.text = planet.name;
        Text_PlanetAlias.text = planet.alias;
        Text_PlanetDescription.text = planet.description;
        Image_PlanetIcon.texture = planet.icon2D;
        Text_Button.text = "Kembali ke Galaxy";

        planetVO = planet.planetVO;
        PlayPlanetVO();

        // Look up the template based on the planet ID
        if (planetList.templateDictionary.TryGetValue(planet.id, out PlanetList.TemplatePlanet template))
        {
            CallImage(template);
        }

        // Apply color from the planet data if available
        if (ColorUtility.TryParseHtmlString(planet.colorHex, out Color planetColor))
        {
            Button_Image.color = planetColor;
            Name_Image.color = planetColor;
            Line_Image.color = planetColor;
            PopUpLeft.color = planetColor;
            PopUpRight.color = planetColor;
            Alias_Background_Image.color = planetColor;
            Alias_Image.color = planetColor;
        }
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
        StartCoroutine(FadeCanvasGroup(canvasGroup, 0, 1, fadeDuration));
    }

    public void FadeOut()
    {
        StartCoroutine(FadeCanvasGroup(canvasGroup, 1, 0, fadeDuration));
    }

    public void CallImage(PlanetList.TemplatePlanet templatePlanet)
    {
        Button_Image.sprite = templatePlanet.buttonImage;
        Alias_Image.sprite = templatePlanet.buttonImage;
        Name_Image.sprite = templatePlanet.buttonImage;
        Alias_Background_Image.sprite = templatePlanet.buttonImage;
        PopUpLeft.sprite = templatePlanet.backgroundLeft;
        PopUpRight.sprite = templatePlanet.backgroundRight;
        Line_Image.sprite = templatePlanet.lineImage;
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