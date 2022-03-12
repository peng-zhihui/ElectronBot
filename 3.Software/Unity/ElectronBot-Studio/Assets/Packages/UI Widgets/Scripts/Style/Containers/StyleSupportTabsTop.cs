namespace UIWidgets.Styles
{
	using UnityEngine;

	/// <summary>
	/// Style support for the tabs on top.
	/// </summary>
	public class StyleSupportTabsTop : MonoBehaviour, IStylable
	{
		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			var component = Compatibility.GetComponent<IStylable<StyleTabs>>(gameObject);
			if (component != null)
			{
				component.SetStyle(style.TabsTop, style);
			}

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			var component = Compatibility.GetComponent<IStylable<StyleTabs>>(gameObject);
			if (component != null)
			{
				component.GetStyle(style.TabsTop, style);
			}

			return true;
		}
		#endregion
	}
}