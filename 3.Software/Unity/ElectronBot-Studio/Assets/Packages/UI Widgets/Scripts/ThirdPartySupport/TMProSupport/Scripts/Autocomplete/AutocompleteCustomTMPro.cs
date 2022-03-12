#if UIWIDGETS_TMPRO_SUPPORT && (UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER)
namespace UIWidgets.TMProSupport
{
	using TMPro;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// Autocomplete.
	/// Allow quickly find and select from a list of values as user type.
	/// DisplayListView - used to display list of values.
	/// TargetListView - if specified selected value will be added to this list.
	/// DataSource - list of values.
	/// </summary>
	/// <typeparam name="TValue">Type of value.</typeparam>
	/// <typeparam name="TListViewComponent">Type of ListView.DefaultItem.</typeparam>
	/// <typeparam name="TListView">Type of ListView.</typeparam>
	public abstract class AutocompleteCustomTMPro<TValue, TListViewComponent, TListView> : AutocompleteCustom<TValue, TListViewComponent, TListView>
		where TListView : ListViewCustom<TListViewComponent, TValue>
		where TListViewComponent : ListViewItem
	{
		/// <summary>
		/// InputField for autocomplete.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[FormerlySerializedAs("InputFieldTMPro")]
		[System.Obsolete("Replaced with InputFieldAdapter")]
		protected TMP_InputField inputFieldTMPro;

		/// <summary>
		/// InputField for autocomplete.
		/// </summary>
		[System.Obsolete("Replaced with InputFieldAdapter")]
		public TMP_InputField InputFieldTMPro
		{
			get
			{
				return inputFieldTMPro;
			}

			set
			{
				inputFieldTMPro = value;
				InitInputField();
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