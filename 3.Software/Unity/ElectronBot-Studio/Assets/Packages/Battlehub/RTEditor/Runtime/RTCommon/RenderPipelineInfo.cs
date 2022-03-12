using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

namespace Battlehub.RTCommon
{
    public enum RPType
    {
        Unknown,
        Standard,
        HDRP,
        URP,
    }

    public interface IRenderPipelineCameraUtility
    {
        event Action<Camera, bool> PostProcessingEnabled;

        void Stack(Camera baseCamera, Camera overlayCamera);
        void RequiresDepthTexture(Camera camera, bool value);

        bool IsPostProcessingEnabled(Camera camera);
        void EnablePostProcessing(Camera camera, bool value);

        void SetBackgroundColor(Camera camera, Color color);
    }

    public static class RenderPipelineInfo 
    {
        public static bool ForceUseRenderTextures = true;
        public static bool UseRenderTextures
        {
            get { return ForceUseRenderTextures; }
        }

        public static readonly RPType Type;
        public static readonly string DefaultShaderName;
        public static readonly string DefaultTerrainShaderName;
        public static readonly int ColorPropertyID;
        public static readonly int MainTexturePropertyID;

        private static readonly PropertyInfo m_msaaProperty;
        public static int MSAASampleCount
        {
            get
            {
                switch (Type)
                {
                    case RPType.Standard:
                        return QualitySettings.antiAliasing;
                    default:
                        if (m_msaaProperty != null)
                        {
                            try
                            {
                                return Convert.ToInt32(m_msaaProperty.GetValue(GraphicsSettings.renderPipelineAsset));
                            }
                            catch(Exception e)
                            {
                                Debug.LogError(e);
                            }   
                        }
                        return 1;
                }
            }
        }


        private static Material m_defaultMaterial;
        public static Material DefaultMaterial
        {
            get
            {
                if(m_defaultMaterial == null)
                {
                    m_defaultMaterial = new Material(Shader.Find(DefaultShaderName));
                    m_defaultMaterial.Color(Color.white);
                }

                return m_defaultMaterial;
            }
        }

        private static Material m_defaultTerrainMaterial;
        public static Material DefaultTerrainMaterial
        {
            get
            {
                if(m_defaultTerrainMaterial == null)
                {
                    m_defaultTerrainMaterial = new Material(Shader.Find(DefaultTerrainShaderName));
                }
                return m_defaultTerrainMaterial;
            }
        }


        static RenderPipelineInfo()
        {
            if (GraphicsSettings.renderPipelineAsset == null)
            {
                Type = RPType.Standard;
                ForceUseRenderTextures = false;
                DefaultShaderName = "Standard";
                ColorPropertyID = Shader.PropertyToID("_Color");
                MainTexturePropertyID = Shader.PropertyToID("_MainTex");
            }
            else
            {
                Type pipelineType = GraphicsSettings.renderPipelineAsset.GetType();
                if (pipelineType.Name == "UniversalRenderPipelineAsset")
                {
                    Type = RPType.URP;
                    ForceUseRenderTextures = false;
                    m_msaaProperty = pipelineType.GetProperty("msaaSampleCount");
                    DefaultShaderName = "Universal Render Pipeline/Lit";
                    DefaultTerrainShaderName = "Universal Render Pipeline/Terrain/Lit";
                    ColorPropertyID = Shader.PropertyToID("_BaseColor");
                    MainTexturePropertyID = Shader.PropertyToID("_BaseMap");
                }
                else if (pipelineType.Name == "HDRenderPipelineAsset")
                {
                    Type = RPType.HDRP;
                    ForceUseRenderTextures = false;
                    m_msaaProperty = pipelineType.GetProperty("msaaSampleCount");
                    DefaultShaderName = "HDRP/Lit";
                    DefaultTerrainShaderName = "HDRP/TerrainLit";
                    ColorPropertyID = Shader.PropertyToID("_BaseColor"); 
                    MainTexturePropertyID = Shader.PropertyToID("_BaseColorMap"); 
                }
                else
                {
                    Debug.Log(GraphicsSettings.renderPipelineAsset.GetType());
                    Type = RPType.Unknown;
                    ColorPropertyID = Shader.PropertyToID("_Color");
                    MainTexturePropertyID = Shader.PropertyToID("_MainTex");
                }
            }
        }

        public static bool IsXRDeviceResent()
        {
            var xrDisplaySubsystems = new List<XRDisplaySubsystem>();
            SubsystemManager.GetInstances(xrDisplaySubsystems);
            foreach (var xrDisplay in xrDisplaySubsystems)
            {
                if (xrDisplay.running)
                {
                    return true;
                }
            }
            return false;
        }
        public static void XRFix(Camera camera)
        {
            if (Type == RPType.Standard && IsXRDeviceResent())
            {
                if (camera.allowMSAA || camera.allowHDR)
                {
                    Debug.LogFormat("XRDevice present. Setting camera {0} allowMSSA and allowHDR to false", camera.name);

                    //unity 2019.3.15 Standard RP, camera viewport rect behaves incorrectly -> hotfix : allowMSAA = false
                    camera.allowMSAA = false;
                    camera.allowHDR = false;
                }   
            }
        }
    }


    public static class MaterialExt
    {
        public static int MainTexturePropertyID = Shader.PropertyToID("_MainTex");
        public static int ColorPropertyID = Shader.PropertyToID("_Color");

        public static Texture MainTexture(this Material material)
        {
            if(material.HasProperty(RenderPipelineInfo.MainTexturePropertyID))
            {
                return material.GetTexture(RenderPipelineInfo.MainTexturePropertyID);
            }
            else if(material.HasProperty(MainTexturePropertyID))
            {
                return material.GetTexture(MainTexturePropertyID);
            }
            return null;
        }

        public static void MainTexture(this Material material, Texture texture)
        {
            if (material.HasProperty(RenderPipelineInfo.MainTexturePropertyID))
            {
                material.SetTexture(RenderPipelineInfo.MainTexturePropertyID, texture);
            }
            else if(material.HasProperty(MainTexturePropertyID))
            {
                material.SetTexture(MainTexturePropertyID, texture);
            }
        }

        public static Vector2 MainTextureScale(this Material material)
        {
            if (material.HasProperty(RenderPipelineInfo.MainTexturePropertyID))
            {
                return material.GetTextureScale(RenderPipelineInfo.MainTexturePropertyID);
            }
            else if (material.HasProperty(MainTexturePropertyID))
            {
                return material.GetTextureScale(MainTexturePropertyID);
            }
            return Vector2.one;
        }

        public static void MainTextureScale(this Material material, Vector2 scale)
        {
            if (material.HasProperty(RenderPipelineInfo.MainTexturePropertyID))
            {
                material.SetTextureScale(RenderPipelineInfo.MainTexturePropertyID, scale);
            }
            else if (material.HasProperty(MainTexturePropertyID))
            {
                material.SetTextureScale(MainTexturePropertyID, scale);
            }   
        }

        public static Vector2 MainTextureOffset(this Material material)
        {
            if (material.HasProperty(RenderPipelineInfo.MainTexturePropertyID))
            {
                return material.GetTextureOffset(RenderPipelineInfo.MainTexturePropertyID);
            }
            else if (material.HasProperty(MainTexturePropertyID))
            {
                return material.GetTextureOffset(MainTexturePropertyID);
            }
            return Vector2.zero;
        }

        public static void MainTextureOffset(this Material material, Vector2 offset)
        {
            if (material.HasProperty(RenderPipelineInfo.MainTexturePropertyID))
            {
                material.SetTextureOffset(RenderPipelineInfo.MainTexturePropertyID, offset);
            }
            else if (material.HasProperty(MainTexturePropertyID))
            {
                material.SetTextureOffset(MainTexturePropertyID, offset);
            }
        }

        public static Color Color(this Material material)
        {
            if(material.HasProperty(RenderPipelineInfo.ColorPropertyID))
            {
                return material.GetColor(RenderPipelineInfo.ColorPropertyID);
            }
            else if(material.HasProperty(ColorPropertyID))
            {
                return material.GetColor(ColorPropertyID);
            }
            return UnityEngine.Color.white;
        }

        public static void Color(this Material material, Color color)
        {
            if (material.HasProperty(RenderPipelineInfo.ColorPropertyID))
            {
                material.SetColor(RenderPipelineInfo.ColorPropertyID, color);
            }
            else if (material.HasProperty(ColorPropertyID))
            {
                material.SetColor(ColorPropertyID, color);
            }  
        }
    }

    public class MaterialAcessor
    {
        private GameObject m_gameObject;
        private int m_materialIndex;

        public MaterialAcessor(GameObject gameObject, int materialIndex)
        {
            m_gameObject = gameObject;
            m_materialIndex = materialIndex;
        }

        public Material material
        {
            get { return m_gameObject.GetComponent<Renderer>().sharedMaterials[m_materialIndex]; }
        }

        public Texture mainTexture
        {
            get { return material.MainTexture(); }
            set { material.MainTexture(value); }
        }

        public Color color
        {
            get { return material.Color(); }
            set { material.Color(value); }
        }
    }
}

;