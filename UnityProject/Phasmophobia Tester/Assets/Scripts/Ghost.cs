using FMODUnity;
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
    protected GameManager gameManager;

    [SerializeField] protected float walkSpeed, losSpeed, losSpeedGainMultiplier, losSpeedGainTime, losSpeedDecayTime, huntSanityThreashold, huntCooldown, smudgeHuntCooldown, footstepAudioRange;
    [SerializeField] protected Vector2 blinkVisibleMinMax, blinkInvisibleMinMax, perFlickerLengthMinMax, huntStartingDistanceMinMax;
    [SerializeField] protected ghostGender ghostGender;
    
    protected TextMeshProUGUI distanceText, sanityText, timeBetweenHuntsText;
    protected float currentSpeed, blinkOutTimeTemp, blinkInTimeTemp, distanceFromPlayer, currentLOSMult = 1f, currentSanity, timeSinceLastHunt, footstepVolume;
    protected bool isMale, hasLOS, isMovingForward, isMovingBackward, hasBeenSmudged;
    protected bool isMoving => (isMovingForward || isMovingBackward);

    public ghostGender GhostGender => ghostGender;

    protected virtual void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        currentSpeed = walkSpeed;
        distanceText = gameManager.GhostModel.transform.Find("Distance").GetComponent<TextMeshProUGUI>();
        sanityText = gameManager.GhostModel.transform.Find("Sanity").GetComponent<TextMeshProUGUI>();
        timeBetweenHuntsText = gameManager.GhostModel.transform.Find("HuntTime").GetComponent<TextMeshProUGUI>();
        distanceText.enabled = false;
        currentSanity = gameManager.StartingSanity;
    }

    protected virtual void Update()
    {
        if (hasLOS)
        {
            currentLOSMult = Mathf.Clamp(currentLOSMult - (((1f - losSpeedGainMultiplier) / losSpeedGainTime) * Time.deltaTime), 1, losSpeedGainMultiplier);
            currentSpeed = losSpeed * currentLOSMult;
        }
        else if (currentLOSMult > 1f)
        {
            currentLOSMult = Mathf.Clamp(currentLOSMult + (((1f - losSpeedGainMultiplier) / losSpeedDecayTime) * Time.deltaTime), 1, losSpeedGainMultiplier);
            currentSpeed = walkSpeed * currentLOSMult;
        }
        else currentSpeed = walkSpeed;
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
        hasLOS = false;
        currentLOSMult = 1f;
    }

    private void Step()
    {
        PlayOneShotWithVolume(gameManager.Footstep, Mathf.Clamp01(1 - (distanceFromPlayer / footstepAudioRange)));
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

    protected virtual void Move()
    {
        if (hasLOS) distanceFromPlayer -= currentSpeed * gameManager.DistanceUpdateFrequence;
        else distanceFromPlayer += (Random.Range(-currentSpeed, currentSpeed) * gameManager.DistanceUpdateFrequence);
        if (isMovingForward) distanceFromPlayer -= (gameManager.PlayerSpeed * gameManager.DistanceUpdateFrequence);
        else if (isMovingBackward) distanceFromPlayer += (gameManager.PlayerSpeed * gameManager.DistanceUpdateFrequence);
        distanceText.text = "Distance: " + (Mathf.Round(distanceFromPlayer * 100f) / 100f).ToString() + " m";

        if (distanceFromPlayer <= 0.1f) StopHunt();

        Invoke(nameof(Move), gameManager.DistanceUpdateFrequence);
    }

    public void StartBlinks()
    {
        gameManager.GhostModel.GetComponent<Image>().enabled = true;
        Invoke(nameof(BlinkOut), gameManager.GracePeriod);
    }


    protected virtual void BlinkOut()
    {
        gameManager.GhostModel.GetComponent<Image>().enabled = false;
        blinkOutTimeTemp = Random.Range(blinkInvisibleMinMax.x, blinkInvisibleMinMax.y);
        Invoke(nameof(BlinkIn), blinkOutTimeTemp);
    }
    protected virtual void BlinkIn()
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

    protected void PlayOneShotWithVolume(EventReference eventReference, float volume)
    {
        var instance = FMODUnity.RuntimeManager.CreateInstance(eventReference);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        instance.setVolume(volume);
        instance.start();
        instance.release();
    }
}
