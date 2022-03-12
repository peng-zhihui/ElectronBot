namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// Test Notify.
	/// </summary>
	public class TestNotify : MonoBehaviour
	{
		/// <summary>
		/// Notification template.
		/// Gameobject in Hierarchy window, parent gameobject should have Layout component (recommended EasyLayout)
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("notifyPrefab")]
		[FormerlySerializedAs("NotifyTemplate")]
		protected Notify NotificationTemplate;

		/// <summary>
		/// Notification template with custom content.
		/// </summary>
		[SerializeField]
		protected Notify NotificationTemplateContent;

		/// <summary>
		/// Show Notification.
		/// </summary>
		public void ShowNotify()
		{
			NotificationTemplate.Clone().Show("Achievement unlocked. Hide after 3 seconds.", customHideDelay: 3f);
		}

		/// <summary>
		/// Notification with horizontal collapse animation.
		/// </summary>
		public void HorizontalCollapse()
		{
			NotificationTemplate.Clone().Show("Notification message.", customHideDelay: 3f, hideAnimation: NotificationBase.AnimationCollapseHorizontal, slideUpOnHide: false);
		}

		/// <summary>
		/// Notification with horizontal rotate animation.
		/// </summary>
		public void HorizontalRotate()
		{
			NotificationTemplate.Clone().Show("Notification message.", customHideDelay: 3f, hideAnimation: NotificationBase.AnimationRotateHorizontal, slideUpOnHide: false);
		}

		/// <summary>
		/// Notification with slide animation to left.
		/// </summary>
		public void SlideLeft()
		{
			NotificationTemplate.Clone().Show("Notification message.", customHideDelay: 3f, hideAnimation: NotificationBase.AnimationSlideLeft);
		}

		/// <summary>
		/// Notification with slide animation to right.
		/// </summary>
		public void SlideRight()
		{
			NotificationTemplate.Clone().Show("Notification message.", customHideDelay: 3f, hideAnimation: NotificationBase.AnimationSlideRight);
		}

		/// <summary>
		/// Notification with slide animation to left.
		/// </summary>
		public void SlideLeftCustom()
		{
			NotificationTemplate.Clone().Show("Notification message.", customHideDelay: 3f, hideAnimation: x => NotificationBase.HideAnimationSlideBase(x, true, -1f, 200f, true));
		}

		/// <summary>
		/// Notification with slide animation to right.
		/// </summary>
		public void SlideRightCustom()
		{
			NotificationTemplate.Clone().Show("Notification message.", customHideDelay: 3f, hideAnimation: x => NotificationBase.HideAnimationSlideBase(x, true, +1f, 200f, true));
		}

		/// <summary>
		/// Notification with slide animation to up.
		/// </summary>
		public void SlideUp()
		{
			NotificationTemplate.Clone().Show("Notification message.", customHideDelay: 3f, hideAnimation: NotificationBase.AnimationSlideUp);
		}

		/// <summary>
		/// Notification with slide animation to down.
		/// </summary>
		public void SlideDown()
		{
			NotificationTemplate.Clone().Show("Notification message.", customHideDelay: 3f, hideAnimation: NotificationBase.AnimationSlideDown);
		}

		/// <summary>
		/// Custom content.
		/// </summary>
		[SerializeField]
		protected RectTransform CustomContent;

		/// <summary>
		/// Parent of the CustomContent.
		/// </summary>
		[SerializeField]
		protected RectTransform CustomContentParent;

		/// <summary>
		/// Notification with slide animation to down.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required.")]
		public void Content()
		{
			CustomContent.gameObject.SetActive(true);

			var notify = NotificationTemplateContent.Clone();
			notify.Show(
				customHideDelay: 3f,
				content: CustomContent,
				onReturn: ReturnContent);
		}

		void ReturnContent()
		{
			CustomContent.SetParent(CustomContentParent);
			CustomContent.gameObject.SetActive(false);
		}
	}
}