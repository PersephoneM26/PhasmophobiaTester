using System.Collections.Generic;
using UnityEngine;

public enum ghostGender
{
    Male,
    female,
    Androgynous
}


public class Ghost : MonoBehaviour
{
    private GameManager gameManager;

    [SerializeField] protected float walkSpeed, losSpeed, losSpeedGainMultiplier, losSpeedGainTime, huntSanityThreashold, huntCooldown, smudgeHuntCooldown;
    [SerializeField] protected Vector2 blinkVisibleMinMax, blinkInvisibleMinMax, perFlickerLengthMinMax;
    [SerializeField] protected ghostGender ghostGender;

    protected Canvas canvas;
    protected GameObject ghostModel;
    protected float currentSpeed, currentStepsPerSecond;

    public ghostGender GhostGender => ghostGender;

    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        canvas = gameManager.Canvas;
        ghostModel = gameManager.GhostModel;
        Instantiate(ghostModel, canvas.transform);
        currentSpeed = walkSpeed;
        currentStepsPerSecond = SpeedToStepsPerSecond(currentSpeed);
    }


    protected virtual void GainLOS(bool hasEquipment)
    {

    }

    protected virtual void LoseLOS()
    {

    }

    public void PlayFootsteps()
    {
        InvokeRepeating(nameof(Step), 0, currentStepsPerSecond);
    }

    public void StopFootsteps()
    {
        CancelInvoke();
    }

    private void Step()
    {
        FMODUnity.RuntimeManager.PlayOneShot(gameManager.Footstep);
    }

    private float SpeedToStepsPerSecond(float speed)
    {
        return 60f / (5.3847f * (Mathf.Pow(speed, 2f)) + 46.3881f * speed + 22.9986f);
    }
}
