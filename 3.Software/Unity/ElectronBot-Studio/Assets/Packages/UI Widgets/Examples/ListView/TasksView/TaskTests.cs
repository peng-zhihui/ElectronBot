namespace UIWidgets.Examples.Tasks
{
	using System.Collections;
	using UnityEngine;

	/// <summary>
	/// Task test.
	/// </summary>
	public class TaskTests : MonoBehaviour
	{
		/// <summary>
		/// TaskView.
		/// </summary>
		public TaskView Tasks;

		/// <summary>
		/// Add task.
		/// </summary>
		public void AddTask()
		{
			var task = new Task() { Name = "Random Task", Progress = 0 };

			Tasks.DataSource.Add(task);

			StartCoroutine(UpdateProgress(task, 1f, Random.Range(1, 10)));
		}

		/// <summary>
		/// Update progress.
		/// </summary>
		/// <param name="task">Task.</param>
		/// <param name="time">Time.</param>
		/// <param name="delta">Delta.</param>
		/// <returns>IEnumerator.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0401:Possible allocation of reference type enumerator", Justification = "Enumerator is reusable.")]
		protected virtual IEnumerator UpdateProgress(Task task, float time, int delta)
		{
			while (task.Progress < 100)
			{
				yield return UtilitiesTime.Wait(time, true);
				task.Progress = Mathf.Min(task.Progress + delta, 100);
			}
		}
	}
}