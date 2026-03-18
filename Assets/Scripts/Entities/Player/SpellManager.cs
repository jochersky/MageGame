using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerStateMachine))]
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
    readonly float windForce = 75f;
    [SerializeField] float spellCooldown = 0.2f;
    [SerializeField] GameObject windSpell;
    [SerializeField] GameObject fireSpell;
    [SerializeField] GameObject lightSpell;
    Rigidbody2D player;
    PlayerStateMachine psm;
    int currentMana = 100;
    int startingMana = 100;
    bool casting = false;
    float distanceAbovePlayersHead = 2f;

    enum SPELL_NAMES
    {
        GIFT_OF_LIGHT,
        WINDLORDS_BLESSING,
        FURY_OF_THE_DRAGON
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<Rigidbody2D>();
        psm = GetComponent<PlayerStateMachine>();
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
        if (!casting)
        {
            casting = true;
            Cast(spell2);
        }
    }

    void Cast(SPELL_NAMES spellName)
    {
        switch (spellName)
        {
            case SPELL_NAMES.GIFT_OF_LIGHT: StartCoroutine(GiftOfLight()); break;
            case SPELL_NAMES.WINDLORDS_BLESSING: StartCoroutine(WindlordsBlessing()); break;
            case SPELL_NAMES.FURY_OF_THE_DRAGON: StartCoroutine(FuryOfTheDragon()); break;
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
            Instantiate(windSpell, player.position, windSpell.transform.rotation);
            player.linearVelocity = Vector2.zero;
            player.angularVelocity = 0f;
            player.AddForce(new Vector2(0f, windForce), ForceMode2D.Impulse);
        }
        yield return new WaitForSeconds(spellCooldown);
        casting = false;
    }

    private IEnumerator FuryOfTheDragon()
    {
        int manaCost = 10;
        if (currentMana < manaCost)
        {
            // spell fizzle sounds
            Debug.Log("CAN'T CAST NO MANA HAHAHAHA");
        } else
        {
            Debug.Log("CASTING FURY OF THE DRAGON");
            currentMana -= manaCost;
            UpdateMana();
            // there has to be a more elegant solution but I can't find it
            Quaternion spellRotation = fireSpell.transform.rotation;
            if (psm.PreviousDirection.x < 0)
            {
                spellRotation = Quaternion.Inverse(spellRotation);
            }
            Instantiate(fireSpell, player.position + psm.PreviousDirection, spellRotation); 
        }
        yield return new WaitForSeconds(spellCooldown);
        casting = false;
    }

    private IEnumerator GiftOfLight()
    {
        int manaCost = 25;
        if (currentMana < manaCost)
        {
            // spell fizzle sounds
            Debug.Log("CAN'T CAST NO MANA HAHAHAHA");
        } else
        {
            Debug.Log("CASTING GIFT OF LIGHT");
            currentMana -= manaCost;
            UpdateMana();
            Vector2 abovePlayersHead = new(player.position.x, player.position.y + distanceAbovePlayersHead);
            Instantiate(lightSpell, abovePlayersHead, player.transform.rotation, transform);
        }
        yield return new WaitForSeconds(spellCooldown);
        casting = false;
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

    public void AddMana(int mana)
    {
        currentMana += mana;
        UpdateMana();
    }
}
