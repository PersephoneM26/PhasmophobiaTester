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
    [SerializeField] private List<GameObject> ghostModelsMale = new(), ghostModelsFemale = new();
    [SerializeField] private EventReference footstep;
    [SerializeField] private float gracePeriod;

    // Internal ghost variables
    private bool isMale;
    private Ghost ghostPrefab, ghost;
    private GameObject ghostModel;

    // Ghost properties
    public GameObject GhostModel => ghostModel;

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
        if(isMale) ghostModel = ghostModelsMale[Random.Range(0, ghostModelsMale.Count)];
        else ghostModel = ghostModelsFemale[Random.Range(0, ghostModelsFemale.Count)];
        ghost = Instantiate(ghostPrefab);

        // Get ghost name
        string name = "";
        if (isMale) name = ghostNamesMale[Random.Range(0, ghostNamesMale.Count)];
        else name = ghostNamesFemale[Random.Range(0, ghostNamesFemale.Count)];
        name += " " + ghostLastNames[Random.Range(0, ghostLastNames.Count)];

        // Get ghost model
        if (isMale) ghostModel = Instantiate(ghostModelsMale[Random.Range(0, ghostModelsMale.Count)], canvas.transform);
        else ghostModel = Instantiate(ghostModelsFemale[Random.Range(0, ghostModelsFemale.Count)], canvas.transform);
        ghostModel.GetComponentInChildren<TextMeshProUGUI>().text = name;
        ghostModel.GetComponent<Image>().enabled = false;
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
    }

    public void StopHunt()
    {
        ghost.StopHunt();
    }
}
