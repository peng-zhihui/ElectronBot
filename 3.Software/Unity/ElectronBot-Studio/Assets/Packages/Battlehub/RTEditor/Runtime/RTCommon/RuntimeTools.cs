using System;
using UnityEngine;
using UnityEngine.EventSystems;

using UnityObject = UnityEngine.Object;
namespace Battlehub.RTCommon
{
    public enum RuntimeTool
    {
        None,
        Move,
        Rotate,
        Scale,
        View,
        Rect,
        Custom
    }

    public enum RuntimePivotRotation
    {
        Local,
        Global
    }

    public enum RuntimePivotMode
    {
        Center = 0,
        Pivot = 1
    }

    public enum SnappingMode
    {
        BoundingBox,
        Vertex,
    }

    public delegate void RuntimeToolsEvent<T1, T2>(T1 arg1, T2 arg2);
    public delegate void RuntimeToolsEvent();
    public delegate void SpawnPrefabChanged(GameObject oldPrefab);
  
    /// <summary>
    /// Runtime tools and flags
    /// </summary>
    public class RuntimeTools
    {
        public event RuntimeToolsEvent<RuntimeTool, object> ToolChanging;
        public event RuntimeToolsEvent ToolChanged;

        public event RuntimeToolsEvent PivotRotationChanging;
        public event RuntimeToolsEvent PivotRotationChanged;
        public event RuntimeToolsEvent PivotModeChanging;
        public event RuntimeToolsEvent PivotModeChanged;
    
        public event RuntimeToolsEvent IsViewingChanged;
        public event RuntimeToolsEvent ShowSelectionGizmosChanged;
        public event RuntimeToolsEvent ShowGizmosChanged;
        public event RuntimeToolsEvent AutoFocusChanged;
        public event RuntimeToolsEvent UnitSnappingChanged;
        public event RuntimeToolsEvent IsSnappingChanged;
        public event RuntimeToolsEvent SnappingModeChanged;
        public event RuntimeToolsEvent LockAxesChanged;

        private RuntimeTool m_current;
        private RuntimePivotMode m_pivotMode;
        private RuntimePivotRotation m_pivotRotation;

        private bool m_isViewing;
        public bool IsViewing
        {
            get { return m_isViewing; }
            set
            {
                if(m_isViewing != value)
                {
                    m_isViewing = value;
                    if(m_isViewing)
                    {
                        ActiveTool = null;
                    }
                    if(IsViewingChanged != null)
                    {
                        IsViewingChanged();
                    }
                }
            }
        }

        private bool m_showSelectionGizmos;
        public bool ShowSelectionGizmos
        {
            get { return m_showSelectionGizmos; }
            set
            {
                if(m_showSelectionGizmos != value)
                {
                    m_showSelectionGizmos = value;
                    if(ShowSelectionGizmosChanged != null)
                    {
                        ShowSelectionGizmosChanged();
                    }
                }
            }
        }

        private bool m_showGizmos;
        public bool ShowGizmos
        {
            get { return m_showGizmos; }
            set
            {
                if(m_showGizmos != value)
                {
                    m_showGizmos = value;
                    if(ShowGizmosChanged != null)
                    {
                        ShowGizmosChanged();
                    }
                }
            }
        }

        private bool m_autoFocus;
        public bool AutoFocus
        {
            get { return m_autoFocus; }
            set
            {
                if(m_autoFocus != value)
                {
                    m_autoFocus = value;
                    if(AutoFocusChanged != null)
                    {
                        AutoFocusChanged();
                    }
                }
            }
        }

        private bool m_unitSnapping;
        public bool UnitSnapping
        {
            get { return m_unitSnapping; }
            set
            {
                if(m_unitSnapping != value)
                {
                    m_unitSnapping = value;
                    if(UnitSnappingChanged != null)
                    {
                        UnitSnappingChanged();
                    }
                }
            }
        }

        private bool m_isSnapping;
        public bool IsSnapping
        {
            get { return m_isSnapping; }
            set
            {
                if(m_isSnapping != value)
                {
                    m_isSnapping = value;
                    if(IsSnappingChanged != null)
                    {
                        IsSnappingChanged();
                    }
                }
            }
        }

        private SnappingMode m_snappingMode = SnappingMode.Vertex;
        public SnappingMode SnappingMode
        {
            get { return m_snappingMode; }
            set
            {
                if(m_snappingMode != value)
                {
                    m_snappingMode = value;
                    if(SnappingModeChanged != null)
                    {
                        SnappingModeChanged();
                    }
                }
            }
        }

        private UnityObject m_activeTool;
        public UnityObject ActiveTool
        {
            get { return m_activeTool; }
            set
            {
                m_activeTool = value;
            }
        }

        public LockObject m_lockAxes;
        public LockObject LockAxes
        {
            get { return m_lockAxes; }
            set
            {
                if(m_lockAxes != value)
                {
                    m_lockAxes = value;
                    if(LockAxesChanged != null)
                    {
                        LockAxesChanged();
                    }
                }
            }
        }

        public RuntimeTool Current
        {
            get { return m_current; }
            set
            {
                if (m_current != value)
                {
                    if(ToolChanging != null)
                    {
                        ToolChanging(value, null);
                    }
                    m_current = value;
                    if(m_current != RuntimeTool.Custom)
                    {
                        m_isBoxSelectionEnabled = true;
                    }
                    m_custom = null;
                    if (ToolChanged != null)
                    {
                        ToolChanged();
                    }
                }
            }
        }

        private object m_custom;
        public object Custom
        {
            get { return m_custom; }
            set
            {
                if(m_custom != value)
                {
                    if (ToolChanging != null)
                    {
                        ToolChanging(RuntimeTool.Custom, value);
                    }
                    m_current = RuntimeTool.Custom;
                    m_custom = value;
                    if(ToolChanged != null)
                    {
                        ToolChanged();
                    }
                }
            }
        }

        private bool m_isBoxSelectionEnabled = true;
        public bool IsBoxSelectionEnabled
        {
            get { return m_isBoxSelectionEnabled; }
            set { m_isBoxSelectionEnabled = value; }
        }

        public RuntimePivotRotation PivotRotation
        {
            get { return m_pivotRotation; }
            set
            {
                if (m_pivotRotation != value)
                {
                    if(PivotRotationChanging != null)
                    {
                        PivotRotationChanging();
                    }
                    m_pivotRotation = value;
                    if (PivotRotationChanged != null)
                    {
                        PivotRotationChanged();
                    }
                }
            }
        }

        public RuntimePivotMode PivotMode
        {
            get { return m_pivotMode; }
            set
            {
                if(m_pivotMode != value)
                {
                    if(PivotModeChanging != null)
                    {
                        PivotModeChanging();
                    }
                    m_pivotMode = value;
                    if(PivotModeChanged != null)
                    {
                        PivotModeChanged();
                    }
                }
            }
        }

        public RuntimeTools()
        {
            Reset();
        }

        public void Reset()
        {
            ActiveTool = null;
            LockAxes = null;
            Custom = null;
            m_isViewing = false;
            m_isSnapping = false;
            m_showSelectionGizmos = true;
            m_showGizmos = true;
            m_unitSnapping = false;
            m_pivotMode = RuntimePivotMode.Center;
        }
    }
}
