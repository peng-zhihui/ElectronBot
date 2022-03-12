namespace UIWidgets.Examples
{
	using UnityEngine;

	/// <summary>
	/// LVInputFields test.
	/// </summary>
	[RequireComponent(typeof(LVInputFields))]
	public class LVInputFieldsTest : MonoBehaviour
	{
		LVInputFields list;

		/// <summary>
		/// ListView.
		/// </summary>
		public LVInputFields List
		{
			get
			{
				if (list == null)
				{
					list = GetComponent<LVInputFields>();
				}

				return list;
			}
		}

		/// <summary>
		/// Set first item text.
		/// </summary>
		public void Set()
		{
			List.DataSource[0].Text1 = "new text";
		}

		/// <summary>
		/// Get first item text.
		/// </summary>
		public void Get()
		{
			Debug.Log(List.DataSource[0].Text1);
		}

		/// <summary>
		/// Add new item.
		/// </summary>
		public void Add()
		{
			List.DataSource.Add(new LVInputFieldsItem() { Text1 = "String 1", Text2 = "String 2", IsOn = true });
		}

		/// <summary>
		/// Change data of item with specified index.
		/// </summary>
		/// <param name="index">Index.</param>
		public void ChangeItem(int index)
		{
			var item = List.DataSource[index];

			item.Text1 = "data changed 1";
			item.Text2 = "data changed 2";
			item.IsOn = false;
		}

		/// <summary>
		/// Display item data with specified index.
		/// </summary>
		/// <param name="index">Index.</param>
		public void Log(int index)
		{
			var item = List.DataSource[index];

			Debug.Log(string.Format("Text1: {0}; Text2: {1}, IsOn: {2}", item.Text1, item.Text2, item.IsOn.ToString()));
		}
	}
}