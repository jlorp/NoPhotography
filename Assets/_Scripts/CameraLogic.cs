using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLogic : MonoBehaviour
{
    bool picReady = true;
	public List<Item> PhotoContents;

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
			RaycastHit[] photoCasts = RaycastArray(20, 30);
			PhotoContents = SortArray(photoCasts);
		}
	}

	List<Item> SortArray(RaycastHit[] hits)
	{
		List<Item> itemList = new List<Item>();

		foreach (RaycastHit hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject.TryGetComponent<Item>(out Item _item))
			{
				if(!itemList.Contains(_item))
				{
					itemList.Add(_item);
				}
			}
        }

		return itemList;
	}

	RaycastHit[] RaycastArray(int castDensity, float castDistance)
	{
		int castAmount = (castDensity) * (castDensity);
		RaycastHit[] hits = new RaycastHit[castAmount];

		//Set Array Spacing
		Vector3[] corners = new Vector3[4];
        UIManager.Instance.viewfinderBounds.GetWorldCorners(corners);

		Vector3 startPosition = corners[1];
		Vector3 endPosition = corners[3];

		float screenWidth = endPosition.x - startPosition.x;
		float spacingX = screenWidth/ (castDensity-1);

		float screenHeight = endPosition.y - startPosition.y;
		float spacingY = screenHeight / (castDensity-1);

		Camera cam = Camera.main;
		for (int x = 0; x < castDensity; x++)
        {
            for (int y = 0; y < castDensity; y++)
            {
				int index = (y * castDensity) + x;
				float xPosition = startPosition.x + (spacingX * x);
				float yPosition = startPosition.y + (spacingY * y);

				Vector3 point = cam.ScreenToWorldPoint(new Vector3(xPosition, yPosition, cam.nearClipPlane));
				Vector3 direction = (point - cam.transform.position).normalized;

				//Debug.DrawRay(point, direction * castDistance, Color.red, 2);
				if(Physics.Raycast(point, direction, out hits[index], castDistance))
					Debug.DrawRay(hits[index].point, -direction * hits[index].distance, Color.red, 2);
            }
        }
		return hits;
	}
}
