namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// Notification and Dialog tests.
	/// </summary>
	public class UITestSamples : MonoBehaviour
	{
		/// <summary>
		/// Question icon.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("questionIcon")]
		protected Sprite QuestionIcon;

		/// <summary>
		/// Attention icon.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("attentionIcon")]
		protected Sprite AttentionIcon;

		/// <summary>
		/// Simple notification template.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("notifySimple")]
		protected Notify NotifySimpleTemplate;

		/// <summary>
		/// Auto-hide notification template.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("notifyAutoHide")]
		protected Notify NotifyAutoHideTemplate;

		/// <summary>
		/// Sample dialog template.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("dialogSample")]
		protected Dialog DialogSampleTemplate;

		/// <summary>
		/// Sign-in dialog template.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("dialogSignIn")]
		protected Dialog DialogSignInTemplate;

		/// <summary>
		/// TreeView dialog template.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("dialogTreeView")]
		protected Dialog DialogTreeViewTemplate;

		/// <summary>
		/// Popup template.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("popupSample")]
		protected Popup PopupTemplate;

		/// <summary>
		/// Modal popup template.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("popupModalSample")]
		protected Popup PopupModalTemplate;

		/// <summary>
		/// Show sticky notification.
		/// </summary>
		public void ShowNotifySticky()
		{
			NotifySimpleTemplate.Clone().Show(
				"Sticky Notification. Click on the × above to close.",
				customHideDelay: 0f);
		}

		/// <summary>
		/// Show 3 notification, one by one in this order:
		/// - Queue Notification 3
		/// - Queue Notification 2
		/// - Queue Notification 1
		/// </summary>
		public void ShowNotifyStack()
		{
			NotifySimpleTemplate.Clone().Show(
				"Stack Notification 1.",
				customHideDelay: 3f,
				sequenceType: NotifySequence.First);
			NotifySimpleTemplate.Clone().Show(
				"Stack Notification 2.",
				customHideDelay: 3f,
				sequenceType: NotifySequence.First);
			NotifySimpleTemplate.Clone().Show(
				"Stack Notification 3.",
				customHideDelay: 3f,
				sequenceType: NotifySequence.First);
		}

		/// <summary>
		/// Show 3 notification, one by one in this order:
		/// - Queue Notification 1.
		/// - Queue Notification 2.
		/// - Queue Notification 3.
		/// </summary>
		public void ShowNotifyQueue()
		{
			NotifySimpleTemplate.Clone().Show(
				"Queue Notification 1.",
				customHideDelay: 3f,
				sequenceType: NotifySequence.Last);
			NotifySimpleTemplate.Clone().Show(
				"Queue Notification 2.",
				customHideDelay: 3f,
				sequenceType: NotifySequence.Last);
			NotifySimpleTemplate.Clone().Show(
				"Queue Notification 3.",
				customHideDelay: 3f,
				sequenceType: NotifySequence.Last);
		}

		/// <summary>
		/// Show only one notification and hide current notifications from sequence, if exists.
		/// Will be displayed only Queue Notification 3.
		/// </summary>
		public void ShowNotifySequenceClear()
		{
			NotifySimpleTemplate.Clone().Show(
				"Stack Notification 1.",
				customHideDelay: 3f,
				sequenceType: NotifySequence.First);
			NotifySimpleTemplate.Clone().Show(
				"Stack Notification 2.",
				customHideDelay: 3f,
				sequenceType: NotifySequence.First);
			NotifySimpleTemplate.Clone().Show(
				"Stack Notification 3.",
				customHideDelay: 3f,
				sequenceType: NotifySequence.First,
				clearSequence: true);
		}

		/// <summary>
		/// Show notify and close after 3 seconds.
		/// </summary>
		public void ShowNotifyAutohide()
		{
			NotifyAutoHideTemplate.Clone().Show("Achievement unlocked. Hide after 3 seconds.", customHideDelay: 3f);
		}

		bool CallShowNotifyAutohide(DialogBase dialog, int buttonIndex)
		{
			ShowNotifyAutohide();
			return true;
		}

		/// <summary>
		/// Show notify with rotate animation.
		/// </summary>
		public void ShowNotifyAutohideRotate()
		{
			NotifyAutoHideTemplate.Clone().Show(
				"Achievement unlocked. Hide after 4 seconds.",
				customHideDelay: 4f,
				hideAnimation: NotificationBase.AnimationRotateVertical);
		}

		/// <summary>
		/// Show notify with collapse animation.
		/// </summary>
		public void ShowNotifyBlack()
		{
			NotifyAutoHideTemplate.Clone().Show(
				"Another Notification. Hide after 5 seconds or click on the × above to close.",
				customHideDelay: 5f,
				hideAnimation: NotificationBase.AnimationCollapseVertical,
				slideUpOnHide: false);
		}

		bool ShowNotifyYes(DialogBase dialog, int buttonIndex)
		{
			NotifyAutoHideTemplate.Clone().Show("Action on 'Yes' button click.", customHideDelay: 3f);
			return true;
		}

		bool ShowNotifyNo(DialogBase dialog, int buttonIndex)
		{
			NotifyAutoHideTemplate.Clone().Show("Action on 'No' button click.", customHideDelay: 3f);
			return true;
		}

		/// <summary>
		/// Show simple dialog.
		/// </summary>
		public void ShowDialogSimple()
		{
			var canvas = UtilitiesUI.FindTopmostCanvas(transform).GetComponent<Canvas>();

			var dialog = DialogSampleTemplate.Clone();

			var actions = new DialogButton[]
			{
				new DialogButton("Close", DialogBase.DefaultClose),
			};

			dialog.Show(
				title: "Simple Dialog",
				message: "Simple dialog with only close button.",
				buttons: actions,
				focusButton: "Close",
				canvas: canvas);
		}

		/// <summary>
		/// Show dialog in the same position when it was closed.
		/// </summary>
		public void ShowDialogInPosition()
		{
			var dialog = DialogSampleTemplate.Clone();

			var actions = new DialogButton[]
			{
				new DialogButton("Close", Close),
			};

			dialog.Show(
				title: "Simple Dialog",
				message: "Simple dialog with only close button.",
				buttons: actions,
				focusButton: "Close",
				position: dialog.transform.localPosition);
		}

		/// <summary>
		/// Check if dialog can be closed.
		/// </summary>
		/// <param name="dialog">Current dialog.</param>
		/// <param name="buttonIndex">Index of the clicked button.</param>
		/// <returns>true if dialog can be closed; otherwise, false.</returns>
		public virtual bool Close(DialogBase dialog, int buttonIndex)
		{
			return true;
		}

		bool CallShowDialogSimple(DialogBase dialog, int buttonIndex)
		{
			ShowDialogSimple();
			return true;
		}

		/// <summary>
		/// Show warning.
		/// </summary>
		public void ShowWarning()
		{
			var actions = new DialogButton[]
			{
				new DialogButton("OK", DialogBase.DefaultClose),
			};

			DialogSampleTemplate.Clone().Show(
				title: "Warning window",
				message: "Warning test",
				buttons: actions,
				focusButton: "OK",
				icon: AttentionIcon);
		}

		/// <summary>
		/// Show dialog with Yes/No/Cancel buttons.
		/// </summary>
		public void ShowDialogYesNoCancel()
		{
			var actions = new DialogButton[]
			{
				new DialogButton("Yes", ShowNotifyYes),
				new DialogButton("No", ShowNotifyNo),
				new DialogButton("Cancel", DialogBase.DefaultClose),
			};

			DialogSampleTemplate.Clone().Show(
				title: "Dialog Yes No Cancel",
				message: "Question?",
				buttons: actions,
				focusButton: "Yes",
				icon: QuestionIcon);
		}

		/// <summary>
		/// Show dialog with lots of text.
		/// </summary>
		public void ShowDialogExtended()
		{
			var actions = new DialogButton[]
			{
				new DialogButton("Show notification", CallShowNotifyAutohide),
				new DialogButton("Open simple dialog", CallShowDialogSimple),
				new DialogButton("Close", DialogBase.DefaultClose),
			};

			DialogSampleTemplate.Clone().Show(
				title: "Another Dialog",
				message: "Same template with another position and long text.\nChange\nheight\nto\nfit\ntext.",
				buttons: actions,
				focusButton: "Show notification",
				position: new Vector3(40, -40, 0));
		}

		/// <summary>
		/// Show modal dialog.
		/// </summary>
		public void ShowDialogModal()
		{
			var actions = new DialogButton[]
			{
				new DialogButton("Close", DialogBase.DefaultClose),
			};

			DialogSampleTemplate.Clone().Show(
				title: "Modal Dialog",
				message: "Simple Modal Dialog.",
				buttons: actions,
				focusButton: "Close",
				modal: true,
				modalColor: new Color(0, 0, 0, 0.8f));
		}

		/// <summary>
		/// Show sing-in dialog.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0302:Display class allocation to capture closure", Justification = "Required.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0301:Closure Allocation Source", Justification = "Required.")]
		public void ShowDialogSignIn()
		{
			// create dialog from template
			var dialog = DialogSignInTemplate.Clone();

			// helper component with references to input fields
			var helper = dialog.GetComponent<DialogInputHelper>();

			// reset input fields to default
			helper.Refresh();

			var actions = new DialogButton[]
			{
				// on click call SignInNotify
				new DialogButton("Sign in", SignInNotify),

				// on click close dialog
				new DialogButton("Cancel", DialogBase.DefaultClose),
			};

			// open dialog
			dialog.Show(
				title: "Sign into your Account",
				buttons: actions,
				focusButton: "Sign in",
				modal: true,
				modalColor: new Color(0, 0, 0, 0.8f));
		}

		// using dialog
		bool SignInNotify(DialogBase dialog, int index)
		{
			var helper = dialog.GetComponent<DialogInputHelper>();

			// return true if Username.text and Password not empty; otherwise, false
			if (!helper.Validate())
			{
				// return false to keep dialog open
				return false;
			}

			// using dialog input
			var message = string.Format("Sign in.\nUsername: {0}\nPassword: <hidden>", helper.UsernameAdapter.text);
			NotifyAutoHideTemplate.Clone().Show(message, customHideDelay: 3f);

			// return true to close dialog
			return true;
		}

		/// <summary>
		/// Show dialog with TreeView.
		/// </summary>
		public void ShowDialogTreeView()
		{
			// create dialog from template
			var dialog = DialogTreeViewTemplate.Clone();

			// helper component with references to input fields
			var helper = dialog.GetComponent<DialogTreeViewInputHelper>();

			var actions = new DialogButton[]
			{
				// on click close dialog
				new DialogButton("Close", DialogBase.DefaultClose),
			};

			// open dialog
			dialog.Show(
				title: "Dialog with TreeView",
				buttons: actions,
				focusButton: "Close",
				modal: true,
				modalColor: new Color(0, 0, 0, 0.8f));

			// reset input fields to default
			helper.Refresh();
		}

		/// <summary>
		/// Show simple popup.
		/// </summary>
		public void ShowPopup()
		{
			PopupTemplate.Clone().Show(
				title: "Simple Popup",
				message: "Simple Popup.");
		}

		/// <summary>
		/// Show modal popup.
		/// </summary>
		public void ShowPopupModal()
		{
			PopupModalTemplate.Clone().Show(
				title: "Modal Popup",
				message: "Alert text.",
				modal: true,
				modalColor: new Color(0.0f, 0.0f, 0.0f, 0.8f));
		}
	}
}