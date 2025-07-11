using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandleJoystickState : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private bool hideOnAwake = true;
    [SerializeField] private Image joystickKnob = default, joystickSlot = default;

    // From unity's documentation:
    // "If you want to be notified when the user starts and/or stops touching the on-screen stick
    // implement IPointerDownHandler and/or IPointerUpHandler on a component and add it to the stick GameObject"

    private void SetVisualState(bool state)
    {
        joystickKnob.gameObject.SetActive(state);
        joystickSlot.gameObject.SetActive(state);
    }

    private void Awake()
    {
        SetVisualState(!hideOnAwake);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SetVisualState(true);

        // Since the canvas is set as "Screen Space - Camera" we need to transform the position to the rect localspace
        RectTransformUtility.ScreenPointToLocalPointInRectangle
        (
            joystickSlot.canvas.transform as RectTransform,
            eventData.pressPosition,
            joystickSlot.canvas.worldCamera,
            out Vector2 localPoint
        );
        joystickSlot.rectTransform.anchoredPosition = localPoint;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SetVisualState(false);
    }
}
