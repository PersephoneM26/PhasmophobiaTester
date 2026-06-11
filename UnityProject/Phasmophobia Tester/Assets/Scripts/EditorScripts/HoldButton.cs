using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float holdDuration = 0.5f;
    [SerializeField] private UnityEvent onHoldComplete;

    private float startPressTime;
    private bool isHolding;
    private GameManager gameManager;
    private Slider slider;

    private void OnValidate()
    {
        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }
        if (slider == null)
        {
            Slider childSlider = null;
            foreach (Transform child in transform)
            {
                if (child.TryGetComponent<Slider>(out childSlider)) break;
            }
            if (childSlider != null) slider = childSlider;
            else
            {
                slider = Instantiate(gameManager.SliderPrefab, transform).GetComponent<Slider>();
            }
        }
    }

    private void Update()
    {
        if (isHolding)
        {
            slider.value = (Time.time - startPressTime) / holdDuration;
            if (Time.time - startPressTime > holdDuration)
            {
                onHoldComplete.Invoke();
                OnPointerUp(null);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        startPressTime = Time.time;
        slider.gameObject.SetActive(true);
        isHolding = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;
        slider.gameObject.SetActive(false);
    }
}
