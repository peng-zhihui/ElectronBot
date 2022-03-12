using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;
using System;
using UnityEngine.UI;
using UnityEditor.AnimatedValues;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(ProceduralImage), true)]
    [CanEditMultipleObjects]
    public class ProceduralImageEditor : ImageEditor
    {
        static List<ModifierID> attrList;

        SerializedProperty m_borderWidth;
        SerializedProperty m_falloffDist;

        SerializedProperty m_FillMethod;
        SerializedProperty m_FillOrigin;
        SerializedProperty m_FillAmount;
        SerializedProperty m_FillClockwise;
        SerializedProperty m_Type;
        SerializedProperty m_Sprite;

        AnimBool showFilled;

        GUIContent spriteTypeContent = new GUIContent("Image Type");
        GUIContent clockwiseContent = new GUIContent("Clockwise");

        int selectedId;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_Type = serializedObject.FindProperty("m_Type");
            m_FillMethod = serializedObject.FindProperty("m_FillMethod");
            m_FillOrigin = serializedObject.FindProperty("m_FillOrigin");
            m_FillClockwise = serializedObject.FindProperty("m_FillClockwise");
            m_FillAmount = serializedObject.FindProperty("m_FillAmount");
            m_Sprite = serializedObject.FindProperty("m_Sprite");

            var typeEnum = (Image.Type)m_Type.enumValueIndex;

            showFilled = new AnimBool(!m_Type.hasMultipleDifferentValues && typeEnum == Image.Type.Filled);
            showFilled.valueChanged.AddListener(Repaint);

            attrList = ModifierUtility.GetAttributeList();

            m_borderWidth = serializedObject.FindProperty("borderWidth");
            m_falloffDist = serializedObject.FindProperty("falloffDistance");

            if ((target as ProceduralImage).GetComponent<ProceduralImageModifier>() != null)
            {
                selectedId = attrList.IndexOf(((ModifierID[])(target as ProceduralImage).GetComponent<ProceduralImageModifier>().GetType().GetCustomAttributes(typeof(ModifierID), false))[0]);
            }
            selectedId = Mathf.Max(selectedId, 0);
            EditorApplication.update -= UpdateProceduralImage;
            EditorApplication.update += UpdateProceduralImage;
        }

        /// <summary>
        /// Updates the procedural image in Edit mode. This will prevent issues when working with layout components.
        /// </summary>
        public void UpdateProceduralImage()
        {
            if (target != null)
            {
                (target as ProceduralImage).Update();
            }
            else
            {
                EditorApplication.update -= UpdateProceduralImage;
            }
        }

        public override void OnInspectorGUI()
        {

            CheckForShaderChannelsGUI();

            serializedObject.Update();

            ProceduralImageSpriteGUI();

            EditorGUILayout.PropertyField(m_Color);
            //EditorGUILayout.PropertyField(m_Material);
            
            RaycastControlsGUI();
            ProceduralImageTypeGUI();
            EditorGUILayout.Space();
            ModifierGUI();
            EditorGUILayout.PropertyField(m_borderWidth);
            EditorGUILayout.PropertyField(m_falloffDist);
            serializedObject.ApplyModifiedProperties();
        }

        protected void ProceduralImageSpriteGUI()
        {
            if (m_Sprite.hasMultipleDifferentValues)
            {
                EditorGUILayout.PropertyField(m_Sprite);
            }
            else
            {
                Sprite s = (Sprite)EditorGUILayout.ObjectField("Sprite", EmptySprite.IsEmptySprite((Sprite)m_Sprite.objectReferenceValue) ? null : m_Sprite.objectReferenceValue, typeof(Sprite), false, GUILayout.Height(16));
                if (s != null)
                {
                    m_Sprite.objectReferenceValue = s;
                }
                else
                {
                    m_Sprite.objectReferenceValue = EmptySprite.Get();
                }
            }
        }

        /// <summary>
        /// Sprites's custom properties based on the type.
        /// </summary>

        protected void ProceduralImageTypeGUI()
        {
            if (m_Type.hasMultipleDifferentValues)
            {
                int idx = Convert.ToInt32(EditorGUILayout.EnumPopup(spriteTypeContent, (ProceduralImageType)(-1)));
                if (idx != -1)
                {
                    m_Type.enumValueIndex = idx;
                }
            }
            else
            {
                m_Type.enumValueIndex = Convert.ToInt32(EditorGUILayout.EnumPopup(spriteTypeContent, (ProceduralImageType)m_Type.enumValueIndex));
            }

            ++EditorGUI.indentLevel;
            {
                Image.Type typeEnum = (Image.Type)m_Type.enumValueIndex;

                showFilled.target = (!m_Type.hasMultipleDifferentValues && typeEnum == Image.Type.Filled);

                if (EditorGUILayout.BeginFadeGroup(showFilled.faded))
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(m_FillMethod);
                    if (EditorGUI.EndChangeCheck())
                    {
                        m_FillOrigin.intValue = 0;
                    }
                    switch ((Image.FillMethod)m_FillMethod.enumValueIndex)
                    {
                        case Image.FillMethod.Horizontal:
                            m_FillOrigin.intValue = (int)(Image.OriginHorizontal)EditorGUILayout.EnumPopup("Fill Origin", (Image.OriginHorizontal)m_FillOrigin.intValue);
                            break;
                        case Image.FillMethod.Vertical:
                            m_FillOrigin.intValue = (int)(Image.OriginVertical)EditorGUILayout.EnumPopup("Fill Origin", (Image.OriginVertical)m_FillOrigin.intValue);
                            break;
                        case Image.FillMethod.Radial90:
                            m_FillOrigin.intValue = (int)(Image.Origin90)EditorGUILayout.EnumPopup("Fill Origin", (Image.Origin90)m_FillOrigin.intValue);
                            break;
                        case Image.FillMethod.Radial180:
                            m_FillOrigin.intValue = (int)(Image.Origin180)EditorGUILayout.EnumPopup("Fill Origin", (Image.Origin180)m_FillOrigin.intValue);
                            break;
                        case Image.FillMethod.Radial360:
                            m_FillOrigin.intValue = (int)(Image.Origin360)EditorGUILayout.EnumPopup("Fill Origin", (Image.Origin360)m_FillOrigin.intValue);
                            break;
                    }
                    EditorGUILayout.PropertyField(m_FillAmount);
                    if ((Image.FillMethod)m_FillMethod.enumValueIndex > Image.FillMethod.Vertical)
                    {
                        EditorGUILayout.PropertyField(m_FillClockwise, clockwiseContent);
                    }
                }
                EditorGUILayout.EndFadeGroup();
            }
            --EditorGUI.indentLevel;
        }

        void CheckForShaderChannelsGUI()
        {
            Canvas c = (target as Component).GetComponentInParent<Canvas>();
            if (c != null && (c.additionalShaderChannels | AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.TexCoord2 | AdditionalCanvasShaderChannels.TexCoord3) != c.additionalShaderChannels)
            {
                //Texcoord1 not enabled;
                EditorGUILayout.HelpBox("TexCoord1,2,3 are not enabled as an additional shader channel in parent canvas. Procedural Image will not work properly", MessageType.Error);
                if (GUILayout.Button("Fix: Enable TexCoord1,2,3 in Canvas: " + c.name))
                {
                    Undo.RecordObject(c, "enable TexCoord1,2,3 as additional shader channels");
                    c.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.TexCoord2 | AdditionalCanvasShaderChannels.TexCoord3;
                }
            }
        }

        protected void ModifierGUI()
        {
            GUIContent[] con = new GUIContent[attrList.Count];
            for (int i = 0; i < con.Length; i++)
            {
                con[i] = new GUIContent(attrList[i].Name);
            }


            bool hasMultipleValues = false;
            if (targets.Length > 1)
            {
                Type t = (targets[0] as ProceduralImage).GetComponent<ProceduralImageModifier>().GetType();
                foreach (var item in targets)
                {
                    if ((item as ProceduralImage).GetComponent<ProceduralImageModifier>().GetType() != t)
                    {
                        hasMultipleValues = true;
                        break;
                    }
                }
            }

            if(!hasMultipleValues)
            {
                int index = EditorGUILayout.Popup(new GUIContent("Modifier Type"), selectedId, con);
                if (selectedId != index)
                {
                    selectedId = index;
                    foreach (var item in targets)
                    {
                        (item as ProceduralImage).ModifierType = ModifierUtility.GetTypeWithId(attrList[selectedId].Name);

                        MoveComponentBehind((item as ProceduralImage), (item as ProceduralImage).GetComponent<ProceduralImageModifier>());
                    }
                    //Exit GUI prevents Unity from trying to draw destroyed components editor;
                    EditorGUIUtility.ExitGUI();
                }
            }
            else{
                int index = EditorGUILayout.Popup(new GUIContent("Modifier Type"), -1, con);
                if (index != -1)
                {
                    selectedId = index;
                    foreach (var item in targets)
                    {
                        (item as ProceduralImage).ModifierType = ModifierUtility.GetTypeWithId(attrList[selectedId].Name);

                        MoveComponentBehind((item as ProceduralImage), (item as ProceduralImage).GetComponent<ProceduralImageModifier>());
                    }
                    //Exit GUI prevents Unity from trying to draw destroyed components editor;
                    EditorGUIUtility.ExitGUI();
                }
            }
        }

        public override string GetInfoString()
        {
            ProceduralImage image = target as ProceduralImage;
            return string.Format("Modifier: {0}, Line-Weight: {1}", attrList[selectedId].Name, image.BorderWidth);
        }
        /// <summary>
        /// Moves a component behind a reference component.
        /// </summary>
        /// <param name="reference">Reference component.</param>
        /// <param name="componentToMove">Component to move.</param>
        static void MoveComponentBehind(Component reference, Component componentToMove)
        {
            if (reference == null || componentToMove == null || reference.gameObject != componentToMove.gameObject)
            {
                return;
            }
            Component[] comps = reference.GetComponents<Component>();
            List<Component> list = new List<Component>();
            list.AddRange(comps);
            int i = list.IndexOf(componentToMove) - list.IndexOf(reference);
            while (i != 1)
            {
                if (i < 1)
                {
                    UnityEditorInternal.ComponentUtility.MoveComponentDown(componentToMove);
                    i++;
                }
                else if (i > 1)
                {
                    UnityEditorInternal.ComponentUtility.MoveComponentUp(componentToMove);
                    i--;
                }
            }
        }

        protected enum ProceduralImageType
        {
            Simple = 0,
            Filled = 3
        }
    }
}