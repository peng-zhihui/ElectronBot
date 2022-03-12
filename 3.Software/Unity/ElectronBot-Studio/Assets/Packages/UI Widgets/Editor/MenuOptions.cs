#if UNITY_EDITOR
namespace UIWidgets
{
	using System;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Menu options.
	/// </summary>
	public static class MenuOptions
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required.")]
		static void Create(GameObject prefab)
		{
			UtilitiesEditor.CreateWidgetFromPrefab(prefab, true, ConverterTMPro.Widget2TMPro);
		}

		#region Collections

		/// <summary>
		/// Create AutocompleteCombobox.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/AutocompleteCombobox", false, 1000)]
		public static void CreateAutocompleteCombobox()
		{
			Create(PrefabsMenu.Instance.AutocompleteCombobox);
		}

		/// <summary>
		/// Create AutoComboboxIcons.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/AutoComboboxIcons", false, 1002)]
		public static void CreateAutoComboboxIcons()
		{
			Create(PrefabsMenu.Instance.AutoComboboxIcons);
		}

		/// <summary>
		/// Create AutoComboboxString.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/AutoComboboxString", false, 1004)]
		public static void CreateAutoComboboxString()
		{
			Create(PrefabsMenu.Instance.AutoComboboxString);
		}

		/// <summary>
		/// Create Combobox.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/Combobox", false, 1005)]
		public static void CreateCombobox()
		{
			Create(PrefabsMenu.Instance.Combobox);
		}

		/// <summary>
		/// Create ComboboxEnum.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/ComboboxEnum", false, 1007)]
		public static void CreateComboboxEnum()
		{
			Create(PrefabsMenu.Instance.ComboboxEnum);
		}

		/// <summary>
		/// Create ComboboxEnumMultiselect.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/ComboboxEnumMultiselect", false, 1008)]
		public static void CreateComboboxEnumMultiselect()
		{
			Create(PrefabsMenu.Instance.ComboboxEnumMultiselect);
		}

		/// <summary>
		/// Create ComboboxIcons.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/ComboboxIcons", false, 1010)]
		public static void CreateComboboxIcons()
		{
			Create(PrefabsMenu.Instance.ComboboxIcons);
		}

		/// <summary>
		/// Create ComboboxIconsMultiselect.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/ComboboxIconsMultiselect", false, 1020)]
		public static void CreateComboboxIconsMultiselect()
		{
			Create(PrefabsMenu.Instance.ComboboxIconsMultiselect);
		}

		/// <summary>
		/// Create ComboboxInputField.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/ComboboxInputField", false, 1005)]
		public static void CreateComboboxInputField()
		{
			Create(PrefabsMenu.Instance.ComboboxInputField);
		}

		/// <summary>
		/// Create DirectoryTreeView.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/DirectoryTreeView", false, 1030)]
		public static void CreateDirectoryTreeView()
		{
			Create(PrefabsMenu.Instance.DirectoryTreeView);
		}

		/// <summary>
		/// Create FileListView.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/FileListView", false, 1040)]
		public static void CreateFileListView()
		{
			Create(PrefabsMenu.Instance.FileListView);
		}

		/// <summary>
		/// Create ListView.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/ListView", false, 1050)]
		public static void CreateListView()
		{
			Create(PrefabsMenu.Instance.ListView);
		}

		/// <summary>
		/// Create istViewColors.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/ListViewColors", false, 1055)]
		public static void CreateListViewColors()
		{
			Create(PrefabsMenu.Instance.ListViewColors);
		}

		/// <summary>
		/// Create ListViewEnum.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/ListViewEnum", false, 1057)]
		public static void CreateListViewEnum()
		{
			Create(PrefabsMenu.Instance.ListViewEnum);
		}

		/// <summary>
		/// Create ListViewInt.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/ListViewInt", false, 1060)]
		public static void CreateListViewInt()
		{
			Create(PrefabsMenu.Instance.ListViewInt);
		}

		/// <summary>
		/// Create ListViewHeight.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/ListViewHeight", false, 1070)]
		public static void CreateListViewHeight()
		{
			Create(PrefabsMenu.Instance.ListViewHeight);
		}

		/// <summary>
		/// Create ListViewIcons.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/ListViewIcons", false, 1090)]
		public static void CreateListViewIcons()
		{
			Create(PrefabsMenu.Instance.ListViewIcons);
		}

		/// <summary>
		/// Create ListViewPaginator.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/ListViewPaginator", false, 1100)]
		public static void CreateListViewPaginator()
		{
			Create(PrefabsMenu.Instance.ListViewPaginator);
		}

		/// <summary>
		/// Create TreeView.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/TreeView", false, 1110)]
		public static void CreateTreeView()
		{
			Create(PrefabsMenu.Instance.TreeView);
		}
		#endregion

		#region Containers

		/// <summary>
		/// Create Accordion.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Containers/Accordion", false, 2000)]
		public static void CreateAccordion()
		{
			Create(PrefabsMenu.Instance.Accordion);
		}

		/// <summary>
		/// Create Tabs.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Containers/Tabs", false, 2010)]
		public static void CreateTabs()
		{
			Create(PrefabsMenu.Instance.Tabs);
		}

		/// <summary>
		/// Create TabsLeft.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Containers/TabsLeft", false, 2020)]
		public static void CreateTabsLeft()
		{
			Create(PrefabsMenu.Instance.TabsLeft);
		}

		/// <summary>
		/// Create TabsIcons.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Containers/TabsIcons", false, 2030)]
		public static void CreateTabsIcons()
		{
			Create(PrefabsMenu.Instance.TabsIcons);
		}

		/// <summary>
		/// Create TabsIconsLeft.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Containers/TabsIconsLeft", false, 2040)]
		public static void CreateTabsIconsLeft()
		{
			Create(PrefabsMenu.Instance.TabsIconsLeft);
		}
		#endregion

		#region Controls

		/// <summary>
		/// Create ButtonBig.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Controls/ButtonBig", false, 2500)]
		public static void CreateButtonBig()
		{
			Create(PrefabsMenu.Instance.ButtonBig);
		}

		/// <summary>
		/// Create ButtonSmall.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Controls/ButtonSmall", false, 2510)]
		public static void CreateButtonSmall()
		{
			Create(PrefabsMenu.Instance.ButtonSmall);
		}

		/// <summary>
		/// Create ContextMenu template.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Controls/ContextMenu Template", false, 2520)]
		public static void CreateContextMenu()
		{
			Create(PrefabsMenu.Instance.ContextMenuTemplate);
		}

		/// <summary>
		/// Create ScrollRectPaginator.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Controls/ScrollRectPaginator", false, 2530)]
		public static void CreateScrollRectPaginator()
		{
			Create(PrefabsMenu.Instance.ScrollRectPaginator);
		}

		/// <summary>
		/// Create ScrollRectNumericPaginator.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Controls/ScrollRectNumericPaginator", false, 2540)]
		public static void CreateScrollRectNumericPaginator()
		{
			Create(PrefabsMenu.Instance.ScrollRectNumericPaginator);
		}

		/// <summary>
		/// Create Sidebar.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Controls/Sidebar", false, 2550)]
		public static void CreateSidebar()
		{
			Create(PrefabsMenu.Instance.Sidebar);
		}

		/// <summary>
		/// Create SplitButton.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Controls/SplitButton", false, 2560)]
		public static void CreateSplitButton()
		{
			Create(PrefabsMenu.Instance.SplitButton);
		}

		#endregion

		#region Dialogs

		/// <summary>
		/// Create DatePicker.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Dialogs/DatePicker", false, 3000)]
		public static void CreateDatePicker()
		{
			Create(PrefabsMenu.Instance.DatePicker);
		}

		/// <summary>
		/// Create DateTimePicker.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Dialogs/DateTimePicker", false, 3005)]
		public static void CreateDateTimePicker()
		{
			Create(PrefabsMenu.Instance.DateTimePicker);
		}

		/// <summary>
		/// Create Dialog.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Dialogs/Dialog Template", false, 3010)]
		public static void CreateDialog()
		{
			Create(PrefabsMenu.Instance.DialogTemplate);
		}

		/// <summary>
		/// Create FileDialog.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Dialogs/FileDialog", false, 3020)]
		public static void CreateFileDialog()
		{
			Create(PrefabsMenu.Instance.FileDialog);
		}

		/// <summary>
		/// Create FolderDialog.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Dialogs/FolderDialog", false, 3030)]
		public static void CreateFolderDialog()
		{
			Create(PrefabsMenu.Instance.FolderDialog);
		}

		/// <summary>
		/// Create Notify.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Dialogs/Notify Template", false, 3040)]
		public static void CreateNotify()
		{
			Create(PrefabsMenu.Instance.NotifyTemplate);
		}

		/// <summary>
		/// Create PickerBool.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Dialogs/PickerBool", false, 3050)]
		public static void CreatePickerBool()
		{
			Create(PrefabsMenu.Instance.PickerBool);
		}

		/// <summary>
		/// Create PickerIcons.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Dialogs/PickerIcons", false, 3060)]
		public static void CreatePickerIcons()
		{
			Create(PrefabsMenu.Instance.PickerIcons);
		}

		/// <summary>
		/// Create PickerInt.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Dialogs/PickerInt", false, 3070)]
		public static void CreatePickerInt()
		{
			Create(PrefabsMenu.Instance.PickerInt);
		}

		/// <summary>
		/// Create PickerString.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Dialogs/PickerString", false, 3080)]
		public static void CreatePickerString()
		{
			Create(PrefabsMenu.Instance.PickerString);
		}

		/// <summary>
		/// Create Popup.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Dialogs/Popup", false, 3090)]
		public static void CreatePopup()
		{
			Create(PrefabsMenu.Instance.Popup);
		}

		/// <summary>
		/// Create TimePicker.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Dialogs/TimePicker", false, 3100)]
		public static void CreateTimePicker()
		{
			Create(PrefabsMenu.Instance.TimePicker);
		}
		#endregion

		#region Input

		/// <summary>
		/// Create Autocomplete.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/Autocomplete", false, 3980)]
		public static void CreateAutocomplete()
		{
			Create(PrefabsMenu.Instance.Autocomplete);
		}

		/// <summary>
		/// Create AutocompleteIcons.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/AutocompleteIcons", false, 3990)]
		public static void CreateAutocompleteIcons()
		{
			Create(PrefabsMenu.Instance.AutocompleteIcons);
		}

		/// <summary>
		/// Create Calendar.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/Calendar", false, 4020)]
		public static void CreateCalendar()
		{
			Create(PrefabsMenu.Instance.Calendar);
		}

		/// <summary>
		/// Create CenteredSlider.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/CenteredSlider", false, 4030)]
		public static void CreateCenteredSlider()
		{
			Create(PrefabsMenu.Instance.CenteredSlider);
		}

		/// <summary>
		/// Create CenteredSliderVertical.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/CenteredSliderVertical", false, 4040)]
		public static void CreateCenteredSliderVertical()
		{
			Create(PrefabsMenu.Instance.CenteredSliderVertical);
		}

		/// <summary>
		/// Create CircularSlider.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/CircularSlider", false, 4043)]
		public static void CircularSlider()
		{
			Create(PrefabsMenu.Instance.CircularSlider);
		}

		/// <summary>
		/// Create CircularSliderFloat.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/CircularSliderFloat", false, 4046)]
		public static void CircularSliderFloat()
		{
			Create(PrefabsMenu.Instance.CircularSliderFloat);
		}

		/// <summary>
		/// Create ColorPicker.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/ColorPicker", false, 4050)]
		public static void CreateColorPicker()
		{
			Create(PrefabsMenu.Instance.ColorPicker);
		}

		/// <summary>
		/// Create ColorPickerRange.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/ColorPickerRange", false, 4060)]
		public static void CreateColorPickerRange()
		{
			Create(PrefabsMenu.Instance.ColorPickerRange);
		}

		/// <summary>
		/// Create ColorPickerRangeHSV.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/ColorPickerRangeHSV", false, 4063)]
		public static void CreateColorPickerRangeHSV()
		{
			Create(PrefabsMenu.Instance.ColorPickerRangeHSV);
		}

		/// <summary>
		/// Create ColorsList.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/ColorsList", false, 4065)]
		public static void CreateColorsList()
		{
			Create(PrefabsMenu.Instance.ColorsList);
		}

		/// <summary>
		/// Create DateTime.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/DateTime", false, 4067)]
		public static void CreateDateTime()
		{
			Create(PrefabsMenu.Instance.DateTime);
		}

		/// <summary>
		/// Create DateScroller.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/DateScroller", false, 4068)]
		public static void CreateDateScroller()
		{
			Create(PrefabsMenu.Instance.DateScroller);
		}

		/// <summary>
		/// Create DateTimeScroller.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/DateTimeScroller", false, 4069)]
		public static void CreateDateTimeScroller()
		{
			Create(PrefabsMenu.Instance.DateTimeScroller);
		}

		/// <summary>
		/// Create DateTimeScrollerSeparate.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/DateTimeScrollerSeparate", false, 4070)]
		public static void CreateDateTimeScrollerSeparate()
		{
			Create(PrefabsMenu.Instance.DateTimeScrollerSeparate);
		}

		/// <summary>
		/// Create RangeSlider.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/RangeSlider", false, 4071)]
		public static void CreateRangeSlider()
		{
			Create(PrefabsMenu.Instance.RangeSlider);
		}

		/// <summary>
		/// Create RangeSliderFloat.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/RangeSliderFloat", false, 4080)]
		public static void CreateRangeSliderFloat()
		{
			Create(PrefabsMenu.Instance.RangeSliderFloat);
		}

		/// <summary>
		/// Create RangeSliderVertical.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/RangeSliderVertical", false, 4090)]
		public static void CreateRangeSliderVertical()
		{
			Create(PrefabsMenu.Instance.RangeSliderVertical);
		}

		/// <summary>
		/// Create RangeSliderFloatVertical.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/RangeSliderFloatVertical", false, 4100)]
		public static void CreateRangeSliderFloatVertical()
		{
			Create(PrefabsMenu.Instance.RangeSliderFloatVertical);
		}

		/// <summary>
		/// Create ScaleHorizontal.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/ScaleHorizontal", false, 4104)]
		public static void CreateScaleHorizontal()
		{
			Create(PrefabsMenu.Instance.ScaleHorizontal);
		}

		/// <summary>
		/// Create ScaleVertical.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/ScaleVertical", false, 4107)]
		public static void CreateScaleVertical()
		{
			Create(PrefabsMenu.Instance.ScaleVertical);
		}

		/// <summary>
		/// Create Spinner.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/Spinner", false, 4110)]
		public static void CreateSpinner()
		{
			Create(PrefabsMenu.Instance.Spinner);
		}

		/// <summary>
		/// Create SpinnerFloat.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/SpinnerFloat", false, 4120)]
		public static void CreateSpinnerFloat()
		{
			Create(PrefabsMenu.Instance.SpinnerFloat);
		}

		/// <summary>
		/// Create Switch.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/Switch", false, 4130)]
		public static void CreateSwitch()
		{
			Create(PrefabsMenu.Instance.Switch);
		}

		/// <summary>
		/// Create Time12.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/Time12", false, 4140)]
		public static void CreateTime12()
		{
			Create(PrefabsMenu.Instance.Time12);
		}

		/// <summary>
		/// Create Time24.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/Time24", false, 4150)]
		public static void CreateTime24()
		{
			Create(PrefabsMenu.Instance.Time24);
		}

		/// <summary>
		/// Create TimeAnalog.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/TimeAnalog", false, 4155)]
		public static void CreateTimeAnalog()
		{
			Create(PrefabsMenu.Instance.TimeAnalog);
		}

		/// <summary>
		/// Create TimeScroller.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/TimeScroller", false, 4160)]
		public static void CreateTimeScroller()
		{
			Create(PrefabsMenu.Instance.TimeScroller);
		}

		#endregion

		/// <summary>
		/// Create AudioPlayer.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Miscellaneous/AudioPlayer", false, 5000)]
		public static void CreateAudioPlayer()
		{
			Create(PrefabsMenu.Instance.AudioPlayer);
		}

		/// <summary>
		/// Create Progressbar.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Miscellaneous/(obsolete) Progressbar", false, 5010)]
		public static void CreateProgressbar()
		{
			Create(PrefabsMenu.Instance.Progressbar);
		}

		/// <summary>
		/// Create ProgressbarDeterminate.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Miscellaneous/ProgressbarDeterminate", false, 5014)]
		public static void CreateProgressbarDeterminate()
		{
			Create(PrefabsMenu.Instance.ProgressbarDeterminate);
		}

		/// <summary>
		/// Create ProgressbarIndeterminate.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Miscellaneous/ProgressbarIndeterminate", false, 5017)]
		public static void CreateProgressbarIndeterminate()
		{
			Create(PrefabsMenu.Instance.ProgressbarIndeterminate);
		}

		/// <summary>
		/// Create Tooltip.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Miscellaneous/Simple Tooltip", false, 5020)]
		public static void CreateSimpleTooltip()
		{
			Create(PrefabsMenu.Instance.Tooltip);
		}

		/// <summary>
		/// Create TooltipString.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Miscellaneous/TooltipString", false, 5030)]
		public static void CreateTooltipString()
		{
			Create(PrefabsMenu.Instance.TooltipString);
		}
	}
}
#endif