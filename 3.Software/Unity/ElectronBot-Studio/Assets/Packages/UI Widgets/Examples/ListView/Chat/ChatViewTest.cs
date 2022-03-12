namespace UIWidgets.Examples
{
	using System;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// ChatView test script.
	/// </summary>
	public class ChatViewTest : MonoBehaviour, IUpgradeable
	{
		/// <summary>
		/// ChatView.
		/// </summary>
		[SerializeField]
		public ChatView Chat;

		/// <summary>
		/// The message.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with MessageAdapter.")]
		public InputField Message;

		/// <summary>
		/// The name of the user.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with MessageAdapter.")]
		public InputField UserName;

		/// <summary>
		/// The message.
		/// </summary>
		[SerializeField]
		public InputFieldAdapter MessageAdapter;

		/// <summary>
		/// The name of the user.
		/// </summary>
		[SerializeField]
		public InputFieldAdapter UserNameAdapter;

		/// <summary>
		/// The message type.
		/// </summary>
		[SerializeField]
		public Switch Type;

		/// <summary>
		/// Attach audio?
		/// </summary>
		[SerializeField]
		public Switch AttachAudio;

		/// <summary>
		/// Attach image?
		/// </summary>
		[SerializeField]
		public Switch AttachImage;

		/// <summary>
		/// Attached image.
		/// </summary>
		[SerializeField]
		public Sprite TestImage;

		/// <summary>
		/// Attached audio.
		/// </summary>
		[SerializeField]
		public AudioClip TestAudio;

		/// <summary>
		/// Sends the message.
		/// </summary>
		public void SendMessage()
		{
			if (string.IsNullOrEmpty(UserNameAdapter.text.Trim()))
			{
				return;
			}

			if (string.IsNullOrEmpty(MessageAdapter.text.Trim()) && !AttachImage.IsOn && !AttachAudio.IsOn)
			{
				return;
			}

			// add new message to chat
			var line = new ChatLine()
			{
				UserName = UserNameAdapter.text,
				Message = MessageAdapter.text,
				Time = DateTime.Now,
				Type = Type.IsOn ? ChatLineType.Outgoing : ChatLineType.Incoming,
			};

			if (AttachImage.IsOn)
			{
				line.Image = TestImage;
			}

			if (AttachAudio.IsOn)
			{
				line.Audio = TestAudio;
			}

			Chat.DataSource.Add(line);

			MessageAdapter.text = string.Empty;

			// scroll to end
			Chat.ScrollRect.verticalNormalizedPosition = 0f;
		}

		/// <summary>
		/// Add messages to chat.
		/// </summary>
		public void Test()
		{
			var now = DateTime.Now;
			var lines = new ObservableList<ChatLine>()
			{
				new ChatLine()
				{
					UserName = "Test",
					Message = "line 1",
					Time = now,
					Type = ChatLineType.Incoming,
					Image = TestImage,
				},
				new ChatLine()
				{
					UserName = "Test",
					Message = "line 1\nline2",
					Time = now,
					Type = ChatLineType.Incoming,
					Audio = TestAudio,
				},
				new ChatLine()
				{
					UserName = "Test",
					Message = "line 1\nline2\nline3",
					Time = now,
					Type = ChatLineType.Incoming,
				},
				new ChatLine()
				{
					UserName = "Test",
					Message = "line 1\nline2",
					Time = now,
					Type = ChatLineType.Incoming,
				},
				new ChatLine()
				{
					UserName = "Test",
					Message = "line 1\nline2",
					Time = now,
					Type = ChatLineType.Outgoing,
				},
				new ChatLine()
				{
					UserName = "Test",
					Message = "line 1\nline2",
					Time = now,
					Type = ChatLineType.Outgoing,
					Audio = TestAudio,
				},
				new ChatLine()
				{
					UserName = "Test",
					Message = "line 1\nline2",
					Time = now,
					Type = ChatLineType.Incoming,
				},
				new ChatLine()
				{
					UserName = "Test",
					Message = "line 1\nline2",
					Time = now,
					Type = ChatLineType.Outgoing,
					Image = TestImage,
				},
				new ChatLine()
				{
					UserName = "Test",
					Message = "line 1\nline2\nline3",
					Time = now,
					Type = ChatLineType.Incoming,
				},
				new ChatLine()
				{
					UserName = "Test",
					Message = "line 1\nline2\nline3\nline4",
					Time = now,
					Type = ChatLineType.Incoming,
					Image = TestImage,
					Audio = TestAudio,
				},
			};

			Chat.DataSource = lines;
			Chat.ScrollRect.verticalNormalizedPosition = 0f;
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public virtual void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.GetOrAddComponent(UserName, ref UserNameAdapter);
			Utilities.GetOrAddComponent(Message, ref MessageAdapter);
#pragma warning restore 0612, 0618
		}

#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected virtual void OnValidate()
		{
			Compatibility.Upgrade(this);
		}
#endif
	}
}