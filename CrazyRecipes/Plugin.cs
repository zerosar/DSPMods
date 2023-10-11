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
        public const string VERSION = "1.1.0";

        private static ConfigEntry<bool> getRecipesFromFile;
        public EditRecipes EditRecipes { get; } = new EditRecipes();

        private void Start()
        {
            getRecipesFromFile = Config.Bind<bool>(
                "",
                "getRecipesFromFile",
                false,
                "Apply settings for recipes from the Recipes.xml file"
                );

            if (getRecipesFromFile.Value) LDBTool.PostAddDataAction = (Action)Delegate.Combine(LDBTool.PostAddDataAction, new Action(EditRecipes.Run));
        }
    }
}
