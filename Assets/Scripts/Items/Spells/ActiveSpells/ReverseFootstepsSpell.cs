using System.Collections;
using UnityEngine;

public class ReverseFootstepsSpell : ActiveSpell
{
    private PlayerStateMachine _psm;
    private Vector2 _storedPosition;
    private GameObject _marker;
    
    public override void OnEquip()
    {
        _psm = SpellManager.Instance.psm;
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
        }
        else
        {
            _psm.transform.position = _storedPosition;
            _storedPosition = Vector2.zero;
            Destroy(_marker);
        }
        
        yield return new WaitForSeconds(spellCooldown);
        casting = false;
    }
}
