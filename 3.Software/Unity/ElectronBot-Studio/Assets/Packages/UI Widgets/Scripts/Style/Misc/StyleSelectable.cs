namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Style for the selectable.
	/// </summary>
	[Serializable]
	public class StyleSelectable : IStyleDefaultValues
	{
		/// <summary>
		/// Style for the transition.
		/// </summary>
		[SerializeField]
		public Selectable.Transition Transition = Selectable.Transition.ColorTint;

		/// <summary>
		/// Style for the colors.
		/// </summary>
		[SerializeField]
		public ColorBlock Colors = new ColorBlock()
		{
			normalColor = Color.clear,
			highlightedColor = new Color32(196, 156, 39, 255),
			pressedColor = new Color32(181, 122, 36, 255),
			disabledColor = new Color32(200, 200, 200, 128),
			colorMultiplier = 1f,
			fadeDuration = 0.1f,
#if UNITY_2019_1_OR_NEWER
			selectedColor = new Color32(181, 122, 36, 255),
#endif
		};

		/// <summary>
		/// Style for the sprites.
		/// </summary>
		[SerializeField]
		public SpriteState Sprites;

		/// <summary>
		/// Style for the animations.
		/// </summary>
		[SerializeField]
		public AnimationTriggers Animation;

		/// <summary>
		/// Apply style to the specified Selectable.
		/// </summary>
		/// <param name="component">Selectable</param>
		public void ApplyTo(Selectable component)
		{
			component.transition = Transition;
			component.colors = Colors;
			component.spriteState = Sprites;
			component.animationTriggers = Copy(Animation);
		}

		/// <summary>
		/// Set style options from the specified Selectable.
		/// </summary>
		/// <param name="component">Selectable.</param>
		public void GetFrom(Selectable component)
		{
			Transition = component.transition;
			Colors = component.colors;
			Sprites = component.spriteState;
			Animation = Copy(component.animationTriggers);
		}

		/// <summary>
		/// Apply style to the specified Selectable.
		/// </summary>
		/// <param name="component">Selectable</param>
		public void ApplyTo(SelectableHelper component)
		{
			component.Transition = Transition;
			component.Colors = Colors;
			component.SpriteState = Sprites;
			component.AnimationTriggers = Copy(Animation);
		}

		/// <summary>
		/// Set style options from the specified Selectable.
		/// </summary>
		/// <param name="component">Selectable.</param>
		public void GetFrom(SelectableHelper component)
		{
			Transition = component.Transition;
			Colors = component.Colors;
			Sprites = component.SpriteState;
			Animation = Copy(component.AnimationTriggers);
		}

		/// <summary>
		/// Create a copy of the animation triggers.
		/// </summary>
		/// <param name="source">Source.</param>
		/// <returns>Animation triggers.</returns>
		protected AnimationTriggers Copy(AnimationTriggers source)
		{
			return new AnimationTriggers()
			{
				normalTrigger = source.normalTrigger,
				highlightedTrigger = source.highlightedTrigger,
				pressedTrigger = source.pressedTrigger,
				disabledTrigger = source.disabledTrigger,
				#if UNITY_2019_1_OR_NEWER
				selectedTrigger = source.selectedTrigger,
				#endif
			};
		}

#if UNITY_EDITOR
		/// <inheritdoc/>
		public void SetDefaultValues()
		{
		}
#endif
	}
}