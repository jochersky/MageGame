using System;
using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "ReverseFootstepsSpellStrategy", menuName = "Spell Strategies/ReverseFootstepsSpellStrategy")]
public class ReverseFootstepsSpellStrategy : SpellStrategy
{
    public float spellCancelTime = 5f;
    private SpellManager _spellManager = null;
    private PlayerStateMachine _psm = null;
    private GameObject _marker;
    private Vector2 _storedPosition = Vector2.zero;
    
    private Coroutine _cancelRoutine;

    // TODO: see if builder pattern could be used instead of multiple constructors
    public override void Equip(SpellManager spellManager, PlayerStateMachine playerStateMachine)
    {
        _spellManager = spellManager;
        _psm = playerStateMachine;
    }
    
    public override void CastSpell(Transform spawnTransform, Vector3 spawnPosition)
    {
        if (_storedPosition == Vector2.zero)
        {
            _storedPosition = spawnPosition;
            _marker = Instantiate(prefab, spawnTransform);
            _marker.transform.position = spawnPosition;
            // null so that it won't follow the player's movement 
            _marker.transform.parent = null;
            _cancelRoutine = _spellManager.StartCoroutine(_spellManager.CancelSpellAfterDuration(spellCancelTime, this));
        }
        else
        {
            if (_cancelRoutine != null) _spellManager.StopCoroutine(_cancelRoutine);
            _psm.transform.position = _storedPosition;
            _storedPosition = Vector2.zero;
            Destroy(_marker);
        }
    }

    public override void Cancel()
    {
        _storedPosition = Vector2.zero;
        if (_marker) Destroy(_marker);
    }
}