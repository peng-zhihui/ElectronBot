namespace UIWidgets.WidgetGeneration
{
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Picker generator helper.
	/// </summary>
	public class PickerGeneratorHelper : MonoBehaviour
	{
		/// <summary>
		/// Main object.
		/// </summary>
		public GameObject Main;

		/// <summary>
		/// Cancel button.
		/// </summary>
		public GameObject Title;

		/// <summary>
		/// Container.
		/// </summary>
		public Transform Content;

		/// <summary>
		/// Close button.
		/// </summary>
		public Button ButtonClose;

		/// <summary>
		/// OK button.
		/// </summary>
		public StyleSupportButton ButtonOK;

		/// <summary>
		/// Cancel button.
		/// </summary>
		public StyleSupportButton ButtonCancel;
	}
}