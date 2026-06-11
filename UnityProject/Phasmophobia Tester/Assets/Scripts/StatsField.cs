using UnityEngine;
using UnityEngine.UI;

public class StatsField : MonoBehaviour
{
    private Image image;

    private void OnValidate()
    {
        if (image == null)
        {
            image = GetComponent<Image>();
        }
    }

    private void Update()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeInHierarchy == true)
            {
                image.enabled = true;
                return;
            }
        }
        image.enabled = false;
    }
}
