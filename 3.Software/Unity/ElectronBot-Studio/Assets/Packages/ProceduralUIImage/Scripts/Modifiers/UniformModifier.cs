using UnityEngine;
using UnityEngine.UI.ProceduralImage;

[ModifierID("Uniform")]
public class UniformModifier : ProceduralImageModifier {
	[SerializeField]private float radius;

	public float Radius {
		get {
			return radius;
		}
		set {
			radius = value;
			_Graphic.SetVerticesDirty();
		}
	}

	#region implemented abstract members of ProceduralImageModifier

	public override Vector4 CalculateRadius (Rect imageRect){
		float r = this.radius;
		return new Vector4(r,r,r,r);
	}

	#endregion
	
}
