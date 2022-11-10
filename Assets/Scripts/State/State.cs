using System;

using UnityEngine;

[Serializable]
public class State
{
    [Tooltip("The ID of the state used to identify the state.")]
    public string id;

    [Tooltip("The value of the state.")]
    public int amount;

    public State(string id, int amount)
    {
        this.id = id;
        this.amount = amount;
    }
}
