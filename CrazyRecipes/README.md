This mod allows you to make changes to recipes in the Dyson Sphere Program by using the Recipes.xml file.

**Settings:**

- **getRecipesFromFile** (true/false): Determines whether to apply settings for recipes from the Recipes.xml file.

**Mod Description:**

The mod has been revamped. Now, you need to edit each recipe in the Recipes.xml file. Currently, Recipes.xml is generated, updated, and applied each time the game is launched if the getRecipesFromFile option is set to true.

**Limitations:**

The number of items (<count>0</count>) is restricted in the range of 0 to 1,000,000.

**Notes:**

At the moment, there hasn't been time to check whether LDBTool recognizes recipes from other mods, including those that add recipes without using LDBTool. If LDBTool does recognize recipes from other mods, Recipes.xml should be generated to include all recipes present in the game and added by mods.

If a recipe with a specific ID is missing in the current game session, such a recipe will be automatically removed from the Recipes.xml file.

I've noticed an issue with the production of matrices hanging. I'll fix this later.
