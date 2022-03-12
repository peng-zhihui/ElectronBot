namespace UIWidgets.WidgetGeneration
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Scene generator helper.
	/// </summary>
	public class SceneGeneratorHelper : MonoBehaviour
	{
		/// <summary>
		/// The canvas.
		/// </summary>
		[SerializeField]
		public GameObject Canvas;

		/// <summary>
		/// The Accordion.
		/// </summary>
		[SerializeField]
		public Accordion Accordion;

		/// <summary>
		/// The ListView's parent.
		/// </summary>
		[SerializeField]
		public RectTransform ListsParent;

		/// <summary>
		/// The TileView parent.
		/// </summary>
		[SerializeField]
		public RectTransform TileViewParent;

		/// <summary>
		/// the TreeView parent.
		/// </summary>
		[SerializeField]
		public RectTransform TreeViewParent;

		/// <summary>
		/// The ListView label.
		/// </summary>
		[SerializeField]
		public GameObject LabelListView;

		/// <summary>
		/// The ListView button.
		/// </summary>
		[SerializeField]
		public Button ListViewButton;

		/// <summary>
		/// The TreeView button.
		/// </summary>
		[SerializeField]
		public Button TreeViewButton;

		/// <summary>
		/// The button to set default style.
		/// </summary>
		[SerializeField]
		public Button StyleDefaultButton;

		/// <summary>
		/// The button to set blue style.
		/// </summary>
		[SerializeField]
		public Button StyleBlueButton;

		/// <summary>
		/// The Table parent.
		/// </summary>
		[SerializeField]
		public RectTransform TableParent;

		/// <summary>
		/// The Table label.
		/// </summary>
		[SerializeField]
		public GameObject LabelTable;

		/// <summary>
		/// The TreeGraph parent.
		/// </summary>
		[SerializeField]
		public RectTransform TreeGraphParent;

		/// <summary>
		/// The TreeGraph label.
		/// </summary>
		[SerializeField]
		public GameObject LabelTreeGraph;

		/// <summary>
		/// The Autocomplete parent.
		/// </summary>
		[SerializeField]
		public RectTransform AutocompleteParent;

		/// <summary>
		/// The Autocomplete label.
		/// </summary>
		[SerializeField]
		public GameObject LabelAutocomplete;

		/// <summary>
		/// The AutoCombobox label.
		/// </summary>
		[SerializeField]
		public GameObject LabelAutoCombobox;

		/// <summary>
		/// The Combobox label.
		/// </summary>
		[SerializeField]
		public GameObject LabelCombobox;

		/// <summary>
		/// The ComboboxMultiselect label.
		/// </summary>
		[SerializeField]
		public GameObject LabelComboboxMultiselect;
	}
}