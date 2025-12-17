using System.Text.Json;

namespace Gomoku;

// Статический класс для управления сохранением/загрузкой игры и счета. Он отделен от логики игры, чтобы код был чище (принцип единственной ответственности)
public static class SaveManager
{
    // Пути к файлам. const означает, что эти строки нельзя изменить во время работы программы
    private const string _saveGameFilePath = "game_save.json"; // Путь к файлу для сохранения состояния игры
    private const string _saveScoreFilePath = "score_save.json"; // Путь к файлу для сохранения счетов игроков

    // БЛОК СОХРАНЕНИЯ ИГРЫ
    // Сохраняет текущее состояние игры в файл
    public static void SaveGame(Game game)
    {
        // Сериализация объекта игры в JSON с пользовательскими настройками
        // WriteIndented = true делает файл красивым (с отступами)
        // Добавляем наш конвертер для правильной обработки двумерного массива доски
        string json = JsonSerializer.Serialize(
            game,
            new JsonSerializerOptions { WriteIndented = true, Converters = { new TwoDimensionalIntArrayConverter() } }
        );

        // Запись сериализованного JSON в файл сохранения
        File.WriteAllText(_saveGameFilePath, json); // File.WriteAllText создает файл (или перезаписывает существующий) и кладет туда текст.

        // Уведомление пользователя о том, что игра сохранена
        ConsoleHelper.WriteLineColored($"Game saved to {_saveGameFilePath}", ConsoleColor.Green);
    }

    //  БЛОК ЗАГРУЗКИ ИГРЫ 
    public static Game? LoadGame() // Загружает состояние игры из файла
    {
        // Проверка существования файла сохранения, иначе будет ошибка
        if (!File.Exists(_saveGameFilePath))
        {
            ConsoleHelper.WriteLineColored("No save file found.", ConsoleColor.Red);
            return null; // Возврат null, если файл сохранения не найден
        }

        // Чтение содержимого JSON из файла сохранения в одну строковую переменную.
        string json = File.ReadAllText(_saveGameFilePath);

        // Превращаем текст обратно в объект Game
        var game = JsonSerializer.Deserialize<Game>(
            json,
            new JsonSerializerOptions { WriteIndented = true, Converters = { new TwoDimensionalIntArrayConverter() } }
        );

        // Проверка успешности десериализации
        if (game == null)
        {
            ConsoleHelper.WriteLineColored("Failed to load game state.", ConsoleColor.Red);
            return null; // Возврат null, если десериализация не удалась
        }

        // Уведомление пользователя об успешной загрузке игры
        ConsoleHelper.WriteLineColored("Game loaded successfully.", ConsoleColor.Green);
        return game; // Возврат загруженного объекта игры
    }

    //  БЛОК СОХРАНЕНИЯ РЕКОРДОВ 
    public static void SaveScores(List<Player> players) // Сохраняет счета всех игроков в файл
    {
        // Сериализация списка игроков в JSON
        // Тут конвертер для массивов не нужен, так как List сериализуется стандартно
        string json = JsonSerializer.Serialize(
            players,
            new JsonSerializerOptions { WriteIndented = true }
        );

        // Запись сериализованного JSON в файл сохранения счета
        File.WriteAllText(_saveScoreFilePath, json);

        // Уведомление пользователя о том, что счета сохранены
        ConsoleHelper.WriteLineColored($"Scores saved to {_saveScoreFilePath}", ConsoleColor.Green);
    }

    // БЛОК ЗАГРУЗКИ РЕКОРДОВ 
    public static List<Player> LoadScores() // Загружает счета всех игроков из файла
    {
        // Проверка существования файла сохранения счета
        if (!File.Exists(_saveScoreFilePath))
        {
            ConsoleHelper.WriteLineColored("No save file found.", ConsoleColor.Red);
            return new List<Player>(); // Возврат пустого списка, если файл сохранения не найден
        }

        // Чтение содержимого JSON из файла сохранения счета
        string json = File.ReadAllText(_saveScoreFilePath);

        // Десериализация содержимого JSON в список объектов Player
        var scores = JsonSerializer.Deserialize<List<Player>>(
            json,
            new JsonSerializerOptions { WriteIndented = true }
        );

        // Проверка успешности десериализации
        if (scores == null)
        {
            ConsoleHelper.WriteLineColored("Failed to load top scores.", ConsoleColor.Red);
            return new List<Player>(); // Возврат пустого списка, если десериализация не удалась
        }

        // Уведомление пользователя об успешной загрузке счетов
        ConsoleHelper.WriteLineColored("Scores loaded successfully.", ConsoleColor.Green);
        return scores; // Возврат загруженного списка счетов игроков
    }
}