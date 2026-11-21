using System.Collections;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EndDoor : MonoBehaviour
{
    [SerializeField] private int embersRequiredCount = 5000;
    [SerializeField] private TextMeshProUGUI emberCount;
    [SerializeField] private TextMeshProUGUI embersDepositedCount;
    [SerializeField] private EndDoorNPC endDoorNPC;
    [SerializeField] private GameObject offeringButton;
    [SerializeField] private Image doorImage;
    [SerializeField] private Sprite openedDoor;
    [SerializeField] private ParticleSystem particles;
    private EventSystem _eventSystem;
    
    private int _embersAmt;
    private int _embersDeposited;
    private bool _extraOfferingComplete;
    
    private void Start()
    {
        _eventSystem = EventSystem.current;
        
        offeringButton.SetActive(false);

        endDoorNPC.OnStoppedTalking += ShowOfferingButton;
        
        _embersAmt = GameManager.Instance.EmberStats[0];
        _embersDeposited = GameManager.Instance.EmberStats[1];

        emberCount.text = $"*~ {_embersAmt}";
        embersDepositedCount.text = $"*~ {_embersDeposited} / {embersRequiredCount}";
        
        endDoorNPC.InitiateDialogue();
    }

    private void ShowOfferingButton()
    {
        offeringButton.SetActive(true);
        StartCoroutine(EnableButtonAfterDelay());
    }
    
    public void UpdateEmberTotal()
    {
        particles.Play();
        
        _embersDeposited += _embersAmt;
        emberCount.text = $"*~ 0";
        embersDepositedCount.text = $"*~ {_embersDeposited} / {embersRequiredCount}";
        
        SaveSystem.Save();

        // Unlock true ending
        if (_embersDeposited >= embersRequiredCount)
            StartCoroutine(OpenDoorAfterDelay());
        
        offeringButton.SetActive(false);
    }

    private IEnumerator EnableButtonAfterDelay()
    {
        yield return new WaitForSeconds(0.2f);
        offeringButton.GetComponent<Button>().interactable = true;
        _eventSystem.SetSelectedGameObject(offeringButton);
    }

    public void Save(ref GameSaveData data)
    {
        data.embersDeposited = _embersDeposited;
    }

    private IEnumerator OpenDoorAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        doorImage.sprite = openedDoor;
    }
}
