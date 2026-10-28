using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform fillBarRectTransform;
    [SerializeField] private TextMeshProUGUI counterText;
    [SerializeField] private Slider slider;
    
    [Header("Options")] 
    [SerializeField] private bool growBySegment;
    [SerializeField] private float startingRightVal;
    [SerializeField] private float rightGrowAmt;
    [SerializeField] private float maxWidth;
    
    public void InitializeBar(int initialValue, int maxValue)
    {
        slider.maxValue = maxValue;
        Vector2 temp = fillBarRectTransform.offsetMax;
        // Width of the bar shouldn't surpass the max width, will continue to increase max value though.
        float width = Mathf.Min(-startingRightVal - rightGrowAmt * maxValue, maxWidth);
        fillBarRectTransform.offsetMax = new Vector2(width, temp.y);
        
        UpdateValue(initialValue);
    }
    
    public void UpdateValue(int newValue)
    {
        slider.value = Mathf.Clamp(newValue, slider.minValue, slider.maxValue);
        counterText.text = newValue + " | " + slider.maxValue;
    }

    public void UpdateMaxValue(int newMax)
    {
        InitializeBar((int)slider.value, newMax);
    }
}
