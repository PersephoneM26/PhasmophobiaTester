using UnityEngine;

public class Hantu : Ghost
{
    protected override void Update()
    {
        switch (currentTemp)
        {
            case > 15f:
                walkSpeed = 1.4f;
            break;

            case > 12f and <= 15f:
                walkSpeed = 1.75f; 
            break;

            case > 9f and <= 12f:
                walkSpeed = 2.1f;
            break;

            case > 6f and <= 9f:
                walkSpeed = 2.3f;
            break;

            case > 3f and <= 6f:
                walkSpeed = 2.4f;
            break;

            case > 0f and <= 3f:
                walkSpeed = 2.5f;
            break;
        }
        losSpeed = walkSpeed;
        base.Update();
    }
}
