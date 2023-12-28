using System.Text;
using System;
using System.Collections.Generic;

namespace RecipeCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            try
            {
                UserInterface.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }
    }

    public class Recipe
    {
        public string Name { get; set; }
        public List<Ingredient> Ingredients { get; set; }

        public Recipe(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Назва рецепту не може бути порожньою.");

            Name = name;
            Ingredients = new List<Ingredient>();
        }

        public void AddIngredient(Ingredient ingredient)
        {
            if (ingredient == null)
                throw new ArgumentNullException(nameof(ingredient), "Інгредієнт не може бути null.");

            Ingredients.Add(ingredient);
        }

        public void RemoveIngredient(Ingredient ingredient)
        {
            if (ingredient == null)
                throw new ArgumentNullException(nameof(ingredient), "Інгредієнт не може бути null.");

            Ingredients.Remove(ingredient);
        }
    }

    public class Ingredient
    {
        public string Name { get; set; }
        public double Cost { get; set; }
        public double Quantity { get; set; }

        public Ingredient(string name, double cost, double quantity)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Назва інгредієнта не може бути порожньою.");

            if (cost <= 0)
                throw new ArgumentException("Вартість має бути більше нуля.");

            if (quantity <= 0)
                throw new ArgumentException("Кількість має бути більше нуля.");

            Name = name;
            Cost = cost;
            Quantity = quantity;
        }
    }

    public static class Calculator
    {
        public static double CalculateTotalCost(Recipe recipe)
        {
            if (recipe == null)
                throw new ArgumentNullException(nameof(recipe), "Рецепт не може бути null.");

            double totalCost = 0;
            foreach (var ingredient in recipe.Ingredients)
            {
                totalCost += ingredient.Cost * ingredient.Quantity;
            }
            return totalCost;
        }
    }

    public static class DatabaseManager
    {
        private static List<Recipe> recipes = new List<Recipe>();

        public static void AddRecipe(Recipe recipe)
        {
            if (recipe == null)
                throw new ArgumentNullException(nameof(recipe), "Рецепт не може бути null.");

            recipes.Add(recipe);
        }

        public static void RemoveRecipe(Recipe recipe)
        {
            if (recipe == null)
                throw new ArgumentNullException(nameof(recipe), "Рецепт не може бути null.");

            recipes.Remove(recipe);
        }

        public static List<Recipe> GetRecipes()
        {
            return new List<Recipe>(recipes);
        }
    }

    public static class UserInterface
    {
        public static void Start()
        {
            while (true)
            {
                Console.WriteLine("1. Переглянути рецепти");
                Console.WriteLine("2. Додати рецепт");
                Console.WriteLine("3. Видалити рецепт");
                Console.WriteLine("4. Розрахувати вартість страви");
                Console.WriteLine("5. Вихід");
                Console.Write("Оберіть опцію: ");
                string option = Console.ReadLine();

                try
                {
                    switch (option)
                    {
                        case "1":
                            ViewRecipes();
                            break;
                        case "2":
                            AddRecipe();
                            break;
                        case "3":
                            RemoveRecipe();
                            break;
                        case "4":
                            CalculateCost();
                            break;
                        case "5":
                            Console.WriteLine("Програма завершена.");
                            return;
                        default:
                            Console.WriteLine("Невідома опція. Будь ласка, введіть номер від 1 до 5.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Виникла помилка: {ex.Message}");
                }
            }
        }

        private static void ViewRecipes()
        {
            try
            {
                var recipes = DatabaseManager.GetRecipes();
                if (recipes.Count == 0)
                {
                    Console.WriteLine("Жодного рецепту не знайдено.");
                    return;
                }

                foreach (var recipe in recipes)
                {
                    Console.WriteLine($"Назва: {recipe.Name}");
                    foreach (var ingredient in recipe.Ingredients)
                    {
                        Console.WriteLine($"  - {ingredient.Name}: {ingredient.Quantity} за {ingredient.Cost} грн.");
                    }
                    Console.WriteLine($"Вартість страви: {Calculator.CalculateTotalCost(recipe)} грн.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при перегляді рецептів: {ex.Message}");
            }
        }

        public static void AddRecipe()
        {
            try
            {
                Console.Write("Введіть назву рецепту: ");
                string name = Console.ReadLine();
                var recipe = new Recipe(name);

                string addMore;
                do
                {
                    Console.Write("Введіть назву інгредієнта: ");
                    string ingredientName = Console.ReadLine();

                    Console.Write("Введіть кількість: ");
                    if (!double.TryParse(Console.ReadLine(), out double quantity) || quantity <= 0)
                    {
                        throw new ArgumentException("Будь ласка, введіть дійсне додатне число для кількості.");
                    }

                    Console.Write("Введіть вартість за одиницю: ");
                    if (!double.TryParse(Console.ReadLine(), out double cost) || cost <= 0)
                    {
                        throw new ArgumentException("Будь ласка, введіть дійсне додатне число для вартості.");
                    }

                    recipe.AddIngredient(new Ingredient(ingredientName, cost, quantity));

                    Console.Write("Додати ще один інгредієнт? (так/ні): ");
                    addMore = Console.ReadLine();
                } while (addMore.ToLower() == "так");

                DatabaseManager.AddRecipe(recipe);
                Console.WriteLine("Рецепт додано успішно!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при додаванні рецепту: {ex.Message}");
            }
        }

        public static void RemoveRecipe()
        {
            try
            {
                var recipes = DatabaseManager.GetRecipes();
                if (recipes.Count == 0)
                {
                    Console.WriteLine("Жодного рецепту для видалення.");
                    return;
                }

                for (int i = 0; i < recipes.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {recipes[i].Name}");
                }

                Console.Write("Введіть номер рецепту для видалення: ");
                if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > recipes.Count)
                {
                    throw new ArgumentException("Будь ласка, введіть дійсний номер рецепту.");
                }

                index -= 1; // Adjust for 0-based index
                DatabaseManager.RemoveRecipe(recipes[index]);
                Console.WriteLine("Рецепт видалено успішно!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при видаленні рецепту: {ex.Message}");
            }
        }

        public static void CalculateCost()
        {
            try
            {
                var recipes = DatabaseManager.GetRecipes();
                if (recipes.Count == 0)
                {
                    Console.WriteLine("Жодного рецепту для розрахунку.");
                    return;
                }

                for (int i = 0; i < recipes.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {recipes[i].Name}");
                }

                Console.Write("Введіть номер рецепту для розрахунку вартості: ");
                if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > recipes.Count)
                {
                    throw new ArgumentException("Будь ласка, введіть дійсний номер рецепту.");
                }

                index -= 1; // Adjust for 0-based index
                double totalCost = Calculator.CalculateTotalCost(recipes[index]);
                Console.WriteLine($"Загальна вартість {recipes[index].Name}: {totalCost} грн.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при розрахунку вартості: {ex.Message}");
            }
        }
    }
}