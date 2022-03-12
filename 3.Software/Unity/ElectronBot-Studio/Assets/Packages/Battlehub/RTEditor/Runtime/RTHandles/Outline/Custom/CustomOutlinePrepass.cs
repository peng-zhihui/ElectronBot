using UnityEngine;

namespace Battlehub.RTHandles
{
    /// <summary>
    /// Default Implementation of ICustomOutlinePrepass.
    /// Attach to a gameobject to use a custom material for rendering the outline
    /// (e.g. when using vertex displacement or transparency in the material)
    /// </summary>
    public class CustomOutlinePrepass: MonoBehaviour, ICustomOutlinePrepass
    {
        public Renderer Renderer;
        public Material PrepassMaterial;

        public Renderer GetRenderer()
        {
            return Renderer;
        }
        
        public Material GetOutlinePrepassMaterial()
        {
            return PrepassMaterial;
        }
    }
}