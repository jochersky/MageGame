using UnityEngine;

public class Mushelle : NPC
{
    [SerializeField] string[] successLines;
    [SerializeField] GameObject reward;
    private int questProgress = 0;
    private readonly int questGoal = 3;
    private bool questCompleted = false;

    public override void Start()
    {
        base.Start();
        foreach (GameObject enemy in specialEnemies)
        {
            enemy.GetComponent<Health>().OnDeath += UpdateQuestProgress;
        }
    }

    void UpdateQuestProgress()
    {
        
        questProgress += 1;
        if (questProgress == questGoal)
        {
            Success();
        }
    }

    void Success()
    {
        dialogue = successLines;
        questCompleted = true;
    }

    void GrantReward()
    {
        Instantiate(reward, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    public override void CloseDialogue()
    {
        base.CloseDialogue();
        if (questCompleted)
        {
            GrantReward();
        }
    }
}
