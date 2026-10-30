using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour
{
    [SerializeField] GameObject dialogueOptionPrefab;
    [SerializeField] GameObject dialogueOptions;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //ClearDialogueOptions();
    }

    public void ClearDialogueOptions()
    {
        //Transform[] buttons = dialogueOptions.transform.GetComponentsInChildren<Transform>();
        foreach (Transform button in dialogueOptions.transform)
        {
            Destroy(button.gameObject);
        }
        // for (int bidx = 0; bidx < buttons.Length; bidx++)
        // {
        //     print("destroying button");
        //     Destroy(buttons[bidx].gameObject);
        // }
    }

    public void AddDialogueOption(string text, UnityEngine.Events.UnityAction onClickListener)
    {
        GameObject button = Instantiate(dialogueOptionPrefab, dialogueOptions.transform);
        button.GetComponentInChildren<TextMeshProUGUI>().text = text;
        button.GetComponent<Button>().onClick.AddListener(onClickListener);
    }
}
