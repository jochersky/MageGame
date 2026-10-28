using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class NPC : MonoBehaviour
{
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] SpriteRenderer outline;
    [SerializeField] protected GameObject dialogueBox;
    [SerializeField] string npcName;
    [SerializeField] TextMeshProUGUI nameTextBox;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] GameObject dialogueOptions;
    // the set of dialogue options to be shown next
    [SerializeField] protected Dictionary<string, UnityEngine.Events.UnityAction> currentDialogueOptions = new();
    // list of lines of dialogue to be spoken right now
    public string[] dialogue;
    // time in seconds between each character appearing
    [SerializeField] float textSpeed = 0;
    // information for generating any associated room
    public Sprite room;
    public GameObject specialEnemy;
    public TileBase specialTile;
    public TileBase specialObject;
    public List<GameObject> specialEnemies = new();
    public Vector2 spawnPoint;
    protected MapGenerator mapGenerator;
    // current line of dialogue in the list of lines
    int _lineIdx = 0;
    private bool _inRange = false;
    private bool _isTalking = false;
    private Player player;
    private PlayerInput _input;
    private Coroutine talking;
    private const float FORWARD = 0.0f;
    private const float BACKWARD = 180.0f;

    void Awake()
    {
        MapGenerator _mapGenerator = FindAnyObjectByType<MapGenerator>();
        mapGenerator = _mapGenerator;
        if (mapGenerator)
        {
            mapGenerator.OnPlayerPlaced += PlayerSetup;
        } else
        {
            Player player = FindAnyObjectByType<Player>();
            if (player)
            {
                PlayerSetup(player);
            } else
            {
                Debug.Log("No player found. Dialogue will not function.");
            }
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        nameTextBox.text = npcName;
        text.text = string.Empty;
        dialogueBox.SetActive(false);
    }

    protected virtual void PlayerSetup(Player _player)
    {
        player = _player;
        _input = _player.GetComponent<PlayerInput>();
        _input.actions["Interact"].performed += OnInteract;
    }

    void OnInteract(InputAction.CallbackContext context)
    {
        if (_inRange)
        {
            if (player.gameObject.transform.position.x > transform.position.x)
            {
                sprite.transform.SetPositionAndRotation(transform.position, Quaternion.Euler(new Vector3(0, BACKWARD, 0)));
            } else
            {
                sprite.transform.SetPositionAndRotation(transform.position, Quaternion.Euler(new Vector3(0, FORWARD, 0)));
            }
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

    protected void PlayNewLines(string[] _newDialogue)
    {
        dialogue = _newDialogue;
        _lineIdx = 0;
        Talk();
    }


    IEnumerator AnimateText()
    {
        _isTalking = true;
        text.text = string.Empty;
        if (_lineIdx < dialogue.Length)
        {
            dialogueBox.SetActive(true);
            foreach (char ch in dialogue[_lineIdx])
            {
                text.text += ch;
                yield return new WaitForSeconds(textSpeed);
            }
            _lineIdx += 1;
            if (_lineIdx == dialogue.Length)
            {
                DialogueBox dBox = dialogueBox.GetComponent<DialogueBox>();
                foreach (string key in currentDialogueOptions.Keys)
                {
                    dBox.AddDialogueOption(key, currentDialogueOptions[key]);
                }
                currentDialogueOptions.Clear();
            }
        } else
        {
            CloseDialogue();
        }
        _isTalking = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _inRange = true;
            outline.enabled = true;
        }
        
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _inRange = false;
            outline.enabled = false;
            text.text = string.Empty;
            CloseDialogue();
            if (talking != null)
            {
                StopCoroutine(talking);
                _isTalking = false;
                
            }
        }
        
    }

    public virtual void CloseDialogue()
    {
        ResetToDefaultDialogueOptions();
        dialogueBox.SetActive(false);
        
        _lineIdx = 0;
    }

    // Define the dialogue options for when the player initiates a conversation
    protected virtual void ResetToDefaultDialogueOptions()
    {
        dialogueBox.GetComponent<DialogueBox>().ClearDialogueOptions();
    }
}
