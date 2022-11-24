using System.Collections;
using System.Collections.Generic;

using FMODUnity;

using UnityEngine;
using UnityEngine.Events;

public class PlayerSounds : MonoBehaviour
{
    #region Inspector

    [SerializeField] private StudioEventEmitter stepSound;
    
    [SerializeField] private StudioEventEmitter landSound;
    
    [SerializeField] private string stepSoundParameterName = "surface";

    [SerializeField] private PhysicMaterial defaultStepSoundPhysicMaterial;

    [Header("Raycast")]

    [SerializeField] private LayerMask layerMask;
    
    [Header("Unity Events")]

    [SerializeField] private UnityEvent onStep;
    
    [SerializeField] private UnityEvent onLand;

    #endregion

    #region Animation Events

    // ReSharper disable once UnusedMember.Global - Called by Animation Events
    public void PlaySound(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight < 0.5f) { return; }
        
        switch (animationEvent.stringParameter)
        {
            case "Step":
                stepSound.Play();
                ChangeStepSound(stepSound); // Call AFTER calling Function to play!
                onStep.Invoke();
                break;
            case "Land":
                landSound.Play();
                ChangeStepSound(landSound);
                onLand.Invoke();
                return;
            default:
                Debug.LogWarning($"Unknown sound parameter '{animationEvent.stringParameter}'.", this);
                break;
        }
    }

    #endregion

    private void ChangeStepSound(StudioEventEmitter emitter)
    {
        if (!Physics.Raycast(transform.position + Vector3.up * 0.01f,
                            Vector3.down,
                            out RaycastHit hit,
                            5f,
                            layerMask,
                            QueryTriggerInteraction.Ignore))
        {
            return;
        }

        PhysicMaterial groundPhysicMaterial = hit.collider.sharedMaterial;
        int stepSoundParameterValue = GetStepSoundParameterValue(groundPhysicMaterial);

        emitter.SetParameter(stepSoundParameterName, stepSoundParameterValue);
    }

    private int GetStepSoundParameterValue(PhysicMaterial physicMaterial)
    {
        if (physicMaterial == null)
        {
            physicMaterial = defaultStepSoundPhysicMaterial;
        }
        
        switch (physicMaterial.name)
        {
            case "Grass": // PhysicMaterial name, not parameter label
                return 0; // Parameter value in FMOD!
            case "Wood":
                return 1;
            // todo Edit & Extend with more PhysicMaterials and parameters
            default:
                Debug.LogWarning($"PhysicMaterial name '{physicMaterial.name})' is missing in the switch statement." +
                                 "Extend your switch or check your spelling.", this);
                return 0;
        }
    }
}
