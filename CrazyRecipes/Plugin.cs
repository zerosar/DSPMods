using BepInEx;
using BepInEx.Configuration;
using System;
using xiaoye97;

namespace CrazyRecipes
{
    [BepInDependency(LDBToolPlugin.MODGUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInProcess("DSPGAME.exe")]
    public class Plugin : BaseUnityPlugin
    {
        public const string NAME = "CrazyRecipes";
        public const string GUID = "com.MeredithRodneyMackay." + NAME;
        public const string VERSION = "1.0.0";

        private static ConfigEntry<int> multiplier;

        private void Start()
        {
            multiplier = Config.Bind<int>(
                "General",
                "Multiplier",
                1,
                new ConfigDescription(
                    "Ramps up item production across all recipes with a multiplier",
                    new AcceptableValueRange<int>(1, 100),
                    Array.Empty<object>()
                    )
                );

            LDBTool.PostAddDataAction = (Action)Delegate.Combine(LDBTool.PostAddDataAction, new Action(IncreaseOutputForAllRecipes));
        }

        private void IncreaseOutputForAllRecipes()
        {
            for (int i = 0; i < LDB.recipes.dataArray.Length; i++)
            {
                for (int j = 0; j < LDB.recipes.dataArray[i].ResultCounts.Length; j++)
                {
                    LDB.recipes.dataArray[i].ResultCounts[j] *= multiplier.Value;
                }
            }
        }
    }
}
