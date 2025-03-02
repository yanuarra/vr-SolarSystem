using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

public class ChangeOutlineHandler : MonoBehaviour
{
    public List<Color> colors;  // Array of materials to cycle through
    private Outline rend;
    private int currentIndex = 0;
    private float changeInterval = 0.6f; // Change material every 0.1 seconds
    private float delay = 0.3f; // Delay after changing more than 4 times
    private int changeCount = 0;
    private float timer = 0f;
    public bool isBlinking;
    public enum OutlineType
    {
        isBlinking,
        isAlwaysOn
    }

    public OutlineType type;

    void Start()
    {
        rend = GetComponent<Outline>();
        if (colors.Count == 0)
        {
            colors.Add(new Color(rend.OutlineColor.r, rend.OutlineColor.g, rend.OutlineColor.b, 255f));
            colors.Add(new Color(rend.OutlineColor.r, rend.OutlineColor.g, rend.OutlineColor.b, 0f));
        }
        else
        {
            rend.OutlineColor = colors[currentIndex];
        }
    }

    void Update()
    {
        //rend.enabled = isBlinking;
        rend.enabled = type == OutlineType.isAlwaysOn? true : false;
        if (!isBlinking) return;
        timer += Time.deltaTime;

        if (timer >= changeInterval)
        {
            // Change to the next material
            currentIndex = (currentIndex + 1) % colors.Count;
            rend.OutlineColor = colors[currentIndex];
            timer = 0f; // Reset the timer

            changeCount++;

            // Check if it's time to add a delay
            if (changeCount > 3)
            {
                Invoke("ApplyDelay", delay);
                changeCount = 0;
            }
        }
    }

    void ApplyDelay()
    {
        // Do nothing for 'delay' seconds (0.3 seconds in this case)
    }

    public void OutlineIsBlinking()
    {
        isBlinking = true;
    }

    public void OutlineNotBlinking()
    {
        isBlinking = false;
    }

}
