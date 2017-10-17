using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.Core;
using Rocket.Core.Plugins;
using Rocket.Core.Logging;
using SDG.Unturned;
using SDG.Framework.Modules;
using Steamworks;

namespace CopiedLobbyMess
{
    public class CopiedLobbyMess : RocketPlugin<Configuration>
    {
        #region Properties
        public static CopiedLobbyMess Instance { get; private set; }
        #endregion

        protected override void Load()
        {
            R.Plugins.OnPluginsLoaded += OnLoad;
        }

        protected override void Unload()
        {
            R.Plugins.OnPluginsLoaded -= OnLoad;
        }

        #region Functions
        public int GetWorkshopCount()
        {
            string text = string.Empty;
            for (int l = 0; l < Provider.serverWorkshopFileIDs.Count; l++)
            {
                if (text.Length > 0)
                {
                    text += ',';
                }
                text += Provider.serverWorkshopFileIDs[l];
            }
            return (text.Length - 1) / 120 + 1;
        }

        public int GetConfigurationCount()
        {
            string text2 = string.Empty;
            Type type = Provider.modeConfigData.GetType();
            FieldInfo[] fields = type.GetFields();
            for (int n = 0; n < fields.Length; n++)
            {
                FieldInfo fieldInfo = fields[n];
                object value = fieldInfo.GetValue(Provider.modeConfigData);
                Type type2 = value.GetType();
                FieldInfo[] fields2 = type2.GetFields();
                for (int num7 = 0; num7 < fields2.Length; num7++)
                {
                    FieldInfo fieldInfo2 = fields2[num7];
                    object value2 = fieldInfo2.GetValue(value);
                    if (text2.Length > 0)
                    {
                        text2 += ',';
                    }
                    if (value2 is bool)
                    {
                        text2 += ((!(bool)value2) ? "F" : "T");
                    }
                    else
                    {
                        text2 += value2;
                    }
                }
            }
            return (text2.Length - 1) / 120 + 1;
        }

        public void ModifyGameTags()
        {
            string tags = "";

            #region Workshop
            if (Configuration.Instance.HideWorkshop)
            {
                tags += "KROW"; // No workshop
                SteamGameServer.SetKeyValue("Browser_Workshop_Count", "0");
            }
            else if (Configuration.Instance.MessWorkshop)
            {
                tags += "WORK"; // Workshop

                string txt = "";
                foreach(string a in Configuration.Instance.Workshop)
                {
                    if (txt.Length > 0)
                        txt += ",";
                    txt += a;
                }
                SteamGameServer.SetKeyValue("Browser_Workshop_Count", ((txt.Length - 1) / 120 + 1).ToString());

                int num5 = 0;
                for (int m = 0; m < txt.Length; m += 120)
                {
                    int num6 = 120;
                    if (m + num6 > txt.Length)
                    {
                        num6 = txt.Length - m;
                    }
                    string pValue2 = txt.Substring(m, num6);
                    SteamGameServer.SetKeyValue("Browser_Workshop_Line_" + num5, pValue2);
                    num5++;
                }
            }
            else
            {
                if(Provider.serverWorkshopFileIDs.Count <= 0)
                {
                    tags += "KROW";
                }
                else
                {
                    tags += "WORK"; // Workshop
                    SteamGameServer.SetKeyValue("Browser_Workshop_Count", GetWorkshopCount().ToString());
                }
            }
            #endregion

            #region Gamemode
            if (Configuration.Instance.MessGamemode)
                tags += ",GAMEMODE:" + Configuration.Instance.Gamemode;
            else if (Provider.gameMode != null)
                tags += ",GAMEMODE:" + Provider.gameMode.GetType().Name;
            #endregion

            #region Config
            if (Configuration.Instance.MessConfig)
            {
                tags += ",";
                if (Configuration.Instance.IsPVP)
                    tags += "PVP";
                else
                    tags += "PVE";
                tags += ",";
                if (Configuration.Instance.HasCheats)
                    tags += "CHEATS";
                else
                    tags += "STAEHC";
                tags += ",";
                tags += Configuration.Instance.Difficulty;
                tags += ",";
                tags += Configuration.Instance.CameraMode;
                tags += ",";
                if (Configuration.Instance.GoldOnly)
                    tags += "GOLDONLY";
                else
                    tags += "YLNODLOG";
                tags += ",";
                if (Configuration.Instance.HasBattleye)
                    tags += "BATTLEYE_ON";
                else
                    tags += "BATTLEYE_OFF";
            }
            else
            {
                tags += ",";
                tags += (!Provider.isPvP) ? "PVE" : "PVP";
                tags += ",";
                tags += (!Provider.hasCheats) ? "STAEHC" : "CHEATS";
                tags += ",";
                tags += Provider.mode.ToString();
                tags += ",";
                tags += Provider.cameraMode.ToString();
                tags += ",";
                tags += (!Provider.isGold) ? "YLNODLOG" : "GOLDONLY";
                tags += ",";
                tags += (!Provider.configData.Server.BattlEye_Secure) ? "BATTLEYE_OFF" : "BATTLEYE_ON";
            }
            #endregion

            #region Configuration
            if (Configuration.Instance.HideConfig)
                SteamGameServer.SetKeyValue("Browser_Config_Count", "0");
            else
                SteamGameServer.SetKeyValue("Browser_Config_Count", GetConfigurationCount().ToString());
            #endregion

            SteamGameServer.SetGameTags(tags);

            #region Plugins
            if (Configuration.Instance.InvisibleRocket)
                SteamGameServer.SetBotPlayerCount(0); // Bypasses unturned's filter for rocket <3

            if (!Configuration.Instance.HidePlugins)
            {
                if (Configuration.Instance.MessPlugins)
                    SteamGameServer.SetKeyValue("rocketplugins", string.Join(",", Configuration.Instance.Plugins));
                else
                    SteamGameServer.SetKeyValue("rocketplugins", string.Join(",", R.Plugins.GetPlugins().Select(p => p.Name).ToArray()));
            }
            else
            {
                SteamGameServer.SetKeyValue("rocketplugins", "");
            }

            if (Configuration.Instance.IsVannila)
            {
                SteamGameServer.SetBotPlayerCount(0);
                SteamGameServer.SetKeyValue("rocketplugins", "");
                SteamGameServer.SetKeyValue("rocket", "");
            }
            else
            {
                if (!Configuration.Instance.InvisibleRocket)
                    SteamGameServer.SetBotPlayerCount(1);
                if (!Configuration.Instance.HidePlugins && !Configuration.Instance.MessPlugins)
                    SteamGameServer.SetKeyValue("rocketplugins", string.Join(",", R.Plugins.GetPlugins().Select(p => p.Name).ToArray()));
                SteamGameServer.SetKeyValue("rocket", ModuleHook.modules.FirstOrDefault(a => a.config.Name == "Rocket.Unturned").config.Version);
            }
            #endregion
        }
        #endregion

        #region Event Functions
        void OnLoad()
        {
            ModifyGameTags();
        }
        #endregion
    }
}
