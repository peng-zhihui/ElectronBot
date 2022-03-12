namespace UIWidgets.Examples
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Drop gameobject to terrain.
	/// Required PhysicsRaycaster on camera.
	/// </summary>
	public class DropGameObject2Terrain : MonoBehaviour, IDropSupport<GameObject>
	{
		class RaycastHitComparer : IComparer<RaycastHit>
		{
			public int Compare(RaycastHit x, RaycastHit y)
			{
				return x.distance.CompareTo(y.distance);
			}
		}

		struct DropData
		{
			public bool Allow;
			public Vector3 Position;
		}

		/// <summary>
		/// Pivot fix (for the demo purpose).
		/// </summary>
		[SerializeField]
		protected Vector3 PivotFix = new Vector3(0, 2f, 0f);

		RaycastHit[] hits = new RaycastHit[10];

		RaycastHitComparer comparer = new RaycastHitComparer();

		/// <inheritdoc/>
		public void Drop(GameObject data, PointerEventData eventData)
		{
			var drop = ProcessEvent(data, eventData);

			if (drop.Allow)
			{
				var instance = Instantiate(data);
				instance.transform.position = drop.Position + PivotFix;
			}

			data.SetActive(false);
		}

		/// <inheritdoc/>
		public bool CanReceiveDrop(GameObject data, PointerEventData eventData)
		{
			var drop = ProcessEvent(data, eventData);

			data.transform.position = drop.Position + PivotFix;
			data.SetActive(drop.Allow);

			return drop.Allow;
		}

		/// <inheritdoc/>
		public void DropCanceled(GameObject data, PointerEventData eventData)
		{
			data.SetActive(false);
		}

		DropData ProcessEvent(GameObject target, PointerEventData eventData)
		{
			var result = default(DropData);

			var instance_id = gameObject.GetInstanceID();
			var target_id = target.GetInstanceID();

			var ray = Camera.main.ScreenPointToRay(eventData.position);
			var hits_count = Physics.RaycastNonAlloc(ray, hits);

			Array.Sort(hits, 0, hits_count, comparer);

			var terrain_id = 0;
			for (int i = 0; i < hits_count; i++)
			{
				var hit = hits[i];
				var hit_id = hit.transform.gameObject.GetInstanceID();
				if (hit_id == target_id)
				{
					terrain_id += 1;
					continue;
				}

				if (hit_id != instance_id)
				{
					continue;
				}

				result.Allow = i == terrain_id;
				result.Position = hit.point;
				break;
			}

			return result;
		}
	}
}