using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField]
    GameObject cameraUIParent;
    public float minFov = 50;
    public float maxFov = 30;

    void Start()
    {
        Instance = this;
        CloseCamera();
    }

    public void Zoom(float inOut)
    {
        if(inOut == 0) return;

        float zoomspeed = 20;
        float targetFov = Mathf.Sign(inOut) > 0 ? minFov : maxFov;
        Camera.main.fieldOfView = Mathf.MoveTowards(Camera.main.fieldOfView, targetFov, zoomspeed * Time.deltaTime);
    }

    public void OpenCamera()
    {
        cameraUIParent.SetActive(true);
        Camera.main.fieldOfView = maxFov;
    }

    public void CloseCamera()
    {
        cameraUIParent.SetActive(false);
        Camera.main.fieldOfView = 60;
    }
}
