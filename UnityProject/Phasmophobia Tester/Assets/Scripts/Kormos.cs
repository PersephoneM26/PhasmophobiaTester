using NUnit.Framework.Constraints;
using UnityEngine;

public class Kormos : Ghost
{
    [SerializeField] private float movingHuntThreashold, stillHuntThreashold;

    protected override void Update()
    {
        if (isMoving) huntSanityThreashold = movingHuntThreashold;
        else huntSanityThreashold = stillHuntThreashold;
        base.Update();
    }

    protected override void Move()
    {
        if (isMoving) hasLOS = true;
        base.Move();
    }

    public override void GainLOS()
    {
        // my eyes! im binndddddd!!
    }
    public override void LoseLOS()
    {
        // mothing to ... *see*  here
    }
}
