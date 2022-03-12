namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// Tsunami effect.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	[AddComponentMenu("UI/New UI Widgets/Effects/Tsunami Effect")]
	public class TsunamiEffect : MonoBehaviour, IUpdatable
	{
		/// <summary>
		/// Minimal size when distance to cursor more than Distance.
		/// </summary>
		public Vector2 MinSize = new Vector2(200f, 40f);

		/// <summary>
		/// Maximum size when cursor over gameobject.
		/// </summary>
		public Vector2 MaxSize = new Vector2(250f, 60f);

		/// <summary>
		/// Distance when size changed from MinSize to MaxSize.
		/// </summary>
		[FormerlySerializedAs("OnDistance")]
		public float Distance = 100f;

		/// <summary>
		/// Scroll compensation.
		/// </summary>
		protected TsunamiEffectScrollCompensation ScrollCompensation;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			ScrollCompensation = transform.parent.GetComponent<TsunamiEffectScrollCompensation>();
		}

		/// <summary>
		/// Process the enable event.
		/// </summary>
		protected virtual void OnEnable()
		{
			Updater.Add(this);
		}

		/// <summary>
		/// Process the disable event.
		/// </summary>
		protected virtual void OnDisable()
		{
			Updater.Remove(this);
		}

		/// <summary>
		/// Update.
		/// </summary>
		public virtual void RunUpdate()
		{
			var rt = transform as RectTransform;
			var canvas = UtilitiesUI.FindCanvas(rt).GetComponent<Canvas>();

			Vector2 point;
			float distance;

			var camera = canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : null;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, CompatibilityInput.MousePosition, camera, out point);

			var old_size = rt.rect.size;
			var half_size = old_size / 2f;
			var delta_x = point.x - Mathf.Max(Mathf.Min(point.x, half_size.x), -half_size.x);
			var delta_y = point.y - Mathf.Max(Mathf.Min(point.y, half_size.y), -half_size.y);

			distance = Mathf.Sqrt(Mathf.Pow(delta_x, 2) + Mathf.Pow(delta_y, 2));
			distance = Mathf.Clamp(distance, 0f, Distance);

			var new_size = Vector2.Lerp(MaxSize, MinSize, distance / Distance);
			new_size.x = Mathf.Round(new_size.x);
			new_size.y = Mathf.Round(new_size.y);

			if ((old_size.x != new_size.x) || (old_size.y != new_size.y))
			{
				rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Round(new_size.x));
				rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Round(new_size.y));
			}

			if (ScrollCompensation != null)
			{
				var compensation = new_size - MinSize;
				compensation.x *= Mathf.Clamp(point.x / half_size.x, -1f, 1f);
				compensation.y *= Mathf.Clamp(point.y / half_size.y, -1f, 1f);

				ScrollCompensation.Add(compensation);
			}
		}
	}
}