using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        public static CopiedLobbyMess Instance;
        public string Difficulty;
        public string CameraMode;

        protected override void Load()
        {
            Instance = this;

            string Difficulty = Configuration.Instance.Difficulty.ToUpperInvariant();
            switch(Difficulty)
            {
                case "NRM":
                case "NORMAL":
                    Instance.Difficulty = "NRM";
                    break;
                case "HARD":
                case "HRD":
                    Instance.Difficulty = "HRD";
                    break;
                case "EZY":
                case "EASY":
                    Instance.Difficulty = "EZY";
                    break;
                default:
                    if (Configuration.Instance.Logging)
                        Logger.LogError($"Difficulty: {Difficulty} not recognized. Defaulting to NRM (Normal)");
                    Instance.Difficulty = "NRM";
                    break;
            }

            string CameraMode = Configuration.Instance.CameraMode.ToUpperInvariant();
            switch(CameraMode)
            {
                case "FIRST":
                case "1PP":
                    Instance.CameraMode = "1Pp";
                    break;
                case "BOTH":
                case "2PP":
                    Instance.CameraMode = "2Pp";
                    break;
                case "THIRD":
                case "3PP":
                    Instance.CameraMode = "3Pp";
                    break;
                case "VEHICLE":
                case "4PP":
                    Instance.CameraMode = "4Pp";
                    break;
                default:
                    if (Configuration.Instance.Logging)
                        Logger.LogError($"Camera Mode; {CameraMode} not recognized. Defaulting to Both (2Pp)");
                    Instance.CameraMode = "2Pp";
                    break;
            }

            if (Level.isLoaded)
                CreateThread();

            Level.onPostLevelLoaded += OnPostLevelLoaded;
        }

        protected override void Unload() => Level.onPostLevelLoaded -= OnPostLevelLoaded;

        public void OnPostLevelLoaded(int xd) => CreateThread();

        #region Helpers

        public static int GetWorkshopCount() =>
            (String.Join(",", Provider.getServerWorkshopFileIDs().Select(x => x.ToString()).ToArray()).Length - 1) / 120 + 1;

        public static int GetConfigurationCount() =>
            (String.Join(",", typeof(ModeConfigData).GetFields()
            .SelectMany(x => x.FieldType.GetFields().Select(y => y.GetValue(x.GetValue(Provider.modeConfigData))))
            .Select(x => x is bool v ? v ? "T" : "F" : (String.Empty + x)).ToArray()).Length - 1) / 120 + 1;

        private void CreateThread()
        {
            Thread thread = new Thread(ModifyLobbyInfo);
            thread.Start();
        }

        #endregion

        private void ModifyLobbyInfo()
        {
            Thread.Sleep(1000);

            bool workshop = ModifyWorkshop();

            if (Configuration.Instance.MessConfig)
                ModifyConfig(workshop);

            if (Configuration.Instance.HideConfig)
                SteamGameServer.SetKeyValue("Browser_Config_Count", "0");
            else
                SteamGameServer.SetKeyValue("Browser_Config_Count", GetConfigurationCount().ToString());

            ModifyPlugins();
        }

        private bool ModifyWorkshop()
        {
            bool workshop = Provider.getServerWorkshopFileIDs().Count > 0;

            if (Configuration.Instance.HideWorkshop)
            {
                workshop = false;
                SteamGameServer.SetKeyValue("Browser_Workshop_Count", "0");
                Logger.Log($"Workshop Count: {GetWorkshopCount()}");
            }
            else if (Configuration.Instance.MessWorkshop)
            {
                workshop = true;
                string txt = String.Join(",", Configuration.Instance.Workshop);
                SteamGameServer.SetKeyValue("Browser_Workshop_Count", ((txt.Length - 1) / 120 + 1).ToString());

                int line = 0;
                for (int i = 0; i < txt.Length; i += 120)
                {
                    int num6 = 120;

                    if (i + num6 > txt.Length)
                        num6 = txt.Length - i;

                    string pValue2 = txt.Substring(i, num6);
                    SteamGameServer.SetKeyValue("Browser_Workshop_Line_" + line, pValue2);
                    line++;
                }
            }
            else
            {
                SteamGameServer.SetKeyValue("Browser_Workshop_Count", GetWorkshopCount().ToString());
            }

            return workshop;
        }

        private void ModifyConfig(bool workshop)
        {
            string tags = "";
            tags += String.Concat(new string[]
            {
                    Configuration.Instance.IsPVP ? "PVP" : "PVE",
                    ",<gm>",
                    Configuration.Instance.MessGamemode ? Configuration.Instance.Gamemode : Provider.gameMode.GetType().Name,
                    "</gm>,",
                    Configuration.Instance.HasCheats ? "CHy" : "CHn",
                    ",",
                    Instance.Difficulty,
                    ",",
                    Instance.CameraMode,
                    ",",
                    workshop ? "WSy" : "WSn",
                    ",",
                    Configuration.Instance.GoldOnly ? "GLD" : "F2P",
                    ",",
                    Configuration.Instance.HasBattleye ? "BEy" : "BEn"
            });

            if (!String.IsNullOrEmpty(Provider.configData.Browser.Thumbnail))
                tags += ",<tn>" + Provider.configData.Browser.Thumbnail + "</tn>";


            SteamGameServer.SetGameTags(tags);
        }

        private void ModifyPlugins()
        {
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

            if (Configuration.Instance.IsVanilla)
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
                string version = ModuleHook.modules.Find(a => a.config.Name == "Rocket.Unturned")?.config.Version ?? "0.0.0.69";
                SteamGameServer.SetKeyValue("rocket", version);
            }
        }
    }
}
