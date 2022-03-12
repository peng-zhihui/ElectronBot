using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;

namespace UnityEditor.UI {
	/// <summary>
	/// This class adds a Menu Item "GameObject/UI/Procedural Image"
	/// Bahviour of this command is the same as with regular Images
	/// </summary>
	public class ProceduralImageEditorUtility {
		[MenuItem("GameObject/UI/Procedural Image")]
		public static void AddProceduralImage(){
			GameObject o = new GameObject ();
			o.AddComponent<ProceduralImage> ();
			o.layer = LayerMask.NameToLayer("UI");
			o.name = "Procedural Image";
			if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponentInParent<Canvas> () != null) {
				o.transform.SetParent (Selection.activeGameObject.transform, false);
				Selection.activeGameObject = o;
			}
			else {
				if(GameObject.FindObjectOfType<Canvas>()==null)	{
					EditorApplication.ExecuteMenuItem("GameObject/UI/Canvas");
				}
				Canvas c = GameObject.FindObjectOfType<Canvas>();

                //Set Texcoord shader channels for canvas
                c.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.TexCoord2 | AdditionalCanvasShaderChannels.TexCoord3;

                o.transform.SetParent (c.transform, false);
				Selection.activeGameObject = o;
			}
		}
		/// <summary>
		/// Replaces an Image Component with a Procedural Image Component.
		/// </summary>
		[MenuItem("CONTEXT/Image/Replace with Procedural Image")]
		public static void ReplaceWithProceduralImage(MenuCommand command){
			Image image = (Image)command.context;
			GameObject obj = image.gameObject;
			GameObject.DestroyImmediate (image);
			obj.AddComponent<ProceduralImage> ();
		}
	}
}
