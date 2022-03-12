namespace UIWidgets.Examples
{
	using UnityEngine;

	/// <summary>
	/// Test draggable target.
	/// </summary>
	public class TestDraggableTarget : MonoBehaviour
	{
		/// <summary>
		/// Set target.
		/// </summary>
		/// <param name="target">Target.</param>
		public void SetTarget(RectTransform target)
		{
			gameObject.SetActive(true);

			var draggable = GetComponent<Draggable>();
			if (draggable != null)
			{
				draggable.Target = target;
			}

			var resizable = GetComponent<Resizable>();
			if (resizable != null)
			{
				resizable.Target = target;
			}

			var rotatable = GetComponent<Rotatable>();
			if (rotatable != null)
			{
				rotatable.Target = target;
			}
		}
	}
}