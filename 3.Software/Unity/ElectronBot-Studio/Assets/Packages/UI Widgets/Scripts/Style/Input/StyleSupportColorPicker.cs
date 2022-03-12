namespace UIWidgets.Styles
{
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Style support for the ColorPicker.
	/// </summary>
	public class StyleSupportColorPicker : MonoBehaviour, IStylable
	{
		/// <summary>
		/// Style support for the palette toggle.
		/// </summary>
		[SerializeField]
		public StyleSupportButton PaletteToggle;

		/// <summary>
		/// Style support for the input toggle.
		/// </summary>
		[SerializeField]
		public StyleSupportButton InputToggle;

		/// <summary>
		/// The input channels labels.
		/// </summary>
		public List<GameObject> InputChannelLabels;

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			style.ColorPicker.PaletteToggle.ApplyTo(PaletteToggle);
			style.ColorPicker.InputToggle.ApplyTo(InputToggle);

			for (int i = 0; i < InputChannelLabels.Count; i++)
			{
				style.ColorPicker.InputChannelLabel.ApplyTo(InputChannelLabels[i]);
			}

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			style.ColorPicker.PaletteToggle.GetFrom(PaletteToggle);
			style.ColorPicker.InputToggle.GetFrom(InputToggle);

			for (int i = 0; i < InputChannelLabels.Count; i++)
			{
				style.ColorPicker.InputChannelLabel.GetFrom(InputChannelLabels[i]);
			}

			return true;
		}
		#endregion
	}
}