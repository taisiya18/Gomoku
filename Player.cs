namespace Gomoku;

// Класс, описывающий игрока (участника матча).
// Он хранит данные: как зовут, каким цветом играет и сколько раз выиграл.
public class Player
{
    public string Name { get; set; } = null!; // Имя игрока. "= null!" отключает предупреждение, так как имя будет задано позже
    public ConsoleColor Color { get; set; } // Цвет для отображения фишек и имени в консоли
    public int Score { get; set; } // Текущий счет (количество побед)
}
