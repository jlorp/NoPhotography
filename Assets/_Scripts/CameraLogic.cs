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
			RaycastArray(15, 10);
		}
	}

	RaycastHit[] RaycastArray(int castDensity, float castDistance)
	{
		int castAmount = castDensity * castDensity;
		RaycastHit[] hits = new RaycastHit[castAmount];

		Camera cam = Camera.main;
		for (int x = 0; x < castDensity; x++)
        {
            for (int y = 0; y < castDensity; y++)
            {
				int xPosition = (Screen.width / (castDensity - 1)) * x;
				int yPosition = (Screen.height / (castDensity - 1)) * y;

				Vector3 point = cam.ScreenToWorldPoint(new Vector3(xPosition, yPosition, cam.nearClipPlane));
				Vector3 direction = (point - cam.transform.position).normalized;

				Debug.DrawRay(point, direction * castDistance, Color.red, 2);

				Physics.Raycast(point, direction, out hits[x*y], castDistance);
            }
        }
		return hits;
	}
}
