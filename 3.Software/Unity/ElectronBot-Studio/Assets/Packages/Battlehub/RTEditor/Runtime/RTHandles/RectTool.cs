using Battlehub.RTCommon;
using Battlehub.Utils;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace Battlehub.RTHandles
{
    public class RectTool : BaseHandle
    {
        public float GridSize = 1.0f;

        [SerializeField]
        private TextMeshPro m_txtSize1 = null;
        [SerializeField]
        private TextMeshPro m_txtSize2 = null;
        [SerializeField]
        private bool m_metric = true;
        public bool Metric
        {
            get { return m_metric; }
            set
            {
                if (m_metric != value)
                {
                    m_metric = value;
                    UpdateText();
                }
            }
        }


        private Quaternion m_rotation;
        private Vector3 m_position;
        private Vector3 m_localScale;
        private float m_currentDot;
        private RuntimeHandleAxis m_currentAxis;
        private int m_selectedPointIndex = -1;
        private int m_selectedEdgeIndex = -1;
        private Vector3 m_beginDragPoint;
        private Vector3 m_beginDragOffset;

        private Vector3[] m_referencePoints;
        private Bounds m_referenceBounds;
        private Vector3[] m_referenceScale;
        private Vector3[] m_referencePositions;
        private Vector2[] m_referenceRectSizes;
        
        private MeshFilter m_lines;
        private MeshRenderer m_linesRenderer;
        private MeshFilter m_points;
        private MeshRenderer m_pointsRenderer;

        public override RuntimeTool Tool
        {
            get { return RuntimeTool.Rect; }
        }

        protected override float CurrentGridUnitSize
        {
            get { return SizeOfGrid; }
        }

        public override float SizeOfGrid
        {
            get { return GridSize; }
            set { GridSize = value; }
        }

        private static readonly List<RectTool> m_connectedTools = new List<RectTool>();
        private Bounds m_bounds;
        private Bounds Bounds
        {
            get { return m_bounds; }
            set
            {
                m_bounds = value;
                UpdatePointsMesh(m_points.sharedMesh, m_currentAxis, m_bounds);
                UpdateLinesMesh(m_lines.sharedMesh, m_currentAxis, m_bounds);
                UpdateText();
            }
        }


        protected override Transform[] Targets_Internal
        {
            get
            {
                return base.Targets_Internal;
            }
            set
            {
                base.Targets_Internal = value;
                RecalculateBoundsAndRebuild();
            }
        }

        protected override RuntimePivotMode PivotMode
        {
            get { return RuntimePivotMode.Center; }
        }

        protected override Vector3 GetCommonCenterPosition()
        {
            return m_bounds.center;
        }

        public void RecalculateBoundsAndRebuild()
        {
            m_rotation = Quaternion.identity;
            m_position = Vector3.zero;
            m_localScale = Vector3.one;
            m_bounds = new Bounds();

            if (m_txtSize1 != null)
            {
                m_txtSize1.text = string.Empty;
            }

            if (m_txtSize2 != null)
            {
                m_txtSize2.text = string.Empty;
            }


            if (RealTargets == null || RealTargets.Length == 0)
            {
                return;
            }

            if (ActiveRealTargets.Length == 1 && ActiveRealTargets[0].GetComponent<ExposeToEditor>())
            {
                ExposeToEditor exposeToEditor = ActiveRealTargets[0].GetComponent<ExposeToEditor>();
                
                m_bounds = exposeToEditor.Bounds;
                m_position = exposeToEditor.transform.position;
                m_rotation = exposeToEditor.transform.rotation;
                m_localScale = exposeToEditor.transform.lossyScale;

                if (m_bounds.extents == Vector3.zero)
                {
                    if (m_lines != null)
                    {
                        m_lines.sharedMesh.Clear();
                    }

                    if (m_points != null)
                    {
                        m_points.sharedMesh.Clear();
                    }
                    return;
                }
            }
            else
            {
                Bounds[] allBounds = ActiveRealTargets.Where(t => t != null)
                    .SelectMany(t => t.GetComponentsInChildren<Renderer>())
                    .Select(r => r.bounds)
                    .Union(ActiveRealTargets
                        .OfType<RectTransform>()
                        .Select(rt => TransformExtensions.TransformBounds(rt.localToWorldMatrix, rt.CalculateRelativeRectTransformBounds())))
                    .ToArray();

                if (allBounds.Length == 0)
                {
                    if (m_lines != null)
                    {
                        m_lines.sharedMesh.Clear();
                    }

                    if (m_points != null)
                    {
                        m_points.sharedMesh.Clear();
                    }

                    return;
                }
                else
                {
                    m_bounds = allBounds[0];
                    for (int i = 1; i < allBounds.Length; ++i)
                    {
                        Bounds bounds = allBounds[i];
                        m_bounds.Encapsulate(bounds);
                    }
                }
            }

            if (m_lines == null && m_points == null)
            {
                return;
            }

            m_lines.transform.position = m_position;
            m_lines.transform.rotation = m_rotation;
            m_lines.transform.localScale = m_localScale;
            m_points.transform.position = m_position;
            m_points.transform.rotation = m_rotation;
            m_points.transform.localScale = m_localScale;

            m_currentAxis = GetAxis(out m_currentDot);
            BuildPointsMesh(m_points.sharedMesh, m_currentAxis, m_bounds);
            BuildLineMesh(m_lines.sharedMesh, m_currentAxis, m_bounds);

            if (m_txtSize1 != null)
            {
                m_txtSize1.gameObject.layer = Editor.CameraLayerSettings.RuntimeGraphicsLayer + Window.Index;
            }
            if (m_txtSize2 != null)
            {
                m_txtSize2.gameObject.layer = Editor.CameraLayerSettings.RuntimeGraphicsLayer + Window.Index;
            }

            UpdateText();
            UpdateFontSize();
        }

        protected override void Awake()
        {
            base.Awake();
        
            GameObject lines = new GameObject("Lines");
            lines.transform.SetParent(transform);
            lines.layer = Editor.CameraLayerSettings.RuntimeGraphicsLayer + Window.Index;

            m_lines = lines.AddComponent<MeshFilter>();
            m_lines.sharedMesh = new Mesh();

            m_linesRenderer = lines.AddComponent<MeshRenderer>();

            Material lineMaterial = new Material(Shader.Find("Battlehub/RTCommon/LineBillboard"));
            lineMaterial.SetFloat("_Scale", 1.0f);
            lineMaterial.SetColor("_Color", Color.white);
            lineMaterial.SetInt("_HandleZTest", (int)CompareFunction.Always);
            m_linesRenderer.sharedMaterial = lineMaterial;

            GameObject points = new GameObject("Points");
            points.transform.SetParent(transform);
            points.layer = Editor.CameraLayerSettings.RuntimeGraphicsLayer + Window.Index;

            m_points = points.AddComponent<MeshFilter>();
            m_points.sharedMesh = new Mesh();

            m_pointsRenderer = points.AddComponent<MeshRenderer>();

            Material pointMaterial = new Material(Shader.Find("Hidden/RTHandles/PointBillboard"));
            pointMaterial.SetFloat("_Scale", 4.5f);
            pointMaterial.SetColor("_Color", Color.white);
            pointMaterial.SetInt("_HandleZTest", (int)CompareFunction.Always);
            m_pointsRenderer.sharedMaterial = pointMaterial;

            float scale = Appearance.HandleScale;

            //GK set text and control sizes to match display
            if (m_txtSize1 != null && m_txtSize2 != null)
            {
                m_txtSize1.transform.localScale = m_txtSize2.transform.localScale = new Vector3(scale, scale, scale);
            }

            lineMaterial.SetFloat("_Scale", scale);
            pointMaterial.SetFloat("_Scale", 4.5f * scale);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        
            RecalculateBoundsAndRebuild();
            m_connectedTools.Add(this);

            if(RTECamera != null)
            {
                Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>(true);
                RTECamera.RenderersCache.Add(renderers, false, true);
                RTECamera.RenderersCache.Refresh();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            m_connectedTools.Remove(this);

            if (RTECamera != null)
            {
                Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>(true);
                if (RTECamera.RenderersCache != null)
                {
                    RTECamera.RenderersCache.Remove(renderers);
                    RTECamera.RenderersCache.Refresh();
                }
            }
        }

        protected override void Start()
        {
            base.Start();
            if (RTECamera != null)
            {
                Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>(true);
                RTECamera.RenderersCache.Add(renderers, false, true);
                RTECamera.RenderersCache.Refresh();
            }
        }
      
        protected override void UpdateOverride()
        {
            if (!IsDragging)
            {
                RuntimeHandleAxis axis = GetAxis(out m_currentDot);
                if (m_currentAxis != axis)
                {
                    m_currentAxis = axis;
                    RecalculateBoundsAndRebuild();
                    m_selectedPointIndex = -1;
                    m_selectedEdgeIndex = -1;
                }

                Vector3[] vertices = m_points.sharedMesh.vertices;
                if (vertices.Length == 0)
                {
                    return;
                }

                PickResult pointPickResult = PickPoint(vertices);
                pointPickResult.Distance *= 0.1f;
                PickResult edgePickResult = PickEdge(vertices);

                if (pointPickResult.Distance < edgePickResult.Distance)
                {
                    if (m_selectedEdgeIndex != -1)
                    {
                        Color[] colors = m_lines.sharedMesh.colors;
                        Color color = GetColor(m_currentAxis);
                        colors[m_selectedEdgeIndex * 2] = color;
                        colors[m_selectedEdgeIndex * 2 + 1] = color;
                        m_lines.sharedMesh.colors = colors;

                        m_selectedEdgeIndex = -1;
                    }

                    if (pointPickResult.Index != m_selectedPointIndex)
                    {
                        Color[] colors = m_points.sharedMesh.colors;
                        if (m_selectedPointIndex >= 0)
                        {
                            colors[m_selectedPointIndex] = GetColor(m_currentAxis);
                        }

                        m_selectedPointIndex = pointPickResult.Index;

                        if (m_selectedPointIndex >= 0)
                        {
                            colors[m_selectedPointIndex] = Appearance.Colors.SelectionColor;
                        }

                        m_points.sharedMesh.colors = colors;
                    }
                }
                else if (edgePickResult.Distance < pointPickResult.Distance)
                {
                    if (m_selectedPointIndex != -1)
                    {
                        Color[] colors = m_points.sharedMesh.colors;
                        colors[m_selectedPointIndex] = GetColor(m_currentAxis);
                        m_points.sharedMesh.colors = colors;
                        m_selectedPointIndex = -1;
                    }

                    if (edgePickResult.Index != m_selectedEdgeIndex)
                    {
                        Color[] colors = m_lines.sharedMesh.colors;
                        if (m_selectedEdgeIndex >= 0)
                        {
                            Color color = GetColor(m_currentAxis);
                            colors[m_selectedEdgeIndex * 2] = color;
                            colors[m_selectedEdgeIndex * 2 + 1] = color;
                        }

                        m_selectedEdgeIndex = edgePickResult.Index;

                        if (m_selectedEdgeIndex >= 0)
                        {
                            Color color = Appearance.Colors.SelectionColor;
                            colors[m_selectedEdgeIndex * 2] = color;
                            colors[m_selectedEdgeIndex * 2 + 1] = color;
                        }
                    }
                }
            }
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
            UpdateFontSize();
        }

        private void UpdateConnectedTools()
        {
            for (int i = 0; i < m_connectedTools.Count; ++i)
            {
                RectTool tool = m_connectedTools[i];
                if (tool != this)
                {
                    tool.Bounds = m_bounds;
                    tool.m_lines.transform.position = m_lines.transform.position;
                    tool.m_points.transform.position = m_points.transform.position;
                }
            }
        }

        protected override bool OnBeginDrag()
        {
            if (!base.OnBeginDrag())
            {
                return false;
            }

            if (m_bounds.extents == Vector3.zero)
            {
                return false;
            }

            if (m_currentAxis == RuntimeHandleAxis.XY)
            {
                DragPlane = new Plane(m_lines.transform.forward, m_lines.transform.TransformPoint(m_bounds.center));
            }
            else if (m_currentAxis == RuntimeHandleAxis.XZ)
            {
                DragPlane = new Plane(m_lines.transform.up, m_lines.transform.TransformPoint(m_bounds.center));
            }
            else
            {
                DragPlane = new Plane(m_lines.transform.right, m_lines.transform.TransformPoint(m_bounds.center));
            }

            m_referenceBounds = m_bounds;
            m_referenceBounds.extents = NonZero(m_referenceBounds.extents);
            if (!GetPointOnDragPlane(Window.Pointer, out m_beginDragPoint))
            {
                return false;
            }

            if (m_selectedEdgeIndex >= 0)
            {
                m_referenceRectSizes = ActiveTargets.Select(t => (t as RectTransform) ? ((RectTransform)t).rect.size : Vector2.zero).ToArray();
                m_referencePositions = ActiveTargets.Select(t => t.position).ToArray();
                m_referenceScale = ActiveTargets.Select(t => t.localScale).ToArray();

                m_referencePoints = m_lines.sharedMesh.vertices;
                m_beginDragPoint = m_lines.transform.InverseTransformPoint(m_beginDragPoint);

                Vector3 p0;
                Vector3 p1;
                m_beginDragOffset = NonZero(GetOffset(m_selectedEdgeIndex, m_beginDragPoint, out p0, out p1));
                SetSelectionColorColors();
                return true;
            }
            else if (m_selectedPointIndex >= 0)
            {
                m_referenceRectSizes = ActiveTargets.Select(t => (t as RectTransform) ? ((RectTransform)t).rect.size : Vector2.zero).ToArray();
                m_referencePositions = ActiveTargets.Select(t => t.position).ToArray();
                m_referenceScale = ActiveTargets.Select(t => t.localScale).ToArray();
                m_referencePoints = m_points.sharedMesh.vertices;
                if (m_selectedPointIndex < m_referencePoints.Length - 1)
                {
                    Vector3 refPoint;
                    m_beginDragPoint = m_points.transform.InverseTransformPoint(m_beginDragPoint);
                    m_beginDragOffset = NonZero(GetOffset(m_selectedPointIndex, m_beginDragPoint, out refPoint));
                }
                else
                {
                    RecalculateBoundsAndRebuild();
                    UpdateConnectedTools();
                }
                SetSelectionColorColors();
                return true;
            }

            return false;
        }

        protected override void OnDrag()
        {
            base.OnDrag();

            Vector3 pointOnPlane;
            if (GetPointOnDragPlane(Window.Pointer, out pointOnPlane))
            {
                if (m_selectedPointIndex == m_referencePoints.Length - 1)
                {
                    Vector3 offset = pointOnPlane - m_beginDragPoint;

                    m_points.transform.position = m_position + offset;
                    m_lines.transform.position = m_position + offset;

                    if (EffectiveGridUnitSize > 0.001)
                    {
                        Vector3 gridOffset = GetGridOffset(EffectiveGridUnitSize, m_points.transform.position);
                        m_points.transform.position += gridOffset;
                        m_lines.transform.position += gridOffset;
                        offset += gridOffset;
                    }

                    for (int i = 0; i < ActiveTargets.Length; ++i)
                    {
                        ActiveTargets[i].position = m_referencePositions[i] + offset;
                    }
                    UpdateText();
                }
                else
                {
                    Vector3 sign = Vector3.one;
                    if (m_selectedPointIndex >= 0)
                    {
                        Vector3 refPoint;
                        pointOnPlane = m_points.transform.InverseTransformPoint(pointOnPlane);
                        Vector3 offset = GetOffset(m_selectedPointIndex, pointOnPlane, out refPoint);
                        if (EffectiveGridUnitSize > 0.001)
                        {
                            float gridSize = EffectiveGridUnitSize;
                            gridSize /= 2;

                            if (!Mathf.Approximately(m_localScale.x, 0))
                            {
                                float gridSizeX = gridSize / m_localScale.x;
                                offset.x = Mathf.RoundToInt(offset.x / gridSizeX) * gridSizeX;
                            }

                            if (!Mathf.Approximately(m_localScale.y, 0))
                            {
                                float gridSizeY = gridSize / m_localScale.y;
                                offset.y = Mathf.RoundToInt(offset.y / gridSizeY) * gridSizeY;
                            }

                            if (!Mathf.Approximately(m_localScale.z, 0))
                            {
                                float gridSizeZ = gridSize / m_localScale.z;

                                offset.z = Mathf.RoundToInt(offset.z / gridSizeZ) * gridSizeZ;
                            }
                        }

                        m_bounds.center = refPoint + offset;
                        Vector3 extents = m_bounds.extents;
                        if (m_currentAxis == RuntimeHandleAxis.XY)
                        {
                            offset.z = extents.z;
                            sign.x = Mathf.Sign(offset.x / m_beginDragOffset.x);
                            sign.y = Mathf.Sign(offset.y / m_beginDragOffset.y);

                        }
                        else if (m_currentAxis == RuntimeHandleAxis.XZ)
                        {
                            offset.y = extents.y;
                            sign.x = Mathf.Sign(offset.x / m_beginDragOffset.x);
                            sign.z = Mathf.Sign(offset.z / m_beginDragOffset.z);
                        }
                        else
                        {
                            offset.x = extents.x;
                            sign.y = Mathf.Sign(offset.y / m_beginDragOffset.y);
                            sign.z = Mathf.Sign(offset.z / m_beginDragOffset.z);
                        }
                        m_bounds.extents = new Vector3(Mathf.Abs(offset.x), Mathf.Abs(offset.y), Mathf.Abs(offset.z));

                        UpdatePointsMesh(m_points.sharedMesh, m_currentAxis, m_bounds);
                        UpdateLinesMesh(m_lines.sharedMesh, m_currentAxis, m_bounds);
                        UpdateText();
                    }
                    else if (m_selectedEdgeIndex >= 0)
                    {
                        pointOnPlane = m_lines.transform.InverseTransformPoint(pointOnPlane);

                        Vector3 p0;
                        Vector3 p1;
                        Vector3 offset = GetOffset(m_selectedEdgeIndex, pointOnPlane, out p0, out p1);
                        if (EffectiveGridUnitSize > 0.001)
                        {
                            float gridSize = EffectiveGridUnitSize;

                            if (!Mathf.Approximately(m_localScale.x, 0))
                            {
                                float gridSizeX = gridSize / m_localScale.x;
                                offset.x = Mathf.RoundToInt(offset.x / gridSizeX) * gridSizeX;
                            }

                            if (!Mathf.Approximately(m_localScale.y, 0))
                            {
                                float gridSizeY = gridSize / m_localScale.y;
                                offset.y = Mathf.RoundToInt(offset.y / gridSizeY) * gridSizeY;
                            }

                            if (!Mathf.Approximately(m_localScale.z, 0))
                            {
                                float gridSizeZ = gridSize / m_localScale.z;

                                offset.z = Mathf.RoundToInt(offset.z / gridSizeZ) * gridSizeZ;
                            }
                        }

                        Vector3 p2 = p1 + offset;
                        Vector3 ext = (p2 - p0) / 2;

                        m_bounds.center = ((p0 + p1) + offset) / 2;

                        Vector3 extents = m_bounds.extents;
                        if (m_currentAxis == RuntimeHandleAxis.XY)
                        {
                            ext.z = extents.z;
                            if (Mathf.Abs(offset.y) > Mathf.Abs(offset.x))
                            {
                                sign.y = Mathf.Sign(offset.y / m_beginDragOffset.y);
                            }
                            else
                            {
                                sign.x = Mathf.Sign(offset.x / m_beginDragOffset.x);
                            }
                        }
                        else if (m_currentAxis == RuntimeHandleAxis.XZ)
                        {
                            ext.y = extents.y;

                            if (Mathf.Abs(offset.z) > Mathf.Abs(offset.x))
                            {
                                sign.z = Mathf.Sign(offset.z / m_beginDragOffset.z);
                            }
                            else
                            {
                                sign.x = Mathf.Sign(offset.x / m_beginDragOffset.x);
                            }
                        }
                        else
                        {
                            ext.x = extents.x;
                            if (Mathf.Abs(offset.z) > Mathf.Abs(offset.y))
                            {
                                sign.z = Mathf.Sign(offset.z / m_beginDragOffset.z);
                            }
                            else
                            {
                                sign.y = Mathf.Sign(offset.y / m_beginDragOffset.y);
                            }
                        }
                        m_bounds.extents = new Vector3(Mathf.Abs(ext.x), Mathf.Abs(ext.y), Mathf.Abs(ext.z));

                        UpdatePointsMesh(m_points.sharedMesh, m_currentAxis, m_bounds);
                        UpdateLinesMesh(m_lines.sharedMesh, m_currentAxis, m_bounds);
                        UpdateText();
                    }

                    for (int i = 0; i < ActiveTargets.Length; ++i)
                    {
                        Transform target = ActiveTargets[i];
                        Vector3 referenceScale = m_referenceScale[i];
                        
                        if (target is RectTransform)
                        {
                            Vector3 scale = Vector3.Scale(referenceScale,
                                   new Vector3(m_bounds.extents.x / m_referenceBounds.extents.x * sign.x,
                                           m_bounds.extents.y / m_referenceBounds.extents.y * sign.y,
                                           m_bounds.extents.z / m_referenceBounds.extents.z * sign.z));

                            RectTransform rt = (RectTransform)target;
                            Vector2 size = m_referenceRectSizes[i];
                            
                            if(!Mathf.Approximately(referenceScale.x, 0))
                            {
                                size.x *= scale.x / referenceScale.x;
                            }
                            if(!Mathf.Approximately(referenceScale.y, 0))
                            {
                                size.y *= scale.y / referenceScale.y;
                            }
                            
                            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
                            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
                        }
                        else
                        {
                            target.localScale = Vector3.Scale(referenceScale,
                                   new Vector3(m_bounds.extents.x / m_referenceBounds.extents.x * sign.x,
                                           m_bounds.extents.y / m_referenceBounds.extents.y * sign.y,
                                           m_bounds.extents.z / m_referenceBounds.extents.z * sign.z));
                        }

                        Vector3 pivotOffset = Vector3.zero;
                        ExposeToEditor exposeToEditor = target.GetComponent<ExposeToEditor>();
                        if (exposeToEditor != null)
                        {
                            pivotOffset = target.TransformVector(-exposeToEditor.Bounds.center);
                        }

                        target.position = m_referencePositions[i] + (m_points.transform.TransformPoint(m_bounds.center) - m_referencePositions[i]) + pivotOffset;
                    }
                }

                UpdateConnectedTools();
            }
        }

        protected override void OnDrop()
        {
            base.OnDrop();

            Targets = RealTargets;
            
            for (int i = 0; i < m_connectedTools.Count; ++i)
            {
                RectTool tool = m_connectedTools[i];
                if (tool != this)
                {
                    tool.RecalculateBoundsAndRebuild();
                }
            }


            m_referencePoints = null;
            m_referencePositions = null;
            m_referenceScale = null;
            m_referenceRectSizes = null;
        }

        private Vector3 GetOffset(int selectedEdgeIndex, Vector3 pointOnPlane, out Vector3 p0, out Vector3 p1)
        {
            p0 = m_referencePoints[((selectedEdgeIndex + 2) % 4) * 2];
            p1 = m_referencePoints[((selectedEdgeIndex + 2) % 4) * 2 + 1];

            Vector3 nearest = NearestPointOnLine(p0, p1 - p0, pointOnPlane);
            Vector3 beginNearest = NearestPointOnLine(m_referencePoints[selectedEdgeIndex * 2],
                m_referencePoints[selectedEdgeIndex * 2 + 1] - m_referencePoints[selectedEdgeIndex * 2], m_beginDragPoint);

            Vector3 delta = beginNearest - m_beginDragPoint;
            Vector3 offset = pointOnPlane + delta - nearest;

            return offset;
        }

        private Vector3 GetOffset(int selectedPointIndex, Vector3 pointOnPlane, out Vector3 refPoint)
        {
            refPoint = m_referencePoints[(selectedPointIndex + 2) % 4];

            Vector3 delta = Vector3.zero;
            delta = m_referencePoints[m_selectedPointIndex] - m_beginDragPoint;

            Vector3 offset = (pointOnPlane + delta - refPoint) / 2;
            return offset;
        }

        private Vector3 NonZero(Vector3 v)
        {
            if (Mathf.Approximately(v.x, .0f))
            {
                v.x = 0.000000001f;
            }
            if (Mathf.Approximately(v.y, .0f))
            {
                v.y = 0.000000001f;
            }
            if (Mathf.Approximately(v.z, .0f))
            {
                v.z = 0.000000001f;
            }
            return v;
        }




        private RuntimeHandleAxis GetAxis(out float dot)
        {
            Vector3 cam = Window.Camera.transform.forward;

            float dotZ = Vector3.Dot(m_lines.transform.forward, cam);
            float dotY = Vector3.Dot(m_lines.transform.up, cam);
            float dotX = Vector3.Dot(m_lines.transform.right, cam);
            float zDotAbs = Mathf.Abs(dotZ);
            float yDotAbs = Mathf.Abs(dotY);
            float xDotAbs = Mathf.Abs(dotX);

            if (zDotAbs >= yDotAbs && zDotAbs >= xDotAbs)
            {
                dot = dotZ;
                return RuntimeHandleAxis.XY;
            }

            if (yDotAbs >= xDotAbs && yDotAbs >= zDotAbs)
            {
                dot = dotY;
                return RuntimeHandleAxis.XZ;
            }

            dot = dotX;
            return RuntimeHandleAxis.YZ;
        }

        private void BuildPointsMesh(Mesh target, RuntimeHandleAxis axis, Bounds bounds)
        {
            Color color;
            Vector3[] vertices;

            GetVerticesAndColors(axis, bounds, out color, out vertices);

            int[] indices = new[]
            {
                0, 1, 2, 3, 4
            };

            Color[] colors = new[]
            {
                color, color, color, color, color
            };

            target.Clear();
            target.subMeshCount = 1;
            target.name = "RectToolVertices";
            target.vertices = vertices;
            target.SetIndices(indices, MeshTopology.Points, 0);
            target.colors = colors;
            target.RecalculateBounds();
        }

        private void UpdatePointsMesh(Mesh target, RuntimeHandleAxis axis, Bounds bounds)
        {
            Vector3[] vertices = GetVertices(axis, bounds);
            target.vertices = vertices;
            target.RecalculateBounds();
        }

        private void BuildLineMesh(Mesh target, RuntimeHandleAxis axis, Bounds bounds)
        {
            Color color;
            Vector3[] v;

            GetVerticesAndColors(axis, bounds, out color, out v);

            Vector3[] vertices = new[]
            {
                v[0], v[1], v[1], v[2], v[2], v[3], v[3], v[0]
            };

            int[] indices = new[]
            {
                0, 1, 2, 3, 4, 5, 6, 7
            };

            Color[] colors = new[]
            {
                color, color, color, color, color, color, color, color
            };

            target.Clear();
            target.subMeshCount = 1;
            target.name = "RectToolLines";
            target.vertices = vertices;
            target.SetIndices(indices, MeshTopology.Lines, 0);
            target.colors = colors;
            target.RecalculateBounds();
        }

        private void UpdateLinesMesh(Mesh target, RuntimeHandleAxis axis, Bounds bounds)
        {
            Vector3[] v = GetVertices(axis, bounds);
            target.vertices = new[]
            {
                v[0], v[1], v[1], v[2], v[2], v[3], v[3], v[0]
            };
            target.RecalculateBounds();
        }

        private void UpdateText()
        {
            if (m_txtSize1 == null && m_txtSize2 == null)
            {
                return;
            }

            if(m_points == null)
            {
                return;
            }

            Vector3[] v = m_points.sharedMesh.vertices;
            if (m_txtSize1 != null)
            {
                float size;
                Vector3 offset = Vector3.zero;
                Quaternion textRotation;
                if (m_currentAxis == RuntimeHandleAxis.XY)
                {
                    size = m_bounds.size.x * m_localScale.x;
                    textRotation = Mathf.Sign(m_currentDot) > 0 ? Quaternion.identity : Quaternion.Euler(0, 180, 0);
                }
                else if (m_currentAxis == RuntimeHandleAxis.XZ)
                {
                    size = m_bounds.size.x * m_localScale.x;
                    textRotation = Mathf.Sign(m_currentDot) > 0 ? Quaternion.Euler(270, 0, 180) : Quaternion.Euler(90, 0, 0);
                }
                else
                {
                    textRotation = Mathf.Sign(m_currentDot) > 0 ? Quaternion.Euler(180, -90, -90) : Quaternion.Euler(0, -90, -90);
                    size = m_bounds.size.y * m_localScale.y;
                }

                m_txtSize1.transform.localRotation = m_rotation * textRotation;
                m_txtSize1.transform.position = m_points.transform.TransformPoint(v[0] + (v[1] - v[0]) / 2);
                m_txtSize1.text = m_metric ? size.ToString("F2") : UnitsConverter.MetersToFeetInches(size);
            }

            if (m_txtSize2 != null)
            {
                float size;
                Quaternion textRotation;
                Vector3 position = m_points.transform.TransformPoint(v[1] + (v[2] - v[1]) / 2);
                if (m_currentAxis == RuntimeHandleAxis.XY)
                {
                    size = m_bounds.size.y * m_localScale.y;
                    textRotation = Mathf.Sign(m_currentDot) > 0 ? Quaternion.Euler(0, 0, 90) : Quaternion.Euler(180, 0, 90);
                }
                else if (m_currentAxis == RuntimeHandleAxis.XZ)
                {
                    size = m_bounds.size.z * m_localScale.z;
                    textRotation = Mathf.Sign(m_currentDot) > 0 ? Quaternion.Euler(270, 0, 90) : Quaternion.Euler(90, 0, 90);
                }
                else
                {
                    size = m_bounds.size.z * m_localScale.z;
                    textRotation = Mathf.Sign(m_currentDot) > 0 ? Quaternion.Euler(0, -270, 0) : Quaternion.Euler(0, -90, 0);
                    position = m_points.transform.TransformPoint(v[0] + (v[3] - v[0]) / 2);
                }

                m_txtSize2.transform.localRotation = m_rotation * textRotation;
                m_txtSize2.transform.position = position;
                m_txtSize2.text = m_metric ? size.ToString("F2") : UnitsConverter.MetersToFeetInches(size);
            }
        }

        private void UpdateFontSize()
        {
            if (m_txtSize1 != null)
            {
                m_txtSize1.fontSize = GraphicsUtility.GetScreenScale(m_txtSize1.transform.position, Window.Camera) * 1.7f;
            }

            if (m_txtSize2 != null)
            {
                m_txtSize2.fontSize = GraphicsUtility.GetScreenScale(m_txtSize2.transform.position, Window.Camera) * 1.7f;
            }
        }

        private Color GetColor(RuntimeHandleAxis axis)
        {
            if (axis == RuntimeHandleAxis.XY)
            {
                return Appearance.Colors.ZColor;
            }
            else if (axis == RuntimeHandleAxis.XZ)
            {
                return Appearance.Colors.YColor;
            }

            return Appearance.Colors.XColor;
        }

        private void GetVerticesAndColors(RuntimeHandleAxis axis, Bounds bounds, out Color color, out Vector3[] vertices)
        {
            Vector3 c = bounds.center;
            Vector3 e = bounds.extents;
            if (axis == RuntimeHandleAxis.XY)
            {
                color = Appearance.Colors.ZColor;
                vertices = new[]
                {
                    c + new Vector3(e.x, e.y, 0), c + new Vector3(-e.x, e.y, 0), c + new Vector3(-e.x, -e.y, 0), c + new Vector3(e.x, -e.y, 0), c
                };
            }
            else if (axis == RuntimeHandleAxis.XZ)
            {
                color = Appearance.Colors.YColor;
                vertices = new[]
                {
                    c + new Vector3(e.x, 0, e.z), c + new Vector3(-e.x, 0, e.z), c + new Vector3(-e.x, 0, -e.z), c + new Vector3(e.x, 0, -e.z), c
                };
            }
            else
            {
                color = Appearance.Colors.XColor;
                vertices = new[]
                {
                    c + new Vector3(0, e.y, e.z), c + new Vector3(0, -e.y, e.z), c + new Vector3(0, -e.y, -e.z), c + new Vector3(0, e.y, -e.z), c
                };
            }
        }

        private Vector3[] GetVertices(RuntimeHandleAxis axis, Bounds bounds)
        {
            Vector3 c = bounds.center;
            Vector3 e = bounds.extents;
            if (axis == RuntimeHandleAxis.XY)
            {
                return new[]
                {
                    c + new Vector3(e.x, e.y, 0), c + new Vector3(-e.x, e.y, 0), c + new Vector3(-e.x, -e.y, 0), c + new Vector3(e.x, -e.y, 0), c
                };
            }
            else if (axis == RuntimeHandleAxis.XZ)
            {
                return new[]
                {
                    c + new Vector3(e.x, 0, e.z), c + new Vector3(-e.x, 0, e.z), c + new Vector3(-e.x, 0, -e.z), c + new Vector3(e.x, 0, -e.z), c
                };
            }

            return new[]
            {
                c + new Vector3(0, e.y, e.z), c + new Vector3(0, -e.y, e.z), c + new Vector3(0, -e.y, -e.z), c + new Vector3(0, e.y, -e.z), c
            };
        }

        private struct PickResult
        {
            public int Index;
            public float Distance;

            public PickResult(int index, float distance)
            {
                Index = index;
                Distance = distance;
            }
        }

        private PickResult PickPoint(Vector3[] points, float maxDistance = 20.0f)
        {
            int minIndex = -1;
            float minDistance = maxDistance * maxDistance;
            Vector3 screenPoint = Window.Pointer.ScreenPoint;
            for (int i = 0; i < points.Length; ++i)
            {
                Vector3 point = points[i];
                point = m_points.transform.transform.TransformPoint(point);
                point = Window.Camera.WorldToScreenPoint(point);
                point.z = screenPoint.z;

                float dist = (point - screenPoint).sqrMagnitude;

                if (dist < minDistance)
                {
                    minIndex = i;
                    minDistance = dist;
                }
            }

            return new PickResult(minIndex, minDistance);
        }

        private PickResult PickEdge(Vector3[] points, float maxDistance = 20.0f)
        {
            int minIndex = -1;
            float minDistance = maxDistance * maxDistance;
            Vector3 screenPoint = Window.Pointer.ScreenPoint;

            for (int i = 0; i < points.Length - 2; ++i)
            {
                Vector3 p0 = points[i];
                Vector3 p1 = points[(i + 1) % points.Length];
                TryPickEdge(p0, p1, screenPoint, i, ref minDistance, ref minIndex);
            }
            TryPickEdge(points[3], points[0], screenPoint, 3, ref minDistance, ref minIndex);

            return new PickResult(minIndex, minDistance);
        }

        private void TryPickEdge(Vector3 p0, Vector3 p1, Vector3 screenPoint, int i, ref float minDistance, ref int minIndex)
        {
            p0 = m_points.transform.transform.TransformPoint(p0);
            p1 = m_points.transform.transform.TransformPoint(p1);
            p0 = Window.Camera.WorldToScreenPoint(p0);
            p1 = Window.Camera.WorldToScreenPoint(p1);
            p0.z = p1.z = screenPoint.z;

            Vector3 nearest = NearestPointOnSegment(p0, p1, screenPoint);

            float dist = (nearest - screenPoint).sqrMagnitude;

            if (dist < minDistance)
            {
                minIndex = i;
                minDistance = dist;
            }
        }

        private Vector2 NearestPointOnSegment(Vector2 origin, Vector2 end, Vector2 point)
        {
            Vector2 heading = (end - origin);
            float magnitudeMax = heading.magnitude;
            heading.Normalize();

            Vector2 lhs = point - origin;
            float dotP = Vector2.Dot(lhs, heading);
            dotP = Mathf.Clamp(dotP, 0f, magnitudeMax);
            return origin + heading * dotP;
        }

        public Vector3 NearestPointOnLine(Vector3 linePnt, Vector3 lineDir, Vector3 pnt)
        {
            lineDir.Normalize();//this needs to be a unit vector
            var v = pnt - linePnt;
            var d = Vector3.Dot(v, lineDir);
            return linePnt + lineDir * d;
        }

        private void SetSelectionColorColors()
        {
            Color[] colors = m_points.sharedMesh.colors;
            for (int i = 0; i < colors.Length; ++i)
            {
                colors[i] = Appearance.Colors.SelectionColor;
            }
            m_points.sharedMesh.colors = colors;
            colors = m_lines.sharedMesh.colors;
            for (int i = 0; i < colors.Length; ++i)
            {
                colors[i] = Appearance.Colors.SelectionColor;
            }
            m_lines.sharedMesh.colors = colors;
        }

        public override void Refresh()
        {
            base.Refresh();
            RecalculateBoundsAndRebuild();
        }
    }

}

