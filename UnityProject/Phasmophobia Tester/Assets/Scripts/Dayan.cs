using UnityEngine;

public class Dayan : Ghost
{
    [SerializeField] private float walkingHuntSanity, stillHuntSanity, awayHuntSanity, walkingSpeed, stillSpeed, awaySpeed;
    protected override void Update()
    {
        if (isMoving & distanceFromPlayer <= 10f)
        {
            walkSpeed = walkingSpeed;
            huntSanityThreashold = walkingHuntSanity;
        }
        else if (distanceFromPlayer <= 10f & !isMoving)
        {
            walkSpeed = stillSpeed;
            huntSanityThreashold = stillHuntSanity;
        }
        else if (distanceFromPlayer > 10f)
        {
            walkSpeed = awaySpeed;
            huntSanityThreashold = awayHuntSanity;
        }
        losSpeed = walkingSpeed;
        base.Update();
    }
}
