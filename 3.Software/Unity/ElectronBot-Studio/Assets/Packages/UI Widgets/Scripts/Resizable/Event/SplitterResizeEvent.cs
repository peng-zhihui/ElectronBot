namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.Events;

	/// <summary>
	/// Splitter resize event.
	/// </summary>
	[SerializeField]
	public class SplitterResizeEvent : UnityEvent<Splitter>
	{
	}
}