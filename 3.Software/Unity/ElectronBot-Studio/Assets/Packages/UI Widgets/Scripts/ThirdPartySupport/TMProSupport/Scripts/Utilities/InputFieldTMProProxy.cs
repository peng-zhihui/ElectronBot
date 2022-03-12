#if UIWIDGETS_TMPRO_SUPPORT && (UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER)
namespace UIWidgets.TMProSupport
{
	using TMPro;

	/// <summary>
	/// InputFieldTMProProxy.
	/// </summary>
	[System.Obsolete("Class moved to UIWidgets namespace.")]
	public class InputFieldTMProProxy : UIWidgets.InputFieldTMProProxy
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="InputFieldTMProProxy"/> class.
		/// </summary>
		/// <param name="input">Input.</param>
		public InputFieldTMProProxy(TMP_InputField input)
			: base(input)
		{
		}
	}
}
#endif