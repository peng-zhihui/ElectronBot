namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.Events;

	/// <summary>
	/// Transform listener.
	/// </summary>
	[ExecuteInEditMode]
	public class TransformListener : MonoBehaviour, IUpdatable
	{
		/// <summary>
		/// The transform changed event.
		/// </summary>
		public UnityEvent OnTransformChanged = new UnityEvent();

		/// <summary>
		/// Update is called every frame, if the MonoBehaviour is enabled.
		/// </summary>
		public virtual void RunUpdate()
		{
			if (transform.hasChanged)
			{
				OnTransformChanged.Invoke();
				transform.hasChanged = false;
			}
		}

		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		protected virtual void OnEnable()
		{
			if (Application.isPlaying)
			{
				Updater.Add(this);
			}

			OnTransformChanged.Invoke();
		}

		/// <summary>
		/// This function is called when the behavior becomes disabled or inactive.
		/// </summary>
		protected virtual void OnDisable()
		{
			if (Application.isPlaying)
			{
				Updater.Remove(this);
			}

			OnTransformChanged.Invoke();
		}
	}
}