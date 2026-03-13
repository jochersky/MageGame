using TMPro;
using UnityEngine;

public class SpellListItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI spellName;

    public void Initialize(Spell spell)
    {
        spellName.text = spell.spellType.ToString();
    }
}
