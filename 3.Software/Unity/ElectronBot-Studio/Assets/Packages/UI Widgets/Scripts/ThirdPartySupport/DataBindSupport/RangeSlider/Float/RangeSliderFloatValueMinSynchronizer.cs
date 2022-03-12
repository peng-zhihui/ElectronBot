#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Synchronizers;
	using UnityEngine;

	/// <summary>
	/// Synchronizer for the ValueMin of a RangeSliderFloat.
	/// </summary>
	public class RangeSliderFloatValueMinSynchronizer : ComponentDataSynchronizer<UIWidgets.RangeSliderFloat, float>
	{
		RangeSliderFloatValueMinObserver observer;

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
				observer = new RangeSliderFloatValueMinObserver
				{
					Target = target,
				};
				observer.ValueChanged += OnObserverValueChanged;
			}
		}

		/// <inheritdoc />
		protected override void SetTargetValue(UIWidgets.RangeSliderFloat target, float newContextValue)
		{
			target.ValueMin = newContextValue;
		}

		void OnObserverValueChanged()
		{
			this.OnComponentValueChanged(this.Target.ValueMin);
		}
	}
}
#endif