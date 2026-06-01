using FMODUnity;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Serialized ghost variables
    [SerializeField] private List<Ghost> ghostTypes = new(), forcedGhostTypes = new();
    [SerializeField] private List<string> ghostNamesMale, ghostNamesFemale, ghostLastNames;
    [SerializeField] private List<Sprite> ghostModelsMale = new(), ghostModelsFemale = new();
    [SerializeField] private GameObject ghostModelPrefab;
    [SerializeField] private EventReference footstep;
    [SerializeField] private float gracePeriod, startingSanity, playerSpeed, distanceUpdateFrequence;

    // Internal ghost variables
    private bool isMale;
    private Ghost ghostPrefab, ghost;
    private GameObject ghostModel;

    // Ghost properties
    public GameObject GhostModel => ghostModel;
    public float StartingSanity => startingSanity;
    public float PlayerSpeed => playerSpeed;
    public float DistanceUpdateFrequence => distanceUpdateFrequence;

    // Misc variables
    [SerializeField] private Canvas canvas;

    //Misc properties
    public EventReference Footstep => footstep;
    public float GracePeriod => gracePeriod;

    private void Start()
    {
        //setup ghost variables
        isMale = Random.value > 0.5f;
        if (forcedGhostTypes.Count > 0)
        {
            ghostTypes.Clear();
            ghostTypes.AddRange(forcedGhostTypes);
        }
        List<Ghost> genderedGhostTypes = GetGenderedGhostList(isMale, ghostTypes);
        ghostPrefab = genderedGhostTypes[Random.Range(0, genderedGhostTypes.Count)];

        // Get ghost name
        string name = "";
        if (isMale) name = ghostNamesMale[Random.Range(0, ghostNamesMale.Count)];
        else name = ghostNamesFemale[Random.Range(0, ghostNamesFemale.Count)];
        name += " " + ghostLastNames[Random.Range(0, ghostLastNames.Count)];

        // Get ghost model
        ghostModel = Instantiate(ghostModelPrefab, canvas.transform);
        if (isMale) ghostModel.GetComponent<Image>().sprite = ghostModelsMale[Random.Range(0, ghostModelsMale.Count)];
        else ghostModel.GetComponent<Image>().sprite = ghostModelsFemale[Random.Range(0, ghostModelsFemale.Count)];
        ghostModel.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = name;
        ghostModel.GetComponent<Image>().enabled = false;

        // Instantiate ghost
        ghost = Instantiate(ghostPrefab);
    }

    private List<Ghost> GetGenderedGhostList(bool isMale, List<Ghost> allGhosts)
    {
        for (int i = 0; i < allGhosts.Count; i++)
        {
            Ghost ghost = allGhosts[i];
            if (isMale && ghost.GhostGender == ghostGender.female)
            {
                allGhosts.Remove(ghost);
            }
            else if (!isMale && ghost.GhostGender == ghostGender.Male)
            {
                allGhosts.Remove(ghost);
            }
        }
        return allGhosts;
    }

    public void StartHunt()
    {
        ghost.PlayFootsteps();
        ghost.StartBlinks();
        ghost.StartMoving();
        ghost.GetTimeSinceLastHunt();
        ghost.GetHuntSanity();
    }

    public void StopHunt()
    {
        ghost.StopHunt();
    }
    public void GhostGainLOS()
    {
        ghost.GainLOS();
    }

    public void GhostLoseLOS()
    {
        ghost.LoseLOS();
    }
    public void SmudgeGhost()
    {
        ghost.Smudge();
        ghost.StopHunt();
    }
}
