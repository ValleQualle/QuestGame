using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
   #region Unity Event Functions

   private void Start()
   {
      EnterPlayMode();
   }

   #endregion

   #region Modes

   private void EnterPlayMode()
   {
      // Esc changes the cursor visibility (ONLY in the unity editor!)
      Cursor.lockState = CursorLockMode.Locked;
   }

   #endregion
}
