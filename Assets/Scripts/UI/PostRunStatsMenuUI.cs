using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PostRunStatsMenuUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject menuUI;
    [SerializeField] private TextMeshProUGUI runOutcomeText;
    [SerializeField] private TextMeshProUGUI mainStatsText;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private Image characterPortrait;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private GameObject spellsGrid;
    private GameObject[] _postRunSpellIcons = Array.Empty<GameObject>();
    
    [Header("Character Portraits")]
    [SerializeField] private Sprite pyroPortrait;
    [SerializeField] private Sprite houndPortrait;
    [SerializeField] private Sprite wardenPortrait;

    private void Awake()
    {
        // Get reference to all spell icons in grid
        _postRunSpellIcons = new GameObject[spellsGrid.transform.childCount];
        for (int i = 0; i < spellsGrid.transform.childCount; i++)
            _postRunSpellIcons[i] = spellsGrid.transform.GetChild(i).gameObject;
    }
    
    private void Start()
    {
        menuUI.SetActive(false);
    }
    
    public void ShowMenu()
    {
        UpdateInfo();
        menuUI.SetActive(true);
    }

    private void UpdateInfo()
    {
        
    }

    public void Load(ref SaveSystem.SaveData data)
    {
        GameSaveData runData = data.gameSaveData;
        
        mainStatsText.text = $"Stage: {runData.currentStage}   " +
                             $"Total Time: {GetFormattedTime(runData.runTime)}   " +
                             $"Seed: {runData.runSeed}";
        
        statsText.text = "";
        statsText.text += $"~ Enemies Slain: {runData.enemiesSlain}\n";
        statsText.text += $"~ Damage Taken: {runData.damageTaken}\n";
        statsText.text += $"~ Money Collected: {runData.moneyCollected}\n";
        statsText.text += $"~ Wishes Made: {runData.wishesMade}\n";
        
        switch (runData.characterType)
        {
            case CharacterType.Base: characterPortrait.sprite = pyroPortrait; break;
            case CharacterType.Pyromancer: characterPortrait.sprite = pyroPortrait; break;
            case CharacterType.Hound: characterPortrait.sprite = houndPortrait; break;
            case CharacterType.Warden:characterPortrait.sprite = wardenPortrait; break;
        }

        InventorySaveData itemData = data.inventoryData;

        if (_postRunSpellIcons.Length == 0) return;
        for (int i = 0; i < _postRunSpellIcons.Length; i++)
        {
            if (i > itemData.spells.Length - 1)
            {
                // Hide any spell icons that were not populated
                _postRunSpellIcons[i].SetActive(false);
            }
            else
            {
                // Populate spell icons with spells collected during the run
                GameObject spellIconGO = _postRunSpellIcons[i].transform.GetChild(0).gameObject;
                if (spellIconGO.TryGetComponent<Image>(out var image))
                {
                    Sprite spellSprite = InventoryManager.Instance.SpellDictionary.GetConfig(itemData.spells[i].name).icon;
                    image.sprite = spellSprite;
                    image.enabled = true;
                } 
            }
        }
    }

    private string GetFormattedTime(float time)
    {
        // TODO: make format look more like time 
        return "" + Mathf.Round(time * 100) / 100;
    }
}
