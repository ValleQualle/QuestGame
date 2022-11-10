using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameState : MonoBehaviour
{
    #region Inspector

    [SerializeField] private List<State> states;

    #endregion

    public State Get(string id)
    {
        foreach (State state in states)
        {
            if (state.id == id)
            {
                return state;
            }
        }

        return null;
    }

    public void Add(string id, int amount)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            Debug.LogError("ID of state is empty.", this);
            return;
        }
        
        State state = Get(id);

        if (state == null)
        {
            State newState = new State(id, amount);
            states.Add(newState);
        }
        else
        {
            state.amount += amount;
        }
    }

    public void Add(State state)
    {
        Add(state.id, state.amount);
    }

    public void Add(List<State> states)
    {
        foreach (State state in states)
        {
            Add(state);
        }
    }
}

