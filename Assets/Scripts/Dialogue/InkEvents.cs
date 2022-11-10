using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InkEvents : MonoBehaviour
{
    #region Inspector

    [Tooltip("")]
    [SerializeField] private List<InkEvent> inkEvents;

    #endregion

    #region Unity Event Functions

    private void OnEnable()
    {
        DialogueController.InkEvent += TryInvokeEvent;
    }

    private void OnDisable()
    {
        DialogueController.InkEvent -= TryInvokeEvent;

    }

    private void TryInvokeEvent(string eventName)
    {
        foreach (InkEvent inkEvent in inkEvents)
        {
            if (inkEvent.name == eventName)
            {
                inkEvent.onEvent.Invoke();
                return;
            }
        }
    }

    #endregion
}

[Serializable]
public struct InkEvent
{
    [Tooltip("Name of the ink Event")]
    public string name;

    [Tooltip("Invoked when the ink event is Invoked.")]
    public UnityEvent onEvent;
}
