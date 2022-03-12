using UnityEngine;
using UnityEngine.UI.ProceduralImage;

/* Uncomment this to work from it as a base for your own modifier
 * 
 * 
[ModifierID("Your Modifier Identity here")]
public class CustomPremadeModifier : ProceduralImageModifier {

	#region implemented abstract members of ProceduralImageModifier

	public override Vector4 CalculateRadius (Rect imageRect){
		float r = Mathf.Min (imageRect.width,imageRect.height)*0.5f;
		return new Vector4(r,r,r,0);
	}

	#endregion
	
}
*/
