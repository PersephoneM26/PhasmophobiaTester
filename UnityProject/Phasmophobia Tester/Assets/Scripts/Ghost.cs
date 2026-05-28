using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    protected float currentSpeed, blinkOutTimeTemp, blinkInTimeTemp;
    protected bool isMale;

    public ghostGender GhostGender => ghostGender;

    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
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
        Invoke(nameof(Step), gameManager.GracePeriod);
    }

    public void StopHunt()
    {
        CancelInvoke();
        gameManager.GhostModel.GetComponent<Image>().enabled = false;
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
        gameManager.GhostModel.GetComponent<Image>().enabled = true;
        Invoke(nameof(BlinkOut), gameManager.GracePeriod);
    }


    public void BlinkOut()
    {
        gameManager.GhostModel.GetComponent<Image>().enabled = false;
        blinkOutTimeTemp = Random.Range(blinkInvisibleMinMax.x, blinkInvisibleMinMax.y);
        Invoke(nameof(BlinkIn), blinkOutTimeTemp);
    }
    public void BlinkIn()
    {
        gameManager.GhostModel.GetComponent<Image>().enabled = true;
        blinkInTimeTemp = Random.Range(blinkVisibleMinMax.x, blinkVisibleMinMax.y);
        blinkInTimeTemp = Mathf.Clamp((blinkInTimeTemp + blinkOutTimeTemp), perFlickerLengthMinMax.x, perFlickerLengthMinMax.y) - blinkOutTimeTemp;
        Invoke(nameof(BlinkOut), blinkInTimeTemp);
    }

    public void SetGender(bool male)
    {
        isMale = male;
    }
}
