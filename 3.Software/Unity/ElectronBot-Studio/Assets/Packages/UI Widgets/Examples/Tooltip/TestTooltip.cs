namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Test tooltip.
	/// </summary>
	public class TestTooltip : MonoBehaviour
	{
		/// <summary>
		/// The tooltip.
		/// </summary>
		[SerializeField]
		protected TooltipString Tooltip;

		/// <summary>
		/// The target.
		/// </summary>
		[SerializeField]
		protected GameObject Target;

		/// <summary>
		/// Test this instance.
		/// </summary>
		public void Test()
		{
			Tooltip.Register(Target, "Script Tooltip", new TooltipSettings(TooltipPosition.TopCenter));
		}
	}
}