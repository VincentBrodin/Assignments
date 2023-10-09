namespace Assignment2;

internal static class Program
{
    //SETTINGS
    private const string StartNotation = "1/3/5/7";
    
    
    private static void Main(string[] args)
    {
        //Rules
        Console.WriteLine("Welcome to Nim!");
        Console.WriteLine("The rules are simple:");
        Console.WriteLine("There are X piles of sticks");
        Console.WriteLine("Each player takes turns taking sticks from a pile");
        Console.WriteLine("You can only take sticks from one pile per turn");
        Console.WriteLine("You can only take 1 to N sticks from a pile");
        Console.WriteLine("The player that takes the last stick wins");
        //Get players
        int playerCount = GetInput("How many players? (0/1/2)", 0, 2);
        int currentPlayerIndex = 0;
        Player[] players = new Player[2];
        switch (playerCount)
        {
            case 0:
                players[0] = new Player("AI1", true);
                players[1] = new Player("AI2", true);
                break;
            case 1:
            {
                Console.WriteLine("Enter player 1 name");
                string player1 = Console.ReadLine() ?? "Player 1";
                players[0] = new Player(player1, false);
                players[1] = new Player("AI", true);
                break;
            }
            case 2:
            {
                Console.WriteLine("Enter player 1 name");
                string player1 = Console.ReadLine() ?? "Player 1";
                Console.WriteLine("Enter player 2 name");
                string player2 = Console.ReadLine() ?? "Player 2";
            
                players[0] = new Player(player1, false);
                players[1] = new Player(player2, false);
                break;
            }
        }

        Console.Clear();
        
        //Get start notation
        string startNotation = StartNotation;
        Console.WriteLine("Do you want to enter your own start notation?(E.g: 1/3/5/7) (y/n)");
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

            int chosenPile, stickAmount;
            if (currentPlayer.IsAi)
            {
                Console.WriteLine($"{currentPlayer.Name} is thinking...");
                (chosenPile, stickAmount) = DetermineOptimalNimMove(piles.GetNotation());
                Console.WriteLine($"{currentPlayer.Name} chose pile {chosenPile+1} and took {stickAmount} sticks. Press any key to continue");
                Console.ReadKey();
            }
            else
            {
                //Get pile
                chosenPile = GetInput("Pick a pile", 1, piles.Length) - 1;
                if(piles[chosenPile].SticksLeft() == 0)
                    while (piles[chosenPile].SticksLeft() == 0)
                    {
                        chosenPile = GetInput("Pick a pile", 1, piles.Length) - 1;
                    }
                //Print chosen pile
                Console.Write("Pile " + (chosenPile + 1) + ": ");
                piles[chosenPile].Print();
                
                //Get sticks
                stickAmount = GetInput("How many sticks do you want to take", 1, piles[chosenPile].SticksLeft());
                
            }

            piles[chosenPile].RemoveSticks(stickAmount);
            
            if (piles.All(pile => pile.Sticks.All(stick => stick.IsTaken)))
            {
                Console.WriteLine($"{currentPlayer.Name} Won :) Press any key to continue");
                Console.ReadKey();
                break;
            }
            
            currentPlayerIndex = (currentPlayerIndex + 1) % 2;
        }
    }
    
    private static (int,int) DetermineOptimalNimMove(string notation)
    {
        // Calculate the nim-sum of the piles in O(n) 
        string[] states = notation.Split("/");
        int[] pilesOfSticks = Array.ConvertAll(states, int.Parse);
        int nimSum = 0;
        foreach (int count in pilesOfSticks)
        {
            nimSum ^= count;
        }
        for (int i = 0; i < pilesOfSticks.Length; i++)
        {
            int desiredCount = pilesOfSticks[i] ^ nimSum;
            if (desiredCount < pilesOfSticks[i])
            {
                return (i, pilesOfSticks[i] - desiredCount);
            }
        }

        // If no move can make the desired nim-sum, just make a valid move
        for (int i = 0; i < pilesOfSticks.Length; i++)
        {
            if (pilesOfSticks[i] > 0)
            {
                return (i, 1);
            }
        }

        // No valid move found (should not happen if game is not over)
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
        
        public int SticksLeft()
        {
            return Sticks.Count(stick => !stick.IsTaken);
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
        public readonly bool IsAi = isAi;
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
