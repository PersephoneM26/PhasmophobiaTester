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
    protected float currentSpeed;

    public ghostGender GhostGender => ghostGender;

    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        canvas = gameManager.Canvas;
        ghostModel = gameManager.GhostModel;
        Instantiate(ghostModel, canvas.transform);
        currentSpeed = walkSpeed;
    }


    protected virtual void GainLOS(bool hasEquipment)
    {

    }

    protected virtual void LoseLOS()
    {

    }

    public void PlayFootsteps()
    {
        Invoke(nameof(Step), 0);
    }

    public void StopFootsteps()
    {
        CancelInvoke();
    }

    private void Step()
    {
        FMODUnity.RuntimeManager.PlayOneShot(gameManager.Footstep);
        Debug.Log(SpeedToStepsPerSecond(currentSpeed));
        Invoke(nameof(Step), SpeedToStepsPerSecond(currentSpeed));
    }

    private float SpeedToStepsPerSecond(float speed)
    {
        return 60f / (60f / (Random.Range(-0.1f, -0.05f) + (1f / speed)));
    }
}
