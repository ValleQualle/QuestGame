using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InkDialogue : MonoBehaviour
{
    #region Inspector

    
    [SerializeField] private string dialoguePath;
    
    [SerializeField] private UnityEvent onEndDialogue;
    
    #endregion

    public void StartDialogue()
    {
        if (string.IsNullOrWhiteSpace(dialoguePath))
        {
            Debug.LogWarning("No dialogue path defined.", this);
            return;
        }
        
        StartDialogue(dialoguePath);
    }

    public void StartDialogue(string dialoguePath)
    {
        FindObjectOfType<GameController>().StartDialogue(dialoguePath, onEndDialogue);
    }
}
