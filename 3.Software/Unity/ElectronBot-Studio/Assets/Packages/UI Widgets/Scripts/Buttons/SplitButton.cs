namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Split Button.
	/// </summary>
	public class SplitButton : MonoBehaviour, IStylable
	{
		/// <summary>
		/// Primary button.
		/// </summary>
		[SerializeField]
		protected Button primaryButton;

		/// <summary>
		/// Primary button.
		/// </summary>
		public Button PrimaryButton
		{
			get
			{
				return primaryButton;
			}

			set
			{
				if (primaryButton != value)
				{
					DisablePrimaryButton(primaryButton);

					primaryButton = value;

					EnablePrimaryButton(primaryButton);
				}
			}
		}

		/// <summary>
		/// Toggle button.
		/// </summary>
		[SerializeField]
		protected Button toggleButton;

		/// <summary>
		/// Toggle button.
		/// </summary>
		public Button ToggleButton
		{
			get
			{
				return toggleButton;
			}

			set
			{
				if (toggleButton != value)
				{
					DisableToggleButton(toggleButton);

					toggleButton = value;

					EnableToggleButton(toggleButton);
				}
			}
		}

		/// <summary>
		/// Block with additional buttons.
		/// </summary>
		[SerializeField]
		protected GameObject additionalButtonsBlock;

		/// <summary>
		/// Block with additional buttons.
		/// </summary>
		public GameObject AdditionalButtonsBlock
		{
			get
			{
				return additionalButtonsBlock;
			}

			set
			{
				if (additionalButtonsBlock != value)
				{
					EnableAdditionalButtonsBlock(additionalButtonsBlock);

					additionalButtonsBlock = value;

					DisableAdditionalButtonsBlock(additionalButtonsBlock);
				}
			}
		}

		/// <summary>
		/// Additional buttons.
		/// </summary>
		[SerializeField]
		protected List<Button> additionalButtons = new List<Button>();

		/// <summary>
		/// Additional buttons.
		/// </summary>
		public List<Button> AdditionalButtons
		{
			get
			{
				return additionalButtons;
			}

			set
			{
				if (additionalButtons != value)
				{
					EnableAdditionalButtons(additionalButtons);

					additionalButtons = value;

					DisableAdditionalButtons(additionalButtons);
				}
			}
		}

		/// <summary>
		/// Modal sprite.
		/// </summary>
		[SerializeField]
		public Sprite ModalSprite = null;

		/// <summary>
		/// Modal color.
		/// </summary>
		[SerializeField]
		public Color ModalColor = new Color(0, 0, 0, 0.0f);

		/// <summary>
		/// Modal key.
		/// </summary>
		[NonSerialized]
		protected int? ModalKey;

		/// <summary>
		/// Is open?
		/// </summary>
		[NonSerialized]
		protected bool IsOpen;

		/// <summary>
		/// AdditionalButtons parent.
		/// </summary>
		[NonSerialized]
		protected RectTransform OriginalParent;

		/// <summary>
		/// Sibling index.
		/// </summary>
		[NonSerialized]
		protected int SiblingIndex;

		/// <summary>
		/// Parent canvas.
		/// </summary>
		[SerializeField]
		public RectTransform ParentCanvas;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			EnablePrimaryButton(PrimaryButton);
			EnableToggleButton(ToggleButton);
			EnableAdditionalButtonsBlock(AdditionalButtonsBlock);
			EnableAdditionalButtons(AdditionalButtons);
		}

		/// <summary>
		/// Enable primary button.
		/// </summary>
		/// <param name="button">Button.</param>
		protected void EnablePrimaryButton(Button button)
		{
			if (button != null)
			{
				button.onClick.AddListener(Close);
			}
		}

		/// <summary>
		/// Disable primary button.
		/// </summary>
		/// <param name="button">Button.</param>
		protected void DisablePrimaryButton(Button button)
		{
			if (button != null)
			{
				button.onClick.RemoveListener(Close);
			}
		}

		/// <summary>
		/// Enable toggle button.
		/// </summary>
		/// <param name="button">Button.</param>
		protected void EnableToggleButton(Button button)
		{
			if (button != null)
			{
				button.onClick.AddListener(Toggle);
			}
		}

		/// <summary>
		/// Disable toggle button.
		/// </summary>
		/// <param name="button">Button.</param>
		protected void DisableToggleButton(Button button)
		{
			if (button != null)
			{
				button.onClick.RemoveListener(Toggle);
			}
		}

		/// <summary>
		/// Enable block with additional buttons.
		/// </summary>
		/// <param name="go">Block.</param>
		protected virtual void EnableAdditionalButtonsBlock(GameObject go)
		{
			if (go.activeSelf)
			{
				go.SetActive(false);
			}
		}

		/// <summary>
		/// Disable block with additional buttons.
		/// </summary>
		/// <param name="go">Block.</param>
		protected virtual void DisableAdditionalButtonsBlock(GameObject go)
		{
		}

		/// <summary>
		/// Enable additional buttons.
		/// </summary>
		/// <param name="buttons">Buttons.</param>
		protected void EnableAdditionalButtons(List<Button> buttons)
		{
			foreach (var button in buttons)
			{
				if (button != null)
				{
					button.onClick.AddListener(Close);
				}
			}
		}

		/// <summary>
		/// Disable additional buttons.
		/// </summary>
		/// <param name="buttons">Buttons.</param>
		protected void DisableAdditionalButtons(List<Button> buttons)
		{
			foreach (var button in buttons)
			{
				if (button != null)
				{
					button.onClick.RemoveListener(Close);
				}
			}
		}

		/// <summary>
		/// Process destroy event.
		/// </summary>
		protected virtual void Destroy()
		{
			DisableToggleButton(ToggleButton);
			DisableAdditionalButtonsBlock(AdditionalButtonsBlock);
			DisableAdditionalButtons(AdditionalButtons);
		}

		/// <summary>
		/// Toggle.
		/// </summary>
		public void Toggle()
		{
			if (IsOpen)
			{
				Close();
			}
			else
			{
				Open();
			}
		}

		/// <summary>
		/// Open.
		/// </summary>
		public void Open()
		{
			if (IsOpen)
			{
				return;
			}

			IsOpen = true;

			if (ParentCanvas == null)
			{
				ParentCanvas = UtilitiesUI.FindTopmostCanvas(transform);
			}

			if (ParentCanvas != null)
			{
				OriginalParent = transform.parent as RectTransform;
				transform.SetParent(ParentCanvas, true);
			}

			SiblingIndex = transform.GetSiblingIndex();

			ModalKey = ModalHelper.Open(this, ModalSprite, ModalColor, Close, ParentCanvas);

			transform.SetAsLastSibling();
			AdditionalButtonsBlock.SetActive(true);
		}

		/// <summary>
		/// Close.
		/// </summary>
		public void Close()
		{
			if (!IsOpen)
			{
				return;
			}

			IsOpen = false;

			if (ModalKey.HasValue)
			{
				ModalHelper.Close(ModalKey.Value);
			}

			AdditionalButtonsBlock.SetActive(false);

			transform.SetParent(OriginalParent, true);
			transform.SetSiblingIndex(SiblingIndex);
		}

		#region IStylable implementation

		/// <inheritdoc/>
		public bool SetStyle(Style style)
		{
			if (PrimaryButton != null)
			{
				style.ApplyTo(PrimaryButton.gameObject);
			}

			if (ToggleButton != null)
			{
				style.Spinner.ButtonMinus.ApplyTo(ToggleButton.gameObject);
			}

			foreach (var button in AdditionalButtons)
			{
				if (button != null)
				{
					style.ApplyTo(button.gameObject);
				}
			}

			return true;
		}

		/// <inheritdoc/>
		public bool GetStyle(Style style)
		{
			if (PrimaryButton != null)
			{
				style.GetFrom(PrimaryButton.gameObject);
			}

			if (ToggleButton != null)
			{
				style.Spinner.ButtonMinus.GetFrom(ToggleButton.gameObject);
			}

			foreach (var button in AdditionalButtons)
			{
				if (button != null)
				{
					style.GetFrom(button.gameObject);
				}
			}

			return true;
		}
		#endregion

		#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected virtual void OnValidate()
		{
			if (ParentCanvas == null)
			{
				ParentCanvas = UtilitiesUI.FindTopmostCanvas(transform);
			}
		}

		/// <summary>
		/// Reset this instance.
		/// </summary>
		protected virtual void Reset()
		{
			if (ParentCanvas == null)
			{
				ParentCanvas = UtilitiesUI.FindTopmostCanvas(transform);
			}
		}
		#endif
	}
}