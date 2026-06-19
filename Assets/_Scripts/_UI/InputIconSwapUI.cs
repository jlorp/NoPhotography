using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InputIconSwapUI : MonoBehaviour
{
    public System.Action OnDeviceChanged;
    public string CurrentDeviceType { get; private set; } = "Keyboard";

    [Header("Xbox Prompts")]
    public Sprite pauseSpriteX;
    public Sprite openCloseCameraSpriteX;
    public Sprite actionSpriteX;
    public Sprite takePhotoSpriteX;
    public Sprite zoomSpriteX;
    public Sprite thrustSpriteX;
    public Sprite boostSpriteX;

    [Header("PC Prompts")]
    public Sprite pauseSpritePC;
    public Sprite openCloseCameraSpritePC;
    public Sprite actionSpritePC;
    public Sprite takePhotoSpritePC;
    public Sprite zoomSpritePC;
    public Sprite thrustSpritePC;
    public Sprite boostSpritePC;

    [Header("Images to Swap")]
    public Image openCameraImage;
    public Image closeCameraImage;
    public Image pauseImage;
    public Image actionImage;
    public Image takePhotoImage;
    public Image zoomImage;
    public Image thrustImage;
    public Image boostImage;
    public Image introImage;


    private void OnEnable()
    {
        InputSystem.onActionChange += HandleActionChange;
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.anyKey.isPressed)
        {
            SetKeyboardGlyphs();
            CurrentDeviceType = "Keyboard";
        }
    }

    private void OnDisable()
    {
        InputSystem.onActionChange -= HandleActionChange;
    }

    void SetXboxGlyphs()
    {
        introImage.sprite = openCameraImage.sprite = closeCameraImage.sprite = openCloseCameraSpriteX;
        pauseImage.sprite = pauseSpriteX;
        actionImage.sprite = actionSpriteX;
        takePhotoImage.sprite = takePhotoSpriteX;
        zoomImage.sprite = zoomSpriteX;
        thrustImage.sprite = thrustSpriteX;
        boostImage.sprite = boostSpriteX;
    }

    void SetKeyboardGlyphs()
    {
        openCameraImage.sprite = closeCameraImage.sprite = openCloseCameraSpritePC;
        pauseImage.sprite = pauseSpritePC;
        actionImage.sprite = actionSpritePC;
        takePhotoImage.sprite = takePhotoSpritePC;
        zoomImage.sprite = zoomSpritePC;
        thrustImage.sprite = thrustSpritePC;
        boostImage.sprite = boostSpritePC;
    }

    private void HandleActionChange(object obj, InputActionChange change)
    {
        // Only trigger when an action is actively performed
        if (change != InputActionChange.ActionPerformed) return;

        var action = (InputAction)obj;
        if (action.activeControl == null) return;

        string deviceName = action.activeControl.device.name;
        string newDeviceType = "Keyboard";

        if (deviceName.Contains("DualShock") || deviceName.Contains("DualSense"))
            newDeviceType = "PlayStation";
        else if (deviceName.Contains("Xbox") || deviceName.Contains("XInput"))
            newDeviceType = "Xbox";
        else if (deviceName.Contains("Switch") || deviceName.Contains("DualMuck"))
            newDeviceType = "Nintendo";

        if (newDeviceType != CurrentDeviceType)
        {
            CurrentDeviceType = newDeviceType;
            if(CurrentDeviceType == "Xbox")
            {
                SetXboxGlyphs();
            }
        }
    }
}
