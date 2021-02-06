using UnityEngine;

public class ThirdCamera : MonoBehaviour
{
    private Camera heroCamera;
    void Start()
    {
        heroCamera = GetComponentInChildren<Camera>();
    }
}
