namespace UnityEngine.UI.ProceduralImage
{
	[System.AttributeUsage(System.AttributeTargets.Class)]
	public class ModifierID : System.Attribute{
		private string name;
		
		public ModifierID(string name){
			this.name = name;
		}
		
		public string Name{
			get{
				return name;
			}
		}
	}
}