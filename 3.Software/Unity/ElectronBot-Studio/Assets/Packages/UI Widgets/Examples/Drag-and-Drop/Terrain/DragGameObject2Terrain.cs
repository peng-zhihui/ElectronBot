namespace UIWidgets.Examples
{
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Drag gameobject to terrain.
	/// Required PhysicsRaycaster on camera.
	/// </summary>
	public class DragGameObject2Terrain : DragSupport<GameObject>
	{
		/// <summary>
		/// Prefab.
		/// </summary>
		[SerializeField]
		protected GameObject Prefab;

		/// <inheritdoc/>
		protected override void InitDrag(PointerEventData eventData)
		{
			Data = Prefab;
		}
	}
}