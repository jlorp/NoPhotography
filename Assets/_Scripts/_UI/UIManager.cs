using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Camera Menu")]

    public GameObject cameraUIParent;
    public Image flashImage; 
    public RectTransform viewfinderBounds;
    public GameObject photoParent;

    public float minFov = 50;
    public float maxFov = 30;


    [Header("Pause Menu")]

    public GameObject pauseMenuUI;
    public GameObject goalListParent;

    public static bool gameIsPaused = false;

    void Awake()
    {
        Instance = this;
        CloseCamera();
    }

    void Update()
    {
        if(Input.GetButtonDown("Pause"))
        {
            if(gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1;
        gameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0;
        gameIsPaused = true;
    }

    public void TakePicture()
    {
        StartCoroutine(CameraFlash());
    }

    IEnumerator HidePhotoAfterDelay()
    {
        float duration = 1.5f;
        float time = 0f;

        while (time < duration) 
        {
            time += Time.deltaTime;
            yield return null; 
        }
        photoParent.SetActive(false);
    }

    IEnumerator CameraFlash()
    {
        float duration = 0.5f;
        float time = 0f;
        yield return null; 

        while (time < duration) 
        {
            photoParent.SetActive(true);
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
