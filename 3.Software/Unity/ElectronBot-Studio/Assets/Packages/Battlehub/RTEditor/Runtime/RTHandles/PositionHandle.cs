using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Battlehub.RTCommon;

namespace Battlehub.RTHandles
{
    [DefaultExecutionOrder(1)]
    public class PositionHandle : BaseHandle
    {
        public float GridSize = 1.0f;
        
        private Vector3 m_cursorPosition;
        private Vector3 m_currentPosition;

        private Vector3 m_prevPoint;
        private Matrix4x4 m_matrix;
        private Matrix4x4 m_inverse;

        private Vector2 m_prevMousePosition;
        private int[] m_targetLayers;
        private Transform[] m_snapTargets;
        private Bounds[] m_snapTargetsBounds;
        private ExposeToEditor[] m_allExposedToEditor;
    
        public override float SizeOfGrid
        {
            get { return GridSize; }
            set { GridSize = value; }
        }
        protected override float CurrentGridUnitSize
        {
            get { return SizeOfGrid; }
        }

        public bool SnapToGround
        {
            get;
            set;
        }

        private bool m_isInVertexSnappingMode = false;
        public bool IsInVertexSnappingMode
        {
            get { return m_isInVertexSnappingMode; }
            set
            {
                m_isInVertexSnappingMode = value;

                if(m_isInVertexSnappingMode)
                {
                    if (LockObject == null || !LockObject.IsPositionLocked)
                    {
                        if (Window.Pointer.XY(Position, out m_prevMousePosition))
                        {
                            BeginSnap();
                        }
                    }
                }
                else
                {
                    SelectedAxis = RuntimeHandleAxis.None;
                    if (!(IsInVertexSnappingMode || Editor.Tools.IsSnapping))
                    {
                        m_handleOffset = Vector3.zero;
                    }
                }

                if (Model != null && Model is PositionHandleModel)
                {
                    ((PositionHandleModel)Model).IsVertexSnapping = value;
                }
            }
        }

        private Vector3[] m_boundingBoxCorners = new Vector3[8];
        private Vector3 m_handleOffset;
        public override Vector3 Position
        {
            get { return transform.position + m_handleOffset; }
            set
            {
                transform.position = value - m_handleOffset;
            }
        }

        public override RuntimeTool Tool
        {
            get { return RuntimeTool.Move; }
        }


        protected override void OnEnable()
        {
            base.OnEnable();
        
            BaseHandleInput input = GetComponent<BaseHandleInput>();
            if (input == null || input.Handle != this)
            {
                input = gameObject.AddComponent<PositionHandleInput>();
                input.Handle = this;
            }

            m_isInVertexSnappingMode = false;
            Editor.Tools.IsSnapping = false;
            m_handleOffset = Vector3.zero;
            m_targetLayers = null;
            m_snapTargets = null;
            m_snapTargetsBounds = null;
            m_allExposedToEditor = null;

            Editor.Tools.IsSnappingChanged += OnSnappingChanged;
            OnSnappingChanged();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if(Window != null && Editor != null)
            {
                Editor.Tools.IsSnapping = false;
                Editor.Tools.IsSnappingChanged -= OnSnappingChanged;
            }
            
            m_targetLayers = null;
            m_snapTargets = null;
            m_snapTargetsBounds = null;
            m_allExposedToEditor = null;
        }

        protected override void UpdateOverride()
        {
            base.UpdateOverride();
            if (Editor.Tools.IsViewing)
            {
                SelectedAxis = RuntimeHandleAxis.None;
                return;
            }

            if (!IsWindowActive || !Window.IsPointerOver)
            {
                return;
            }
            IRTE editor = Editor;
            if (IsDragging)
            {
                if (SnapToGround && SelectedAxis != RuntimeHandleAxis.Y)
                {
                    SnapActiveTargetsToGround(ActiveTargets, Window.Camera, true);
                    transform.position = Targets[0].position;
                }
            }

            if (IsInVertexSnappingMode || Editor.Tools.IsSnapping)
            {
                Vector2 mousePosition;
                if(Window.Pointer.XY(Position, out mousePosition))
                {
                    if (editor.Tools.SnappingMode == SnappingMode.BoundingBox)
                    {
                        if (IsDragging)
                        {
                            SelectedAxis = RuntimeHandleAxis.Snap;
                            if (m_prevMousePosition != mousePosition)
                            {
                                m_prevMousePosition = mousePosition;
                                float minDistance = float.MaxValue;
                                Vector3 minPoint = Vector3.zero;
                                bool minPointFound = false;
                                for (int i = 0; i < m_allExposedToEditor.Length; ++i)
                                {
                                    ExposeToEditor exposeToEditor = m_allExposedToEditor[i];
                                    Bounds bounds = exposeToEditor.Bounds;
                                    m_boundingBoxCorners[0] = bounds.center + new Vector3(bounds.extents.x, bounds.extents.y, bounds.extents.z);
                                    m_boundingBoxCorners[1] = bounds.center + new Vector3(bounds.extents.x, bounds.extents.y, -bounds.extents.z);
                                    m_boundingBoxCorners[2] = bounds.center + new Vector3(bounds.extents.x, -bounds.extents.y, bounds.extents.z);
                                    m_boundingBoxCorners[3] = bounds.center + new Vector3(bounds.extents.x, -bounds.extents.y, -bounds.extents.z);
                                    m_boundingBoxCorners[4] = bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, bounds.extents.z);
                                    m_boundingBoxCorners[5] = bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, -bounds.extents.z);
                                    m_boundingBoxCorners[6] = bounds.center + new Vector3(-bounds.extents.x, -bounds.extents.y, bounds.extents.z);
                                    m_boundingBoxCorners[7] = bounds.center + new Vector3(-bounds.extents.x, -bounds.extents.y, -bounds.extents.z);
                                    GetMinPoint(ref minDistance, ref minPoint, ref minPointFound, exposeToEditor.BoundsObject.transform);
                                }

                                if (minPointFound)
                                {
                                    Position = minPoint;
                                }
                            }
                        }
                        else
                        {
                            SelectedAxis = RuntimeHandleAxis.None;
                            if (m_prevMousePosition != mousePosition)
                            {
                                m_prevMousePosition = mousePosition;

                                float minDistance = float.MaxValue;
                                Vector3 minPoint = Vector3.zero;
                                bool minPointFound = false;
                                for (int i = 0; i < m_snapTargets.Length; ++i)
                                {
                                    Transform snapTarget = m_snapTargets[i];
                                    Bounds bounds = m_snapTargetsBounds[i];

                                    m_boundingBoxCorners[0] = bounds.center + new Vector3(bounds.extents.x, bounds.extents.y, bounds.extents.z);
                                    m_boundingBoxCorners[1] = bounds.center + new Vector3(bounds.extents.x, bounds.extents.y, -bounds.extents.z);
                                    m_boundingBoxCorners[2] = bounds.center + new Vector3(bounds.extents.x, -bounds.extents.y, bounds.extents.z);
                                    m_boundingBoxCorners[3] = bounds.center + new Vector3(bounds.extents.x, -bounds.extents.y, -bounds.extents.z);
                                    m_boundingBoxCorners[4] = bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, bounds.extents.z);
                                    m_boundingBoxCorners[5] = bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, -bounds.extents.z);
                                    m_boundingBoxCorners[6] = bounds.center + new Vector3(-bounds.extents.x, -bounds.extents.y, bounds.extents.z);
                                    m_boundingBoxCorners[7] = bounds.center + new Vector3(-bounds.extents.x, -bounds.extents.y, -bounds.extents.z);
                                    if (Targets[i] != null)
                                    {
                                        GetMinPoint(ref minDistance, ref minPoint, ref minPointFound, snapTarget);
                                    }
                                }

                                if (minPointFound)
                                {
                                    m_handleOffset = minPoint - transform.position;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (IsDragging)
                        {
                            SelectedAxis = RuntimeHandleAxis.Snap;
                            if (m_prevMousePosition != mousePosition)
                            {
                                m_prevMousePosition = mousePosition;

                                Ray ray = Window.Pointer;
                                RaycastHit hitInfo;

                                LayerMask layerMask = (1 << Physics.IgnoreRaycastLayer);
                                layerMask = ~layerMask;
                                layerMask &= Editor.CameraLayerSettings.RaycastMask;

                                for (int i = 0; i < m_snapTargets.Length; ++i)
                                {
                                    m_targetLayers[i] = m_snapTargets[i].gameObject.layer;
                                    m_snapTargets[i].gameObject.layer = Physics.IgnoreRaycastLayer;
                                }

                                GameObject closestObject = null;
                                if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, layerMask))
                                {
                                    closestObject = hitInfo.collider.gameObject;
                                }
                                else
                                {
                                    float minDistance = float.MaxValue;
                                    for (int i = 0; i < m_allExposedToEditor.Length; ++i)
                                    {
                                        ExposeToEditor exposedToEditor = m_allExposedToEditor[i];
                                        Bounds bounds = exposedToEditor.Bounds;

                                        m_boundingBoxCorners[0] = bounds.center + new Vector3(bounds.extents.x, bounds.extents.y, bounds.extents.z);
                                        m_boundingBoxCorners[1] = bounds.center + new Vector3(bounds.extents.x, bounds.extents.y, -bounds.extents.z);
                                        m_boundingBoxCorners[2] = bounds.center + new Vector3(bounds.extents.x, -bounds.extents.y, bounds.extents.z);
                                        m_boundingBoxCorners[3] = bounds.center + new Vector3(bounds.extents.x, -bounds.extents.y, -bounds.extents.z);
                                        m_boundingBoxCorners[4] = bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, bounds.extents.z);
                                        m_boundingBoxCorners[5] = bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, -bounds.extents.z);
                                        m_boundingBoxCorners[6] = bounds.center + new Vector3(-bounds.extents.x, -bounds.extents.y, bounds.extents.z);
                                        m_boundingBoxCorners[7] = bounds.center + new Vector3(-bounds.extents.x, -bounds.extents.y, -bounds.extents.z);

                                        for (int j = 0; j < m_boundingBoxCorners.Length; ++j)
                                        {
                                            Vector2 screenPoint;
                                            if(Window.Pointer.WorldToScreenPoint(Position, exposedToEditor.BoundsObject.transform.TransformPoint(m_boundingBoxCorners[j]), out screenPoint))
                                            {
                                                float distance = (screenPoint - mousePosition).magnitude;
                                                if (distance < minDistance)
                                                {
                                                    closestObject = exposedToEditor.gameObject;
                                                    minDistance = distance;
                                                }
                                            }   
                                        }
                                    }
                                }

                                if (closestObject != null)
                                {
                                    float minDistance = float.MaxValue;
                                    Vector3 minPoint = Vector3.zero;
                                    bool minPointFound = false;
                                    Transform meshTransform;
                                    Mesh mesh = GetMesh(closestObject, out meshTransform);
                                    GetMinPoint(meshTransform, ref minDistance, ref minPoint, ref minPointFound, mesh);

                                    if (minPointFound)
                                    {
                                        Position = minPoint;
                                    }

                                }

                                for (int i = 0; i < m_snapTargets.Length; ++i)
                                {
                                    m_snapTargets[i].gameObject.layer = m_targetLayers[i];
                                }
                            }
                        }
                        else
                        {
                            SelectedAxis = RuntimeHandleAxis.None;
                            if (m_prevMousePosition != mousePosition)
                            {
                                m_prevMousePosition = mousePosition;

                                float minDistance = float.MaxValue;
                                Vector3 minPoint = Vector3.zero;
                                bool minPointFound = false;
                                for (int i = 0; i < RealTargets.Length; ++i)
                                {
                                    Transform snapTarget = RealTargets[i];
                                    Transform meshTranform;
                                    Mesh mesh = GetMesh(snapTarget.gameObject, out meshTranform);
                                    GetMinPoint(meshTranform, ref minDistance, ref minPoint, ref minPointFound, mesh);
                                }
                                if (minPointFound)
                                {
                                    m_handleOffset = minPoint - transform.position;
                                }
                            }
                        }
                    }
                }
            }     
        }

        private void GetMinPoint(Transform meshTransform, ref float minDistance, ref Vector3 minPoint, ref bool minPointFound, Mesh mesh)
        {
            if (mesh != null && mesh.isReadable)
            {
                IRTE editor = Editor;
                Vector3[] vertices = mesh.vertices;
                for (int i = 0; i < vertices.Length; ++i)
                {
                    Vector3 vert = vertices[i];
                    vert = meshTransform.TransformPoint(vert);

                    Vector2 screenPoint;
                    if(Window.Pointer.WorldToScreenPoint(Position, vert, out screenPoint))
                    {
                        Vector2 mousePoint;
                        if (Window.Pointer.XY(Position, out mousePoint))
                        {
                            float distance = (screenPoint - mousePoint).magnitude;
                            if (distance < minDistance)
                            {
                                minPointFound = true;
                                minDistance = distance;
                                minPoint = vert;
                            }
                        }
                    }
                }
            }
        }

        private static Mesh GetMesh(GameObject go, out Transform meshTransform)
        {
            Mesh mesh = null;
            meshTransform = null;
            MeshFilter filter = go.GetComponentInChildren<MeshFilter>();
            if (filter != null)
            {
                mesh = filter.sharedMesh;
                meshTransform = filter.transform;
            }
            else
            {
                SkinnedMeshRenderer skinnedMeshRender = go.GetComponentInChildren<SkinnedMeshRenderer>();
                if (skinnedMeshRender != null)
                {
                    mesh = skinnedMeshRender.sharedMesh;
                    meshTransform = skinnedMeshRender.transform;
                }
                else
                {
                    MeshCollider collider = go.GetComponentInChildren<MeshCollider>();
                    if (collider != null)
                    {
                        mesh = collider.sharedMesh;
                        meshTransform = collider.transform;
                    }
                }
            }

            return mesh;
        }

        protected override void OnDrop()
        {
            base.OnDrop();

            if (SnapToGrid)
            {
                SnapActiveTargetsToGrid();
            }

            if (SnapToGround)
            {
                SnapActiveTargetsToGround(ActiveTargets, Window.Camera, true);
                transform.position = Targets[0].position;
            }
        }

        private static void SnapActiveTargetsToGround(Transform[] targets, Camera camera,  bool rotate)
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
            for (int i = 0; i < targets.Length; ++i)
            {
                Transform activeTarget = targets[i];
                Ray ray = new Ray(activeTarget.position, Vector3.up);
                bool hitFrustum = false;
                Vector3 topPoint = activeTarget.position;
                for (int j = 0; j < planes.Length; ++j)
                {
                    float distance;
                    if (planes[j].Raycast(ray, out distance))
                    {
                        hitFrustum = true;
                        topPoint = ray.GetPoint(distance);
                    }
                }

                if (!hitFrustum)
                {
                    continue;
                }

                ray = new Ray(topPoint, Vector3.down);

                RaycastHit[] hits = Physics.RaycastAll(ray).Where(hit => !hit.transform.IsChildOf(activeTarget)).ToArray();
                if (hits.Length == 0)
                {
                    continue;
                }

                float minDistance = float.PositiveInfinity;
                RaycastHit bestHit = hits[0];
                for (int j = 0; j < hits.Length; ++j)
                {
                    float mag = (activeTarget.position - hits[j].point).magnitude;
                    if (mag < minDistance)
                    {
                        minDistance = mag;
                        bestHit = hits[j];
                    }
                }

                activeTarget.position += (bestHit.point - activeTarget.position);
                if (rotate)
                {
                    activeTarget.rotation = Quaternion.FromToRotation(activeTarget.up, bestHit.normal) * activeTarget.rotation;
                }
            }
        }

        private void OnSnappingChanged()
        {
            if (Editor.Tools.IsSnapping)
            {
                BeginSnap();
            }
            else
            {
                m_handleOffset = Vector3.zero;
                if(Model != null && Model is PositionHandleModel)
                {
                    ((PositionHandleModel)Model).IsVertexSnapping = false;
                }
            }
        }

        private void BeginSnap()
        {
            if(Window.Camera == null)
            {
                return;
            }

            if (Model != null && Model is PositionHandleModel)
            {
                ((PositionHandleModel)Model).IsVertexSnapping = true;
            }

            HashSet<Transform> snapTargetsHS = new HashSet<Transform>();
            List<Transform> snapTargets = new List<Transform>();
            List<Bounds> snapTargetBounds = new List<Bounds>();
            
            if (Target != null)
            {
                for (int i = 0; i < RealTargets.Length; ++i)
                {
                    Transform target = RealTargets[i];
                    if (target != null)
                    {
                        ExposeToEditor exposeToEditor = target.GetComponent<ExposeToEditor>();
                        if (exposeToEditor != null)
                        {
                            snapTargetBounds.Add(exposeToEditor.Bounds);
                            snapTargets.Add(exposeToEditor.BoundsObject.transform);
                            snapTargetsHS.Add(exposeToEditor.BoundsObject.transform);
                        }
                        else
                        {
                            snapTargets.Add(target);
                            snapTargetsHS.Add(target);

                            MeshFilter filter = target.GetComponent<MeshFilter>();
                            if(filter != null && filter.sharedMesh != null)
                            {
                                snapTargetBounds.Add(filter.sharedMesh.bounds);
                            }
                            else
                            {
                                SkinnedMeshRenderer smr = target.GetComponent<SkinnedMeshRenderer>();
                                if(smr != null && smr.sharedMesh != null)
                                {
                                    snapTargetBounds.Add(smr.sharedMesh.bounds);
                                }
                                else
                                {
                                    Bounds b = new Bounds(Vector3.zero, Vector3.zero);
                                    snapTargetBounds.Add(b);
                                }
                            }
                        }
                    }
                }
            }

            m_snapTargets = snapTargets.ToArray();
            m_targetLayers = new int[m_snapTargets.Length];
            m_snapTargetsBounds = snapTargetBounds.ToArray();

            Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(Window.Camera);
            ExposeToEditor[] exposeToEditorObjects = FindObjectsOfType<ExposeToEditor>();
            List<ExposeToEditor> insideOfFrustum = new List<ExposeToEditor>();
            for (int i = 0; i < exposeToEditorObjects.Length; ++i)
            {
                ExposeToEditor exposeToEditor = exposeToEditorObjects[i];
                if (exposeToEditor.CanSnap)
                {
                    if (GeometryUtility.TestPlanesAABB(frustumPlanes, new Bounds(exposeToEditor.transform.TransformPoint(exposeToEditor.Bounds.center), Vector3.zero)))
                    {
                        if (!snapTargetsHS.Contains(exposeToEditor.transform))
                        {
                            insideOfFrustum.Add(exposeToEditor);
                        }
                    }
                }

            }
            m_allExposedToEditor = insideOfFrustum.ToArray();
        }

        private void GetMinPoint(ref float minDistance, ref Vector3 minPoint, ref bool minPointFound, Transform tr)
        {
            IRTE editor = Editor;
            for (int j = 0; j < m_boundingBoxCorners.Length; ++j)
            {
                Vector3 worldPoint = tr.TransformPoint(m_boundingBoxCorners[j]);
                Vector2 screenPoint;

                if(Window.Pointer.WorldToScreenPoint(Position, worldPoint, out screenPoint))
                {
                    Vector2 mousePoint;
                    if (Window.Pointer.XY(Position, out mousePoint))
                    {
                        float distance = (screenPoint - mousePoint).magnitude;
                        if (distance < minDistance)
                        {
                            minPointFound = true;
                            minDistance = distance;
                            minPoint = worldPoint;
                        }
                    }
                }
            }
        }

        private bool HitSnapHandle()
        {
            Vector2 sp;

            if(Window.Pointer.WorldToScreenPoint(Position, Position, out sp))
            {
                Vector2 mp;
                if (Window.Pointer.XY(Position, out mp))
                {
                    const float pixelSize = 10;

                    return sp.x - pixelSize <= mp.x && mp.x <= sp.x + pixelSize &&
                           sp.y - pixelSize <= mp.y && mp.y <= sp.y + pixelSize;
                }
            }
            
            return false;
        }

        protected override bool OnBeginDrag()
        {
            if(!base.OnBeginDrag())
            {
                return false;
            }
           
            m_currentPosition = Position;
            m_cursorPosition = Position;

            if ((IsInVertexSnappingMode || Editor.Tools.IsSnapping) && SelectedAxis != RuntimeHandleAxis.Snap)
            {
                return HitSnapHandle();
            }

            if (SelectedAxis == RuntimeHandleAxis.XZ)
            {
                DragPlane = GetDragPlane(m_matrix, Vector3.up);
                return GetPointOnDragPlane(Window.Pointer, out m_prevPoint);
            }

            if (SelectedAxis == RuntimeHandleAxis.YZ)
            {
                DragPlane = GetDragPlane(m_matrix, Vector3.right);
                return GetPointOnDragPlane(Window.Pointer, out m_prevPoint);
            }

            if (SelectedAxis == RuntimeHandleAxis.XY)
            {
                DragPlane = GetDragPlane(m_matrix, Vector3.forward);
                return GetPointOnDragPlane(Window.Pointer, out m_prevPoint);
            }

            if (SelectedAxis != RuntimeHandleAxis.None)
            {
                Vector3 axis = Vector3.zero;
                switch (SelectedAxis)
                {
                    case RuntimeHandleAxis.X:
                        axis = Vector3.right;
                        break;
                    case RuntimeHandleAxis.Y:
                        axis = Vector3.up;
                        break;
                    case RuntimeHandleAxis.Z:
                        axis = Vector3.forward;
                        break;
                }


                DragPlane = GetDragPlane(axis);
                bool result = GetPointOnDragPlane(Window.Pointer, out m_prevPoint);
                if(!result)
                {
                    SelectedAxis = RuntimeHandleAxis.None;
                }
                return result;
            }

            return false;
        }

        protected override void OnDrag()
        {
            if (IsInVertexSnappingMode || Editor.Tools.IsSnapping)
            {
                return;
            }

            Vector3 point;
            if (GetPointOnDragPlane(Window.Pointer, out point))
            {
                Vector3 offset = m_inverse.MultiplyVector(point - m_prevPoint);
                float mag = offset.magnitude;
                if (SelectedAxis == RuntimeHandleAxis.X)
                {
                    offset.y = offset.z = 0.0f;
                }
                else if (SelectedAxis == RuntimeHandleAxis.Y)
                {
                    offset.x = offset.z = 0.0f;
                }
                else if (SelectedAxis == RuntimeHandleAxis.Z)
                {
                    offset.x = offset.y = 0.0f;
                }

                if(LockObject != null)
                {
                    if (LockObject.PositionX)
                    {
                        offset.x = 0.0f;
                    }
                    if (LockObject.PositionY)
                    {
                        offset.y = 0.0f;
                    }
                    if (LockObject.PositionZ)
                    {
                        offset.z = 0.0f;
                    }
                }

                Vector3 prevPosition = Position;
                Vector3 prevCurrentPosition = m_currentPosition;
                if (EffectiveGridUnitSize == 0.0)
                {
                    offset = m_matrix.MultiplyVector(offset).normalized * mag;
                    transform.position += offset;
                    m_currentPosition = Position;
                    m_cursorPosition = Position;
                }
                else
                {
                    offset = m_matrix.MultiplyVector(offset).normalized * mag;
                    m_cursorPosition += offset;
                    Vector3 toCurrentPosition = m_cursorPosition - m_currentPosition;
                    Vector3 gridOffset = Vector3.zero;
                    if (Mathf.Abs(toCurrentPosition.x * 1.5f) >= EffectiveGridUnitSize)
                    {
                        gridOffset.x = EffectiveGridUnitSize * Mathf.Sign(toCurrentPosition.x); 
                    }

                    if (Mathf.Abs(toCurrentPosition.y * 1.5f) >= EffectiveGridUnitSize)
                    {
                        gridOffset.y = EffectiveGridUnitSize * Mathf.Sign(toCurrentPosition.y);
                    }

                    if (Mathf.Abs(toCurrentPosition.z * 1.5f) >= EffectiveGridUnitSize)
                    {
                        gridOffset.z = EffectiveGridUnitSize * Mathf.Sign(toCurrentPosition.z);
                    }
                  
                    m_currentPosition += gridOffset;
                    Position = m_currentPosition;

                    if (SnapToGrid)
                    {
                        float gridSize = SizeOfGrid;
                        if (!Mathf.Approximately(gridSize, 0))
                        {
                            gridOffset = GetGridOffset(gridSize, Position);
                            m_currentPosition += gridOffset;
                            Position = m_currentPosition;
                        }
                    }
                }

                float allowedRadius = Window.Camera.farClipPlane * 0.95f;
                Vector3 toHandle = Position - Window.Camera.transform.position;
                if(toHandle.magnitude > allowedRadius)
                {
                    Position = prevPosition;
                    m_currentPosition = prevCurrentPosition;
                }
                else
                {
                    m_prevPoint = point;
                }
            }
        }

        private void SnapActiveTargetsToGrid()
        {
            float gridSize = SizeOfGrid;
            if (Mathf.Approximately(gridSize, 0))
            {
                return;
            }

            for (int i = 0; i < ActiveTargets.Length; ++i)
            {
                Transform activeTransform = ActiveTargets[i];
                Vector3 position = activeTransform.position;

                Vector3 offset = GetGridOffset(gridSize, position);

                activeTransform.position += offset;
            }
        }

        private RTHDrawingSettings m_settings = new RTHDrawingSettings();
        protected override void RefreshCommandBuffer(IRTECamera camera)
        {
            m_settings.Position = Position;
            m_settings.Rotation = Rotation;
            m_settings.SelectedAxis = SelectedAxis;
            m_settings.LockObject = LockObject;

            Appearance.DoPositionHandle(camera.CommandBuffer, camera.Camera, m_settings, IsInVertexSnappingMode || Editor.Tools.IsSnapping);
        }

        public override RuntimeHandleAxis HitTest(out float distance)
        {
            m_matrix = Matrix4x4.TRS(Position, Rotation, Appearance.InvertZAxis ? new Vector3(1, 1, -1) : Vector3.one);
            m_inverse = m_matrix.inverse;

            if (Model != null)
            {
                return Model.HitTest(Window.Pointer, out distance);
            }

            return Appearance.HitTestPositionHandle(Window.Camera, Window.Pointer, m_settings, out distance);
        }

    }
}
