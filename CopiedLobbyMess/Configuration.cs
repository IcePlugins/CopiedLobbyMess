using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.API;

namespace CopiedLobbyMess
{
    public class Configuration : IRocketPluginConfiguration
    {
        public bool InvisibleRocket;
        public bool HidePlugins;
        public bool MessPlugins;
        public string[] Plugins;

        public bool HideWorkshop;
        public bool MessWorkshop;
        public string[] Workshop;

        public bool MessGamemode;
        public string Gamemode;

        public bool HideConfig;
        public bool MessConfig;
        public bool IsPVP;
        public bool HasCheats;
        public string Difficulty;
        public string CameraMode;
        public bool GoldOnly;
        public bool HasBattleye;
        public bool IsVanilla;

        public void LoadDefaults()
        {
            InvisibleRocket = false;
            HidePlugins = false;
            MessPlugins = false;
            Plugins = new string[]
            {
                "Plugins removed by",
                "CopiedLobbyMess",
                "AtiLion is gay"
            };

            HideWorkshop = true;
            MessWorkshop = false;
            Workshop = new string[]
            {
                "936182532",
                "1112690830"
            };

            MessGamemode = false;
            Gamemode = "Nothing you have ever seen before";

            HideConfig = true;
            MessConfig = false;
            IsPVP = true;
            HasCheats = false;
            Difficulty = "NRM";
            CameraMode = "2p";
            GoldOnly = false;
            HasBattleye = true;
            IsVanilla = false;
        }
    }
}
