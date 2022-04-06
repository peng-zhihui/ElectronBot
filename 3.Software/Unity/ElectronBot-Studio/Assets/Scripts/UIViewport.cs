using UnityEngine;

/// <summary>
/// This script can be used to restrict camera rendering to a specific part of the screen by specifying the two corners.
/// </summary>

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("UI/Viewport Camera")]
public class UIViewport : MonoBehaviour
{
	public Transform topLeft;
	public Transform bottomRight;
	public float fullSize = 1f;

	public int screenWidth = 0;
	public int screenHeight = 0;

	public Vector2 topLeftPos = new Vector2();
	public Vector2 bottomRightPos = new Vector2();

	Camera mCam;

	void Start ()
	{
		mCam = GetComponent<Camera>();
	}

	void LateUpdate ()
	{
		screenWidth = Screen.width;
		screenHeight = Screen.height;

		if (topLeft != null) {
			topLeftPos.x = topLeft.position.x;
			topLeftPos.y = topLeft.position.y;
		}
		if (bottomRightPos != null) {
			bottomRightPos.x = bottomRight.position.x;
			bottomRightPos.y = bottomRight.position.y;
		}
		if (topLeft != null && bottomRight != null)
		{
			Vector3 tl = topLeft.position;
			Vector3 br = bottomRight.position;

			Rect rect = new Rect(tl.x / Screen.width, br.y / Screen.height,
				(br.x - tl.x) / Screen.width, (tl.y - br.y) / Screen.height);

			float size = fullSize * rect.height;

			if (rect != mCam.rect) mCam.rect = rect;
			if (mCam.orthographicSize != size) mCam.orthographicSize = size;
		}
	}
}