namespace UIWidgets
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Base class for the SnapGrid.
	/// </summary>
	public abstract partial class SnapGridBase : UIBehaviourConditional
	{
		/// <summary>
		/// Line at X axis.
		/// </summary>
		[Serializable]
		public struct LineX
		{
			[SerializeField]
			float x;

			/// <summary>
			/// X.
			/// </summary>
			public float X
			{
				get
				{
					return x;
				}
			}

			[SerializeField]
			bool snapLeft;

			/// <summary>
			/// Snap by target left side.
			/// </summary>
			public bool SnapLeft
			{
				get
				{
					return snapLeft;
				}
			}

			[SerializeField]
			bool snapRight;

			/// <summary>
			/// Snap by target right side.
			/// </summary>
			public bool SnapRight
			{
				get
				{
					return snapRight;
				}
			}

			/// <summary>
			/// Create instance.
			/// </summary>
			public static readonly Func<float, bool, bool, LineX> Create = (x, left, right) => new LineX(x, left, right);

			/// <summary>
			/// Initializes a new instance of the <see cref="LineX"/> struct.
			/// </summary>
			/// <param name="x">X.</param>
			/// <param name="snapLeft">Snap by target left side.</param>
			/// <param name="snapRight">Snap by target right side.</param>
			public LineX(float x, bool snapLeft = true, bool snapRight = true)
			{
				this.x = x;
				this.snapLeft = snapLeft;
				this.snapRight = snapRight;
			}
		}
	}
}