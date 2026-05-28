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
    protected GameObject ghostModelPrefab, ghostModel;
    protected float currentSpeed, blinkOutTimeTemp, blinkInTimeTemp;

    public ghostGender GhostGender => ghostGender;

    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        canvas = gameManager.Canvas;
        ghostModelPrefab = gameManager.GhostModel;
        ghostModel = Instantiate(ghostModelPrefab, canvas.transform);
        ghostModel.SetActive(false);
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

    public void StopHunt()
    {
        CancelInvoke();
        ghostModel?.SetActive(false);
    }

    private void Step()
    {
        FMODUnity.RuntimeManager.PlayOneShot(gameManager.Footstep);
        //Debug.Log(SpeedToStepsPerSecond(currentSpeed));
        Invoke(nameof(Step), SpeedToStepsPerSecond(currentSpeed));
    }

    private float SpeedToStepsPerSecond(float speed)
    {
        return 60f / (60f / (Random.Range(-0.1f, -0.05f) + (1f / speed)));
    }

    public void StartBlinks()
    {
        ghostModel.SetActive(true);
        Invoke(nameof(BlinkOut), 2f);
    }


    public void BlinkOut()
    {
        ghostModel.SetActive(false);
        blinkOutTimeTemp = Random.Range(blinkInvisibleMinMax.x, blinkInvisibleMinMax.y);
        Invoke(nameof(BlinkIn), blinkOutTimeTemp);
    }
    public void BlinkIn()
    {
        ghostModel.SetActive(true);
        blinkInTimeTemp = Random.Range(blinkVisibleMinMax.x, blinkVisibleMinMax.y);
        blinkInTimeTemp = Mathf.Clamp((blinkInTimeTemp + blinkOutTimeTemp), perFlickerLengthMinMax.x, perFlickerLengthMinMax.y) - blinkOutTimeTemp;
        Invoke(nameof(BlinkOut), blinkInTimeTemp);
    }
}
