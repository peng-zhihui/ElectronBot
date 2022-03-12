namespace UIWidgets
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Component to display lines with distance from Target borders to parent borders.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	public class DistanceLines : MonoBehaviour
	{
		[SerializeField]
		DistanceLine left;

		/// <summary>
		/// Left line.
		/// </summary>
		public DistanceLine Left
		{
			get
			{
				return left;
			}

			set
			{
				left = value;

				if (left != null)
				{
					SetLineParent(left);
					UpdateLines();
				}
			}
		}

		[SerializeField]
		DistanceLine right;

		/// <summary>
		/// Right line.
		/// </summary>
		public DistanceLine Right
		{
			get
			{
				return right;
			}

			set
			{
				right = value;

				if (right != null)
				{
					SetLineParent(right);
					UpdateLines();
				}
			}
		}

		[SerializeField]
		DistanceLine top;

		/// <summary>
		/// Top line.
		/// </summary>
		public DistanceLine Top
		{
			get
			{
				return top;
			}

			set
			{
				top = value;

				if (top != null)
				{
					SetLineParent(top);
					UpdateLines();
				}
			}
		}

		[SerializeField]
		DistanceLine bottom;

		/// <summary>
		/// Bottom line.
		/// </summary>
		public DistanceLine Bottom
		{
			get
			{
				return bottom;
			}

			set
			{
				bottom = value;

				if (bottom != null)
				{
					SetLineParent(bottom);
					UpdateLines();
				}
			}
		}

		[SerializeField]
		[Tooltip("If disabled lines are drawn from parent border to the nearest Target border;\notherwise from parent border to the same Target border.")]
		bool allowIntersection = false;

		/// <summary>
		/// Allow lines intersection.
		/// If disabled lines are drawn from parent border to the nearest Target border; otherwise from parent border to the same Target border.
		/// </summary>
		public bool AllowIntersection
		{
			get
			{
				return allowIntersection;
			}

			set
			{
				if (allowIntersection != value)
				{
					allowIntersection = value;

					UpdateLines();
				}
			}
		}

		RectTransform target;

		/// <summary>
		/// Target.
		/// </summary>
		public RectTransform Target
		{
			get
			{
				if (target == null)
				{
					target = transform as RectTransform;
				}

				return target;
			}

			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}

				target = value;

				SetLinesParent();
			}
		}

		/// <summary>
		/// Starts this instance.
		/// </summary>
		protected virtual void Start()
		{
			Init();
		}

		bool isInited;

		/// <summary>
		/// Init this instance.
		/// </summary>
		public virtual void Init()
		{
			if (isInited)
			{
				return;
			}

			isInited = true;

			SetLinesParent();
			UpdateLines();
			Hide();
		}

		/// <summary>
		/// Set lines parent.
		/// </summary>
		protected void SetLinesParent()
		{
			SetLineParent(left);
			SetLineParent(right);
			SetLineParent(top);
			SetLineParent(bottom);
		}

		/// <summary>
		/// Set line parent.
		/// </summary>
		/// <param name="line">Line.</param>
		protected void SetLineParent(DistanceLine line)
		{
			if (line != null)
			{
				line.transform.SetParent(Target.parent, false);
			}
		}

		/// <summary>
		/// Show line.
		/// </summary>
		/// <param name="line">Line.</param>
		protected virtual void ShowLine(DistanceLine line)
		{
			if (line != null)
			{
				line.gameObject.SetActive(true);
			}
		}

		/// <summary>
		/// Hide line.
		/// </summary>
		/// <param name="line">Line.</param>
		protected virtual void HideLine(DistanceLine line)
		{
			if (line != null)
			{
				line.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Show lines.
		/// </summary>
		public virtual void Show()
		{
			Init();

			ShowLine(left);
			ShowLine(right);
			ShowLine(top);
			ShowLine(bottom);

			UpdateLines();
		}

		/// <summary>
		/// Update lines position and distance.
		/// </summary>
		public virtual void UpdateLines()
		{
			Init();

			var rect = GetRect(Target);
			var parent_size = (Target.transform.parent as RectTransform).rect.size;

			var angle = Target.localRotation.eulerAngles.z;
			if (angle > 180f)
			{
				angle -= 360f;
			}

			if (angle < -180f)
			{
				angle += 360f;
			}

			if (((angle >= -45f) && (angle <= 45f)) || AllowIntersection)
			{
				LinesN45P45(angle, rect, parent_size);
			}
			else if ((angle >= -135f) && (angle <= -45f))
			{
				LinesN135N45(angle, rect, parent_size);
			}
			else if ((angle >= 45f) && (angle <= 135f))
			{
				LinesP45P135(angle, rect, parent_size);
			}
			else
			{
				LinesP135N135(angle, rect, parent_size);
			}
		}

		/// <summary>
		/// Set lines positions and sizes for rotation in range -45..45.
		/// </summary>
		/// <param name="angle">Rotation.</param>
		/// <param name="target">Target.</param>
		/// <param name="parentSize">Parent size.</param>
		protected void LinesN45P45(float angle, Rect target, Vector2 parentSize)
		{
			var angle_rad = angle * Mathf.Deg2Rad;
			var cos = Mathf.Cos(angle_rad);
			var sin = Mathf.Sin(angle_rad);
			var half_size = new Vector2(target.width / 2f, target.height / 2f);
			var rotation_fix_h = new Vector2(half_size.x * cos, half_size.x * sin);
			var rotation_fix_v = new Vector2(half_size.y * cos, half_size.y * sin);

			SetLine(left, RectTransform.Edge.Left, target.x + half_size.x - rotation_fix_h.x, RectTransform.Edge.Bottom, target.y + half_size.y - rotation_fix_h.y);
			SetLine(right, RectTransform.Edge.Right, parentSize.x - target.xMax + half_size.x - rotation_fix_h.x, RectTransform.Edge.Bottom, target.y + half_size.y + rotation_fix_h.y);

			SetLine(top, RectTransform.Edge.Top, parentSize.y - target.yMax + half_size.y - rotation_fix_v.x, RectTransform.Edge.Left, target.x + half_size.x - rotation_fix_v.y);
			SetLine(bottom, RectTransform.Edge.Bottom, target.y + half_size.y - rotation_fix_v.x, RectTransform.Edge.Left, target.x + half_size.x + rotation_fix_v.y);
		}

		/// <summary>
		/// Set lines positions and sizes for rotation in range -135..-45.
		/// </summary>
		/// <param name="angle">Rotation.</param>
		/// <param name="target">Target.</param>
		/// <param name="parentSize">Parent size.</param>
		protected void LinesN135N45(float angle, Rect target, Vector2 parentSize)
		{
			angle += 180;
			LinesP45P135(angle, target, parentSize);
		}

		/// <summary>
		/// Set lines positions and sizes for rotation in range 45..135.
		/// </summary>
		/// <param name="angle">Rotation.</param>
		/// <param name="target">Target.</param>
		/// <param name="parentSize">Parent size.</param>
		protected void LinesP45P135(float angle, Rect target, Vector2 parentSize)
		{
			var angle_rad = angle * Mathf.Deg2Rad;
			var cos = Mathf.Cos(angle_rad);
			var sin = Mathf.Sin(angle_rad);
			var half_size = new Vector2(target.width / 2f, target.height / 2f);
			var rotation_fix_h = new Vector2(half_size.x * cos, half_size.x * sin);
			var rotation_fix_v = new Vector2(half_size.y * cos, half_size.y * sin);

			SetLine(left, RectTransform.Edge.Left, target.x + half_size.x - rotation_fix_v.y, RectTransform.Edge.Bottom, target.y + half_size.y + rotation_fix_v.x);
			SetLine(right, RectTransform.Edge.Right, parentSize.x - target.xMax + half_size.x - rotation_fix_v.y, RectTransform.Edge.Bottom, target.y + half_size.y - rotation_fix_v.x);

			SetLine(top, RectTransform.Edge.Top, parentSize.y - target.yMax + half_size.y - rotation_fix_h.y, RectTransform.Edge.Left, target.x + half_size.x + rotation_fix_h.x);
			SetLine(bottom, RectTransform.Edge.Bottom, target.y + half_size.y - rotation_fix_h.y, RectTransform.Edge.Left, target.x + half_size.x - rotation_fix_h.x);
		}

		/// <summary>
		/// Set lines positions and sizes for rotation in range -180..-135 and 135..180.
		/// </summary>
		/// <param name="angle">Rotation.</param>
		/// <param name="target">Target.</param>
		/// <param name="parentSize">Parent size.</param>
		protected void LinesP135N135(float angle, Rect target, Vector2 parentSize)
		{
			if (angle > 135)
			{
				angle -= 180;
			}

			if (angle < -135)
			{
				angle += 180;
			}

			LinesN45P45(angle, target, parentSize);
		}

		/// <summary>
		/// Set line position and distance.
		/// </summary>
		/// <param name="line">Line.</param>
		/// <param name="sizeEdge">Edge of the line to set size.</param>
		/// <param name="size">Size.</param>
		/// <param name="insetEdge">Inset edge.</param>
		/// <param name="inset">Inset.</param>
		protected virtual void SetLine(DistanceLine line, RectTransform.Edge sizeEdge, float size, RectTransform.Edge insetEdge, float inset)
		{
			if (line == null)
			{
				return;
			}

			var rt = line.RectTransform;
			var is_horizontal = (insetEdge == RectTransform.Edge.Left) || (insetEdge == RectTransform.Edge.Right);
			rt.SetInsetAndSizeFromParentEdge(sizeEdge, 0f, size);
			rt.SetInsetAndSizeFromParentEdge(insetEdge, inset, is_horizontal ? rt.rect.width : rt.rect.height);
			line.ShowDistance(size);
		}

		/// <summary>
		/// Get rect.
		/// </summary>
		/// <param name="rectTransform">RectTransform.</param>
		/// <returns>Rect.</returns>
		protected virtual Rect GetRect(RectTransform rectTransform)
		{
			var rect = rectTransform.rect;
			var pivot = rectTransform.pivot;
			var pos = rectTransform.localPosition;
			rect.x = pos.x - (pivot.x * rect.width);
			rect.y = pos.y - (pivot.y * rect.height);

			var parent = rectTransform.transform.parent as RectTransform;
			rect.x += parent.pivot.x * parent.rect.width;
			rect.y += parent.pivot.y * parent.rect.height;

			return rect;
		}

		/// <summary>
		/// Hide lines.
		/// </summary>
		public virtual void Hide()
		{
			HideLine(left);
			HideLine(right);
			HideLine(top);
			HideLine(bottom);
		}
	}
}