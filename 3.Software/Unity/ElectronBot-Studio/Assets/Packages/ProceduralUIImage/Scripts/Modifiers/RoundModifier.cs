using UnityEngine;
using System.Collections;
using UnityEngine.UI.ProceduralImage;

[ModifierID("Round")]
public class RoundModifier : ProceduralImageModifier {
	#region implemented abstract members of ProceduralImageModifier
	public override Vector4 CalculateRadius (Rect imageRect){
		float r = Mathf.Min (imageRect.width,imageRect.height)*0.5f;
		return new Vector4 (r,r,r,r);
	}
	#endregion
}
