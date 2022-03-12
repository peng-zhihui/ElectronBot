namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Test notification with buttons.
	/// </summary>
	public class TestNotifyButton : MonoBehaviour
	{
		/// <summary>
		/// Notification template.
		/// Gameobject in Hierarchy window, parent gameobject should have Layout component (recommended EasyLayout)
		/// </summary>
		[SerializeField]
		protected Notify NotificationTemplate;

		/// <summary>
		/// Show notification.
		/// </summary>
		public void ShowNotify()
		{
			var actions = new NotificationButton[]
			{
				new NotificationButton("Close", NotificationClose),
				new NotificationButton("Log", NotificationClick),
			};

			var instance = NotificationTemplate.Clone();
			instance.Show("Notification with buttons. Hide after 3 seconds.", customHideDelay: 5f);
			instance.SetButtons(actions);
		}

		bool NotificationClose(NotificationBase notification, int index)
		{
			Debug.Log("close notification");
			return true;
		}

		bool NotificationClick(NotificationBase notification, int index)
		{
			Debug.Log("click notification button");
			return false;
		}
	}
}