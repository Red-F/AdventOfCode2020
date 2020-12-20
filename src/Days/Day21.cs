using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Days
{
    [Day(2020, 21)]
    public class Day21 : BaseDay
    {
        public override string PartOne(string input)
        {
            var (foods, _, safeIngredients) = ProcessFoods(input);
            return CountIngredients(foods, safeIngredients).ToString();
        }

        public override string PartTwo(string input)
        {
            var (foods, allAllergens, safeIngredients) = ProcessFoods(input);
            StripInertIngredients(foods, safeIngredients);
            var allergenCombinations = IngredientAllergenCombinations(foods, allAllergens);
            var sb = new StringBuilder();
            for (var i = 0; i < allergenCombinations.Length - 1; i++)
                sb.Append(allergenCombinations[i].ingredient + ",");
            sb.Append(allergenCombinations[^1].ingredient);
            return sb.ToString();
        }

        private static (Food[] foods, string[] allAllergens, IEnumerable<string> safeIngredients) ProcessFoods(string input)
        {
            var foods = input.ParseStrings(FoodParser).ToList().OrderByDescending(x => x.Allergens.Count).ToArray();
            var allAllergens = AllAllergens(foods);
            var safeForAllergens = allAllergens.Select(allergen => SafeForAllergen(allergen, foods)).ToArray();
            var safeIngredients = SafeIngredients(safeForAllergens);
            return (foods, allAllergens, safeIngredients);
        }

        private static (string ingredient, string allergen)[] IngredientAllergenCombinations(Food[] foods,
            string[] allAllergens)
        {
            var allIngredients = AllIngredients(foods);
            var rc = new List<(string ingredient, string allergen)>();
            while (rc.Count < allAllergens.Length)
            {
                foreach (var allergen in allAllergens)
                {
                    if (rc.Any(t => t.allergen == allergen)) continue;
                    var foodsForAllergen = foods.Where(f => f.Allergens.Contains(allergen)).ToArray();
                    var allergenList = (from ingredient in allIngredients
                        where foodsForAllergen.All(f => f.Ingredients.Contains(ingredient))
                        select (ingredient, allergen)).ToArray();
                    if (allergenList.Length != 1) continue;
                    var combo = allergenList[0];
                    rc.Add(combo);
                    foods.ForEach(f => f.Ingredients.Remove(combo.ingredient));
                }
            }
            rc = rc.OrderBy(t => t.allergen).ToList();
            return rc.ToArray();
        }

        private static void StripInertIngredients(Food[] foods, IEnumerable<string> safeIngredients)
        {
            foreach (var ingredient in safeIngredients)
            foreach (var t in foods)
                t.Ingredients.Remove(ingredient);
        }

        private static long CountIngredients(IReadOnlyCollection<Food> foods, IEnumerable<string> safeIngredients)
        {
            var rc = safeIngredients.Aggregate(0L,
                (current, ingredient) => current + foods.Count(f => f.Ingredients.Contains(ingredient)));
            return rc;
        }

        private static Food SafeForAllergen(string allergen, Food[] foods)
        {
            var allIngredients = AllIngredients(foods);
            var foodsForAllergen = foods.Where(f => f.Allergens.Contains(allergen)).ToArray();
            var safeIngredientsForAllergen = allIngredients
                .Where(ingredient => foodsForAllergen.Any(f => !f.Ingredients.Contains(ingredient))).ToList();
            return new Food(safeIngredientsForAllergen, new List<string> {$"safe for {allergen}"});
        }

        private static IEnumerable<string> SafeIngredients(IReadOnlyCollection<Food> safeForAllergens)
        {
            var ingredients = new List<string>();
            foreach (var food in safeForAllergens)
                ingredients.AddRange(food.Ingredients.Where(ingredient =>
                    safeForAllergens.All(s => s.Ingredients.Contains(ingredient))));
            return ingredients.Distinct().ToArray();
        }

        private static string[] AllIngredients(IEnumerable<Food> foods) =>
            foods.SelectMany(f => f.Ingredients.Select(i => i)).Distinct().ToArray();

        private static string[] AllAllergens(IEnumerable<Food> foods) =>
            foods.SelectMany(f => f.Allergens.Select(i => i)).Distinct().ToArray();

        private static Food FoodParser(string arg)
        {
            var parts = arg.Split(new[] {'(', ')'}, StringSplitOptions.RemoveEmptyEntries);
            var ingredients = parts[0].Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries).ToList();
            var allergens = parts[1].Replace("contains ", string.Empty)
                .Split(new[] {',', ' '}, StringSplitOptions.RemoveEmptyEntries).ToList();
            return new Food(ingredients, allergens);
        }
    }

    internal record Food (List<string> Ingredients, List<string> Allergens);
}