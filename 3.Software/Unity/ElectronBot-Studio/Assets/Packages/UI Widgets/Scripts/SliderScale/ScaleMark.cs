namespace UIWidgets
{
	using System;
	using System.ComponentModel;
	using UnityEngine;

	/// <summary>
	/// Scale mark.
	/// </summary>
	[Serializable]
	public class ScaleMark : IObservable, INotifyPropertyChanged
	{
		[SerializeField]
		float step;

		/// <summary>
		/// Step.
		/// </summary>
		public float Step
		{
			get
			{
				return step;
			}

			set
			{
				if (step != value)
				{
					step = value;
					NotifyPropertyChanged("Step");
				}
			}
		}

		[SerializeField]
		ScaleMarkTemplate template;

		/// <summary>
		/// Template.
		/// </summary>
		public ScaleMarkTemplate Template
		{
			get
			{
				return template;
			}

			set
			{
				if (template != value)
				{
					template = value;
					NotifyPropertyChanged("Template");
				}
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
	}
}