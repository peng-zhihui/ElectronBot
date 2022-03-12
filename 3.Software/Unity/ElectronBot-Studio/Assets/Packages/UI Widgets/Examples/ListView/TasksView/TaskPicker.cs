namespace UIWidgets.Examples.Tasks
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Task Picker.
	/// </summary>
	public class TaskPicker : Picker<Task, TaskPicker>
	{
		/// <summary>
		/// TaskView.
		/// </summary>
		[SerializeField]
		public TaskView TaskView;

		/// <summary>
		/// Set initial value and add callback.
		/// </summary>
		/// <param name="defaultValue">Default value.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		public override void BeforeOpen(Task defaultValue)
		{
			// set default value
			TaskView.SelectedIndex = TaskView.DataSource.IndexOf(defaultValue);

			// add callback
			TaskView.OnSelectObject.AddListener(Select);
		}

		/// <summary>
		/// Callback when value selected.
		/// </summary>
		/// <param name="index">Selected item index.</param>
		void Select(int index)
		{
			// apply selected value and close picker
			Selected(TaskView.DataSource[index]);
		}

		/// <summary>
		/// Remove listener.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		public override void BeforeClose()
		{
			// remove callback
			TaskView.OnSelectObject.RemoveListener(Select);
		}
	}
}