using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField]
    GameObject cameraUIParent;
    [SerializeField]
    Image flashImage; 
    [SerializeField]
    public RectTransform viewfinderBounds;
    [SerializeField]
    GameObject PhotoParent;

    public float minFov = 50;
    public float maxFov = 30;

    void Awake()
    {
        Instance = this;
        CloseCamera();
    }

    public void TakePicture()
    {
        StartCoroutine(CameraFlash());
    }

    IEnumerator HidePhotoAfterDelay()
    {
        float duration = 2f;
        float time = 0f;

        while (time < duration) 
        {
            time += Time.deltaTime;
            yield return null; 
        }
        PhotoParent.SetActive(false);
    }

    IEnumerator CameraFlash()
    {
        float duration = 0.5f;
        float time = 0f;
        yield return null; 

        while (time < duration) 
        {
            PhotoParent.SetActive(true);
            time += Time.deltaTime;
            float percentComplete = time/duration;
            flashImage.color = new Color(255f, 255f, 255f, 1 - percentComplete); 
            yield return null; 
        }
        flashImage.color = new Color(255f, 255f, 255f, 0f);
        StartCoroutine(HidePhotoAfterDelay());
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
