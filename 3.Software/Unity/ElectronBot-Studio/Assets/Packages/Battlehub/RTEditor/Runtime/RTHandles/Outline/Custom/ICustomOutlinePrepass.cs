using UnityEngine;

namespace Battlehub.RTHandles
{
    /// <summary>
    /// Interface to allow rendering "outline" selection in the runtime editor with a different material.
    /// 
    /// Material should render the full relevant area in opaque red rgba(1, 0, 0, 1)
    /// </summary>
    public interface ICustomOutlinePrepass
    {
        Renderer GetRenderer();
        Material GetOutlinePrepassMaterial();
    }
}