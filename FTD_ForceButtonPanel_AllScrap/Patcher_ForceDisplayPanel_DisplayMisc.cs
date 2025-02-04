using BrilliantSkies.Core.Logger;
using BrilliantSkies.FromTheDepths.Planets.Map;
using BrilliantSkies.Ftd.Planets.Instances.Factions.Fleets.Forces;
using BrilliantSkies.Ui.Elements;
using BrilliantSkies.Ui.Special.PopUps;
using BrilliantSkies.Ui.Tips;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FTD_ForceButtonPanel_AllScrap
{
    [HarmonyLib.HarmonyPatch(typeof(ForceButtonPanel), "DisplayMisc")]
    class Patcher_ForceDisplayPanel_DisplayMisc
    {
        private static UiDef FORCE_KILL_MULTIPLE = new UiDef("killdesignmultiple", new Guid("fea38e46-8d20-46bf-bd6b-e0715c9d3f94")
            , new ToolTip("Scrap <color=red>these forces</color> and return 100% of all materials. This will destroy them completely", 200f));
        private static UiDef FORCE_RECYCLE_MULTIPLE = new UiDef("recycledesignmultiple", new Guid("5001aba7-3615-49e6-9e18-e45bc0313a03")
    , new ToolTip("Scrap <color=red>these forces</color> and return 100% of all materials- this will turn forces into dead blueprints, not completely destroy them", 200f));
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
                DisplayButtonNew(Applicability.All, Applicability.None, FORCE_RECYCLE_MULTIPLE,
                       delegate (List<Force> fp)
                       {
                           string question = string.Format("Are you sure you want to scrap {0} and return them to dead blueprints?", fp.Count);
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
                                               ForceScrappingSync.Instance.ScrapInPlayAndMakeDeadBlueprint(force, force.FactionId);
                                           }
                                           else if (force.State == enumForceState.OutOfPlay)
                                           {
                                               ForceScrappingSync.Instance._ScrapOutOfPlayAndMakeDeadBlueprint(force, force.FactionId);
                                           }
                                       }
                                   }
                               }
                           }));
                       },
                       delegate (Force fp)
                       {

                       }, __instance);
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
                if (def.DisplayButton(null).IsClick())
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
