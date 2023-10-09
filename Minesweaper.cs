namespace Struct;

internal static class Program
{
    //SETTINGS
    private const int Width = 9;
    private const int Height = 9;
    private const string Space = " "; 
    private const int BombCount = 18;
    
    private static void Main(string[] args)
    {
        Tile[,] tiles = new Tile[Width+2,Height+2];
        for (int x = 0; x < Width+2; x++)
        {
            for (int y = 0; y < Height + 2; y++)
            {
                tiles[x,y] = new Tile(x,y);
                if(x == 0 || y == 0 || x == Width+1 || y == Height+1)
                    tiles[x,y].IsWall = true;
                else 
                    tiles[x,y].IsWall = false;
            }
        }

        Random random = new Random();
        for (int i = 0; i < BombCount; i++)
        {
            int x = random.Next(1, Width);
            int y = random.Next(1, Height);
            if (!tiles[x, y].IsBomb)
            {
                tiles[x, y].IsBomb = true;
            }
        }
        
        while (true)
        {
            Console.Clear();
            tiles.UpdateBoard();
            tiles.PrintGrid();
            Console.WriteLine("Enter X:");
            string? xInput = Console.ReadLine();
            if (!int.TryParse(xInput, out int x))
                continue;
            Console.WriteLine("Enter Y:");
            string? yInput = Console.ReadLine();
            if (!int.TryParse(yInput, out int y))
                continue;
            
            if(!(y is > 0 and < Height+1 && x is > 0 and < Height+1))
                continue;
            
            Console.WriteLine("enter action (r/f)");
            string? action = Console.ReadLine();
            
            if (action == "r")
            {
                tiles[x, y].IsRevealed = true;
            }
            else if (action == "f")
            {
                tiles[x, y].IsFlagged = true;
            }
        }
    }

    private static void UpdateBoard(this Tile[,] tiles)
    {
        for(int x = 1; x < Width+1; x++)
        {
            for (int y = 1; y < Height+1; y++)
            {
                Tile tile = tiles[x, y];
                if (tile.IsBomb)
                {
                    continue;
                }

                int bombCount = 0;
                for (int x1 = x - 1; x1 <= x + 1; x1++)
                {
                    for (int y1 = y - 1; y1 <= y + 1; y1++)
                    {
                        if (tiles[x1, y1].IsBomb)
                        {
                            bombCount++;
                        }
                    }
                }
                tiles[x,y].BombCount = bombCount;

                if (bombCount == 0 && tile.IsRevealed)
                {
                    for (int x1 = x - 1; x1 <= x + 1; x1++)
                    {
                        for (int y1 = y - 1; y1 <= y + 1; y1++)
                        {
                            tiles[x1, y1].IsRevealed = true;
                        }
                    }
                }
            }
        }
    }

    private static void PrintGrid(this Tile[,] tiles)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("X");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Y");
        Console.ForegroundColor = ConsoleColor.White;
        for (int x = 0; x < Width+2; x++)
        {
            for (int y = 0; y < Height+2; y++)
            {
                Tile tile = tiles[x, y];
                if (tile.IsWall)
                {
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    if (x == 0 && y is > 0 and < Height+1)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write($"|{Space}{y}{Space}|");
                        Console.ForegroundColor = ConsoleColor.White;

                    }
                    else if (y == 0 && x is > 0 and < Width+1)
                    { 
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write($"|{Space}{x}{Space}|");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.Write($"|{Space}#{Space}|");
                    }
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                else if (tile.IsFlagged)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($"|{Space}|{Space}|");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if (tile.IsRevealed)
                {
                    if (tile.IsBomb)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write($"|{Space}*{Space}|");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (tile.BombCount != 0)
                    {
                        switch (tile.BombCount)
                        {
                            case 1:
                                Console.ForegroundColor = ConsoleColor.Blue;
                                break;
                            case 2:
                                Console.ForegroundColor = ConsoleColor.Green;
                                break;
                            default:
                                Console.ForegroundColor = ConsoleColor.Red;
                                break;
                        }
                        Console.Write($"|{Space}{tile.BombCount}{Space}|");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.Write($"|{Space} {Space}|");
                    }
                }
                else 
                {
                    Console.Write($"|{Space}X{Space}|");
                }
            }

            Console.WriteLine();
        }
    }

    public static ConsoleColor HexToColor(string hex)
    {
        int number = Convert.ToInt32(hex, 16);
        ConsoleColor color = (ConsoleColor)number;
        return color;
    }
}
