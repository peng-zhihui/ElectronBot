#if UIWIDGETS_TMPRO_SUPPORT && (UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER)
namespace UIWidgets.TMProSupport
{
	using TMPro;
	using UnityEngine;

	/// <summary>
	/// Autocomplete.
	/// Allow quickly find and select from a list of values as user type.
	/// DisplayListView - used to display list of values.
	/// TargetListView - if specified selected value will be added to this list.
	/// DataSource - list of values.
	/// </summary>
	[System.Obsolete("Replaced with AutocompleteString.")]
	public class AutocompleteTMPro : Autocomplete
	{
		/// <summary>
		/// TMPro InputField.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with InputFieldAdapter.")]
		protected TMP_InputField InputFieldTMPro;

		/// <summary>
		/// Gets the InputFieldProxy.
		/// </summary>
		[System.Obsolete("Replaced with InputFieldAdapter.")]
		public override IInputFieldProxy InputFieldProxy
		{
			get
			{
				return InputFieldAdapter;
			}
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public override void Upgrade()
		{
#pragma warning disable 0618
			Utilities.GetOrAddComponent(InputFieldTMPro, ref inputFieldAdapter);
#pragma warning restore 0618
		}
	}
}
#endif