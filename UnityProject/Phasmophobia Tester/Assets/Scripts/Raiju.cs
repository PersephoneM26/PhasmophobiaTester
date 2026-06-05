using UnityEngine;

public class Raiju : Ghost
{
    [SerializeField] private float equipmentSpeed, equipmentThreashold, equipmentRange;

    private float startingSpeed, startingThreashold;

    protected override void Awake()
    {
        base.Awake();
        startingSpeed = walkSpeed;
        startingThreashold = huntSanityThreashold;
    }

    protected override void Update()
    {
        base.Update();
        if (gameManager.EquipmentOn && distanceFromPlayer <= equipmentRange)
        {
            walkSpeed = equipmentSpeed;
            losSpeed = equipmentSpeed;
            huntSanityThreashold = equipmentThreashold;
            currentSpeed = equipmentSpeed;
        }
        else
        {
            walkSpeed = startingSpeed;
            huntSanityThreashold = startingThreashold;
        }
    }
}
