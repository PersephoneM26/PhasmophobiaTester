using UnityEngine;

public enum GalluState
{
    Calm,
    Aggressive,
    Normal
}

public class Gallu : Ghost
{
    [SerializeField] private float calmSpeed, calmThreashold, aggressiveSpeed, aggressiveThreashold, normalSpeed, normalThreshold;

    private GalluState state = GalluState.Normal;

    protected override void StepInSalt(int position)
    {
        if (state == GalluState.Aggressive) return;
        base.StepInSalt(position);
        NextState();
    }

    public override void Smudge()
    {
        base.Smudge();
        NextState();
    }

    private void NextState()
    {
        switch (state)
        {
            case GalluState.Normal:
                state = GalluState.Aggressive;
                walkSpeed = aggressiveSpeed;
                losSpeed = aggressiveSpeed;
                huntSanityThreashold = aggressiveThreashold;
                break;
            case GalluState.Aggressive:
                state = GalluState.Calm;
                walkSpeed = calmSpeed;
                losSpeed = calmSpeed;
                huntSanityThreashold = calmThreashold;
                break;
            case GalluState.Calm:
                state = GalluState.Normal;
                walkSpeed = normalSpeed;
                losSpeed = normalSpeed;
                huntSanityThreashold = normalThreshold;
                break;
        }
    }
}
