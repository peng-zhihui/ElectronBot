namespace UIWidgets
{
	/// <summary>
	/// Notification.
	/// How to use:
	/// 1. Create container or containers with layout component. Notifications will be shown in those containers. You can check how it works with NotifyContainer in the sample scene.
	/// 2. Create template for notification with Notify component.
	/// 3. If you want change text in runtime set Text property in Notify component.
	/// 4. If you want close notification by button set Hide button property in Notify component.
	/// 5. Write code to show notification
	/// <example>
	/// notificationPrefab.Clone().Show("Sticky Notification. Click on the × above to close.");
	/// </example>
	/// notificationPrefab.Clone() - return the notification instance by template name.
	/// Show("Sticky Notification. Click on the × above to close.") - show notification with following text;
	/// or
	/// Show(message: "Simple Notification.", customHideDelay = 4.5f, hideAnimation = UIWidgets.Notify.AnimationCollapse, slideUpOnHide = false);
	/// Show notification with following text, hide it after 4.5 seconds, run specified animation on hide without SlideUpOnHide.
	/// </summary>
	public class Notify : NotificationCustom<Notify>
	{
	}
}