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

    public ghostGender GhostGender => ghostGender;

    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        canvas = gameManager.Canvas;
        ghostModel = gameManager.GhostModel;
        Instantiate(ghostModel, canvas.transform);
    }


    protected virtual void GainLOS(bool hasEquipment)
    {

    }

    protected virtual void LoseLOS()
    {

    }
}
