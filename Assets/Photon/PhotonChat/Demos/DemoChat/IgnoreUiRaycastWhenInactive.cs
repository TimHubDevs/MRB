using UnityEngine;


public class IgnoreUiRaycastWhenInactive : MonoBehaviour, ICanvasRaycastFilter
{
    private int hideX= -170;
    public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        return gameObject.activeInHierarchy;
    }

    public void HideChat()
    {
        gameObject.GetComponent<RectTransform>().anchoredPosition3D =
            new Vector3(hideX, 105, transform.position.z);
    }
    public void ShowChat()
    {
        gameObject.GetComponent<RectTransform>().anchoredPosition3D =
            new Vector3(-hideX, 105, transform.position.z);
    }
}