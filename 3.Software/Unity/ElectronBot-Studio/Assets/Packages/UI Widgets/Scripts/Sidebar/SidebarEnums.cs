namespace UIWidgets
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Sidebar axis.
	/// </summary>
	public enum SidebarAxis
	{
		/// <summary>
		/// Left to right.
		/// </summary>
		LeftToRight = 0,

		/// <summary>
		/// Top to bottom.
		/// </summary>
		TopToBottom = 1,

		/// <summary>
		/// Right to left.
		/// </summary>
		RightToLeft = 2,

		/// <summary>
		/// Bottom to top.
		/// </summary>
		BottomToTop = 3,
	}

	/// <summary>
	/// Sidebar animation.
	/// </summary>
	public enum SidebarAnimation
	{
		/// <summary>
		/// Overlay.
		/// </summary>
		Overlay = 0,

		/// <summary>
		/// Push.
		/// </summary>
		Push = 1,

		/// <summary>
		/// ScaleDown.
		/// </summary>
		ScaleDown = 2,

		/// <summary>
		/// Uncover.
		/// </summary>
		Uncover = 3,

		/// <summary>
		/// SlideAlong.
		/// </summary>
		SlideAlong = 4,

		/// <summary>
		/// SlideOut.
		/// </summary>
		SlideOut = 5,

		/// <summary>
		/// Resize.
		/// </summary>
		Resize = 6,

		/// <summary>
		/// The scale down and slide content.
		/// </summary>
		ScaleDownAndPush = 7,
	}
}