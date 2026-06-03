
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Obake : Ghost
{
    [SerializeField] private List<int> shapeshiftBlinks = new();
    [SerializeField] private List<Sprite> maleModels = new();
    [SerializeField] private List<Sprite> femaleModels = new();

    private Sprite startingModel;
    private int blinkNumber;

    protected override void Awake()
    {
        base.Awake();
        startingModel = gameManager.GhostModel.GetComponent<Image>().sprite;
    }

    protected override void BlinkIn()
    {
        base.BlinkIn();
        blinkNumber++;
        if (shapeshiftBlinks.Contains(blinkNumber))
        {
            if (isMale) gameManager.SetGhostModel(maleModels[Mathf.RoundToInt(Mathf.Repeat(shapeshiftBlinks.IndexOf(blinkNumber), maleModels.Count))]);
            else gameManager.SetGhostModel(femaleModels[Mathf.RoundToInt(Mathf.Repeat(shapeshiftBlinks.IndexOf(blinkNumber), femaleModels.Count))]);
        }
        else gameManager.SetGhostModel(startingModel);
    }
}
