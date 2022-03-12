namespace UIWidgets
{
	using System;
	using System.Collections;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Animations.
	/// </summary>
	public static class Animations
	{
		/// <summary>
		/// Run animation with animation curve value.
		/// </summary>
		/// <param name="curve">Animation curve.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <param name="setValue">Function to set value.</param>
		/// <param name="onEnd">Action on animation end.</param>
		/// <returns>Animation coroutine.</returns>
		public static IEnumerator Run(AnimationCurve curve, bool unscaledTime, Action<float> setValue, Action onEnd = null)
		{
			var last = curve[curve.length - 1];
			var time = last.time;
			var start_time = UtilitiesTime.GetTime(unscaledTime);
			var end_time = start_time + time;

			while (UtilitiesTime.GetTime(unscaledTime) <= end_time)
			{
				var position = UtilitiesTime.GetTime(unscaledTime) - start_time;

				setValue(curve.Evaluate(position));
				yield return null;
			}

			setValue(last.value);

			if (onEnd != null)
			{
				onEnd();
			}
		}

		/// <summary>
		/// Run animation with float value.
		/// </summary>
		/// <param name="curve">Animation curve with value in range [0..1].</param>
		/// <param name="startValue">Start value.</param>
		/// <param name="endValue">End value.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <param name="setValue">Function to set value.</param>
		/// <param name="onEnd">Action on animation end.</param>
		/// <returns>Animation coroutine.</returns>
		public static IEnumerator Run(AnimationCurve curve, float startValue, float endValue, bool unscaledTime, Action<float> setValue, Action onEnd = null)
		{
			var last = curve[curve.length - 1];
			var time = last.time;
			var start_time = UtilitiesTime.GetTime(unscaledTime);
			var end_time = start_time + time;

			while (UtilitiesTime.GetTime(unscaledTime) <= end_time)
			{
				var position = UtilitiesTime.GetTime(unscaledTime) - start_time;
				var value = Mathf.Lerp(startValue, endValue, curve.Evaluate(position));

				setValue(value);
				yield return null;
			}

			setValue(endValue);

			if (onEnd != null)
			{
				onEnd();
			}
		}

		/// <summary>
		/// Run animation with Vector2 values.
		/// </summary>
		/// <param name="curve">Animation curve with value in range [0..1].</param>
		/// <param name="startValue">Start value.</param>
		/// <param name="endValue">End value.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <param name="setValue">Function to set value.</param>
		/// <param name="onEnd">Action on animation end.</param>
		/// <returns>Animation coroutine.</returns>
		public static IEnumerator Run(AnimationCurve curve, Vector2 startValue, Vector2 endValue, bool unscaledTime, Action<Vector2> setValue, Action onEnd = null)
		{
			var last = curve[curve.length - 1];
			var time = last.time;
			var start_time = UtilitiesTime.GetTime(unscaledTime);
			var end_time = start_time + time;

			while (UtilitiesTime.GetTime(unscaledTime) <= end_time)
			{
				var position = UtilitiesTime.GetTime(unscaledTime) - start_time;
				var value = Vector2.Lerp(startValue, endValue, curve.Evaluate(position));

				setValue(value);
				yield return null;
			}

			setValue(endValue);

			if (onEnd != null)
			{
				onEnd();
			}
		}

		/// <summary>
		/// Run animation with Vector3 values.
		/// </summary>
		/// <param name="curve">Animation curve with value in range [0..1].</param>
		/// <param name="startValue">Start value.</param>
		/// <param name="endValue">End value.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <param name="setValue">Function to set value.</param>
		/// <param name="onEnd">Action on animation end.</param>
		/// <returns>Animation coroutine.</returns>
		public static IEnumerator Run(AnimationCurve curve, Vector3 startValue, Vector3 endValue, bool unscaledTime, Action<Vector3> setValue, Action onEnd = null)
		{
			var last = curve[curve.length - 1];
			var time = last.time;
			var start_time = UtilitiesTime.GetTime(unscaledTime);
			var end_time = start_time + time;

			while (UtilitiesTime.GetTime(unscaledTime) <= end_time)
			{
				var position = UtilitiesTime.GetTime(unscaledTime) - start_time;
				var value = Vector3.Lerp(startValue, endValue, curve.Evaluate(position));

				setValue(value);
				yield return null;
			}

			setValue(endValue);

			if (onEnd != null)
			{
				onEnd();
			}
		}

		/// <summary>
		/// Run animation Quaternion values.
		/// </summary>
		/// <param name="curve">Animation curve with value in range [0..1].</param>
		/// <param name="startValue">Start value.</param>
		/// <param name="endValue">End value.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <param name="setValue">Function to set value.</param>
		/// <param name="onEnd">Action on animation end.</param>
		/// <returns>Animation coroutine.</returns>
		public static IEnumerator Run(AnimationCurve curve, Quaternion startValue, Quaternion endValue, bool unscaledTime, Action<Quaternion> setValue, Action onEnd = null)
		{
			var last = curve[curve.length - 1];
			var time = last.time;
			var start_time = UtilitiesTime.GetTime(unscaledTime);
			var end_time = start_time + time;

			while (UtilitiesTime.GetTime(unscaledTime) <= end_time)
			{
				var position = UtilitiesTime.GetTime(unscaledTime) - start_time;
				var value = Quaternion.Lerp(startValue, endValue, curve.Evaluate(position));

				setValue(value);
				yield return null;
			}

			setValue(endValue);

			if (onEnd != null)
			{
				onEnd();
			}
		}

		/// <summary>
		/// Rotate animation.
		/// </summary>
		/// <returns>Animation.</returns>
		/// <param name="rectTransform">RectTransform.</param>
		/// <param name="time">Time.</param>
		/// <param name="startAngle">Start rotation angle.</param>
		/// <param name="endAngle">End rotation angle.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <param name="actionAfter">Action to run after animation.</param>
		public static IEnumerator Rotate(RectTransform rectTransform, float time = 0.5f, float startAngle = 0, float endAngle = 90, bool unscaledTime = false, Action actionAfter = null)
		{
			var start_rotarion = rectTransform.localRotation.eulerAngles;

			var end_time = UtilitiesTime.GetTime(unscaledTime) + time;

			while (UtilitiesTime.GetTime(unscaledTime) <= end_time)
			{
				var t = 1 - ((end_time - UtilitiesTime.GetTime(unscaledTime)) / time);
				var rotation_x = Mathf.Lerp(startAngle, endAngle, t);

				rectTransform.localRotation = Quaternion.Euler(rotation_x, start_rotarion.y, start_rotarion.z);
				yield return null;
			}

			// return rotation back for future use
			rectTransform.localRotation = Quaternion.Euler(start_rotarion);

			if (actionAfter != null)
			{
				actionAfter();
			}
		}

		/// <summary>
		/// Rotate animation.
		/// </summary>
		/// <returns>Animation.</returns>
		/// <param name="rectTransform">RectTransform.</param>
		/// <param name="time">Time.</param>
		/// <param name="startAngle">Start rotation angle.</param>
		/// <param name="endAngle">End rotation angle.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <param name="actionAfter">Action to run after animation.</param>
		public static IEnumerator RotateZ(RectTransform rectTransform, float time = 0.5f, float startAngle = 0, float endAngle = 90, bool unscaledTime = false, Action actionAfter = null)
		{
			var start_rotarion = rectTransform.localRotation.eulerAngles;
			var end_time = UtilitiesTime.GetTime(unscaledTime) + time;

			while (UtilitiesTime.GetTime(unscaledTime) <= end_time)
			{
				var t = 1 - ((end_time - UtilitiesTime.GetTime(unscaledTime)) / time);
				var rotation_z = Mathf.Lerp(startAngle, endAngle, t);

				rectTransform.localRotation = Quaternion.Euler(start_rotarion.x, start_rotarion.y, rotation_z);
				yield return null;
			}

			// return rotation back for future use
			rectTransform.localRotation = Quaternion.Euler(start_rotarion);

			if (actionAfter != null)
			{
				actionAfter();
			}
		}

		static readonly AnimationCurve LinearCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

		/// <summary>
		/// Collapse animation.
		/// </summary>
		/// <returns>Animation.</returns>
		/// <param name="rectTransform">RectTransform.</param>
		/// <param name="time">Time.</param>
		/// <param name="isHorizontal">Is Horizontal animated width changes; otherwise height.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <param name="actionAfter">Action to run after animation.</param>
		public static IEnumerator Collapse(RectTransform rectTransform, float time = 0.5f, bool isHorizontal = false, bool unscaledTime = false, Action actionAfter = null)
		{
			return Collapse(rectTransform, time, LinearCurve, isHorizontal, unscaledTime, actionAfter);
		}

		/// <summary>
		/// Collapse animation.
		/// </summary>
		/// <returns>Animation.</returns>
		/// <param name="rectTransform">RectTransform.</param>
		/// <param name="rate">Speed rate.</param>
		/// <param name="curve">Animation curve.</param>
		/// <param name="isHorizontal">Is Horizontal animated width changes; otherwise height.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <param name="actionAfter">Action to run after animation.</param>
		public static IEnumerator Collapse(RectTransform rectTransform, float rate, AnimationCurve curve, bool isHorizontal = false, bool unscaledTime = false, Action actionAfter = null)
		{
			var layoutElement = Utilities.GetOrAddComponent<LayoutElement>(rectTransform);
			var size = isHorizontal
				? (LayoutUtilities.IsWidthControlled(rectTransform) ? LayoutUtility.GetPreferredWidth(rectTransform) : rectTransform.rect.width)
				: (LayoutUtilities.IsHeightControlled(rectTransform) ? LayoutUtility.GetPreferredHeight(rectTransform) : rectTransform.rect.height);

			var time = curve[curve.length - 1].time * rate;
			var end_time = UtilitiesTime.GetTime(unscaledTime) + time;

			while (UtilitiesTime.GetTime(unscaledTime) <= end_time)
			{
				var t = (end_time - UtilitiesTime.GetTime(unscaledTime)) / time;
				var new_size = curve.Evaluate(t) * size;
				if (isHorizontal)
				{
					rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, new_size);
					layoutElement.preferredWidth = new_size;
				}
				else
				{
					rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, new_size);
					layoutElement.preferredHeight = new_size;
				}

				yield return null;
			}

			// return size back for future use
			if (isHorizontal)
			{
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
				layoutElement.preferredWidth = size;
			}
			else
			{
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
				layoutElement.preferredHeight = size;
			}

			if (actionAfter != null)
			{
				actionAfter();
			}
		}

		/// <summary>
		/// Collapse animation.
		/// </summary>
		/// <returns>Animation.</returns>
		/// <param name="rectTransform">RectTransform.</param>
		/// <param name="time">Time.</param>
		/// <param name="isHorizontal">Is Horizontal animated width changes; otherwise height.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <param name="actionAfter">Action to run after animation.</param>
		public static IEnumerator CollapseFlexible(RectTransform rectTransform, float time = 0.5f, bool isHorizontal = false, bool unscaledTime = false, Action actionAfter = null)
		{
			return CollapseFlexible(rectTransform, time, LinearCurve, isHorizontal, unscaledTime, actionAfter);
		}

		/// <summary>
		/// Collapse animation.
		/// </summary>
		/// <returns>Animation.</returns>
		/// <param name="rectTransform">RectTransform.</param>
		/// <param name="rate">Speed rate.</param>
		/// <param name="curve">Animation curve.</param>
		/// <param name="isHorizontal">Is Horizontal animated width changes; otherwise height.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <param name="actionAfter">Action to run after animation.</param>
		public static IEnumerator CollapseFlexible(RectTransform rectTransform, float rate, AnimationCurve curve, bool isHorizontal = false, bool unscaledTime = false, Action actionAfter = null)
		{
			var layoutElement = Utilities.GetOrAddComponent<LayoutElement>(rectTransform);
			if (isHorizontal)
			{
				layoutElement.preferredWidth = -1f;
			}
			else
			{
				layoutElement.preferredHeight = -1f;
			}

			var time = curve[curve.length - 1].time * rate;
			var end_time = UtilitiesTime.GetTime(unscaledTime) + time;

			while (UtilitiesTime.GetTime(unscaledTime) <= end_time)
			{
				var t = (end_time - UtilitiesTime.GetTime(unscaledTime)) / time;
				var size = curve.Evaluate(t);
				if (isHorizontal)
				{
					layoutElement.flexibleWidth = size;
				}
				else
				{
					layoutElement.flexibleHeight = size;
				}

				yield return null;
			}

			if (isHorizontal)
			{
				layoutElement.flexibleWidth = 0f;
			}
			else
			{
				layoutElement.flexibleHeight = 0f;
			}

			if (actionAfter != null)
			{
				actionAfter();
			}
		}

		/// <summary>
		/// Open animation.
		/// </summary>
		/// <returns>Animation.</returns>
		/// <param name="rectTransform">RectTransform.</param>
		/// <param name="time">Time.</param>
		/// <param name="isHorizontal">Is Horizontal animated width changes; otherwise height.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <param name="actionAfter">Action to run after animation.</param>
		public static IEnumerator Open(RectTransform rectTransform, float time = 0.5f, bool isHorizontal = false, bool unscaledTime = false, Action actionAfter = null)
		{
			return Open(rectTransform, time, LinearCurve, isHorizontal, unscaledTime, actionAfter);
		}

		/// <summary>
		/// Open animation.
		/// </summary>
		/// <returns>Animation.</returns>
		/// <param name="rectTransform">RectTransform.</param>
		/// <param name="rate">Speed rate.</param>
		/// <param name="curve">Animation curve.</param>
		/// <param name="isHorizontal">Is Horizontal animated width changes; otherwise height.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <param name="actionAfter">Action to run after animation.</param>
		public static IEnumerator Open(RectTransform rectTransform, float rate, AnimationCurve curve, bool isHorizontal, bool unscaledTime, Action actionAfter = null)
		{
			var layoutElement = Utilities.GetOrAddComponent<LayoutElement>(rectTransform);
			var size = isHorizontal
				? (LayoutUtilities.IsWidthControlled(rectTransform) ? LayoutUtility.GetPreferredWidth(rectTransform) : rectTransform.rect.width)
				: (LayoutUtilities.IsHeightControlled(rectTransform) ? LayoutUtility.GetPreferredHeight(rectTransform) : rectTransform.rect.height);

			var time = curve[curve.length - 1].time * rate;
			var end_time = UtilitiesTime.GetTime(unscaledTime) + time;

			while (UtilitiesTime.GetTime(unscaledTime) <= end_time)
			{
				var t = 1f - ((end_time - UtilitiesTime.GetTime(unscaledTime)) / time);
				var new_size = curve.Evaluate(t) * size;
				if (isHorizontal)
				{
					rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, new_size);
					layoutElement.preferredWidth = new_size;
				}
				else
				{
					rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, new_size);
					layoutElement.preferredHeight = new_size;
				}

				yield return null;
			}

			// return size back for future use
			if (isHorizontal)
			{
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
				layoutElement.preferredWidth = size;
			}
			else
			{
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
				layoutElement.preferredHeight = size;
			}

			if (actionAfter != null)
			{
				actionAfter();
			}
		}

		/// <summary>
		/// Open animation.
		/// </summary>
		/// <returns>Animation.</returns>
		/// <param name="rectTransform">RectTransform.</param>
		/// <param name="time">Time.</param>
		/// <param name="isHorizontal">Is Horizontal animated width changes; otherwise height.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <param name="actionAfter">Action to run after animation.</param>
		public static IEnumerator OpenFlexible(RectTransform rectTransform, float time = 0.5f, bool isHorizontal = false, bool unscaledTime = false, Action actionAfter = null)
		{
			return OpenFlexible(rectTransform, time, LinearCurve, isHorizontal, unscaledTime, actionAfter);
		}

		/// <summary>
		/// Open animation.
		/// </summary>
		/// <returns>Animation.</returns>
		/// <param name="rectTransform">RectTransform.</param>
		/// <param name="rate">Speed rate.</param>
		/// <param name="curve">Animation curve.</param>
		/// <param name="isHorizontal">Is Horizontal animated width changes; otherwise height.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <param name="actionAfter">Action to run after animation.</param>
		public static IEnumerator OpenFlexible(RectTransform rectTransform, float rate, AnimationCurve curve, bool isHorizontal = false, bool unscaledTime = false, Action actionAfter = null)
		{
			var layoutElement = Utilities.GetOrAddComponent<LayoutElement>(rectTransform);
			if (isHorizontal)
			{
				layoutElement.preferredWidth = -1f;
			}
			else
			{
				layoutElement.preferredHeight = -1f;
			}

			var time = curve[curve.length - 1].time * rate;
			var end_time = UtilitiesTime.GetTime(unscaledTime) + time;

			while (UtilitiesTime.GetTime(unscaledTime) <= end_time)
			{
				var t = 1f - ((end_time - UtilitiesTime.GetTime(unscaledTime)) / time);
				var size = curve.Evaluate(t);
				if (isHorizontal)
				{
					layoutElement.flexibleWidth = size;
				}
				else
				{
					layoutElement.flexibleHeight = size;
				}

				yield return null;
			}

			// return size back for future use
			if (isHorizontal)
			{
				layoutElement.flexibleWidth = 1f;
			}
			else
			{
				layoutElement.flexibleHeight = 1f;
			}

			if (actionAfter != null)
			{
				actionAfter();
			}
		}
	}
}