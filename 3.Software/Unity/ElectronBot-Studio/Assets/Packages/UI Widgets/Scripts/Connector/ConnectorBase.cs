namespace UIWidgets
{
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Connector base class.
	/// </summary>
	[RequireComponent(typeof(TransformListener))]
	public abstract class ConnectorBase : MaskableGraphic, IStylable
	{
		[SerializeField]
		Sprite sprite;

		/// <summary>
		/// Gets or sets the sprite.
		/// </summary>
		/// <value>The sprite.</value>
		public Sprite Sprite
		{
			get
			{
				return sprite;
			}

			set
			{
				sprite = value;
				SetAllDirty();
			}
		}

		/// <summary>
		/// Image's texture comes from the UnityEngine.Image.
		/// </summary>
		public override Texture mainTexture
		{
			get
			{
				return sprite == null ? s_WhiteTexture : sprite.texture;
			}
		}

		TransformListener currentTransformListener;

		/// <summary>
		/// Root canvas.
		/// </summary>
		protected Canvas RootCanvas
		{
			get
			{
				if (canvas == null)
				{
					return null;
				}

				if (canvas.isRootCanvas)
				{
					return canvas;
				}

				return canvas.rootCanvas;
			}
		}

		ILineBuilder builder = new LineBuilder();

		/// <summary>
		/// Line builder.
		/// </summary>
		public ILineBuilder Builder
		{
			get
			{
				return builder;
			}

			set
			{
				builder = value;
				SetAllDirty();
			}
		}

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected override void Start()
		{
			base.Start();

			Init();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		protected virtual void Init()
		{
			AddSelfListener();
		}

		/// <summary>
		/// Adds the self listener.
		/// </summary>
		protected void AddSelfListener()
		{
			if (currentTransformListener == null)
			{
				currentTransformListener = GetComponent<TransformListener>();
			}

			if (currentTransformListener != null)
			{
				currentTransformListener.OnTransformChanged.AddListener(SetVerticesDirty);
			}
		}

		/// <summary>
		/// Removes the self listener.
		/// </summary>
		protected void RemoveSelfListener()
		{
			if (currentTransformListener != null)
			{
				currentTransformListener.OnTransformChanged.RemoveListener(SetVerticesDirty);
			}
		}

#if UNITY_EDITOR
		/// <summary>
		/// This function is called when the script is loaded or a value is changed in the inspector (Called in the editor only).
		/// </summary>
		protected override void OnValidate()
		{
			RemoveSelfListener();
			AddSelfListener();
			SetVerticesDirty();

			base.OnValidate();
		}
#endif

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected override void OnDestroy()
		{
			RemoveSelfListener();
			base.OnDestroy();
		}

		/// <summary>
		/// Gets the point.
		/// </summary>
		/// <returns>The point.</returns>
		/// <param name="targetRectTransform">Target RectTransform.</param>
		/// <param name="position">Position.</param>
		public Vector3 GetPoint(RectTransform targetRectTransform, ConnectorPosition position)
		{
			var rect = GetPixelAdjustedRect();

			Vector3 delta;

			if (RootCanvas.renderMode == RenderMode.ScreenSpaceCamera)
			{
				var camera = RootCanvas.worldCamera;
				delta = camera.WorldToScreenPoint(rectTransform.position) - camera.WorldToScreenPoint(targetRectTransform.position);

				if (canvas.scaleFactor != 0f)
				{
					delta /= canvas.scaleFactor;
				}
			}
			else
			{
				delta = rectTransform.position - targetRectTransform.position;

				if (RootCanvas.renderMode == RenderMode.WorldSpace)
				{
					var canvas_scale = (RootCanvas.transform as RectTransform).lossyScale;

					if (canvas_scale.x != 0)
					{
						delta.x /= canvas_scale.x;
					}

					if (canvas_scale.y != 0)
					{
						delta.y /= canvas_scale.y;
					}
				}
				else
				{
					if (canvas.scaleFactor != 0f)
					{
						delta /= canvas.scaleFactor;
					}
				}
			}

			rect.x -= delta.x;
			rect.y -= delta.y;

			switch (position)
			{
				case ConnectorPosition.Left:
					rect.y += rect.height / 2f;
					break;
				case ConnectorPosition.Right:
					rect.x += rect.width;
					rect.y += rect.height / 2f;
					break;
				case ConnectorPosition.Top:
					rect.x += rect.width / 2f;
					rect.y += rect.height;
					break;
				case ConnectorPosition.Bottom:
					rect.x += rect.width / 2f;
					break;
				case ConnectorPosition.Center:
					rect.x += rect.width / 2f;
					rect.y += rect.height / 2f;
					break;
			}

			return new Vector3(rect.x, rect.y, 0);
		}

		/// <summary>
		/// Adds the line.
		/// </summary>
		/// <returns>The line.</returns>
		/// <param name="source">Source.</param>
		/// <param name="line">Line.</param>
		/// <param name="vh">Vertex helper.</param>
		/// <param name="index">Index.</param>
		public int AddLine(RectTransform source, ConnectorLine line, VertexHelper vh, int index)
		{
			if (RootCanvas == null)
			{
				return 0;
			}

			return Builder.Build(this, source, line, vh, index);
		}

		/// <summary>
		/// Rotates the point.
		/// </summary>
		/// <returns>The point.</returns>
		/// <param name="point">Point.</param>
		/// <param name="pivot">Pivot.</param>
		/// <param name="angle">Angle.</param>
		public static Vector3 RotatePoint(Vector3 point, Vector3 pivot, Quaternion angle)
		{
			var direction = angle * (point - pivot);

			return direction + pivot;
		}

		#region IStylable

		/// <inheritdoc/>
		public bool SetStyle(Style style)
		{
			color = style.Connector.Color;
			material = style.Connector.Material;
			Sprite = style.Connector.Sprite;

			return true;
		}

		/// <inheritdoc/>
		public bool GetStyle(Style style)
		{
			style.Connector.Color = color;
			style.Connector.Sprite = Sprite;
			Style.SetValue(material, ref style.Connector.Material);

			return true;
		}
		#endregion
	}
}