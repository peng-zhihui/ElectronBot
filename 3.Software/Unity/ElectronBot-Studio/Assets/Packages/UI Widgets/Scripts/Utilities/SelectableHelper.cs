namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// SelectableHelper.
	/// Allow to control additional Graphic component according selection state of current gameobject.
	/// </summary>
	[RequireComponent(typeof(Selectable))]
	[AddComponentMenu("UI/New UI Widgets/Helpers/Selectable Helper")]
	[ExecuteInEditMode]
	public class SelectableHelper : UIBehaviour,
		IPointerDownHandler, IPointerUpHandler,
		IPointerEnterHandler, IPointerExitHandler,
		ISelectHandler, IDeselectHandler
	{
		/// <summary>
		/// Selection states.
		/// </summary>
		public enum SelectionState
		{
			/// <summary>
			/// Normal.
			/// </summary>
			Normal,

			/// <summary>
			/// Highlighted.
			/// </summary>
			Highlighted,

			/// <summary>
			/// Pressed.
			/// </summary>
			Pressed,

			/// <summary>
			/// Disabled.
			/// </summary>
			Disabled,
		}

		[SerializeField]
		Selectable.Transition transition = Selectable.Transition.ColorTint;

		[SerializeField]
		ColorBlock colors = ColorBlock.defaultColorBlock;

		[SerializeField]
		SpriteState spriteState;

		[SerializeField]
		AnimationTriggers animationTriggers = new AnimationTriggers();

		[SerializeField]
		Graphic targetGraphic;

		SelectionState currentSelectionState;

		/// <summary>
		/// The type of transition that will be applied to the targetGraphic when the state changes.
		/// </summary>
		/// <value>The transition.</value>
		public Selectable.Transition Transition
		{
			get
			{
				return transition;
			}

			set
			{
				if (transition != value)
				{
					transition = value;
					OnSetProperty();
				}
			}
		}

		/// <summary>
		/// The ColorBlock for this object.
		/// </summary>
		/// <value>The colors.</value>
		public ColorBlock Colors
		{
			get
			{
				return colors;
			}

			set
			{
				if (!colors.Equals(value))
				{
					colors = value;
					OnSetProperty();
				}
			}
		}

		/// <summary>
		/// The SpriteState for this object.
		/// </summary>
		/// <value>The state of the sprite.</value>
		public SpriteState SpriteState
		{
			get
			{
				return spriteState;
			}

			set
			{
				if (!spriteState.Equals(value))
				{
					spriteState = value;
					OnSetProperty();
				}
			}
		}

		/// <summary>
		/// The AnimationTriggers for this object.
		/// </summary>
		/// <value>The animation triggers.</value>
		public AnimationTriggers AnimationTriggers
		{
			get
			{
				return animationTriggers;
			}

			set
			{
				animationTriggers = value;
				OnSetProperty();
			}
		}

		/// <summary>
		/// Graphic that will be transitioned upon.
		/// </summary>
		/// <value>The target graphic.</value>
		public Graphic TargetGraphic
		{
			get
			{
				return targetGraphic;
			}

			set
			{
				targetGraphic = value;
				OnSetProperty();
			}
		}

		Selectable parent;

		/// <summary>
		/// Selectable component.
		/// </summary>
		/// <value>The parent.</value>
		protected Selectable Parent
		{
			get
			{
				if (parent == null)
				{
					parent = GetComponent<Selectable>();
				}

				return parent;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this object is interactable.
		/// </summary>
		/// <value><c>true</c> if interactable; otherwise, <c>false</c>.</value>
		public bool Interactable
		{
			get
			{
				return (Parent != null) && Parent.IsInteractable();
			}
		}

		bool isPointerInside;

		bool isPointerDown;

		bool hasSelection;

		/// <summary>
		/// Convenience function that converts the referenced Graphic to a Image, if possible.
		/// </summary>
		/// <value>The image.</value>
		public Image Image
		{
			get
			{
				return TargetGraphic as Image;
			}

			set
			{
				TargetGraphic = value;
			}
		}

		/// <summary>
		/// Convenience function to get the Animator component on the GameObject.
		/// </summary>
		/// <value>The animator.</value>
		public Animator Animator
		{
			get
			{
				return GetComponent<Animator>();
			}
		}

		/// <summary>
		/// Awake this instance.
		/// </summary>
		protected override void Awake()
		{
			if (TargetGraphic == null)
			{
				TargetGraphic = GetComponent<Graphic>();
			}
		}

		/// <summary>
		/// Determines whether this instance is interactable.
		/// </summary>
		/// <returns><c>true</c> if this instance is interactable; otherwise, <c>false</c>.</returns>
		public virtual bool IsInteractable()
		{
			return Interactable;
		}

		/// <summary>
		/// Callback for when properties have been changed by animation.
		/// </summary>
		protected override void OnDidApplyAnimationProperties()
		{
			OnSetProperty();
		}

		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();

			currentSelectionState = hasSelection ? SelectionState.Highlighted : SelectionState.Normal;
			TransitionToSelectionState(true);
		}

		void OnSetProperty()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				TransitionToSelectionState(true);
			}
			else
#endif
			{
				TransitionToSelectionState(false);
			}
		}

		/// <summary>
		/// This function is called when the behaviour becomes disabled or inactive.
		/// </summary>
		protected override void OnDisable()
		{
			InstantClearState();
			base.OnDisable();
		}

#if UNITY_EDITOR
		/// <summary>
		/// This function is called when the script is loaded or a value is changed in the inspector (Called in the editor only).
		/// </summary>
		protected override void OnValidate()
		{
			base.OnValidate();
			colors.fadeDuration = Mathf.Max(colors.fadeDuration, 0.0f);

			if (IsActive())
			{
				SpriteChange(null);

				ColorChange(Color.white, true);
				AnimationChange(AnimationTriggers.normalTrigger);

				TransitionToSelectionState(true);
			}
		}

		/// <summary>
		/// Reset to default values.
		/// </summary>
		protected override void Reset()
		{
			TargetGraphic = GetComponent<Graphic>();
		}
#endif

		/// <summary>
		/// Gets the current selection state.
		/// </summary>
		/// <value>The state current selection state.</value>
		public SelectionState CurrentSelectionState
		{
			get
			{
				return currentSelectionState;
			}
		}

		/// <summary>
		/// Clear any internal state from the SelectableHelper (used when disabling).
		/// </summary>
		protected virtual void InstantClearState()
		{
			string triggerName = AnimationTriggers.normalTrigger;

			isPointerInside = false;
			isPointerDown = false;
			hasSelection = false;

			switch (Transition)
			{
				case Selectable.Transition.ColorTint:
					ColorChange(Color.white, true);
					break;
				case Selectable.Transition.SpriteSwap:
					SpriteChange(null);
					break;
				case Selectable.Transition.Animation:
					AnimationChange(triggerName);
					break;
			}
		}

		/// <summary>
		/// Transition the SelectableHelper to the entered state.
		/// </summary>
		/// <param name="state">State.</param>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		protected virtual void DoStateTransition(SelectionState state, bool instant)
		{
			if (!gameObject.activeInHierarchy)
			{
				return;
			}

			switch (Transition)
			{
				case Selectable.Transition.ColorTint:
					TransitionColor(state, instant);
					break;
				case Selectable.Transition.SpriteSwap:
					TransitionSprite(state, instant);
					break;
				case Selectable.Transition.Animation:
					TransitionAnimation(state, instant);
					break;
			}
		}

		/// <summary>
		/// Change color according selection state.
		/// </summary>
		/// <param name="state">State.</param>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		protected void TransitionColor(SelectionState state, bool instant)
		{
			Color tintColor;

			switch (state)
			{
				case SelectionState.Normal:
					tintColor = Colors.normalColor;
					break;
				case SelectionState.Highlighted:
					tintColor = Colors.highlightedColor;
					break;
				case SelectionState.Pressed:
					tintColor = Colors.pressedColor;
					break;
				case SelectionState.Disabled:
					tintColor = Colors.disabledColor;
					break;
				default:
					tintColor = Color.black;
					break;
			}

			ColorChange(tintColor * Colors.colorMultiplier, instant);
		}

		/// <summary>
		/// Change color.
		/// </summary>
		/// <param name="targetColor">Target color.</param>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		protected void ColorChange(Color targetColor, bool instant)
		{
			if (TargetGraphic == null)
			{
				return;
			}

			TargetGraphic.CrossFadeColor(targetColor, instant ? 0f : Colors.fadeDuration, true, true);
		}

		/// <summary>
		/// Change sprite according selection state.
		/// </summary>
		/// <param name="state">State.</param>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		protected virtual void TransitionSprite(SelectionState state, bool instant)
		{
			switch (state)
			{
				case SelectionState.Normal:
					SpriteChange(null);
					break;
				case SelectionState.Highlighted:
					SpriteChange(SpriteState.highlightedSprite);
					break;
				case SelectionState.Pressed:
					SpriteChange(SpriteState.pressedSprite);
					break;
				case SelectionState.Disabled:
					SpriteChange(SpriteState.disabledSprite);
					break;
				default:
					SpriteChange(null);
					break;
			}
		}

		/// <summary>
		/// Change sprite.
		/// </summary>
		/// <param name="newSprite">New sprite.</param>
		protected void SpriteChange(Sprite newSprite)
		{
			if (Image == null)
			{
				return;
			}

			Image.overrideSprite = newSprite;
		}

		/// <summary>
		/// Change animation according selection state.
		/// </summary>
		/// <param name="state">State.</param>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		protected virtual void TransitionAnimation(SelectionState state, bool instant)
		{
			switch (state)
			{
				case SelectionState.Normal:
					AnimationChange(AnimationTriggers.normalTrigger);
					break;
				case SelectionState.Highlighted:
					AnimationChange(AnimationTriggers.highlightedTrigger);
					break;
				case SelectionState.Pressed:
					AnimationChange(AnimationTriggers.pressedTrigger);
					break;
				case SelectionState.Disabled:
					AnimationChange(AnimationTriggers.disabledTrigger);
					break;
				default:
					AnimationChange(string.Empty);
					break;
			}
		}

		/// <summary>
		/// Run animation.
		/// </summary>
		/// <param name="triggername">Triggername.</param>
		protected void AnimationChange(string triggername)
		{
			#if UNITY_5_3 || UNITY_5_3_OR_NEWER
			if (Animator == null || !Animator.isActiveAndEnabled || Animator.runtimeAnimatorController == null || string.IsNullOrEmpty(triggername))
			#else
			if (Animator == null || Animator.runtimeAnimatorController == null || string.IsNullOrEmpty(triggername))
			#endif
			{
				return;
			}

			Animator.ResetTrigger(AnimationTriggers.normalTrigger);
			Animator.ResetTrigger(AnimationTriggers.pressedTrigger);
			Animator.ResetTrigger(AnimationTriggers.highlightedTrigger);
			Animator.ResetTrigger(AnimationTriggers.disabledTrigger);
			Animator.SetTrigger(triggername);
		}

		/// <summary>
		/// Is the selectable currently 'highlighted'.
		/// </summary>
		/// <returns><c>true</c> if selectable 'highlighted'; otherwise, <c>false</c>.</returns>
		/// <param name="eventData">Event data.</param>
		protected bool IsHighlighted(BaseEventData eventData)
		{
			if (!IsActive())
			{
				return false;
			}

			if (IsPressed())
			{
				return false;
			}

			bool selected = hasSelection;
			var pointerData = eventData as PointerEventData;
			if (pointerData != null)
			{
				selected |=
					(isPointerDown && !isPointerInside && pointerData.pointerPress == gameObject)
					|| (!isPointerDown && isPointerInside && pointerData.pointerPress == gameObject)
					|| (!isPointerDown && isPointerInside && pointerData.pointerPress == null);
			}
			else
			{
				selected |= isPointerInside;
			}

			return selected;
		}

		/// <summary>
		/// Is the selectable currently 'pressed'.
		/// </summary>
		/// <returns><c>true</c> if selectable pressed; otherwise, <c>false</c>.</returns>
		protected bool IsPressed()
		{
			if (!IsActive())
			{
				return false;
			}

			return isPointerInside && isPointerDown;
		}

		/// <summary>
		/// Internally update the selection state of the Selectable.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected void UpdateSelectionState(BaseEventData eventData)
		{
			if (IsPressed())
			{
				currentSelectionState = SelectionState.Pressed;
				return;
			}

			if (IsHighlighted(eventData))
			{
				currentSelectionState = SelectionState.Highlighted;
				return;
			}

			currentSelectionState = SelectionState.Normal;
		}

		/// <summary>
		/// Evaluates the selection state and transition to it.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		void EvaluateAndTransitionToSelectionState(BaseEventData eventData)
		{
			if (!IsActive())
			{
				return;
			}

			UpdateSelectionState(eventData);
			TransitionToSelectionState(false);
		}

		/// <summary>
		/// Transition to selection state.
		/// </summary>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		void TransitionToSelectionState(bool instant)
		{
			var transitionState = (IsActive() && !IsInteractable()) ? SelectionState.Disabled : currentSelectionState;

			DoStateTransition(transitionState, instant);
		}

		/// <summary>
		/// Evaluate current state and transition to pressed state.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerDown(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}

			isPointerDown = true;
			EvaluateAndTransitionToSelectionState(eventData);
		}

		/// <summary>
		/// Evaluate eventData and transition to appropriate state.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerUp(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}

			isPointerDown = false;
			EvaluateAndTransitionToSelectionState(eventData);
		}

		/// <summary>
		/// Evaluate current state and transition to appropriate state.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerEnter(PointerEventData eventData)
		{
			isPointerInside = true;
			EvaluateAndTransitionToSelectionState(eventData);
		}

		/// <summary>
		/// Evaluate current state and transition to normal state.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerExit(PointerEventData eventData)
		{
			isPointerInside = false;
			EvaluateAndTransitionToSelectionState(eventData);
		}

		/// <summary>
		/// Set selection and transition to appropriate state.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnSelect(BaseEventData eventData)
		{
			hasSelection = true;
			EvaluateAndTransitionToSelectionState(eventData);
		}

		/// <summary>
		/// Unset selection and transition to appropriate state.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnDeselect(BaseEventData eventData)
		{
			hasSelection = false;
			EvaluateAndTransitionToSelectionState(eventData);
		}
	}
}