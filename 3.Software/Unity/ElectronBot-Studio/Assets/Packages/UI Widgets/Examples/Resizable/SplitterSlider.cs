namespace UIWidgets.Examples
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Splitter Slider.
	/// </summary>
	public class SplitterSlider : MonoBehaviour
	{
		/// <summary>
		/// Left filler.
		/// </summary>
		[SerializeField]
		protected RectTransform LeftFiller;

		/// <summary>
		/// Right filler.
		/// </summary>
		[SerializeField]
		protected RectTransform RightFiller;

		/// <summary>
		/// Track range.
		/// </summary>
		[SerializeField]
		protected RectTransform TrackRange;

		/// <summary>
		/// Audio clip.
		/// </summary>
		[SerializeField]
		public AudioClip Track;

		/// <summary>
		/// Length of the rects.
		/// </summary>
		/// <returns>The length.</returns>
		protected float RectLength()
		{
			return LeftFiller.rect.width + RightFiller.rect.width + TrackRange.rect.width;
		}

		/// <summary>
		/// Gets the start of the track in samples.
		/// </summary>
		/// <returns>The start of the track in samples.</returns>
		public int GetTrackStart()
		{
			return Mathf.RoundToInt(LeftFiller.rect.width / RectLength() * Track.samples);
		}

		/// <summary>
		/// Gets the start of track in seconds.
		/// </summary>
		/// <returns>The track start in seconds.</returns>
		public int GetTrackStartInSeconds()
		{
			return Mathf.RoundToInt(LeftFiller.rect.width / RectLength() * Track.length);
		}

		/// <summary>
		/// Gets the length of the track in samples.
		/// </summary>
		/// <returns>The length of the track in samples.</returns>
		public int GetTrackLength()
		{
			return Mathf.RoundToInt(TrackRange.rect.width / RectLength() * Track.samples);
		}

		/// <summary>
		/// Gets the length of the track in seconds.
		/// </summary>
		/// <returns>The length of the track in seconds.</returns>
		public int GetTrackLengthInSeconds()
		{
			return Mathf.RoundToInt(TrackRange.rect.width / RectLength() * Track.length);
		}

		/// <summary>
		/// Gets the end of the track in samples.
		/// </summary>
		/// <returns>The end of the track in samples.</returns>
		public int GetTrackEnd()
		{
			return Mathf.RoundToInt((LeftFiller.rect.width + TrackRange.rect.width) / RectLength() * Track.samples);
		}

		/// <summary>
		/// Gets the end of the track in seconds.
		/// </summary>
		/// <returns>The end of the track in seconds.</returns>
		public int GetTrackEndInSeconds()
		{
			return Mathf.RoundToInt((LeftFiller.rect.width + TrackRange.rect.width) / RectLength() * Track.length);
		}

		/// <summary>
		/// Test this instance.
		/// </summary>
		public void Test()
		{
			Debug.Log(string.Format("Start: {0} samples; {1} seconds", GetTrackStart().ToString(), GetTrackStartInSeconds().ToString()));
			Debug.Log(string.Format("Length: {0} samples; {1} seconds", GetTrackLength().ToString(), GetTrackLengthInSeconds().ToString()));
			Debug.Log(string.Format("End: {0} samples; {1} seconds", GetTrackEnd().ToString(), GetTrackEndInSeconds().ToString()));
		}
	}
}