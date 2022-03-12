using UnityEngine;
using System.Collections;
using UnityEngine.UI.ProceduralImage;

[ModifierID("Only One Edge")]
public class OnlyOneEdgeModifier : ProceduralImageModifier {
	[SerializeField]private float radius;
	[SerializeField]private ProceduralImageEdge side;

	public enum ProceduralImageEdge{
		Top,
		Bottom,
		Left,
		Right
	}

	public float Radius {
		get {
			return radius;
		}
		set {
			radius = value;
			_Graphic.SetVerticesDirty();
		}
	}

	public ProceduralImageEdge Side {
		get {
			return side;
		}
		set {
			side = value;
		}
	}

	#region implemented abstract members of ProceduralImageModifier

	public override Vector4 CalculateRadius (Rect imageRect){
		switch (side) {
		case ProceduralImageEdge.Top:
				return new Vector4(radius,radius,0,0);
		case ProceduralImageEdge.Right:
				return new Vector4(0,radius,radius,0);
		case ProceduralImageEdge.Bottom:
				return new Vector4(0,0,radius,radius);
		case ProceduralImageEdge.Left:
				return new Vector4(radius,0,0,radius);
		default:
				return new Vector4(0,0,0,0);
		}
	}

	#endregion
}


