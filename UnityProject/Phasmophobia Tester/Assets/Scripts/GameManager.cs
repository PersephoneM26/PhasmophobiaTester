using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Serialized ghost variables
    [SerializeField] private List<Ghost> ghostTypes = new();
    [SerializeField] private bool isMale;
    [SerializeField] private List<string> ghostNamesMale, ghostNamesFemale;
    [SerializeField] private List<GameObject> ghostModelsMale = new(), ghostModelsFemale = new();

    // Internal ghost variables
    private List<Ghost> ghostTypesMale = new(), ghostTypesFemale = new();
    private Ghost ghost;
    private GameObject ghostModel;

    // Ghost properties
    public GameObject GhostModel => ghostModel;

    // Misc variables
    [SerializeField] private Canvas canvas;

    //Misc properties
    public Canvas Canvas => canvas;

    private void Start()
    {
        //setup ghost variables
        isMale = Random.value > 0.5f;
        List<Ghost> genderedGhostTypes = GetGenderedGhostList(isMale, ghostTypes);
        ghost = genderedGhostTypes[Random.Range(0, genderedGhostTypes.Count)];
        if(isMale) ghostModel = ghostModelsMale[Random.Range(0, ghostModelsMale.Count)];
        else ghostModel = ghostModelsFemale[Random.Range(0, ghostModelsFemale.Count)];
        Instantiate(ghost);
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

    private void PlaySounds()
    {

    }

    private void StopSounds()
    {

    }
}
