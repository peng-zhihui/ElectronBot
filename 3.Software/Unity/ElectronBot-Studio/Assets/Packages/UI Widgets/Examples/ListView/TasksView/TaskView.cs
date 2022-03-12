namespace UIWidgets.Examples.Tasks
{
	using UIWidgets;

	/// <summary>
	/// TaskView.
	/// </summary>
	public class TaskView : ListViewCustom<TaskComponent, Task>
	{
		/// <summary>
		/// Tasks comparison.
		/// </summary>
		static readonly System.Comparison<Task> ItemsComparison = (x, y) => UtilitiesCompare.Compare(x.Name, y.Name);

		bool isTaskViewInited;

		/// <summary>
		/// Init this instance.
		/// </summary>
		public override void Init()
		{
			if (isTaskViewInited)
			{
				return;
			}

			isTaskViewInited = true;

			base.Init();

			DataSource.Comparison = ItemsComparison;
		}
	}
}