﻿using UnityEditor;
using UnityEngine;

namespace FrostweepGames.Plugins.GoogleCloud.TextToSpeech
{
    [InitializeOnLoad]
    public class WelcomeDialog : EditorWindow
    {
        private static bool _Inited;

        static WelcomeDialog()
        {
            EditorApplication.update += Startup;
        }

        private static void Startup()
        {
            EditorApplication.update -= Startup;

            if (GeneralConfig.Config.showWelcomeDialogAtStartup)
            {
                Init();
            }
        }

        [MenuItem("Window/Frostweep Games/Google Cloud/Text To Speech")]
        private static void Init()
        {
            if (_Inited)
                return;

            WelcomeDialog window = (WelcomeDialog)GetWindow(typeof(WelcomeDialog), false, "Text To Speech", true);
            window.minSize = new Vector2(500, 400);
            window.maxSize = new Vector2(500, 400);
            window.Show();

            _Inited = true;
        }

		private void OnDestroy()
		{
            _Inited = false;
        }

		private void OnGUI()
        {
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Welcome to Frostweep Games - Google Cloud Text To Speech!", EditorStyles.boldLabel);

            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();

            if (GUILayout.Button("Asset Store Page"))
            {
                Application.OpenURL("https://assetstore.unity.com/publishers/14839");
            }
            if (GUILayout.Button("Frostweep Games Website"))
            {
                Application.OpenURL("https://frostweepgames.com");
            }
            if (GUILayout.Button("Frostweep Games Store Page"))
            {
                Application.OpenURL("https://store.frostweepgames.com");
            }
            if (GUILayout.Button("Official Discord Server"))
            {
                Application.OpenURL("https://discord.gg/TZdhnWy");
            }
            if (GUILayout.Button("Contact Us"))
            {
                Application.OpenURL("mailto: assets@frostweepgames.com");
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Tools");

            if (GUILayout.Button("Locate Google Cloud Text To Speech Settings"))
            {
                Selection.objects = new UnityEngine.Object[] { GeneralConfig.Config };
                EditorGUIUtility.PingObject(GeneralConfig.Config);
            }
            if (GUILayout.Button("Open Documentation"))
            {
                System.Diagnostics.Process.Start(Application.dataPath + "/FrostweepGames/GCTextToSpeech/Documentation.pdf");
            }
            if (GUILayout.Button("Open README"))
            {
                System.Diagnostics.Process.Start(Application.dataPath + "/FrostweepGames/GCTextToSpeech/README.txt");
            }

            EditorGUILayout.Space();
            bool showOnStartup = GUILayout.Toggle(GeneralConfig.Config.showWelcomeDialogAtStartup, "Show on startup");

            if (showOnStartup != GeneralConfig.Config.showWelcomeDialogAtStartup)
            {
                GeneralConfig.Config.showWelcomeDialogAtStartup = showOnStartup;
                EditorUtility.SetDirty(GeneralConfig.Config);
            }
        }
    }
}