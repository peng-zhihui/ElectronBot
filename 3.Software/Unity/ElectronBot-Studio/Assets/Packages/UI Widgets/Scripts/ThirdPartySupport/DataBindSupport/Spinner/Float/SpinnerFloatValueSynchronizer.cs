#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Synchronizers;
	using UnityEngine;

	/// <summary>
	/// Synchronizer for the Value of a SpinnerFloat.
	/// </summary>
	public class SpinnerFloatValueSynchronizer : ComponentDataSynchronizer<UIWidgets.SpinnerFloat, float>
	{
		SpinnerFloatValueObserver observer;

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
				observer = new SpinnerFloatValueObserver
				{
					Target = target,
				};
				observer.ValueChanged += OnObserverValueChanged;
			}
		}

		/// <inheritdoc />
		protected override void SetTargetValue(UIWidgets.SpinnerFloat target, float newContextValue)
		{
			target.Value = newContextValue;
		}

		void OnObserverValueChanged()
		{
			this.OnComponentValueChanged(this.Target.Value);
		}
	}
}
#endif