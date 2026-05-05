using Harmony12;
using Pathea;
using Pathea.GameResPointNs;
using Pathea.ItemDropNs;
using Pathea.ItemSystem;
using Pathea.ModuleNs;
using Pathea.UISystemNs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;
using static Harmony12.AccessTools;

namespace LootMult
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
                Debug.Log((pref ? "LootMultiplier " : "") + str);
        }

        // Send a response to the mod manager about the launch status, success or not.
        private static bool Load(UnityModManager.ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            var harmony = HarmonyInstance.Create(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            return true;
        }

        private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }

        private static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.Label(string.Format("Ore Loot Multiplier: <b>{0}x</b>", settings.OreMult), new GUILayoutOption[0]);
            settings.OreMult = (int)GUILayout.HorizontalSlider((float)Main.settings.OreMult, 1f, 100f, new GUILayoutOption[0]);
            settings.MultData[0] = settings.OreMult;
            GUILayout.Label(string.Format("Wood Loot Multiplier: <b>{0}x</b>", settings.WoodMult), new GUILayoutOption[0]);
            settings.WoodMult = (int)GUILayout.HorizontalSlider((float)Main.settings.WoodMult, 1f, 100f, new GUILayoutOption[0]);
            settings.MultData[1] = settings.WoodMult;
            GUILayout.Label(string.Format("Gems Loot Multiplier: <b>{0}x</b>", settings.GemsMult), new GUILayoutOption[0]);
            settings.GemsMult = (int)GUILayout.HorizontalSlider((float)Main.settings.GemsMult, 1f, 100f, new GUILayoutOption[0]);
            settings.MultData[2] = settings.GemsMult;
            GUILayout.Label(string.Format("Stone Loot Multiplier: <b>{0}x</b>", settings.StoneMult), new GUILayoutOption[0]);
            settings.StoneMult = (int)GUILayout.HorizontalSlider((float)Main.settings.StoneMult, 1f, 100f, new GUILayoutOption[0]);
            settings.MultData[3] = settings.StoneMult;
            GUILayout.Label(string.Format("Monster Drop Loot Multiplier: <b>{0}x</b>", settings.MonsterDropMult), new GUILayoutOption[0]);
            settings.MonsterDropMult = (int)GUILayout.HorizontalSlider((float)Main.settings.MonsterDropMult, 1f, 100f, new GUILayoutOption[0]);
            settings.MultData[4] = settings.MonsterDropMult;
            GUILayout.Label(string.Format("Tree Drop Loot Multiplier: <b>{0}x</b>", settings.TreeDropMult), new GUILayoutOption[0]);
            settings.TreeDropMult = (int)GUILayout.HorizontalSlider((float)Main.settings.TreeDropMult, 1f, 100f, new GUILayoutOption[0]);
            settings.MultData[5] = settings.TreeDropMult;
            GUILayout.Label(string.Format("Tech Loot Multiplier: <b>{0}x</b>", settings.TechMult), new GUILayoutOption[0]);
            settings.TechMult = (int)GUILayout.HorizontalSlider((float)Main.settings.TechMult, 1f, 100f, new GUILayoutOption[0]);
            settings.MultData[6] = settings.TechMult;
        }

        // Called when the mod is turned to on/off.
        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value /* active or inactive */)
        {
            enabled = value;
            return true; // Permit or not.
        }

        [HarmonyPatch(typeof(PlayerAutoPickTarget), "Start")]
        static class PlayerAutoPickTarget_Start_Patch
        {
            static bool Prefix(PlayerAutoPickTarget __instance)
            {
                if (!enabled)
                {
                    return true;
                }

                if (Module<Player>.Self == null || Module<Player>.Self.actor == null)
                {
                    return true;
                }

                ItemDrop component = __instance.GetComponent<ItemDrop>();
                ItemPickFollow follow = __instance.GetComponent<ItemPickFollow>();

                Dbgl("1");
                if (component != null && follow != null)
                {
                    Dbgl("2");
                    if (follow.CheckCanAddBag(component))
                    {
                        Dbgl("3");
                        for (int i = 0; i < 7; i++)
                        {
                            if (LootIdData[i].Contains(component.ItemID))
                            {
                                Module<Player>.Self.bag.AddItem(component.ItemID, settings.MultData[i] - 1, true, AddItemMode.ForceItemBar);
                                break;
                            }
                        }
                    }


                }
                return true;

            }
        }

        static int[][] LootIdData = new int[7][]{ 
        //ore
        new int[] { 4000013, 4000035, 4000118, 4000119, 4000139, 4000162, 4000283, 4000284 },
        //wood
        new int[] { 4000001, 4000077, 4000085, 4000138, 4000279, 4000314 },
        //gems
        new int[] { 4000024, 4000063, 4000079, 4000111, 4000147, 4000148, 4000171, 4000229 },
        //stone
        new int[] { 4000004, 4000005, 4000008, 4000038, 4000045, 4000120, 4000121, 4000297, 4000299 },
        //monster drop
        new int[] { 4000002, 4000006, 4000007, 4000009, 4000011, 4000016, 4000021, 4000022, 4000027, 4000037, 4000051, 4000059, 4000062, 4000073, 4000080, 4000081, 4000094, 4000095, 4000096, 4000098, 4000103, 4000113, 4000133, 4000143, 4000153, 4000161, 4000163, 4000165, 4000176, 4000231, 4000293, 4000302, 4000306, 4000307, 4000315 },
        //tree drop
        new int[] { 4000012, 4000014, 4000037, 4000042, 4000046, 4000047, 4000049, 4000050, 4000054, 4000073, 4000076, 4000078, 4000084, 4000089, 4000090, 4000109, 4000112, 4000124, 4000138, 4000152, 4000166, 4000175, 4000282, 4000303, 4000310, 4000312, 4000319 },
        //tech
        new int[] { 2060001, 4000030, 4000033, 4000040, 4000072, 4000075, 4000083, 4000100, 4000101, 4000106, 4000108, 4000115, 4000132, 4000141, 4000203, 4000274, 4000285, 4000309, 4000311, 4000318, 4000343 }
    };

    }
}
