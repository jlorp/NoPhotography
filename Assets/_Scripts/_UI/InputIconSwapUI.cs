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

    [Header("PC Prompts")]
    public Sprite pauseSpritePC;
    public Sprite openCloseCameraSpritePC;
    public Sprite actionSpritePC;

    [Header("Images to Swap")]
    public Image openCameraImage;
    public Image closeCameraImage;
    public Image pauseImage;
    public Image actionImage;


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
        openCameraImage.sprite = closeCameraImage.sprite = openCloseCameraSpriteX;
        pauseImage.sprite = pauseSpriteX;
        actionImage.sprite = actionSpriteX;
    }

    void SetKeyboardGlyphs()
    {
        openCameraImage.sprite = closeCameraImage.sprite = openCloseCameraSpritePC;
        pauseImage.sprite = pauseSpritePC;
        actionImage.sprite = actionSpritePC;
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
        else 
            Debug.Log("keyboard");

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
