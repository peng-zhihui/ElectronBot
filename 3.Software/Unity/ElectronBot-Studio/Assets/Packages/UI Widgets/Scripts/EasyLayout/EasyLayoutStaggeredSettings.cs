namespace EasyLayoutNS
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// Settings for the staggered layout.
	/// </summary>
	[Serializable]
	public class EasyLayoutStaggeredSettings : IObservable, INotifyPropertyChanged
	{
		[SerializeField]
		[FormerlySerializedAs("FixedBlocksCount")]
		[Tooltip("Layout with fixed amount of blocks (row or columns) instead of the flexible.")]
		private bool fixedBlocksCount;

		/// <summary>
		/// Use fixed blocks count.
		/// </summary>
		public bool FixedBlocksCount
		{
			get
			{
				return fixedBlocksCount;
			}

			set
			{
				if (fixedBlocksCount != value)
				{
					fixedBlocksCount = value;
					NotifyPropertyChanged("FixedBlocksCount");
				}
			}
		}

		[SerializeField]
		[FormerlySerializedAs("BlocksCount")]
		private int blocksCount = 1;

		/// <summary>
		/// Block (row or columns) count.
		/// </summary>
		public int BlocksCount
		{
			get
			{
				return blocksCount;
			}

			set
			{
				if (blocksCount != value)
				{
					blocksCount = value;
					NotifyPropertyChanged("BlocksCount");
				}
			}
		}

		/// <summary>
		/// PaddingInner at the start of the blocks.
		/// Used by ListViewCustom to simulate the space occupied by non-displayable elements.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		public List<float> PaddingInnerStart = new List<float>();

		/// <summary>
		/// PaddingInner at the end of the blocks.
		/// Used by ListViewCustom to simulate the space occupied by non-displayable elements.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		public List<float> PaddingInnerEnd = new List<float>();

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
			sb.Append("\tFixed Blocks Count: ");
			sb.Append(FixedBlocksCount);
			sb.AppendLine();

			sb.Append("\tBlocks Count: ");
			sb.Append(BlocksCount);
			sb.AppendLine();

			sb.AppendLine("\t#####");

			sb.Append("\tPadding Inner Start: ");
			sb.Append(EasyLayoutUtilities.List2String(PaddingInnerStart));
			sb.AppendLine();

			sb.Append("\tPadding Inner End: ");
			sb.Append(EasyLayoutUtilities.List2String(PaddingInnerEnd));
			sb.AppendLine();
		}
	}
}