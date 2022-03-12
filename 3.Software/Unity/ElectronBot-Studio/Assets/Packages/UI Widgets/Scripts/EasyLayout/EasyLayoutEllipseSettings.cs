namespace EasyLayoutNS
{
	using System;
	using System.ComponentModel;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Settings for the staggered layout.
	/// </summary>
	[Serializable]
	public class EasyLayoutEllipseSettings : IObservable, INotifyPropertyChanged
	{
		[SerializeField]
		private bool widthAuto = true;

		/// <summary>
		/// Calculate with or not.
		/// </summary>
		public bool WidthAuto
		{
			get
			{
				return widthAuto;
			}

			set
			{
				if (widthAuto != value)
				{
					widthAuto = value;
					NotifyPropertyChanged("WidthAuto");
				}
			}
		}

		[SerializeField]
		private float width;

		/// <summary>
		/// Width.
		/// </summary>
		public float Width
		{
			get
			{
				return width;
			}

			set
			{
				if (width != value)
				{
					width = value;
					NotifyPropertyChanged("Width");
				}
			}
		}

		[SerializeField]
		private bool heightAuto = true;

		/// <summary>
		/// Calculate height or not.
		/// </summary>
		public bool HeightAuto
		{
			get
			{
				return heightAuto;
			}

			set
			{
				if (heightAuto != value)
				{
					heightAuto = value;
					NotifyPropertyChanged("HeightAuto");
				}
			}
		}

		[SerializeField]
		private float height;

		/// <summary>
		/// Height.
		/// </summary>
		public float Height
		{
			get
			{
				return height;
			}

			set
			{
				if (height != value)
				{
					height = value;
					NotifyPropertyChanged("Height");
				}
			}
		}

		[SerializeField]
		private float angleStart;

		/// <summary>
		/// Angle for the display first element.
		/// </summary>
		public float AngleStart
		{
			get
			{
				return angleStart;
			}

			set
			{
				if (angleStart != value)
				{
					angleStart = value;
					NotifyPropertyChanged("AngleStart");
				}
			}
		}

		[SerializeField]
		private bool angleStepAuto;

		/// <summary>
		/// Calculate or not AngleStep.
		/// </summary>
		public bool AngleStepAuto
		{
			get
			{
				return angleStepAuto;
			}

			set
			{
				if (angleStepAuto != value)
				{
					angleStepAuto = value;
					NotifyPropertyChanged("AngleStepAuto");
				}
			}
		}

		[SerializeField]
		private float angleStep = 20f;

		/// <summary>
		/// Angle distance between elements.
		/// </summary>
		public float AngleStep
		{
			get
			{
				return angleStep;
			}

			set
			{
				if (angleStep != value)
				{
					angleStep = value;
					NotifyPropertyChanged("AngleStep");
				}
			}
		}

		[SerializeField]
		private EllipseFill fill = EllipseFill.Closed;

		/// <summary>
		/// Fill type.
		/// </summary>
		public EllipseFill Fill
		{
			get
			{
				return fill;
			}

			set
			{
				if (fill != value)
				{
					fill = value;
					NotifyPropertyChanged("Fill");
				}
			}
		}

		[SerializeField]
		private float arcLength = 360f;

		/// <summary>
		/// Arc length.
		/// </summary>
		public float ArcLength
		{
			get
			{
				return arcLength;
			}

			set
			{
				if (arcLength != value)
				{
					arcLength = value;
					NotifyPropertyChanged("Length");
				}
			}
		}

		[SerializeField]
		private EllipseAlign align;

		/// <summary>
		/// Align.
		/// </summary>
		public EllipseAlign Align
		{
			get
			{
				return align;
			}

			set
			{
				if (align != value)
				{
					align = value;
					NotifyPropertyChanged("Align");
				}
			}
		}

		[SerializeField]
		[HideInInspector]
		private float angleScroll;

		/// <summary>
		/// Angle padding.
		/// </summary>
		public float AngleScroll
		{
			get
			{
				return angleScroll;
			}

			set
			{
				if (angleScroll != value)
				{
					angleScroll = value;
					NotifyPropertyChanged("AngleScroll");
				}
			}
		}

		[SerializeField]
		[HideInInspector]
		private float angleFiller;

		/// <summary>
		/// Angle filler.
		/// </summary>
		public float AngleFiller
		{
			get
			{
				return angleFiller;
			}

			set
			{
				if (angleFiller != value)
				{
					angleFiller = value;
					NotifyPropertyChanged("AngleFiller");
				}
			}
		}

		[SerializeField]
		private bool elementsRotate = true;

		/// <summary>
		/// Rotate elements.
		/// </summary>
		public bool ElementsRotate
		{
			get
			{
				return elementsRotate;
			}

			set
			{
				if (elementsRotate != value)
				{
					elementsRotate = value;
					NotifyPropertyChanged("ElementsRotate");
				}
			}
		}

		[SerializeField]
		private float elementsRotationStart;

		/// <summary>
		/// Start rotation for elements.
		/// </summary>
		public float ElementsRotationStart
		{
			get
			{
				return elementsRotationStart;
			}

			set
			{
				if (elementsRotationStart != value)
				{
					elementsRotationStart = value;
					NotifyPropertyChanged("ElementsRotationStart");
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
			sb.Append("\tWidth Auto: ");
			sb.Append(WidthAuto);
			sb.AppendLine();

			sb.Append("\tWidth: ");
			sb.Append(Width);
			sb.AppendLine();

			sb.Append("\tHeight Auto: ");
			sb.Append(HeightAuto);
			sb.AppendLine();

			sb.Append("\tHeight: ");
			sb.Append(Height);
			sb.AppendLine();

			sb.Append("\tAngle Start: ");
			sb.Append(AngleStart);
			sb.AppendLine();

			sb.Append("\tAngle Step Auto: ");
			sb.Append(AngleStepAuto);
			sb.AppendLine();

			sb.Append("\tAngle Step: ");
			sb.Append(AngleStep);
			sb.AppendLine();

			sb.Append("\tAlign: ");
			sb.Append(EnumHelper<EllipseAlign>.ToString(Align));
			sb.AppendLine();

			sb.Append("\tElements Rotate: ");
			sb.Append(ElementsRotate);
			sb.AppendLine();

			sb.Append("\tElements Rotation Start: ");
			sb.Append(ElementsRotationStart);
			sb.AppendLine();

			sb.AppendLine("\t#####");

			sb.Append("\tFill: ");
			sb.Append(EnumHelper<EllipseFill>.ToString(Fill));
			sb.AppendLine();

			sb.Append("\tAngle Filler: ");
			sb.Append(AngleFiller);
			sb.AppendLine();

			sb.Append("\tAngle Scroll: ");
			sb.Append(AngleScroll);
			sb.AppendLine();

			sb.Append("\tArc Length: ");
			sb.Append(ArcLength);
			sb.AppendLine();
		}
	}
}