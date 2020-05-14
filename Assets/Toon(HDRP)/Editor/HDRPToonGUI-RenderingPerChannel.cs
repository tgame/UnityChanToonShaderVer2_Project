﻿//Unitychan Toon Shader ver.8.0
//v.8.0.0
//nobuyuki@unity3d.com
//toshiyuki@unity3d.com (Univerasl RP/HDRP) 
//https://github.com/unity3d-jp/UnityChanToonShaderVer2_Project
//(C)Unity Technologies Japan/UCL
//Everything realated to Rendering per channel is controlled in this file.
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace UnityEditor.Rendering.HDRP.Toon
{
    public partial class HDRPToonGUI : ShaderGUI
    {

        public enum _ChannelEnum
        {
            BaseColor,
            FirstShade,
            SecondShade,
            Highlight,
            AngelRing,
            Outline,
            Max,
        };



        ReorderableList m_ReorderableList;
        List<string> m_channelNames;
        GUIContent m_colorPickerContent;
        Texture2D m_texIconVisible;
        Texture2D m_texIconInvisible;

        GUIStyle  m_ToggleStyle;



        const string ShaderProp_BaseColorVisible = "_BaseColorVisible";
        const string ShaderProp_BaseColorOverridden = "_BaseColorOverridden";

        const string ShaderProp_FirstShadeVisible = "_FirstShadeVisible";
        const string ShaderProp_FirstShadeOverridden = "_FirstShadeOverridden";

        const string ShaderProp_SecondShadeVisible = "_SecondShadeVisible";
        const string ShaderProp_SecondShadeOverridden = "_SecondShadeOverridden";

        const string ShaderProp_HighlightVisible = "_HighlightVisible";
        const string ShaderProp_HighlightOverridden = "_HighlightOverridden";

        const string ShaderProp_AngelRingVisible = "_AngelRingVisible";
        const string ShaderProp_AngelRingOverridden = "_AngelRingOverridden";

        const string ShaderProp_OutlineVisible = "_OutlineVisible";
        const string ShaderProp_OutlineOverridden = "_OutlineOverridden";

        static bool _PerChanelShaderSettings_Foldout = false;

        void RenderingPerChennelsSetting(Material material)
        {
            SetupChannelSettings(material);


            if (m_ReorderableList != null)
            {
                m_ReorderableList.DoLayoutList();
            }

        }

        void SetupChannelSettings(Material material)
        {

            if ( m_texIconVisible == null )
            {
                m_texIconVisible = (Texture2D)EditorGUIUtility.Load("d_scenevis_visible_hover@2x");
//                m_texIconVisible = ResizeThumbnailTexture(m_texIconVisible, 20, 20);

            }
            if (m_texIconInvisible == null)
            {
                m_texIconInvisible = (Texture2D)EditorGUIUtility.Load("d_SceneViewVisibility@2x");
//                m_texIconInvisible = ResizeThumbnailTexture(m_texIconInvisible, 20, 20);

            }

            if (m_ToggleStyle == null)
            {
                m_ToggleStyle = new GUIStyle(EditorStyles.toggle);


                m_ToggleStyle.normal.background = m_texIconInvisible;
                m_ToggleStyle.normal.scaledBackgrounds = new Texture2D[] { m_texIconInvisible };
                m_ToggleStyle.onNormal.background = m_texIconVisible;
                m_ToggleStyle.onNormal.scaledBackgrounds = new Texture2D[] { m_texIconVisible };

                m_ToggleStyle.active.background = m_texIconInvisible;
                m_ToggleStyle.active.scaledBackgrounds = new Texture2D[] { m_texIconInvisible };
                m_ToggleStyle.onActive.background = m_texIconVisible;
                m_ToggleStyle.onActive.scaledBackgrounds = new Texture2D[] { m_texIconVisible };
              
                m_ToggleStyle.focused.background = m_texIconInvisible;
                m_ToggleStyle.focused.scaledBackgrounds = new Texture2D[] { m_texIconInvisible };
                m_ToggleStyle.onFocused.background = m_texIconVisible;
                m_ToggleStyle.onFocused.scaledBackgrounds = new Texture2D[] { m_texIconVisible };

                m_ToggleStyle.hover.background = m_texIconInvisible;
                m_ToggleStyle.hover.scaledBackgrounds = new Texture2D[] { m_texIconInvisible };
                m_ToggleStyle.onHover.background = m_texIconVisible;
                m_ToggleStyle.onHover.scaledBackgrounds = new Texture2D[] { m_texIconVisible };
            }
            if (m_channelNames == null)
            {
                m_channelNames = new List<string>();
                m_channelNames.Add(_ChannelEnum.BaseColor.ToString());
                m_channelNames.Add(_ChannelEnum.FirstShade.ToString());
                m_channelNames.Add(_ChannelEnum.SecondShade.ToString());
                m_channelNames.Add(_ChannelEnum.Highlight.ToString());
                m_channelNames.Add(_ChannelEnum.AngelRing.ToString());
                m_channelNames.Add(_ChannelEnum.Outline.ToString());
            }
            if (m_colorPickerContent == null )
            {
                m_colorPickerContent = new GUIContent(string.Empty);
            }
            if (m_ReorderableList == null)
            {
                m_ReorderableList = new ReorderableList(m_channelNames, typeof(string));
                m_ReorderableList.displayAdd = false;
                m_ReorderableList.displayRemove = false;
                m_ReorderableList.draggable = false;
                m_ReorderableList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Channel Mask Setting");
                m_ReorderableList.drawElementCallback = (rect_, index, isActive, isFocused) =>
                {
                    Rect toggleRectVislble = new Rect(rect_)
                    {
                        height = 16,
                        width = 16,
                        x = rect_.x + 6,
                        y = rect_.y 
                    };
                    Rect toggleRectOverride = new Rect(rect_)
                    {
                        height = 16,
                        width = 16,
                        x = rect_.x + 6 + 22,
                        y = rect_.y
                    };
                    Rect colorPickerRect = new Rect(rect_)
                    {
                        height = 16,
                        width = 16,
                        x = rect_.x + 6 + 22 * 2,
                        y = rect_.y
                    };
                    Rect nameRect = new Rect(rect_)
                    {
                        height = 16,
                        width = 120,
                        x = rect_.x + 6 + 22 * 3,
                        y = rect_.y
                    };
                    string propNameVisible = "_" + m_channelNames[index].ToString() + "Visible";
                    string propNameOverriden = "_" + m_channelNames[index].ToString() + "Overridden";
                    string propNameColor = "_" + m_channelNames[index].ToString() + "MaskColor";
                    bool isVisible = material.GetFloat(propNameVisible) > 0.0f;
                    EditorGUI.BeginChangeCheck();
                    isVisible = EditorGUI.Toggle(toggleRectVislble, isVisible, m_ToggleStyle);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(material, "Layer visiblity is changed");
                        material.SetFloat(propNameVisible, isVisible ? 1.0f : 0.0f);
                    }

                    using (new EditorGUI.DisabledScope(isVisible == false))
                    {
                        EditorGUI.BeginChangeCheck();
                        bool toggleOverride = material.GetFloat(propNameOverriden) > 0.0f;
                        toggleOverride = EditorGUI.Toggle(toggleRectOverride, toggleOverride);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(material, "Layer mask is changed");
                            material.SetFloat(propNameOverriden, toggleOverride ? 1.0f : 0.0f);
                        }


                        Color color = material.GetColor(propNameColor);
                        color *= toggleOverride == false ? 0.5f : 1.0f;
                        EditorGUI.BeginChangeCheck();
                        color = EditorGUI.ColorField(colorPickerRect, m_colorPickerContent, color, false, false, false);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(material, "Layer mask color is changed");
                            material.SetColor(propNameColor, color);
                        }
                        EditorGUI.LabelField(nameRect, m_channelNames[index]);
                    }
                };
            }
        }
        void ApplyRenderingPerChennelsSetting(Material material)
        {

        }

        Texture2D ResizeThumbnailTexture(Texture2D tex, int sizX, int sizY)
        {
            var rt = RenderTexture.GetTemporary(sizX, sizY);
            var previous = RenderTexture.active;

            RenderTexture.active = rt;
            Graphics.Blit(tex, rt);

            var newTexture = new Texture2D(sizX, sizY);
            newTexture.ReadPixels(new Rect(0, 0, sizX, sizY), 0, 0);
            newTexture.Apply();
            RenderTexture.active = previous;

            RenderTexture.ReleaseTemporary(rt);
            return newTexture;

        }
    } // End of UTS2GUI2
}// End of namespace UnityChan