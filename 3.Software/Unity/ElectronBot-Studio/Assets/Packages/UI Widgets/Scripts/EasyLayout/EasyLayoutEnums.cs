namespace EasyLayoutNS
{
	/// <summary>
	/// Grid constraints.
	/// </summary>
	public enum GridConstraints
	{
		/// <summary>
		/// Don't constraint the number of rows or columns.
		/// </summary>
		Flexible = 0,

		/// <summary>
		/// Constraint the number of columns to a specified number.
		/// </summary>
		FixedColumnCount = 1,

		/// <summary>
		/// Constraint the number of rows to a specified number.
		/// </summary>
		FixedRowCount = 2,
	}

	/// <summary>
	/// Compact constraints.
	/// </summary>
	public enum CompactConstraints
	{
		/// <summary>
		/// Don't constraint the number of rows or columns.
		/// </summary>
		Flexible = 0,

		/// <summary>
		/// Constraint the number of columns to a specified number.
		/// </summary>
		MaxColumnCount = 1,

		/// <summary>
		/// Constraint the number of rows to a specified number.
		/// </summary>
		MaxRowCount = 2,
	}

	/// <summary>
	/// Children size.
	/// </summary>
	public enum ChildrenSize
	{
		/// <summary>
		/// Don't change children sizes.
		/// </summary>
		DoNothing = 0,

		/// <summary>
		/// Set children sizes to preferred.
		/// </summary>
		SetPreferred = 1,

		/// <summary>
		/// Set children size to maximum size of children preferred.
		/// </summary>
		SetMaxFromPreferred = 2,

		/// <summary>
		/// Change children size from min to preferred to fit container.
		/// </summary>
		FitContainer = 3,

		/// <summary>
		/// Stretch children size to fit container.
		/// </summary>
		SetPreferredAndFitContainer = 5,

		/// <summary>
		/// Shrink children size if UI size more than RectTransform size.
		/// </summary>
		ShrinkOnOverflow = 4,
	}

	/// <summary>
	/// Anchors.
	/// </summary>
	public enum Anchors
	{
		/// <summary>
		/// UpperLeft.
		/// </summary>
		UpperLeft = 0,

		/// <summary>
		/// UpperCenter.
		/// </summary>
		UpperCenter = 1,

		/// <summary>
		/// UpperRight.
		/// </summary>
		UpperRight = 2,

		/// <summary>
		/// MiddleLeft.
		/// </summary>
		MiddleLeft = 3,

		/// <summary>
		/// MiddleCenter.
		/// </summary>
		MiddleCenter = 4,

		/// <summary>
		/// MiddleRight.
		/// </summary>
		MiddleRight = 5,

		/// <summary>
		/// LowerLeft.
		/// </summary>
		LowerLeft = 6,

		/// <summary>
		/// LowerCenter.
		/// </summary>
		LowerCenter = 7,

		/// <summary>
		/// LowerRight.
		/// </summary>
		LowerRight = 8,
	}

	/// <summary>
	/// Stacking.
	/// </summary>
	public enum Stackings
	{
		/// <summary>
		/// Horizontal.
		/// </summary>
		Horizontal = 0,

		/// <summary>
		/// Vertical.
		/// </summary>
		Vertical = 1,
	}

	/// <summary>
	/// Axis.
	/// </summary>
	public enum Axis
	{
		/// <summary>
		/// Horizontal.
		/// </summary>
		Horizontal = 0,

		/// <summary>
		/// Vertical.
		/// </summary>
		Vertical = 1,
	}

	/// <summary>
	/// Horizontal aligns.
	/// </summary>
	public enum HorizontalAligns
	{
		/// <summary>
		/// Left.
		/// </summary>
		Left = 0,

		/// <summary>
		/// Center.
		/// </summary>
		Center = 1,

		/// <summary>
		/// Right.
		/// </summary>
		Right = 2,
	}

	/// <summary>
	/// Inner aligns.
	/// </summary>
	public enum InnerAligns
	{
		/// <summary>
		/// Top.
		/// </summary>
		Top = 0,

		/// <summary>
		/// Middle.
		/// </summary>
		Middle = 1,

		/// <summary>
		/// Bottom.
		/// </summary>
		Bottom = 2,
	}

	/// <summary>
	/// Layout type to use.
	/// </summary>
	public enum LayoutTypes
	{
		/// <summary>
		/// Compact.
		/// </summary>
		Compact = 0,

		/// <summary>
		/// Grid.
		/// </summary>
		Grid = 1,

		/// <summary>
		/// Flex.
		/// </summary>
		Flex = 2,

		/// <summary>
		/// Staggered.
		/// </summary>
		Staggered = 3,

		/// <summary>
		/// Ellipse.
		/// </summary>
		Ellipse = 4,
	}

	/// <summary>
	/// Align for the Ellipse layout type.
	/// </summary>
	public enum EllipseAlign
	{
		/// <summary>
		/// Outer align.
		/// </summary>
		Outer = 0,

		/// <summary>
		/// Center align.
		/// </summary>
		Center = 1,

		/// <summary>
		/// Inner align.
		/// </summary>
		Inner = 2,
	}

	/// <summary>
	/// Fill for the Ellipse layout type.
	/// </summary>
	public enum EllipseFill
	{
		/// <summary>
		/// Closed.
		/// </summary>
		Closed = 0,

		/// <summary>
		/// Arc.
		/// </summary>
		Arc = 1,
	}

	/// <summary>
	/// Resize type.
	/// </summary>
	[System.Flags]
	public enum ResizeType
	{
		/// <summary>
		/// None.
		/// </summary>
		None = 0,

		/// <summary>
		/// Horizontal resize.
		/// </summary>
		Horizontal = 1,

		/// <summary>
		/// Vertical resize.
		/// </summary>
		Vertical = 2,
	}
}