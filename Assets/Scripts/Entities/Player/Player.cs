using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private BaseStats stats;
    private Health _health;
    private SpellManager _spellManager;

    public HUDBar HealthBar { get; set; }
    public HUDBar ManaBar { get; set; }
    
    public delegate void StartDone();
    public event StartDone OnStartDone;
    
    private void Awake()
    {
        GameManager.Instance.Player = this;
        
        _health = GetComponent<Health>();
        _spellManager = GetComponent<SpellManager>();
        
        GameManager.Instance.PlayerHealth = _health;
        GameManager.Instance.SpellManager = _spellManager;
    }

    private void Start()
    {
        // _health = GetComponent<Health>();
        // _spellManager = GetComponent<SpellManager>();
        
        // GameManager.Instance.PlayerHealth = _health;
        // GameManager.Instance.SpellManager = _spellManager;

        _health.CurrentHealth = stats.health;
        _health.UpdateMaxHealth(stats.health);
        _spellManager.MaxMana = stats.mana;
        _spellManager.Mana = _spellManager.MaxMana;

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

    public void Save(ref PlayerSaveData data)
    {
        data.position = transform.position;
        data.healthAmt = _health.CurrentHealth;
        data.manaAmt = _spellManager.Mana;
        data.moneyAmt = InventoryManager.Instance.GetMoneyCount();
    }

    public void Load(ref PlayerSaveData data)
    {
        transform.position = data.position;
        _health.CurrentHealth = data.healthAmt;
        _spellManager.Mana = data.manaAmt;
        InventoryManager.Instance.UpdateMoney(data.moneyAmt);
    }
}

[System.Serializable]
public struct PlayerSaveData
{
    public Vector3 position;
    public int healthAmt;
    public int manaAmt;
    public int moneyAmt;
}