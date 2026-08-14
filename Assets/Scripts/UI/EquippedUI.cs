using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquippedUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI consumableCountText;
    [SerializeField] private Image consumableImage;
    [SerializeField] private Image spell1Image;
    [SerializeField] private Image spell1Cooldown;
    [SerializeField] private Image spell2Image;
    [SerializeField] private Image spell2Cooldown;

    private ConsumableConfig _consumableConfig1;
    private ConsumableConfig _consumableConfig2;
    private ConsumableConfig _equippedConsumable;
    private Material _spell1CooldownMat;
    private Material _spell2CooldownMat;

    private static readonly int Progress = Shader.PropertyToID("_Progress");

    private void Start()
    {
        _spell1CooldownMat = spell1Cooldown.material;
        _spell2CooldownMat = spell2Cooldown.material;
    }

    private void OnEnable()
    {
        // Subscribe to events
        InventoryManager.Instance.OnConsumableSwitched += UpdateEquippedConsumableUI;
        InventoryManager.Instance.OnConsumableCountUpdated += UpdateConsumableCountUI;
        InventoryManager.Instance.OnSpell1Equipped += UpdateEquippedSpell1UI;
        InventoryManager.Instance.OnSpell2Equipped += UpdateEquippedSpell2UI;
        InventoryManager.Instance.OnSpell1Unequipped += UpdateEquippedSpell1UI;
        InventoryManager.Instance.OnSpell2Unequipped += UpdateEquippedSpell2UI;
        GameManager.Instance.SpellManager.OnSpell1Casted += cooldown => StartCoroutine(CooldownSpell1(cooldown));
        GameManager.Instance.SpellManager.OnSpell2Casted += cooldown => StartCoroutine(CooldownSpell2(cooldown));
        
    }

    private void UpdateEquippedConsumableUI(ConsumableConfig config, int amount)
    {
        consumableImage.enabled = true;
        _equippedConsumable = config;
        UpdateConsumableCountUI(config, amount);
        consumableImage.sprite = config.icon;
    }
    
    private void UpdateConsumableCountUI(ConsumableConfig config, int count)
    {
        // don't update the text if the config isn't the same as the equipped config
        if (config != _equippedConsumable) return;
        consumableCountText.text = count.ToString();
    }
    
    private void UpdateEquippedSpell1UI(Sprite spellSprite, bool visible)
    {
        spell1Image.color = visible ? Color.white : Color.clear;
        spell1Image.sprite = spellSprite;
    }

    private void UpdateEquippedSpell2UI(Sprite spellSprite, bool visible)
    {
        spell2Image.color = visible ? Color.white : Color.clear;
        spell2Image.sprite = spellSprite;
    }

    IEnumerator CooldownSpell1(float duration)
    {
        spell1Cooldown.enabled = true;
        float t = 0;
        while (t < duration)
        {
            float progress = 100 * (1 - (t / duration));
            _spell1CooldownMat.SetFloat(Progress, progress);
            
            t += Time.deltaTime;
            yield return null;
        }
        spell1Cooldown.enabled = false;
    }
    
    IEnumerator CooldownSpell2(float duration)
    {
        spell2Cooldown.enabled = true;
        float t = 0;
        while (t < duration)
        {
            float progress = 100 * (1 - (t / duration));
            _spell2CooldownMat.SetFloat(Progress, progress);
            
            t += Time.deltaTime;
            yield return null;
        }
        spell2Cooldown.enabled = false;
    }
}
