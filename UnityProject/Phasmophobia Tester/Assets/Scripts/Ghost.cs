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

    [SerializeField] protected float walkSpeed, losSpeed, losSpeedGainMultiplier, losSpeedGainTime, huntSanityThreashold, huntCooldown, smudgeHuntCooldown, distanceUpdateFrequence;
    [SerializeField] protected Vector2 blinkVisibleMinMax, blinkInvisibleMinMax, perFlickerLengthMinMax, huntStartingDistanceMinMax;
    [SerializeField] protected ghostGender ghostGender;
    [SerializeField] protected TextMeshProUGUI distanceText, sanityText, timeBetweenHuntsText;

    protected float currentSpeed, blinkOutTimeTemp, blinkInTimeTemp, distanceFromPlayer, currentLOSMult = 1f, currentSanity, timeSinceLastHunt;
    protected bool isMale, hasLOS, isMovingForward, isMovingBackward, hasBeenSmudged;
    protected bool isMoving => (isMovingForward || isMovingBackward);

    public ghostGender GhostGender => ghostGender;

    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        currentSpeed = walkSpeed;
        distanceText = gameManager.GhostModel.transform.Find("Distance").GetComponent<TextMeshProUGUI>();
        sanityText = gameManager.GhostModel.transform.Find("Sanity").GetComponent<TextMeshProUGUI>();
        timeBetweenHuntsText = gameManager.GhostModel.transform.Find("HuntTime").GetComponent<TextMeshProUGUI>();
        distanceText.enabled = false;
        currentSanity = gameManager.StartingSanity;
    }

    protected void Update()
    {
        if (hasLOS)
        {
            currentLOSMult = Mathf.Clamp(currentLOSMult - (((1f - losSpeedGainMultiplier) / losSpeedGainTime) * Time.deltaTime), 0, losSpeedGainMultiplier);
            currentSpeed = losSpeed * currentLOSMult;
        }
        if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            isMovingForward = true;
            isMovingBackward = false;
        }
        else if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
        {
            isMovingBackward = true;
            isMovingForward = false;
        }
        else
        {
            isMovingBackward = false;
            isMovingForward = false;
        }
    }

    public virtual void GainLOS()
    {
        hasLOS = true;
    }

    public virtual void LoseLOS()
    {
        hasLOS = false;
        currentSpeed /= currentLOSMult;
    }

    public void PlayFootsteps()
    {
        Invoke(nameof(Step), gameManager.GracePeriod);
    }

    public void StopHunt()
    {
        CancelInvoke();
        gameManager.GhostModel.GetComponent<Image>().enabled = false;
        distanceText.enabled = false;
        sanityText.gameObject.SetActive(false);
        timeBetweenHuntsText.gameObject.SetActive(false);
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

    public void StartMoving()
    {
        distanceFromPlayer = Random.Range(huntStartingDistanceMinMax.x, huntStartingDistanceMinMax.y);
        distanceText.enabled = true;
        Invoke(nameof(Move), gameManager.GracePeriod);
    }

    private void Move()
    {
        if (hasLOS) distanceFromPlayer -= currentSpeed * distanceUpdateFrequence;
        else distanceFromPlayer += (Random.Range(-currentSpeed, currentSpeed) * distanceUpdateFrequence);
        if (isMovingForward) distanceFromPlayer -= (gameManager.PlayerSpeed * distanceUpdateFrequence);
        else if (isMovingBackward) distanceFromPlayer += (gameManager.PlayerSpeed * distanceUpdateFrequence);
        distanceText.text = "Distance: " + (Mathf.Round(distanceFromPlayer * 100f) / 100f).ToString() + " m";

        if (distanceFromPlayer <= 0.1f) StopHunt();

        Invoke(nameof(Move), distanceUpdateFrequence);
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

    public void GetHuntSanity()
    {
        currentSanity = Mathf.Lerp(0f, Mathf.Min((currentSanity * 0.95f), huntSanityThreashold), Mathf.Pow(Random.value, (1f/2.75f)));
        sanityText.text = "Sanity: " + Mathf.RoundToInt(currentSanity).ToString();
        sanityText.gameObject.SetActive(true);
    }

    public void GetTimeSinceLastHunt()
    {
        if (currentSanity == gameManager.StartingSanity) return;
        timeSinceLastHunt = Mathf.Lerp(1f, (currentSanity + 20f) / 3f, Mathf.Pow(Random.value, 1.5f)) + huntCooldown;
        if (hasBeenSmudged)
        {
            timeSinceLastHunt += smudgeHuntCooldown - huntCooldown;
            hasBeenSmudged = false;
        }
        timeBetweenHuntsText.text = "Time since last hunt: " + (Mathf.RoundToInt(timeSinceLastHunt) / 60).ToString() + ":" + Mathf.RoundToInt(timeSinceLastHunt % 60f);
        timeBetweenHuntsText.gameObject.SetActive(true);
    }

    public void Smudge()
    {
        hasBeenSmudged = true;
    }

    public void SetGender(bool male)
    {
        isMale = male;
    }
}
