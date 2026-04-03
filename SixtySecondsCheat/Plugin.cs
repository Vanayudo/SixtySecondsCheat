using BepInEx;
using BepInEx.Logging;
using System;
using System.Reflection;
using UnityEngine;

namespace SixtySecondsCheat
{
    [BepInPlugin("com.roeyqian.sixtysecondscheat", "60 Seconds Cheat", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private static ManualLogSource _log;
        private bool _windowOpen;
        private Vector2 _scrollPos;
        private string _statusText = "按 F6 打开/关闭菜单";

        private static readonly string[][] ItemNames =
        [
            ["医疗包", "FirstAid", "Remedium"],
            ["防毒面具", "GasMask", "Item"],
            ["门锁", "Padlock", "Item"],
            ["杀虫剂", "Spray", "Item"],
            ["弹药", "Ammo", "Item"],
            ["步枪", "Rifle", "Item"],
            ["斧头", "Axe", "Item"],
            ["地图", "Map", "Item"],
            ["收音机", "Radio", "Item"],
            ["手电筒", "Torch", "Item"],
            ["童子军手册", "Handbook", "Item"],
            ["手提箱", "Suitcase", "Item"],
            ["跳棋", "Checkers", "Item"],
            ["扑克牌", "Cards", "Item"],
            ["口琴", "Harmonica", "Item"]
        ];

        private void Awake()
        {
            _log = Logger;
            _log.LogInfo("=== 60 Seconds Cheat v1.0 ===");
            _log.LogInfo("F6 = 打开/关闭菜单");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F6))
            {
                _windowOpen = !_windowOpen;
            }
        }

        private void OnGUI()
        {
            if (!_windowOpen) return;

            GUI.Window(12345, new Rect(10, 10, 400, 700), DrawWindow, "60 Seconds Cheat v1.0");
        }

        private void DrawWindow(int id)
        {
            GUILayout.Space(5);

            _scrollPos = GUILayout.BeginScrollView(_scrollPos, GUILayout.Height(580));

            GUILayout.BeginHorizontal();
            GUILayout.Label("食物", GUILayout.Width(80));
            if (GUILayout.Button("+1", GUILayout.Width(60))) ModifyConsumable("Food", 1);
            if (GUILayout.Button("-1", GUILayout.Width(60))) ModifyConsumable("Food", -1);
            if (GUILayout.Button("+10", GUILayout.Width(60))) ModifyConsumable("Food", 10);
            if (GUILayout.Button("满", GUILayout.Width(60))) SetConsumable("Food", 999);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("水", GUILayout.Width(80));
            if (GUILayout.Button("+1", GUILayout.Width(60))) ModifyConsumable("Water", 1);
            if (GUILayout.Button("-1", GUILayout.Width(60))) ModifyConsumable("Water", -1);
            if (GUILayout.Button("+10", GUILayout.Width(60))) ModifyConsumable("Water", 10);
            if (GUILayout.Button("满", GUILayout.Width(60))) SetConsumable("Water", 999);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.Box("", GUILayout.Height(2), GUILayout.ExpandWidth(true));
            GUILayout.Space(5);

            foreach (var item in ItemNames)
            {
                string cnName = item[0];
                string enName = item[1];
                string itemType = item[2];

                GUILayout.BeginHorizontal();
                GUILayout.Label(cnName, GUILayout.Width(80));

                if (itemType == "Remedium")
                {
                    if (GUILayout.Button("启用", GUILayout.Width(60)))
                    {
                        SetRemediumState(enName, true, false);
                    }

                    if (GUILayout.Button("禁用", GUILayout.Width(60)))
                    {
                        SetRemediumState(enName, true, true);
                    }

                    if (GUILayout.Button("卸载", GUILayout.Width(60)))
                    {
                        SetRemediumState(enName, false, false);
                    }
                }
                else
                {
                    if (GUILayout.Button("启用", GUILayout.Width(60)))
                    {
                        SetItemState(enName, true, false, 100);
                    }

                    if (GUILayout.Button("禁用", GUILayout.Width(60)))
                    {
                        SetItemState(enName, true, true, 0);
                    }

                    if (GUILayout.Button("卸载", GUILayout.Width(60)))
                    {
                        SetItemState(enName, false, false, 100);
                    }
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.Space(10);
            GUILayout.Box("", GUILayout.Height(2), GUILayout.ExpandWidth(true));
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("全部启用", GUILayout.Height(35)))
            {
                EnableAll();
            }
            if (GUILayout.Button("全部卸载", GUILayout.Height(35)))
            {
                UnloadAll();
            }
            GUILayout.EndHorizontal();

            GUILayout.EndScrollView();

            GUILayout.Label(_statusText);

            if (GUILayout.Button("关闭"))
            {
                _windowOpen = false;
            }

            GUI.DragWindow();
        }

        private void ModifyConsumable(string objectName, float delta)
        {
            try
            {
                Type consumableType = FindType("RG.SecondsRemaster.Core.SecondsConsumableRemedium");
                if (consumableType == null)
                {
                    _statusText = "错误: 未找到消耗品类型";
                    return;
                }

                var items = Resources.FindObjectsOfTypeAll(consumableType);
                foreach (var item in items)
                {
                    string itemName = item ?.name ?? "";
                    if (itemName == objectName)
                    {
                        object rd = GetPropertyValue(item, "RuntimeData");
                        if (rd != null)
                        {
                            float current = GetFieldValue<float>(rd, "_amount");
                            float newVal = Mathf.Max(0, current + delta);
                            SetFieldValue(rd, "_amount", newVal);
                            SetFieldValue(rd, "_isAvailable", true);
                            _statusText = $"{objectName}: {current} -> {newVal}";
                        }
                        return;
                    }
                }
                _statusText = $"未找到: {objectName}";
            }
            catch (Exception e)
            {
                _statusText = $"错误: {e.Message}";
            }
        }

        private void SetConsumable(string objectName, float amount)
        {
            try
            {
                Type consumableType = FindType("RG.SecondsRemaster.Core.SecondsConsumableRemedium");
                if (consumableType == null)
                {
                    _statusText = "错误: 未找到消耗品类型";
                    return;
                }

                var items = Resources.FindObjectsOfTypeAll(consumableType);
                foreach (var item in items)
                {
                    string itemName = item?.name ?? "";
                    if (itemName == objectName)
                    {
                        object rd = GetPropertyValue(item, "RuntimeData");
                        if (rd != null)
                        {
                            float current = GetFieldValue<float>(rd, "_amount");
                            SetFieldValue(rd, "_amount", amount);
                            SetFieldValue(rd, "_isAvailable", true);
                            _statusText = $"{objectName}: {current} -> {amount}";
                        }
                        return;
                    }
                }
                _statusText = $"未找到: {objectName}";
            }
            catch (Exception e)
            {
                _statusText = $"错误: {e.Message}";
            }
        }

        private void SetItemState(string objectName, bool available, bool damaged, int durability)
        {
            try
            {
                Type itemType = FindType("RG.SecondsRemaster.Core.SecondsItem");
                if (itemType == null)
                {
                    _statusText = "错误: 未找到道具类型";
                    return;
                }

                var items = Resources.FindObjectsOfTypeAll(itemType);
                foreach (var item in items)
                {
                    string itemName = item?.name ?? "";
                    if (itemName == objectName)
                    {
                        object rd = GetPropertyValue(item, "RuntimeData");
                        if (rd != null)
                        {
                            SetFieldValue(rd, "_isAvailable", available);
                            SetFieldValue(rd, "_isDamaged", damaged);
                            SetFieldValue(rd, "_durability", durability);

                            string state = available ? (damaged ? "禁用" : "启用") : "卸载";
                            _statusText = $"{objectName}: {state}";
                        }
                        return;
                    }
                }
                _statusText = $"未找到: {objectName}";
            }
            catch (Exception e)
            {
                _statusText = $"错误: {e.Message}";
            }
        }

        private void SetRemediumState(string objectName, bool available, bool damaged)
        {
            try
            {
                Type remediumType = FindType("RG.SecondsRemaster.Core.SecondsRemedium");
                if (remediumType == null)
                {
                    _statusText = "错误: 未找到医疗包类型";
                    return;
                }

                var items = Resources.FindObjectsOfTypeAll(remediumType);
                foreach (var item in items)
                {
                    string itemName = item?.name ?? "";
                    if (itemName == objectName)
                    {
                        // 修改 RuntimeData._isAvailable
                        object rd = GetPropertyValue(item, "RuntimeData");
                        if (rd != null)
                        {
                            SetFieldValue(rd, "_isAvailable", available);
                        }

                        object srrd = GetPropertyValue(item, "SecondsRemediumRuntimeData");
                        if (srrd == null)
                        {
                            srrd = GetFieldValue<object>(item, "_secondsRemediumRuntimeData");
                        }
                        if (srrd != null)
                        {
                            SetFieldValue(srrd, "_isDamaged", damaged);
                        }

                        string state = available ? (damaged ? "禁用" : "启用") : "卸载";
                        _statusText = $"{objectName}: {state}";
                        return;
                    }
                }
                _statusText = $"未找到: {objectName}";
            }
            catch (Exception e)
            {
                _statusText = $"错误: {e.Message}";
            }
        }

        private void EnableAll()
        {
            int count = 0;

            Type itemType = FindType("RG.SecondsRemaster.Core.SecondsItem");
            if (itemType != null)
            {
                var items = Resources.FindObjectsOfTypeAll(itemType);
                foreach (var item in items)
                {
                    object rd = GetPropertyValue(item, "RuntimeData");
                    if (rd != null)
                    {
                        SetFieldValue(rd, "_isAvailable", true);
                        SetFieldValue(rd, "_isDamaged", false);
                        SetFieldValue(rd, "_durability", 100);
                        count++;
                    }
                }
            }

            Type remediumType = FindType("RG.SecondsRemaster.Core.SecondsRemedium");
            if (remediumType != null)
            {
                var items = Resources.FindObjectsOfTypeAll(remediumType);
                foreach (var item in items)
                {
                    object rd = GetPropertyValue(item, "RuntimeData");
                    if (rd != null)
                    {
                        SetFieldValue(rd, "_isAvailable", true);
                    }

                    object srrd = GetPropertyValue(item, "SecondsRemediumRuntimeData");
                    if (srrd == null)
                    {
                        srrd = GetFieldValue<object>(item, "_secondsRemediumRuntimeData");
                    }
                    if (srrd != null)
                    {
                        SetFieldValue(srrd, "_isDamaged", false);
                    }
                    count++;
                }
            }

            Type consumableType = FindType("RG.SecondsRemaster.Core.SecondsConsumableRemedium");
            if (consumableType != null)
            {
                var items = Resources.FindObjectsOfTypeAll(consumableType);
                foreach (var item in items)
                {
                    object rd = GetPropertyValue(item, "RuntimeData");
                    if (rd != null)
                    {
                        SetFieldValue(rd, "_amount", 999f);
                        SetFieldValue(rd, "_isAvailable", true);
                        count++;
                    }
                }
            }

            _statusText = $"全部启用: {count} 个对象";
        }

        private void UnloadAll()
        {
            int count = 0;

            Type itemType = FindType("RG.SecondsRemaster.Core.SecondsItem");
            if (itemType != null)
            {
                var items = Resources.FindObjectsOfTypeAll(itemType);
                foreach (var item in items)
                {
                    object rd = GetPropertyValue(item, "RuntimeData");
                    if (rd != null)
                    {
                        SetFieldValue(rd, "_isAvailable", false);
                        SetFieldValue(rd, "_isDamaged", false);
                        SetFieldValue(rd, "_durability", 100);
                        count++;
                    }
                }
            }

            Type remediumType = FindType("RG.SecondsRemaster.Core.SecondsRemedium");
            if (remediumType != null)
            {
                var items = Resources.FindObjectsOfTypeAll(remediumType);
                foreach (var item in items)
                {
                    object rd = GetPropertyValue(item, "RuntimeData");
                    if (rd != null)
                    {
                        SetFieldValue(rd, "_isAvailable", false);
                    }

                    object srrd = GetPropertyValue(item, "SecondsRemediumRuntimeData");
                    if (srrd == null)
                    {
                        srrd = GetFieldValue<object>(item, "_secondsRemediumRuntimeData");
                    }
                    if (srrd != null)
                    {
                        SetFieldValue(srrd, "_isDamaged", false);
                    }
                    count++;
                }
            }

            Type consumableType = FindType("RG.SecondsRemaster.Core.SecondsConsumableRemedium");
            if (consumableType != null)
            {
                var items = Resources.FindObjectsOfTypeAll(consumableType);
                foreach (var item in items)
                {
                    object rd = GetPropertyValue(item, "RuntimeData");
                    if (rd != null)
                    {
                        SetFieldValue(rd, "_amount", 0f);
                        SetFieldValue(rd, "_isAvailable", false);
                        count++;
                    }
                }
            }

            _statusText = $"全部卸载: {count} 个对象";
        }

        // ========== 辅助方法 ==========

        private Type FindType(string fullName)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType(fullName);
                if (type != null) return type;
            }
            return null;
        }

        private object GetPropertyValue(object obj, string propName)
        {
            if (obj == null) return null;

            Type type = obj.GetType();
            while (type != null)
            {
                PropertyInfo prop = type.GetProperty(propName,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (prop != null && prop.CanRead)
                {
                    try
                    {
                        return prop.GetValue(obj);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"获取属性 {prop.Name} 的值失败: {e.Message}");
                        return null;
                    }
                }

                type = type.BaseType;
            }
            return null;
        }

        private T GetFieldValue<T>(object obj, string fieldName)
        {
            if (obj == null) return default;

            Type type = obj.GetType();
            while (type != null)
            {
                FieldInfo field = type.GetField(fieldName,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (field != null)
                {
                    try
                    {
                        return (T)field.GetValue(obj);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"获取字段 {field.Name} 的值失败: {ex.Message}");
                        return default;
                    }
                }

                type = type.BaseType;
            }
            return default;
        }

        private void SetFieldValue(object obj, string fieldName, object value)
        {
            if (obj == null) return;

            Type type = obj.GetType();
            while (type != null)
            {
                FieldInfo field = type.GetField(fieldName,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (field != null)
                {
                    try
                    {
                        object newVal = ConvertValue(value, field.FieldType);
                        field.SetValue(obj, newVal);
                        return;
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"设置字段 {field.Name} 的值失败: {ex.Message}");
                        return;
                    }
                }

                type = type.BaseType;
            }
        }

        private object ConvertValue(object value, Type targetType)
        {
            if (value == null) return null;
            if (targetType.IsInstanceOfType(value)) return value;

            if (targetType == typeof(int)) return Convert.ToInt32(value);
            if (targetType == typeof(float)) return Convert.ToSingle(value);
            if (targetType == typeof(double)) return Convert.ToDouble(value);
            if (targetType == typeof(bool)) return Convert.ToBoolean(value);

            return Convert.ChangeType(value, targetType);
        }
    }
}