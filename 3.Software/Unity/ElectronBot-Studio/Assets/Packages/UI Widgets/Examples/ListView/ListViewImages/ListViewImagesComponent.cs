namespace UIWidgets.Examples
{
	using System.Collections;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// ListViewImages component.
	/// </summary>
	public class ListViewImagesComponent : ListViewItem, IViewData<ListViewImagesItem>
	{
		/// <summary>
		/// Url.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with UrlAdapter.")]
		public Text Url;

		/// <summary>
		/// Url.
		/// </summary>
		[SerializeField]
		public TextAdapter UrlAdapter;

		/// <summary>
		/// Image.
		/// </summary>
		[SerializeField]
		public RawImage Image;

		/// <summary>
		/// Sprite Image.
		/// </summary>
		[SerializeField]
		public Image SpriteImage;

		/// <summary>
		/// Image LayoutElement.
		/// </summary>
		[SerializeField]
		protected LayoutElement ImageLayoutElement;

		/// <summary>
		/// Current item.
		/// </summary>
		protected ListViewImagesItem Item;

		/// <summary>
		/// Is image loading?
		/// </summary>
		protected bool IsLoading;

		/// <summary>
		/// Load coroutine.
		/// </summary>
		protected IEnumerator LoadCoroutine;

		/// <summary>
		/// Handle OnEnable event.
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();

			if (IsLoading)
			{
				return;
			}

			StartLoadImage();
		}

		/// <summary>
		/// Handle OnDisable event.
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable();

			if (IsLoading)
			{
				IsLoading = false;
				StopCoroutine(LoadCoroutine);
			}
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(ListViewImagesItem item)
		{
			// save item so later can fix item.Height to actual value
			Item = item;

			if (UrlAdapter != null)
			{
				UrlAdapter.text = !string.IsNullOrEmpty(Item.Url) ? Item.Url : "No image";
			}

			SetImage();

			StartLoadImage();
		}

		void StartLoadImage()
		{
			if ((Item != null) && (!string.IsNullOrEmpty(Item.Url)) && (Item.Texture == null))
			{
				LoadCoroutine = LoadImage();
				StartCoroutine(LoadCoroutine);
			}
		}

		void SetImage()
		{
			if (Image != null)
			{
				Image.color = Color.white;

				if (Item.Texture != null)
				{
					Image.texture = Item.Texture;
					if (ImageLayoutElement != null)
					{
						ImageLayoutElement.preferredWidth = Item.Texture.width;
						ImageLayoutElement.preferredHeight = Item.Texture.height;
					}
				}
				else
				{
					Image.texture = null;

					if (string.IsNullOrEmpty(Item.Url))
					{
						Image.color = Color.clear;
						if (ImageLayoutElement != null)
						{
							ImageLayoutElement.minWidth = -1;
							ImageLayoutElement.minHeight = -1;
						}
					}
					else
					{
						if (ImageLayoutElement != null)
						{
							ImageLayoutElement.minWidth = 100;
							ImageLayoutElement.minHeight = 100;
						}
					}
				}
			}
			else if (SpriteImage != null)
			{
				SpriteImage.color = Color.white;

				if (Item.Sprite != null)
				{
					SpriteImage.sprite = Item.Sprite;
					if (ImageLayoutElement != null)
					{
						ImageLayoutElement.preferredWidth = Item.Sprite.rect.width;
						ImageLayoutElement.preferredHeight = Item.Sprite.rect.height;
					}
				}
				else
				{
					SpriteImage.sprite = null;

					if (string.IsNullOrEmpty(Item.Url))
					{
						SpriteImage.color = Color.clear;
						if (ImageLayoutElement != null)
						{
							ImageLayoutElement.minHeight = -1;
							ImageLayoutElement.minWidth = -1;
						}
					}
					else
					{
						if (ImageLayoutElement != null)
						{
							ImageLayoutElement.minHeight = 100;
							ImageLayoutElement.minWidth = 100;
						}
					}
				}
			}
		}

		IEnumerator LoadImage()
		{
			var item = Item;

			if (item.Texture == null)
			{
				IsLoading = true;

				yield return null;

#if UNITY_2018_3_OR_NEWER
				using (var www = UnityEngine.Networking.UnityWebRequest.Get(new System.Uri(item.Url)))
				{
					yield return www.SendWebRequest();

					if (Compatibility.IsError(www))
					{
						Debug.Log(www.error);
					}
					else
					{
						item.Texture = UnityEngine.Networking.DownloadHandlerTexture.GetContent(www);
					}
				}
#else
				var www = new WWW(item.Url);

				yield return www;

				item.Texture = www.texture;

				www.Dispose();
#endif

				IsLoading = false;
			}

			if (item == Item)
			{
				SetImage();
			}
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public override void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.GetOrAddComponent(Url, ref UrlAdapter);
#pragma warning restore 0612, 0618
		}
	}
}