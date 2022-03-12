namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// List view with GameObjects.
	/// Outdated. Replaced with ListViewCustom. It's provide better interface and usability.
	/// </summary>
	[Obsolete("Outdated. Replaced with ListViewCustom. It's provide better interface and usability.")]
	public class ListViewGameObjects : ListViewBase
	{
		[SerializeField]
		List<GameObject> objects = new List<GameObject>();

		/// <summary>
		/// Gets the objects.
		/// </summary>
		/// <value>The objects.</value>
		public List<GameObject> Objects
		{
			get
			{
				return new List<GameObject>(objects);
			}

			protected set
			{
				UpdateItems(value);
			}
		}

		/// <summary>
		/// Sort function.
		/// </summary>
		public Func<IEnumerable<GameObject>, IEnumerable<GameObject>> SortFunc;

		/// <summary>
		/// What to do when the object selected.
		/// </summary>
		public ListViewGameObjectsEvent OnSelectObject = new ListViewGameObjectsEvent();

		/// <summary>
		/// What to do when the object deselected.
		/// </summary>
		public ListViewGameObjectsEvent OnDeselectObject = new ListViewGameObjectsEvent();

		[NonSerialized]
		bool isListViewGameObjectsInited = false;

		/// <summary>
		/// Init this instance.
		/// </summary>
		public override void Init()
		{
			if (isListViewGameObjectsInited)
			{
				return;
			}

			isListViewGameObjectsInited = true;

			base.Init();

			DestroyGameObjects = true;

			UpdateItems();

			OnSelect.AddListener(OnSelectCallback);
			OnDeselect.AddListener(OnDeselectCallback);
		}

		/// <summary>
		/// Process the select callback event.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="item">Item.</param>
		void OnSelectCallback(int index, ListViewItem item)
		{
			OnSelectObject.Invoke(index, objects[index]);
		}

		/// <summary>
		/// Process the deselect callback event.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="item">Item.</param>
		void OnDeselectCallback(int index, ListViewItem item)
		{
			OnDeselectObject.Invoke(index, objects[index]);
		}

		/// <summary>
		/// Updates the items.
		/// </summary>
		public override void UpdateItems()
		{
			UpdateItems(objects);
		}

		/// <summary>
		/// Add the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Index of added item.</returns>
		public virtual int Add(GameObject item)
		{
			var newObjects = Objects;
			newObjects.Add(item);
			UpdateItems(newObjects);

			var index = objects.IndexOf(item);

			return index;
		}

		/// <summary>
		/// Remove the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Index of removed item.</returns>
		public virtual int Remove(GameObject item)
		{
			var index = objects.IndexOf(item);
			if (index == -1)
			{
				return index;
			}

			var newObjects = Objects;
			newObjects.Remove(item);
			UpdateItems(newObjects);

			return index;
		}

		/// <summary>
		/// Sorts the items.
		/// </summary>
		/// <returns>The items.</returns>
		/// <param name="newItems">New items.</param>
		List<GameObject> SortItems(IEnumerable<GameObject> newItems)
		{
			var temp = newItems;
			if (SortFunc != null)
			{
				temp = SortFunc(temp);
			}

			return new List<GameObject>(temp);
		}

		/// <inheritdoc/>
		public override void Clear()
		{
			if (DestroyGameObjects)
			{
				foreach (var obj in objects)
				{
					Destroy(obj);
				}
			}

			UpdateItems(new List<GameObject>());
		}

		/// <inheritdoc/>
		public override void RemoveItemAt(int index)
		{
			objects.RemoveAt(index);
			UpdateItems(objects);
		}

		/// <summary>
		/// Updates the items.
		/// </summary>
		/// <param name="newItems">New items.</param>
		void UpdateItems(List<GameObject> newItems)
		{
			newItems = SortItems(newItems);

			var new_selected_indices = new List<int>();
			var old_selected_indices = SelectedIndicesList;

			foreach (var index in old_selected_indices)
			{
				var new_index = objects.Count > index ? newItems.IndexOf(objects[index]) : -1;
				if (new_index != -1)
				{
					new_selected_indices.Add(new_index);
				}
				else
				{
					Deselect(index);
				}
			}

			objects = newItems;
			Items.Clear();
			foreach (var item in newItems)
			{
				Items.Add(Utilities.GetOrAddComponent<ListViewItem>(item));
			}

			SelectedIndices = new_selected_indices;
		}

		/// <inheritdoc/>
		protected override void OnDestroy()
		{
			OnSelect.RemoveListener(OnSelectCallback);
			OnDeselect.RemoveListener(OnDeselectCallback);

			base.OnDestroy();
		}
	}
}