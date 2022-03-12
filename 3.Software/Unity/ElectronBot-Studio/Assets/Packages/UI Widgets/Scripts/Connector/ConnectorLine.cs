namespace UIWidgets
{
	using System;
	using System.ComponentModel;
	using UnityEngine;

	/// <summary>
	/// Connector line.
	/// </summary>
	[Serializable]
	public class ConnectorLine : IObservable, INotifyPropertyChanged
	{
		[SerializeField]
		RectTransform target;

		/// <summary>
		/// Gets or sets the target.
		/// </summary>
		/// <value>The target.</value>
		public RectTransform Target
		{
			get
			{
				return target;
			}

			set
			{
				target = value;
				NotifyPropertyChanged("Target");
			}
		}

		[SerializeField]
		ConnectorPosition start = ConnectorPosition.Right;

		/// <summary>
		/// Gets or sets the start.
		/// </summary>
		/// <value>The start.</value>
		public ConnectorPosition Start
		{
			get
			{
				return start;
			}

			set
			{
				start = value;
				NotifyPropertyChanged("Start");
			}
		}

		[SerializeField]
		ConnectorPosition end = ConnectorPosition.Left;

		/// <summary>
		/// Gets or sets the end.
		/// </summary>
		/// <value>The end.</value>
		public ConnectorPosition End
		{
			get
			{
				return end;
			}

			set
			{
				end = value;
				NotifyPropertyChanged("End");
			}
		}

		[SerializeField]
		[HideInInspector]
		ConnectorArrow arrow = ConnectorArrow.None;

		/// <summary>
		/// Gets or sets the arrow.
		/// </summary>
		/// <value>The arrow.</value>
		public ConnectorArrow Arrow
		{
			get
			{
				return arrow;
			}

			set
			{
				arrow = value;
				NotifyPropertyChanged("Arrow");
			}
		}

		[SerializeField]
		ConnectorType type;

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>The type.</value>
		public ConnectorType Type
		{
			get
			{
				return type;
			}

			set
			{
				type = value;
				NotifyPropertyChanged("Type");
			}
		}

		[SerializeField]
		float thickness = 1f;

		/// <summary>
		/// Gets or sets the thickness.
		/// </summary>
		/// <value>The thickness.</value>
		public float Thickness
		{
			get
			{
				return thickness;
			}

			set
			{
				thickness = value;
				NotifyPropertyChanged("Thickness");
			}
		}

		[SerializeField]
		float margin = 10f;

		/// <summary>
		/// Gets or sets the margin.
		/// </summary>
		/// <value>The margin.</value>
		public float Margin
		{
			get
			{
				return margin;
			}

			set
			{
				margin = value;
				NotifyPropertyChanged("Margin");
			}
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
		/// Changed the specified propertyName.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		protected void NotifyPropertyChanged(string propertyName)
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
	}
}