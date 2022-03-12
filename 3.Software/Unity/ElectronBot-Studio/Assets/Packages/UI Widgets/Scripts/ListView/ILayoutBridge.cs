namespace UIWidgets
{
	using UnityEngine;

	/// <summary>
	/// ILayoutBridge.
	/// </summary>
	public interface ILayoutBridge
	{
		/// <summary>
		/// Gets or sets a value indicating whether this instance is horizontal.
		/// </summary>
		/// <value><c>true</c> if this instance is horizontal; otherwise, <c>false</c>.</value>
		bool IsHorizontal
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="UIWidgets.ILayoutBridge"/> update content size fitter.
		/// </summary>
		/// <value><c>true</c> if update content size fitter; otherwise, <c>false</c>.</value>
		bool UpdateContentSizeFitter
		{
			get;
			set;
		}

		/// <summary>
		/// Updates the layout.
		/// </summary>
		void UpdateLayout();

		/// <summary>
		/// Sets the filler.
		/// </summary>
		/// <param name="first">First.</param>
		/// <param name="last">Last.</param>
		void SetFiller(float first, float last);

		/// <summary>
		/// Gets the size of the item.
		/// </summary>
		/// <returns>The item size.</returns>
		Vector2 GetItemSize();

		/// <summary>
		/// Gets the top or left margin.
		/// </summary>
		/// <returns>The margin.</returns>
		float GetMargin();

		/// <summary>
		/// Return MarginX or MarginY depend of direction.
		/// </summary>
		/// <returns>The full margin.</returns>
		float GetFullMargin();

		/// <summary>
		/// Gets sum of left and right margin.
		/// </summary>
		/// <returns>The margin.</returns>
		float GetFullMarginX();

		/// <summary>
		/// Gets sum of top and bottom margin.
		/// </summary>
		/// <returns>The full margin.</returns>
		float GetFullMarginY();

		/// <summary>
		/// Gets the size of the margin.
		/// </summary>
		/// <returns>The margin size.</returns>
		Vector4 GetMarginSize();

		/// <summary>
		/// Gets the spacing.
		/// </summary>
		/// <returns>The spacing.</returns>
		float GetSpacing();

		/// <summary>
		/// Gets the horizontal spacing.
		/// </summary>
		/// <returns>The spacing.</returns>
		float GetSpacingX();

		/// <summary>
		/// Gets the vertical spacing.
		/// </summary>
		/// <returns>The spacing.</returns>
		float GetSpacingY();

		/// <summary>
		/// Blocks constraint count.
		/// </summary>
		/// <param name="blocks">Blocks count.</param>
		/// <returns>The constraint.</returns>
		int BlocksConstraint(int blocks);

		/// <summary>
		/// Columns constraint count.
		/// </summary>
		/// <param name="columns">Columns count.</param>
		/// <returns>The constraint.</returns>
		int ColumnsConstraint(int columns);

		/// <summary>
		/// Rows the constraint count.
		/// </summary>
		/// <param name="rows">Rows count.</param>
		/// <returns>The constraint.</returns>
		int RowsConstraint(int rows);
	}
}