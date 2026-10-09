using UnityEngine;

public class Roulette : NPC
{
    [SerializeField] string[] declineLines;
    [SerializeField] string[] fiveKindLines;
    [SerializeField] string[] fourKindLines;
    [SerializeField] string[] threeKindLines;
    [SerializeField] string[] twoPairLines;
    [SerializeField] string[] onePairLines;
    [SerializeField] string[] fullHouseLines;
    [SerializeField] string[] straightLines;
    [SerializeField] string[] bustLines;

    // Update is called once per frame
    void Update()
    {
        
    }

    // rather than rolling actual dice, we use approximate probabilities
    void Roll()
    {
        System.Random randy = new();
        int roll = randy.Next(0, 10000);
        if (roll <= 617)
        {
            dialogue = bustLines;
        } else if (roll <= 5247)
        {
            dialogue = onePairLines;
        } else if (roll <= 7562)
        {
            dialogue = twoPairLines;
        } else if (roll <= 9105)
        {
            dialogue = threeKindLines;
        } else if (roll <= 9414)
        {
            dialogue = straightLines;
        } else if (roll <= 9800)
        {
            dialogue = fullHouseLines;
        } else if (roll <= 9993)
        {
            dialogue = fourKindLines;
        } else
        {
            dialogue = fiveKindLines;
        }
    }
}
