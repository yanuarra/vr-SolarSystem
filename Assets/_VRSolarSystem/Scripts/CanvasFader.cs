using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CanvasFader : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private GameObject[] children;
    [SerializeField] private Button nextButton;
    public UnityEvent OnOnboardingDone;

    private int currentIndex = 0;

    private void Start()
    {
        // Ensure all children are inactive except the first one
        for (int i = 0; i < children.Length; i++)
        {
            children[i].SetActive(i == currentIndex);
            if (i == currentIndex)
            {
                CanvasGroup cg = children[i].GetComponent<CanvasGroup>();
                if (cg != null)
                {
                    cg.alpha = 1;
                }
            }
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

    public void ShowNextChild()
    {
        //int nextIndex = (currentIndex + 1) % children.Length;
        int nextIndex = currentIndex+1;
        if (nextIndex >= children.Length)
        {
            OnOnboardingDone?.Invoke();
            gameObject.SetActive(false);
            return;
        }
        StartCoroutine(TransitionToChild(children[currentIndex], children[nextIndex]));
        currentIndex = nextIndex;
    }

    private IEnumerator TransitionToChild(GameObject currentChild, GameObject nextChild)
    {
        nextButton.interactable = false;
        if (currentChild != null)
        {
            CanvasGroup currentCanvasGroup = currentChild.GetComponent<CanvasGroup>();
            if (currentCanvasGroup != null)
            {
                Debug.Log($"Fading out {currentChild.name}");
                yield return FadeCanvasGroup(currentCanvasGroup, 1, 0, fadeDuration);
                currentChild.SetActive(false);
                Debug.Log($"{currentChild.name} is now inactive");
            }
        }

        nextChild.SetActive(true);
        CanvasGroup nextCanvasGroup = nextChild.GetComponent<CanvasGroup>();
        if (nextCanvasGroup != null)
        {
            Debug.Log($"Fading in {nextChild.name}");
            yield return FadeCanvasGroup(nextCanvasGroup, 0, 1, fadeDuration);
        }
        nextButton.interactable = true;
    }
}