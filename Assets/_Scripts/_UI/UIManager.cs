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
    public static bool cameraIsOpen = false;

    [Header("Popup")]
    public GameObject popupUiTransform;
    public PopupUI _popupUI;

    [Header("Breath Meter")]
    public BarUI breathMeter;
    public Image deathImage;

    public Transform spawnpoint;

    [Header("Wallet")]
    public CashUI cashUI;

    [Header("Dependencies")]
    public MovingSphere _player;

    [Header("InteractionsPrompt")]
    public InteractPromptUI _interactPrompt;

    void Awake()
    {
        Instance = this;
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

    public void AddInteractPrompt(string _text)
    {
        _interactPrompt.AddPrompt(_text);
    }

    public void RemoveInteractPrompt()
    {
        _interactPrompt.RemovePrompt();
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0;
        gameIsPaused = true;
    }

    public void UpdateWallet(float walletAmount)
    {
        cashUI.UpdateWallet(walletAmount);
    }

    public void Popup(string newtext)
    {
        popupUiTransform.SetActive(true);
        _popupUI.UpdatePopupText(newtext);
        StartCoroutine(HideGameObjectAfterDelay(popupUiTransform,2f));
    }

    public void TakePicture()
    {
        StartCoroutine(CameraFlash());
    }

    IEnumerator HideGameObjectAfterDelay(GameObject _tohide, float duration)
    {
        float time = 0f;

        while (time < duration) 
        {
            time += Time.deltaTime;
            yield return null; 
        }
        _tohide.SetActive(false);
    }

    public void BlackFade()
    {
        _player.LockInput();
        StartCoroutine(DeathFlash(1f, 1, true));
    }

    IEnumerator DeathFlash(float duration, float targetOpacity, bool firstPhase)
    {
        float time = 0f;
        float startOpacity = deathImage.color.a;
        
        if(!firstPhase)
        {
            _player.lockInput = false;
        }

        while (time < duration) 
        {
            time += Time.deltaTime;
            float percentComplete = time/duration;
            float opacity = Mathf.Lerp(startOpacity, targetOpacity, percentComplete);
            deathImage.color = new Color(deathImage.color.r, deathImage.color.g, deathImage.color.b, opacity); 
            yield return null; 
        }

        deathImage.color = new Color(deathImage.color.r, deathImage.color.g, deathImage.color.b, targetOpacity);

        if(firstPhase == true)
        {
            ResetPlayer();
            yield return new WaitForSeconds(1);
            StartCoroutine(DeathFlash(1.5f, 0,false));
        }
    }

    public void ResetPlayer()
    {
        _player.transform.position = spawnpoint.position;
        OrbitCamera.Instance.transform.rotation = spawnpoint.rotation;
        _player._breath.ResetBreath();
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
        StartCoroutine(HideGameObjectAfterDelay(photoParent,1.5f));
    }

    public void Zoom(float inOut)
    {
        if(inOut == 0) return;

        float zoomspeed = 20 * Mathf.Abs(inOut);
        float targetFov = Mathf.Sign(inOut) > 0 ? minFov : maxFov;
        Camera.main.fieldOfView = Mathf.MoveTowards(Camera.main.fieldOfView, targetFov, zoomspeed * Time.deltaTime);
    }

    public void OpenCamera()
    {
        cameraUIParent.SetActive(true);
        Camera.main.fieldOfView = maxFov;
        cameraIsOpen = true;
    }

    public void CloseCamera()
    {
        OrbitCamera.Instance.ForceFlattenTilt();
        cameraUIParent.SetActive(false);
        Camera.main.fieldOfView = 60;
        cameraIsOpen = false;
    }
}
