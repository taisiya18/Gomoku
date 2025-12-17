using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gomoku;

// Класс-конвертер для JSON
// По умолчанию JSON не умеет работать с многомерными массивами C# вида int[,]
// Он понимает только зубчатые массивы (массив массивов) вида int[][]
// Этот класс учит программу переводить одно в другое
// Пользовательский конвертер JSON для сериализации и десериализации двумерных массивов (int[,])
public class TwoDimensionalIntArrayConverter : JsonConverter<int[,]>
{
    // метод чтения (Deserialization): Из файла JSON -> в код C#
    public override int[,]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Парсим (разбираем) текущий кусок JSON-данных
        // Используем 'using', чтобы ресурсы освободились сразу после завершения работы с jsonDoc
        using var jsonDoc = JsonDocument.ParseValue(ref reader);

        // Определяем размеры будущего массива
        // RootElement - корень текущего JSON-объекта (внешний массив)
        // GetArrayLength() возвращает количество элементов (строк)
        var rowLength = jsonDoc.RootElement.GetArrayLength();
        // Чтобы узнать количество столбцов, берем первый элемент (первую строку) и смотрим его длину
        var columnLength = jsonDoc.RootElement.EnumerateArray().First().GetArrayLength();

        // Создаем в памяти C# пустую сетку (матрицу) нужного размера
        int[,] grid = new int[rowLength, columnLength];

        // Начинаем заполнять сетку данными из JSON
        int row = 0; // Индекс текущей строки
        foreach (var array in jsonDoc.RootElement.EnumerateArray()) // EnumerateArray() позволяет перебирать элементы JSON как в цикле foreach
        {
            int column = 0; // Индекс текущего столбца (сбрасываем для каждой новой строки)
            // Внутренний цикл: перебираем числа внутри одной строки
            foreach (var number in array.EnumerateArray())
            {
                grid[row, column] = number.GetInt32(); // GetInt32() превращает элемент JSON в обычное число int
                column++; // Сдвигаемся вправо
            }
            row++; // Сдвигаемся вниз (на следующую строку)
        }

        return grid; // Возвращаем готовый заполненный массив программе
    }

    // МЕТОД ЗАПИСИ (Serialization): Из кода C# -> в файл JSON
    public override void Write(Utf8JsonWriter writer, int[,] value, JsonSerializerOptions options)
    {
        writer.WriteStartArray(); // Начинаем писать внешний массив. В файле появится символ '['
        for (int i = 0; i < value.GetLength(0); i++) // Цикл по строкам. GetLength(0) возвращает количество строк (1-е измерение)
        {
            writer.WriteStartArray(); // Начинаем писать внутренний массив (строку). В файле появится еще один '['
            for (int j = 0; j < value.GetLength(1); j++) // Цикл по столбцам. GetLength(1) возвращает количество столбцов (2-е измерение)
            {
                writer.WriteNumberValue(value[i, j]); // Записываем конкретное число из ячейки [i, j]
            }
            writer.WriteEndArray(); // Закрываем внутренний массив. В файле появится ']'
        }
        writer.WriteEndArray(); // Закрываем внешний массив. В файле появится финальный ']'
    }
}
