#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Synchronizers;
	using UnityEngine;

	/// <summary>
	/// Synchronizer for the ValueMin of a RangeSlider.
	/// </summary>
	public class RangeSliderValueMinSynchronizer : ComponentDataSynchronizer<UIWidgets.RangeSlider, int>
	{
		RangeSliderValueMinObserver observer;

		/// <inheritdoc />
		public override void Disable()
		{
			base.Disable();

			if (observer != null)
			{
				observer.ValueChanged -= OnObserverValueChanged;
				observer = null;
			}
		}

		/// <inheritdoc />
		public override void Enable()
		{
			base.Enable();

			var target = Target;
			if (target != null)
			{
				observer = new RangeSliderValueMinObserver
				{
					Target = target,
				};
				observer.ValueChanged += OnObserverValueChanged;
			}
		}

		/// <inheritdoc />
		protected override void SetTargetValue(UIWidgets.RangeSlider target, int newContextValue)
		{
			target.ValueMin = newContextValue;
		}

		void OnObserverValueChanged()
		{
			OnComponentValueChanged(Target.ValueMin);
		}
	}
}
#endif