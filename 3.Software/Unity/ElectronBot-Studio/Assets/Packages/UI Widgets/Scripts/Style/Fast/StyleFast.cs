namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Fast style definition.
	/// </summary>
	[Serializable]
	public class StyleFast : IStyleDefaultValues
	{
		/// <summary>
		/// Primary color.
		/// </summary>
		[Header("Colors")]
		[SerializeField]
		public Color ColorPrimary = new Color(1.000f, 0.843f, 0.451f, 1.000f);

		/// <summary>
		/// Secondary color.
		/// </summary>
		[SerializeField]
		public Color ColorSecondary = new Color(0.000f, 0.000f, 0.000f, 1.000f);

		/// <summary>
		/// Secondary color.
		/// </summary>
		[SerializeField]
		public Color ColorBackground = new Color(0.153f, 0.157f, 0.168f, 1.000f);

		/// <summary>
		/// Highlighted background color.
		/// </summary>
		[SerializeField]
		public Color ColorHighlightedBackground = new Color(0.710f, 0.478f, 0.141f, 1.000f);

		/// <summary>
		/// Selected background color.
		/// </summary>
		[SerializeField]
		public Color ColorSelectedBackground = new Color(0.769f, 0.612f, 0.153f, 1.000f);

		/// <summary>
		/// Disabled color.
		/// </summary>
		[SerializeField]
		public Color ColorDisabled = new Color(0.784f, 0.784f, 0.784f, 1.000f);

		/// <summary>
		/// Calendar weekend color.
		/// </summary>
		[SerializeField]
		public Color ColorCalendarWeekend = new Color(1.000f, 0.000f, 0.000f, 1.000f);

		/// <summary>
		/// Background.
		/// </summary>
		[Header("Common")]
		[SerializeField]
		public StyleImage Background;

		/// <summary>
		/// Highlighted background.
		/// </summary>
		[SerializeField]
		public StyleImage BackgroundHightlighted;

		/// <summary>
		/// Background for collections item.
		/// </summary>
		[SerializeField]
		public StyleImage CollectionsItemBackground;

		/// <summary>
		/// Arrow.
		/// </summary>
		[SerializeField]
		public StyleImage Arrow;

		/// <summary>
		/// The font.
		/// </summary>
		[SerializeField]
		public Font Font;

		/// <summary>
		/// The TMPro font.
		/// </summary>
		[SerializeField]
#if UIWIDGETS_TMPRO_SUPPORT && (UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER)
		public TMPro.TMP_FontAsset FontTMPro;
#elif UIWIDGETS_TMPRO_SUPPORT
		public TMPro.TextMeshProFont FontTMPro;
#else
		public ScriptableObject FontTMPro;
#endif

		/// <summary>
		/// Button.
		/// </summary>
		[Header("Buttons")]
		[SerializeField]
		public StyleFastButton Button;

		/// <summary>
		/// Close button.
		/// </summary>
		[SerializeField]
		public StyleImage ButtonClose;

		/// <summary>
		/// Button pause.
		/// </summary>
		[SerializeField]
		public StyleImage ButtonPause;

		/// <summary>
		/// Button down.
		/// </summary>
		[SerializeField]
		public StyleImage ButtonArrowDown;

		/// <summary>
		/// Button up.
		/// </summary>
		[SerializeField]
		public StyleImage ButtonArrowUp;

		/// <summary>
		/// Horizontal scrollbar.
		/// </summary>
		[Header("Scrollbars")]
		[SerializeField]
		public StyleFastScrollbar ScrollbarHorizontal;

		/// <summary>
		/// Vertical scrollbar.
		/// </summary>
		[SerializeField]
		public StyleFastScrollbar ScrollbarVertical;

		/// <summary>
		/// Determinate progress bar.
		/// </summary>
		[Header("Progressbars")]
		[SerializeField]
		public StyleFastButton ProgressbarDeterminate;

		/// <summary>
		/// Indeterminate progress bar.
		/// </summary>
		[SerializeField]
		public StyleFastProgressbarIndeterminate ProgressbarIndeterminate;

		/// <summary>
		/// Tab content background.
		/// </summary>
		[Header("Tabs")]
		[SerializeField]
		public StyleImage TabContentBackground;

		/// <summary>
		/// Inactive top tab.
		/// </summary>
		[SerializeField]
		public StyleFastButton TabTopInactive;

		/// <summary>
		/// Active top tab.
		/// </summary>
		[SerializeField]
		public StyleFastButton TabTopActive;

		/// <summary>
		/// Inactive left tab.
		/// </summary>
		[SerializeField]
		public StyleFastButton TabLeftInactive;

		/// <summary>
		/// Inactive left tab.
		/// </summary>
		[SerializeField]
		public StyleFastButton TabLeftActive;

		/// <summary>
		/// Dialog delimiter.
		/// </summary>
		[Header("Other")]
		[SerializeField]
		public StyleImage DialogDelimiter;

		/// <summary>
		/// Background for tooltip.
		/// </summary>
		[SerializeField]
		public StyleImage TooltipBackground;

		/// <summary>
		/// Check mark.
		/// </summary>
		[SerializeField]
		public StyleImage Checkmark;

		/// <summary>
		/// Change primary style.
		/// </summary>
		/// <param name="style">Style.</param>
		public void ChangeStyle(Style style)
		{
			SetImages(style);
			SetRawImages(style);
			SetColors(style);
			SetSprites(style);
			SetFonts(style);

			SetScrollBlockStyle(style);
		}

		/// <summary>
		/// Set style for the ScrollBlock.
		/// </summary>
		/// <param name="style">Style.</param>
		public void SetScrollBlockStyle(Style style)
		{
			style.ScrollBlock.Background = Background.Clone();
			style.ScrollBlock.Highlight.Color = ColorBackground;
			style.ScrollBlock.Text.Color = ColorPrimary;
			style.ScrollBlock.Text.Font = Font;
			style.ScrollBlock.Text.FontTMPro = FontTMPro;
		}

		/// <summary>
		/// Set style images.
		/// </summary>
		/// <param name="style">Style.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Reviewed")]
		public void SetImages(Style style)
		{
			style.Collections.MainBackground = Background.Clone();
			style.Collections.Viewport = Background.Clone();
			style.Combobox.Background = Background.Clone();
			style.FileListView.PathItemBackground = Background.Clone();
			style.Accordion.ContentBackground = Background.Clone();
			style.Dialog.Background = Background.Clone();
			style.Dialog.Button.Mask = Background.Clone();
			style.ButtonSmall.Mask = Background.Clone();
			style.Calendar.DaysBackground = Background.Clone();
			style.Calendar.DayBackground = Background.Clone();
			style.CenteredSliderHorizontal.Background = Background.Clone();
			style.CenteredSliderVertical.Background = Background.Clone();
			style.ColorPicker.Background = Background.Clone();
			style.ColorPicker.PaletteToggle.Mask = Background.Clone();
			style.ColorPicker.InputToggle.Mask = Background.Clone();
			style.ColorPicker.InputSpinner.InputBackground = Background.Clone();
			style.ColorPicker.HexInputBackground = Background.Clone();
			style.ColorPickerRangeHorizontal.Background = Background.Clone();
			style.ColorPickerRangeVertical.Background = Background.Clone();
			style.RangeSliderHorizontal.Background = Background.Clone();
			style.RangeSliderVertical.Background = Background.Clone();
			style.Spinner.Background = Background.Clone();
			style.Time.InputBackground = Background.Clone();
			style.Time.AMPMBackground = Background.Clone();
			style.AudioPlayer.Progress.Background = Background.Clone();
			style.AudioPlayer.Play.Mask = Background.Clone();
			style.AudioPlayer.Pause.Mask = Background.Clone();
			style.AudioPlayer.Stop.Mask = Background.Clone();
			style.AudioPlayer.Toggle.Mask = Background.Clone();
			style.Button.Background = Background.Clone();
			style.Dropdown.Background = Background.Clone();
			style.Dropdown.OptionsBackground = Background.Clone();

			style.Collections.DefaultItemBackground = CollectionsItemBackground.Clone();

			style.TreeView.Toggle = Arrow.Clone();
			style.AudioPlayer.Play.Image = Arrow.Clone();

			style.Combobox.SingleInputBackground = Background.Clone();
			style.Accordion.ToggleDefaultBackground = Background.Clone();
			style.Autocomplete.Background = Background.Clone();
			style.CenteredSliderHorizontal.Handle = Background.Clone();
			style.CenteredSliderVertical.Handle = Background.Clone();
			style.ColorPicker.SliderVerticalHandle = Background.Clone();
			style.ColorPicker.SliderHorizontalHandle = Background.Clone();
			style.ColorPickerRangeHorizontal.Handle = Background.Clone();
			style.ColorPickerRangeVertical.Handle = Background.Clone();
			style.RangeSliderHorizontal.HandleMin = Background.Clone();
			style.RangeSliderHorizontal.HandleMax = Background.Clone();
			style.RangeSliderVertical.HandleMin = Background.Clone();
			style.RangeSliderVertical.HandleMax = Background.Clone();
			style.Spinner.InputBackground = Background.Clone();
			style.AudioPlayer.Progress.Handle = Background.Clone();
			style.Paginator.DefaultBackground = Background.Clone();
			style.InputField.Background = Background.Clone();
			style.Slider.Handle = Background.Clone();
			style.Toggle.Background = Background.Clone();

			style.Combobox.MultipleDefaultItemBackground = BackgroundHightlighted.Clone();
			style.Combobox.RemoveBackground = BackgroundHightlighted.Clone();
			style.Sidebar.Background = BackgroundHightlighted.Clone();

			style.Combobox.ToggleButton = ButtonArrowDown.Clone();
			style.FileListView.ButtonToggle = ButtonArrowDown.Clone();
			style.Calendar.NextMonth = ButtonArrowDown.Clone();
			style.ColorPicker.InputSpinner.ButtonMinus = ButtonArrowDown.Clone();
			style.Spinner.ButtonMinus = ButtonArrowDown.Clone();
			style.Time.ButtonDecrease = ButtonArrowDown.Clone();
			style.Dropdown.Arrow = ButtonArrowDown.Clone();

			style.FileListView.ButtonUp = ButtonArrowUp.Clone();
			style.Calendar.PrevMonth = ButtonArrowUp.Clone();
			style.ColorPicker.InputSpinner.ButtonPlus = ButtonArrowUp.Clone();
			style.Spinner.ButtonPlus = ButtonArrowUp.Clone();
			style.Time.ButtonIncrease = ButtonArrowUp.Clone();

			style.TabsTop.DefaultButton.Background = TabTopInactive.Background.Clone();
			style.TabsTop.DefaultButton.Mask = TabTopInactive.Mask.Clone();
			style.TabsTop.DefaultButton.Border = TabTopInactive.Border.Clone();
			style.TabsTop.ActiveButton.Background = TabTopActive.Background.Clone();
			style.TabsTop.ActiveButton.Mask = TabTopActive.Mask.Clone();
			style.TabsTop.ActiveButton.Border = TabTopActive.Border.Clone();
			style.TabsTop.ContentBackground = TabContentBackground.Clone();

			style.TabsLeft.ContentBackground = TabContentBackground.Clone();
			style.TabsLeft.DefaultButton.Background = TabLeftInactive.Background.Clone();
			style.TabsLeft.DefaultButton.Mask = TabLeftInactive.Mask.Clone();
			style.TabsLeft.DefaultButton.Border = TabLeftInactive.Border.Clone();
			style.TabsLeft.ActiveButton.Background = TabLeftActive.Background.Clone();
			style.TabsLeft.ActiveButton.Mask = TabLeftActive.Mask.Clone();
			style.TabsLeft.ActiveButton.Border = TabLeftActive.Border.Clone();

			style.Dialog.Delimiter = DialogDelimiter.Clone();

			style.Dialog.Button.Background = Button.Background.Clone();
			style.ButtonBig.Background = Button.Background.Clone();
			style.ColorPicker.PaletteToggle.Background = Button.Background.Clone();
			style.ColorPicker.InputToggle.Background = Button.Background.Clone();
			style.AudioPlayer.Play.Background = Button.Background.Clone();
			style.AudioPlayer.Pause.Background = Button.Background.Clone();
			style.AudioPlayer.Stop.Background = Button.Background.Clone();
			style.AudioPlayer.Toggle.Background = Button.Background.Clone();

			style.Dialog.Button.Border = Button.Border.Clone();
			style.ButtonSmall.Border = Button.Border.Clone();
			style.ColorPicker.PaletteToggle.Border = Button.Border.Clone();
			style.ColorPicker.InputToggle.Border = Button.Border.Clone();
			style.AudioPlayer.Play.Border = Button.Border.Clone();
			style.AudioPlayer.Pause.Border = Button.Border.Clone();
			style.AudioPlayer.Stop.Border = Button.Border.Clone();
			style.AudioPlayer.Toggle.Border = Button.Border.Clone();

			style.Notify.Background = BackgroundHightlighted.Clone();

			style.ButtonBig.Mask = Button.Mask.Clone();

			style.ButtonBig.Border = Button.Border.Clone();

			style.ButtonSmall.Background = Button.Background.Clone();

			style.ColorPicker.PaletteCursor = BackgroundHightlighted.Clone();
			style.Paginator.ActiveBackground = BackgroundHightlighted.Clone();

			style.AudioPlayer.Pause.Image = ButtonPause.Clone();

			style.ProgressbarDeterminate.FullbarImage = ProgressbarDeterminate.Background.Clone();
			style.ProgressbarDeterminate.FullbarMask = ProgressbarDeterminate.Mask.Clone();
			style.ProgressbarDeterminate.FullbarBorder = ProgressbarDeterminate.Border.Clone();
			style.ProgressbarDeterminate.EmptyBar = Background.Clone();
			style.ProgressbarDeterminate.Background = Background.Clone();

			style.ProgressbarIndeterminate.Mask = ProgressbarIndeterminate.Mask.Clone();
			style.ProgressbarIndeterminate.Border = ProgressbarIndeterminate.Border.Clone();

			style.Tooltip.Background = TooltipBackground.Clone();

			style.ButtonClose.Background = ButtonClose.Clone();

			style.HorizontalScrollbar.Background = ScrollbarHorizontal.Background.Clone();
			style.HorizontalScrollbar.Handle = ScrollbarHorizontal.Handle.Clone();

			style.VerticalScrollbar.Background = ScrollbarVertical.Background.Clone();
			style.VerticalScrollbar.Handle = ScrollbarVertical.Handle.Clone();

			style.Toggle.Checkmark = Checkmark.Clone();
			style.Dropdown.ItemCheckmark = Checkmark.Clone();
		}

		/// <summary>
		/// Set style raw images.
		/// </summary>
		/// <param name="style">Style.</param>
		public void SetRawImages(Style style)
		{
			style.ProgressbarIndeterminate.Texture = ProgressbarIndeterminate.Background.Clone();
		}

		/// <summary>
		/// Set style colors.
		/// </summary>
		/// <param name="style">Style.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Reviewed")]
		public void SetColors(Style style)
		{
			style.Fast.ColorPrimary = ColorPrimary;
			style.Fast.ColorSecondary = ColorSecondary;
			style.Fast.ColorHighlightedBackground = ColorHighlightedBackground;
			style.Fast.ColorSelectedBackground = ColorSelectedBackground;
			style.Fast.ColorDisabled = ColorDisabled;
			style.Fast.ColorCalendarWeekend = ColorCalendarWeekend;

			style.Collections.HighlightedColor = ColorSecondary;
			style.Collections.SelectedColor = ColorSecondary;
			style.TabsTop.DefaultButton.Text.Color = ColorSecondary;
			style.TabsTop.ActiveButton.Text.Color = ColorSecondary;
			style.TabsLeft.DefaultButton.Text.Color = ColorSecondary;
			style.TabsLeft.ActiveButton.Text.Color = ColorSecondary;
			style.Dialog.Button.Text.Color = ColorSecondary;
			style.Notify.Text.Color = ColorSecondary;
			style.ButtonBig.Text.Color = ColorSecondary;
			style.ButtonSmall.Text.Color = ColorSecondary;
			style.CenteredSliderHorizontal.UsableRange.Color = ColorSecondary;
			style.CenteredSliderVertical.UsableRange.Color = ColorSecondary;
			style.ColorPicker.PaletteToggle.Text.Color = ColorSecondary;
			style.ColorPicker.InputToggle.Text.Color = ColorSecondary;
			style.RangeSliderHorizontal.UsableRange.Color = ColorSecondary;
			style.RangeSliderVertical.UsableRange.Color = ColorSecondary;
			style.Switch.Border.Color = ColorSecondary;
			style.Switch.BackgroundDefault.Color = ColorSecondary;
			style.Switch.BackgroundOnColor = ColorSecondary;
			style.Switch.BackgroundOffColor = ColorSecondary;
			style.Slider.Background.Color = ColorSecondary;

			style.Calendar.ColorWeekend = ColorCalendarWeekend;

			style.Collections.DefaultColor = ColorPrimary;
			style.Collections.DefaultItemText.Color = ColorPrimary;
			style.Combobox.SingleDefaultItemText.Color = ColorPrimary;
			style.Combobox.MultipleDefaultItemText.Color = ColorPrimary;
			style.Combobox.Input.Color = ColorPrimary;
			style.Combobox.Placeholder.Color = ColorPrimary;
			style.Combobox.RemoveText.Color = ColorPrimary;
			style.Table.Border.Color = ColorPrimary;
			style.FileListView.PathItemText.Color = ColorPrimary;
			style.IOCollectionsErrors.Color = ColorPrimary;
			style.Dialog.Title.Color = ColorPrimary;
			style.Dialog.ContentText.Color = ColorPrimary;
			style.Calendar.CurrentDate.Color = ColorPrimary;
			style.Calendar.CurrentMonth.Color = ColorPrimary;
			style.Calendar.DayOfWeekText.Color = ColorPrimary;
			style.Calendar.DayText.Color = ColorPrimary;
			style.Calendar.ColorSelectedDay = ColorPrimary;
			style.Calendar.ColorCurrentMonth = ColorPrimary;
			style.CenteredSliderHorizontal.Fill.Color = ColorPrimary;
			style.CenteredSliderVertical.Fill.Color = ColorPrimary;
			style.ColorPicker.InputSpinner.InputText.Color = ColorPrimary;
			style.ColorPicker.InputSpinner.InputPlaceholder.Color = ColorPrimary;
			style.ColorPicker.HexInputText.Color = ColorPrimary;
			style.ColorPicker.HexInputPlaceholder.Color = ColorPrimary;
			style.RangeSliderHorizontal.Fill.Color = ColorPrimary;
			style.RangeSliderVertical.Fill.Color = ColorPrimary;
			style.Spinner.InputText.Color = ColorPrimary;
			style.Switch.MarkDefault.Color = ColorPrimary;
			style.Switch.MarkOnColor = ColorPrimary;
			style.Time.AMPMText.Color = ColorPrimary;
			style.AudioPlayer.Progress.Fill.Color = ColorPrimary;
			style.ProgressbarDeterminate.FullBarText.Color = ColorPrimary;
			style.Tooltip.Text.Color = ColorPrimary;
			style.ButtonClose.Text.Color = ColorPrimary;
			style.Text.Color = ColorSecondary;
			style.Button.Text.Color = ColorPrimary;
			style.Slider.Fill.Color = ColorPrimary;
			style.Toggle.Label.Color = ColorPrimary;

			style.Collections.HighlightedBackgroundColor = ColorHighlightedBackground;

			style.Collections.SelectedBackgroundColor = ColorSelectedBackground;

			style.Collections.DisabledColor = ColorDisabled;

			style.Table.Background.Color = ColorBackground;
			style.Canvas.Background.Color = ColorBackground;

			style.Table.HeaderText.Color = ColorSecondary;
			style.Accordion.ToggleDefaultText.Color = ColorSecondary;
			style.Accordion.ContentText.Color = ColorSecondary;
			style.Autocomplete.InputField.Color = ColorSecondary;
			style.Autocomplete.Placeholder.Color = ColorSecondary;
			style.ColorPicker.InputChannelLabel.Color = ColorSecondary;
			style.Time.InputText.Color = ColorSecondary;
			style.AudioPlayer.Play.Text.Color = ColorSecondary;
			style.AudioPlayer.Pause.Text.Color = ColorSecondary;
			style.AudioPlayer.Stop.Text.Color = ColorSecondary;
			style.AudioPlayer.Toggle.Text.Color = ColorSecondary;
			style.ProgressbarDeterminate.EmptyBarText.Color = ColorSecondary;
			style.Paginator.DefaultText.Color = ColorSecondary;
			style.Paginator.ActiveText.Color = ColorSecondary;
			style.InputField.Text.Color = ColorSecondary;
			style.Dropdown.Label.Color = ColorSecondary;
			style.Dropdown.ItemLabel.Color = ColorSecondary;

			style.Calendar.ColorOtherMonth = ColorDisabled;

			style.Spinner.InputPlaceholder.Color = ColorDisabled;

			style.Switch.MarkOffColor = ColorDisabled;
			style.InputField.Placeholder.Color = ColorDisabled;
		}

		/// <summary>
		/// Set style sprites.
		/// </summary>
		/// <param name="style">Style.</param>
		public void SetSprites(Style style)
		{
			style.Calendar.SelectedDayBackground = BackgroundHightlighted.Sprite;
		}

		/// <summary>
		/// Set style fonts.
		/// </summary>
		/// <param name="style">Style.</param>
		public void SetFonts(Style style)
		{
			style.Collections.DefaultItemText.Font = Font;
			style.Collections.DefaultItemText.FontTMPro = FontTMPro;
			style.Combobox.SingleDefaultItemText.Font = Font;
			style.Combobox.SingleDefaultItemText.FontTMPro = FontTMPro;
			style.Combobox.MultipleDefaultItemText.Font = Font;
			style.Combobox.MultipleDefaultItemText.FontTMPro = FontTMPro;
			style.Combobox.Input.Font = Font;
			style.Combobox.Input.FontTMPro = FontTMPro;
			style.Combobox.Placeholder.Font = Font;
			style.Combobox.Placeholder.FontTMPro = FontTMPro;
			style.Combobox.RemoveText.Font = Font;
			style.Combobox.RemoveText.FontTMPro = FontTMPro;
			style.Table.HeaderText.Font = Font;
			style.Table.HeaderText.FontTMPro = FontTMPro;
			style.FileListView.PathItemText.Font = Font;
			style.FileListView.PathItemText.FontTMPro = FontTMPro;
			style.IOCollectionsErrors.Font = Font;
			style.IOCollectionsErrors.FontTMPro = FontTMPro;
			style.Accordion.ToggleDefaultText.Font = Font;
			style.Accordion.ToggleDefaultText.FontTMPro = FontTMPro;
			style.Accordion.ContentText.Font = Font;
			style.Accordion.ContentText.FontTMPro = FontTMPro;
			style.TabsTop.DefaultButton.Text.Font = Font;
			style.TabsTop.DefaultButton.Text.FontTMPro = FontTMPro;
			style.TabsTop.ActiveButton.Text.Font = Font;
			style.TabsTop.ActiveButton.Text.FontTMPro = FontTMPro;
			style.TabsLeft.DefaultButton.Text.Font = Font;
			style.TabsLeft.DefaultButton.Text.FontTMPro = FontTMPro;
			style.TabsLeft.ActiveButton.Text.Font = Font;
			style.TabsLeft.ActiveButton.Text.FontTMPro = FontTMPro;
			style.Dialog.Title.Font = Font;
			style.Dialog.Title.FontTMPro = FontTMPro;
			style.Dialog.ContentText.Font = Font;
			style.Dialog.ContentText.FontTMPro = FontTMPro;
			style.Dialog.Button.Text.Font = Font;
			style.Dialog.Button.Text.FontTMPro = FontTMPro;
			style.Notify.Text.Font = Font;
			style.Notify.Text.FontTMPro = FontTMPro;
			style.Autocomplete.InputField.Font = Font;
			style.Autocomplete.InputField.FontTMPro = FontTMPro;
			style.Autocomplete.Placeholder.Font = Font;
			style.Autocomplete.Placeholder.FontTMPro = FontTMPro;
			style.ButtonBig.Text.Font = Font;
			style.ButtonBig.Text.FontTMPro = FontTMPro;
			style.ButtonSmall.Text.Font = Font;
			style.ButtonSmall.Text.FontTMPro = FontTMPro;
			style.Calendar.CurrentDate.Font = Font;
			style.Calendar.CurrentDate.FontTMPro = FontTMPro;
			style.Calendar.CurrentMonth.Font = Font;
			style.Calendar.CurrentMonth.FontTMPro = FontTMPro;
			style.Calendar.DayOfWeekText.Font = Font;
			style.Calendar.DayOfWeekText.FontTMPro = FontTMPro;
			style.Calendar.DayText.Font = Font;
			style.Calendar.DayText.FontTMPro = FontTMPro;
			style.ColorPicker.PaletteToggle.Text.Font = Font;
			style.ColorPicker.PaletteToggle.Text.FontTMPro = FontTMPro;
			style.ColorPicker.InputToggle.Text.Font = Font;
			style.ColorPicker.InputToggle.Text.FontTMPro = FontTMPro;
			style.ColorPicker.InputChannelLabel.Font = Font;
			style.ColorPicker.InputChannelLabel.FontTMPro = FontTMPro;
			style.ColorPicker.InputSpinner.InputText.Font = Font;
			style.ColorPicker.InputSpinner.InputText.FontTMPro = FontTMPro;
			style.ColorPicker.InputSpinner.InputPlaceholder.Font = Font;
			style.ColorPicker.InputSpinner.InputPlaceholder.FontTMPro = FontTMPro;
			style.ColorPicker.HexInputText.Font = Font;
			style.ColorPicker.HexInputText.FontTMPro = FontTMPro;
			style.ColorPicker.HexInputPlaceholder.Font = Font;
			style.ColorPicker.HexInputPlaceholder.FontTMPro = FontTMPro;
			style.Spinner.InputText.Font = Font;
			style.Spinner.InputText.FontTMPro = FontTMPro;
			style.Spinner.InputPlaceholder.Font = Font;
			style.Spinner.InputPlaceholder.FontTMPro = FontTMPro;
			style.Time.InputText.Font = Font;
			style.Time.InputText.FontTMPro = FontTMPro;
			style.Time.AMPMText.Font = Font;
			style.Time.AMPMText.FontTMPro = FontTMPro;
			style.AudioPlayer.Play.Text.Font = Font;
			style.AudioPlayer.Play.Text.FontTMPro = FontTMPro;
			style.AudioPlayer.Pause.Text.Font = Font;
			style.AudioPlayer.Pause.Text.FontTMPro = FontTMPro;
			style.AudioPlayer.Stop.Text.Font = Font;
			style.AudioPlayer.Stop.Text.FontTMPro = FontTMPro;
			style.AudioPlayer.Toggle.Text.Font = Font;
			style.AudioPlayer.Toggle.Text.FontTMPro = FontTMPro;
			style.ProgressbarDeterminate.EmptyBarText.Font = Font;
			style.ProgressbarDeterminate.EmptyBarText.FontTMPro = FontTMPro;
			style.ProgressbarDeterminate.FullBarText.Font = Font;
			style.ProgressbarDeterminate.FullBarText.FontTMPro = FontTMPro;
			style.Tooltip.Text.Font = Font;
			style.Tooltip.Text.FontTMPro = FontTMPro;
			style.Paginator.DefaultText.Font = Font;
			style.Paginator.DefaultText.FontTMPro = FontTMPro;
			style.Paginator.ActiveText.Font = Font;
			style.Paginator.ActiveText.FontTMPro = FontTMPro;
			style.ButtonClose.Text.Font = Font;
			style.ButtonClose.Text.FontTMPro = FontTMPro;
			style.Text.Font = Font;
			style.Text.FontTMPro = FontTMPro;
			style.InputField.Text.Font = Font;
			style.InputField.Text.FontTMPro = FontTMPro;
			style.InputField.Placeholder.Font = Font;
			style.InputField.Placeholder.FontTMPro = FontTMPro;
			style.Button.Text.Font = Font;
			style.Button.Text.FontTMPro = FontTMPro;
			style.Toggle.Label.Font = Font;
			style.Toggle.Label.FontTMPro = FontTMPro;
			style.Dropdown.Label.Font = Font;
			style.Dropdown.Label.FontTMPro = FontTMPro;
			style.Dropdown.ItemLabel.Font = Font;
			style.Dropdown.ItemLabel.FontTMPro = FontTMPro;
		}

#if UNITY_EDITOR
		/// <inheritdoc/>
		public void SetDefaultValues()
		{
			Background.SetDefaultValues();
			BackgroundHightlighted.SetDefaultValues();

			CollectionsItemBackground.SetDefaultValues();
			Arrow.SetDefaultValues();

			Button.SetDefaultValues();
			ButtonClose.SetDefaultValues();
			ButtonPause.SetDefaultValues();
			ButtonArrowDown.SetDefaultValues();
			ButtonArrowUp.SetDefaultValues();

			ScrollbarHorizontal.SetDefaultValues();
			ScrollbarVertical.SetDefaultValues();

			ProgressbarDeterminate.SetDefaultValues();
			ProgressbarIndeterminate.SetDefaultValues();

			TabContentBackground.SetDefaultValues();
			TabTopInactive.SetDefaultValues();
			TabTopActive.SetDefaultValues();
			TabLeftInactive.SetDefaultValues();
			TabLeftActive.SetDefaultValues();

			DialogDelimiter.SetDefaultValues();
			TooltipBackground.SetDefaultValues();
			Checkmark.SetDefaultValues();

			if (Font == null)
			{
				Font = Resources.GetBuiltinResource<Font>("Arial.ttf");
			}

			if (FontTMPro == null)
			{
#if UIWIDGETS_TMPRO_SUPPORT && (UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER)
				FontTMPro = Resources.Load<TMPro.TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
#elif UIWIDGETS_TMPRO_SUPPORT
				FontTMPro = Resources.Load<TMPro.TextMeshProFont>("Fonts & Materials/ARIAL SDF");
#else
				FontTMPro = Resources.Load<ScriptableObject>("Fonts & Materials/ARIAL SDF");
#endif
			}
		}
#endif
	}
}