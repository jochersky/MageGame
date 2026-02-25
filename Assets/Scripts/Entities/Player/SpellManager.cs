using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class SpellManager : MonoBehaviour
{
    [SerializeField] Image manaSymbol;
    [SerializeField] Sprite manaFullIcon;
    [SerializeField] Sprite mana75Icon;
    [SerializeField] Sprite manaHalfIcon;
    [SerializeField] Sprite mana25Icon;
    [SerializeField] Sprite manaEmptyIcon;
    [SerializeField] SPELL_NAMES spell1;
    [SerializeField] SPELL_NAMES spell2;
    [SerializeField] float windForce = 10f;
    [SerializeField] float spellCooldown = 0.2f;
    Rigidbody2D player;
    int currentMana = 100;
    int startingMana = 100;
    bool casting = false;

    enum SPELL_NAMES
    {
        GIFT_OF_LIGHT,
        WINDLORDS_BLESSING
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<Rigidbody2D>();
        currentMana = startingMana;
    }

    public void OnSpell1Cast()
    {
        if (!casting)
        {
            casting = true;
            Cast(spell1);
        }
    }

    public void OnSpell2Cast()
    {
        Cast(spell2);
    }

    void Cast(SPELL_NAMES spellName)
    {
        switch (spellName)
        {
            case SPELL_NAMES.GIFT_OF_LIGHT: GiftOfLight(); break;
            case SPELL_NAMES.WINDLORDS_BLESSING: StartCoroutine(WindlordsBlessing()); break;
        }
    }

    // maybe this should be a class thing, not a method thing

    private IEnumerator WindlordsBlessing()
    {
        int manaCost = 25;
        if (currentMana < manaCost)
        {
            // spell fizzle sounds
            Debug.Log("CAN'T CAST NO MANA HAHAHAHA");
        } else
        {
            Debug.Log("CASTING WINDLORD'S BLESSING");
            currentMana -= manaCost;
            UpdateMana();
            player.linearVelocity = Vector2.zero;
            player.angularVelocity = 0f;
            player.AddForce(new Vector2(0f, windForce), ForceMode2D.Impulse);
        }
        yield return new WaitForSeconds(spellCooldown);
        casting = false;
    }

    private void GiftOfLight()
    {
        Debug.Log("CASTING GIFT OF LIGHT");
    }


    void UpdateMana()
    {
        if (currentMana > 75)
        {
            manaSymbol.sprite = manaFullIcon;
        } else if (currentMana > 50)
        {
            manaSymbol.sprite = mana75Icon;
        } else if (currentMana > 25)
        {
            manaSymbol.sprite = manaHalfIcon;
        } else if (currentMana > 0)
        {
            manaSymbol.sprite = mana25Icon;
        } else
        {
            manaSymbol.sprite = manaEmptyIcon;
        }
    }
}
