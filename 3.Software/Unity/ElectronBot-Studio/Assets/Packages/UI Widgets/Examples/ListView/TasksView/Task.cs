namespace UIWidgets.Examples.Tasks
{
	using System;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// Task.
	/// </summary>
	[Serializable]
	public class Task
	{
		/// <summary>
		/// Name.
		/// </summary>
		public string Name;

		[SerializeField]
		[FormerlySerializedAs("Progress")]
		int progress;

		/// <summary>
		/// Progress.
		/// </summary>
		public int Progress
		{
			get
			{
				return progress;
			}

			set
			{
				progress = value;
				OnProgressChange();
			}
		}

		/// <summary>
		/// Progress changed event.
		/// </summary>
		public event OnChange OnProgressChange = () => { };
	}
}