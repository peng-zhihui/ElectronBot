namespace UIWidgets.Examples
{
	using System.Collections.Generic;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// How to save selected indices for ListView.
	/// </summary>
	[RequireComponent(typeof(ListViewBase))]
	public class ListViewSaveIndices : MonoBehaviour
	{
		/// <summary>
		/// Key.
		/// </summary>
		[SerializeField]
		public string Key = "Unique Key";

		[SerializeField]
		ListViewBase list;

		/// <summary>
		/// Process the start event.
		/// Load saved indices and adds listeners.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected virtual void Start()
		{
			list = GetComponent<ListViewBase>();
			list.Init();

			LoadIndices();

			list.OnSelect.AddListener(SaveIndices);
			list.OnDeselect.AddListener(SaveIndices);
		}

		/// <summary>
		/// Process the destroy event.
		/// Remove listeners.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected virtual void OnDestroy()
		{
			if (list != null)
			{
				list.OnSelect.RemoveListener(SaveIndices);
				list.OnDeselect.RemoveListener(SaveIndices);
			}
		}

		/// <summary>
		/// Load indices.
		/// </summary>
		protected virtual void LoadIndices()
		{
			if (PlayerPrefs.HasKey(Key))
			{
				var indices = String2Indices(PlayerPrefs.GetString(Key));
				for (int i = indices.Count - 1; i >= 0; i--)
				{
					if (!list.IsValid(indices[i]))
					{
						indices.RemoveAt(i);
					}
				}

				list.SelectedIndices = indices;
			}
		}

		/// <summary>
		/// Save indices.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="component">Component.</param>
		protected virtual void SaveIndices(int index, ListViewItem component)
		{
			PlayerPrefs.SetString(Key, Indices2String(list.SelectedIndices));
		}

		static char[] Separator = new char[] { ';' };

		static List<int> String2Indices(string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return new List<int>();
			}

			var result = new List<int>();

			foreach (var index in str.Split(Separator))
			{
				result.Add(int.Parse(index));
			}

			return result;
		}

		static string Indices2String(List<int> indices)
		{
			if ((indices == null) || (indices.Count == 0))
			{
				return string.Empty;
			}

			var arr = new string[indices.Count];

			for (int i = 0; i < indices.Count; i++)
			{
				arr[i] = indices[i].ToString();
			}

			return string.Join(Separator[0].ToString(), arr);
		}
	}
}