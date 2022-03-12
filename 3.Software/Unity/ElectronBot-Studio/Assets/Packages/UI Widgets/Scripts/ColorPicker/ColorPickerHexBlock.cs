namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Color picker Hex block.
	/// </summary>
	public class ColorPickerHexBlock : ColorPickerHexBlockBase
	{
		/// <summary>
		/// The input field for hex.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with InputHexAdapter.")]
		protected InputField InputHex;

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public override void Upgrade()
		{
#pragma warning disable 0612,0618
			Utilities.GetOrAddComponent(InputHex, ref InputHexAdapter);
#pragma warning restore 0612,0618
		}
	}
}