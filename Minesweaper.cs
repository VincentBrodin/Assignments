namespace Assignment2;

internal static class Program
{
    //SETTINGS
    private const string StartNotation = "1/3/5/7";
    
    
    private static void Main(string[] args)
    {
        //Get players
        int playerCount = GetInput("How many players? (1/2)", 1, 2);
        int currentPlayerIndex = 0;
        Player[] players = new Player[2];
        if(playerCount == 1)
        {
            Console.WriteLine("Enter player 1 name");
            string player1 = Console.ReadLine() ?? "Player 1";
            players[0] = new Player(player1, false);
            players[1] = new Player("AI", true);
        }
        else
        {
            Console.WriteLine("Enter player 1 name");
            string player1 = Console.ReadLine() ?? "Player 1";
            Console.WriteLine("Enter player 2 name");
            string player2 = Console.ReadLine() ?? "Player 2";
            
            players[0] = new Player(player1, false);
            players[1] = new Player(player2, false);
        }
        
        Console.Clear();
        
        //Get start notation
        string startNotation = StartNotation;
        Console.WriteLine("Do you want to enter your own start notation? (y/n)");
        string? input = Console.ReadLine();
        if (input == "y")
        {
            Console.WriteLine("Enter your start notation");
            startNotation = Console.ReadLine() ?? StartNotation;
        }
        
        
        //Init piles
        (int pileCount, int maxSticks) = BreakUpNotation(startNotation);
        
        Pile[] piles = new Pile[pileCount];
        for (int i = 0; i < piles.Length; i++)
        {
            piles[i] = new Pile(maxSticks);
            for (int j = 0; j < piles[i].Sticks.Length; j++)
            {
                piles[i].Sticks[j] = new Stick(true);
            }
        }
        
        //Apply start notation
        piles.SetNotation(startNotation);
        
        Console.Clear();
        //Game Loop
        while (true)
        {
            Player currentPlayer = players[currentPlayerIndex];
            Console.Clear();
            Console.WriteLine(currentPlayer.Name + "'s turn");
            piles.Print();

            int chosenPile = 0, stickAmount = 0;
            if (currentPlayer.IsAi)
            {
                (chosenPile, stickAmount) = CalculateAiMove(piles.GetNotation());
            }
            else
            {
                //Get pile
                chosenPile = GetInput("Pick a pile", 1, piles.Length) - 1;
            
                //Get sticks
                stickAmount = GetInput("Pick a number of sticks", 1, piles[chosenPile].Sticks.Count(stick => !stick.IsTaken));
                
            }

            piles[chosenPile].RemoveSticks(stickAmount);
            
            if (piles.All(pile => pile.Sticks.All(stick => stick.IsTaken)))
            {
                break;
            }
            
            currentPlayerIndex = (currentPlayerIndex + 1) % 2;
        }
    }

    private static (int,int) CalculateAiMove(string notation)
    {
        //TODO: Implement AI
        return (0, 0);
    }

    private class Stick(bool isTaken)
    {
        public bool IsTaken = isTaken;
    }
    
    private class Pile(int maxSticks)
    {
        public readonly Stick[] Sticks = new Stick[maxSticks];

        public void Print()
        {
            foreach (var stick in Sticks)
            {
                if(!stick.IsTaken)
                    Console.Write("|");
            }
            Console.WriteLine();
        }
        
        public void RemoveSticks(int amount)
        {
            int taken = 0;
            foreach (var stick in Sticks)
            {
                if (!stick.IsTaken)
                {
                    stick.IsTaken = true;
                    taken++;
                }
                
                if(taken == amount)
                    return;
            }
        }
    }
    
    private class Player(string name, bool isAi)
    {
        public readonly string Name = name;
        public bool IsAi = isAi;
    }

    #region Extensions
    private static void SetNotation(this Pile[] piles, string notation)
    {
        string[] states = notation.Split("/");
        for (int i = 0; i < states.Length; i++)
        {
            int sticksCount = int.Parse(states[i]);
            for (int j = 0; j < sticksCount; j++)
            {
                piles[i].Sticks[j].IsTaken = false;
            }
        }
    }
    
    private static string GetNotation(this Pile[] piles)
    {
        int pileCount = piles.Length;
        string[] states = new string[pileCount];
        for (int i = 0; i < pileCount; i++)
        {
            states[i] = piles[i].Sticks.Count(stick => !stick.IsTaken).ToString();
        }
        
        string notation = string.Join("/", states);
        
        return notation;
    }
    
    private static void Print(this Pile[] piles)
    {
        for (var i = 0; i < piles.Length; i++)
        {
            var pile = piles[i];
            Console.Write(i + 1 + ": ");
            pile.Print();
        }
    }
    #endregion
    
    private static (int,int) BreakUpNotation(string notation)
    {

        string[] states = notation.Split("/");

        int piles = states.Length;

        int maxSticks = states.Select(int.Parse).Prepend(0).Max();

        return (piles, maxSticks);
    }
    
    private static int GetInput(string message, int min, int max)
    {
        while (true)
        {
            Console.WriteLine(message);
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int result))
            {
                if (result >= min && result <= max)
                {
                    return result;
                }
            }
        }
    }


    
}

