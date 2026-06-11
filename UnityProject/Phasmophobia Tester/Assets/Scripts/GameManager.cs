using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
    [SerializeField] private GameObject ghostModelPrefab, saltPrefab, disturbedSaltPrefab, saltParent, saltButtonsParent, sliderPrefab, saltCancelButton, defaultGhostPrefab;
    [SerializeField] private TextMeshProUGUI saltText, contractTimeText, currentSanityText, currentTemperatureText;
    [SerializeField] private EventReference footstep;
    [SerializeField] private float gracePeriod, startingSanity, playerSpeed, distanceUpdateFrequence, initialHuntSecondsPerSanity, totalSalts, huntDuration, statsVisibleDuration, lightFlickerStrength;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image lightImage;

    // Internal variables
    private bool isMale, equipmentOn, isPlacingSalt;
    private Ghost ghostPrefab, ghost;
    private GameObject ghostModel;
    private Dictionary<int, bool> saltPositions = new Dictionary<int, bool>();
    private float saltsPlaced;
    private Coroutine showTimeCoroutine, showTempCoroutine, showSanityCoroutine;

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
    public GameObject SliderPrefab => sliderPrefab;
    public float HuntDuration => huntDuration;
    public GameObject DefaultGhostPrefab => defaultGhostPrefab;
    public Image Light => lightImage;
    public float LightFlickerStrength => lightFlickerStrength;

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

    public void ToggleEquipment(GameObject light)
    {
        equipmentOn = !equipmentOn;
        light.SetActive(equipmentOn);
    }

    public void CheckTime()
    {
        if (showTimeCoroutine != null) StopCoroutine(showTimeCoroutine);
        showTimeCoroutine = StartCoroutine(ShowTime(Time.time));
    }

    private IEnumerator ShowTime(float startTime)
    {
        contractTimeText.gameObject.SetActive(true);
        while (Time.time - startTime < statsVisibleDuration)
        {
            contractTimeText.text = "Total time in contract: " + FloatToTimeString(ghost.TotalContractTime);
            yield return new WaitForEndOfFrame();
        }

        contractTimeText.gameObject.SetActive(false);
        yield return null;
    }

    public void CheckTemperature()
    {
        if (showTempCoroutine != null) StopCoroutine(showTempCoroutine);
        showTempCoroutine = StartCoroutine(ShowTemperature(Time.time));
    }

    private IEnumerator ShowTemperature(float startTime)
    {
        currentTemperatureText.gameObject.SetActive(true);
        currentTemperatureText.text = "Current Temperature: " + Mathf.RoundToInt(ghost.CurrentTemp).ToString() + "°C";
        while (Time.time - startTime < statsVisibleDuration)
        {
            yield return new WaitForEndOfFrame();
        }
        currentTemperatureText.gameObject.SetActive(false);
        yield return null;
    }

    public void CheckSanity()
    {
       if (showSanityCoroutine != null) StopCoroutine(showSanityCoroutine);
        showSanityCoroutine = StartCoroutine(ShowSanity(Time.time));
    }

    private IEnumerator ShowSanity(float startTime)
    {
        currentSanityText.gameObject.SetActive(true);
        currentSanityText.text = "Current Sanity: " + Mathf.RoundToInt(ghost.CurrentSanity).ToString() + "%";
        while (Time.time - startTime < statsVisibleDuration)
        {
            yield return new WaitForEndOfFrame();
        }
        currentSanityText.gameObject.SetActive(false);
        yield return null;
    }

    public void SetGhostModel(Sprite model)
    {
        ghostModel.GetComponent<Image>().sprite = model;
    }

    public void SetSaltPositions(int pos, bool isStepped)
    {
        saltPositions[pos] = isStepped;
    }

    public void EnableSaltPlacement()
    {
        if (saltsPlaced >= totalSalts)
        {
            Debug.Log("Already placed max salts");
            return;
        }
        foreach (Button b in saltButtonsParent.GetComponentsInChildren<Button>())
        {
            ColorBlock cb = b.colors;
            cb.normalColor = Color.forestGreen;
            cb.highlightedColor = Color.springGreen;
            cb.selectedColor = Color.springGreen;
            b.colors = cb;
        }
        saltCancelButton.SetActive(true);
        isPlacingSalt = true;
    }

    public void CancelPlacingSalt()
    {
        foreach (Button b in saltButtonsParent.GetComponentsInChildren<Button>())
        {
            ColorBlock cb = b.colors;
            cb.selectedColor = Color.white;
            cb.normalColor = Color.white;
            cb.highlightedColor = Color.white;
            b.colors = cb;
        }
        saltCancelButton.SetActive(false);
        isPlacingSalt = false;
    }

    public void PlaceSalt(Transform t)
    {
        if (!isPlacingSalt) return;
        CancelPlacingSalt();
        if (saltsPlaced >= totalSalts)
        {
            Debug.Log("Already placed max salts");
            return;
        }
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
        saltText.text = Mathf.RoundToInt(totalSalts - saltsPlaced).ToString() + "/" + Mathf.RoundToInt(totalSalts).ToString() + " Salts Remaining";
    }

    public void StartMoveForward()
    {
        ghost.StartMoveForward();
    }

    public void StopMovingForward()
    {
        ghost.StopMovingForward();
    }

    public void StartMovingBackward()
    {
        ghost.StartMovingBackward();
    }

    public void StopMovingBackward()
    {
        ghost.StopMovingBackward();
    }

    public string FloatToTimeString(float time)
    {
        string seconds = Mathf.RoundToInt(time % 60f).ToString();
        if (seconds.Length == 1) seconds = "0" + seconds;
        return (Mathf.RoundToInt(time) / 60).ToString() + ":" + seconds;
    }
}
