namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// TileView resize helper. Resize all items components when size one of them is changed.
	/// </summary>
	[RequireComponent(typeof(TileViewComponentSample))]
	[RequireComponent(typeof(Resizable))]
	public class TileViewResizeHelper : MonoBehaviour
	{
		/// <summary>
		/// TileView.
		/// </summary>
		[SerializeField]
		protected TileViewSample Tiles;

		/// <summary>
		/// Adds listeners.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected virtual void Start()
		{
			GetComponent<Resizable>().OnEndResize.AddListener(OnResize);
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected virtual void OnDestroy()
		{
			var r = GetComponent<Resizable>();
			if (r != null)
			{
				r.OnEndResize.AddListener(OnResize);
			}
		}

		/// <summary>
		/// Handle resize event.
		/// </summary>
		/// <param name="item">Item.</param>
		protected virtual void OnResize(Resizable item)
		{
			var size = item.RectTransform.rect.size;
			foreach (var component in Tiles.GetComponentsEnumerator(PoolEnumeratorMode.All))
			{
				var current_size = component.RectTransform.rect.size;

				if (current_size.x != size.x)
				{
					component.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
				}

				if (current_size.y != size.y)
				{
					component.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
				}
			}

			Tiles.Resize();
		}
	}
}