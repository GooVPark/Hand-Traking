#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

#pragma warning disable CS0618 // Obsolete

// ?�짜 : 2021-03-08 AM 1:12:05
// ?�성??: Rito

namespace Rito
{
    /// <summary> 주기?�으�??�재 ???�동 ?�??</summary>
    [InitializeOnLoad]
    public static class SceneAutoSaver
    {
        private const string Prefix = "SAS_";

        public static bool Activated { get; set; }
        public static bool ShowLog { get; set; }
        public static double SaveCycle
        {
            get => _saveCycle;
            set
            {
                if(value < 5f) value = 5f;
                _saveCycle = value;
            }
        }
        public static DateTime LastSavedTimeForLog { get; private set; } // 최근 ?�???�간(보여주기??
        public static double NextSaveRemaining { get; private set; }

        private static double _saveCycle = 10f;
        private static DateTime _lastSavedTime; // 최근 ?�???�간
        
        // ?�적 ?�성??: ?�디??Update ?�벤?�에 ?�들???�록
        static SceneAutoSaver()
        {
            var handlers = EditorApplication.update.GetInvocationList();

            bool hasAlready = false;
            foreach (var handler in handlers)
            {
                if(handler.Method.Name == nameof(UpdateAutoSave))
                    hasAlready = true;
            }

            if(!hasAlready)
                EditorApplication.update += UpdateAutoSave;

            _lastSavedTime = LastSavedTimeForLog = DateTime.Now;

            LoadOptions();
        }

        public static void SaveOptions()
        {
            EditorPrefs.SetBool(Prefix + nameof(Activated), Activated);
            EditorPrefs.SetBool(Prefix + nameof(ShowLog), ShowLog);
            EditorPrefs.SetFloat(Prefix + nameof(SaveCycle), (float)SaveCycle);
        }

        private static void LoadOptions()
        {
            Activated = EditorPrefs.GetBool(Prefix + nameof(Activated), true);
            ShowLog   = EditorPrefs.GetBool(Prefix + nameof(ShowLog), true);
            SaveCycle = EditorPrefs.GetFloat(Prefix + nameof(SaveCycle), 10f);

            // ?�수???�자�?�?
            SaveCycle = Math.Floor(SaveCycle * 100.0) * 0.01;
        }
        
        // ?�간??체크?�여 ?�동 ?�??
        private static void UpdateAutoSave()
        {
            DateTime dtNow = DateTime.Now;

            if (!Activated || EditorApplication.isPlaying || !EditorApplication.isSceneDirty)
            {
                _lastSavedTime = dtNow;
                NextSaveRemaining = _saveCycle;
                return;
            }

            // ?�간 계산
            double diff = dtNow.Subtract(_lastSavedTime).TotalSeconds;

            NextSaveRemaining = SaveCycle - diff;
            if(NextSaveRemaining < 0f) NextSaveRemaining = 0f;

            // ?�해�??�간 경과 ???�??�?최근 ?�???�간 갱신
            if (diff > SaveCycle)
            {
                //if(EditorApplication.isSceneDirty)
                EditorSceneManager.SaveOpenScenes();
                _lastSavedTime = LastSavedTimeForLog = dtNow;

                if (ShowLog)
                {
                    string dateStr = dtNow.ToString("yyyy-MM-dd  hh:mm:ss");
                    UnityEngine.Debug.Log($"[Auto Save] {dateStr}");
                }
            }
        }
    }
}

#endif