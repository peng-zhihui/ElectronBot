namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// Style will be applied to specified object.
	/// Should be used for composite widgets when inner widget not styled automatically like Table with TableHeader.
	/// </summary>
	[Serializable]
	public class StyleSupportAny : MonoBehaviour, IStylable
	{
		[SerializeField]
		[FormerlySerializedAs("Object")]
		[HideInInspector]
		GameObject obj;

		/// <summary>
		/// GameObject to apply style.
		/// </summary>
		[Obsolete("Replaced with Objects.")]
		public GameObject Object
		{
			get
			{
				return obj;
			}
		}

		/// <summary>
		/// GameObjects to apply style.
		/// </summary>
		[SerializeField]
		public GameObject[] Objects = Compatibility.EmptyArray<GameObject>();

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			for (var i = 0; i < Objects.Length; i++)
			{
				style.ApplyTo(Objects[i]);
			}

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			for (var i = 0; i < Objects.Length; i++)
			{
				style.GetFrom(Objects[i]);
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
			var has_object = obj != null;
			var no_objects = (Objects == null) || (Objects.Length == 0);
			if (has_object && no_objects)
			{
				Objects = new[] { obj };
				obj = null;
			}
		}
#endif
	}
}