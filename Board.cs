namespace Gomoku;

public class Board
{
    // Свойство BoardArray хранит состояние доски
    // 'init' означает, что массив можно назначить только при создании объекта, но менять его содержимое внутри можно
    public int[,] BoardArray { get; init; } // Двумерный массив, представляющий доску

    private const int _size = 15; // Размер доски (15x15)

    public Board() // Конструктор доски
    {
        BoardArray = new int[_size, _size]; // Выделяем память под двумерный массив 15x15
        InitializeBoard(); // Заполняем нулями
    }

    // Инициализирует доску пустыми клетками
    public void InitializeBoard()
    {
        for (int i = 0; i < _size; i++) // Внешний цикл 'i' бежит по строкам (от 0 до 14)
        {
            for (int j = 0; j < _size; j++) // Внутренний цикл 'j' бежит по столбцам (от 0 до 14)
            {
                BoardArray[i, j] = 0; // 0 представляет пустую клетку
            }
        }
    }

    // Отображает доску в консоли
    public void DisplayBoard(ConsoleColor player1Color = ConsoleColor.Red, ConsoleColor player2Color = ConsoleColor.Blue)
    {
        // 1. Рисуем шапку с номерами столбцов (00 01 02 ... 14)
        // Enumerable.Range генерирует числа, Select форматирует их в строку (D2 - два знака, 01 вместо 1)
        ConsoleHelper.WriteLineColored("   " + string.Join(" ", Enumerable.Range(0, _size).Select(x => x.ToString("D2"))), ConsoleColor.Yellow);
        // 2. Рисуем строки одну за другой
        for (int i = 0; i < _size; i++)
        {
            ConsoleHelper.WriteColored($"{i:D2} ", ConsoleColor.Yellow); // Выводим номер текущей строки слева (боковая координатная сетка)
            for (int j = 0; j < _size; j++) // Проходим по всем клеткам в строке
            {
                char symbol = BoardArray[i, j] switch // Определяем, какой символ рисовать
                {
                    1 => 'X', // Player 1
                    2 => 'O', // Player 2
                    _ => '.'  // Пустая клетка
                };
                ConsoleColor color = BoardArray[i, j] switch // Определяем цвет для этого символа
                {
                    1 => player1Color,
                    2 => player2Color,
                    _ => ConsoleColor.Gray
                };
                ConsoleHelper.WriteColored($"{symbol}  ", color); // Выводим символ и два пробела для отступа
            }
            Console.WriteLine(); // После завершения строки переходим на новую строку консоли
        }
    }

    // Делает ход на доске
    public bool MakeMove(int x, int y, int player) // Метод выполнения хода. Параметры: x (строка), y (столбец), player (номер игрока, 1 или 2)
    {
        // Проверка на валидность (корректность) хода:
        // 1. Не меньше ли нуля координаты? (x < 0)
        // 2. Не больше ли они размера доски? (x >= _size)
        // 3. Не занята ли уже клетка? (BoardArray[x, y] != 0)
        if (x < 0 || x >= _size || y < 0 || y >= _size || BoardArray[x, y] != 0)
        {
            return false; // Invalid move // Если что-то не так, возвращаем false (ход не сделан)
        }

        // Если все проверки пройдены, записываем номер игрока в массив
        BoardArray[x, y] = player;
        return true;
    }

    // Проверяет, выиграл ли игрок
    public bool CheckWin(int player)
    {
        // Проверка строк, столбцов и диагоналей на последовательность из 5
        for (int i = 0; i < _size; i++)
        {
            for (int j = 0; j < _size; j++)
            {
                if (BoardArray[i, j] == player) // Если в текущей клетке [i, j] стоит фишка игрока, то запускаем проверку линий в 4 направлениях от этой точки
                {
                    if (CheckDirection(i, j, 1, 0, player) || // Горизонталь
                        CheckDirection(i, j, 0, 1, player) || // Вертикаль
                        CheckDirection(i, j, 1, 1, player) || // Диагональ \
                        CheckDirection(i, j, 1, -1, player))  // Диагональ /
                    {
                        return true; // Если хоть одна линия собралась — Победа!
                    }
                }
            }
        }
        return false; // Если перебрали всю доску и ничего не нашли
    }

    // Вспомогательный метод для проверки направления на последовательность из 5, x, y — стартовая точка, dx, dy — смещение по осям (определяет направление)
    private bool CheckDirection(int x, int y, int dx, int dy, int player)
    {
        int count = 0; // Считаем, сколько фишек подряд нашли
        for (int i = 0; i < 5; i++) // Цикл от 0 до 4 (нам нужно 5 фишек)
        {
            // Рассчитываем координаты следующей проверяемой клетки.
            // Например, при dx=1, dy=0 (горизонталь):
            // i=0: nx = x + 0 (стартовая)
            // i=1: nx = x + 1 (соседняя справа)
            // i=2: nx = x + 2 ...
            int nx = x + i * dx;
            int ny = y + i * dy;

            // Проверяем:
            // 1. Координата nx внутри доски (>= 0 и < 15)?
            // 2. Координата ny внутри доски?
            // 3. В этой клетке стоит фишка именно нашего игрока?
            if (nx >= 0 && nx < _size && ny >= 0 && ny < _size && BoardArray[nx, ny] == player)
            {
                count++; // Совпадение, увеличиваем счетчик
            }
            else
            {
                break; // Цепочка прервалась (там враг или пустота). Прерываем цикл
            }
        }
        return count >= 5; // Если в итоге насчитали 5 (или больше, хотя тут цикл до 5), возвращаем true
    }
}
