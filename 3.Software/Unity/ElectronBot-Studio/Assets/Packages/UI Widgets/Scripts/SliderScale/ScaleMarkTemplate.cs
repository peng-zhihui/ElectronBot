namespace UIWidgets
{
	using System.Collections.Generic;
	using UIWidgets.Styles;
	using UnityEngine;

	/// <summary>
	/// Scale mark template.
	/// </summary>
	public class ScaleMarkTemplate : MonoBehaviour, IStylable
	{
		/// <summary>
		/// Mark.
		/// </summary>
		[SerializeField]
		public RectTransform Mark;

		/// <summary>
		/// Label.
		/// </summary>
		[SerializeField]
		public TextAdapter Label;

		/// <summary>
		/// Original mark.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected ScaleMarkTemplate OriginalMark;

		/// <summary>
		/// Cache.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<ScaleMarkTemplate> Cache = new List<ScaleMarkTemplate>();

		RectTransform rectTransform;

		/// <summary>
		/// RectTransform.
		/// </summary>
		public RectTransform RectTransform
		{
			get
			{
				if (rectTransform == null)
				{
					rectTransform = transform as RectTransform;
				}

				return rectTransform;
			}
		}

		/// <summary>
		/// Get instance.
		/// </summary>
		/// <param name="parent">Parent gameobject.</param>
		/// <returns>Mark instance.</returns>
		public virtual ScaleMarkTemplate GetInstance(RectTransform parent)
		{
			if (OriginalMark != null)
			{
				return null;
			}

			ScaleMarkTemplate instance;
			if (Cache.Count > 0)
			{
				var last = Cache.Count - 1;
				instance = Cache[last];
				Cache.RemoveAt(last);
			}
			else
			{
				instance = Compatibility.Instantiate(this);
				Utilities.FixInstantiated(this, instance);
			}

			instance.gameObject.SetActive(true);
			instance.RectTransform.SetParent(parent, false);
			instance.OriginalMark = this;

			return instance;
		}

		/// <summary>
		/// Return mark instance to cache.
		/// </summary>
		public virtual void Return()
		{
			if (OriginalMark == null)
			{
				return;
			}

			OriginalMark.Cache.Add(this);
			gameObject.SetActive(false);
		}

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			style.Scale.MarkLine.ApplyTo(Mark);

			if (Label != null)
			{
				style.Scale.MarkLabel.ApplyTo(Label.gameObject);
			}

			for (int i = 0; i < Cache.Count; i++)
			{
				Cache[i].SetStyle(style);
			}

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			style.Scale.MarkLine.GetFrom(Mark);

			if (Label != null)
			{
				style.Scale.MarkLabel.GetFrom(Label.gameObject);
			}

			return true;
		}
		#endregion

	}
}