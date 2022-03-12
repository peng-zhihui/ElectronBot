namespace UIWidgets.Examples
{
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// Test ListViewVariableHeight.
	/// </summary>
	public class TestListViewVariableHeight : MonoBehaviour
	{
		/// <summary>
		/// ListViewVariableHeight.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("list")]
		protected ListViewVariableHeight ListView;

		/// <summary>
		/// Add item.
		/// </summary>
		public void Add()
		{
			ListView.DataSource.Add(new ListViewVariableHeightItemDescription()
			{
				Name = "Added",
				Text = "Test\nTest\ntest3",
			});
		}
	}
}