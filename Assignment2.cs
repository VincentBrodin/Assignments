namespace Assignment2;

internal static class Program
{
    //SETTINGS
    private const string DefaultNotation = "1/3/5/7";
    
    
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
        int playerCount = GetInputInt("How many players? (0/1/2)", 0, 2);
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
                players[0] = new Player(GetInputString("Enter player 1 name", "Player 1"), false);
                players[1] = new Player("AI", true);
                break;
            }
            case 2:
            {
                players[0] = new Player(GetInputString("Enter player 1 name", "Player 1"), false);
                players[1] = new Player(GetInputString("Enter player 2 name", "Player 2"), false);
                break;
            }
        }

        Console.Clear();
        
        /*******************************************
         *                                         *
         *             Set Up Board                *
         *                                         *
         *******************************************/
        
        //Let the user choose the start notation
        string startNotation = DefaultNotation;
        Console.WriteLine("Do you want to enter your own start notation? (y/n)");
        string? input = Console.ReadLine();
        if (input == "y")
            startNotation = GetInputNotation("Enter your start notation. (E.g: 1/3/5/7)");
        
        
        //Init piles with start notation
        (int pileCount, int maxSticks) = BreakUpNotation(startNotation);
        
        Pile[] piles = new Pile[pileCount];
        for (int i = 0; i < piles.Length; i++)
        {
            piles[i] = new Pile(maxSticks);
        }
        
        //Apply start notation
        piles.SetNotation(startNotation);
        
        Console.Clear();
        
        /*******************************************
         *                                         *
         *              Game Loop                  *
         *                                         *
         *******************************************/
        while (true)
        {
            Player currentPlayer = players[currentPlayerIndex];
            //Console.Clear();
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
                chosenPile = GetInputInt("Pick a pile", 1, piles.Length) - 1;
                if(piles[chosenPile].Sticks == 0)
                    while (piles[chosenPile].Sticks == 0)
                    {
                        chosenPile = GetInputInt("Pick a pile", 1, piles.Length) - 1;
                    }
                //Print chosen pile
                Console.Write("Pile " + (chosenPile + 1) + ": ");
                piles[chosenPile].Print();
                
                //Get sticks
                stickAmount = GetInputInt("How many sticks do you want to take", 1, piles[chosenPile].Sticks);
                
            }

            piles[chosenPile].RemoveSticks(stickAmount);
            
            if (piles.All(pile => pile.Sticks == 0))
            {
                Console.WriteLine($"{currentPlayer.Name} Won :) Press any key to continue");
                Console.ReadKey();
                break;
            }
            
            currentPlayerIndex = (currentPlayerIndex + 1) % 2;
        }
    }
    
    /// <summary>
    /// Determines the optimal next move in a game of NIM
    /// </summary>
    /// <param name="notation">The notation for the board state on which the next move will be calculated.</param>
    /// <returns>The pile and amount of sticks that should be removed</returns>
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

    /// <summary>
    /// A class that holds the amount of sticks in a pile
    /// </summary>
    private class Pile
    {
        public int Sticks;

        /// <summary>
        /// Creates a pile with the given amount of sticks
        /// </summary>
        /// <param name="sticks"></param>
        public Pile(int sticks)
        {
            Sticks = sticks;
        }
        
        /// <summary>
        /// Prints the pile
        /// </summary>
        public void Print()
        {
            if (Sticks != 0)
            {
                for (int i = 0; i < Sticks; i++)
                {
                    Console.Write("|");
                }
                Console.Write($" ({Sticks})");
            }
            Console.WriteLine();
        }
        
        /// <summary>
        /// Removes sticks from the pile
        /// </summary>
        /// <param name="amount">The amount of sticks to be removed</param>
        public void RemoveSticks(int amount)
        {
            Sticks -= amount;
        }
    }
    
    /// <summary>
    /// A class that holds the name and if the player is an AI
    /// </summary>
    /// <param name="name">The players name</param>
    /// <param name="isAi">If the player is an AI</param>
    private class Player(string name, bool isAi)
    {
        public readonly string Name = name;
        public readonly bool IsAi = isAi;
    }

    #region Extensions
    /// <summary>
    /// Applies a notation to the piles
    /// </summary>
    /// <param name="piles">The piles the notation should be set to</param>
    /// <param name="notation">The notation the piles should be set to</param>
    private static void SetNotation(this Pile[] piles, string notation)
    {
        string[] states = notation.Split("/");
        for (int i = 0; i < states.Length; i++)
        {
            int sticks = int.Parse(states[i]);
            piles[i].Sticks = sticks;
        }
    }
    
    /// <summary>
    /// Gets the notation of the piles
    /// </summary>
    /// <param name="piles">The that should be converted to a notation</param>
    /// <returns>The notation for the given piles</returns>
    private static string GetNotation(this Pile[] piles)
    {
        int pileCount = piles.Length;
        string[] states = new string[pileCount];
        for (int i = 0; i < pileCount; i++)
        {
            states[i] = piles[i].Sticks.ToString();
        }
        
        string notation = string.Join("/", states);
        
        return notation;
    }
    
    /// <summary>
    /// Prints the given array of piles
    /// </summary>
    /// <param name="piles">The array of piles that should be printed</param>
    private static void Print(this Pile[] piles)
    {
        for (var i = 0; i < piles.Length; i++)
        {
            Pile pile = piles[i];
            Console.Write(i + 1 + ": ");
            pile.Print();
        }
    }
    #endregion
    
    /// <summary>
    /// Breaks up a notation into the amount of piles and the max amount of sticks in a pile
    /// </summary>
    /// <param name="notation">The notation to be broken up</param>
    /// <returns>The amount of piles, The max amount of sticks in one pile</returns>
    private static (int,int) BreakUpNotation(string notation)
    {

        string[] states = notation.Split("/");

        int piles = states.Length;

        int maxSticks = states.Select(int.Parse).Prepend(0).Max();
        Console.WriteLine(maxSticks);

        return (piles, maxSticks);
    }

    #region GetInputInt
    /// <summary>
    /// Gets an int from the user
    /// </summary>
    /// <param name="message">The message that will be displayed</param>
    /// <param name="min">The minimum amount that will be accepted</param>
    /// <param name="max">The maximum amount that will be accepted</param>
    /// <returns> </returns>
    private static int GetInputInt(string message, int min, int max)
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
    
    /// <summary>
    /// Gets an int from the user
    /// </summary>
    /// <param name="message">The message that will be displayed</param>
    /// <returns>The int from the user</returns>
    private static int GetInputInt(string message)
    {
        while (true)
        {
            Console.WriteLine(message);
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int result))
            {
                return result;
            }
        }
    }
    #endregion
    
    #region GetInputString
    /// <summary>
    /// Gets a string from the user
    /// </summary>
    /// <param name="message">The message that will be displayed</param>
    /// <param name="autoAssign">The string that will be used if user leaves empty or null</param>
    /// <returns>The string form the user</returns>
    private static string GetInputString(string message, string autoAssign)
    {
        Console.WriteLine(message);
        string? input = Console.ReadLine();
        
        if(string.IsNullOrEmpty(input))
            return autoAssign;
        return input;
    }
    
    /// <summary>
    /// Gets a string from the user
    /// </summary>
    /// <param name="message">The message that will be displayed</param>
    /// <returns>The string form the user</returns>
    private static string GetInputString(string message)
    {
        Console.WriteLine(message);
        string? input = Console.ReadLine();

        while (true)
        {
            if (!string.IsNullOrEmpty(input))
                return input;
        }
    }
    #endregion

    #region GetInputNotation
    /// <summary>
    /// Gets a notation from the user
    /// </summary>
    /// <param name="message">The message that will be displayed</param>
    /// <returns>A valid notation from the user</returns>
    private static string GetInputNotation(string message)
    {
        while (true)
        {
            Console.WriteLine(message);
            string? input = Console.ReadLine();
            if (ValidNotation(input, out string result))
            {
                return result;
            }
        }
    }
    
    /// <summary>
    /// Checks if a notation is valid
    /// </summary>
    /// <param name="input">The notation to be checked</param>
    /// <param name="notation">The valid notation</param>
    /// <returns>True if given notation is a valid notation, False if given notation is not valid</returns>
    private static bool ValidNotation(string? input, out string notation)
    {
        notation = "";
        if (input == null)
        {
            return false;
        }

        string[] states = input.Split("/");
        foreach (string state in states)
        {
            if (!int.TryParse(state, out int result))
            {
                return false;
            }

            if (result < 0)
            {
                return false;
            }

            notation += result + "/";
        }

        notation = notation.Remove(notation.Length - 1);
        return true;
    }
    #endregion
}
