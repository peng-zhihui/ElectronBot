using UnityEngine;

namespace Battlehub.RTHandles
{
    public class BillboardFacingCamera : MonoBehaviour
    {
        private void OnWillRenderObject()
        {
            transform.LookAt(transform.position - (Camera.current.transform.position - transform.position), Vector3.up);
        }
    }
}


