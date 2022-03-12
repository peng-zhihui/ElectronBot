namespace EasyLayoutNS
{
	using System;
	using System.ComponentModel;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// Flex settings.
	/// </summary>
	[Serializable]
	public class EasyLayoutFlexSettings : IObservable, INotifyPropertyChanged
	{
		/// <summary>
		/// Content positions.
		/// </summary>
		[Serializable]
		public enum Content
		{
			/// <summary>
			/// Position at the start of the block.
			/// </summary>
			Start = 0,

			/// <summary>
			/// Position at the center of the block.
			/// </summary>
			Center = 1,

			/// <summary>
			/// Position at the end of the block.
			/// </summary>
			End = 2,

			/// <summary>
			/// Position with space between.
			/// </summary>
			SpaceBetween = 3,

			/// <summary>
			/// Position with space around.
			/// </summary>
			SpaceAround = 4,

			/// <summary>
			/// Position with space evenly.
			/// </summary>
			SpaceEvenly = 5,
		}

		/// <summary>
		/// Items align.
		/// </summary>
		[Serializable]
		public enum Items
		{
			/// <summary>
			/// Start position.
			/// </summary>
			Start = 0,

			/// <summary>
			/// Center position.
			/// </summary>
			Center = 1,

			/// <summary>
			/// End position.
			/// </summary>
			End = 2,
		}

		[SerializeField]
		[FormerlySerializedAs("Wrap")]
		bool wrap = true;

		/// <summary>
		/// Wrap.
		/// </summary>
		public bool Wrap
		{
			get
			{
				return wrap;
			}

			set
			{
				if (wrap != value)
				{
					wrap = value;
					NotifyPropertyChanged("Wrap");
				}
			}
		}

		[SerializeField]
		[FormerlySerializedAs("JustifyContent")]
		Content justifyContent = Content.Start;

		/// <summary>
		/// Elements positions at the main axis.
		/// </summary>
		public Content JustifyContent
		{
			get
			{
				return justifyContent;
			}

			set
			{
				if (justifyContent != value)
				{
					justifyContent = value;
					NotifyPropertyChanged("JustifyContent");
				}
			}
		}

		[SerializeField]
		[FormerlySerializedAs("AlignContent")]
		Content alignContent = Content.Start;

		/// <summary>
		/// Elements positions at the sub axis.
		/// </summary>
		public Content AlignContent
		{
			get
			{
				return alignContent;
			}

			set
			{
				if (alignContent != value)
				{
					alignContent = value;
					NotifyPropertyChanged("AlignContent");
				}
			}
		}

		[SerializeField]
		[FormerlySerializedAs("AlignItems")]
		Items alignItems = Items.Start;

		/// <summary>
		/// Items align.
		/// </summary>
		public Items AlignItems
		{
			get
			{
				return alignItems;
			}

			set
			{
				if (alignItems != value)
				{
					alignItems = value;
					NotifyPropertyChanged("AlignItems");
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
		/// Property changed.
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
		/// Get debug information.
		/// </summary>
		/// <param name="sb">String builder.</param>
		public virtual void GetDebugInfo(System.Text.StringBuilder sb)
		{
			sb.Append("\tWrap: ");
			sb.Append(Wrap);
			sb.AppendLine();

			sb.Append("\tJustify Content: ");
			sb.Append(EnumHelper<Content>.ToString(JustifyContent));
			sb.AppendLine();

			sb.Append("\tAlign Content: ");
			sb.Append(EnumHelper<Content>.ToString(AlignContent));
			sb.AppendLine();

			sb.Append("\tAlign Items: ");
			sb.Append(EnumHelper<Items>.ToString(AlignItems));
			sb.AppendLine();
		}
	}
}