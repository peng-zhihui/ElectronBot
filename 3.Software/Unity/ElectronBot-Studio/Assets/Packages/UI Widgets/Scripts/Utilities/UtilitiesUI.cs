namespace UIWidgets
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// UI utilities.
	/// </summary>
	public static class UtilitiesUI
	{
		static List<Canvas> ParentCanvases = new List<Canvas>();

		/// <summary>
		/// Finds the canvas.
		/// </summary>
		/// <returns>The canvas.</returns>
		/// <param name="currentObject">Current object.</param>
		public static RectTransform FindCanvas(Transform currentObject)
		{
			var canvas = currentObject.GetComponentInParent<Canvas>();
			if (canvas == null)
			{
				return null;
			}

			return canvas.transform as RectTransform;
		}

		/// <summary>
		/// Finds the topmost canvas.
		/// </summary>
		/// <returns>The canvas.</returns>
		/// <param name="currentObject">Current object.</param>
		public static RectTransform FindTopmostCanvas(Transform currentObject)
		{
			currentObject.GetComponentsInParent<Canvas>(true, ParentCanvases);
			if (ParentCanvases.Count == 0)
			{
				return null;
			}

			var result = ParentCanvases[ParentCanvases.Count - 1].transform as RectTransform;
			ParentCanvases.Clear();

			return result;
		}

		/// <summary>
		/// Calculates the drag position.
		/// </summary>
		/// <returns>The drag position.</returns>
		/// <param name="screenPosition">Screen position.</param>
		/// <param name="canvas">Canvas.</param>
		/// <param name="canvasRect">Canvas RectTransform.</param>
		public static Vector3 CalculateDragPosition(Vector3 screenPosition, Canvas canvas, RectTransform canvasRect)
		{
			Vector3 result;
			var canvasSize = canvasRect.sizeDelta;
			Vector2 min = Vector2.zero;
			Vector2 max = canvasSize;

			var isOverlay = canvas.renderMode == RenderMode.ScreenSpaceOverlay;
			var noCamera = canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera == null;
			if (isOverlay || noCamera)
			{
				result = screenPosition;
			}
			else
			{
				var ray = canvas.worldCamera.ScreenPointToRay(screenPosition);
				var plane = new Plane(canvasRect.forward, canvasRect.position);

				float distance;
				plane.Raycast(ray, out distance);

				result = canvasRect.InverseTransformPoint(ray.origin + (ray.direction * distance));

				min = -Vector2.Scale(max, canvasRect.pivot);
				max = canvasSize - min;
			}

			result.x = Mathf.Clamp(result.x, min.x, max.y);
			result.y = Mathf.Clamp(result.y, min.x, max.y);

			return result;
		}

		/// <summary>
		/// Determines if slider is horizontal.
		/// </summary>
		/// <returns><c>true</c> if slider is horizontal; otherwise, <c>false</c>.</returns>
		/// <param name="slider">Slider.</param>
		public static bool IsHorizontal(Slider slider)
		{
			return slider.direction == Slider.Direction.LeftToRight || slider.direction == Slider.Direction.RightToLeft;
		}

		/// <summary>
		/// Determines if scrollbar is horizontal.
		/// </summary>
		/// <returns><c>true</c> if scrollbar is horizontal; otherwise, <c>false</c>.</returns>
		/// <param name="scrollbar">Scrollbar.</param>
		public static bool IsHorizontal(Scrollbar scrollbar)
		{
			return scrollbar.direction == Scrollbar.Direction.LeftToRight || scrollbar.direction == Scrollbar.Direction.RightToLeft;
		}

		/// <summary>
		/// Get graphic component from TextAdapter.
		/// </summary>
		/// <param name="adapter">Adapter.</param>
		/// <returns>Graphic component.</returns>
		public static Graphic GetGraphic(TextAdapter adapter)
		{
			return (adapter != null) ? adapter.Graphic : null;
		}
	}
}