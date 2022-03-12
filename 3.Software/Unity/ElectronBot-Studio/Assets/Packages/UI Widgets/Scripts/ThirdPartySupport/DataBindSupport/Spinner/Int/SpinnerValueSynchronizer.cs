#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Synchronizers;
	using UnityEngine;

	/// <summary>
	/// Synchronizer for the Value of a Spinner.
	/// </summary>
	public class SpinnerValueSynchronizer : ComponentDataSynchronizer<UIWidgets.Spinner, int>
	{
		SpinnerValueObserver observer;

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
				observer = new SpinnerValueObserver
				{
					Target = target,
				};
				observer.ValueChanged += OnObserverValueChanged;
			}
		}

		/// <inheritdoc />
		protected override void SetTargetValue(UIWidgets.Spinner target, int newContextValue)
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