using System;
using UnityEngine;

public class EndDoorNPC : NPC
{
    [SerializeField] private InputManager input;
    
    private new void Start()
    {
        base.Start();

        input.OnPlayerPressedInteract += OnInteract;
        OnStoppedTalking += () => { gameObject.SetActive(false); };
    }

    private void OnDisable()
    {
        input.OnPlayerPressedInteract -= OnInteract;
    }

    public void InitiateDialogue()
    {
        Talk();
    }

    private void OnInteract()
    {
        Talk();
    }
}
