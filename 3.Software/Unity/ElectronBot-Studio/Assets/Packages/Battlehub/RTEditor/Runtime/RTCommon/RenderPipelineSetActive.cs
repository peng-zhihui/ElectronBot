using UnityEngine;

namespace Battlehub.RTCommon
{
    public class RenderPipelineSetActive : MonoBehaviour
    {
        public RPType PipelineType;
        public bool IsActive;

        private void Awake()
        {
            if(RenderPipelineInfo.Type == PipelineType)
            {
                gameObject.SetActive(IsActive);
            }
        }
    }
}
