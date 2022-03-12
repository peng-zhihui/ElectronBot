namespace UIWidgets.Examples
{
	using System;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Test dialog with custom content.
	/// </summary>
	public class TestDialogWithCustomContent : MonoBehaviour
	{
		/// <summary>
		/// Dialog template.
		/// </summary>
		[SerializeField]
		public Dialog DialogTemplate;

		/// <summary>
		/// Custom content.
		/// </summary>
		[SerializeField]
		public RectTransform CustomContent;

		[NonSerialized]
		Transform ContentParent;

		/// <summary>
		/// Show dialog.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		public void ShowDialog()
		{
			ContentParent = CustomContent.parent;
			CustomContent.gameObject.SetActive(true);

			var actions = new DialogButton[]
			{
				new DialogButton("Close", DialogBase.DefaultClose),
			};

			var dialog = DialogTemplate.Clone();
			dialog.Show(
				title: "Dialog With Custom Content",
				buttons: actions,
				focusButton: "Close",
				modal: true,
				modalColor: new Color(0, 0, 0, 0.8f),
				content: CustomContent,
				onClose: RestoreContentParent);
		}

		void RestoreContentParent()
		{
			CustomContent.SetParent(ContentParent, false);
			CustomContent.gameObject.SetActive(false);
		}
	}
}