namespace UIWidgets
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using EasyLayoutNS;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Base class for notifications.
	/// </summary>
	public abstract class NotificationBase : MonoBehaviour, ITemplatable, IHideable
	{
		[SerializeField]
		bool unscaledTime;

		/// <summary>
		/// Use unscaled time.
		/// </summary>
		public bool UnscaledTime
		{
			get
			{
				return unscaledTime;
			}

			protected set
			{
				unscaledTime = value;
			}
		}

		bool isTemplate = true;

		/// <summary>
		/// Gets a value indicating whether this instance is template.
		/// </summary>
		/// <value><c>true</c> if this instance is template; otherwise, <c>false</c>.</value>
		public bool IsTemplate
		{
			get
			{
				return isTemplate;
			}

			set
			{
				isTemplate = value;
			}
		}

		/// <summary>
		/// Template name.
		/// </summary>
		protected string NotificationTemplateName;

		/// <summary>
		/// Gets the name of the template.
		/// </summary>
		/// <value>The name of the template.</value>
		public virtual string TemplateName
		{
			get
			{
				return NotificationTemplateName;
			}

			set
			{
				NotificationTemplateName = value;
			}
		}

		/// <summary>
		/// Time between previous notification was hidden and next will be showed.
		/// </summary>
		public float SequenceDelay;

		/// <summary>
		/// The notification manager.
		/// </summary>
		static NotifySequenceManager notificationManager;

		/// <summary>
		/// Gets the notification manager.
		/// </summary>
		/// <value>The notification manager.</value>
		public static NotifySequenceManager NotifyManager
		{
			get
			{
				if (notificationManager == null)
				{
					var go = new GameObject("NotificationSequenceManager");
					notificationManager = go.AddComponent<NotifySequenceManager>();
				}

				return notificationManager;
			}
		}

		/// <summary>
		/// Return this instance to cache.
		/// </summary>
		public abstract void Return();

		/// <summary>
		/// Display notification.
		/// </summary>
		/// <param name="onHideCallback">On hide callback.</param>
		public abstract void Display(Action onHideCallback = null);

		/// <summary>
		/// Replacements cache.
		/// </summary>
		protected static readonly Stack<RectTransform> Replacements = new Stack<RectTransform>();

		/// <summary>
		/// Get replacement for this instance.
		/// </summary>
		/// <returns>Replacement.</returns>
		protected static RectTransform GetReplacement()
		{
			RectTransform rect;

			if (Replacements.Count == 0)
			{
				var obj = new GameObject("NotificationReplacement");
				obj.SetActive(false);
				rect = obj.AddComponent<RectTransform>();

				// change size don't work without graphic component
				var image = obj.AddComponent<Image>();
				image.color = Color.clear;
			}
			else
			{
				do
				{
					rect = (Replacements.Count > 0) ? Replacements.Pop() : GetReplacement();
				}
				while (rect == null);
			}

			return rect;
		}

		/// <summary>
		/// Get notification replacement.
		/// </summary>
		/// <param name="notification">Notification instance.</param>
		/// <returns>Replacement.</returns>
		protected static RectTransform GetReplacement(NotificationBase notification)
		{
			var target = GetReplacement();
			var source_rect = notification.transform as RectTransform;

			target.localRotation = source_rect.localRotation;
			target.localPosition = source_rect.localPosition;
			target.localScale = source_rect.localScale;
			target.anchorMin = source_rect.anchorMin;
			target.anchorMax = source_rect.anchorMax;
			target.anchoredPosition = source_rect.anchoredPosition;
			target.sizeDelta = source_rect.sizeDelta;
			target.pivot = source_rect.pivot;

			target.transform.SetParent(notification.transform.parent, false);
			target.transform.SetSiblingIndex(notification.transform.GetSiblingIndex());

			target.gameObject.SetActive(true);

			return target;
		}

		/// <summary>
		/// Clear notifications sequence.
		/// </summary>
		public static void ClearSequence()
		{
			NotifyManager.Clear();
		}

		/// <summary>
		/// Set container (parent gameobject).
		/// </summary>
		/// <param name="container">Container.</param>
		public virtual void SetContainer(Transform container)
		{
			if (container != null)
			{
				transform.SetParent(container, false);
			}
		}

		/// <summary>
		/// Returns replacement slide to cache.
		/// </summary>
		/// <param name="replacement">Replacement.</param>
		public static void FreeSlide(RectTransform replacement)
		{
			Replacements.Push(replacement);
		}

		/// <summary>
		/// Hide notification.
		/// </summary>
		public abstract void Hide();

		#region HideAnimationRotate

#if CSHARP_7_3_OR_NEWER
		/// <summary>
		/// Vertical rotate animation to hide notification.
		/// </summary>
		/// <returns>Animation coroutine.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Cached")]
		[Obsolete("Renamed to AnimationRotateVertical.")]
		public static readonly Func<NotificationBase, IEnumerator> AnimationRotate = AnimationRotateMethod;

		static IEnumerator AnimationRotateMethod(NotificationBase notification)
#else
		/// <summary>
		/// Vertical rotate animation to hide notification.
		/// </summary>
		/// <param name="notification">Notification.</param>
		/// <returns>Animation coroutine.</returns>
		[Obsolete("Renamed to AnimationRotateVertical.")]
		public static IEnumerator AnimationRotate(NotificationBase notification)
#endif
		{
			return AnimationRotateVertical(notification);
		}

#if CSHARP_7_3_OR_NEWER
		/// <summary>
		/// Vertical rotate animation to hide notification.
		/// </summary>
		/// <returns>Animation coroutine.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Cached")]
		public static readonly Func<NotificationBase, IEnumerator> AnimationRotateVertical = AnimationRotateVerticalMethod;

		static IEnumerator AnimationRotateVerticalMethod(NotificationBase notification)
#else
		/// <summary>
		/// Vertical rotate animation to hide notification.
		/// </summary>
		/// <param name="notification">Notification.</param>
		/// <returns>Animation coroutine.</returns>
		public static IEnumerator AnimationRotateVertical(NotificationBase notification)
#endif
		{
			return HideAnimationRotateBase(notification, false, 0.5f);
		}

#if CSHARP_7_3_OR_NEWER
		/// <summary>
		/// Horizontal rotate animation to hide notification.
		/// </summary>
		/// <returns>Animation coroutine.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Cached")]
		public static readonly Func<NotificationBase, IEnumerator> AnimationRotateHorizontal = AnimationRotateHorizontalMethod;

		static IEnumerator AnimationRotateHorizontalMethod(NotificationBase notification)
#else
		/// <summary>
		/// Horizontal rotate animation to hide notification.
		/// </summary>
		/// <param name="notification">Notification.</param>
		/// <returns>Animation coroutine.</returns>
		public static IEnumerator AnimationRotateHorizontal(NotificationBase notification)
#endif
		{
			return HideAnimationRotateBase(notification, true, 0.5f);
		}

#if CSHARP_7_3_OR_NEWER
		/// <summary>
		/// Base rotate animation to hide notification.
		/// </summary>
		/// <returns>Animation coroutine.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Cached")]
		[Obsolete("Renamed to HideAnimationRotateBase.")]
		public static readonly Func<NotificationBase, bool, float, IEnumerator> AnimationRotateBase = AnimationRotateBaseMethod;

		static IEnumerator AnimationRotateBaseMethod(NotificationBase notification, bool isHorizontal, float timeLength)
#else
		/// <summary>
		/// Base rotate animation to hide notification.
		/// </summary>
		/// <param name="notification">Notification.</param>
		/// <param name="isHorizontal">Is horizontal></param>
		/// <param name="timeLength">Animation length in seconds.</param>
		/// <returns>Animation coroutine.</returns>
		[Obsolete("Renamed to HideAnimationRotateBase.")]
		public static IEnumerator AnimationRotateBase(NotificationBase notification, bool isHorizontal, float timeLength)
#endif
		{
			return HideAnimationRotateBase(notification, isHorizontal, timeLength);
		}

#if CSHARP_7_3_OR_NEWER
		/// <summary>
		/// Base rotate animation to hide notification.
		/// </summary>
		/// <returns>Animation coroutine.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Cached")]
		public static readonly Func<NotificationBase, bool, float, IEnumerator> HideAnimationRotateBase = HideAnimationRotateBaseMethod;

		static IEnumerator HideAnimationRotateBaseMethod(NotificationBase notification, bool isHorizontal, float timeLength)
#else
		/// <summary>
		/// Base rotate animation to hide notification.
		/// </summary>
		/// <param name="notification">Notification.</param>
		/// <param name="isHorizontal">Is horizontal></param>
		/// <param name="timeLength">Animation length in seconds.</param>
		/// <returns>Animation coroutine.</returns>
		public static IEnumerator HideAnimationRotateBase(NotificationBase notification, bool isHorizontal, float timeLength)
#endif
		{
			var rect = notification.transform as RectTransform;
			var base_rotation = rect.localRotation.eulerAngles;

			var end_time = UtilitiesTime.GetTime(notification.UnscaledTime) + timeLength;

			while (UtilitiesTime.GetTime(notification.UnscaledTime) <= end_time)
			{
				var t = 1f - ((end_time - UtilitiesTime.GetTime(notification.UnscaledTime)) / timeLength);
				var rotation = Mathf.Lerp(0f, 90f, t);

				rect.localRotation = isHorizontal
					? Quaternion.Euler(base_rotation.x, rotation, base_rotation.z)
					: Quaternion.Euler(rotation, base_rotation.y, base_rotation.z);
				yield return null;
			}

			// return rotation back for future use
			rect.localRotation = Quaternion.Euler(base_rotation);
		}

#if CSHARP_7_3_OR_NEWER
		/// <summary>
		/// Rotate animation to hide notification.
		/// </summary>
		/// <returns>Animation coroutine.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Cached")]
		[Obsolete("AnimationRotate() now supports UnscaledTime.")]
		public static readonly Func<NotificationBase, IEnumerator> AnimationRotateUnscaledTime = AnimationRotateUnscaledTimeMethod;

		static IEnumerator AnimationRotateUnscaledTimeMethod(NotificationBase notification)
#else
		/// <summary>
		/// Rotate animation to hide notification.
		/// </summary>
		/// <param name="notification">Notification.</param>
		/// <returns>Animation coroutine.</returns>
		[Obsolete("AnimationRotate() now supports UnscaledTime.")]
		public static IEnumerator AnimationRotateUnscaledTime(NotificationBase notification)
#endif
		{
			return AnimationRotateVertical(notification);
		}

		#endregion

		#region HideAnimationCollapse

		/// <summary>
		/// Base collapse animation.
		/// </summary>
		/// <returns>Animation coroutine.</returns>
		[Obsolete("Renamed to HideAnimationCollapseBase.")]
#if CSHARP_7_3_OR_NEWER
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Cached")]
		public static readonly Func<NotificationBase, bool, float, IEnumerator> AnimationCollapseBase = AnimationCollapseBaseMethod;

		static IEnumerator AnimationCollapseBaseMethod(NotificationBase notification, bool isHorizontal, float speed)
#else
		/// <summary>
		/// Base collapse animation.
		/// </summary>
		/// <param name="notification">Notification.</param>
		/// <param name="isHorizontal">Is horizontal?</param>
		/// <param name="speed">Animation speed in points per second.</param>
		/// <returns>Animation coroutine.</returns>
		public static IEnumerator AnimationCollapseBase(NotificationBase notification, bool isHorizontal, float speed)
#endif
		{
			return HideAnimationCollapseBase(notification, isHorizontal, speed);
		}

		/// <summary>
		/// Base collapse animation.
		/// </summary>
		/// <returns>Animation coroutine.</returns>
#if CSHARP_7_3_OR_NEWER
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Cached")]
		public static readonly Func<NotificationBase, bool, float, IEnumerator> HideAnimationCollapseBase = HideAnimationCollapseBaseMethod;

		static IEnumerator HideAnimationCollapseBaseMethod(NotificationBase notification, bool isHorizontal, float speed)
#else
		/// <summary>
		/// Base collapse animation.
		/// </summary>
		/// <param name="notification">Notification.</param>
		/// <param name="isHorizontal">Is horizontal?</param>
		/// <param name="speed">Animation speed in points per second.</param>
		/// <returns>Animation coroutine.</returns>
		public static IEnumerator HideAnimationCollapseBase(NotificationBase notification, bool isHorizontal, float speed)
#endif
		{
			var rect = notification.transform as RectTransform;
			var layout = notification.GetComponentInParent<EasyLayout>();
			var axis = isHorizontal ? RectTransform.Axis.Horizontal : RectTransform.Axis.Vertical;
			var base_size = isHorizontal ? rect.rect.width : rect.rect.height;

			var time = base_size / speed;
			var end_time = UtilitiesTime.GetTime(notification.UnscaledTime) + time;

			while (UtilitiesTime.GetTime(notification.UnscaledTime) <= end_time)
			{
				var t = 1f - ((end_time - UtilitiesTime.GetTime(notification.UnscaledTime)) / time);
				var size = Mathf.Lerp(base_size, 0f, t);
				rect.SetSizeWithCurrentAnchors(axis, size);
				if (layout != null)
				{
					layout.NeedUpdateLayout();
				}

				yield return null;
			}

			// return height back for future use
			rect.SetSizeWithCurrentAnchors(axis, base_size);
		}

#if CSHARP_7_3_OR_NEWER
		/// <summary>
		/// Vertical collapse animation.
		/// </summary>
		/// <returns>Animation coroutine.</returns>
		[Obsolete("Renamed to AnimationCollapseVertical.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Cached")]
		public static readonly Func<NotificationBase, IEnumerator> AnimationCollapse = AnimationCollapseMethod;

		static IEnumerator AnimationCollapseMethod(NotificationBase notification)
#else
		/// <summary>
		/// Vertical collapse animation.
		/// </summary>
		/// <param name="notification">Notification.</param>
		/// <returns>Animation coroutine.</returns>
		[Obsolete("Renamed to AnimationCollapseVertical.")]
		public static IEnumerator AnimationCollapse(NotificationBase notification)
#endif
		{
			return AnimationCollapseVertical(notification);
		}

#if CSHARP_7_3_OR_NEWER
		/// <summary>
		/// Vertical collapse animation.
		/// </summary>
		/// <returns>Animation coroutine.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Cached")]
		public static readonly Func<NotificationBase, IEnumerator> AnimationCollapseVertical = AnimationCollapseVerticalMethod;

		static IEnumerator AnimationCollapseVerticalMethod(NotificationBase notification)
#else
		/// <summary>
		/// Vertical collapse animation.
		/// </summary>
		/// <param name="notification">Notification.</param>
		/// <returns>Animation coroutine.</returns>
		public static IEnumerator AnimationCollapseVertical(NotificationBase notification)
#endif
		{
			return HideAnimationCollapseBase(notification, false, 200f);
		}

#if CSHARP_7_3_OR_NEWER
		/// <summary>
		/// Horizontal collapse animation.
		/// </summary>
		/// <returns>Animation coroutine.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Cached")]
		public static readonly Func<NotificationBase, IEnumerator> AnimationCollapseHorizontal = AnimationCollapseHorizontalMethod;

		static IEnumerator AnimationCollapseHorizontalMethod(NotificationBase notification)
#else
		/// <summary>
		/// Horizontal collapse animation.
		/// </summary>
		/// <param name="notification">Notification.</param>
		/// <returns>Animation coroutine.</returns>
		public static IEnumerator AnimationCollapseHorizontal(NotificationBase notification)
#endif
		{
			return HideAnimationCollapseBase(notification, true, 200f);
		}

#if CSHARP_7_3_OR_NEWER
		/// <summary>
		/// Vertical collapse animation.
		/// </summary>
		/// <returns>Animation coroutine.</returns>
		[Obsolete("AnimationCollapse now supports UnscaledTime.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Cached")]
		public static readonly Func<NotificationBase, IEnumerator> AnimationCollapseUnscaledTime = AnimationCollapseUnscaledTimeMethod;

		static IEnumerator AnimationCollapseUnscaledTimeMethod(NotificationBase notification)
#else
		/// <summary>
		/// Vertical collapse animation.
		/// </summary>
		/// <param name="notification">Notification.</param>
		/// <returns>Animation coroutine.</returns>
		[Obsolete("AnimationCollapse now supports UnscaledTime.")]
		public static IEnumerator AnimationCollapseUnscaledTime(NotificationBase notification)
#endif
		{
			return AnimationCollapseVertical(notification);
		}

		#endregion

		#region HideAnimationSlide

#if CSHARP_7_3_OR_NEWER
		/// <summary>
		/// Slide animation to hide notification to right.
		/// </summary>
		/// <returns>Animation coroutine.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Cached")]
		public static readonly Func<NotificationBase, IEnumerator> AnimationSlideRight = AnimationSlideRightMethod;

		static IEnumerator AnimationSlideRightMethod(NotificationBase notification)
#else
		/// <summary>
		/// Slide animation to hide notification to right.
		/// </summary>
		/// <param name="notification">Notification.</param>
		/// <returns>Animation coroutine.</returns>
		public static IEnumerator AnimationSlideRight(NotificationBase notification)
#endif
		{
			return HideAnimationSlideBase(notification, true, +1f, 200f);
		}

#if CSHARP_7_3_OR_NEWER
		/// <summary>
		/// Slide animation to hide notification to left.
		/// </summary>
		/// <returns>Animation coroutine.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Cached")]
		public static readonly Func<NotificationBase, IEnumerator> AnimationSlideLeft = AnimationSlideLeftMethod;

		static IEnumerator AnimationSlideLeftMethod(NotificationBase notification)
#else
		/// <summary>
		/// Slide animation to hide notification to left.
		/// </summary>
		/// <param name="notification">Notification.</param>
		/// <returns>Animation coroutine.</returns>
		public static IEnumerator AnimationSlideLeft(NotificationBase notification)
#endif
		{
			return HideAnimationSlideBase(notification, true, -1f, 200f);
		}

#if CSHARP_7_3_OR_NEWER
		/// <summary>
		/// Slide animation to hide notification to top.
		/// </summary>
		/// <returns>Animation coroutine.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Cached")]
		public static readonly Func<NotificationBase, IEnumerator> AnimationSlideUp = AnimationSlideUpMethod;

		static IEnumerator AnimationSlideUpMethod(NotificationBase notification)
#else
		/// <summary>
		/// Slide animation to hide notification to top.
		/// </summary>
		/// <param name="notification">Notification.</param>
		/// <returns>Animation coroutine.</returns>
		public static IEnumerator AnimationSlideUp(NotificationBase notification)
#endif
		{
			return HideAnimationSlideBase(notification, false, +1f, 200f);
		}

#if CSHARP_7_3_OR_NEWER
		/// <summary>
		/// Slide animation to hide notification to bottom.
		/// </summary>
		/// <returns>Animation coroutine.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Cached")]
		public static readonly Func<NotificationBase, IEnumerator> AnimationSlideDown = AnimationSlideDownMethod;

		static IEnumerator AnimationSlideDownMethod(NotificationBase notification)
#else
		/// <summary>
		/// Slide animation to hide notification to bottom.
		/// </summary>
		/// <param name="notification">Notification.</param>
		/// <returns>Animation coroutine.</returns>
		public static IEnumerator AnimationSlideDown(NotificationBase notification)
#endif
		{
			return HideAnimationSlideBase(notification, false, -1f, 200f);
		}

		/// <summary>
		/// Base slide animation to hide notification.
		/// </summary>
		/// <param name="notification">Notification.</param>
		/// <param name="isHorizontal">Is horizontal slide?</param>
		/// <param name="direction">Slide direction.</param>
		/// <param name="speed">Speed.</param>
		/// <param name="animateOthers">Animate other notifications.</param>
		/// <returns>Animation coroutine.</returns>
		[Obsolete("Renamed to AnimationSlideBaseHide.")]
		public static IEnumerator AnimationSlideBase(NotificationBase notification, bool isHorizontal, float direction, float speed, bool animateOthers = true)
		{
			return HideAnimationSlideBase(notification, isHorizontal, direction, speed, animateOthers);
		}

		/// <summary>
		/// Base slide animation.
		/// </summary>
		/// <param name="notification">Notification.</param>
		/// <param name="isHorizontal">Is horizontal slide?</param>
		/// <param name="direction">Slide direction.</param>
		/// <param name="speed">Speed.</param>
		/// <param name="animateOthers">Animate other notifications.</param>
		/// <returns>Animation coroutine.</returns>
		public static IEnumerator HideAnimationSlideBase(NotificationBase notification, bool isHorizontal, float direction, float speed, bool animateOthers = true)
		{
			var replacement = GetReplacement(notification);

			var layout_element = Utilities.GetOrAddComponent<LayoutElement>(notification);
			layout_element.ignoreLayout = true;

			var layout = notification.GetComponentInParent<EasyLayout>();
			var rect = notification.transform as RectTransform;
			var base_size = isHorizontal ? rect.rect.width : rect.rect.height;
			var base_pos = rect.anchoredPosition;

			var time = base_size / speed;
			var end_time = UtilitiesTime.GetTime(notification.UnscaledTime) + time;
			var axis = isHorizontal ? RectTransform.Axis.Horizontal : RectTransform.Axis.Vertical;

			while (UtilitiesTime.GetTime(notification.UnscaledTime) <= end_time)
			{
				if (!animateOthers)
				{
					base_pos = replacement.anchoredPosition;
				}

				var t = 1 - ((end_time - UtilitiesTime.GetTime(notification.UnscaledTime)) / time);
				var size = Mathf.Lerp(0, base_size, t);
				rect.anchoredPosition = isHorizontal
					? new Vector2(base_pos.x + (size * direction), base_pos.y)
					: new Vector2(base_pos.x, base_pos.y + (size * direction));

				if (animateOthers)
				{
					replacement.SetSizeWithCurrentAnchors(axis, base_size - size);
					if (layout != null)
					{
						layout.NeedUpdateLayout();
					}
				}

				yield return null;
			}

			layout_element.ignoreLayout = false;

			Replacements.Push(replacement);
			replacement.gameObject.SetActive(false);
			replacement.SetSizeWithCurrentAnchors(axis, base_size);

			if (layout != null)
			{
				layout.NeedUpdateLayout();
			}
		}

		#endregion

		#region ShowAnimationSlide

#if CSHARP_7_3_OR_NEWER
		/// <summary>
		/// Slide animation to show notification from right.
		/// </summary>
		/// <returns>Animation coroutine.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Cached")]
		public static readonly Func<NotificationBase, IEnumerator> ShowAnimationSlideRight = ShowAnimationSlideRightMethod;

		static IEnumerator ShowAnimationSlideRightMethod(NotificationBase notification)
#else
		/// <summary>
		/// Slide animation to show notification from right.
		/// </summary>
		/// <param name="notification">Notification.</param>
		/// <returns>Animation coroutine.</returns>
		public static IEnumerator ShowAnimationSlideRight(NotificationBase notification)
#endif
		{
			return ShowAnimationSlideBase(notification, true, +1f, 200f);
		}

#if CSHARP_7_3_OR_NEWER
		/// <summary>
		/// Slide animation to show notification from left.
		/// </summary>
		/// <returns>Animation coroutine.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Cached")]
		public static readonly Func<NotificationBase, IEnumerator> ShowAnimationSlideLeft = ShowAnimationSlideLeftMethod;

		static IEnumerator ShowAnimationSlideLeftMethod(NotificationBase notification)
#else
		/// <summary>
		/// Slide animation to show notification from left.
		/// </summary>
		/// <param name="notification">Notification.</param>
		/// <returns>Animation coroutine.</returns>
		public static IEnumerator ShowAnimationSlideLeft(NotificationBase notification)
#endif
		{
			return ShowAnimationSlideBase(notification, true, -1f, 200f);
		}

#if CSHARP_7_3_OR_NEWER
		/// <summary>
		/// Slide animation to show notification from top.
		/// </summary>
		/// <returns>Animation coroutine.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Cached")]
		public static readonly Func<NotificationBase, IEnumerator> ShowAnimationSlideUp = ShowAnimationSlideUpMethod;

		static IEnumerator ShowAnimationSlideUpMethod(NotificationBase notification)
#else
		/// <summary>
		/// Slide animation to show notification from top.
		/// </summary>
		/// <param name="notification">Notification.</param>
		/// <returns>Animation coroutine.</returns>
		public static IEnumerator ShowAnimationSlideUp(NotificationBase notification)
#endif
		{
			return ShowAnimationSlideBase(notification, false, +1f, 200f);
		}

#if CSHARP_7_3_OR_NEWER
		/// <summary>
		/// Slide animation to show notification from bottom.
		/// </summary>
		/// <returns>Animation coroutine.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Cached")]
		public static readonly Func<NotificationBase, IEnumerator> ShowAnimationSlideDown = ShowAnimationSlideDownMethod;

		static IEnumerator ShowAnimationSlideDownMethod(NotificationBase notification)
#else
		/// <summary>
		/// Slide animation to show notification from bottom.
		/// </summary>
		/// <param name="notification">Notification.</param>
		/// <returns>Animation coroutine.</returns>
		public static IEnumerator ShowAnimationSlideDown(NotificationBase notification)
#endif
		{
			return ShowAnimationSlideBase(notification, false, -1f, 200f);
		}

		/// <summary>
		/// Base slide animation to show notification.
		/// </summary>
		/// <param name="notification">Notification.</param>
		/// <param name="isHorizontal">Is horizontal slide?</param>
		/// <param name="direction">Slide direction.</param>
		/// <param name="speed">Speed.</param>
		/// <param name="animateOthers">Animate other notifications.</param>
		/// <returns>Animation coroutine.</returns>
		public static IEnumerator ShowAnimationSlideBase(NotificationBase notification, bool isHorizontal, float direction, float speed, bool animateOthers = true)
		{
			var layout = notification.GetComponentInParent<EasyLayout>();
			if (layout != null)
			{
				layout.UpdateLayout();
			}

			var replacement = GetReplacement(notification);

			var layout_element = Utilities.GetOrAddComponent<LayoutElement>(notification);
			layout_element.ignoreLayout = true;

			var rect = notification.transform as RectTransform;
			var base_size = isHorizontal ? rect.rect.width : rect.rect.height;
			var base_pos = rect.anchoredPosition;

			var time = base_size / speed;
			var end_time = UtilitiesTime.GetTime(notification.UnscaledTime) + time;
			var axis = isHorizontal ? RectTransform.Axis.Horizontal : RectTransform.Axis.Vertical;

			while (UtilitiesTime.GetTime(notification.UnscaledTime) <= end_time)
			{
				if (!animateOthers)
				{
					base_pos = replacement.anchoredPosition;
				}

				var t = 1 - ((end_time - UtilitiesTime.GetTime(notification.UnscaledTime)) / time);
				var size = Mathf.Lerp(base_size, 0, t);
				rect.anchoredPosition = isHorizontal
					? new Vector2(base_pos.x + (size * direction), base_pos.y)
					: new Vector2(base_pos.x, base_pos.y + (size * direction));

				if (animateOthers)
				{
					replacement.SetSizeWithCurrentAnchors(axis, base_size - size);
					if (layout != null)
					{
						layout.NeedUpdateLayout();
					}
				}

				yield return null;
			}

			layout_element.ignoreLayout = false;

			Replacements.Push(replacement);
			replacement.gameObject.SetActive(false);
			replacement.SetSizeWithCurrentAnchors(axis, base_size);

			if (layout != null)
			{
				layout.NeedUpdateLayout();
			}
		}

		#endregion

		#region ShowAnimationExplode

#if CSHARP_7_3_OR_NEWER
		/// <summary>
		/// Base explode animation to show notification.
		/// </summary>
		/// <returns>Animation coroutine.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Cached")]
		public static readonly Func<NotificationBase, bool, float, IEnumerator> ShowAnimationExplodeBase = ShowAnimationExplodeBaseMethod;

		static IEnumerator ShowAnimationExplodeBaseMethod(NotificationBase notification, bool isHorizontal, float speed)
#else
		/// <summary>
		/// Base explode animation to show notification.
		/// </summary>
		/// <param name="notification">Notification.</param>
		/// <param name="isHorizontal">Is horizontal?</param>
		/// <param name="speed">Animation speed in points per second.</param>
		/// <returns>Animation coroutine.</returns>
		public static IEnumerator ShowAnimationExplodeBase(NotificationBase notification, bool isHorizontal, float speed)
#endif
		{
			var rect = notification.transform as RectTransform;
			var layout = notification.GetComponentInParent<EasyLayout>();
			var axis = isHorizontal ? RectTransform.Axis.Horizontal : RectTransform.Axis.Vertical;
			var base_size = isHorizontal ? rect.rect.width : rect.rect.height;

			var time = base_size / speed;
			var end_time = UtilitiesTime.GetTime(notification.UnscaledTime) + time;

			while (UtilitiesTime.GetTime(notification.UnscaledTime) <= end_time)
			{
				var t = 1f - ((end_time - UtilitiesTime.GetTime(notification.UnscaledTime)) / time);
				var size = Mathf.Lerp(0f, base_size, t);
				rect.SetSizeWithCurrentAnchors(axis, size);
				if (layout != null)
				{
					layout.NeedUpdateLayout();
				}

				yield return null;
			}

			// return height back for future use
			rect.SetSizeWithCurrentAnchors(axis, base_size);
		}

#if CSHARP_7_3_OR_NEWER
		/// <summary>
		/// Vertical explode animation to show notification.
		/// </summary>
		/// <returns>Animation coroutine.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Cached")]
		public static readonly Func<NotificationBase, IEnumerator> ShowAnimationExplodeVertical = ShowAnimationExplodeVerticalMethod;

		static IEnumerator ShowAnimationExplodeVerticalMethod(NotificationBase notification)
#else
		/// <summary>
		/// Vertical explode animation to show notification.
		/// </summary>
		/// <param name="notification">Notification.</param>
		/// <returns>Animation coroutine.</returns>
		public static IEnumerator ShowAnimationExplodeVertical(NotificationBase notification)
#endif
		{
			return ShowAnimationExplodeBase(notification, false, 200f);
		}

#if CSHARP_7_3_OR_NEWER
		/// <summary>
		/// Horizontal explode animation to show notification.
		/// </summary>
		/// <returns>Animation coroutine.</returns>
		public static readonly Func<NotificationBase, IEnumerator> ShowAnimationExplodeHorizontal = ShowAnimationExplodeHorizontalMethod;

		static IEnumerator ShowAnimationExplodeHorizontalMethod(NotificationBase notification)
#else
		/// <summary>
		/// Horizontal explode animation to show notification.
		/// </summary>
		/// <param name="notification">Notification.</param>
		/// <returns>Animation coroutine.</returns>
		public static IEnumerator ShowAnimationExplodeHorizontal(NotificationBase notification)
#endif
		{
			return ShowAnimationExplodeBase(notification, true, 200f);
		}
		#endregion

		#region ShowAnimationRotate

#if CSHARP_7_3_OR_NEWER
		/// <summary>
		/// Vertical rotate animation to show notification.
		/// </summary>
		/// <returns>Animation coroutine.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Cached")]
		public static readonly Func<NotificationBase, IEnumerator> ShowAnimationRotateVertical = ShowAnimationRotateVerticalMethod;

		static IEnumerator ShowAnimationRotateVerticalMethod(NotificationBase notification)
#else
		/// <summary>
		/// Vertical rotate animation to show notification.
		/// </summary>
		/// <param name="notification">Notification.</param>
		/// <returns>Animation coroutine.</returns>
		public static IEnumerator ShowAnimationRotateVertical(NotificationBase notification)
#endif
		{
			return ShowAnimationRotateBase(notification, false, 0.5f);
		}

#if CSHARP_7_3_OR_NEWER
		/// <summary>
		/// Horizontal rotate animation to show notification.
		/// </summary>
		/// <returns>Animation coroutine.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Cached")]
		public static readonly Func<NotificationBase, IEnumerator> ShowAnimationRotateHorizontal = ShowAnimationRotateHorizontalMethod;

		static IEnumerator ShowAnimationRotateHorizontalMethod(NotificationBase notification)
#else
		/// <summary>
		/// Horizontal rotate animation to show notification.
		/// </summary>
		/// <param name="notification">Notification.</param>
		/// <returns>Animation coroutine.</returns>
		public static IEnumerator ShowAnimationRotateHorizontal(NotificationBase notification)
#endif
		{
			return ShowAnimationRotateBase(notification, true, 0.5f);
		}

#if CSHARP_7_3_OR_NEWER
		/// <summary>
		/// Base rotate animation to show notification.
		/// </summary>
		/// <returns>Animation coroutine.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Cached")]
		public static readonly Func<NotificationBase, bool, float, IEnumerator> ShowAnimationRotateBase = ShowAnimationRotateBaseMethod;

		static IEnumerator ShowAnimationRotateBaseMethod(NotificationBase notification, bool isHorizontal, float timeLength)
#else
		/// <summary>
		/// Base rotate animation to show notification.
		/// </summary>
		/// <param name="notification">Notification.</param>
		/// <param name="isHorizontal">Is horizontal></param>
		/// <param name="timeLength">Animation length in seconds.</param>
		/// <returns>Animation coroutine.</returns>
		public static IEnumerator ShowAnimationRotateBase(NotificationBase notification, bool isHorizontal, float timeLength)
#endif
		{
			var rect = notification.transform as RectTransform;
			var base_rotation = rect.localRotation.eulerAngles;

			var end_time = UtilitiesTime.GetTime(notification.UnscaledTime) + timeLength;

			while (UtilitiesTime.GetTime(notification.UnscaledTime) <= end_time)
			{
				var t = 1f - ((end_time - UtilitiesTime.GetTime(notification.UnscaledTime)) / timeLength);
				var rotation = Mathf.Lerp(90f, 0f, t);

				rect.localRotation = isHorizontal
					? Quaternion.Euler(base_rotation.x, rotation, base_rotation.z)
					: Quaternion.Euler(rotation, base_rotation.y, base_rotation.z);
				yield return null;
			}

			// return rotation back for future use
			rect.localRotation = Quaternion.Euler(base_rotation);
		}

		#endregion
	}
}