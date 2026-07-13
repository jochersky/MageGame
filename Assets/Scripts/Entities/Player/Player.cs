using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private BaseStats stats;
    private Health _health;
    private SpellManager _spellManager;
    
    private void Awake()
    {
        GameManager.Instance.Player = this;
    }

    private void Start()
    {
        _health = GetComponent<Health>();
        _spellManager = GetComponent<SpellManager>();
        
        GameManager.Instance.PlayerHealth = _health;
        GameManager.Instance.SpellManager = _spellManager;

        _health.UpdateMaxHealth(stats.health);
        _spellManager.MaxMana = stats.mana;
        _spellManager.Mana = _spellManager.MaxMana;
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