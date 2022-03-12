namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Style for the AudioPlayer.
	/// </summary>
	[Serializable]
	public class StyleAudioPlayer : IStyleDefaultValues
	{
		/// <summary>
		/// Style for the progress.
		/// </summary>
		[SerializeField]
		public StyleSlider Progress;

		/// <summary>
		/// Style for the play.
		/// </summary>
		[SerializeField]
		public StyleButton Play;

		/// <summary>
		/// Style for the pause.
		/// </summary>
		[SerializeField]
		public StyleButton Pause;

		/// <summary>
		/// Style for the stop.
		/// </summary>
		[SerializeField]
		public StyleButton Stop;

		/// <summary>
		/// Style for the toggle.
		/// </summary>
		[SerializeField]
		public StyleButton Toggle;

#if UNITY_EDITOR
		/// <inheritdoc/>
		public void SetDefaultValues()
		{
			Progress.SetDefaultValues();
			Play.SetDefaultValues();
			Pause.SetDefaultValues();
			Stop.SetDefaultValues();
			Toggle.SetDefaultValues();
		}
#endif
	}
}