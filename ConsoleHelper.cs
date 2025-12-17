namespace Gomoku;

public static class ConsoleHelper
{
    // Выводим сообщение заданным цветом, не переходя на новую строку
    public static void WriteColored(string message, ConsoleColor color = ConsoleColor.Gray)
    {
        // Сохраняем текущий цвет консоли, чтобы потом его вернуть
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = color; // Устанавливаем нужный цвет
        Console.Write(message); // Выводим сообщение на экран
        Console.ForegroundColor = originalColor; // Возвращаем исходный цвет
    }

    // Выводит сообщение заданным цветом и переводит курсор на следующую строку
    public static void WriteLineColored(string message, ConsoleColor color = ConsoleColor.Gray)
    {
        var originalColor = Console.ForegroundColor; // Сохраняем исходный цвет
        Console.ForegroundColor = color; // Меняем цвет на нужный
        Console.WriteLine(message); // Выводим сообщение и переводим курсор на следующую строку
        Console.ForegroundColor = originalColor; // Возвращаем исходный цвет обратно
    }

    // Метод: пишет сообщение, переходит на новую строку и ставит программу на паузу, пока пользователь не нажмет любую кнопку
    public static void WriteLineColoredWithReadKey(string message, ConsoleColor color = ConsoleColor.Gray)
    {
        WriteLineColored(message, color); // Используем метод для вывода текста
        Console.WriteLine("Press any key to continue..."); // Пишем подсказку пользователю
        // Console.ReadKey(true) ждет нажатия клавиши
        // Аргумент 'true' (intercept) означает, что нажатый символ НЕ появится на экране
        Console.ReadKey(true);
    }

    // Очищает консоль и при необходимости отображает заголовок
    public static void ClearWithHeader(string header = "")
    {
        Console.Clear(); // Полная очистка экрана
        if (!string.IsNullOrEmpty(header)) // Проверяем: если строка заголовка не пустая и не null
        {
            // Если заголовок есть, пишем его голубым цветом и добавляем подчеркивание
            WriteLineColored(header, ConsoleColor.Cyan); //выводим заголовок голубым цветом
            Console.WriteLine(new string('-', header.Length)); // Рисуем подчеркивание
        }
    }
}
