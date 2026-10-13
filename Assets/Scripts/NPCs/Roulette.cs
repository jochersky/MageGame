using Unity.VisualScripting;
using UnityEngine;

public class Roulette : NPC
{
    // lines
    [SerializeField] string[] greetingLines;
    [SerializeField] string[] declineLines;
    [SerializeField] string[] notEnoughMoneyLines;
    [SerializeField] string[] fiveKindLines;
    [SerializeField] string[] fourKindLines;
    [SerializeField] string[] threeKindLines;
    [SerializeField] string[] twoPairLines;
    [SerializeField] string[] onePairLines;
    [SerializeField] string[] fullHouseLines;
    [SerializeField] string[] straightLines;
    [SerializeField] string[] bustLines;
    [SerializeField] string decline;
    [SerializeField] string wager1;
    [SerializeField] string wager5;
    [SerializeField] string wager10;

    // player managers
    InventoryManager inventoryManager;
    Health playerHealth;
    SpellManager spellManager;

    // Payouts
    const int BUST_DAMAGE = 1;
    const int TWO_PAIR_PAYOUT = 2;
    const int THREE_KIND_PAYOUT = 5;
    const int FOUR_KIND_PAYOUT = 10;
    const int FIVE_KIND_PAYOUT = 50;


    public override void Start()
    {
        base.Start();
        ResetToDefaultDialogueOptions();
        
    }

    protected override void PlayerSetup(Player _player)
    {
        base.PlayerSetup(_player);
        inventoryManager = _player.GetComponent<InventoryManager>();
        playerHealth = _player.GetComponent<Health>();
        spellManager = _player.GetComponent<SpellManager>();
    }


    // rather than rolling actual dice, we use approximate probabilities
    void Roll(int wager)
    {
        
        ResetToDefaultDialogueOptions();
        // charge the player
        if (inventoryManager == null)
        {
            print("Error: Can't find inventory manager");
            return;
        }
        if (inventoryManager.GetMoneyCount() < wager)
        {
            PlayNewLines(notEnoughMoneyLines);
            return;
        }
        inventoryManager.UpdateMoney(-wager);
        System.Random randy = new();
        int roll = randy.Next(0, 10000);
        if (roll <= 617)
        {
            PlayNewLines(bustLines);
            if (playerHealth == null)
            {
                print("Error: No Health component found");
            } else
            {
                playerHealth.CurrentHealth = playerHealth.CurrentHealth - BUST_DAMAGE;
            }
        } else if (roll <= 5247)
        {
            PlayNewLines(onePairLines);
        } else if (roll <= 7562)
        {
            PlayNewLines(twoPairLines);
            inventoryManager.UpdateMoney(TWO_PAIR_PAYOUT * wager);
        } else if (roll <= 9105)
        {
            PlayNewLines(threeKindLines);
            inventoryManager.UpdateMoney(THREE_KIND_PAYOUT * wager);
        } else if (roll <= 9414)
        {
            PlayNewLines(straightLines);
            inventoryManager.UpdateMoney(wager);
        } else if (roll <= 9800)
        {
            PlayNewLines(fullHouseLines);
            inventoryManager.UpdateMoney(wager);
            if (playerHealth == null || spellManager == null)
            {
                print("Error: No Health or SpellManager component found");
            } else
            {
                playerHealth.Heal(playerHealth.MaxHealth);
                spellManager.UpdateMana(spellManager.MaxMana);
            }
        } else if (roll <= 9993)
        {
            PlayNewLines(fourKindLines);
            inventoryManager.UpdateMoney(FOUR_KIND_PAYOUT * wager);
        } else
        {
            PlayNewLines(fiveKindLines);
            inventoryManager.UpdateMoney(FIVE_KIND_PAYOUT * wager);
        }
    }

    void RollWager1()
    {
        Roll(1);
    }

    void RollWager5()
    {
        Roll(5);
    }

    void RollWager10()
    {
        Roll(10);
    }

    protected override void ResetToDefaultDialogueOptions()
    {
        base.ResetToDefaultDialogueOptions();
        dialogue = greetingLines;
        currentDialogueOptions[decline] = CloseDialogue;
        currentDialogueOptions[wager1] = RollWager1;
        currentDialogueOptions[wager5] = RollWager5;
        currentDialogueOptions[wager10] = RollWager10;
    }
}
