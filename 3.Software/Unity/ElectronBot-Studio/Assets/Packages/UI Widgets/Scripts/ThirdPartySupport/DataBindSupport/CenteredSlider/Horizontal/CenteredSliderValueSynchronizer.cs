#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Synchronizers;
	using UnityEngine;

	/// <summary>
	/// Synchronizer for the Value of a CenteredSlider.
	/// </summary>
	public class CenteredSliderValueSynchronizer : ComponentDataSynchronizer<UIWidgets.CenteredSlider, int>
	{
		CenteredSliderValueObserver observer;

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
				observer = new CenteredSliderValueObserver
				{
					Target = target,
				};
				observer.ValueChanged += OnObserverValueChanged;
			}
		}

		/// <inheritdoc />
		protected override void SetTargetValue(UIWidgets.CenteredSlider target, int newContextValue)
		{
			target.Value = newContextValue;
		}

		void OnObserverValueChanged()
		{
			OnComponentValueChanged(Target.Value);
		}
	}
}
#endif