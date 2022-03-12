namespace UIWidgets
{
	using UnityEngine;

	/// <summary>
	/// IResizableItem.
	/// </summary>
	public interface IResizableItem
	{
		/// <summary>
		/// Gets the objects to resize.
		/// </summary>
		/// <value>The objects to resize.</value>
		GameObject[] ObjectsToResize
		{
			get;
		}
	}
}