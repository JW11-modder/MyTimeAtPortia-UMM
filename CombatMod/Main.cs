using Harmony12;
using Pathea;
using Pathea.ActorNs;
using Pathea.ClashNs;
using Pathea.CompoundSystem;
using Pathea.ConfigNs;
using Pathea.FeatureNs;
using Pathea.GameResPointNs;
using Pathea.ItemDropNs;
using Pathea.ItemSystem;
using Pathea.Missions;
using Pathea.ModuleNs;
using Pathea.SkillNs;
using Pathea.UISystemNs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityModManagerNet;
using static Harmony12.AccessTools;
using static Pathea.AttrNs.AttrData;

namespace CombatMod
{
    static class Main
    {
        // Send a response to the mod manager about the launch status, success or not.
        public static bool enabled;
        public static Settings settings { get; private set; }

        private static readonly bool isDebug = false;

        public static void Dbgl(string str = "", bool pref = true)
        {
            if (isDebug)
                Debug.Log((pref ? "CombatMod " : "") + str);
        }

        // Send a response to the mod manager about the launch status, success or not.
        private static bool Load(UnityModManager.ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            var harmony = HarmonyInstance.Create(modEntry.Info.Id);
            /*harmony.Patch(
                original: AccessTools.Method(typeof(Player), "Update"),
                postfix: new HarmonyMethod(typeof(Main), nameof(Player_Update_Postfix))
            );*/
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            return true;
        }

        private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }

        private static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.Label(string.Format("Health Regen per second: <b>{0}x</b>", settings.HPRegenMult), new GUILayoutOption[0]);
            settings.HPRegenMult = (int)GUILayout.HorizontalSlider((float)Main.settings.HPRegenMult, 0f, 100f, new GUILayoutOption[0]);

            GUILayout.Label(string.Format("Stamina Regen per second: <b>{0}x</b>", settings.SPRegenMult), new GUILayoutOption[0]);
            settings.SPRegenMult = (int)GUILayout.HorizontalSlider((float)Main.settings.SPRegenMult, 0f, 100f, new GUILayoutOption[0]);

            //GUILayout.Label(string.Format("Endurance Regen Multiplier: <b>{0}x</b>", settings.VPRegenMult), new GUILayoutOption[0]);
            //settings.VPRegenMult = (int)GUILayout.HorizontalSlider((float)Main.settings.VPRegenMult, 1f, 100f, new GUILayoutOption[0]);
            
            GUILayout.Label(string.Format("Attack Damage Multiplier: <b>{0}x</b>", settings.DamageMult), new GUILayoutOption[0]);
            settings.DamageMult = (int)GUILayout.HorizontalSlider((float)Main.settings.DamageMult, 1f, 100f, new GUILayoutOption[0]);

            GUILayout.Space(10);
            settings.UnlimitedAmmo = GUILayout.Toggle(settings.UnlimitedAmmo, "Enable unlimited gun ammo", new GUILayoutOption[0]);
            GUILayout.Space(20);

            GUILayout.Space(10);
            settings.NoEndLoss = GUILayout.Toggle(settings.NoEndLoss, "Enable unlimited endurance (running and rolling)", new GUILayoutOption[0]);
            GUILayout.Space(20);

        }

        // Called when the mod is turned to on/off.
        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value /* active or inactive */)
        {
            enabled = value;
            return true; // Permit or not.
        }

        private static float lastRegenTick = 0;

        [HarmonyPatch(typeof(Player), "Update")]
        static class Player_Update_Patch
        {
            static void Postfix(Player __instance)
            {
                if (!enabled || __instance.actor == null)
                    return;

                if (Time.fixedTime > lastRegenTick + 1)
                {
                    __instance.actor.hp += settings.HPRegenMult;
                    __instance.actor.cp += settings.SPRegenMult;
                    lastRegenTick = Time.fixedTime;
                }
            }
        }
        
        [HarmonyPatch(typeof(Player), "OnSucceedShoot")]
        static class Player_OnSucceedShoot_Patch
        {
            static bool Prefix(Player __instance)
            {
                if (!enabled || __instance.actor == null || !settings.UnlimitedAmmo)
                {
                    return true;
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(Player), "ConsumeRollVp")]
        static class Player_ConsumeRollVp_Patch
        {
            static bool Prefix(Player __instance)
            {
                if (!enabled || __instance.actor == null || !settings.NoEndLoss)
                {
                    return true;
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(Player), "ConsumeFastRunVp")]
        static class Player_ConsumeFastRunVp_Patch
        {
            static bool Prefix(Player __instance)
            {
                if (!enabled || __instance.actor == null || !settings.NoEndLoss)
                {
                    return true;
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(Actor), nameof(Actor.HpChange))]
        static class hpChangeEvent_Patch
        {
            
            static bool Prefix(Actor __instance, Caster caster, HpChange hpChange)
            {
                if (!enabled || __instance == Module<Player>.Self.actor || hpChange.value > 0)
                {
                    Dbgl("Actor self: " + hpChange.value.ToString());
                    return true;
                }
                hpChange.value *= settings.DamageMult;
                Dbgl("Actor NOT self: " + hpChange.value.ToString());
                __instance.hp += hpChange.value;

                return true;

            }
        }



    }
}
