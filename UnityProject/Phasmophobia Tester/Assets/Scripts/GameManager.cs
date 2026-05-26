using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Serialized ghost variables
    [SerializeField] private List<Ghost> ghostTypes = new();
    [SerializeField] private List<string> ghostNamesMale, ghostNamesFemale;
    [SerializeField] private List<GameObject> ghostModelsMale = new(), ghostModelsFemale = new();
    [SerializeField] private EventReference footstep;

    // Internal ghost variables
    private bool isMale;
    private Ghost ghostPrefab, ghost;
    private GameObject ghostModel;

    // Ghost properties
    public GameObject GhostModel => ghostModel;

    // Misc variables
    [SerializeField] private Canvas canvas;

    //Misc properties
    public Canvas Canvas => canvas;
    public EventReference Footstep => footstep;

    private void Start()
    {
        //setup ghost variables
        isMale = Random.value > 0.5f;
        List<Ghost> genderedGhostTypes = GetGenderedGhostList(isMale, ghostTypes);
        ghostPrefab = genderedGhostTypes[Random.Range(0, genderedGhostTypes.Count)];
        if(isMale) ghostModel = ghostModelsMale[Random.Range(0, ghostModelsMale.Count)];
        else ghostModel = ghostModelsFemale[Random.Range(0, ghostModelsFemale.Count)];
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

    public void PlaySounds()
    {
        ghost.PlayFootsteps();
    }

    public void StopSounds()
    {
        ghost.StopFootsteps();
    }
}
