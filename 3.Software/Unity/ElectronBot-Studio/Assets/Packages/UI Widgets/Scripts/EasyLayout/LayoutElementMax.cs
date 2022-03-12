namespace EasyLayoutNS
{
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Allow to control the maximum preferred sizes of the LayoutGroup.
	/// </summary>
	[RequireComponent(typeof(LayoutGroup))]
	public class LayoutElementMax : UIBehaviour, ILayoutElement, ILayoutIgnorer
	{
		LayoutGroup layoutGroup;

		/// <summary>
		/// Layout Group.
		/// </summary>
		protected LayoutGroup Layout
		{
			get
			{
				if (layoutGroup == null)
				{
					layoutGroup = GetComponent<LayoutGroup>();
				}

				return layoutGroup;
			}
		}

		[SerializeField]
		[FormerlySerializedAs("_ignoreLayout")]
		bool pignoreLayout = false;

		[SerializeField]
		[FormerlySerializedAs("_layoutPriority")]
		int playoutPriority = 1;

		[SerializeField]
		float maxWidth = -1f;

		[SerializeField]
		float maxHeight = -1f;

		/// <summary>
		/// Maximum preferred height for the Layout Group.
		/// </summary>
		public virtual float MaxHeight
		{
			get
			{
				return maxHeight;
			}

			set
			{
				if (maxHeight != value)
				{
					maxHeight = value;
					SetDirty();
				}
			}
		}

		/// <summary>
		/// Maximum preferred width for the Layout Group.
		/// </summary>
		public virtual float MaxWidth
		{
			get
			{
				return maxWidth;
			}

			set
			{
				if (maxWidth != value)
				{
					maxWidth = value;
					SetDirty();
				}
			}
		}

		/// <summary>
		/// Should this RectTransform be ignored by the layout system?
		/// </summary>
		/// <remarks>
		/// Setting this property to true will make a parent layout group component not consider this RectTransform part of the group. The RectTransform can then be manually positioned despite being a child GameObject of a layout group.
		/// </remarks>
		public bool ignoreLayout
		{
			get
			{
				return pignoreLayout;
			}

			set
			{
				if (pignoreLayout != value)
				{
					pignoreLayout = value;
					SetDirty();
				}
			}
		}

		/// <summary>
		/// The Priority of layout this element has.
		/// </summary>
		public int layoutPriority
		{
			get
			{
				return playoutPriority;
			}

			set
			{
				playoutPriority = value;
			}
		}

		/// <summary>
		/// The minimum height this layout element may be allocated.
		/// </summary>
		public float minHeight
		{
			get
			{
				return -1;
			}
		}

		/// <summary>
		/// The minimum width this layout element may be allocated.
		/// </summary>
		public float minWidth
		{
			get
			{
				return -1;
			}
		}

		/// <summary>
		/// The preferred width this layout element should be allocated if there is sufficient space. The preferredWidth can be set to -1 to remove the size.
		/// </summary>
		public float preferredHeight
		{
			get
			{
				return Mathf.Min(Layout.preferredHeight, MaxHeight);
			}
		}

		/// <summary>
		/// The preferred width this layout element should be allocated if there is sufficient space. The preferredWidth can be set to -1 to remove the size.
		/// </summary>
		public float preferredWidth
		{
			get
			{
				return Mathf.Min(Layout.preferredWidth, MaxWidth);
			}
		}

		/// <summary>
		/// The extra relative height this layout element should be allocated if there is additional available space.
		/// </summary>
		public float flexibleHeight
		{
			get
			{
				return -1;
			}
		}

		/// <summary>
		/// The extra relative width this layout element should be allocated if there is additional available space.
		/// </summary>
		public float flexibleWidth
		{
			get
			{
				return -1;
			}
		}

		/// <summary>
		/// Calculates the layout input horizontal.
		/// </summary>
		public void CalculateLayoutInputHorizontal()
		{
		}

		/// <summary>
		/// Calculates the layout input vertical.
		/// </summary>
		public void CalculateLayoutInputVertical()
		{
		}

		/// <summary>
		/// Process instance enable event.
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();
			SetDirty();
		}

		/// <summary>
		/// Process transform parent changed event.
		/// </summary>
		protected override void OnTransformParentChanged()
		{
			SetDirty();
		}

		/// <summary>
		/// Process instance disable event.
		/// </summary>
		protected override void OnDisable()
		{
			SetDirty();
			base.OnDisable();
		}

		/// <summary>
		/// Process property animation event.
		/// </summary>
		protected override void OnDidApplyAnimationProperties()
		{
			SetDirty();
		}

		/// <summary>
		/// Process before transform parent changed event.
		/// </summary>
		protected override void OnBeforeTransformParentChanged()
		{
			SetDirty();
		}

		/// <summary>
		/// Mark the LayoutElement as dirty.
		/// </summary>
		/// <remarks>
		/// This will make the auto layout system process this element on the next layout pass. This method should be called by the LayoutElement whenever a change is made that potentially affects the layout.
		/// </remarks>
		protected void SetDirty()
		{
			if (!IsActive())
			{
				return;
			}

			LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
		}

#if UNITY_EDITOR
		/// <summary>
		/// Process validation event.
		/// </summary>
		protected override void OnValidate()
		{
			SetDirty();
		}
#endif
	}
}