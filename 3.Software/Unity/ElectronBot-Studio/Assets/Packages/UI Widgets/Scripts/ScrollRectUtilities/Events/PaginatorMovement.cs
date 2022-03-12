namespace UIWidgets
{
	using System;
	using UnityEngine.Events;

	/// <summary>
	/// Movement event.
	/// Params:
	/// - page
	/// - relative distance between this page and next page in range 0..1
	/// </summary>
	[Serializable]
	public class PaginatorMovement : UnityEvent<int, float>
	{
	}
}