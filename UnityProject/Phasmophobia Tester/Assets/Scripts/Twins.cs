using UnityEngine;

public class Twins : Ghost
{
    [SerializeField] private float slowTwinSpeed, fastTwinSpeed;

    public override void StartHunt()
    {
        if (Random.value > 0.5f)
        {
            walkSpeed = fastTwinSpeed;
            losSpeed = fastTwinSpeed;
        }
        else
        {
            walkSpeed = slowTwinSpeed;
            losSpeed = slowTwinSpeed;
        }
        base.StartHunt();
    }
}
