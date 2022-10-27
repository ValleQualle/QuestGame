using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;

public class Interaction : MonoBehaviour
{
    #region Inspector

    [SerializeField] private UnityEvent onInteracted;

    [Tooltip("Next interaction to be activated once this interaction was executed. It will be executed the next time the player interacts.")]
    [SerializeField] private Interaction nextInteraction;

    #endregion

    #region Unity Event Functions

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        List<Interaction> interactions = transform.parent.GetComponentsInChildren<Interaction>().ToList();

        foreach (Interaction interaction in interactions)
        {
            // Skip self
            if (interaction == this) { continue; }
            
            interaction.gameObject.SetActive(false);
        }
    }

    #endregion

    public void Execute()
    {
        if (nextInteraction != null)
        {
            nextInteraction.gameObject.SetActive(true);
        }
        
        onInteracted.Invoke();
    }
}
