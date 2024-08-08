using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrilliantSkies.FromTheDepths.Planets.Map;
using BrilliantSkies.Ftd.LearningMaterial;
using UnityEngine;
using Ftd;
using BrilliantSkies.Ftd.Planets.Instances.Factions.Fleets.Forces;
using BrilliantSkies.Ui.Elements;
using BrilliantSkies.Ui.Tips;
using HarmonyLib;
using System.Reflection;
using System.Runtime.CompilerServices;
using BrilliantSkies.Ui.Special.PopUps;
using BrilliantSkies.Ftd;
using BrilliantSkies.Core.Logger;

namespace FTD_ForceButtonPanel_AllScrap
{
    [HarmonyLib.HarmonyPatch(typeof(ForceButtonPanel), "DisplayMisc")]
    class Patcher_ForceDisplayPanel_DisplayMisc
    {
        private static UiDef FORCE_KILL_MULTIPLE = new UiDef("killdesignmultiple", new Guid("fea38e46-8d20-46bf-bd6b-e0715c9d3f94")
            , new ToolTip("Scrap <color=red>these forces</color> and return 100% of all materials. This will destroy them completely", 200f));
        private static Type Applicability_type;
        public static void Postfix(ForceButtonPanel __instance)
        {
            if (__instance.ForceCount > 1)
            {
                __instance._forces.ForEach((f) =>
                {
                    if (f.ControlLimitations.HasAFlagNoGarbage(ControlLimitations.ResourceIsSeparate))
                    {
                        return;
                    }
                });
                GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
                DisplayButtonNew(Applicability.All, Applicability.None, FORCE_KILL_MULTIPLE,
                        delegate (List<Force> fp)
                        {
                            string question = string.Format("Are you sure you want to scrap these {0} forces and destory them completely?", fp.Count);
                            GuiPopUp.Instance.Add(new PopupConfirmation("", question, (bool b) =>
                            {
                                if (b)
                                {
                                    foreach (var force in fp)
                                    {
                                        if (fp != null)
                                        {
                                            if (force.State == enumForceState.InPlay)
                                            {
                                                ForceScrappingSync.Instance.ScrapInPlayAndCompletelyDestroy(force);
                                            }
                                            else if (force.State == enumForceState.OutOfPlay)
                                            {
                                                ForceScrappingSync.Instance.ScrapOutOfPlayAndCompletelyDestroy(force);
                                            }
                                        }
                                    }
                                }
                            }));
                        },
                        delegate (Force fp)
                        {

                        }, __instance);


                GUILayout.EndHorizontal();
            }
        }

        private static void DisplayButtonNew(Applicability available, Applicability currentState, UiDef def, Action<List<Force>> fnAll, Action<Force> fnSingle, ForceButtonPanel instance)
        {
           // AdvLogger.Log(LogPriority.Info, def._tooltipId, LogOptions.None);
            if (available != Applicability.None)
            {
                if (currentState == Applicability.All)
                {
                    def.ForegroundTint = Color.green;
                }
                else if (currentState == Applicability.Some)
                {
                    def.ForegroundTint = Color.cyan;
                }
                else
                {
                    def.ForegroundTint = Color.white;
                }
                def.Width = 0.8f;
                def.Height = 0.8f;
                def.HeightOverride = -1;
                def.WidthOverride = -1;
                if (available == Applicability.Some)
                {
                    def.Height *= 0.75f;
                }
                if (def.DisplayButton(null) > InputType.None)
                {
                    if (fnAll != null)
                    {
                        fnAll(instance._forces);
                    }
                    if (fnSingle != null)
                    {
                        foreach (Force f in instance._forces)
                        {
                            fnSingle(f);
                        }
                    }
                }
                def.Width = 1f;
                def.Height = 1f;
                def.ForegroundTint = Color.white;
            }

        }

        private enum Applicability
        {
            All,
            None,
            Some
        }
    }
}
