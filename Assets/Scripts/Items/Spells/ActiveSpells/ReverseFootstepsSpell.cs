using System.Collections;
using UnityEngine;

public class ReverseFootstepsSpell : ActiveSpell
{
    [SerializeField] private float spellCancelTime = 5f;
    private PlayerStateMachine _psm;
    private Vector2 _storedPosition;
    private GameObject _marker;
    
    private Coroutine _cancelRoutine;
    
    public override void OnEquip()
    {
        // _psm = SpellManager.Instance.psm;
    }
    
    public override void CastSpell()
    {
        if (casting) return;
        StartCoroutine(ReverseFootsteps());
    }
    
    private IEnumerator ReverseFootsteps()
    {
        casting = true;
        
        if (_storedPosition == Vector2.zero)
        {
            _storedPosition = _psm.transform.position;
            _marker = Instantiate(projectilePrefab, parentTransform);
            _marker.transform.position = _psm.transform.position;
            _cancelRoutine = StartCoroutine(CancelSpellAfterDuration());
        }
        else
        {
            StopCoroutine(_cancelRoutine);
            _psm.transform.position = _storedPosition;
            _storedPosition = Vector2.zero;
            Destroy(_marker);
        }
        
        yield return new WaitForSeconds(spellCooldown);
        casting = false;
    }

    private IEnumerator CancelSpellAfterDuration()
    {
        yield return new WaitForSeconds(spellCancelTime);
        _storedPosition = Vector2.zero;
        if (_marker) Destroy(_marker);
    }
}
