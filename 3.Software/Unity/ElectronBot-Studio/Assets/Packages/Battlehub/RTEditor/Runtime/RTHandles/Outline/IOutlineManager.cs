using Battlehub.RTCommon;
using UnityEngine;

namespace Battlehub.RTHandles
{
    public interface IOutlineManager
    {
        IRuntimeSelection Selection
        {
            get;
            set;
        }
        bool ContainsRenderer(Renderer renderer);
        void AddRenderers(Renderer[] renderers);
        void RemoveRenderers(Renderer[] renderers);
        void RecreateCommandBuffer();
    }
}

