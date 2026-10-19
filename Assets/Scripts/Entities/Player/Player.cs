using System;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [Header("Stats/Effects")]
    [SerializeField] private BaseStats stats;
    [SerializeField] private PassiveSpellAffects passiveSpellAffects;
    [Header("References")]
    [SerializeField] private Transform lightTransform;
    [SerializeField] private PlayerTorch playerTorch;
    private Health _health;
    private SpellManager _spellManager;
    private PlayerStateMachine _psm;
    [Header("Player Stats")]
    [SerializeField] private int embersForWish;

    public HUDBar HealthBar { get; set; }
    public HUDBar ManaBar { get; set; }
    public Health Health => _health;
    public int Embers => _embers;
    public bool CanGetWish => _embers >= embersForWish;
    
    public delegate void StartDone();
    public event StartDone OnStartDone;

    public delegate void EmbersUpdated(int amt);
    public event EmbersUpdated OnEmbersUpdated;

    private int _embers;
    
    private void Awake()
    {
        // GameManager.Instance.Player = this;
        
        _health = GetComponent<Health>();
        _spellManager = GetComponent<SpellManager>();
        _psm = GetComponent<PlayerStateMachine>();
        
        GameManager.Instance.PlayerHealth = _health;
        GameManager.Instance.SpellManager = _spellManager;
    }

    private void Start()
    {
        // _health = GetComponent<Health>();
        // _spellManager = GetComponent<SpellManager>();
        
        // GameManager.Instance.PlayerHealth = _health;
        // GameManager.Instance.SpellManager = _spellManager;

        // _health.CurrentHealth = stats.health;
        // _health.UpdateMaxHealth(stats.health);
        // _spellManager.MaxMana = stats.mana;
        // _spellManager.Mana = _spellManager.MaxMana;
        
        UpdateHealthAndMana();

        float size = stats.lightRadiusSize + passiveSpellAffects.LightRadiusDiff;
        lightTransform.localScale = new Vector3(size, size, size);

        passiveSpellAffects.OnLightRadiusUpdated += (lightRadiusDiff) =>
        {
            float newSize = stats.lightRadiusSize + lightRadiusDiff;
            if (lightTransform) lightTransform.localScale = new Vector3(newSize, newSize, newSize);
        };

        playerTorch.OnExpGained += UpdateEmbers;

        if (HealthBar)
        {
            HealthBar.InitializeBar(_health.CurrentHealth, _health.MaxHealth);
            _health.OnHealthChanged += HealthBar.UpdateValue;
        }
        if (ManaBar)
        {
            ManaBar.InitializeBar(_spellManager.Mana, _spellManager.MaxMana);
            _spellManager.OnManaChanged += ManaBar.UpdateValue;
        }
        
        OnStartDone?.Invoke();
    }

    public void UpdateEmbers(int amt)
    {
        _embers += amt;
        OnEmbersUpdated?.Invoke(_embers);
    }

    public bool LevelUp(Wish wish)
    {
        if (CanGetWish)
        {
            UpdateEmbers(-embersForWish);
            BasicStatModifier wishModifier = 
                new BasicStatModifier(wish.statType, wish.duration, s => s + wish.increaseAmt,
                    $"{wish.statType.ToString()} wish", true);
            _psm.Stats.Mediator.AddModifier(wishModifier);
            SyncStats();
            return true;
        }
        
        return false;
    }

    private void SyncStats()
    {
        // Update the max stat values for when they're updated (i.e. when making wish)
        // _health.UpdateMaxHealth((int)_psm.Stats.Health);
        // _spellManager.UpdateMaxMana((int)_psm.Stats.Mana);
        HealthBar.UpdateMaxValue((int)_psm.Stats.Health);
        ManaBar.UpdateMaxValue((int)_psm.Stats.Mana);
    }

    public void UpdateHealthAndMana()
    {
        _health.CurrentHealth = stats.health;
        _health.UpdateMaxHealth(stats.health);
        _spellManager.MaxMana = stats.mana;
        _spellManager.Mana = _spellManager.MaxMana;
    }

    public void Save(ref PlayerSaveData data)
    {
        data.healthAmt = _health.CurrentHealth;
        data.manaAmt = _spellManager.Mana;
        data.moneyAmt = InventoryManager.Instance.GetMoneyCount();
    }

    public void Load(ref PlayerSaveData data)
    {
        _health.CurrentHealth = data.healthAmt;
        _spellManager.Mana = data.manaAmt;
        InventoryManager.Instance.UpdateMoney(data.moneyAmt);
    }
}

[System.Serializable]
public struct PlayerSaveData
{
    public int healthAmt;
    public int manaAmt;
    public int moneyAmt;
}