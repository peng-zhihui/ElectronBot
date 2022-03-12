namespace UIWidgets.Examples
{
	using System;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Test dialog with different buttons.
	/// </summary>
	public class TestDialogWithDifferentButtons : MonoBehaviour
	{
		/// <summary>
		/// Dialog template.
		/// </summary>
		[SerializeField]
		public Dialog DialogTemplate;

		/// <summary>
		/// Show dialog.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		public void ShowDialog()
		{
			var actions = new DialogButton[]
			{
				new DialogButton("Cancel Button", Close, 0),
				new DialogButton("Main Button", Close, 1),
			};

			var dialog = DialogTemplate.Clone();
			dialog.Show(
				title: "Dialog With Different Buttons",
				message: "Test",
				buttons: actions,
				focusButton: "Close",
				modal: true,
				modalColor: new Color(0, 0, 0, 0.8f));
			dialog.OnDialogCancel = Close;
		}

		bool Close(DialogBase dialog, int buttonIndex)
		{
			Debug.Log(string.Format("clicked: {0}", buttonIndex.ToString()));
			return true;
		}
	}
}