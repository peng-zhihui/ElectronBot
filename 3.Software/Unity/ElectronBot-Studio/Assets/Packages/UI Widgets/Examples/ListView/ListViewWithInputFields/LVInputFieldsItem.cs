namespace UIWidgets.Examples
{
	using System;
	using System.ComponentModel;
	using UnityEngine;

	/// <summary>
	/// LVInputFields item.
	/// </summary>
	[Serializable]
	public class LVInputFieldsItem : IObservable, INotifyPropertyChanged
	{
		[SerializeField]
		string text1;

		/// <summary>
		/// Text1.
		/// </summary>
		public string Text1
		{
			get
			{
				return text1;
			}

			set
			{
				if (text1 != value)
				{
					text1 = value;

					// when value changed will be raised PropertyChanged event
					Changed("Text1");
				}
			}
		}

		[SerializeField]
		string text2;

		/// <summary>
		/// Text2.
		/// </summary>
		public string Text2
		{
			get
			{
				return text2;
			}

			set
			{
				if (text2 != value)
				{
					text2 = value;
					Changed("Text2");
				}
			}
		}

		[SerializeField]
		bool isOn;

		/// <summary>
		/// IsOn.
		/// </summary>
		public bool IsOn
		{
			get
			{
				return isOn;
			}

			set
			{
				if (isOn != value)
				{
					isOn = value;
					Changed("IsOn");
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

		void Changed(string propertyName)
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