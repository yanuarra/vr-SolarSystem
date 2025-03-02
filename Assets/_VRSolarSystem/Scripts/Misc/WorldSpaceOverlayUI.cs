using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode] //Disable if you don't care about previewing outside of play mode
public class WorldSpaceOverlayUI : MonoBehaviour
{
    private const string shaderTestMode = "unity_GUIZTestMode"; //The magic property we need to set
    [SerializeField] UnityEngine.Rendering.CompareFunction desiredUIComparison = UnityEngine.Rendering.CompareFunction.Always; //If you want to try out other effects
    [Tooltip("Set to blank to automatically populate from the child UI elements")]
    [SerializeField] List<Graphic> uiElementsToApplyTo = new List<Graphic>();
    //Allows us to reuse materials
    private Dictionary<Material, Material> materialMappings = new Dictionary<Material, Material>();

    private void Start()
    {
        Init();
    }

    public void AddElements(Graphic graphic)
    {
        uiElementsToApplyTo.Add(graphic);
    }

    protected virtual void Init()
    {
        if (uiElementsToApplyTo.Count == 0)
        {
            uiElementsToApplyTo = gameObject.GetComponentsInChildren<Graphic>().ToList();
        }
        foreach (var graphic in uiElementsToApplyTo)
        {
            Material material = graphic.materialForRendering;
            if (material == null)
            {
                Debug.LogError($"{nameof(WorldSpaceOverlayUI)}: skipping target without material {graphic.name}.{graphic.GetType().Name}");
                continue;
            }
            if (!materialMappings.TryGetValue(material, out Material materialCopy))
            {
                materialCopy = new Material(material);
                materialMappings.Add(material, materialCopy);
            }
            materialCopy.SetInt(shaderTestMode, (int)desiredUIComparison);
            graphic.material = materialCopy;
        }
    }
}