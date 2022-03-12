namespace UIWidgets
{
	using System;

	/// <summary>
	/// Base class for the SnapGrid.
	/// </summary>
	public abstract partial class SnapGridBase : UIBehaviourConditional
	{
		/// <summary>
		/// Distance value to find value nearest to 0.
		/// </summary>
		protected struct DistanceValue
		{
			/// <summary>
			/// Absolute value.
			/// </summary>
			public float Abs
			{
				get;
				private set;
			}

			/// <summary>
			/// Value.
			/// </summary>
			public float Value
			{
				get;
				private set;
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="DistanceValue"/> struct.
			/// </summary>
			/// <param name="defaultValue">Default value.</param>
			public DistanceValue(float defaultValue)
			{
				Abs = defaultValue;
				Value = defaultValue;
			}

			/// <summary>
			/// Update value.
			/// </summary>
			/// <param name="newValue">New value.</param>
			public void Update(float newValue)
			{
				var new_value_abs = Math.Abs(newValue);
				if (new_value_abs < Abs)
				{
					Abs = new_value_abs;
					Value = newValue;
				}
			}
		}
	}
}