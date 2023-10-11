using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using UnityEngine;

namespace CrazyRecipes
{
    public class EditRecipes
    {
        private static readonly string Dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private readonly string FilePathRecipes = Path.Combine(Dir, "Recipes.xml");
        private const int MaxItemCount = 1000000;

        public  void Run()
        {
            List<Recipe> FromGame = GetRecipesFromGame();
            List<Recipe> FromFile = GetRecipesFromFile();

            if (FromFile == null)
            {
                FromFile = FromGame;
            }

            RemoveDuplicates(FromFile);
            //RemoveRecipesNotInGame(FromFile, FromGame);
            AddRecipesFromFile(FromFile, FromGame);

            //foreach (var recipe in FromFile)
            //{
            //    CleanItems(recipe.inItem);
            //    CleanItems(recipe.outItem);
            //}

            SetRecipesToGame(FromFile);
            SetRecipesToFile(FromFile);
        }

        private void RemoveDuplicates(List<Recipe> fromFile)
        {
            fromFile = fromFile.GroupBy(recipe => recipe.ID).Select(group => group.First()).ToList();
        }

        //private void RemoveRecipesNotInGame(List<Recipe> fromFile, List<Recipe> fromGame)
        //{
        //    fromFile.RemoveAll(recipe => !fromGame.Any(gameRecipe => gameRecipe.ID == recipe.ID));
        //}

        private void AddRecipesFromFile(List<Recipe> fromFile, List<Recipe> fromGame)
        {
            foreach (var gameRecipe in fromGame)
            {
                if (!fromFile.Any(fileRecipe => fileRecipe.ID == gameRecipe.ID))
                {
                    fromFile.Add(gameRecipe);
                }
            }
        }

        //private void CleanItems(List<Item> items)
        //{
        //    for (int i = items.Count - 1; i >= 0; i--)
        //    {
        //        var item = items[i];
        //        var itemProto = LDB.items.Select(item.ID);
        //        if (itemProto == null)
        //        {
        //            items.RemoveAt(i);
        //        }
        //        else
        //        {
        //            item.count = Mathf.Clamp(item.count, 0, MaxItemCount);
        //        }
        //    }
        //}

        private List<Recipe> GetRecipesFromGame()
        {
            List<Recipe> recipes = new List<Recipe>();

            foreach (var recipe in LDB.recipes.dataArray)
            {
                List<Item> inItems = GetItemRecipes(recipe.Items, recipe.ItemCounts);
                List<Item> outItems = GetItemRecipes(recipe.Results, recipe.ResultCounts);

                recipes.Add(new Recipe
                {
                    name = recipe.name,
                    ID = recipe.ID,
                    inItem = inItems,
                    outItem = outItems
                });
            }

            return recipes.Count > 0 ? recipes : null;
        }

        private List<Recipe> GetRecipesFromFile()
        {
            if (!File.Exists(FilePathRecipes))
            {
                return null;
            }

            XmlSerializer serializer = new XmlSerializer(typeof(List<Recipe>));
            try
            {
                using (FileStream stream = new FileStream(FilePathRecipes, FileMode.Open))
                {
                    List<Recipe> recipes = (List<Recipe>)serializer.Deserialize(stream);
                    return recipes.Count > 0 ? recipes : null;
                }
            }
            catch
            {
                return null;
            }
        }

        private void SetRecipesToGame(List<Recipe> recipes)
        {
            foreach (Recipe recipe in recipes)
            {
                RecipeProto gameRecipe = LDB.recipes.Select(recipe.ID);
                int[] matrixRecipesID = { 9, 18, 27, 55, 75, 102 };
                if (gameRecipe == null || matrixRecipesID.Contains(gameRecipe.ID))
                {
                    continue;
                }

                List<int> Items = new List<int>();
                List<int> ItemCounts = new List<int>();
                foreach (Item item in recipe.inItem)
                {
                    var itemProto = LDB.items.Select(item.ID);
                    if (itemProto != null)
                    {
                        Items.Add(item.ID);
                        ItemCounts.Add(Mathf.Clamp(item.count, 0, MaxItemCount));
                    }
                }

                List<int> Results = new List<int>();
                List<int> ResultCounts = new List<int>();
                foreach (Item item in recipe.outItem)
                {
                    var itemProto = LDB.items.Select(item.ID);
                    if (itemProto != null)
                    {
                        Results.Add(item.ID);
                        ResultCounts.Add(Mathf.Clamp(item.count, 0, MaxItemCount));
                    }
                }

                gameRecipe.Items = Items.ToArray();
                gameRecipe.ItemCounts = ItemCounts.ToArray();
                gameRecipe.Results = Results.ToArray();
                gameRecipe.ResultCounts = ResultCounts.ToArray();
            }
        }

        private void SetRecipesToFile(List<Recipe> recipes)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Recipe>));
            try
            {
                using (FileStream stream = new FileStream(FilePathRecipes, FileMode.Create))
                {
                    serializer.Serialize(stream, recipes);
                }
            }
            catch
            {
                // Handle the exception
            }
        }

        private List<Item> GetItemRecipes(int[] itemIds, int[] itemCounts)
        {
            List<Item> itemRecipes = new List<Item>();

            for (int i = 0; i < itemIds.Length; i++)
            {
                ItemProto selectItem = LDB.items.Select(itemIds[i]);
                itemRecipes.Add(new Item
                {
                    name = selectItem.name,
                    ID = selectItem.ID,
                    count = itemCounts[i]
                });
            }

            return itemRecipes;
        }
    }

    [Serializable]
    public class Recipe
    {
        public string name;
        public int ID;
        public List<Item> inItem;
        public List<Item> outItem;
    }

    [Serializable]
    public class Item
    {
        public string name;
        public int ID;
        public int count;
    }
}
