using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;

public class Witness : NPC
{
    [SerializeField] string[] firstMeetLines;
    [SerializeField] string[] alreadyDoneLines;
    [SerializeField] string[] declineLines;
    [SerializeField] string[] agreeLines;
    [SerializeField] string offerLineBasis;
    [SerializeField] string decline;
    [SerializeField] string accept;
    [SerializeField] List<string> offerWords;
    readonly string offer = "offer";
    Offer currentOffer;
    Offer currentReward;
    string[] offerLines = new string[1];
    readonly int numOffers = 5;
    enum Offer
    {
        HEALTH,
        POWER,
        WEALTH,
       // VISION,
       // WISDOM
    }
    Dictionary<Offer, string> offerMap = new();

     // player managers
    InventoryManager inventoryManager;
    Health playerHealth;
    SpellManager spellManager;


    override protected void Start()
    {
        base.Start();
        offerMap[Offer.HEALTH] = offerWords[0];
        offerMap[Offer.POWER] = offerWords[1];
        offerMap[Offer.WEALTH] = offerWords[2];
        //offerMap[Offer.VISION] = offerWords[3];
        //offerMap[Offer.WISDOM] = offerWords[4];
        RollOffer();
        ResetToDefaultDialogueOptions();
    }

    protected override void PlayerSetup(Player _player)
    {
        base.PlayerSetup(_player);
        inventoryManager = _player.GetComponent<InventoryManager>();
        playerHealth = _player.GetComponent<Health>();
        spellManager = _player.GetComponent<SpellManager>();
    }
    void RollOffer()
    {
        System.Random randy = new();
        List<Offer> offers = offerMap.Keys.ToList<Offer>();
        int rand_idx = randy.Next(offers.Count);
        print(rand_idx);
        currentOffer = offers[rand_idx];
        offers.RemoveAt(rand_idx);
        rand_idx = randy.Next(offers.Count);
        currentReward = offers[rand_idx];
        offerLines[0] = offerLineBasis + offerMap[currentOffer] + " for " + offerMap[currentReward] + ". Do you accept?";
    }
    void Exchange()
    {
        PlayNewLines(agreeLines);
        dialogueBox.GetComponent<DialogueBox>().ClearDialogueOptions();
        switch (currentOffer)
        {
            // cut max health in half
            case Offer.HEALTH:
                playerHealth.UpdateMaxHealth(playerHealth.MaxHealth / 2); break;
            // cut max mana in half
            case Offer.POWER:
                spellManager.MaxMana /= 2; break;
            // take all of their money
            case Offer.WEALTH:
                inventoryManager.UpdateMoney(-inventoryManager.GetMoneyCount()); break;
            //case Offer.WISDOM:
            //default:
        }
        switch (currentReward)
        {
            case Offer.HEALTH:
                playerHealth.UpdateMaxHealth(playerHealth.MaxHealth * 2); break;
            // cut max mana in half
            case Offer.POWER:
                spellManager.MaxMana *= 2; break;
            // take all of their money
            case Offer.WEALTH:
                inventoryManager.UpdateMoney(100); break;
        }
    }

    void Exchange(Offer offer, Offer reward)
    {
        
    }

    protected override void ResetToDefaultDialogueOptions()
    {
        base.ResetToDefaultDialogueOptions();
        dialogue = offerLines;
        currentDialogueOptions[decline] = CloseDialogue;
        currentDialogueOptions[accept] = Exchange;
    }
}
