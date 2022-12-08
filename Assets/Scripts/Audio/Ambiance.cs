using System;
using System.Collections;
using System.Collections.Generic;

using FMODUnity;

using UnityEngine;

[RequireComponent(typeof(StudioEventEmitter))]
public class Ambiance : MonoBehaviour
{
    #region Inspector

    [SerializeField] private string insideParameterName = "inside";

    #endregion

    private StudioEventEmitter ambienceEmitter;

    #region Unity Event Functions

    private void Awake()
    {
        ambienceEmitter = GetComponent<StudioEventEmitter>();
    }

    #endregion

    public void SetInside(bool inside)
    {
        ambienceEmitter.SetParameter(insideParameterName, inside ? 1 : 0);
    }
}
