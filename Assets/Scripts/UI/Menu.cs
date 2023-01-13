using System;
using System.Collections;
using System.Collections.Generic;

using FMODUnity;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    #region Inspector

    [Tooltip("Hide the menu when the game starts.")]
    [SerializeField] private bool disableOnAwake;

    [Tooltip("Remember the Selectable that was selected when the menu was opened and reselect it once the menu is closed.")]
    [SerializeField] private bool selectPreviousOnClose;

    [Tooltip("Selectable to be selected when the menu is opened.")]
    [SerializeField] private Selectable selectOnOpen;

    #endregion

    private Selectable selectOnClose;
    
    #region Unity Event Functions

    private void Awake()
    {
        if (disableOnAwake)
        {
            gameObject.SetActive(false);
        }
        else
        {
            Open();
        }
    }

    #endregion

    public void Open()
    {
        // TODO DoTween animations
        
       gameObject.SetActive(true);

       // Save the previous selection.
       if (selectPreviousOnClose)
       {
           GameObject previousSelection = EventSystem.current.currentSelectedGameObject;
           if (previousSelection != null)
           {
               selectOnClose = previousSelection.GetComponent<Selectable>();
           }
       }
       
       // Coroutine only necessary for select animation
       // Select UI event is not called if it was enabled in the same frame
       StartCoroutine(DelayedSelect(selectOnOpen));
    }

    public void Close()
    {
        if (selectPreviousOnClose && selectOnClose != null)
        {
            selectOnClose.StartCoroutine(DelayedSelect(selectOnClose));
        }
        
        // TODO DoTween animations
        
        gameObject.SetActive(false);
    }

    public void Show()
    {
        // TODO DoTween animations
        gameObject.SetActive(true);
    }
    
    public void Hide()
    {
        // TODO DoTween animations
        gameObject.SetActive(false);
    }
    
    private IEnumerator DelayedSelect(Selectable newSelection)
    {
        // Wait a frame
        yield return null;
        Select(newSelection);
    }

    private void Select(Selectable newSelection)
    {
        if (newSelection == null) { return; }
        newSelection.Select();
    }
}
