using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.UI.ProceduralImage
{
	[DisallowMultipleComponent]
	public abstract class ProceduralImageModifier : MonoBehaviour {
		/// <summary>
		/// Calculates the border-radius for Procedural Image.
		/// </summary>
		/// <returns>The radius as Vector4. (started top-left, clockwise)</returns>
		/// <param name="imageRect">Rect of ProceduralImages RectTransform</param>
		
		protected Graphic graphic;

		protected Graphic _Graphic{
			get{
				if(graphic == null){
					graphic = this.GetComponent<Graphic>();
				}
				return graphic;
			}
		}
		public abstract Vector4 CalculateRadius (Rect imageRect);
	}
}