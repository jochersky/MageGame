using System.Collections.Generic;
using UnityEditor.Rendering.BuiltIn.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;

public class ConsumableManager : MonoBehaviour
{
    [SerializeField] private Transform consumableSpawnTransform;
    [SerializeField] private Transform consumableParentTransform;
    [SerializeField] private ConsumableConfig bombConfig;
    [SerializeField] private int bombStartCount;
    
    private PlayerStateMachine _psm;
    private LayerMask _layerMask;

    public ConsumableConfig consumableConfig1;
    public ConsumableConfig consumableConfig2;

    private ConsumableConfig _equippedConsumable;

    // TODO: move dictionary to config file to have counts persist between levels
    private Dictionary<string, int> consumableCounts = new();

    private void Start()
    {
        _psm = GetComponent<PlayerStateMachine>();
        _layerMask = LayerMask.GetMask("Environment");
    }

    public void InitializeCounts()
    {
        // Always want to start run with some bombs.
        // Only want to add bombs if there aren't any bombs already.
        AddConsumable(bombConfig, bombStartCount);
        InventoryManager.Instance.EquipConsumableToSlot1(bombConfig);
    }

    public int AddConsumable(ConsumableConfig consumableConfig, int count)
    {
        // consumable already exists in inventory, just update count
        if (!consumableCounts.TryAdd(consumableConfig.itemName, count))
            consumableCounts[consumableConfig.itemName] += count;
        else
            InventoryManager.Instance.OnNewConsumableAdded(consumableConfig);

        int consumableEquipped = 0;
        if (!consumableConfig1 && consumableConfig2 != consumableConfig)
        {
            EquipConsumable1(consumableConfig);
            consumableEquipped = 1;
            InventoryManager.Instance.UpdateConsumableEquipped(_equippedConsumable, consumableCounts[_equippedConsumable.itemName]);
        }
        else if (!consumableConfig2 && consumableConfig1 != consumableConfig)
        {
            EquipConsumable2(consumableConfig);
            consumableEquipped = 2;
            InventoryManager.Instance.UpdateConsumableEquipped(_equippedConsumable, consumableCounts[_equippedConsumable.itemName]);
        }
        
        // if (consumableConfig == _equippedConsumable)
        InventoryManager.Instance.UpdateConsumableCount(consumableConfig, consumableCounts[consumableConfig.itemName]);
        
        return consumableEquipped;
    }

    public void EquipConsumable1(ConsumableConfig consumableConfig)
    {
        consumableConfig1 = consumableConfig;
        if (!_equippedConsumable) 
            _equippedConsumable = consumableConfig1;
    }
    
    public void EquipConsumable2(ConsumableConfig consumableConfig)
    {
        consumableConfig2 = consumableConfig;
        if (!_equippedConsumable)
            _equippedConsumable = consumableConfig2;
    }

    public void UnequipConsumable(int consumableID)
    {
        switch (consumableID)
        {
            case 1:
                if (_equippedConsumable == consumableConfig1) _equippedConsumable = null;
                consumableConfig1 = null;
                break;
            case 2:
                if (_equippedConsumable == consumableConfig2) _equippedConsumable = null;
                consumableConfig2 = null;
                break;
        }
    }

    public void OnUseConsumablePressed(InputAction.CallbackContext context)
    {
        if (!_equippedConsumable) return;
        
        int count = consumableCounts[_equippedConsumable.itemName];
        if (context.performed || context.canceled || _psm.IsDead || count <= 0) return;

        ConsumableStrategy strategy = _equippedConsumable.strategy;
        bool changePos = _equippedConsumable.changePositionOnObstruction;
        bool placeAtCenter = changePos &&
                             Physics2D.Raycast(_psm.transform.position, _psm.PreviousDirection, 1, _layerMask);
        
        if (_equippedConsumable.type == ConsumableType.Base)
        {
            if (placeAtCenter) strategy.UseConsumable(_psm.transform, _psm.transform.position);
            else strategy.UseConsumable(consumableParentTransform, consumableSpawnTransform.position);
        }
        else if (_equippedConsumable.type == ConsumableType.Placeable)
        {
            if (placeAtCenter) strategy.UsePlaceableConsumable(_psm.transform, _psm.transform.position);
            else strategy.UsePlaceableConsumable(consumableParentTransform, consumableSpawnTransform.position);
        }
        else if (_equippedConsumable.type == ConsumableType.Throwable)
        {
            if (placeAtCenter) strategy.UseThrowingConsumable(_psm.transform, _psm.transform.position, Vector3.zero, Vector3.zero);
            else strategy.UseThrowingConsumable(_psm.transform, _psm.transform.position, _psm.PreviousDirection, _psm.Rigidbody.linearVelocity);
        }
        
        consumableCounts[_equippedConsumable.itemName]--;
        int consumableEquipped = _equippedConsumable == consumableConfig1 ? 1 : 2;
        InventoryManager.Instance.UpdateConsumableCount(_equippedConsumable, consumableCounts[_equippedConsumable.itemName]);
    }

    public int GetConsumableCount(ConsumableConfig config)
    {
        
        return config ? consumableCounts[config.itemName] : 0;
    }

    public int UpdateConsumableCount(ConsumableConfig config, int amount)
    {
        consumableCounts[config.itemName] += amount;
        return consumableCounts[config.itemName];
    }

    public void OnSwitchEquippedConsumable(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled || !consumableConfig1 || !consumableConfig2) return;
        
        _equippedConsumable = _equippedConsumable == consumableConfig1 ? consumableConfig2 : consumableConfig1;
        int consumableEquipped = _equippedConsumable == consumableConfig1 ? 1 : 2;
        
        InventoryManager.Instance.UpdateConsumableEquipped(_equippedConsumable, consumableCounts[_equippedConsumable.itemName]);
    }
}
