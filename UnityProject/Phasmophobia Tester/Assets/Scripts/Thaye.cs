using UnityEngine;

public class Thaye : Ghost
{
    [SerializeField] private float ageTime, speedLoss, huntThreasholdLoss;

    private int totalTimesAged;

    protected override void SetHuntTimeTextAndVariables()
    {
        base.SetHuntTimeTextAndVariables();
        if (totalContractTime - (ageTime * totalTimesAged) > ageTime)
        {
            totalTimesAged++;
            walkSpeed -= speedLoss;
            losSpeed -= speedLoss;
            huntSanityThreashold -= huntThreasholdLoss;
        }
    }
}
