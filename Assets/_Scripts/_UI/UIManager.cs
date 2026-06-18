using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public bool skipStartup;

    [Header("Start Menu")]
    public GameObject startMenuParent;

    [Header("Camera Menu")]

    public GameObject cameraUIParent;
    public Image flashImage; 
    public RectTransform viewfinderBounds;
    public GameObject photoParent;

    public float minFov = 50;
    public float maxFov = 30;

    public static bool cameraIsOpen = false;


    [Header("Pause Menu")]

    public GameObject pauseMenuUI;
    public GameObject goalListParent;
    public RectTransform goalRect;
    float goalRectYStart;

    public static bool gameIsPaused = false;

    [Header("Popup")]
    public GameObject popupUiTransform;
    public PopupUI _popupUI;

    [Header("Breath Meter")]
    public OxygenBarMaster_UI breathMeter;
    public Image deathImage;
    public GameObject breathParent;

    public Transform spawnpoint;

    [Header("Dependencies")]
    public MovingSphere _player;

    [Header("InteractionsPrompt")]
    public InteractPromptUI _interactPrompt;

    [Header("Exp Bar")]
    public GameObject expParent;
    public BarUI expMeter;

    [Header("Unlock Item")]
    public ItemUnlockUI _itemUI;

    [Header("Input Prompts")]
    public GameObject openCameraPrompt;
    public GameObject closeCameraPrompt;
    public GameObject snapPhotoPrompt;

    bool inStartMenu = true;

    void Awake()
    {
        Instance = this;
        goalRectYStart = goalRect.rect.position.y;
    }

    public void CloseStartMenu()
    {
        inStartMenu = false;
        startMenuParent.SetActive(false);
        breathParent.SetActive(true);
        pauseMenuUI.SetActive(true);
        openCameraPrompt.SetActive(true);
        
        CloseCamera();
    }

    void Start()
    {
        if(skipStartup)
        {
            OrbitCamera.Instance.LerpToActivation();
            CloseStartMenu();
            _player.transform.SetParent(null);
            _player.body.isKinematic = false;
            ResetPlayer();
            StartCoroutine(DeathFlash(0.5f, 0,false, 0.25f));
            _player.isHeldByClaw = false;
        }
        else
        {
            _player.isHeldByClaw = true;
            breathParent.SetActive(false);
            StartCoroutine(DeathFlash(1.5f, 0,false, 0.25f));
        }

        expParent.SetActive(false);
    }


    void Update()
    {
        if(inStartMenu) return;

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
        gameIsPaused = false;
        goalRect.position = new Vector2(goalRect.rect.position.x, goalRectYStart);
    }

    void Pause()
    {
        gameIsPaused = true;
        float goalCount = GoalManager.Instance.Goals.Count + GoalManager.Instance.HeldGoals.Count;
        float goalHeightAmount = (25 + 10) * goalCount;
        goalRect.position = new Vector2(goalRect.rect.position.x, goalRectYStart + goalHeightAmount);
    }

    public void AddInteractPrompt(string _text)
    {
        _interactPrompt.AddPrompt(_text);
    }

    public void RemoveInteractPrompt()
    {
        _interactPrompt.RemovePrompt();
    }



    public void Popup(string newtext, int _color)
    {
        popupUiTransform.SetActive(true);
        _popupUI.UpdatePopupText(newtext, _color);
        StartCoroutine(HideGameObjectAfterDelay(popupUiTransform,3f));
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
        OrbitCamera.Instance.DeactivateCamera();
        StartCoroutine(DeathFlash(1f, 1, true, 0.5f));
    }

    IEnumerator DeathFlash(float duration, float targetOpacity, bool firstPhase, float startDelay = 0)
    {
        float time = 0f;
        float startOpacity = 1-targetOpacity;
        deathImage.color = new Color(deathImage.color.r, deathImage.color.g, deathImage.color.b, startOpacity); 

        yield return new WaitForSeconds(startDelay);
        
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
            _player.submarine.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            StartCoroutine(DeathFlash(1.5f, 0,false));
            OrbitCamera.Instance.GoToRespawnCameraPosition();
        }
    }

    public void ResetPlayer()
    {
        _player.transform.SetParent(spawnpoint);
        _player.transform.localPosition = Vector3.zero;
        _player.transform.localRotation = Quaternion.Euler(new Vector3(0,180,0));
        _player.body.isKinematic = true;
        _player.isHeldByClaw = true;

        ClawLogic.Instance.DropClaw();
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
        if(inStartMenu) return;

        openCameraPrompt.SetActive(false);
        closeCameraPrompt.SetActive(true);

        pauseMenuUI.SetActive(false);
        cameraUIParent.SetActive(true);
        breathParent.SetActive(false);
        Camera.main.fieldOfView = maxFov;
        cameraIsOpen = true;
        OrbitCamera.Instance.distance=0;
        _player.submarine.gameObject.SetActive(false);
        _player.LockInput();
    }

    public void CloseCamera()
    {
        closeCameraPrompt.SetActive(false);
        OrbitCamera.Instance.ForceFlattenTilt();
        cameraUIParent.SetActive(false);

        if(!_player.isHeldByClaw)
        {
            openCameraPrompt.SetActive(true);
            pauseMenuUI.SetActive(true);
        }
        breathParent.SetActive(true);
        Camera.main.fieldOfView = 60;
        cameraIsOpen = false;
        OrbitCamera.Instance.distance=4;
        _player.submarine.gameObject.SetActive(true);
        _player.UnlockInput();
    }
}
