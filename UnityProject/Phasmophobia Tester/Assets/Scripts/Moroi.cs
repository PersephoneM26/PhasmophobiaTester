using System.Runtime.CompilerServices;
using UnityEngine;

public class Moroi : Ghost
{
    [SerializeField] private float speedScaling, scaleStep;
    private float baseWalkSpeed;

    protected override void Awake()
    {
        base.Awake();
        baseWalkSpeed = walkSpeed;
    }

    protected override void Update()
    {
        if (huntSanityThreashold - currentSanity > scaleStep)
        {
            walkSpeed = baseWalkSpeed + (huntSanityThreashold - scaleStep - currentSanity) / scaleStep * speedScaling;
            losSpeed = walkSpeed;
        }
        base.Update();
    }
}
