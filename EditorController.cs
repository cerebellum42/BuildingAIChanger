﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace BuildingAIChanger
{
    public class EditorController : MonoBehaviour
    {
        private UIView m_view;
        private SelectAIPanel m_selectAIPanel;
        private ToolController m_toolController;
        private UIPanel m_propPanel;
        private UIComponent m_uiContainer;

        public void Start()
        {
            m_view = UIView.GetAView();

            m_selectAIPanel = m_view.FindUIComponent<SelectAIPanel>("SelectAIPanel");
            m_uiContainer = m_view.FindUIComponent("FullScreenContainer");
            m_propPanel = m_uiContainer.Find<UIPanel>("DecorationProperties");

            m_toolController = ToolsModifierControl.toolController;
            m_selectAIPanel.eventValueChanged += OnAIFieldChanged;
            m_toolController.eventEditPrefabChanged += OnEditPrefabChanged;
        }

        private void OnEditPrefabChanged(PrefabInfo info)
        {
            m_selectAIPanel.value = info.GetAI().GetType().FullName;
        }

        private void OnAIFieldChanged(UIComponent component, string value)
        {
            var buildingInfo = (BuildingInfo) m_toolController.m_editPrefabInfo;
            if (m_selectAIPanel.IsValueValid())
            {
                // remove old ai
                var oldAI = buildingInfo.gameObject.GetComponent<PrefabAI>();
                DestroyImmediate(oldAI);

                // add new ai
                var type = m_selectAIPanel.TryGetAIType();
                var newAI = (PrefabAI) buildingInfo.gameObject.AddComponent(type);

                TryCopyAttributes(oldAI, newAI);

                buildingInfo.DestroyPrefabInstance();
                buildingInfo.InitializePrefabInstance();
                RefreshPropertiesPanel(buildingInfo);
            }
        }

        /**
         * Copies all attributes that share the same name and type from one AI object to another
         */
        private void TryCopyAttributes(PrefabAI oldAI, PrefabAI newAI)
        {
            var oldAIFields =
                oldAI.GetType()
                    .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                               BindingFlags.FlattenHierarchy);
            var newAIFields = newAI.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                           BindingFlags.FlattenHierarchy);

            var newAIFieldDic = new Dictionary<String, FieldInfo>(newAIFields.Length);
            foreach (FieldInfo field in newAIFields)
            {
                newAIFieldDic.Add(field.Name, field);
            }

            foreach (FieldInfo fieldInfo in oldAIFields)
            {
                if (fieldInfo.IsDefined(typeof (CustomizablePropertyAttribute), true))
                {
                    FieldInfo newAIField;
                    newAIFieldDic.TryGetValue(fieldInfo.Name, out newAIField);

                    try
                    {
                        if (newAIField.GetType().Equals(fieldInfo.GetType()))
                        {
                            newAIField.SetValue(newAI, fieldInfo.GetValue(oldAI));
                        }
                    }
                    catch (NullReferenceException) {}
                }
            }
        }

        private void RefreshPropertiesPanel(BuildingInfo prefabInfo)
        {
            var decorationPropertiesPanel = m_propPanel.GetComponent<DecorationPropertiesPanel>();
            decorationPropertiesPanel.GetType()
                .InvokeMember("Refresh", BindingFlags.InvokeMethod | BindingFlags.NonPublic, null,
                    decorationPropertiesPanel, new object[] {prefabInfo});
        }
    }
}
