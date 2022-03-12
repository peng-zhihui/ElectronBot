namespace UIWidgets
{
	using UIWidgets.Styles;
	using UnityEngine;

	/// <summary>
	/// References to the prefabs for the menu.
	/// </summary>
	public class PrefabsMenu : ScriptableObject
	{
#if UNITY_EDITOR
		static PrefabsMenu instance;

		/// <summary>
		/// Instance.
		/// </summary>
		public static PrefabsMenu Instance
		{
			get
			{
				if (instance == null)
				{
					instance = UtilitiesEditor.LoadAssetWithGUID<PrefabsMenu>(ReferenceGUID.PrefabsMenu);
				}

				return instance;
			}

			set
			{
				instance = value;
			}
		}
#endif

		#region Collections

		/// <summary>
		/// AutocompleteCombobox.
		/// </summary>
		[Header("Collections")]
		[SerializeField]
		public GameObject AutocompleteCombobox;

		/// <summary>
		/// AutoComboboxIcons.
		/// </summary>
		[SerializeField]
		public GameObject AutoComboboxIcons;

		/// <summary>
		/// AutoComboboxString.
		/// </summary>
		[SerializeField]
		public GameObject AutoComboboxString;

		/// <summary>
		/// Combobox.
		/// </summary>
		[SerializeField]
		public GameObject Combobox;

		/// <summary>
		/// ComboboxEnum.
		/// </summary>
		[SerializeField]
		public GameObject ComboboxEnum;

		/// <summary>
		/// ComboboxEnumMultiselect.
		/// </summary>
		[SerializeField]
		public GameObject ComboboxEnumMultiselect;

		/// <summary>
		/// ComboboxIcons.
		/// </summary>
		[SerializeField]
		public GameObject ComboboxIcons;

		/// <summary>
		/// ComboboxIcons with enabled multiselect.
		/// </summary>
		[SerializeField]
		public GameObject ComboboxIconsMultiselect;

		/// <summary>
		/// Combobox with InputField.
		/// </summary>
		[SerializeField]
		public GameObject ComboboxInputField;

		/// <summary>
		/// DirectoryTreeView.
		/// </summary>
		[SerializeField]
		public GameObject DirectoryTreeView;

		/// <summary>
		/// FileListView.
		/// </summary>
		[SerializeField]
		public GameObject FileListView;

		/// <summary>
		/// ListView.
		/// </summary>
		[SerializeField]
		public GameObject ListView;

		/// <summary>
		/// ListViewColors.
		/// </summary>
		[SerializeField]
		public GameObject ListViewColors;

		/// <summary>
		/// ListViewEnum.
		/// </summary>
		[SerializeField]
		public GameObject ListViewEnum;

		/// <summary>
		/// ListViewInt.
		/// </summary>
		[SerializeField]
		public GameObject ListViewInt;

		/// <summary>
		/// ListViewHeight.
		/// </summary>
		[SerializeField]
		public GameObject ListViewHeight;

		/// <summary>
		/// ListViewIcons.
		/// </summary>
		[SerializeField]
		public GameObject ListViewIcons;

		/// <summary>
		/// ListViewPaginator.
		/// </summary>
		[SerializeField]
		public GameObject ListViewPaginator;

		/// <summary>
		/// TreeView.
		/// </summary>
		[SerializeField]
		public GameObject TreeView;

		#endregion

		#region Containers

		/// <summary>
		/// Accordion.
		/// </summary>
		[Header("Containers")]
		[SerializeField]
		public GameObject Accordion;

		/// <summary>
		/// Tabs.
		/// </summary>
		[SerializeField]
		public GameObject Tabs;

		/// <summary>
		/// TabsLeft.
		/// </summary>
		[SerializeField]
		public GameObject TabsLeft;

		/// <summary>
		/// TabsIcons.
		/// </summary>
		[SerializeField]
		public GameObject TabsIcons;

		/// <summary>
		/// TabsIconsLeft.
		/// </summary>
		[SerializeField]
		public GameObject TabsIconsLeft;

		#endregion

		#region Dialogs

		/// <summary>
		/// DatePicker.
		/// </summary>
		[Header("Dialogs")]
		[SerializeField]
		public GameObject DatePicker;

		/// <summary>
		/// DateTimePicker.
		/// </summary>
		[SerializeField]
		public GameObject DateTimePicker;

		/// <summary>
		/// Dialog template.
		/// </summary>
		[SerializeField]
		public GameObject DialogTemplate;

		/// <summary>
		/// FileDialog.
		/// </summary>
		[SerializeField]
		public GameObject FileDialog;

		/// <summary>
		/// FolderDialog.
		/// </summary>
		[SerializeField]
		public GameObject FolderDialog;

		/// <summary>
		/// Notify template.
		/// </summary>
		[SerializeField]
		public GameObject NotifyTemplate;

		/// <summary>
		/// PickerBool.
		/// </summary>
		[SerializeField]
		public GameObject PickerBool;

		/// <summary>
		/// PickerIcons.
		/// </summary>
		[SerializeField]
		public GameObject PickerIcons;

		/// <summary>
		/// PickerInt.
		/// </summary>
		[SerializeField]
		public GameObject PickerInt;

		/// <summary>
		/// PickerString.
		/// </summary>
		[SerializeField]
		public GameObject PickerString;

		/// <summary>
		/// Popup.
		/// </summary>
		[SerializeField]
		public GameObject Popup;

		/// <summary>
		/// TimePicker.
		/// </summary>
		[SerializeField]
		public GameObject TimePicker;

		#endregion

		#region Input

		/// <summary>
		/// Autocomplete.
		/// </summary>
		[Header("Input")]
		[SerializeField]
		public GameObject Autocomplete;

		/// <summary>
		/// AutocompleteIcons.
		/// </summary>
		[SerializeField]
		public GameObject AutocompleteIcons;

		/// <summary>
		/// ButtonBig.
		/// </summary>
		[SerializeField]
		public GameObject ButtonBig;

		/// <summary>
		/// ButtonSmall.
		/// </summary>
		[SerializeField]
		public GameObject ButtonSmall;

		/// <summary>
		/// Calendar.
		/// </summary>
		[SerializeField]
		public GameObject Calendar;

		/// <summary>
		/// CenteredSlider.
		/// </summary>
		[SerializeField]
		public GameObject CenteredSlider;

		/// <summary>
		/// CenteredSliderVertical.
		/// </summary>
		[SerializeField]
		public GameObject CenteredSliderVertical;

		/// <summary>
		/// CircularSlider.
		/// </summary>
		[SerializeField]
		public GameObject CircularSlider;

		/// <summary>
		/// CircularSliderFloat.
		/// </summary>
		[SerializeField]
		public GameObject CircularSliderFloat;

		/// <summary>
		/// ColorPicker.
		/// </summary>
		[SerializeField]
		public GameObject ColorPicker;

		/// <summary>
		/// ColorPickerRange.
		/// </summary>
		[SerializeField]
		public GameObject ColorPickerRange;

		/// <summary>
		/// ColorPickerRangeHSV.
		/// </summary>
		[SerializeField]
		public GameObject ColorPickerRangeHSV;

		/// <summary>
		/// ColorsList.
		/// </summary>
		[SerializeField]
		public GameObject ColorsList;

		/// <summary>
		/// DateTime.
		/// </summary>
		[SerializeField]
		public GameObject DateTime;

		/// <summary>
		/// DateScroller.
		/// </summary>
		[SerializeField]
		public GameObject DateScroller;

		/// <summary>
		/// DateTimeScroller.
		/// </summary>
		[SerializeField]
		public GameObject DateTimeScroller;

		/// <summary>
		/// DateTimeScrollerSeparate.
		/// </summary>
		[SerializeField]
		public GameObject DateTimeScrollerSeparate;

		/// <summary>
		/// RangeSlider.
		/// </summary>
		[SerializeField]
		public GameObject RangeSlider;

		/// <summary>
		/// RangeSliderFloat.
		/// </summary>
		[SerializeField]
		public GameObject RangeSliderFloat;

		/// <summary>
		/// RangeSliderVertical.
		/// </summary>
		[SerializeField]
		public GameObject RangeSliderVertical;

		/// <summary>
		/// RangeSliderFloatVertical.
		/// </summary>
		[SerializeField]
		public GameObject RangeSliderFloatVertical;

		/// <summary>
		/// ScaleHorizontal.
		/// </summary>
		[SerializeField]
		public GameObject ScaleHorizontal;

		/// <summary>
		/// ScaleVertical.
		/// </summary>
		[SerializeField]
		public GameObject ScaleVertical;

		/// <summary>
		/// Spinner.
		/// </summary>
		[SerializeField]
		public GameObject Spinner;

		/// <summary>
		/// SpinnerFloat.
		/// </summary>
		[SerializeField]
		public GameObject SpinnerFloat;

		/// <summary>
		/// SplitButton.
		/// </summary>
		[SerializeField]
		public GameObject SplitButton;

		/// <summary>
		/// Switch.
		/// </summary>
		[SerializeField]
		public GameObject Switch;

		/// <summary>
		/// Time12.
		/// </summary>
		[SerializeField]
		public GameObject Time12;

		/// <summary>
		/// Time24.
		/// </summary>
		[SerializeField]
		public GameObject Time24;

		/// <summary>
		/// TimeAnalog.
		/// </summary>
		[SerializeField]
		public GameObject TimeAnalog;

		/// <summary>
		/// TimeScroller.
		/// </summary>
		[SerializeField]
		public GameObject TimeScroller;

		#endregion

		#region Miscellaneous

		/// <summary>
		/// AudioPlayer.
		/// </summary>
		[Header("Miscellaneous")]
		[SerializeField]
		public GameObject AudioPlayer;

		/// <summary>
		/// ContextMenu Template.
		/// </summary>
		[SerializeField]
		public GameObject ContextMenuTemplate;

		/// <summary>
		/// Progressbar.
		/// </summary>
		[SerializeField]
		public GameObject Progressbar;

		/// <summary>
		/// ProgressbarDeterminate.
		/// </summary>
		[SerializeField]
		public GameObject ProgressbarDeterminate;

		/// <summary>
		/// ProgressbarIndeterminate.
		/// </summary>
		[SerializeField]
		public GameObject ProgressbarIndeterminate;

		/// <summary>
		/// ScrollRectPaginator.
		/// </summary>
		[SerializeField]
		public GameObject ScrollRectPaginator;

		/// <summary>
		/// ScrollRectNumericPaginator.
		/// </summary>
		[SerializeField]
		public GameObject ScrollRectNumericPaginator;

		/// <summary>
		/// Sidebar.
		/// </summary>
		[SerializeField]
		public GameObject Sidebar;

		/// <summary>
		/// Tooltip.
		/// </summary>
		[SerializeField]
		public GameObject Tooltip;

		/// <summary>
		/// TooltipString.
		/// </summary>
		[SerializeField]
		public GameObject TooltipString;

		#endregion

		#region Other

		/// <summary>
		/// Default style.
		/// </summary>
		[Header("Other (not menu)")]
		[SerializeField]
		public Style DefaultStyle;

		#endregion
	}
}