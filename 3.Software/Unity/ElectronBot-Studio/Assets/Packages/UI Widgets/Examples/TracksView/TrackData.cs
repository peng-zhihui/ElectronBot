namespace UIWidgets.Examples
{
	using System;
	using System.ComponentModel;
	using UnityEngine;

	/// <summary>
	/// Track data.
	/// </summary>
	[Serializable]
	public class TrackData : ITrackData<DateTime>, IObservable, INotifyPropertyChanged
	{
		[SerializeField]
		DateTime startPoint;

		/// <summary>
		/// Start point.
		/// </summary>
		public DateTime StartPoint
		{
			get
			{
				return startPoint;
			}

			set
			{
				if (startPoint != value)
				{
					startPoint = value;
					NotifyPropertyChanged("StartPoint");
				}
			}
		}

		[SerializeField]
		DateTime endPoint;

		/// <summary>
		/// End point.
		/// </summary>
		public DateTime EndPoint
		{
			get
			{
				return endPoint;
			}

			set
			{
				if (endPoint != value)
				{
					endPoint = value;
					NotifyPropertyChanged("EndPoint");
				}
			}
		}

		[SerializeField]
		[HideInInspector]
		int order;

		/// <summary>
		/// Order.
		/// </summary>
		public int Order
		{
			get
			{
				return order;
			}

			set
			{
				if (order != value)
				{
					order = value;
					fixedOrder = false;
				}
			}
		}

		[SerializeField]
		[HideInInspector]
		bool fixedOrder;

		/// <summary>
		/// Is order fixed?
		/// </summary>
		public bool FixedOrder
		{
			get
			{
				return fixedOrder;
			}

			set
			{
				if (fixedOrder != value)
				{
					fixedOrder = value;
				}
			}
		}

		[SerializeField]
		string name;

		/// <summary>
		/// Name.
		/// </summary>
		public string Name
		{
			get
			{
				return name;
			}

			set
			{
				if (name != value)
				{
					name = value;
					NotifyPropertyChanged("Name");
				}
			}
		}

		[SerializeField]
		string description;

		/// <summary>
		/// Description.
		/// </summary>
		public string Description
		{
			get
			{
				return description;
			}

			set
			{
				if (description != value)
				{
					description = value;
					NotifyPropertyChanged("Description");
				}
			}
		}

		/// <summary>
		/// Is item dragged?
		/// </summary>
		public bool IsDragged
		{
			get;
			set;
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
		/// Raise PropertyChanged event.
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

		/// <summary>
		/// Get new EndPoint by specified StartPoint to maintain same length.
		/// </summary>
		/// <param name="newStart">New start point.</param>
		/// <returns>New EndPoint value.</returns>
		public DateTime EndPointByStartPoint(DateTime newStart)
		{
			return newStart + (EndPoint - StartPoint);
		}

		/// <summary>
		/// Set StartPoint and EndPoint to maintain same length.
		/// </summary>
		/// <param name="newStart">New start point.</param>
		/// <param name="newEnd">New end point.</param>
		public void SetPoints(DateTime newStart, DateTime newEnd)
		{
			StartPoint = newStart;
			EndPoint = endPoint;
		}

		/// <summary>
		/// Change StartPoint and EndPoint.
		/// </summary>
		/// <param name="newStart">New StartPoint.</param>
		/// <param name="newEnd">New EndPoint.</param>
		public void ChangePoints(DateTime newStart, DateTime newEnd)
		{
			var changed = (startPoint != newStart) || (endPoint != newEnd);
			if (changed)
			{
				startPoint = newStart;
				endPoint = newEnd;
				NotifyPropertyChanged("StartPoint");
			}
		}

		/// <summary>
		/// Copy data to target.
		/// </summary>
		/// <param name="target">Target.</param>
		public void CopyTo(TrackData target)
		{
			target.StartPoint = StartPoint;
			target.EndPoint = EndPoint;
			target.Name = Name;
			target.Description = Description;
		}

		/// <summary>
		/// Convert this instance to string.
		/// </summary>
		/// <returns>String representation.</returns>
		public override string ToString()
		{
			return Name;
		}
	}
}