namespace Gomoku;

public class Game
{
    // Свойства игры, 'get; set;' означает, что мы можем и читать, и записывать эти данные
    public Player Player1 { get; set; }
    public Player Player2 { get; set; }
    public int MoveCount { get; set; } // Счетчик ходов
    public bool IsStarted { get; set; } // Флаг: true, если игра активна; false, если завершена или не началась
    public Board Board { get; set; } // Объект доски, хранящий массив клеток (15x15)
    public int CurrentPlayer { get; set; } // Номер игрока, который должен ходить сейчас (1 или 2)

    // Конструктор класса. Вызывается один раз при создании объекта Game
    public Game()
    {
        // Инициализируем игроков значениями по умолчанию, чтобы избежать ошибок null reference
        Player1 = new Player() { Name = "Player1", Color = ConsoleColor.Red, Score = 0 };
        Player2 = new Player() { Name = "Player2", Color = ConsoleColor.Blue, Score = 0 };

        InitializeNewGame(); // Подготавливаем доску и сбрасываем счетчики
    }

    // Метод сброса игры в начальное состояние (начало новой партии)
    public void InitializeNewGame()
    {
        Board = new Board(); // Выделяем новую память под доску (старая удалится сборщиком мусора)
        CurrentPlayer = 1; // Первым всегда ходит Игрок 1 (красные)
        MoveCount = 0; // Сбрасываем счетчик ходов
        IsStarted = false; // Игра готова, но пока стоит на паузе
    }

    // Метод для обновления имени и цвета игрока
    public void ChangePlayer(int playerNumber, string name, ConsoleColor color)
    {
        // Используем тернарный оператор для выбора нужного игрока для редактирования
        // Если playerNumber == 1, берем Player1, иначе Player2.
        Player playerToEdit = playerNumber == 1 ? Player1 : Player2;

        playerToEdit.Name = name;
        playerToEdit.Color = color;
    }

    // Заглушка для загрузки игры (реализация находится в SaveManager)
    public void LoadSave()
    {
        Console.WriteLine("Load game functionality is not implemented yet.");
    }

    // ОСНОВНОЙ ИГРОВОЙ ЦИКЛ

    public void Play() // Главный метод, запускающий игровой процесс
    {
        IsStarted = true; // Ставим флаг, что игра началась

        while (true) // Бесконечный цикл while(true). Он будет крутиться, пока не произойдет победа или выход в меню
        {
            DisplayBoard(); // 1. Отрисовываем текущее состояние доски

            // 2. Пишем, чей ход
            // Получаем имя и цвет текущего игрока, чтобы вывести сообщение в его цвете
            ConsoleHelper.WriteLineColored($"{GetCurrentPlayer().Name}'s turn (Symbol: {(GetCurrentPlayer().Color == ConsoleColor.Red ? "X" : "O")})", GetCurrentPlayer().Color);
            ConsoleHelper.WriteColored("Enter row and column (e.g., 3 4) or \"menu\": ", ConsoleColor.Gray);

            // 3. Читаем ввод пользователя. Если ввели пустоту, заменяем на пустую строку
            string input = Console.ReadLine() ?? string.Empty;

            // Если пользователь ввел команду "menu", мы прерываем цикл и выходим в главное меню
            if (input == "menu")
            {
                break; // Прерываем цикл while, возвращая управление в класс Menu
            }

            // 4. Пытаемся понять, ввел ли пользователь координаты (два числа)
            if (TryParseMove(input, out int x, out int y))
            {
                // 5. Пытаемся сделать ход на доске
                // Метод MakeMove сам проверит, не занята ли клетка и не выходят ли координаты за границы
                if (Board.MakeMove(x, y, CurrentPlayer))
                {
                    // 6. Если ход успешен, сразу проверяем: не победа ли это?
                    if (Board.CheckWin(CurrentPlayer))
                    {
                        DisplayBoard(); // Показываем финальную доску с победным ходом
                        ConsoleHelper.WriteLineColoredWithReadKey($"{GetCurrentPlayer().Name} wins!", GetCurrentPlayer().Color);
                        GetCurrentPlayer().Score++; // Начисляем очко победителю
                        IsStarted = false; // Помечаем игру как завершенную
                        break; // Выходим из цикла while(true) — игра окончена
                    }
                    SwitchPlayer(); // Если не победа, передаем ход следующему игроку
                    MoveCount++; // Увеличиваем общий счетчик ходов
                }
                else
                {
                    // Сюда попадаем, если MakeMove вернул false (занято или за границей)
                    ConsoleHelper.WriteLineColoredWithReadKey("Invalid move. Cell is already occupied or out of bounds.", ConsoleColor.Red);
                }
            }
            else
            {
                // Сюда попадаем, если TryParseMove не нашел два числа в строке
                ConsoleHelper.WriteLineColoredWithReadKey("Invalid input. Please enter row and column as two numbers separated by a space.", ConsoleColor.Red);
            }
        }
    }

    // Приватный метод для очистки экрана и вызова отрисовки доски (инкапсуляция)
    // Скрывает детали реализации очистки от основного цикла
    private void DisplayBoard()
    {
        ConsoleHelper.ClearWithHeader("Gomoku Game");
        Board.DisplayBoard(Player1.Color, Player2.Color);
    }

    // Вспомогательный метод: возвращает ссылку на объект того игрока, чей сейчас ход
    // Используется, чтобы не писать везде конструкцию (CurrentPlayer == 1 ? Player1 : Player2)
    private Player GetCurrentPlayer()
    {
        return CurrentPlayer == 1 ? Player1 : Player2;
    }

    //Метод переключения очередности хода, меняет номер текущего игрока с 1 на 2 и обратно
    private void SwitchPlayer()
    {
        CurrentPlayer = CurrentPlayer == 1 ? 2 : 1;
    }

    // Метод разбора строки ввода
    // Пытается превратить строку вида "3 5" в две целочисленные переменные x и y
    // Возвращает true, если преобразование прошло успешно, иначе false
    private bool TryParseMove(string input, out int x, out int y)
    {
        x = y = -1; // Инициализируем переменные некорректными значениями
        var parts = input.Split(' '); // Разбиваем строку по пробелам

        // Проверяем:
        // 1. Получилось ровно две части?
        // 2. Первая часть — это число?
        // 3. Вторая часть — это число?
        return parts.Length == 2 &&
               int.TryParse(parts[0], out x) &&
               int.TryParse(parts[1], out y);
    }
}
