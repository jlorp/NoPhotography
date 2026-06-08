using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLogic : MonoBehaviour
{
    bool picReady = true;

    public void CameraControls()
	{
		float zoomInput = Input.GetAxisRaw("CameraZoom");
		UIManager.Instance.Zoom(zoomInput);

        float shutterAxis = Input.GetAxisRaw("Shutter");
        picReady |= shutterAxis < .1f;

		if((picReady && shutterAxis > .9f) || Input.GetButtonDown("Shutter Mouse"))
		{
			UIManager.Instance.TakePicture();
            picReady = false;
		}
	}
}
