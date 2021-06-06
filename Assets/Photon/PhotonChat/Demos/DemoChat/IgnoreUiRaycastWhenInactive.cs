using UnityEngine;


public class IgnoreUiRaycastWhenInactive : MonoBehaviour, ICanvasRaycastFilter
{
    [SerializeField] private RectTransform _rectTransform;
    private int hideX= -346;
    public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        return gameObject.activeInHierarchy;
    }

    public void HideChat()
    {
        _rectTransform.anchoredPosition3D =
            new Vector3(hideX, -215, transform.position.z);
    }
    public void ShowChat()
    {
        _rectTransform.anchoredPosition3D =
            new Vector3(-hideX, -215, transform.position.z);
    }
}