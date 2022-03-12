namespace UIWidgets.Examples
{
	using System;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Origin of chat line.
	/// </summary>
	public enum ChatLineType
	{
		/// <summary>
		/// Incoming message.
		/// </summary>
		Incoming,

		/// <summary>
		/// Outgoing message.
		/// </summary>
		Outgoing,
	}

	/// <summary>
	/// Chat line.
	/// </summary>
	[Serializable]
	public class ChatLine
	{
		/// <summary>
		/// Username.
		/// </summary>
		[SerializeField]
		public string UserName;

		/// <summary>
		/// Message.
		/// </summary>
		[SerializeField]
		public string Message;

		/// <summary>
		/// Message time.
		/// </summary>
		[SerializeField]
		public DateTime Time;

		/// <summary>
		/// Attached image.
		/// </summary>
		[SerializeField]
		public Sprite Image;

		/// <summary>
		/// Attached audio.
		/// </summary>
		[SerializeField]
		public AudioClip Audio;

		/// <summary>
		/// Message type.
		/// </summary>
		[SerializeField]
		public ChatLineType Type;

		/// <summary>
		/// Convert this instance to string.
		/// </summary>
		/// <returns>String.</returns>
		public override string ToString()
		{
			return string.Format("{0} [{1}]: {2}", Time.ToString(), UserName, Message);
		}
	}
}