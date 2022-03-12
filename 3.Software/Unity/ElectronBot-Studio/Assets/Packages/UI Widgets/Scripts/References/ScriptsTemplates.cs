namespace UIWidgets
{
	using UnityEngine;

	/// <summary>
	/// Scripts templates.
	/// </summary>
	public class ScriptsTemplates : ScriptableObject
	{
#if UNITY_EDITOR
		static ScriptsTemplates instance;

		/// <summary>
		/// Instance.
		/// </summary>
		public static ScriptsTemplates Instance
		{
			get
			{
				if (instance == null)
				{
					instance = UtilitiesEditor.LoadAssetWithGUID<ScriptsTemplates>(ReferenceGUID.ScriptsTemplates);
				}

				return instance;
			}

			set
			{
				instance = value;
			}
		}
#endif

		/// <summary>
		/// Autocomplete.
		/// </summary>
		[Header("Collections")]
		[SerializeField]
		public TextAsset Autocomplete;

		/// <summary>
		/// AutoCombobox.
		/// </summary>
		[SerializeField]
		public TextAsset AutoCombobox;

		/// <summary>
		/// Combobox.
		/// </summary>
		[SerializeField]
		public TextAsset Combobox;

		/// <summary>
		/// ListView.
		/// </summary>
		[SerializeField]
		public TextAsset ListView;

		/// <summary>
		/// ListViewComponent.
		/// </summary>
		[SerializeField]
		public TextAsset ListViewComponent;

		/// <summary>
		/// TreeGraph.
		/// </summary>
		[SerializeField]
		public TextAsset TreeGraph;

		/// <summary>
		/// TreeGraphComponent.
		/// </summary>
		[SerializeField]
		public TextAsset TreeGraphComponent;

		/// <summary>
		/// TreeView.
		/// </summary>
		[SerializeField]
		public TextAsset TreeView;

		/// <summary>
		/// TreeViewComponent.
		/// </summary>
		[SerializeField]
		public TextAsset TreeViewComponent;

		/// <summary>
		/// Comparers.
		/// </summary>
		[Header("Collections Support")]
		[SerializeField]
		public TextAsset Comparers;

		/// <summary>
		/// ListViewDragSupport.
		/// </summary>
		[SerializeField]
		public TextAsset ListViewDragSupport;

		/// <summary>
		/// ListViewDropSupport.
		/// </summary>
		[SerializeField]
		public TextAsset ListViewDropSupport;

		/// <summary>
		/// Tooltip.
		/// </summary>
		[SerializeField]
		public TextAsset Tooltip;

		/// <summary>
		/// Tooltip viewer.
		/// </summary>
		[SerializeField]
		public TextAsset TooltipViewer;

		/// <summary>
		/// TreeViewDropSupport.
		/// </summary>
		[SerializeField]
		public TextAsset TreeViewDropSupport;

		/// <summary>
		/// TreeViewNodeDragSupport.
		/// </summary>
		[SerializeField]
		public TextAsset TreeViewNodeDragSupport;

		/// <summary>
		/// TreeViewNodeDropSupport.
		/// </summary>
		[SerializeField]
		public TextAsset TreeViewNodeDropSupport;

		/// <summary>
		/// PickerListView.
		/// </summary>
		[Header("Dialogs")]
		[SerializeField]
		public TextAsset PickerListView;

		/// <summary>
		/// PickerTreeView.
		/// </summary>
		[SerializeField]
		public TextAsset PickerTreeView;

		/// <summary>
		/// Test.
		/// </summary>
		[Header("Test")]
		[SerializeField]
		public TextAsset Test;

		/// <summary>
		/// TestItem.
		/// </summary>
		[SerializeField]
		public TextAsset TestItem;

		/// <summary>
		/// TestItemOff.
		/// </summary>
		[SerializeField]
		public TextAsset TestItemOff;

		/// <summary>
		/// MenuOptions.
		/// </summary>
		[Header("Menu")]
		[SerializeField]
		public TextAsset MenuOptions;

		/// <summary>
		/// PrefabGenerator.
		/// </summary>
		[Header("Generators")]
		[SerializeField]
		public TextAsset PrefabGenerator;

		/// <summary>
		/// PrefabGeneratorAutocomplete.
		/// </summary>
		[SerializeField]
		public TextAsset PrefabGeneratorAutocomplete;

		/// <summary>
		/// PrefabGeneratorAutocompleteOff.
		/// </summary>
		[SerializeField]
		public TextAsset PrefabGeneratorAutocompleteOff;

		/// <summary>
		/// PrefabGeneratorTable.
		/// </summary>
		[SerializeField]
		public TextAsset PrefabGeneratorTable;

		/// <summary>
		/// PrefabGeneratorTableOff.
		/// </summary>
		[SerializeField]
		public TextAsset PrefabGeneratorTableOff;

		/// <summary>
		/// PrefabGeneratorScene.
		/// </summary>
		[SerializeField]
		public TextAsset PrefabGeneratorScene;
	}
}