using UnityModManagerNet;

namespace LootMult
{
    public class Settings : UnityModManager.ModSettings
    {
        public int OreMult { get; set; } = 1;
        public int WoodMult { get; set; } = 1;
        public int GemsMult { get; set; } = 1;
        public int StoneMult { get; set; } = 1;
        public int MonsterDropMult { get; set; } = 1;
        public int TreeDropMult { get; set; } = 1;
        public int TechMult { get; set; } = 1;

        public int[] MultData = new int[7];

        public bool InstantLoot { get; set; } = false;
        public int AutolootDistanceMult { get; set; } = 1;

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }
    }
}
