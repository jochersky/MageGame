using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class NPC : MonoBehaviour
{
    [SerializeField] SpriteRenderer outline;
    [SerializeField] Image dialogueBox;
    [SerializeField] TextMeshProUGUI text;
    // list of lines of dialogue to be spoken right now
    [SerializeField] string[] dialogue;
    // time in seconds between each character appearing
    [SerializeField] float textSpeed = 0;
    // current line of dialogue in the list of lines
    int _lineIdx = 0;
    private bool _inRange = false;
    private bool _isTalking = false;
    private PlayerInput _input;
    private Coroutine talking;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // this is only way to do this without disrupting the current event setup it seems
        _input = FindAnyObjectByType<PlayerInput>();
        _input.actions["Interact"].performed += OnInteract;
        text.text = string.Empty;
        dialogueBox.enabled = false;
    }

    void OnInteract(InputAction.CallbackContext context)
    {
        if (_inRange)
        {
            Talk();
        }
    }

    void Talk()
    {
        if (!_isTalking)
        {
            talking = StartCoroutine(AnimateText());
        }
        
    }

    IEnumerator AnimateText()
    {
        _isTalking = true;
        text.text = string.Empty;
        if (_lineIdx < dialogue.Length)
        {
            dialogueBox.enabled = true;
            foreach (char ch in dialogue[_lineIdx])
            {
                text.text += ch;
                yield return new WaitForSeconds(textSpeed);
            }
            _lineIdx += 1;
        } else
        {
            dialogueBox.enabled = false;
            _lineIdx = 0;
        }
        _isTalking = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        _inRange = true;
        outline.enabled = true;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        _inRange = false;
        outline.enabled = false;
        text.text = string.Empty;
        dialogueBox.enabled = false;
        _lineIdx = 0;
        if (talking != null)
        {
            StopCoroutine(talking);
            _isTalking = false;
        }
    }
}
