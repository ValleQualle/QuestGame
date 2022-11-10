using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameController : MonoBehaviour
{

   private PlayerController player;
   private DialogueController dialogueController;



   #region Unity Event Functions

   private void Awake()
   {
      player = FindObjectOfType<PlayerController>();

      if (player == null)
      {
         Debug.Log("No player found in scene", this);
      }
      
      dialogueController = FindObjectOfType<DialogueController>();

      if (dialogueController == null)
      {
         Debug.Log("No player found in scene", this);
      }
   }
   
   private void OnEnable()
   {
      DialogueController.DialogueClosed += EndDialogue;
   }

   private void Start()
   {
      EnterPlayMode();
   }

   private void OnDisable()
   {
      DialogueController.DialogueClosed -= EndDialogue;
   }

   #endregion

   #region Modes

   private void EnterPlayMode()
   {
      // Esc changes the cursor visibility (ONLY in the unity editor!)
      Cursor.lockState = CursorLockMode.Locked;
      player.EnableInput();
   }

   private void EnterDialogueMode()
   {
      Cursor.lockState = CursorLockMode.None;
      player.DisableInput();
   }

   #endregion

   public void StartDialogue(string dialoguePath, UnityEvent onEndDialogue)
   {
      EnterDialogueMode();
      dialogueController.StartDialogue(dialoguePath, onEndDialogue);
   }

   private void EndDialogue()
   {
      EnterPlayMode();
   }
}
