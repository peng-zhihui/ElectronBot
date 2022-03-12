namespace UIWidgets.Examples
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Helper for the drag-and-drop to terrain.
	/// </summary>
	public class DragTerrainHelper : MonoBehaviour
	{
		/// <summary>
		/// Images to disable to make terrain visible.
		/// </summary>
		[SerializeField]
		protected Image[] DisableImages = new Image[] { };

		/// <summary>
		/// Terrain.
		/// </summary>
		[SerializeField]
		protected Terrain Terrain;

		/// <summary>
		/// Process enable event.
		/// </summary>
		protected virtual void OnEnable()
		{
			foreach (var image in DisableImages)
			{
				image.enabled = false;
			}

			Terrain.gameObject.SetActive(true);
		}

		/// <summary>
		/// Process disable event.
		/// </summary>
		protected virtual void OnDisable()
		{
			foreach (var image in DisableImages)
			{
				image.enabled = true;
			}

			Terrain.gameObject.SetActive(false);
		}
	}
}