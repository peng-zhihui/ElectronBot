namespace UIWidgets
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Accordion.
	/// </summary>
	public class Accordion : UIBehaviourConditional, IStylable
	{
		/// <summary>
		/// Encapsulates a method that has five parameters and returns a value of the type specified by the TResult parameter.
		/// </summary>
		/// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
		/// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
		/// <typeparam name="T3">The type of the third parameter of the method that this delegate encapsulates.</typeparam>
		/// <typeparam name="T4">The type of the fourth parameter of the method that this delegate encapsulates.</typeparam>
		/// <typeparam name="T5">The type of the fifth parameter of the method that this delegate encapsulates.</typeparam>
		/// <typeparam name="T6">The type of the sixth parameter of the method that this delegate encapsulates.</typeparam>
		/// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
		/// <param name="p1">The first parameter of the method that this delegate encapsulates.</param>
		/// <param name="p2">The second parameter of the method that this delegate encapsulates.</param>
		/// <param name="p3">The third parameter of the method that this delegate encapsulates.</param>
		/// <param name="p4">The fourth parameter of the method that this delegate encapsulates.</param>
		/// <param name="p5">The fifth parameter of the method that this delegate encapsulates.</param>
		/// <param name="p6">The sixth parameter of the method that this delegate encapsulates.</param>
		/// <returns>The return value of the method that this delegate encapsulates.</returns>
		public delegate TResult AnimationFunc<T1, T2, T3, T4, T5, T6, TResult>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6);

		/// <summary>
		/// Items.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("Items")]
		protected List<AccordionItem> items = new List<AccordionItem>();

		/// <summary>
		/// Items.
		/// </summary>
		protected ObservableList<AccordionItem> dataSource;

		/// <summary>
		/// Accordion items.
		/// </summary>
		[Obsolete("Use DataSource instead.")]
		public List<AccordionItem> Items
		{
			get
			{
				return new List<AccordionItem>(DataSource);
			}

			set
			{
				DataSource = new ObservableList<AccordionItem>(value);
			}
		}

		/// <summary>
		/// Accordion items.
		/// </summary>
		public virtual ObservableList<AccordionItem> DataSource
		{
			get
			{
				if (dataSource == null)
				{
					#pragma warning disable 0618
					dataSource = new ObservableList<AccordionItem>(items);
					dataSource.OnChange += UpdateItems;
					#pragma warning restore 0618
				}

				return dataSource;
			}

			set
			{
				if (dataSource != null)
				{
					dataSource.OnChange -= UpdateItems;
				}

				dataSource = value;
				if (dataSource != null)
				{
					dataSource.OnChange += UpdateItems;
				}

				UpdateItems();
			}
		}

		/// <summary>
		/// Only one item can be opened.
		/// </summary>
		[SerializeField]
		public bool OnlyOneOpen = true;

		/// <summary>
		/// All items can be closed.
		/// </summary>
		[SerializeField]
		public bool AllItemsCanBeClosed = false;

		/// <summary>
		/// Animate open and close.
		/// </summary>
		[SerializeField]
		public bool Animate = true;

		/// <summary>
		/// The duration of the animation.
		/// </summary>
		[SerializeField]
		[EditorConditionBool("Animate")]
		public float AnimationDuration = 0.5f;

		/// <summary>
		/// Animation curve.
		/// </summary>
		[SerializeField]
		[EditorConditionBool("Animate")]
		public AnimationCurve Curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		/// <summary>
		/// Use unscaled time.
		/// </summary>
		[SerializeField]
		[EditorConditionBool("Animate")]
		public bool UnscaledTime = true;

		/// <summary>
		/// The direction.
		/// </summary>
		[SerializeField]
		[EditorConditionBool("Animate")]
		public AccordionDirection Direction = AccordionDirection.Vertical;

		/// <summary>
		/// The item resize method.
		/// </summary>
		[SerializeField]
		[EditorConditionBool("Animate")]
		public ResizeMethods ResizeMethod = ResizeMethods.Size;

		/// <summary>
		/// Disable closed items.
		/// </summary>
		[SerializeField]
		public bool DisableClosed = true;

		/// <summary>
		/// Toggle item event.
		/// </summary>
		[SerializeField]
		public AccordionEvent OnToggleItem = new AccordionEvent();

		/// <summary>
		/// Start toggle animation event.
		/// </summary>
		[SerializeField]
		public AccordionEvent OnStartToggleAnimation = new AccordionEvent();

		/// <summary>
		/// DataSource changed event.
		/// </summary>
		[SerializeField]
		public UnityEvent OnDataSourceChanged = new UnityEvent();

		/// <summary>
		/// Open animation.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		public AnimationFunc<RectTransform, float, AnimationCurve, bool, bool, Action, IEnumerator> AnimationOpen = Animations.Open;

		/// <summary>
		/// Open animation with resize method = flexible.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		public AnimationFunc<RectTransform, float, AnimationCurve, bool, bool, Action, IEnumerator> AnimationOpenFlexible = Animations.OpenFlexible;

		/// <summary>
		/// Close animation.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		public AnimationFunc<RectTransform, float, AnimationCurve, bool, bool, Action, IEnumerator> AnimationClose = Animations.Collapse;

		/// <summary>
		/// Close animation with resize method = flexible.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		public AnimationFunc<RectTransform, float, AnimationCurve, bool, bool, Action, IEnumerator> AnimationCloseFlexible = Animations.CollapseFlexible;

		/// <summary>
		/// The components.
		/// </summary>
		protected List<AccordionItemComponent> Components = new List<AccordionItemComponent>();

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected override void Start()
		{
			base.Start();
			UpdateItems();
		}

		/// <summary>
		/// Updates the items.
		/// </summary>
		protected virtual void UpdateItems()
		{
			RemoveCallbacks();
			AddCallbacks();

			UpdateLayout();

			OnDataSourceChanged.Invoke();
		}

		/// <summary>
		/// Adds the callback.
		/// </summary>
		/// <param name="item">Item.</param>
		protected virtual void AddCallback(AccordionItem item)
		{
			item.ContentObject.SetActive(true);

			var component = Utilities.GetOrAddComponent<AccordionItemComponent>(item.ToggleObject);
			component.Item = item;
			component.OnItemClick.AddListener(ToggleItem);

			item.ToggleLabel = item.ToggleObject.GetComponentInChildren<TextAdapter>();
			item.ContentObjectRect = item.ContentObject.transform as RectTransform;
			item.ContentLayoutElement = Utilities.GetOrAddComponent<LayoutElement>(item.ContentObject);
			if (IsHorizontal())
			{
				item.ContentLayoutElement.minWidth = 0f;
			}
			else
			{
				item.ContentLayoutElement.minHeight = 0f;
			}

			UpdateItemSize(item);

			if (item.Open)
			{
				Open(item, false);
			}
			else
			{
				Close(item, false);
			}

			Components.Add(component);
		}

		/// <summary>
		/// Adds the callbacks.
		/// </summary>
		protected virtual void AddCallbacks()
		{
			foreach (var item in DataSource)
			{
				AddCallback(item);
			}
		}

		/// <summary>
		/// Removes the callback.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void RemoveCallback(AccordionItemComponent component)
		{
			if (component == null)
			{
				return;
			}

			component.Item = null;
			component.OnItemClick.RemoveListener(ToggleItem);
		}

		/// <summary>
		/// Removes the callbacks.
		/// </summary>
		protected virtual void RemoveCallbacks()
		{
			foreach (var c in Components)
			{
				RemoveCallback(c);
			}

			Components.Clear();
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected override void OnDestroy()
		{
			if (dataSource != null)
			{
				dataSource.OnChange -= UpdateItems;
				dataSource = null;
			}

			RemoveCallbacks();

			base.OnDestroy();
		}

		/// <summary>
		/// Toggles the item.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void ToggleItem(AccordionItem item)
		{
			if (item.Open)
			{
				if (!OnlyOneOpen || AllItemsCanBeClosed)
				{
					Close(item, Animate);
				}
			}
			else
			{
				if (OnlyOneOpen)
				{
					CloseAll(Animate);
				}

				Open(item, Animate);
			}
		}

		/// <summary>
		/// Open the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void Open(AccordionItem item)
		{
			if (item.Open)
			{
				return;
			}

			if (OnlyOneOpen)
			{
				CloseAll(Animate);
			}

			Open(item, Animate);
		}

		/// <summary>
		/// Close the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void Close(AccordionItem item)
		{
			if (!item.Open)
			{
				return;
			}

			Close(item, Animate);
		}

		/// <summary>
		/// Open the specified item without animation.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void ForceOpen(AccordionItem item)
		{
			if (item.Open)
			{
				return;
			}

			if (OnlyOneOpen)
			{
				CloseAll(false);
			}

			Open(item, false);
		}

		/// <summary>
		/// Close the specified item without animation.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void ForceClose(AccordionItem item)
		{
			if (!item.Open)
			{
				return;
			}

			Close(item, false);
		}

		/// <summary>
		/// Determines whether this instance is open the specified item.
		/// </summary>
		/// <returns><c>true</c> if this instance is open the specified item; otherwise, <c>false</c>.</returns>
		/// <param name="item">Item.</param>
		protected static bool IsOpen(AccordionItem item)
		{
			return item.Open;
		}

		/// <summary>
		/// Determines whether this instance is horizontal.
		/// </summary>
		/// <returns><c>true</c> if this instance is horizontal; otherwise, <c>false</c>.</returns>
		protected bool IsHorizontal()
		{
			return Direction == AccordionDirection.Horizontal;
		}

		/// <summary>
		/// Open the specified item and animate.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="animate">If set to <c>true</c> animate.</param>
		protected virtual void Open(AccordionItem item, bool animate)
		{
			StopAnimation(item, true);

			if (animate)
			{
				item.CurrentCoroutine = StartCoroutine(OpenCoroutine(item));
			}
			else
			{
				item.ContentObject.SetActive(true);
				item.Open = true;
				OnToggleItem.Invoke(item);
			}
		}

		/// <summary>
		/// Stop running animation.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="isOpen">Is item will be opened or closed?</param>
		protected virtual void StopAnimation(AccordionItem item, bool isOpen)
		{
			if (item.CurrentCoroutine == null)
			{
				return;
			}

			StopCoroutine(item.CurrentCoroutine);

			item.ContentObject.SetActive(true);

			if (IsHorizontal())
			{
				item.ContentObjectRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, item.ContentObjectWidth);

				if (ResizeMethod == ResizeMethods.Size)
				{
					item.ContentLayoutElement.preferredWidth = -1f;
				}
				else if (ResizeMethod == ResizeMethods.Flexible)
				{
					item.ContentLayoutElement.flexibleWidth = isOpen ? 1f : 0f;
				}
			}
			else
			{
				item.ContentObjectRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, item.ContentObjectHeight);

				if (ResizeMethod == ResizeMethods.Size)
				{
					item.ContentLayoutElement.preferredHeight = -1f;
				}
				else if (ResizeMethod == ResizeMethods.Flexible)
				{
					item.ContentLayoutElement.flexibleHeight = isOpen ? 1f : 0f;
				}
			}
		}

		/// <summary>
		/// Close the specified item and animate.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="animate">If set to <c>true</c> animate.</param>
		protected virtual void Close(AccordionItem item, bool animate)
		{
			StopAnimation(item, false);

			if (animate)
			{
				item.CurrentCoroutine = StartCoroutine(HideCoroutine(item));
			}
			else
			{
				item.ContentObject.SetActive(false);
				item.Open = false;
				OnToggleItem.Invoke(item);
			}
		}

		/// <summary>
		/// Open coroutine.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>The coroutine.</returns>
		protected virtual IEnumerator OpenCoroutine(AccordionItem item)
		{
			if (!DisableClosed)
			{
				var axis = IsHorizontal() ? RectTransform.Axis.Horizontal : RectTransform.Axis.Vertical;
				var size = IsHorizontal() ? item.ContentObjectWidth : item.ContentObjectHeight;
				item.ContentObjectRect.SetSizeWithCurrentAnchors(axis, size);
			}

			item.ContentObject.SetActive(true);

			item.Open = true;
			UpdateItemSize(item);
			OnStartToggleAnimation.Invoke(item);

			if (ResizeMethod == ResizeMethods.Size)
			{
				yield return StartCoroutine(AnimationOpen(item.ContentObjectRect, AnimationDuration, Curve, IsHorizontal(), UnscaledTime, DoNothing));
			}
			else if (ResizeMethod == ResizeMethods.Flexible)
			{
				yield return StartCoroutine(AnimationOpenFlexible(item.ContentObjectRect, AnimationDuration, Curve, IsHorizontal(), UnscaledTime, DoNothing));
			}

			AnimationEnded(item, true);
		}

		/// <summary>
		/// Update item size.
		/// </summary>
		/// <param name="item">Item.</param>
		protected void UpdateItemSize(AccordionItem item)
		{
			UpdateLayout();

			item.ContentObjectWidth = LayoutUtilities.IsWidthControlled(item.ContentObjectRect)
				? LayoutUtility.GetPreferredWidth(item.ContentObjectRect)
				: item.ContentObjectRect.rect.width;

			item.ContentObjectHeight = LayoutUtilities.IsHeightControlled(item.ContentObjectRect)
				? LayoutUtility.GetPreferredHeight(item.ContentObjectRect)
				: item.ContentObjectRect.rect.height;
		}

		/// <summary>
		/// Updates the layout.
		/// </summary>
		protected void UpdateLayout()
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
		}

		/// <summary>
		/// Function called when animation ended.
		/// </summary>
		/// <param name="item">Animated item.</param>
		/// <param name="isOpen">Is open?</param>
		protected virtual void AnimationEnded(AccordionItem item, bool isOpen)
		{
			item.CurrentCoroutine = null;

			if (ResizeMethod == ResizeMethods.Size)
			{
				if (IsHorizontal())
				{
					item.ContentLayoutElement.preferredWidth = -1f;
				}
				else
				{
					item.ContentLayoutElement.preferredHeight = -1f;
				}
			}

			if (!isOpen)
			{
				if (DisableClosed)
				{
					item.ContentObject.SetActive(false);
				}
				else
				{
					var axis = IsHorizontal() ? RectTransform.Axis.Horizontal : RectTransform.Axis.Vertical;
					item.ContentObjectRect.SetSizeWithCurrentAnchors(axis, 0f);
				}
			}

			UpdateLayout();

			OnToggleItem.Invoke(item);
		}

		/// <summary>
		/// Hide coroutine.
		/// </summary>
		/// <returns>The coroutine.</returns>
		/// <param name="item">Item.</param>
		protected virtual IEnumerator HideCoroutine(AccordionItem item)
		{
			UpdateItemSize(item);

			item.Open = false;
			OnStartToggleAnimation.Invoke(item);

			if (ResizeMethod == ResizeMethods.Size)
			{
				yield return StartCoroutine(AnimationClose(item.ContentObjectRect, AnimationDuration, Curve, IsHorizontal(), UnscaledTime, DoNothing));
			}
			else if (ResizeMethod == ResizeMethods.Flexible)
			{
				yield return StartCoroutine(AnimationCloseFlexible(item.ContentObjectRect, AnimationDuration, Curve, IsHorizontal(), UnscaledTime, DoNothing));
			}

			AnimationEnded(item, false);
		}

		/// <summary>
		/// Do nothing.
		/// </summary>
		protected static Action DoNothing = () => { };

		/// <summary>
		/// Close all opened items.
		/// </summary>
		/// <param name="animate">Close with animation.</param>
		public virtual void CloseAll(bool animate = true)
		{
			foreach (var elem in DataSource)
			{
				if (elem.Open)
				{
					Close(elem, animate);
				}
			}
		}

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			if (dataSource != null)
			{
				foreach (var item in dataSource)
				{
					style.ApplyForChildren(item.ContentObject, false);
					style.Accordion.ApplyTo(item);
				}
			}
			else if (items != null)
			{
				foreach (var item in items)
				{
					style.ApplyForChildren(item.ContentObject, false);
					style.Accordion.ApplyTo(item);
				}
			}

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			if (dataSource != null)
			{
				foreach (var item in dataSource)
				{
					style.GetFromChildren(item.ContentObject, false);
					style.Accordion.GetFrom(item);
				}
			}
			else if (items != null)
			{
				foreach (var item in items)
				{
					style.GetFromChildren(item.ContentObject, false);
					style.Accordion.GetFrom(item);
				}
			}

			return true;
		}
		#endregion

		#if UNITY_EDITOR
		/// <summary>
		/// Disable closed items in Editor if DisableClosed enabled.
		/// </summary>
		protected override void OnValidate()
		{
			base.OnValidate();

			if (!DisableClosed)
			{
				return;
			}

			if (items != null)
			{
				foreach (var item in items)
				{
					if ((item.ContentObject != null) && (item.CurrentCoroutine == null))
					{
						item.ContentObject.SetActive(item.Open);
					}
				}
			}
		}
		#endif
	}
}