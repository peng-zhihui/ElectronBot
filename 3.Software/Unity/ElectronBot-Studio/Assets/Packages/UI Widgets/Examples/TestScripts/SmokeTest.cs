namespace UIWidgets.Tests
{
	using System;
	using System.Collections;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// Smoke test.
	/// </summary>
	public class SmokeTest : MonoBehaviour
	{
		/// <summary>
		/// Accordion.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("accordion")]
		protected Accordion Accordion;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			#if UNITY_STANDALONE
			var args = System.Environment.GetCommandLineArgs();
			if (Array.IndexOf<string>(args, "-smoke-test") != -1)
			{
				StartCoroutine(SimpleTest());
			}
			#endif
		}

		/// <summary>
		/// Simple test.
		/// </summary>
		/// <returns>Coroutine.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0401:Possible allocation of reference type enumerator", Justification = "Enumerator is reusable.")]
		protected IEnumerator SimpleTest()
		{
			yield return UtilitiesTime.Wait(5f, true);

			var items = Accordion.DataSource;
			if (!Accordion.DataSource[0].Open || !Accordion.DataSource[0].ContentObject.activeSelf)
			{
				throw new UnityException("Overview is not active!");
			}

			foreach (var item in items)
			{
				if (item.ToggleObject.name == "Exit")
				{
					continue;
				}

				Accordion.ToggleItem(item);
				yield return UtilitiesTime.Wait(5f, true);
			}

			Application.Quit();
		}
	}
}