using UnityEngine;

public class Obambo : Ghost
{
    [SerializeField] private float calmSpeed, agressiveSpeed, calmHuntSanity, agressiveHuntSanity, startingCalmTime, switchTime;

    private float contractRepeatingTime, addedSwitchHuntTime;
    private bool isCalm;


    override protected void Awake()
    {
        base.Awake();
        isCalm = true;
    }

    protected override void Update()
    {
        base.Update();
        EvaluateState();
    }

    protected override void GetHuntSanity()
    {
        float currentThreashold = huntSanityThreashold;
        huntSanityThreashold = agressiveHuntSanity;
        base.GetHuntSanity();
        huntSanityThreashold = currentThreashold;
    }

    protected override void AddHuntTimeModifiers()
    {
        base.AddHuntTimeModifiers();

        EvaluateState();

        if (currentSanity > huntSanityThreashold)
        {
            addedSwitchHuntTime = 0f;
            EnterAgressiveState();
            addedSwitchHuntTime = switchTime - Mathf.Repeat(Mathf.Max(0f, totalContractTime - startingCalmTime), switchTime);
            if (totalContractTime < startingCalmTime) addedSwitchHuntTime += startingCalmTime - switchTime;
            timeSinceLastHunt += addedSwitchHuntTime;
        }
    }

    private void EnterCalmState()
    {
        walkSpeed = calmSpeed;
        losSpeed = calmSpeed;
        huntSanityThreashold = calmHuntSanity;
        isCalm = true;
        Debug.Log("Calm");
    }

    private void EnterAgressiveState()
    {
        walkSpeed = agressiveSpeed;
        losSpeed = agressiveSpeed;
        huntSanityThreashold = agressiveHuntSanity;
        isCalm = false;
        Debug.Log("Agressive");
    }

    private void EvaluateState()
    {
        contractRepeatingTime = Mathf.Repeat((Mathf.Max(0, totalContractTime - startingCalmTime)), (2 * switchTime));
        if (contractRepeatingTime > 0 && contractRepeatingTime < switchTime && isCalm) EnterAgressiveState();
        if (contractRepeatingTime > switchTime && !isCalm) EnterCalmState();
    }
}
