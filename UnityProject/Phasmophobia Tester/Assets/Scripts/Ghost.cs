using FMODUnity;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Rendering;
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

    [SerializeField] protected float walkSpeed, losSpeed, losSpeedGainMultiplier, losSpeedGainTime, losSpeedDecayTime, huntSanityThreashold, huntCooldown, smudgeHuntCooldown, footstepAudioRange, maxTempSwing;
    [SerializeField] protected float eqipmentDetectionRange, equipmentDisruptionRange;
    [SerializeField] protected Vector2 blinkVisibleMinMax, blinkInvisibleMinMax, perFlickerLengthMinMax, huntStartingDistanceMinMax, huntDurationRandomMinMax, tempMinMax;
    [SerializeField] protected ghostGender ghostGender;
    
    protected TextMeshProUGUI distanceText, sanityText, timeBetweenHuntsText;
    protected float currentSpeed, blinkOutTimeTemp, blinkInTimeTemp, distanceFromPlayer, previousDistanceFromPlayer, currentLOSMult = 1f, currentSanity, timeSinceLastHunt, footstepVolume, totalContractTime, currentTemp;
    private int targetTemp;
    protected bool isMale, hasLOS, isMovingForward, isMovingBackward, hasBeenSmudged, isHunting, isFirstHunt;
    protected string secondsRemainderSinceLastHunt, contractSecondsRemainder;
    protected List<int> saltsSteppedIn = new List<int>();
    protected Color targetLightColour;

    protected bool isMoving => (isMovingForward || isMovingBackward);
    protected float tempMinMaxAvg => (tempMinMax.x + tempMinMax.y) / 2f;

    public ghostGender GhostGender => ghostGender;
    public float TotalContractTime => totalContractTime;
    public float CurrentSanity => currentSanity;
    public float CurrentTemp => currentTemp;

    private void Reset()
    {
        if (gameManager == null) gameManager = FindAnyObjectByType<GameManager>();
        Ghost ghost = gameManager.DefaultGhostPrefab.GetComponent<Ghost>();
        walkSpeed = ghost.walkSpeed;
        losSpeed = ghost.losSpeed;
        losSpeedGainMultiplier = ghost.losSpeedGainMultiplier;
        losSpeedGainTime = ghost.losSpeedGainTime;
        losSpeedDecayTime = ghost.losSpeedDecayTime;
        huntSanityThreashold = ghost.huntSanityThreashold;
        huntCooldown = ghost.huntCooldown;
        smudgeHuntCooldown = ghost.smudgeHuntCooldown;
        footstepAudioRange = ghost.footstepAudioRange;
        blinkVisibleMinMax = ghost.blinkVisibleMinMax;
        blinkInvisibleMinMax = ghost.blinkInvisibleMinMax;
        perFlickerLengthMinMax = ghost.perFlickerLengthMinMax;
        huntStartingDistanceMinMax = ghost.huntStartingDistanceMinMax;
        huntDurationRandomMinMax = ghost.huntDurationRandomMinMax;
        tempMinMax = ghost.tempMinMax;
        ghostGender = ghost.ghostGender;
    }

    protected virtual void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        currentSpeed = walkSpeed;
        distanceText = gameManager.GhostModel.transform.Find("Distance").GetComponent<TextMeshProUGUI>();
        sanityText = gameManager.GhostModel.transform.Find("Sanity").GetComponent<TextMeshProUGUI>();
        timeBetweenHuntsText = gameManager.GhostModel.transform.Find("HuntTime").GetComponent<TextMeshProUGUI>();
        distanceText.enabled = false;
        currentSanity = gameManager.StartingSanity;
        currentTemp = tempMinMax.y * Random.Range(0.9f, 1f);
        targetTemp = Mathf.RoundToInt(Random.Range(tempMinMax.x, tempMinMax.y));
        isFirstHunt = true;
        targetLightColour = gameManager.Light.color;
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
        totalContractTime += Time.deltaTime;
        EvaluateTemperature();
        EvaluateLightColour();
    }

    protected virtual void EvaluateTemperature()
    {
        if (Mathf.RoundToInt(currentTemp) > targetTemp)
        {
            currentTemp = Mathf.Clamp(currentTemp + (Random.Range(tempMinMaxAvg / 150f, tempMinMaxAvg / 30f) * Time.deltaTime * Mathf.Sign(Random.Range(-1f * (currentTemp - targetTemp + 1f), 1f))), tempMinMax.x, tempMinMax.y);
        }
        else if (Mathf.RoundToInt(currentTemp) < targetTemp)
        {
            currentTemp = Mathf.Clamp(currentTemp + (Random.Range(tempMinMaxAvg / 150f, tempMinMaxAvg / 30f) * Time.deltaTime * Mathf.Sign(Random.Range(-1f, targetTemp - currentTemp + 1f))), tempMinMax.x, tempMinMax.y);
        }
        else targetTemp = Mathf.RoundToInt(Random.Range(Mathf.Max(targetTemp - maxTempSwing, tempMinMax.x), Mathf.Min(targetTemp + maxTempSwing, tempMinMax.y)));
    }

    protected void EvaluateLightColour()
    {
        if (gameManager.Light.color.a > targetLightColour.a)
        {
            gameManager.Light.color = new Color
            (
                gameManager.Light.color.r, 
                gameManager.Light.color.g, 
                gameManager.Light.color.b,
                Mathf.Clamp(gameManager.Light.color.a + (Random.Range(-gameManager.LightFlickerStrength, gameManager.LightFlickerStrength) * Time.deltaTime * Mathf.Sign(Random.Range(-1f * (gameManager.Light.color.a - targetLightColour.a + 1f), 1f))), 0f, 255f)
            );
        }
        else if (gameManager.Light.color.a < targetLightColour.a)
        {
            gameManager.Light.color = new Color
            (
                gameManager.Light.color.r,
                gameManager.Light.color.g,
                gameManager.Light.color.b,
                Mathf.Clamp(gameManager.Light.color.a + (Random.Range(-gameManager.LightFlickerStrength, gameManager.LightFlickerStrength) * Time.deltaTime * Mathf.Sign(Random.Range(-1f, gameManager.Light.color.a - targetLightColour.a + 1f))), 0f, 255f)
            );
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

    protected virtual void StepInSalt(int position)
    {
        GameObject oldSalt = gameManager.SaltParent.transform.Find(position.ToString()).gameObject;
        if (oldSalt == null)
        {
            Debug.LogError("Couldnt find old salt with name " + position.ToString());
            return;
        }
        GameObject newSalt = Instantiate(gameManager.DisturbedSaltPrefab, oldSalt.transform.position, Quaternion.identity, gameManager.SaltParent.transform);
        newSalt.name = position.ToString() + " Disturbed";
        Destroy(oldSalt);
        gameManager.SetSaltPositions(position, false);
    }

    public virtual void StartHunt()
    {
        isHunting = true;
        GetHuntSanity();
        GetTimeSinceLastHunt();
        PlayFootsteps();
        StartBlinks();
        StartMoving();
        Invoke(nameof(StopHunt), gameManager.HuntDuration + Random.Range(huntDurationRandomMinMax.x, huntDurationRandomMinMax.y));
    }

    public void StopHunt()
    {
        isHunting = false;
        CancelInvoke();
        gameManager.GhostModel.GetComponent<Image>().enabled = false;
        distanceText.enabled = false;
        sanityText.gameObject.SetActive(false);
        timeBetweenHuntsText.gameObject.SetActive(false);
        hasLOS = false;
        currentLOSMult = 1f;
    }

    protected void PlayFootsteps()
    {
        Invoke(nameof(Step), gameManager.GracePeriod);
    }

    protected virtual void Step()
    {
        //PlayOneShotWithVolume(gameManager.Footstep, Mathf.Clamp01(1 - (distanceFromPlayer / footstepAudioRange)));
        //Debug.Log(SpeedToStepsPerSecond(currentSpeed));
        Invoke(nameof(Step), SpeedToStepsPerSecond(currentSpeed));
    }

    protected float SpeedToStepsPerSecond(float speed)
    {
        return 60f / (60f / (Random.Range(-0.1f, -0.05f) + (1f / speed)));
    }

    protected void StartMoving()
    {
        distanceFromPlayer = Random.Range(huntStartingDistanceMinMax.x, huntStartingDistanceMinMax.y);
        distanceText.enabled = true;
        Invoke(nameof(Move), gameManager.GracePeriod);
    }

    protected virtual void Move()
    {
        previousDistanceFromPlayer = distanceFromPlayer;
        if (gameManager.EquipmentOn && distanceFromPlayer <= eqipmentDetectionRange) hasLOS = true;
        if (hasLOS) distanceFromPlayer -= currentSpeed * gameManager.DistanceUpdateFrequence;
        else distanceFromPlayer += (Random.Range(-currentSpeed, currentSpeed) * gameManager.DistanceUpdateFrequence);
        if (isMovingForward) distanceFromPlayer -= (gameManager.PlayerSpeed * gameManager.DistanceUpdateFrequence);
        else if (isMovingBackward) distanceFromPlayer += (gameManager.PlayerSpeed * gameManager.DistanceUpdateFrequence);
        distanceText.text = "Distance: " + (Mathf.Round(distanceFromPlayer * 100f) / 100f).ToString() + " m";

        if (distanceFromPlayer <= 0.1f)
        {
            StopHunt();
            return;
        }

        saltsSteppedIn.Clear();
        foreach (int i in gameManager.SaltPositions.Keys)
        {
            if (gameManager.SaltPositions[i] == false) continue;
            if ((distanceFromPlayer <= i && previousDistanceFromPlayer > i) || distanceFromPlayer >= i && previousDistanceFromPlayer < i) saltsSteppedIn.Add(i);
        }
        foreach (int i in saltsSteppedIn)
        {
            StepInSalt(i);
        }

        if (distanceFromPlayer <= equipmentDisruptionRange)
        {
            targetLightColour = new Color(gameManager.Light.color.r, gameManager.Light.color.g, gameManager.Light.color.b, Mathf.Clamp(gameManager.Light.color.a * Random.Range(0.15f, 1.5f), 0f, 255f));
        }

        Invoke(nameof(Move), gameManager.DistanceUpdateFrequence);
    }

    public void StartMoveForward()
    {
        isMovingForward = true;
    }

    public void StopMovingForward()
    {
        isMovingForward = false;
    }

    public void StartMovingBackward()
    {
        isMovingBackward = true;
    }

    public void StopMovingBackward()
    {
        isMovingBackward = false;
    }

    protected void StartBlinks()
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

    protected virtual void GetHuntSanity()
    {
        currentSanity = Mathf.Lerp(0f, Mathf.Min(Mathf.Max(currentSanity - 2f, 0) * 0.95f, huntSanityThreashold), Mathf.Pow(Random.value, 1f/6f));
        sanityText.text = "Sanity: " + Mathf.RoundToInt(currentSanity).ToString();
    }

    private void GetTimeSinceLastHunt()
    {
        GetRandomHuntTime();
        AddHuntTimeModifiers();
        SetHuntTimeTextAndVariables();
    }

    /// <summary>
    /// Gets called first in GetTimeSinceLastHunt
    /// </summary>
    protected virtual void GetRandomHuntTime()
    {
        timeSinceLastHunt = Mathf.Lerp(1f, (currentSanity + 20f) / 3f, Mathf.Pow(Random.value, 1.5f)) + huntCooldown;
    }

    /// <summary>
    /// Gets called second, after GetRandomHuntTime in GetTimeSinceLastHunt
    /// </summary>
    protected virtual void AddHuntTimeModifiers()
    {
        if (hasBeenSmudged)
        {
            timeSinceLastHunt += smudgeHuntCooldown - huntCooldown;
            hasBeenSmudged = false;
        }
        if (isFirstHunt)
        {
            timeSinceLastHunt += (gameManager.StartingSanity - currentSanity) * gameManager.InitialHuntSecondsPerSanity;
            isFirstHunt = false;
        }
    }

    /// <summary>
    /// Gets called third, after GetRandomHuntTime and AddHuntTimeModifiers in GetTimeSinceLastHunt.
    /// </summary>
    protected virtual void SetHuntTimeTextAndVariables()
    {
        secondsRemainderSinceLastHunt = Mathf.RoundToInt(timeSinceLastHunt % 60f).ToString();
        if (secondsRemainderSinceLastHunt.Length == 1) secondsRemainderSinceLastHunt = "0" + secondsRemainderSinceLastHunt;
        timeBetweenHuntsText.text = "Time since last hunt: " + (Mathf.RoundToInt(timeSinceLastHunt) / 60).ToString() + ":" + secondsRemainderSinceLastHunt;
        totalContractTime += timeSinceLastHunt;
    }

    public virtual void Smudge()
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
