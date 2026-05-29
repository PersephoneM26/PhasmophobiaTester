using UnityEngine;

public class Dayan : Ghost
{
    [SerializeField] private float walkingHuntSanity, stillHuntSanity, awayHuntSanity, walkingSpeed, stillSpeed, awaySpeed;
    protected override void Update()
    {
        if (isMoving & distanceFromPlayer <= 10f)
        {
            currentSpeed = walkingSpeed;
            huntSanityThreashold = walkingHuntSanity;
        }
        else if (distanceFromPlayer <= 10f & !isMoving)
        {
            currentSpeed = stillSpeed;
            huntSanityThreashold = stillHuntSanity;
        }
        else if (distanceFromPlayer > 10f)
        {
            currentSpeed = awaySpeed;
            huntSanityThreashold = awayHuntSanity;
        }
            base.Update();
    }
}
