namespace UIWidgets.Examples
{
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Drag support for the InputField.
	/// </summary>
	[RequireComponent(typeof(InputFieldAdapter))]
	public class InputFieldDragSupport : DragSupport<string>
	{
		/// <summary>
		/// Set Data, which will be passed to Drop component.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		protected override void InitDrag(PointerEventData eventData)
		{
			Data = GetData();

			ShowDragInfo();
		}

		string GetData()
		{
			var adapter = GetComponent<InputFieldAdapter>();
			if (adapter != null)
			{
				return adapter.text;
			}

			var input = GetComponent<InputField>();
			if (input != null)
			{
				return input.text;
			}

			return string.Empty;
		}

		/// <summary>
		/// Called when drop completed.
		/// </summary>
		/// <param name="success"><c>true</c> if Drop component received data; otherwise, <c>false</c>.</param>
		public override void Dropped(bool success)
		{
			HideDragInfo();

			base.Dropped(success);
		}

		/// <summary>
		/// Component to display draggable info.
		/// </summary>
		[SerializeField]
		public GameObject DragInfo;

		/// <summary>
		/// DragInfo offset.
		/// </summary>
		[SerializeField]
		public Vector3 DragInfoOffset = new Vector3(-5, 5, 0);

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected override void Start()
		{
			base.Start();

			if (DragInfo != null)
			{
				DragInfo.SetActive(false);
			}
		}

		/// <summary>
		/// Shows the drag info.
		/// </summary>
		protected virtual void ShowDragInfo()
		{
			if (DragInfo == null)
			{
				return;
			}

			DragInfo.transform.SetParent(DragPoint, false);
			DragInfo.transform.localPosition = DragInfoOffset;

			DragInfo.SetActive(true);

			var adapter = DragInfo.GetComponentInChildren<TextAdapter>();
			if (adapter != null)
			{
				adapter.text = Data;
			}
		}

		/// <summary>
		/// Hides the drag info.
		/// </summary>
		protected virtual void HideDragInfo()
		{
			if (DragInfo == null)
			{
				return;
			}

			DragInfo.SetActive(false);
		}
	}
}