namespace Assignment1;

internal abstract class Assignment1
{
    private class Boat
    {
        public string Character = "";
        public int CharacterIndex = -1;
    }
    private static int _currentBeach = 1;
    private static bool _holding;
    

    private static readonly Boat CurrentBoat = new Boat();
    
    private static readonly string[,] Beach = {
        {"" , "" , "" , ""},
        {"farmer", "wolf", "rabbit", "carrot"}  
    };
    
    private static void Main()
    {
        Console.WriteLine("Assignment 1");
        Space();
        Console.WriteLine("You are a farmer faced with a challenge.");
        Console.WriteLine("You have a wolf, a rabbit, and a carrot that you need to transport across a river.");
        
        Space();
        Console.WriteLine("Simple enough, right?");
        Space();
        
        Console.WriteLine("You have a boat that can only carry you and one other object or animal.");
        Console.WriteLine("You cannot leave the wolf alone with the rabbit or the rabbit alone with the carrot.");
        
        Space();
        
        Console.WriteLine("Here are the rules:");
        Console.WriteLine("Rule1: The boat can only carry one object or animal in addition to the farmer.");
        Console.WriteLine("Rule2: The wolf and the rabbit cannot be left alone together.");
        Console.WriteLine("Rule3: The rabbit and the carrot cannot be left alone together.");      
        Space();
        
        Console.WriteLine("Do you want to play or see solution? (p/s)");
        string? input = Console.ReadLine();
        if (input == "p")
        {
            Play();
        }
        else if (input == "s")
        {
            Solution();
        }
        else
        {
            Console.WriteLine("Invalid input");
        }
    }

    #region Modes
    private static void Play()
    {
        Console.Clear();
        
        while (true)
        {
            Space();
            Print();
            Console.WriteLine(_holding
                ? "Whats your next move? (Travel: t) (Place: p)"
                : "Whats your next move? (Travel: t) (Grab: index you want to grab)");
            string? input = Console.ReadLine();
            if (input == "")
            {
                Console.WriteLine("Invalid input");
                continue;
            }
            
            if (input == "t")
            {
                Move();
                if (!CheckIfValid(out string inValidReason))
                {
                    Space();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(inValidReason);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Do you want to restart? (y/n)");
                    string? restart = Console.ReadLine();
                    if (restart == "y")
                    { 
                        Main();
                    }
                    return;
                }
            }
            else if (input != null && char.IsDigit(input[0]) && !_holding)
            {
                int index = int.Parse(input) - 1;
                if(index is > 0 and < 4)
                {
                    GrabOrPlace(index);
                }
                else
                {
                    Console.WriteLine(index == 0 ? "You can't grab the farmer" : "Invalid input");
                }
            }
            else if (_holding && input == "p")
            {
                GrabOrPlace();
                if (CheckIfWin())
                {
                    Space();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("You won!");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Do you want to restart? (y/n)");
                    string? restart = Console.ReadLine();
                    if (restart == "y")
                    { 
                        Main();
                    }
                }
            }
            else
            {
                Console.WriteLine("Invalid input");
            }
        }
    }
    private static void Solution()
    {
        Console.Clear();
        Print();
        Space();

        GrabOrPlace(2);
        Move();
        GrabOrPlace();
        Move();
        Space();
        
        GrabOrPlace(1);
        Move();
        GrabOrPlace();
        Space();
        
        GrabOrPlace(2);
        Move();
        GrabOrPlace();
        Space();
        
        GrabOrPlace(3);
        Move();
        GrabOrPlace();
        Move();
        Space();
        
        GrabOrPlace(2);
        Move();
        GrabOrPlace();
        Space();
        
        Print();
    }
    #endregion

    #region Checks
    private static bool CheckIfValid(out string inValidReason)
    {
        for (int i = 0; i < Beach.GetLength(0); i++)
        {
            bool hasRabbit = false;
            bool hasWolf = false;
            bool hasCarrot = false;
            for (int j = 0; j < Beach.GetLength(1); j++)
            {
                switch (Beach[i, j])
                {
                    case "rabbit":
                        hasRabbit = true;
                        break;
                    case "wolf":
                        hasWolf = true;
                        break;
                    case "carrot":
                        hasCarrot = true;
                        break;
                }
            }

            if (hasRabbit && hasCarrot)
            {
                inValidReason = "The rabbit ate the carrot";
                return false;
            }

            if (hasRabbit && hasWolf)
            {
                inValidReason = "The wolf ate the rabbit";
                return false;
            }
        }

        inValidReason = string.Empty;
        return true;
    }

    private static bool CheckIfWin()
    {
        bool hasRabbit = false;
        bool hasWolf = false;
        bool hasCarrot = false;
        for (int j = 0; j < Beach.GetLength(1); j++)
        {
            switch (Beach[0, j])
            {
                case "rabbit":
                    hasRabbit = true;
                    break;
                case "wolf":
                    hasWolf = true;
                    break;
                case "carrot":
                    hasCarrot = true;
                    break;
            }
        }
        
        return hasWolf && hasRabbit && hasCarrot;
    }
    #endregion
    
    #region Moves
    private static void GrabOrPlace(int i)
    {
        _holding = true;
        CurrentBoat.Character = Beach[_currentBeach, i];
        CurrentBoat.CharacterIndex = i;
        Beach[_currentBeach, i] = "";
            
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.Write("\nFarmer ");
        Console.ForegroundColor = ConsoleColor.White;
            
        Console.Write("grabs ");
            
        Console.ForegroundColor = GetCharacterColor(CurrentBoat.Character);
        Console.Write(CurrentBoat.Character);
        Console.ForegroundColor = ConsoleColor.White;
            
        Console.Write($" from beach {_currentBeach + 1}");
    }
    private static void GrabOrPlace()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.Write("\nFarmer ");
        Console.ForegroundColor = ConsoleColor.White;
            
        Console.Write("places ");
            
        Console.ForegroundColor = GetCharacterColor(CurrentBoat.Character);
        Console.Write(CurrentBoat.Character);
        Console.ForegroundColor = ConsoleColor.White;
            
        Console.Write($" at beach {_currentBeach + 1}");
        
        _holding = false;
        Beach[_currentBeach, CurrentBoat.CharacterIndex] = CurrentBoat.Character;
        CurrentBoat.Character = "";
    }
    
    private static void Move()
    {
        Beach[_currentBeach, 0] = "";
        _currentBeach = _currentBeach == 0 ? 1 : 0;
        Beach[_currentBeach, 0] = "farmer";
        
        if (_holding)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("\nFarmer ");
            Console.ForegroundColor = ConsoleColor.White;
            
            Console.Write("travels with ");
            
            Console.ForegroundColor = GetCharacterColor(CurrentBoat.Character);
            Console.Write(CurrentBoat.Character);
            Console.ForegroundColor = ConsoleColor.White;
            
            Console.Write($" to beach {_currentBeach+1}");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("\nFarmer ");
            Console.ForegroundColor = ConsoleColor.White;

            Console.Write($"travels alone to beach {_currentBeach+1}");
        }
    }
    #endregion

    #region Other
    private static ConsoleColor GetCharacterColor(string boatCharacter)
    {
        switch (boatCharacter)
        {
            case "farmer":
                return ConsoleColor.Magenta;
            case "wolf":
                return ConsoleColor.Red;
            case "rabbit":
                return ConsoleColor.Green;
            case "carrot":
                return ConsoleColor.Yellow;
            default:
                return ConsoleColor.White;
        }
    }

    private static void Print()
    {
        for (int i = 0; i < Beach.GetLength(0); i++)
        {
            Console.WriteLine($"Beach {i+1}");
            for (int j = 0; j < Beach.GetLength(1); j++)
            {
                if (Beach[i, j] != "")
                {
                    Console.ForegroundColor = GetCharacterColor(Beach[i, j]);
                    Console.Write(Beach[i, j] + " ");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("Empty ");
                }
            }
            Console.WriteLine();
        }
    }
    
    private static void Space()
    {
        Console.WriteLine();
    }
    #endregion
}
