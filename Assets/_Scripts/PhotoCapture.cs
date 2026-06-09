using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotoCapture : MonoBehaviour
{  
    [Header("Photo Taker")]
    [SerializeField] private Image photoDisplayArea;

    private Texture2D screenCapture;


    public void TakePicture(Rect viewportRect)
    {
        screenCapture = new Texture2D((int)viewportRect.width, (int)viewportRect.height, TextureFormat.RGB24, false);
        StartCoroutine(CapturePhoto(viewportRect));
    }

    IEnumerator CapturePhoto(Rect viewportRect)
    {
        yield return new WaitForEndOfFrame();
        Vector2 viewCenter = viewportRect.center;

        screenCapture.ReadPixels(viewportRect, 0, 0, false);
        screenCapture.Apply();
        ShowPhoto();
    }

    void ShowPhoto()
    {
        Sprite photoSprite = Sprite.Create(screenCapture, new Rect(0.0f , 0.0f, screenCapture.width, screenCapture.height), new Vector2(0.5f, 0.5f),100.0f);
        photoDisplayArea.sprite = photoSprite;
    }
}
