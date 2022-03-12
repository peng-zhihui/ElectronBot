#if ENABLE_INPUT_SYSTEM
namespace UIWidgets.InputSystem
{
	using System;
	using UnityEngine;
	using UnityEngine.InputSystem;
	using UnityEngine.InputSystem.Layouts;
	using UnityEngine.InputSystem.Utilities;
	using UnityEngine.Scripting;

	/// <summary>
	/// Button with three modifiers.
	/// </summary>
#if UNITY_EDITOR
	[UnityEditor.InitializeOnLoad] // Automatically register in editor.
#endif
	[Preserve]
	[DisplayStringFormat("{modifier1}+{modifier2}+{modifier3}+{button}")]
	public class CustomButtonWithThreeModifiers : InputBindingComposite<float>
	{
		/// <summary>
		/// Modifier1.
		/// </summary>
		[InputControl(layout = "Button")]
		public int Modifier1;

		/// <summary>
		/// Modifier2.
		/// </summary>
		[InputControl(layout = "Button")]
		public int Modifier2;

		/// <summary>
		/// Modifier3.
		/// </summary>
		[InputControl(layout = "Button")]
		public int Modifier3;

		/// <summary>
		/// Button.
		/// </summary>
		[InputControl(layout = "Button")]
		public int Button;

		/// <summary>
		/// Read value.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <returns>Value.</returns>
		public override float ReadValue(ref InputBindingCompositeContext context)
		{
			if (context.ReadValueAsButton(Modifier1) && context.ReadValueAsButton(Modifier2) && context.ReadValueAsButton(Modifier3))
			{
				return context.ReadValue<float>(Button);
			}

			return 0f;
		}

		/// <summary>
		/// Evaluate magnitude.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <returns>Value.</returns>
		public override float EvaluateMagnitude(ref InputBindingCompositeContext context)
		{
			return ReadValue(ref context);
		}

		static CustomButtonWithThreeModifiers()
		{
			InputSystem.RegisterBindingComposite<CustomButtonWithThreeModifiers>();
		}

		[RuntimeInitializeOnLoadMethod]
		static void Init()
		{
			// do nothing, requires to call static constructor
		}
	}
}
#endif