#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Synchronizers;
	using UnityEngine;

	/// <summary>
	/// Synchronizer for the Time of a TimeBase.
	/// </summary>
	public class TimeBaseTimeSynchronizer : ComponentDataSynchronizer<UIWidgets.TimeBase, System.TimeSpan>
	{
		TimeBaseTimeObserver observer;

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
				observer = new TimeBaseTimeObserver
				{
					Target = target,
				};
				observer.ValueChanged += OnObserverValueChanged;
			}
		}

		/// <inheritdoc />
		protected override void SetTargetValue(UIWidgets.TimeBase target, System.TimeSpan newContextValue)
		{
			target.Time = newContextValue;
		}

		void OnObserverValueChanged()
		{
			this.OnComponentValueChanged(this.Target.Time);
		}
	}
}
#endif