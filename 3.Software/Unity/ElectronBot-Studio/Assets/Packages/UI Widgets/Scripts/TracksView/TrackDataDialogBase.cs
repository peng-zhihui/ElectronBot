namespace UIWidgets
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Base class for the dialog to add/edit data.
	/// </summary>
	/// <typeparam name="TData">Data type.</typeparam>
	/// <typeparam name="TPoint">Point type.</typeparam>
	/// <typeparam name="TDataForm">DataForm type.</typeparam>
	public class TrackDataDialogBase<TData, TPoint, TDataForm> : MonoBehaviour
		where TDataForm : TrackDataFormBase<TData, TPoint>
		where TData : ITrackData<TPoint>
		where TPoint : IComparable<TPoint>
	{
		/// <summary>
		/// Dialog template.
		/// </summary>
		[SerializeField]
		public Dialog DialogTemplate;

		/// <summary>
		/// Create header text.
		/// </summary>
		[SerializeField]
		public string HeaderCreate = "Create Task";

		/// <summary>
		/// Create button text.
		/// </summary>
		[SerializeField]
		public string ButtonCreate = "Create";

		/// <summary>
		/// Cancel button text.
		/// </summary>
		[SerializeField]
		public string ButtonCancel = "Cancel";

		/// <summary>
		/// Edit header text.
		/// </summary>
		[SerializeField]
		public string HeaderEdit = "Edit Task";

		/// <summary>
		/// Save button text.
		/// </summary>
		[SerializeField]
		public string ButtonEdit = "Save";

		/// <summary>
		/// Create data with specified StartPoint.
		/// </summary>
		/// <param name="startPoint">StartPoint.</param>
		/// <param name="onSuccess">Function to call after data created.</param>
		public void Create(TPoint startPoint, Action<TData> onSuccess)
		{
			var dialog = DialogTemplate.Clone();
			var form = dialog.GetComponent<TDataForm>();
			form.Create(startPoint);

			OpenDialog(dialog, form, HeaderCreate, ButtonCreate, onSuccess);
		}

		/// <summary>
		/// Edit data.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="onSuccess">Function to call after data edited.</param>
		public void Edit(TData data, Action<TData> onSuccess)
		{
			var dialog = DialogTemplate.Clone();
			var form = dialog.GetComponent<TDataForm>();
			form.Edit(data);

			OpenDialog(dialog, form, HeaderEdit, ButtonEdit, onSuccess);
		}

		/// <summary>
		/// Open dialog.
		/// </summary>
		/// <param name="dialog">Dialog instance.</param>
		/// <param name="form">Form.</param>
		/// <param name="title">Title.</param>
		/// <param name="okButton">OK button text.</param>
		/// <param name="onSuccess">Function to call on OK button click.</param>
		protected void OpenDialog(Dialog dialog, TDataForm form, string title, string okButton, Action<TData> onSuccess)
		{
#if CSHARP_7_3_OR_NEWER
			bool OkAction(DialogBase dialogInstance, int index)
#else
			Func<DialogBase, int, bool> OkAction = (dialogInstance, index) =>
#endif
			{
				onSuccess(form.Data);
				return true;
			}
#if !CSHARP_7_3_OR_NEWER
			;
#endif

			var actions = new DialogButton[]
			{
				new DialogButton(okButton, OkAction),
				new DialogButton(ButtonCancel, DialogBase.DefaultClose),
			};

			dialog.Show(
				title: title,
				buttons: actions,
				focusButton: okButton,
				modal: true,
				modalColor: new Color(0, 0, 0, 0.8f));
		}
	}
}