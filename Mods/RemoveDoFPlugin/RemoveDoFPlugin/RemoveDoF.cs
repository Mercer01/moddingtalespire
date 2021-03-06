﻿using System;
using System.Collections;
using System.Reflection;
using BepInEx;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

namespace RemoveDoFPlugin
{
    [BepInPlugin("org.d20armyknife.plugins.removedof", "Remove Depth Of Field Plug-In", "1.2.0.0")]
    [BepInProcess("TaleSpire.exe")]
    public class DeDoFIt: BaseUnityPlugin
    {

        // Awake is called once when both the game and the plug-in are loaded
        void Awake()
        {
            UnityEngine.Debug.Log("Remove Depth of Field Plug-in loaded");
            // Set the UsingCodeInjection so we don't anger the @Baggers
            AppStateManager.UsingCodeInjection = true;
        }
        
        private void ToggleDoF()
        {
            this.dofEnabled = !this.dofEnabled;

            var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;
            try
            {
                if (SingletonBehaviour<AtmosphereManager>.HasInstance)
                {
                    var am = SingletonBehaviour<AtmosphereManager>.Instance;
                    var aa = (AtmosphereApplier)am.GetType().GetField("_applier", flags).GetValue(am);
                    var dof = (DepthOfField)aa.GetType().GetField("_depthOfField", flags).GetValue(aa);
                    dof.enabled.value = this.dofEnabled;
                }
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.Log("Crash in Depth of Field Plugin");
                UnityEngine.Debug.Log(ex.Message);
                UnityEngine.Debug.Log(ex.StackTrace);
                UnityEngine.Debug.Log(ex.InnerException);
                UnityEngine.Debug.Log(ex.Source);
            }
        }
        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            UnityEngine.Debug.Log("Loading Scene: " + scene.name);
            TextMeshProUGUI[] texts = FindObjectsOfType<TextMeshProUGUI>();
            for (int i = 0; i < texts.Length; i++)
            {
                if (scene.name == "UI" && texts[i].name == "BETA")
                {
                    texts[i].text = "INJECTED BUILD - unstable mods";
                }
                if (scene.name == "Login" && texts[i].name == "TextMeshPro Text")
                {
                    BepInPlugin bepInPlugin = (BepInPlugin)Attribute.GetCustomAttribute(this.GetType(), typeof(BepInPlugin));
                    if (texts[i].text.EndsWith("</size>"))
                    {
                        texts[i].text += "\n\nMods Currently Installed:\n";
                    }
                    texts[i].text += "\n" + bepInPlugin.Name + " - " + bepInPlugin.Version;
                }
            }
        }
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.H))
            {
                ToggleDoF();
            }
        }

        private bool dofEnabled = true;
    }
}
