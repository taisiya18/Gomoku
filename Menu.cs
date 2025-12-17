namespace Gomoku;

public class Menu
{
    // Поле _game хранит состояние текущей игры (доску, игроков, чей ход).
    // private означает, что доступ к нему есть только внутри этого класса Menu
    private Game _game; // Текущий экземпляр игры

    public Menu() // Конструктор класса Menu. Вызывается один раз при старте программы
    {
        _game = new Game(); // Инициализация нового экземпляра игры
    }

    // Главный метод отображения меню. Он отображает главное меню и обрабатывает ввод пользователя
    public void Display()
    {
        int choice = 0; // Переменная для хранения выбора пользователя

        while (choice != 7) // Цикл while работает до тех пор, пока пользователь не выберет пункт 7 (Выход)
        {
            // БЛОК ОТРИСОВКИ ИНФОРМАЦИИ ОБ ИГРОКАХ
            ConsoleHelper.ClearWithHeader("=== Player Info ===");

            // Используем форматирование строк (интерполяцию).
            // {-15} означает: "выдели 15 символов под текст и выровняй его по левому краю"
            ConsoleHelper.WriteLineColored($"{"Name",-15}{"Color",-15}{"Score",-10}", ConsoleColor.Green);
            ConsoleHelper.WriteLineColored(new string('-', 35), ConsoleColor.Cyan);
            // Выводим данные Player1 и Player2 с тем же форматированием, чтобы получилась таблица
            ConsoleHelper.WriteLineColored($"{_game.Player1.Name,-15}{_game.Player1.Color,-15}{_game.Player1.Score,-10}", ConsoleColor.Yellow);
            ConsoleHelper.WriteLineColored($"{_game.Player2.Name,-15}{_game.Player2.Color,-15}{_game.Player2.Score,-10}", ConsoleColor.Yellow);
            ConsoleHelper.WriteLineColored(new string('-', 35), ConsoleColor.Cyan);

            Console.WriteLine();// Пустая строка для отступа

            // БЛОК ИНФОРМАЦИИ О ТЕКУЩЕЙ ПАРТИИ (если она идет)
            if (_game.IsStarted)
            {
                ConsoleHelper.WriteLineColored("=== Current Game ===");
                ConsoleHelper.WriteLineColored($"{"Player 1",-15}{"Player 2",-15}{"Move",-10}{"Current Player",-10}", ConsoleColor.Green);
                ConsoleHelper.WriteLineColored(new string('-', 45), ConsoleColor.Cyan);
                ConsoleHelper.WriteLineColored($"{_game.Player1.Name,-15}{_game.Player2.Name,-15}{_game.MoveCount,-10}{_game.CurrentPlayer,-10}", ConsoleColor.Yellow);
                ConsoleHelper.WriteLineColored(new string('-', 45), ConsoleColor.Cyan);

                Console.WriteLine();
            }

            //ОТРИСОВКА ПУНКТОВ МЕНЮ
            ConsoleHelper.WriteLineColored("=== Gomoku Menu ===", ConsoleColor.Magenta);
            ConsoleHelper.WriteLineColored("1. Start New Game", ConsoleColor.Blue);
            // Тернарный оператор (условие ? если_да : если_нет)
            // Если игра идет (_game.IsStarted), цвет синий. Если нет — темно-синий (показывает, что пункт неактивен)
            ConsoleHelper.WriteLineColored("2. Continue Game", _game.IsStarted ? ConsoleColor.Blue : ConsoleColor.DarkBlue);
            ConsoleHelper.WriteLineColored("3. Load Last Save", ConsoleColor.Blue);
            ConsoleHelper.WriteLineColored("4. Save Current Game", _game.IsStarted ? ConsoleColor.Blue : ConsoleColor.DarkBlue);
            ConsoleHelper.WriteLineColored("5. Settings", ConsoleColor.Blue);
            ConsoleHelper.WriteLineColored("6. Top Scores", ConsoleColor.Blue);
            ConsoleHelper.WriteLineColored("7. Exit", ConsoleColor.Blue);
            ConsoleHelper.WriteColored("Enter your choice: ", ConsoleColor.Gray);

            string? input = Console.ReadLine(); // Читаем ввод пользователя. Может быть null, поэтому ставим '?' у string
            if (int.TryParse(input, out choice)) // Пытаемся превратить строку в число, если получилось — результат запишется в variable choice, а метод вернет true
            {
                // Оператор switch проверяет значение переменной choice
                switch (choice)
                {
                    case 1:
                        _game.InitializeNewGame(); // Сбрасываем доску
                        _game.Play(); // Запускаем игровой цикл
                        break; // Выходим из switch (возвращаемся к началу while)
                    case 2:
                        // Если игра не начата, кнопка "Продолжить" не должна работать
                        if (!_game.IsStarted) break;

                        _game.Play();
                        break;
                    case 3:
                        // Пытаемся загрузить игру
                        // Оператор '??' означает: если LoadGame() вернул null (ошибка), то оставь старое значение _game
                        _game = SaveManager.LoadGame() ?? _game;
                        break;
                    case 4:
                        if (!_game.IsStarted) break; // Нельзя сохранять пустую игру

                        SaveManager.SaveGame(_game); // Сохраняем в файл
                        break;
                    case 5:
                        Settings(); // Идем в подменю настроек
                        break;
                    case 6:
                        TopScores(); // Идем в таблицу рекордов
                        break;
                    case 7:
                        ConsoleHelper.WriteLineColored("Exiting the game. Goodbye!", ConsoleColor.Green); // Просто прощаемся. Цикл while (choice != 7) завершится сам
                        break;
                    default:
                        ConsoleHelper.WriteLineColoredWithReadKey("Invalid choice. Please try again.", ConsoleColor.Red); // Если ввели число, которого нет в меню (например, 99)
                        break;
                }
            }
            else
            {
                ConsoleHelper.WriteLineColoredWithReadKey("Please enter a valid number!\n\n", ConsoleColor.Red); // Если ввели буквы вместо цифр
            }
        }
    }

    // Отображает меню настроек
    private void Settings()
    {
        int choice = 0;

        // Локальный цикл меню настроек. Работает, пока не нажмут 3 (Назад)
        while (choice != 3)
        {
            ConsoleHelper.ClearWithHeader("=== Settings ===");
            ConsoleHelper.WriteLineColored("1. Edit Player1", ConsoleColor.Blue);
            ConsoleHelper.WriteLineColored("2. Edit Player2", ConsoleColor.Blue);
            ConsoleHelper.WriteLineColored("3. Return to main menu", ConsoleColor.Blue);
            ConsoleHelper.WriteColored("Enter your choice: ", ConsoleColor.Gray);

            string? input = Console.ReadLine();
            if (int.TryParse(input, out choice))
            {
                switch (choice)
                {
                    case 1:
                        ChangePlayer(1); // Редактируем первого игрока
                        break;
                    case 2:
                        ChangePlayer(2); // Редактируем второго игрока
                        break;
                    case 3:
                        return; // return выбрасывает нас из метода Settings() обратно в Display()
                    default:
                        ConsoleHelper.WriteLineColored("Invalid choice. Please try again.", ConsoleColor.Red);
                        break;
                }
            }
            else
            {
                ConsoleHelper.WriteLineColored("Please enter a valid number!\n\n", ConsoleColor.Red);
            }
        }
    }

    // Позволяет пользователю изменить имя и цвет игрока
    private void ChangePlayer(int playerNumber)
    {
        ConsoleHelper.WriteColored("Enter new name: ", ConsoleColor.Gray);
        string newName = Console.ReadLine() ?? $"Player{playerNumber}"; // Читаем имя. Если ввели пустоту (null), используем дефолтное "PlayerX"

        ConsoleHelper.WriteLineColored("Choose new color:", ConsoleColor.Cyan);
        foreach (var color in Enum.GetValues(typeof(ConsoleColor))) // Enum.GetValues берет все возможные цвета из перечисления ConsoleColor и цикл выводит их списком
        {
            ConsoleHelper.WriteLineColored($"- {color}", ConsoleColor.Gray);
        }

        ConsoleHelper.WriteColored("Enter your choice: ", ConsoleColor.Gray);
        string? userInput = Console.ReadLine();

        // Enum.TryParse пытается найти цвет по названию (например, "Red")
        // Параметр 'true' означает игнорирование регистра (red = Red)
        if (!Enum.TryParse(userInput, true, out ConsoleColor newColor))
        {
            ConsoleHelper.WriteLineColoredWithReadKey("Invalid color choice. Color was restored to black.", ConsoleColor.Red);
        }

        // Применяем изменения в объекте игры
        _game.ChangePlayer(playerNumber, newName, newColor);
    }

    // Отображает лучшие результаты и позволяет сохранять счет
    private void TopScores()
    {
        int choice = 0;

        while (choice != 3)
        {
            int rank = 1; // Место в рейтинге (1, 2, 3...)
            var scores = SaveManager.LoadScores(); // Загружаем список из файла

            scores.Sort((a, b) => b.Score.CompareTo(a.Score)); // Сортируем список ((a, b) => b.Score.CompareTo(a.Score) означает сортировку по убыванию очков)

            ConsoleHelper.ClearWithHeader("=== Top Scores ===");
            ConsoleHelper.WriteLineColored($"{"Rank",-5}{"Name",-20}{"Score",-10}", ConsoleColor.Green);
            ConsoleHelper.WriteLineColored(new string('-', 35), ConsoleColor.Cyan);

            foreach (var score in scores) // Выводим каждого игрока из отсортированного списка
            {
                ConsoleHelper.WriteLineColored($"{rank,-5}{score.Name,-20}{score.Score,-10}", ConsoleColor.Yellow);
                rank++; // Увеличиваем ранг для следующей строки
            }

            ConsoleHelper.WriteLineColored(new string('-', 35), ConsoleColor.Cyan); // Рисуем разделительную черту голубого цвета для красоты (строка из 35 дефисов)
            // Выводим список доступных действий (опций меню)
            ConsoleHelper.WriteLineColored("1. Save Player1 Score", ConsoleColor.Blue);
            ConsoleHelper.WriteLineColored("2. Save Player2 Score", ConsoleColor.Blue);
            ConsoleHelper.WriteLineColored("3. Return to main menu", ConsoleColor.Blue);
            ConsoleHelper.WriteColored("Enter your choice: ", ConsoleColor.Gray); // Выводим приглашение к вводу (без перехода на новую строку, чтобы курсор мигал тут же)

            string? input = Console.ReadLine(); // Читаем ввод с клавиатуры, string? означает, что переменная может быть null

            // Пытаемся превратить введенную строку в целое число (int)
            // Если успешно: метод вернет true, а число запишется в переменную choice
            // Если ошибка (буквы, пустота): метод вернет false, и мы уйдем в блок else
            if (int.TryParse(input, out choice))
            {
                switch (choice) // Проверяем, какое именно число ввел пользователь
                {
                    case 1:
                        ChnageScore(scores, _game.Player1); // Если выбрали 1, запускаем логику сохранения очков для Игрока 1
                        break;
                    case 2:
                        ChnageScore(scores, _game.Player2); // То же самое для Игрока 2
                        break;
                    case 3:
                        // Команда "Назад"
                        return; // 'return' выходит из switch, полностью прерывает выполнение метода TopScores() и возвращает управление туда, откуда этот метод вызвали (в главное меню Menu.Display)
                    default:
                        ConsoleHelper.WriteLineColoredWithReadKey("Invalid choice. Please try again.", ConsoleColor.Red); // Сюда попадаем, если ввели число, но его нет в меню (например, 0 или 10)
                        break;
                }
            }
            else
            {
                ConsoleHelper.WriteLineColoredWithReadKey("Please enter a valid number!\n\n", ConsoleColor.Red); // Сюда попадаем, если пользователь ввел "абвгд" вместо числа
            }
        }
    }

    // Обновляет или добавляет счет игрока в список лучших результатов
    private void ChnageScore(List<Player> scores, Player score)
    {
        var existingScore = scores.FirstOrDefault(s => s.Name == score.Name); // Ищем в списке scores игрока с таким же именем (LINQ запрос FirstOrDefault)
        if (existingScore != null) // Если такой игрок уже был в списке
        {
            ConsoleHelper.WriteLineColored("Score for Player1 already exists in the top scores.", ConsoleColor.Red); // Выводим предупреждение красным цветом
            ConsoleHelper.WriteColored("Do you want to override it (y/n):", ConsoleColor.Red); // Спрашиваем пользователя, хочет ли он обновить результат

            string? input = Console.ReadLine(); // Читаем ответ. '?' означает, что переменная input может принять значение null
            if (input == "y")
            {
                // берем найденный в списке объект (existingScore) и обновляем ему поле Score.
                // Поскольку existingScore — это ссылка на объект внутри списка scores, список тоже обновится.
                existingScore.Score = score.Score; // Перезаписываем его очки
            }
            else
            {
                return; // Ничего не делаем
            }
        }
        else
        {
            scores.Add(score); // Добавляем нового игрока в список
        }

        SaveManager.SaveScores(scores); // Записываем обновленный список на диск
    }
}