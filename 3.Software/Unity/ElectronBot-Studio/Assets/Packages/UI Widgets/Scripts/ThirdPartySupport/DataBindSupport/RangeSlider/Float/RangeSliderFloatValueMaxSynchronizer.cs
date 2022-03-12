#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Synchronizers;
	using UnityEngine;

	/// <summary>
	/// Synchronizer for the ValueMax of a RangeSliderFloat.
	/// </summary>
	public class RangeSliderFloatValueMaxSynchronizer : ComponentDataSynchronizer<UIWidgets.RangeSliderFloat, float>
	{
		RangeSliderFloatValueMaxObserver observer;

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
				observer = new RangeSliderFloatValueMaxObserver
				{
					Target = target,
				};
				observer.ValueChanged += OnObserverValueChanged;
			}
		}

		/// <inheritdoc />
		protected override void SetTargetValue(UIWidgets.RangeSliderFloat target, float newContextValue)
		{
			target.ValueMax = newContextValue;
		}

		void OnObserverValueChanged()
		{
			this.OnComponentValueChanged(this.Target.ValueMax);
		}
	}
}
#endif