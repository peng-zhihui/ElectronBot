namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Style will not be applied for children gameobjects.
	/// </summary>
	[Serializable]
	public class StyleSupportStop : MonoBehaviour, IStylable
	{
		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			return true;
		}
		#endregion
	}
}