using System;
using System.Collections;
using System.Collections.Generic;

using DG.Tweening;

using UnityEngine;

public class ButtonAnimation : MonoBehaviour
{
    #region Inspector

    [Tooltip("Distance to locally move the button during the press animation in uu.")]
    [SerializeField] private float yMovement = -0.049f;

    [Tooltip("Color to change the button to while pressing down.")]
    [SerializeField] private Color pressColor = Color.yellow;

    [Tooltip("Time in sec to hold down the button before releasing.")]
    [Min(0)]
    [SerializeField] private float downDuration = 0.3f;

    [Header("In")]
    
    [Tooltip("Ease of the press animation")]
    [SerializeField] private Ease easeIn = Ease.InSine;

    [Tooltip("Duration in sec ot the press animation.")]

    [SerializeField] private float durationIn = 0.3f;
    
    [Header("Out")]
    
    [Tooltip("Ease of the release animation")]
    [SerializeField] private Ease easeOut = Ease.OutElastic;

    [Tooltip("Duration in sec ot the release animation.")]
    [SerializeField] private float durationOut = 0.5f;

    #endregion

    private MeshRenderer meshRenderer;
    private Color originalColor;
    private Sequence sequence;

    #region Unity Event Functions

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        originalColor = meshRenderer.material.color; //Die color muss man bei shadern genau definieren! Das klappt nicht immer so einfach.
    }

    #endregion

    public void PlayAnimation()
    {
        sequence.Complete(true);
        
        sequence = DOTween.Sequence();
                //Press down
        sequence.Append(transform.DOLocalMoveY(yMovement, durationIn).SetRelative().SetEase(easeIn))
                .Join(meshRenderer.material.DOColor(pressColor, durationIn).SetEase(Ease.Linear))
                // Wait
                .AppendInterval(downDuration)
                //Release
                .Append(transform.DOLocalMoveY(-yMovement, durationOut).SetRelative().SetEase(easeOut))
                .Join(meshRenderer.material.DOColor(originalColor, durationOut).SetEase(Ease.Linear));

        //Not needed because autoplay is on by default.
        sequence.Play();
    }
}
