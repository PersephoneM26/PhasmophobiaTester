using UnityEngine;

public class Jinn : Ghost
{
    [SerializeField] private float highLOSSpeed, lowLOSSpeed;

    protected override void Move()
    {
        if (hasLOS && distanceFromPlayer >= 3f) losSpeed = highLOSSpeed;
        else losSpeed = lowLOSSpeed;

        base.Move();
    }

    public override void GainLOS()
    {
        base.GainLOS();
        if (distanceFromPlayer >= 3f) losSpeed = highLOSSpeed;
    }
}
