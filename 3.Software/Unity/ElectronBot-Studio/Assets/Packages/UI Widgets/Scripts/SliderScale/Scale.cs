namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using UIWidgets.Attributes;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Scale widget.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	public class Scale : MonoBehaviourConditional, IObservable, INotifyPropertyChanged, IStylable
	{
		/// <summary>
		/// Mark data.
		/// </summary>
		public struct MarkData : IEquatable<MarkData>
		{
			/// <summary>
			/// Position.
			/// </summary>
			public readonly Vector2 Position;

			/// <summary>
			/// Rotation.
			/// </summary>
			public readonly float Rotation;

			/// <summary>
			/// Anchors min.
			/// </summary>
			public readonly Vector2 AnchorMin;

			/// <summary>
			/// Anchors max.
			/// </summary>
			public readonly Vector2 AnchorMax;

			/// <summary>
			/// Pivot.
			/// </summary>
			public readonly Vector2 Pivot;

			/// <summary>
			/// Value.
			/// </summary>
			public readonly float Value;

			/// <summary>
			/// Convert value to label.
			/// </summary>
			public readonly Func<float, string> Value2Label;

			/// <summary>
			/// Label.
			/// </summary>
			public string Label
			{
				get
				{
					return Value2Label != null ? Value2Label(Value) : Value.ToString();
				}
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="MarkData"/> struct.
			/// </summary>
			/// <param name="value">Value.</param>
			/// <param name="position">Position.</param>
			/// <param name="rotation">Rotation.</param>
			/// <param name="anchorMin">Anchors min.</param>
			/// <param name="anchorMax">Anchors max.</param>
			/// <param name="pivot">Pivot.</param>
			/// <param name="value2label">Convert value to label.</param>
			public MarkData(
				float value,
				Vector2 position = default(Vector2),
				float rotation = 0f,
				Vector2 anchorMin = default(Vector2),
				Vector2 anchorMax = default(Vector2),
				Vector2? pivot = null,
				Func<float, string> value2label = null)
			{
				Value = value;

				Position = position;
				Rotation = rotation;

				AnchorMin = anchorMin;
				AnchorMax = anchorMax;
				Pivot = pivot.HasValue ? pivot.Value : new Vector2(0.5f, 0.5f);

				Value2Label = value2label;
			}

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="obj">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public override bool Equals(object obj)
			{
				if (obj is MarkData)
				{
					return Equals((MarkData)obj);
				}

				return false;
			}

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="other">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public bool Equals(MarkData other)
			{
				return (Position == other.Position)
					&& (Rotation == other.Rotation)
					&& (AnchorMin == other.AnchorMin)
					&& (AnchorMax == other.AnchorMax)
					&& (Pivot == other.Pivot)
					&& (Value == other.Value)
					&& (Value2Label == other.Value2Label);
			}

			/// <summary>
			/// Hash function.
			/// </summary>
			/// <returns>A hash code for the current object.</returns>
			public override int GetHashCode()
			{
				return Position.GetHashCode() ^ Rotation.GetHashCode() ^ AnchorMin.GetHashCode() ^ AnchorMax.GetHashCode() ^ Pivot.GetHashCode() ^ Value.GetHashCode() ^ Value2Label.GetHashCode();
			}

			/// <summary>
			/// Compare specified instances.
			/// </summary>
			/// <param name="a">First instance.</param>
			/// <param name="b">Second instance.</param>
			/// <returns>true if the instances are equal; otherwise, false.</returns>
			public static bool operator ==(MarkData a, MarkData b)
			{
				return a.Equals(b);
			}

			/// <summary>
			/// Compare specified instances.
			/// </summary>
			/// <param name="a">First instance.</param>
			/// <param name="b">Second instance.</param>
			/// <returns>true if the instances not equal; otherwise, false.</returns>
			public static bool operator !=(MarkData a, MarkData b)
			{
				return !a.Equals(b);
			}
		}

		[SerializeField]
		RectTransform container;

		/// <summary>
		/// Container.
		/// </summary>
		public RectTransform Container
		{
			get
			{
				return container;
			}

			set
			{
				if (container != value)
				{
					container = value;
					NotifyPropertyChanged("Container");
				}
			}
		}

		[SerializeField]
		Image mainLine;

		/// <summary>
		/// Main line.
		/// </summary>
		public Image MainLine
		{
			get
			{
				return mainLine;
			}

			set
			{
				if (mainLine != value)
				{
					mainLine = value;
					NotifyPropertyChanged("MainLine");
				}
			}
		}

		[SerializeField]
		bool showCurrentValue = true;

		/// <summary>
		/// Show current values.
		/// </summary>
		public bool ShowCurrentValue
		{
			get
			{
				return showCurrentValue;
			}

			set
			{
				if (showCurrentValue != value)
				{
					showCurrentValue = value;
					NotifyPropertyChanged("ShowCurrentValue");
				}
			}
		}

		[SerializeField]
		[EditorConditionBool("showCurrentValue")]
		ScaleMarkTemplate currentMarkTemplate;

		/// <summary>
		/// Current mark template.
		/// </summary>
		public ScaleMarkTemplate CurrentMarkTemplate
		{
			get
			{
				return currentMarkTemplate;
			}

			set
			{
				if (currentMarkTemplate != value)
				{
					currentMarkTemplate = value;
					NotifyPropertyChanged("CurrentTemplate");
				}
			}
		}

		[SerializeField]
		bool showMinValue = true;

		/// <summary>
		/// Show minimal value.
		/// </summary>
		public bool ShowMinValue
		{
			get
			{
				return showMinValue;
			}

			set
			{
				if (showMinValue != value)
				{
					showMinValue = value;
					NotifyPropertyChanged("ShowMinValue");
				}
			}
		}

		[SerializeField]
		[EditorConditionBool("showMinValue")]
		ScaleMarkTemplate minMark;

		/// <summary>
		/// Min mark.
		/// </summary>
		public ScaleMarkTemplate MinMark
		{
			get
			{
				return minMark;
			}

			set
			{
				if (minMark != value)
				{
					minMark = value;
					NotifyPropertyChanged("MinTemplate");
				}
			}
		}

		[SerializeField]
		bool showMaxValue = true;

		/// <summary>
		/// Show max value.
		/// </summary>
		public bool ShowMaxValue
		{
			get
			{
				return showMaxValue;
			}

			set
			{
				if (showMaxValue != value)
				{
					showMaxValue = value;
					NotifyPropertyChanged("ShowMaxValue");
				}
			}
		}

		[SerializeField]
		[EditorConditionBool("showMaxValue")]
		ScaleMarkTemplate maxMark;

		/// <summary>
		/// Max mark.
		/// </summary>
		public ScaleMarkTemplate MaxMark
		{
			get
			{
				return maxMark;
			}

			set
			{
				if (maxMark != value)
				{
					maxMark = value;
					NotifyPropertyChanged("MaxTemplate");
				}
			}
		}

		[SerializeField]
		List<ScaleMark> scaleMarks = new List<ScaleMark>();

		ObservableList<ScaleMark> marks;

		/// <summary>
		/// Marks.
		/// </summary>
		public ObservableList<ScaleMark> Marks
		{
			get
			{
				if (marks == null)
				{
					marks = new ObservableList<ScaleMark>(scaleMarks);
					marks.Comparison = MarkComparison;
				}

				return marks;
			}

			set
			{
				if (marks != null)
				{
					marks.OnChange -= MarksChanged;
				}

				marks = value;

				if (marks != null)
				{
					marks.Comparison = MarkComparison;
					marks.OnChange += MarksChanged;
				}

				MarksChanged();
			}
		}

		RectTransform rectTransform;

		/// <summary>
		/// RectTransform.
		/// </summary>
		public RectTransform RectTransform
		{
			get
			{
				if (rectTransform == null)
				{
					rectTransform = transform as RectTransform;
				}

				return rectTransform;
			}
		}

		bool isInited;

		/// <summary>
		/// Items comparison.
		/// </summary>
		/// <param name="x">First item.</param>
		/// <param name="y">Second item.</param>
		/// <returns>Result of the comparison.</returns>
		public static int MarkComparison(ScaleMark x, ScaleMark y)
		{
			return -x.Step.CompareTo(y.Step);
		}

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected virtual void Start()
		{
			Init();
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected virtual void OnDestroy()
		{
			Clear();

			if (marks != null)
			{
				marks.OnChange -= MarksChanged;
				marks = null;
			}
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		public virtual void Init()
		{
			if (isInited)
			{
				return;
			}

			isInited = true;

			if (container == null)
			{
				container = transform as RectTransform;
			}

			if (currentMarkTemplate != null)
			{
				currentMarkTemplate.gameObject.SetActive(false);
			}

			if (minMark != null)
			{
				minMark.gameObject.SetActive(false);
			}

			if (maxMark != null)
			{
				maxMark.gameObject.SetActive(false);
			}

			if (mainLine != null)
			{
				mainLine.gameObject.SetActive(false);
			}

			for (int i = 0; i < Marks.Count; i++)
			{
				var mark = Marks[i];
				if (mark.Step <= 0)
				{
					Debug.LogWarning("ScaleMark.Step cannot be negative or zero.", this);
					continue;
				}

				mark.Template.gameObject.SetActive(false);
			}

			foreach (var instance in MarksInstances)
			{
				if (instance != null)
				{
					instance.Return();
				}
			}

			MarksInstances.Clear();
		}

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event OnChange OnChange;

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Mark values, temporary list.
		/// </summary>
		[HideInInspector]
		protected List<float> MarkValues = new List<float>();

		/// <summary>
		/// Mark values generator.
		/// </summary>
		public Action<float, float, float, List<float>> MarkValuesGenerator = DefaultGenerator;

		/// <summary>
		/// Used values.
		/// </summary>
		protected HashSet<float> UsedValues = new HashSet<float>();

		/// <summary>
		/// Marks instances.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<ScaleMarkTemplate> MarksInstances = new List<ScaleMarkTemplate>();

		/// <summary>
		/// Properties tracker.
		/// </summary>
		protected DrivenRectTransformTracker PropertiesTracker;

		/// <summary>
		/// Driven properties.
		/// </summary>
		protected DrivenTransformProperties DrivenProperties = DrivenTransformProperties.AnchoredPosition | DrivenTransformProperties.Anchors | DrivenTransformProperties.Pivot | DrivenTransformProperties.Rotation;

		/// <summary>
		/// Raise PropertyChanged event.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		protected virtual void NotifyPropertyChanged(string propertyName)
		{
			var c_handlers = OnChange;
			if (c_handlers != null)
			{
				c_handlers();
			}

			var handlers = PropertyChanged;
			if (handlers != null)
			{
				handlers(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		/// <summary>
		/// Process marks changed events.
		/// </summary>
		protected virtual void MarksChanged()
		{
			Clear();
			NotifyPropertyChanged("Marks");
		}

		/// <summary>
		/// Clear this instance.
		/// </summary>
		public virtual void Clear()
		{
			PropertiesTracker.Clear();

			UsedValues.Clear();

			if (CurrentMarkTemplate != null)
			{
				CurrentMarkTemplate.gameObject.SetActive(false);
			}

			if (MinMark != null)
			{
				MinMark.gameObject.SetActive(false);
			}

			if (MaxMark != null)
			{
				MaxMark.gameObject.SetActive(false);
			}

			foreach (var instance in MarksInstances)
			{
				if (instance != null)
				{
					instance.Return();
				}
			}

			MarksInstances.Clear();
		}

		/// <summary>
		/// Set min mark.
		/// </summary>
		/// <param name="mark">Mark data.</param>
		protected virtual void SetMin(MarkData mark)
		{
			if (MinMark == null)
			{
				return;
			}

			if (ShowMinValue)
			{
				MinMark.gameObject.SetActive(true);
				Set(MinMark, mark);
			}
			else
			{
				MinMark.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Set max mark.
		/// </summary>
		/// <param name="mark">Mark data.</param>
		protected virtual void SetMax(MarkData mark)
		{
			if (MaxMark == null)
			{
				return;
			}

			if (ShowMaxValue)
			{
				MaxMark.gameObject.SetActive(true);
				Set(MaxMark, mark);
			}
			else
			{
				MaxMark.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Set mark.
		/// </summary>
		/// <param name="instance">Instance.</param>
		/// <param name="mark">Mark data.</param>
		protected virtual void Set(ScaleMarkTemplate instance, MarkData mark)
		{
			var rt = instance.RectTransform;

			PropertiesTracker.Add(this, rt, DrivenProperties);

			rt.anchorMin = mark.AnchorMin;
			rt.anchorMax = mark.AnchorMax;
			rt.pivot = mark.Pivot;

			rt.anchoredPosition = mark.Position;
			rt.localRotation = Quaternion.Euler(0f, 0f, mark.Rotation);

			if (instance.Label != null)
			{
				instance.Label.text = mark.Label;

				var label_rt = instance.Label.transform as RectTransform;
				label_rt.localRotation = Quaternion.Euler(0f, 0f, -mark.Rotation);
			}
		}

		/// <summary>
		/// Default marks values generator.
		/// </summary>
		/// <param name="min">Min value.</param>
		/// <param name="max">Max value.</param>
		/// <param name="step">Step.</param>
		/// <param name="output">Result.</param>
		public static void DefaultGenerator(float min, float max, float step, List<float> output)
		{
			for (var value = min; value < max; value += step)
			{
				output.Add(value);
			}
		}

		/// <summary>
		/// Set scale data.
		/// </summary>
		/// <param name="value2mark">Convert value to mark data.</param>
		/// <param name="min">Min value.</param>
		/// <param name="max">Max value.</param>
		/// <param name="current">Current values.</param>
		public virtual void Set(Func<float, MarkData> value2mark, float min, float max, params float[] current)
		{
			Set(value2mark, min, max);

			if (ShowCurrentValue)
			{
				SetCurrent(value2mark, current);
			}
		}

		/// <summary>
		/// Set base scale data.
		/// </summary>
		/// <param name="value2mark">Convert value to mark data.</param>
		/// <param name="min">Min value.</param>
		/// <param name="max">Max value.</param>
		public virtual void Set(Func<float, MarkData> value2mark, float min, float max)
		{
			Init();
			Clear();

			if (MarkValuesGenerator == null)
			{
				MarkValuesGenerator = DefaultGenerator;
			}

			if (MainLine != null)
			{
				MainLine.gameObject.SetActive(true);
			}

			SetMin(value2mark(min));
			if (ShowMinValue)
			{
				UsedValues.Add(min);
			}

			SetMax(value2mark(max));
			if (ShowMinValue)
			{
				UsedValues.Add(max);
			}

			for (int i = 0; i < Marks.Count; i++)
			{
				var mark = Marks[i];
				if (mark.Step <= 0)
				{
					continue;
				}

				MarkValues.Clear();
				MarkValuesGenerator(min, max, mark.Step, MarkValues);

				foreach (var value in MarkValues)
				{
					if (UsedValues.Contains(value))
					{
						continue;
					}

					var instance = mark.Template.GetInstance(Container);
					Set(instance, value2mark(value));

					MarksInstances.Add(instance);
					UsedValues.Add(value);
				}

				MarkValues.Clear();
			}
		}

		/// <summary>
		/// Set scale data.
		/// </summary>
		/// <param name="value2mark">Convert value to mark data.</param>
		/// <param name="min">Min value.</param>
		/// <param name="max">Max value.</param>
		/// <param name="current">Current value 0.</param>
		public virtual void Set(Func<float, MarkData> value2mark, float min, float max, float current)
		{
			Set(value2mark, min, max);

			if (ShowCurrentValue)
			{
				SetCurrent(value2mark, current);
			}
		}

		/// <summary>
		/// Set scale data.
		/// </summary>
		/// <param name="value2mark">Convert value to mark data.</param>
		/// <param name="min">Min value.</param>
		/// <param name="max">Max value.</param>
		/// <param name="current0">Current value 0.</param>
		/// <param name="current1">Current value 1.</param>
		public virtual void Set(Func<float, MarkData> value2mark, float min, float max, float current0, float current1)
		{
			Set(value2mark, min, max);

			if (ShowCurrentValue)
			{
				SetCurrent(value2mark, current0);
				SetCurrent(value2mark, current1);
			}
		}

		/// <summary>
		/// Set scale data.
		/// </summary>
		/// <param name="value2mark">Convert value to mark data.</param>
		/// <param name="min">Min value.</param>
		/// <param name="max">Max value.</param>
		/// <param name="current0">Current value 0.</param>
		/// <param name="current1">Current value 1.</param>
		/// <param name="current2">Current value 2.</param>
		public virtual void Set(Func<float, MarkData> value2mark, float min, float max, float current0, float current1, float current2)
		{
			Set(value2mark, min, max);

			if (ShowCurrentValue)
			{
				SetCurrent(value2mark, current0);
				SetCurrent(value2mark, current1);
				SetCurrent(value2mark, current2);
			}
		}

		/// <summary>
		/// Set scale data.
		/// </summary>
		/// <param name="value2mark">Convert value to mark data.</param>
		/// <param name="min">Min value.</param>
		/// <param name="max">Max value.</param>
		/// <param name="current0">Current value 0.</param>
		/// <param name="current1">Current value 1.</param>
		/// <param name="current2">Current value 2.</param>
		/// <param name="current3">Current value 3.</param>
		public virtual void Set(Func<float, MarkData> value2mark, float min, float max, float current0, float current1, float current2, float current3)
		{
			Set(value2mark, min, max);

			if (ShowCurrentValue)
			{
				SetCurrent(value2mark, current0);
				SetCurrent(value2mark, current1);
				SetCurrent(value2mark, current2);
				SetCurrent(value2mark, current3);
			}
		}

		/// <summary>
		/// Set scale data.
		/// </summary>
		/// <param name="value2mark">Convert value to mark data.</param>
		/// <param name="min">Min value.</param>
		/// <param name="max">Max value.</param>
		/// <param name="current0">Current value 0.</param>
		/// <param name="current1">Current value 1.</param>
		/// <param name="current2">Current value 2.</param>
		/// <param name="current3">Current value 3.</param>
		/// <param name="current4">Current value 4.</param>
		public virtual void Set(Func<float, MarkData> value2mark, float min, float max, float current0, float current1, float current2, float current3, float current4)
		{
			Set(value2mark, min, max);

			if (ShowCurrentValue)
			{
				SetCurrent(value2mark, current0);
				SetCurrent(value2mark, current1);
				SetCurrent(value2mark, current2);
				SetCurrent(value2mark, current3);
				SetCurrent(value2mark, current4);
			}
		}

		/// <summary>
		/// Set current marks.
		/// </summary>
		/// <param name="value2mark">Convert value to mark data.</param>
		/// <param name="current">Current values.</param>
		protected virtual void SetCurrent(Func<float, MarkData> value2mark, params float[] current)
		{
			foreach (var value in current)
			{
				SetCurrent(value2mark, value);
			}
		}

		/// <summary>
		/// Set current mark.
		/// </summary>
		/// <param name="value2mark">Convert value to mark data.</param>
		/// <param name="current">Current value.</param>
		protected virtual void SetCurrent(Func<float, MarkData> value2mark, float current)
		{
			var instance = CurrentMarkTemplate.GetInstance(Container);
			Set(instance, value2mark(current));

			MarksInstances.Add(instance);
		}

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			style.Scale.MainLine.ApplyTo(MainLine);

			foreach (var instance in MarksInstances)
			{
				if (instance != null)
				{
					instance.SetStyle(style);
				}
			}

			if (currentMarkTemplate != null)
			{
				currentMarkTemplate.SetStyle(style);
			}

			if (minMark != null)
			{
				minMark.SetStyle(style);
			}

			if (maxMark != null)
			{
				maxMark.SetStyle(style);
			}

			if (isInited)
			{
				for (int i = 0; i < Marks.Count; i++)
				{
					Marks[i].Template.SetStyle(style);
				}
			}
			else
			{
				for (int i = 0; i < scaleMarks.Count; i++)
				{
					scaleMarks[i].Template.SetStyle(style);
				}
			}

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			style.Scale.MainLine.GetFrom(MainLine);

			if (currentMarkTemplate != null)
			{
				currentMarkTemplate.GetStyle(style);
			}
			else if (minMark != null)
			{
				minMark.GetStyle(style);
			}
			else if (maxMark != null)
			{
				maxMark.GetStyle(style);
			}
			else
			{
				for (int i = 0; i < scaleMarks.Count; i++)
				{
					scaleMarks[i].Template.GetStyle(style);
				}
			}

			return true;
		}
		#endregion
	}
}