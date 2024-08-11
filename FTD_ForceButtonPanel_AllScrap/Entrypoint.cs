using BrilliantSkies.Modding;
using HarmonyLib;
using System;

namespace FTD_ForceButtonPanel_AllScrap
{
    public class Entrypoint : GamePlugin_PostLoad
    {
        public string name => "Punyo_ForceButtonPanel_AllScrap";

        public Version version => null;

        public bool AfterAllPluginsLoaded()
        {
            return true;
        }

        public void OnLoad()
        {
            Harmony harmony = new Harmony("FTD_ForceButtonPanel_AllScrap");
            harmony.PatchAll();
        }

        public void OnSave()
        {

        }
    }
}
