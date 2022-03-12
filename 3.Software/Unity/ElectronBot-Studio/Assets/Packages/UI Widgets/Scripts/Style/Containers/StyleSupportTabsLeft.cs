namespace UIWidgets.Styles
{
	using UnityEngine;

	/// <summary>
	/// Style support for the tabs on left.
	/// </summary>
	[RequireComponent(typeof(IStylable<StyleTabs>))]
	public class StyleSupportTabsLeft : MonoBehaviour, IStylable
	{
		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			var component = Compatibility.GetComponent<IStylable<StyleTabs>>(gameObject);
			if (component != null)
			{
				component.SetStyle(style.TabsLeft, style);
			}

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			var component = Compatibility.GetComponent<IStylable<StyleTabs>>(gameObject);
			if (component != null)
			{
				component.GetStyle(style.TabsLeft, style);
			}

			return true;
		}
		#endregion
	}
}