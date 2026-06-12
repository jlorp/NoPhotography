using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLogic : MonoBehaviour
{
    bool picReady = true;
	public List<Item> PhotoContents;
	List<ItemData> PhotoContentsItemData;

	public PhotoCapture _photoCapture;

	Vector3 viewfinderBounds;
	float viewFinderWidth;
	float viewfinderHeight;
	Vector2 viewFinderCenter;
	public Rect viewportRect;

	void Start()
	{
		SetViewFinderVariables();
	}

	void SetViewFinderVariables()
	{
		//Set Array Spacing
		Vector3[] corners = new Vector3[4];
        UIManager.Instance.viewfinderBounds.GetWorldCorners(corners);

		Vector3 startPosition = corners[1];
		Vector3 endPosition = corners[3];

		viewFinderWidth = endPosition.x - startPosition.x;
		viewfinderHeight = endPosition.y - startPosition.y;
		viewFinderCenter = new Vector2(startPosition.x,endPosition.y);

		viewportRect = new Rect(viewFinderCenter.x, viewFinderCenter.y, viewFinderWidth, Mathf.Abs(viewfinderHeight));
	}

    public void CameraControls()
	{
		if(UIManager.gameIsPaused) return;
		
		float zoomInput = Input.GetAxisRaw("CameraZoom");
		UIManager.Instance.Zoom(zoomInput);

		float zoomInputMouse = Input.GetAxisRaw("CameraZoomMouse");
		UIManager.Instance.Zoom(zoomInputMouse * 30);

        float shutterAxis = Input.GetAxisRaw("Shutter");
        picReady |= shutterAxis < .1f;

		if((picReady && shutterAxis > .9f) || Input.GetButtonDown("Shutter Mouse"))
		{
			SetViewFinderVariables();
			_photoCapture.TakePicture(viewportRect);
			UIManager.Instance.TakePicture();
            picReady = false;
			RaycastHit[] photoCasts = RaycastArray(20, 30);
			PhotoContents = SortArray(photoCasts);
			PhotoContentsItemData = ItemListToItemDataList(PhotoContents);
			GoalManager.Instance.CheckAgainstGoals(PhotoContentsItemData);
		}
	}

	List<ItemData> ItemListToItemDataList(List<Item> _items)
	{
		List<ItemData> itemDataList = new List<ItemData>();

		foreach(Item _item in _items)
		{
			itemDataList.Add(_item.itemData);
		}
		
		return itemDataList;
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

				if(Physics.Raycast(point, direction, out hits[index], castDistance))
				{
					//Debug.DrawRay(hits[index].point, -direction * hits[index].distance, Color.green, 2);
				}
				else
				{
					//Debug.DrawRay(point, direction * castDistance, Color.red, 2);
				}
            }
        }
		return hits;
	}
}
