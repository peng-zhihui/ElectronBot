namespace UIWidgets
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Base class for ProgressbarDeterminate.
	/// http://ilih.ru/images/unity-assets/UIWidgets/ProgressbarDeterminate.png
	/// </summary>
	[DataBindSupport]
	public class ProgressbarDeterminateBase : UIWidgetsMonoBehaviour, IStylable, IValidateable
	{
		/// <summary>
		/// Maximum value of the progress.
		/// </summary>
		[SerializeField]
		[DataBindField]
		public int Max = 100;

		[SerializeField]
		int progressValue;

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		[DataBindField]
		public int Value
		{
			get
			{
				return progressValue;
			}

			set
			{
				progressValue = Mathf.Clamp(value, 0, Max);
				UpdateProgressbar();
			}
		}

		[SerializeField]
		Image fullBarMask;

		/// <summary>
		/// Gets or sets the full bar mask.
		/// </summary>
		/// <value>The full bar.</value>
		public Image FullBarMask
		{
			get
			{
				return fullBarMask;
			}

			set
			{
				fullBarMask = value;
				UpdateProgressbar();
			}
		}

		[SerializeField]
		Image fullBarBorder;

		/// <summary>
		/// Gets or sets the full bar border.
		/// </summary>
		/// <value>The full bar.</value>
		public Image FullBarBorder
		{
			get
			{
				return fullBarBorder;
			}

			set
			{
				fullBarBorder = value;
				UpdateProgressbar();
			}
		}

		[SerializeField]
		ProgressbarTextTypes textType = ProgressbarTextTypes.None;

		/// <summary>
		/// Gets or sets the type of the text.
		/// </summary>
		/// <value>The type of the text.</value>
		public ProgressbarTextTypes TextType
		{
			get
			{
				return textType;
			}

			set
			{
				textType = value;
				ToggleTextType();
			}
		}

		/// <summary>
		/// The speed.
		/// </summary>
		[SerializeField]
		public float Speed = 0.1f;

		/// <summary>
		/// The type of the speed.
		/// </summary>
		[SerializeField]
		public ProgressbarSpeedType SpeedType = ProgressbarSpeedType.ConstantSpeed;

		/// <summary>
		/// The unscaled time.
		/// </summary>
		[SerializeField]
		public bool UnscaledTime = false;

		Func<ProgressbarDeterminateBase, string> textFunc = TextPercent;

		/// <summary>
		/// Gets or sets the text function.
		/// </summary>
		/// <value>The text function.</value>
		public Func<ProgressbarDeterminateBase, string> TextFunc
		{
			get
			{
				return textFunc;
			}

			set
			{
				textFunc = value;
				UpdateText();
			}
		}

		/// <summary>
		/// Background.
		/// </summary>
		[SerializeField]
		public Image Background;

		/// <summary>
		/// Image.
		/// </summary>
		[SerializeField]
		public Image EmptyBar;

		/// <summary>
		/// FullBar texture.
		/// </summary>
		[SerializeField]
		public Image FullBarImage;

		/// <summary>
		/// The empty bar text.
		/// </summary>
		[SerializeField]
		public TextAdapter EmptyBarTextAdapter;

		/// <summary>
		/// The full bar text.
		/// </summary>
		[SerializeField]
		public TextAdapter FullBarTextAdapter;

		/// <summary>
		/// Gets a value indicating whether animation running.
		/// </summary>
		/// <value><c>true</c> if animation running; otherwise, <c>false</c>.</value>
		public bool IsAnimationRunning
		{
			get;
			protected set;
		}

		/// <summary>
		/// Don't show progress text.
		/// </summary>
		/// <returns>string.Empty</returns>
		/// <param name="bar">Progress bar.</param>
		public static string TextNone(ProgressbarDeterminateBase bar)
		{
			return string.Empty;
		}

		/// <summary>
		/// Show progress with percent.
		/// </summary>
		/// <returns>string.Empty</returns>
		/// <param name="bar">Progress bar.</param>
		public static string TextPercent(ProgressbarDeterminateBase bar)
		{
			return string.Format("{0}", ((float)bar.Value / bar.Max).ToString("P0"));
		}

		/// <summary>
		/// Show progress with range.
		/// </summary>
		/// <returns>The range.</returns>
		/// <param name="bar">Progress bar.</param>
		public static string TextRange(ProgressbarDeterminateBase bar)
		{
			return string.Format("{0} / {1}", bar.Value.ToString(), bar.Max.ToString());
		}

		IEnumerator currentAnimation;

		/// <summary>
		/// Animate the progress bar to specified targetValue.
		/// </summary>
		/// <param name="targetValue">Target value.</param>
		public void Animate(int? targetValue = null)
		{
			if (currentAnimation != null)
			{
				StopCoroutine(currentAnimation);
			}

			if (SpeedType == ProgressbarSpeedType.TimeToValueChangedOnOne)
			{
				currentAnimation = AnimationPerOne(targetValue ?? Max);
			}
			else if (SpeedType == ProgressbarSpeedType.ConstantSpeed)
			{
				currentAnimation = AnimationConstantSpeed(targetValue ?? Max);
			}
			else if (SpeedType == ProgressbarSpeedType.ConstantTime)
			{
				currentAnimation = AnimationConstantTime(targetValue ?? Max);
			}

			IsAnimationRunning = true;
			StartCoroutine(currentAnimation);
		}

		/// <summary>
		/// Stop animation.
		/// </summary>
		public void Stop()
		{
			if (IsAnimationRunning)
			{
				StopCoroutine(currentAnimation);
				IsAnimationRunning = false;
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0401:Possible allocation of reference type enumerator", Justification = "Enumerator is reusable.")]
		IEnumerator AnimationPerOne(int targetValue)
		{
			if (targetValue > Max)
			{
				targetValue = Max;
			}

			var delta = targetValue - Value;

			if (delta != 0)
			{
				while (true)
				{
					if (delta > 0)
					{
						progressValue += 1;
					}
					else
					{
						progressValue -= 1;
					}

					UpdateProgressbar();
					if (progressValue == targetValue)
					{
						break;
					}

					yield return UtilitiesTime.Wait(Speed, UnscaledTime);
				}
			}

			IsAnimationRunning = false;
		}

		/// <summary>
		/// Gets the time.
		/// </summary>
		/// <returns>The time.</returns>
		[Obsolete("Use Utilities.GetTime(UnscaledTime).")]
		protected virtual float GetTime()
		{
			return Utilities.GetTime(UnscaledTime);
		}

		IEnumerator AnimationConstantSpeed(int targetValue)
		{
			if (targetValue > Max)
			{
				targetValue = Max;
			}

			var start = Value;
			var delta = targetValue - start;

			if (delta != 0)
			{
				var step = Speed / Max;
				var total_time = Mathf.Abs(step * delta);

				var start_time = UtilitiesTime.GetTime(UnscaledTime);
				var end_time = start_time + total_time;

				do
				{
					var progress = Mathf.Min(1, (UtilitiesTime.GetTime(UnscaledTime) - start_time) / total_time);

					progressValue = start + Mathf.RoundToInt(progress * delta);

					UpdateProgressbar();

					yield return null;
				}
				while (end_time > UtilitiesTime.GetTime(UnscaledTime));

				progressValue = targetValue;
				UpdateProgressbar();
			}

			IsAnimationRunning = false;
		}

		IEnumerator AnimationConstantTime(int targetValue)
		{
			if (targetValue > Max)
			{
				targetValue = Max;
			}

			var start = Value;
			var delta = targetValue - start;

			if (delta != 0)
			{
				var total_time = Speed;

				var start_time = UtilitiesTime.GetTime(UnscaledTime);
				var end_time = start_time + total_time;

				do
				{
					var progress = Mathf.Min(1, (UtilitiesTime.GetTime(UnscaledTime) - start_time) / total_time);

					progressValue = start + Mathf.RoundToInt(progress * delta);

					UpdateProgressbar();

					yield return null;
				}
				while (end_time > UtilitiesTime.GetTime(UnscaledTime));

				progressValue = targetValue;
				UpdateProgressbar();
			}

			IsAnimationRunning = false;
		}

		/// <summary>
		/// Update progress bar.
		/// </summary>
		public void Refresh()
		{
			ToggleTextType();
			UpdateProgressbar();
		}

		void UpdateProgressbar()
		{
			var amount = Value / (float)Max;

			if (FullBarMask != null)
			{
				FullBarMask.fillAmount = amount;
			}

			if (FullBarBorder != null)
			{
				FullBarBorder.fillAmount = amount;
			}

			UpdateText();
		}

		/// <summary>
		/// Updates the text.
		/// </summary>
		protected virtual void UpdateText()
		{
			var text = TextFunc(this);

			if (FullBarTextAdapter != null)
			{
				FullBarTextAdapter.text = text;
			}

			if (EmptyBarTextAdapter != null)
			{
				EmptyBarTextAdapter.text = text;
			}
		}

		/// <summary>
		/// Toggle text type.
		/// </summary>
		protected void ToggleTextType()
		{
			switch (TextType)
			{
				case ProgressbarTextTypes.None:
					textFunc = TextNone;
					break;
				case ProgressbarTextTypes.Percent:
					textFunc = TextPercent;
					break;
				case ProgressbarTextTypes.Range:
					textFunc = TextRange;
					break;
				default:
					Debug.LogWarning(string.Format("Unknown TextType: {0}", EnumHelper<ProgressbarTextTypes>.ToString(TextType)));
					break;
			}
		}

#if UNITY_EDITOR
		/// <summary>
		/// Validate progress bar value.
		/// </summary>
		public virtual void Validate()
		{
			ToggleTextType();
			Value = progressValue;
		}

		/// <summary>
		/// Validate progress bar value.
		/// </summary>
		protected virtual void OnValidate()
		{
			progressValue = Mathf.Clamp(progressValue, 0, Max);
		}
#endif

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			style.ProgressbarDeterminate.FullbarImage.ApplyTo(FullBarImage);
			style.ProgressbarDeterminate.FullbarMask.ApplyTo(FullBarMask);
			style.ProgressbarDeterminate.FullbarBorder.ApplyTo(FullBarBorder);
			style.ProgressbarDeterminate.EmptyBar.ApplyTo(EmptyBar);
			style.ProgressbarDeterminate.Background.ApplyTo(Background);

			Value = progressValue;

			if (FullBarTextAdapter != null)
			{
				style.ProgressbarDeterminate.FullBarText.ApplyTo(FullBarTextAdapter.gameObject);
			}

			if (EmptyBarTextAdapter != null)
			{
				style.ProgressbarDeterminate.EmptyBarText.ApplyTo(EmptyBarTextAdapter.gameObject);
			}

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			style.ProgressbarDeterminate.FullbarImage.GetFrom(FullBarImage);
			style.ProgressbarDeterminate.FullbarMask.GetFrom(FullBarMask);
			style.ProgressbarDeterminate.FullbarBorder.GetFrom(FullBarBorder);
			style.ProgressbarDeterminate.EmptyBar.GetFrom(EmptyBar);
			style.ProgressbarDeterminate.Background.GetFrom(Background);

			if (FullBarTextAdapter != null)
			{
				style.ProgressbarDeterminate.FullBarText.GetFrom(FullBarTextAdapter.gameObject);
			}

			if (EmptyBarTextAdapter != null)
			{
				style.ProgressbarDeterminate.EmptyBarText.GetFrom(EmptyBarTextAdapter.gameObject);
			}

			return true;
		}
		#endregion
	}
}