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
    
    public void InitializeBar(int initialValue, int maxValue)
    {
        slider.maxValue = maxValue;
        Vector2 temp = fillBarRectTransform.offsetMax;
        fillBarRectTransform.offsetMax = new Vector2(-startingRightVal - rightGrowAmt * maxValue, temp.y);
        
        
        // // grow by tick segment increments
        // if (growBySegment)
        // {
        // }
        // // grow purely by value
        // else
        // {
        //     fillBarRectTransform.offsetMax = new Vector2(-startingRightVal - rightGrowAmt * maxValue, temp.y);
        // }
        
        
        
        UpdateValue(initialValue);
    }
    
    public void UpdateValue(int newValue)
    {
        slider.value = newValue;
        counterText.text = newValue + " | " + slider.maxValue;
    }
}
