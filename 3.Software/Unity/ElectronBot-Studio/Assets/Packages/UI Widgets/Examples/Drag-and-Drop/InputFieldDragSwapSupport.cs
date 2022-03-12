namespace UIWidgets.Examples
{
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Drag support with content swap for the InputField.
	/// </summary>
	[RequireComponent(typeof(InputFieldAdapter))]
	public class InputFieldDragSwapSupport : InputFieldDragSupport
	{
		/// <summary>
		/// Called by a BaseInputModule when a drag is ended.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		protected override void OnEndDrag(PointerEventData eventData)
		{
			if (!IsDragged)
			{
				return;
			}

			FillRaycasts(eventData, RaycastResults);
			var target = FindTarget(eventData, RaycastResults);
			if (target != null)
			{
				target.Drop(Data, eventData);
				Dropped(true);

				// replace dragged text with drop target text
				SetData((target as InputFieldDropSupport).OriginalData);
			}
			else
			{
				Dropped(false);
			}

			ResetCursor();
		}

		void SetData(string data)
		{
			var adapter = GetComponent<InputFieldAdapter>();
			if (adapter != null)
			{
				adapter.text = data;
			}

			var input = GetComponent<InputField>();
			if (input != null)
			{
				input.text = data;
			}
		}
	}
}