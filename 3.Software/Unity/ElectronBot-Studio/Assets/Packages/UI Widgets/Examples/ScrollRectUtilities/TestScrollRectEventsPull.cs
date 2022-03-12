namespace UIWidgets.Examples
{
	using System.Collections;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Test ScrollRectEvents.
	/// </summary>
	public class TestScrollRectEventsPull : MonoBehaviour, IUpgradeable
	{
		/// <summary>
		/// ListView to add items.
		/// </summary>
		[SerializeField]
		public ListViewIcons ListView;

		/// <summary>
		/// ScrollRectEvents.
		/// </summary>
		[SerializeField]
		public ScrollRectEvents PullEvents;

		/// <summary>
		/// Text component to display info.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with InfoAdapter.")]
		protected Text Info;

		/// <summary>
		/// Text component to display selected value.
		/// </summary>
		[SerializeField]
		protected TextAdapter InfoAdapter;

		/// <summary>
		/// Start this instance.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		public void Start()
		{
#pragma warning disable 0618
			ListView.Sort = false;
#pragma warning restore 0618

			PullEvents.OnPull.AddListener(Pull);
			PullEvents.OnPullAllowed.AddListener(PullAllowed);
			PullEvents.OnPullCancel.AddListener(PullCancel);
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected void OnDestroy()
		{
			if (PullEvents != null)
			{
				PullEvents.OnPull.RemoveListener(Pull);
				PullEvents.OnPullAllowed.RemoveListener(PullAllowed);
				PullEvents.OnPullCancel.RemoveListener(PullCancel);
			}
		}

		void Pull(ScrollRectEvents.PullDirection direction)
		{
			var items = ListView.DataSource;

			switch (direction)
			{
				case ScrollRectEvents.PullDirection.Up:
					items.Insert(
						0,
						new ListViewIconsItemDescription()
						{
							Name = string.Format("New item. Total: {0}", (items.Count + 1).ToString()),
						});

					InfoAdapter.text = "New item added.";
					break;
				case ScrollRectEvents.PullDirection.Down:
					StartCoroutine(AddItems());

					break;
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0401:Possible allocation of reference type enumerator", Justification = "Enumerator is reusable.")]
		IEnumerator AddItems()
		{
			InfoAdapter.text = "New item will added after 3 seconds.";

			yield return UtilitiesTime.Wait(3f, true);

			var items = ListView.DataSource;
			var item = new ListViewIconsItemDescription()
			{
				Name = string.Format("New item. Total: {0}", (items.Count + 1).ToString()),
			};
			items.Add(item);

			ListView.ScrollToAnimated(items.Count - 1);

			InfoAdapter.text = "New item added.";
		}

		void PullAllowed(ScrollRectEvents.PullDirection direction)
		{
			InfoAdapter.text = "Pull event will be raised on drag release.";
		}

		void PullCancel(ScrollRectEvents.PullDirection direction)
		{
			InfoAdapter.text = "Pull event canceled.";
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public virtual void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.GetOrAddComponent(Info, ref InfoAdapter);
#pragma warning restore 0612, 0618
		}

#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected virtual void OnValidate()
		{
			Compatibility.Upgrade(this);
		}
#endif
	}
}