using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoldButton : MonoBehaviour
{
    [SerializeField] private EventTrigger button;

    private bool isHoldingButton;


    private IEnumerator Click()
    {
        Debug.Log("Button clicked!");
        isHoldingButton = true;
        yield return null;
    }

    private IEnumerator HoldClick()
    {
        while (isHoldingButton)
        {
            Debug.Log("Holding button...");
            yield return null;
        }
    }

    private IEnumerator ReleaseClick()
    {
        //StopCoroutine(HoldClick());
        isHoldingButton = false;
        yield return null;
    }
}
