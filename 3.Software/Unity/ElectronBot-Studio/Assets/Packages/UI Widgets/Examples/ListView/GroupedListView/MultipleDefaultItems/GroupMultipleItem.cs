namespace UIWidgets.Examples
{
	using System;
	using UIWidgets.Attributes;
	using UnityEngine;

	/// <summary>
	/// GroupMultipleItem.
	/// </summary>
	[Serializable]
	public class GroupMultipleItem
	{
		/// <summary>
		/// Item mode.
		/// </summary>
		public enum ItemMode
		{
			/// <summary>
			/// Group.
			/// </summary>
			Group = 0,

			/// <summary>
			/// Checkbox.
			/// </summary>
			Checkbox = 1,

			/// <summary>
			/// Value.
			/// </summary>
			Value = 2,
		}

		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		public string Name;

		/// <summary>
		/// Mode.
		/// </summary>
		[SerializeField]
		public ItemMode Mode = ItemMode.Group;

		/// <summary>
		/// Description.
		/// </summary>
		[EditorConditionEnum("Mode", (int)ItemMode.Group)]
		[SerializeField]
		[Multiline]
		public string Desription;

		/// <summary>
		/// Is on?
		/// </summary>
		[EditorConditionEnum("Mode", (int)ItemMode.Checkbox)]
		[SerializeField]
		public bool IsOn;

		/// <summary>
		/// Value.
		/// </summary>
		[EditorConditionEnum("Mode", (int)ItemMode.Value)]
		[SerializeField]
		public string Value;
	}
}