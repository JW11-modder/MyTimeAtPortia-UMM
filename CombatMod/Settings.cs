using UnityModManagerNet;

namespace CombatMod
{
    public class Settings : UnityModManager.ModSettings
    {
        public int HPRegenMult { get; set; } = 1;
        public int SPRegenMult { get; set; } = 1;
        //public int VPRegenMult { get; set; } = 1;
        public int DamageMult { get; set; } = 1;
        public bool UnlimitedAmmo { get; set; } = false;
        public bool NoEndLoss { get; set; } = false;

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }
    }
}
