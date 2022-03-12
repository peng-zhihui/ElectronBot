namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// AutocompleteIconsHelper.
	/// How to use AutocompleteIcons to select one item.
	/// </summary>
	public class AutocompleteIconsHelper : MonoBehaviour, IUpgradeable
	{
		/// <summary>
		/// Autocomplete.
		/// </summary>
		[SerializeField]
		public AutocompleteIcons Autocomplete;

		/// <summary>
		/// InputField.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with InputAdapter.")]
		public InputField Input;

		/// <summary>
		/// InputField.
		/// </summary>
		[SerializeField]
		public InputFieldAdapter InputAdapter;

		[SerializeField]
		ListViewIconsItemDescription item;

		/// <summary>
		/// Selected item.
		/// </summary>
		public ListViewIconsItemDescription Item
		{
			get
			{
				return item;
			}

			set
			{
				InputAdapter.text = item.Name;
				item = value;
			}
		}

		/// <summary>
		/// Adds listeners.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required.")]
		protected virtual void Start()
		{
			InputAdapter.onEndEdit.AddListener(ResetItem);
			Autocomplete.OnOptionSelectedItem.AddListener(SetItem);
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required.")]
		protected virtual void OnDestroy()
		{
			InputAdapter.onEndEdit.RemoveListener(ResetItem);
			Autocomplete.OnOptionSelectedItem.RemoveListener(SetItem);
		}

		/// <summary>
		/// Reset item.
		/// </summary>
		/// <param name="str">Input string.</param>
		protected virtual void ResetItem(string str)
		{
			item = null;
		}

		/// <summary>
		/// Set item.
		/// </summary>
		/// <param name="it">Item.</param>
		protected virtual void SetItem(ListViewIconsItemDescription it)
		{
			item = Autocomplete.TargetListView.DataSource[0];
			Autocomplete.TargetListView.DataSource.Clear();
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public virtual void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.GetOrAddComponent(Input, ref InputAdapter);
#pragma warning restore 0612, 0618
		}

#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected virtual void OnValidate()
		{
			Compatibility.Upgrade(this);
		}
#endif
	}
}