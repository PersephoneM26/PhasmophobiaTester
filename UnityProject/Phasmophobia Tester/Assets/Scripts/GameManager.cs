using FMODUnity;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class GameManager : MonoBehaviour
{
    // Serialized variables
    [SerializeField] private List<Ghost> ghostTypes = new(), forcedGhostTypes = new();
    [SerializeField] private List<string> ghostNamesMale, ghostNamesFemale, ghostLastNames;
    [SerializeField] private List<Sprite> ghostModelsMale = new(), ghostModelsFemale = new();
    [SerializeField] private GameObject ghostModelPrefab, saltPrefab, disturbedSaltPrefab, saltParent;
    [SerializeField] private EventReference footstep;
    [SerializeField] private float gracePeriod, startingSanity, playerSpeed, distanceUpdateFrequence, initialHuntSecondsPerSanity, totalSalts;
    [SerializeField] private Canvas canvas;

    // Internal variables
    private bool isMale, equipmentOn;
    private Ghost ghostPrefab, ghost;
    private GameObject ghostModel;
    private Dictionary<int, bool> saltPositions = new Dictionary<int, bool>();
    private float saltsPlaced;

    // properties
    public GameObject GhostModel => ghostModel;
    public float StartingSanity => startingSanity;
    public float InitialHuntSecondsPerSanity => initialHuntSecondsPerSanity;
    public Dictionary<int, bool> SaltPositions => saltPositions;
    public GameObject SaltParent => saltParent;
    public float DistanceUpdateFrequence => distanceUpdateFrequence;
    public float PlayerSpeed => playerSpeed;
    public EventReference Footstep => footstep;
    public float GracePeriod => gracePeriod;
    public bool EquipmentOn => equipmentOn;
    public GameObject DisturbedSaltPrefab => disturbedSaltPrefab;

    // Misc variables


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
        ghost.SetGender(isMale);
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
        ghost.StartHunt();
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

    public void ToggleEquipment()
    {
        equipmentOn = !equipmentOn;
    }

    public void SetGhostModel(Sprite model)
    {
        ghostModel.GetComponent<Image>().sprite = model;
    }

    public void SetSaltPositions(int pos, bool isStepped)
    {
        saltPositions[pos] = isStepped;
    }

    public void PlaceSalt(Transform t)
    {
        if (!int.TryParse(Regex.Replace(t.name, @"[^\d]", ""), out int position))
        {
            Debug.LogError("Couldnt read any numbers in button " + t.name);
            return;
        }
        if (saltPositions.ContainsKey(position))
        {
            Debug.Log("contains key " + position.ToString());
            return;
        }
        GameObject s = Instantiate(saltPrefab, t.position, Quaternion.identity, saltParent.transform);
        s.name = position.ToString();
        saltPositions.Add(position, true);
        saltsPlaced++;
    }
}
