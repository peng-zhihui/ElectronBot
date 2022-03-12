namespace UIWidgets.Styles
{
	using System;
	using System.Collections.Generic;
#if UNITY_EDITOR
	using UnityEditor;
#endif
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Style.
	/// </summary>
	public class Style : ScriptableObject, IStyleDefaultValues, IUpgradeable
	{
		/// <summary>
		/// Is default style?
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with PrefabsMenu.Instance.DefaultStyle.")]
		protected bool Default;

		/// <summary>
		/// Style for the collections.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("Common")]
		[FormerlySerializedAs("Short")]
		public StyleFast Fast;

		/// <summary>
		/// Style for the collections.
		/// </summary>
		[Header("Collections")]
		[Tooltip("ListView's, TiveView's and TreeView's style")]
		[SerializeField]
		public StyleCollections Collections;

		/// <summary>
		/// Style for the TreeView.
		/// </summary>
		[SerializeField]
		public StyleTreeView TreeView;

		/// <summary>
		/// Style for the combobox.
		/// </summary>
		[SerializeField]
		public StyleCombobox Combobox;

		/// <summary>
		/// Style for the Table.
		/// </summary>
		[SerializeField]
		public StyleTable Table;

		/// <summary>
		/// Style for the FileListView.
		/// </summary>
		[SerializeField]
		public StyleFileListView FileListView;

		/// <summary>
		/// Style for the IO errors for collections.
		/// </summary>
		[SerializeField]
		public StyleText IOCollectionsErrors;

		/// <summary>
		/// Style for the DropIndicator.
		/// </summary>
		[SerializeField]
		public StyleDropIndicator DropIndicator;

		/// <summary>
		/// Style for the Connector.
		/// </summary>
		[SerializeField]
		public StyleConnector Connector;

		/// <summary>
		/// Style for the accordion.
		/// </summary>
		[Header("Containers")]
		[SerializeField]
		public StyleAccordion Accordion;

		/// <summary>
		/// Style for the tabs on top.
		/// </summary>
		[SerializeField]
		public StyleTabs TabsTop;

		/// <summary>
		/// Style for the tabs on left.
		/// </summary>
		[SerializeField]
		public StyleTabs TabsLeft;

		/// <summary>
		/// Style for the Dialog.
		/// </summary>
		[Header("Dialogs")]
		[SerializeField]
		public StyleDialog Dialog;

		/// <summary>
		/// Style for the Notify.
		/// </summary>
		[SerializeField]
		public StyleNotify Notify;

		/// <summary>
		/// Style for the Autocomplete.
		/// </summary>
		[Header("Input")]
		[SerializeField]
		public StyleAutocomplete Autocomplete;

		/// <summary>
		/// Style for the big button.
		/// </summary>
		[SerializeField]
		public StyleButton ButtonBig;

		/// <summary>
		/// Style for the small button.
		/// </summary>
		[SerializeField]
		public StyleButton ButtonSmall;

		/// <summary>
		/// Style for the calendar.
		/// </summary>
		[SerializeField]
		public StyleCalendar Calendar;

		/// <summary>
		/// Style for the ScrollBlock.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("Scroller")]
		public StyleScroller ScrollBlock;

		/// <summary>
		/// Style for the horizontal centered slider.
		/// </summary>
		[SerializeField]
		public StyleCenteredSlider CenteredSliderHorizontal;

		/// <summary>
		/// Style for the vertical centered slider.
		/// </summary>
		[SerializeField]
		public StyleCenteredSlider CenteredSliderVertical;

		/// <summary>
		/// Style for the circular slider.
		/// </summary>
		[SerializeField]
		public StyleCircularSlider CircularSlider;

		/// <summary>
		/// Style for the ColorPicker.
		/// </summary>
		[SerializeField]
		public StyleColorPicker ColorPicker;

		/// <summary>
		/// Style for the horizontal color picker range.
		/// </summary>
		[SerializeField]
		public StyleColorPickerRange ColorPickerRangeHorizontal;

		/// <summary>
		/// Style for the vertical color picker range.
		/// </summary>
		[SerializeField]
		public StyleColorPickerRange ColorPickerRangeVertical;

		/// <summary>
		/// Style for the context menu.
		/// </summary>
		[SerializeField]
		public StyleContextMenu ContextMenu;

		/// <summary>
		/// Style for the horizontal range slider.
		/// </summary>
		[SerializeField]
		public StyleRangeSlider RangeSliderHorizontal;

		/// <summary>
		/// Style for the vertical range slider.
		/// </summary>
		[SerializeField]
		public StyleRangeSlider RangeSliderVertical;

		/// <summary>
		/// Style for the scale.
		/// </summary>
		[SerializeField]
		public StyleScale Scale;

		/// <summary>
		/// Style for the spinner.
		/// </summary>
		[SerializeField]
		public StyleSpinner Spinner;

		/// <summary>
		/// Style for the switch.
		/// </summary>
		[SerializeField]
		public StyleSwitch Switch;

		/// <summary>
		/// Style for the time widget.
		/// </summary>
		[SerializeField]
		public StyleTime Time;

		/// <summary>
		/// Style for the audio player.
		/// </summary>
		[Header("Misc")]
		[SerializeField]
		public StyleAudioPlayer AudioPlayer;

		/// <summary>
		/// Style for the determinate progress bar.
		/// </summary>
		[SerializeField]
		public StyleProgressbarDeterminate ProgressbarDeterminate;

		/// <summary>
		/// Style for the indeterminate progress bar.
		/// </summary>
		[SerializeField]
		public StyleProgressbarIndeterminate ProgressbarIndeterminate;

		/// <summary>
		/// Style for the paginator.
		/// </summary>
		[SerializeField]
		public StyleTooltip Tooltip;

		/// <summary>
		/// Style for the paginator.
		/// </summary>
		[SerializeField]
		public StylePaginator Paginator;

		/// <summary>
		/// Style for the close button.
		/// </summary>
		[SerializeField]
		public StyleButtonClose ButtonClose;

		/// <summary>
		/// Style for the Sidebar.
		/// </summary>
		[SerializeField]
		public StyleSidebar Sidebar;

		/// <summary>
		/// Style for the canvas.
		/// </summary>
		[Header("Default Unity Widgets")]
		[SerializeField]
		public StyleCanvas Canvas;

		/// <summary>
		/// Style for the text.
		/// </summary>
		[SerializeField]
		public StyleText Text;

		/// <summary>
		/// Style for the horizontal scrollbar.
		/// </summary>
		[SerializeField]
		public StyleScrollbar HorizontalScrollbar;

		/// <summary>
		/// Style for the vertical scrollbar.
		/// </summary>
		[SerializeField]
		public StyleScrollbar VerticalScrollbar;

		/// <summary>
		/// Style for the input field.
		/// </summary>
		[SerializeField]
		public StyleInputField InputField;

		/// <summary>
		/// Style for the button.
		/// </summary>
		[SerializeField]
		public StyleUnityButton Button;

		/// <summary>
		/// Style for the slider.
		/// </summary>
		[SerializeField]
		public StyleSlider Slider;

		/// <summary>
		/// Style for the toggle.
		/// </summary>
		[SerializeField]
		public StyleToggle Toggle;

		/// <summary>
		/// Style for the Dropdown.
		/// </summary>
		[SerializeField]
		public StyleDropdown Dropdown;

		static bool NoTMProSupport(StyleText style, GameObject go)
		{
			return false;
		}

		/// <summary>
		/// The function to process TMPro gameobject.
		/// </summary>
		public static readonly Func<StyleText, GameObject, bool> TMProSupport =
			#if UIWIDGETS_TMPRO_SUPPORT
			UIWidgets.TMProSupport.StyleTMPro.ApplyTo;
			#else
			NoTMProSupport;
			#endif

		/// <summary>
		/// The function to process TMPro gameobject.
		/// </summary>
		public static readonly Func<StyleText, GameObject, bool> TMProSupportGetFrom =
			#if UIWIDGETS_TMPRO_SUPPORT
			UIWidgets.TMProSupport.StyleTMPro.GetFrom;
			#else
			NoTMProSupport;
			#endif

		#region ApplyTo

		/// <summary>
		/// Apply style for the specified component.
		/// </summary>
		/// <returns><c>true</c>, if style was applied for children gameobjects, <c>false</c> otherwise.</returns>
		/// <param name="component">Component.</param>
		protected virtual bool ApplyTo(Component component)
		{
			var applied_for_children = false;
			Text.ApplyTo(component as Text);
			applied_for_children |= ApplyTo(component as Scrollbar);
			applied_for_children |= ApplyTo(component as InputField);
			applied_for_children |= ApplyTo(component as Button);
			applied_for_children |= ApplyTo(component as Slider);
			applied_for_children |= ApplyTo(component as Toggle);
			ApplyTo(component as Canvas);
#if UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
			applied_for_children |= ApplyTo(component as Dropdown);
#endif

			return applied_for_children;
		}

#if UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
		/// <summary>
		/// Apply style for the Dropdown.
		/// </summary>
		/// <returns><c>true</c>, if style was applied for children gameobjects, <c>false</c> otherwise.</returns>
		/// <param name="component">Component.</param>
		protected virtual bool ApplyTo(Dropdown component)
		{
			if (component == null)
			{
				return false;
			}

			Dropdown.ApplyTo(component, this);

			return true;
		}
#endif

		/// <summary>
		/// Apply style for the InputField.
		/// </summary>
		/// <returns><c>true</c>, if style was applied for children gameobjects, <c>false</c> otherwise.</returns>
		/// <param name="component">Component.</param>
		protected virtual bool ApplyTo(Toggle component)
		{
			if (component == null)
			{
				return false;
			}

			Toggle.ApplyTo(component);

			return true;
		}

		/// <summary>
		/// Apply style for the Canvas.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void ApplyTo(Canvas component)
		{
			if (component != null)
			{
				Canvas.ApplyTo(component);
			}
		}

		/// <summary>
		/// Apply style for the Slider.
		/// </summary>
		/// <returns><c>true</c>, if style was applied for children gameobjects, <c>false</c> otherwise.</returns>
		/// <param name="component">Component.</param>
		protected virtual bool ApplyTo(Slider component)
		{
			if (component == null)
			{
				return false;
			}

			Slider.ApplyTo(component);

			return true;
		}

		/// <summary>
		/// Apply style for the InputField.
		/// </summary>
		/// <returns><c>true</c>, if style was applied for children gameobjects, <c>false</c> otherwise.</returns>
		/// <param name="component">Component.</param>
		protected virtual bool ApplyTo(InputField component)
		{
			if (component == null)
			{
				return false;
			}

			InputField.ApplyTo(component);

			return true;
		}

		/// <summary>
		/// Apply style for the button.
		/// </summary>
		/// <returns><c>true</c>, if style was applied for children gameobjects, <c>false</c> otherwise.</returns>
		/// <param name="component">Component.</param>
		protected virtual bool ApplyTo(Button component)
		{
			if (component == null)
			{
				return false;
			}

			Button.ApplyTo(component);

			return true;
		}

		/// <summary>
		/// Apply style for the scrollbar.
		/// </summary>
		/// <returns><c>true</c>, if style was applied for children gameobjects, <c>false</c> otherwise.</returns>
		/// <param name="component">Component.</param>
		protected virtual bool ApplyTo(Scrollbar component)
		{
			if (component == null)
			{
				return false;
			}

			var scrollbar_style = UtilitiesUI.IsHorizontal(component) ? HorizontalScrollbar : VerticalScrollbar;
			scrollbar_style.ApplyTo(component);

			return true;
		}

		/// <summary>
		/// Apply style for the gameobject with specified transform.
		/// </summary>
		/// <param name="target">Transform.</param>
		/// <param name="stylableOnly">Is style should be applied only for objects with IStylable component?</param>
		public virtual void ApplyTo(Transform target, bool stylableOnly = false)
		{
			if (target == null)
			{
				return;
			}

			ApplyTo(target.gameObject, stylableOnly);
		}

		/// <summary>
		/// Apply style for the specified gameobject.
		/// </summary>
		/// <param name="target">Gameobject.</param>
		/// <param name="stylableOnly">Is style should be applied only for objects with IStylable component?</param>
		public virtual void ApplyTo(GameObject target, bool stylableOnly = false)
		{
			Upgrade();

			var stylable = new List<IStylable>();
			Compatibility.GetComponents(target, stylable);

			var children_processed = false;

			if (!stylableOnly && (stylable.Count == 0))
			{
				var components = new List<Component>();
				Compatibility.GetComponents(target, components);
				foreach (var component in components)
				{
					children_processed |= ApplyTo(component);
				}

				if (!children_processed)
				{
					children_processed |= TMProSupport(Text, target);
				}
			}

			foreach (var component in stylable)
			{
				children_processed |= component.SetStyle(this);
			}

			if (!children_processed)
			{
				var t = target.transform;
				for (int i = 0; i < t.childCount; i++)
				{
					ApplyTo(t.GetChild(i).gameObject, stylableOnly);
				}
			}
		}

		/// <summary>
		/// Apply style for children gameobject.
		/// </summary>
		/// <param name="parent">Parent.</param>
		/// <param name="stylableOnly">Is style should be applied only for objects with IStylable component?</param>
		public virtual void ApplyForChildren(GameObject parent, bool stylableOnly = false)
		{
			var t = parent.transform;
			for (int i = 0; i < t.childCount; i++)
			{
				ApplyTo(t.GetChild(i).gameObject, stylableOnly);
			}
		}

		/// <summary>
		/// Apply style to specified target.
		/// </summary>
		/// <param name="tagret">Target.</param>
		/// <param name="style">Style.</param>
		public static void ApplyTo(GameObject tagret, Style style)
		{
			style.ApplyTo(tagret);
		}
		#endregion

		#region GetFrom

		/// <summary>
		/// Set style options from the specified component.
		/// </summary>
		/// <returns><c>true</c>, if children gameobjects was processed, <c>false</c> otherwise.</returns>
		/// <param name="component">Component.</param>
		protected virtual bool GetFrom(Component component)
		{
			var children_processed = false;
			Text.GetFrom(component as Text);
			children_processed |= GetFrom(component as Scrollbar);
			children_processed |= GetFrom(component as InputField);
			children_processed |= GetFrom(component as Button);
			children_processed |= GetFrom(component as Slider);
			children_processed |= GetFrom(component as Toggle);
			GetFrom(component as Canvas);
			#if UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
			children_processed |= GetFrom(component as Dropdown);
			#endif

			return children_processed;
		}

#if UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
		/// <summary>
		/// Set style options from the Dropdown.
		/// </summary>
		/// <returns><c>true</c>, if children gameobjects was processed, <c>false</c> otherwise.</returns>
		/// <param name="component">Component.</param>
		protected virtual bool GetFrom(Dropdown component)
		{
			if (component == null)
			{
				return false;
			}

			Dropdown.GetFrom(component, this);

			return true;
		}
#endif

		/// <summary>
		/// Set style options from the InputField.
		/// </summary>
		/// <returns><c>true</c>, if children gameobjects was processed, <c>false</c> otherwise.</returns>
		/// <param name="component">Component.</param>
		protected virtual bool GetFrom(Toggle component)
		{
			if (component == null)
			{
				return false;
			}

			Toggle.GetFrom(component);

			return true;
		}

		/// <summary>
		/// Set style options from the Canvas.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void GetFrom(Canvas component)
		{
			if (component != null)
			{
				Canvas.GetFrom(component);
			}
		}

		/// <summary>
		/// Set style options from the Slider.
		/// </summary>
		/// <returns><c>true</c>, if children gameobjects was processed, <c>false</c> otherwise.</returns>
		/// <param name="component">Component.</param>
		protected virtual bool GetFrom(Slider component)
		{
			if (component == null)
			{
				return false;
			}

			Slider.GetFrom(component);

			return true;
		}

		/// <summary>
		/// Set style options from the InputField.
		/// </summary>
		/// <returns><c>true</c>, if children gameobjects was processed, <c>false</c> otherwise.</returns>
		/// <param name="component">Component.</param>
		protected virtual bool GetFrom(InputField component)
		{
			if (component == null)
			{
				return false;
			}

			InputField.GetFrom(component);

			return true;
		}

		/// <summary>
		/// Set style options from the button.
		/// </summary>
		/// <returns><c>true</c>, if children gameobjects was processed, <c>false</c> otherwise.</returns>
		/// <param name="component">Component.</param>
		protected virtual bool GetFrom(Button component)
		{
			if (component == null)
			{
				return false;
			}

			Button.GetFrom(component);

			return true;
		}

		/// <summary>
		/// Set style options from the scrollbar.
		/// </summary>
		/// <returns><c>true</c>, if children gameobjects was processed, <c>false</c> otherwise.</returns>
		/// <param name="component">Component.</param>
		protected virtual bool GetFrom(Scrollbar component)
		{
			if (component == null)
			{
				return false;
			}

			var scrollbar_style = UtilitiesUI.IsHorizontal(component) ? HorizontalScrollbar : VerticalScrollbar;
			scrollbar_style.GetFrom(component);

			return true;
		}

		/// <summary>
		/// Set style options from the specified gameobject with specified transform.
		/// </summary>
		/// <param name="target">Transform.</param>
		/// <param name="stylableOnly">Is style should be got only for objects with IStylable component?</param>
		public virtual void GetFrom(Transform target, bool stylableOnly = false)
		{
			if (target == null)
			{
				return;
			}

			GetFrom(target.gameObject, stylableOnly);
		}

		/// <summary>
		/// Set style options from the specified gameobject.
		/// </summary>
		/// <param name="target">Gameobject.</param>
		/// <param name="stylableOnly">Is style should be got only for objects with IStylable component?</param>
		public virtual void GetFrom(GameObject target, bool stylableOnly = false)
		{
			var stylable = new List<IStylable>();
			Compatibility.GetComponents(target, stylable);

			var children_processed = false;

			if (!stylableOnly && (stylable.Count == 0))
			{
				var components = new List<Component>();
				Compatibility.GetComponents(target, components);
				foreach (var component in components)
				{
					children_processed |= GetFrom(component);
				}

				if (!children_processed)
				{
					children_processed |= TMProSupportGetFrom(Text, target);
				}
			}

			foreach (var component in stylable)
			{
				children_processed |= component.GetStyle(this);
			}

			if (!children_processed)
			{
				var t = target.transform;
				for (int i = 0; i < t.childCount; i++)
				{
					GetFrom(t.GetChild(i).gameObject, stylableOnly);
				}
			}
		}

		/// <summary>
		/// Set style options from the children gameobject.
		/// </summary>
		/// <param name="parent">Parent.</param>
		/// <param name="stylableOnly">Is style should be got only for objects with IStylable component?</param>
		public virtual void GetFromChildren(GameObject parent, bool stylableOnly = false)
		{
			var t = parent.transform;
			for (int i = 0; i < t.childCount; i++)
			{
				GetFrom(t.GetChild(i).gameObject, stylableOnly);
			}
		}

		/// <summary>
		/// Set style options from the specified target.
		/// </summary>
		/// <param name="tagret">Target.</param>
		/// <param name="style">Style.</param>
		public static void GetFrom(GameObject tagret, Style style)
		{
			style.GetFrom(tagret);
		}
		#endregion

		/// <summary>
		/// Set target if source is asset.
		/// </summary>
		/// <typeparam name="T">Type of value.</typeparam>
		/// <param name="source">Source.</param>
		/// <param name="target">Target.</param>
		/// <returns>true if source is asset; otherwise false.</returns>
		public static bool SetValue<T>(T source, ref T target)
			where T : UnityEngine.Object
		{
#if UNITY_EDITOR
			if ((source == null) || AssetDatabase.Contains(source))
			{
				target = source;
				return true;
			}
#endif

			return false;
		}

		[SerializeField]
		[HideInInspector]
		int version;

		void UpgradeV1()
		{
			if (DropIndicator == null)
			{
				DropIndicator = new StyleDropIndicator();
				#if UNITY_EDITOR
				DropIndicator.SetDefaultValues();
				#endif
			}

			if (Collections != null)
			{
				DropIndicator.Image.Color = Collections.DefaultColor;
			}
		}

		void UpgradeV2()
		{
			if (Connector == null)
			{
				Connector = new StyleConnector();
				#if UNITY_EDITOR
				Connector.SetDefaultValues();
				#endif
			}

			if (Collections != null)
			{
				Connector.Color = Collections.DefaultColor;
			}
		}

		void UpgradeV3()
		{
			if (ContextMenu == null)
			{
				ContextMenu = new StyleContextMenu();
				#if UNITY_EDITOR
				ContextMenu.SetDefaultValues();
				#endif
			}

			if (Collections != null)
			{
				ContextMenu.MainBackground = Collections.MainBackground.Clone();
				ContextMenu.ItemText = Collections.DefaultItemText.Clone();
				ContextMenu.DelimiterImage.Color = Collections.DefaultColor;

				ContextMenu.ItemBackgroundSelectable.Colors.normalColor = Collections.DefaultBackgroundColor;
				ContextMenu.ItemBackgroundSelectable.Colors.highlightedColor = Collections.HighlightedBackgroundColor;
				ContextMenu.ItemBackgroundSelectable.Colors.pressedColor = Collections.SelectedBackgroundColor;
				#if UNITY_2019_1_OR_NEWER
				ContextMenu.ItemBackgroundSelectable.Colors.selectedColor = Collections.SelectedBackgroundColor;
				#endif

				ContextMenu.ItemTextSelectable.Colors.normalColor = Collections.DefaultColor;
				ContextMenu.ItemTextSelectable.Colors.highlightedColor = Collections.HighlightedColor;
				ContextMenu.ItemTextSelectable.Colors.pressedColor = Collections.SelectedColor;
				#if UNITY_2019_1_OR_NEWER
				ContextMenu.ItemTextSelectable.Colors.selectedColor = Collections.SelectedColor;
				#endif
			}
		}

		void UpgradeV4()
		{
			Accordion.ToggleActiveBackground = Paginator.ActiveBackground.Clone();
		}

		void UpgradeV5()
		{
			CircularSlider = new StyleCircularSlider();
			CircularSlider.Ring = new StyleImage();
			CircularSlider.Handle = new StyleImage();
			CircularSlider.Arrow = new StyleImage();
			#if UNITY_EDITOR
			CircularSlider.SetDefaultValues();
			#endif

			CircularSlider.RingColor = Collections.DefaultColor;
			CircularSlider.Arrow.Color = Collections.DefaultColor;
			CircularSlider.Handle = RangeSliderHorizontal.HandleMin.Clone();
		}

		void UpgradeV6()
		{
			Scale = new StyleScale();
			Scale.MainLine = new StyleImage();
			Scale.MarkLine = new StyleImage();
			Scale.MarkLabel = new StyleText();
			#if UNITY_EDITOR
			Scale.SetDefaultValues();
			#endif

			Scale.MainLine.Color = Collections.DefaultColor;
			Scale.MarkLine.Color = Collections.DefaultColor;
			Scale.MarkLabel.Color = Collections.DefaultColor;
			Scale.MarkLabel.Size = 14;
		}

		void UpgradeV7()
		{
			Time.HourLabel = new StyleText();
			#if UNITY_EDITOR
			Time.HourLabel.SetDefaultValues();
			#endif

			Time.HourLabel.ChangeColor = true;
			Time.HourLabel.Color = Collections.DefaultColor;
		}

		void UpgradeV8()
		{
			Accordion.ToggleActiveText = Accordion.ToggleDefaultText.Clone();
			Accordion.ToggleActiveText.Color = Collections.DefaultColor;
		}

		/// <inheritdoc/>
		public virtual void Upgrade()
		{
			if (version == 0)
			{
				UpgradeV1();
				version = 1;
			}

			if (version == 1)
			{
				UpgradeV2();
				version = 2;
			}

			if (version == 2)
			{
				UpgradeV3();
				version = 3;
			}

			if (version == 3)
			{
				UpgradeV4();
				version = 4;
			}

			if (version == 4)
			{
				UpgradeV5();
				version = 5;
			}

			if (version == 5)
			{
				UpgradeV6();
				version = 6;
			}

			if (version == 6)
			{
				UpgradeV7();
				version = 7;
			}

			if (version == 7)
			{
				UpgradeV8();
				version = 8;
			}
		}

#if UNITY_EDITOR

		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected virtual void OnValidate()
		{
			Upgrade();
		}

		/// <inheritdoc/>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Reviewed.")]
		public virtual void SetDefaultValues()
		{
			Fast.SetDefaultValues();

			Collections.SetDefaultValues();
			TreeView.SetDefaultValues();
			Combobox.SetDefaultValues();
			Table.SetDefaultValues();
			FileListView.SetDefaultValues();
			IOCollectionsErrors.SetDefaultValues();
			DropIndicator.SetDefaultValues();
			Connector.SetDefaultValues();

			Accordion.SetDefaultValues();
			TabsTop.SetDefaultValues();
			TabsLeft.SetDefaultValues();

			Dialog.SetDefaultValues();
			Notify.SetDefaultValues();

			Autocomplete.SetDefaultValues();
			ButtonBig.SetDefaultValues();
			ButtonSmall.SetDefaultValues();
			Calendar.SetDefaultValues();
			CenteredSliderHorizontal.SetDefaultValues();
			CenteredSliderVertical.SetDefaultValues();
			ColorPicker.SetDefaultValues();
			ColorPickerRangeHorizontal.SetDefaultValues();
			ColorPickerRangeVertical.SetDefaultValues();
			ContextMenu.SetDefaultValues();
			RangeSliderHorizontal.SetDefaultValues();
			RangeSliderVertical.SetDefaultValues();
			Spinner.SetDefaultValues();
			Switch.SetDefaultValues();
			Time.SetDefaultValues();
			AudioPlayer.SetDefaultValues();

			ProgressbarDeterminate.SetDefaultValues();
			ProgressbarIndeterminate.SetDefaultValues();
			Tooltip.SetDefaultValues();

			Paginator.SetDefaultValues();
			ButtonClose.SetDefaultValues();

			Canvas.SetDefaultValues();
			Text.SetDefaultValues();
			HorizontalScrollbar.SetDefaultValues();
			VerticalScrollbar.SetDefaultValues();
			InputField.SetDefaultValues();
			Button.SetDefaultValues();
			Slider.SetDefaultValues();
			Toggle.SetDefaultValues();
			Dropdown.SetDefaultValues();
		}

		/// <summary>
		/// Is it default style? (Editor only)
		/// </summary>
		/// <returns><c>true</c> if this style is default; otherwise, <c>false</c>.</returns>
		public bool IsDefault()
		{
			return PrefabsMenu.Instance.DefaultStyle == this;
		}

		/// <summary>
		/// Undo this style as default. (Editor only)
		/// </summary>
		public void SetAsNotDefault()
		{
			Undo.RecordObject(this, "Undo Set Style as Default");
			Undo.RecordObject(PrefabsMenu.Instance, "Undo Set Style as Default");

			PrefabsMenu.Instance.DefaultStyle = null;
			EditorUtility.SetDirty(PrefabsMenu.Instance.DefaultStyle);
		}

		/// <summary>
		/// Set this style as default. (Editor only)
		/// </summary>
		public void SetAsDefault()
		{
			Undo.RecordObject(PrefabsMenu.Instance, "Undo Set Style as Default");

			PrefabsMenu.Instance.DefaultStyle = this;
			EditorUtility.SetDirty(PrefabsMenu.Instance.DefaultStyle);
		}

		/// <summary>
		/// Get the default style (Editor only).
		/// </summary>
		/// <returns>The default style.</returns>
		[Obsolete("Replaced with PrefabsMenu.Instance.DefaultStyle.")]
		public static Style DefaultStyle()
		{
			if (PrefabsMenu.Instance.DefaultStyle != null)
			{
				return PrefabsMenu.Instance.DefaultStyle;
			}

			foreach (var style in GetStyles())
			{
#pragma warning disable 0618
				if (style.Default)
#pragma warning restore 0618
				{
					PrefabsMenu.Instance.DefaultStyle = style;
					EditorUtility.SetDirty(PrefabsMenu.Instance);

					return style;
				}
			}

			return null;
		}

		/// <summary>
		/// Get the list of all styles (Editor only).
		/// </summary>
		/// <returns>The styles.</returns>
		public static List<Style> GetStyles()
		{
			return UtilitiesEditor.GetAssets<Style>("t:" + typeof(Style).FullName);
		}
#endif
	}
}