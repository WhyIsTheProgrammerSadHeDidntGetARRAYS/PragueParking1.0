using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Prague_Parking1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Prague Parking";
            ParkingHouse.Menu();
        }
    }
    class ParkingHouse
    {
        public static string[] parkingSpots = new string[100];

        
        //Main menyn
        public static void Menu()
        {
            while (true)
            {
                Console.Clear();
                Art();
                Console.WriteLine("Choose something from the menu below, by simply typing in the number\n" +
                    "that corresponds to your desired option, and hit 'Enter'\n" +
                                  "     \n" +
                                  "[1] Park vehicle\n" +
                                  "[2] Move vehicle\n" +
                                  "[3] Search for vehicle\n" +
                                  "[4] Quick overview of parkinglot\n" +
                                  "[5] Remove vehicle\n" +
                                  "[6] Optimize double parking\n" +
                                  "[7] Quit");

                string userInput = Console.ReadLine().ToUpper();
                switch (userInput)
                {
                    case "1":
                        AddVehicle();
                        Console.ReadKey();
                        break;

                    case "2":
                        Move();
                        Console.ReadKey();
                        break;

                    case "3":
                        Search();
                        Console.ReadKey();
                        break;

                    case "4":
                        Print();
                        Console.ReadKey();
                        break;

                    case "5":
                        RemoveVehicle();
                        break;

                    case "6":
                        //OptimizeMotorcycles(parkingSpots);
                        DoubleParkAllMotorcycles();
                        break;

                    case "7":
                        Console.WriteLine("Closing down");
                        Environment.Exit(0);
                        break;

                    default:
                        Console.Clear();
                        Console.WriteLine("Pick something from the menu please!\n" +
                            "Hit any key to get back");
                        Console.ReadKey();
                        break;
                }
            }
        }

        
        public static void AddVehicle()
        {
            Console.Clear();
            Art();
            Console.Write("Please type in the registration number of your vehicle: ");
            string regNumber = Console.ReadLine().ToUpper();
            if (string.IsNullOrWhiteSpace(regNumber) || regNumber.Length <= 0 || regNumber.Length > 10)
            {
                Console.WriteLine("No valid input.");
                return;
            }

            //kollar så att inte regnumret redan finns när man lägger till fordon, finns det inte får jag tillbaks -1
            int doesVehicleExist = SearchVehicle(regNumber);

            if (doesVehicleExist != -1)
            {
                Console.WriteLine("Vehicle already parked.");
                return;
            }

            // tar bort eventuella spaces emellan input på regnummer, ifall anvädaren är jävlig
            regNumber = regNumber.Replace(" ", "");
            string vehicleType = "";

            do
            {
                Console.Clear();
                Art();
                Console.WriteLine("Now specify what type of vehilce you want to park.\n(1 + Enter for car) (2 + Enter for motorcycle)\n\n" +
                 "[1] Car\n" +
                 "[2] Motorcycle");
                vehicleType = Console.ReadLine().ToUpper();

                if (vehicleType == "1")
                {
                    AddCar(regNumber);
                    break;
                }

                else if (vehicleType == "2")
                {
                    //int mcPos = 
                    AddMc(regNumber);
                    //Console.WriteLine("Proceed to parkingspot {0}", mcPos + 1);
                    break;
                }

                else
                {
                    Console.WriteLine("Please specify what vehicle you want to park to move on..");
                    Console.ReadKey();
                }

            } while (vehicleType != "1" || vehicleType != "2");
        }

        
        public static void AddMc(string regNumber)
        {
            string mc = regNumber.Insert(0, "MC#");

            for (int i = 0; i < parkingSpots.Length; i++)
            {
                if (parkingSpots[i] == null)
                {
                    parkingSpots[i] = "MC#" + regNumber;
                    Console.WriteLine("Arrival {0}\n" +
                    "Please proceed to parking window {1}", DateTime.Now, i + 1);
                    return;
                }
                else if (!parkingSpots[i].Contains("|") && !parkingSpots[i].Contains("CAR#")) // kollar att parkeringsplatsen inte innehåller skiljetecknet eller en bil
                {
                    string temp = parkingSpots[i].Insert(parkingSpots[i].Length, "|");
                    parkingSpots[i] = temp + mc;
                    Console.WriteLine("You will be sharing the same spot as another motorcycle at spot: {0}\n" +
                        "Arrival: {1}", i + 1, DateTime.Now);
                    return;
                }
            }
            Console.WriteLine("The parkinglot is full, unfortunately");
        }

        
        public static void AddCar(string regNumber)
        {
            
            for (int i = 0; i < parkingSpots.Length; i++)
            {
                if (string.IsNullOrEmpty(parkingSpots[i]))
                {
                    parkingSpots[i] = "CAR#" + regNumber;
                    Console.WriteLine("Arrival {0}\n" +
                        "Please proceed to parking window {1}", DateTime.Now, i + 1);
                    return;
                }
            }
            Console.WriteLine("Not spots available, sorry!");
        }

       
        // flyttar fordon
        public static void Move()
        {
            Console.Clear();
            Art();
            
            Console.WriteLine("Type in registrationnumber of vehicle to move");
            string regNumber = Console.ReadLine().ToUpper();
            // metoden SearchVehicle tar fram index där regnumret står
            int vehiclePosition = SearchVehicle(regNumber);

            // kollar ifall det finns i arrayen, finns det inte ger min metod tillbaks -1
            if (vehiclePosition == -1) { Console.WriteLine("Vehcile not found"); return; }
            //tar fram hela fordenet med fordonstyp
            string identifier = IdentifyVehicleType(regNumber, vehiclePosition); 

            Console.WriteLine("Give me the position \"(1-100)\" you'd like to move too.");

            int pos = 0;
            try
            {
                pos = int.Parse(Console.ReadLine());
            }
            catch (Exception e)
            {
                Console.Clear();
                Console.WriteLine(e.Message + "\n" +
                    "Please make sure to use numbers between '1-100' to indicate what parkingwindow\n" +
                    "you'd like to move your vehicle too.");
                return;
            }
            if (pos > parkingSpots.Length) { Console.WriteLine("That number is way too high. Keep it between 1-100. Try again later"); return; }
            //hindrar användare från att försöka parkera på samma plats som den redan står
            if (pos - 1 == vehiclePosition) { Console.Clear(); Console.WriteLine("You are already on that spot. Try again"); return; }

            //flytta ensamstående mc eller bil
            if (parkingSpots[vehiclePosition] == identifier)
            {
                //kolla om platsen att flytta till är tom
                if (EmptySpot(pos - 1) == true)
                {
                    parkingSpots[pos - 1] = identifier;
                    parkingSpots[vehiclePosition] = null;
                    Console.WriteLine("Your vehicle has been moved from parkingwindow {0} -> {1}", vehiclePosition + 1, pos);
                    return;
                }
                //kolla om en till mc får plats på platsen vi vill stå, då får identifieraren inte innehålla fordonsbeteckningen för bil
                else if (CheckForDoubleParkingMc(pos - 1) == true && !identifier.Contains("CAR#"))
                {
                    parkingSpots[pos - 1] += "|" + identifier;
                    parkingSpots[vehiclePosition] = null;
                    Console.WriteLine("Your vehicle has been moved from parkingwindow {0} -> {1}", vehiclePosition + 1, pos);
                    return;
                }
                Console.WriteLine("No room to park there. Try another spot.");
                return;
            }
            //flytta en mc som står dubbelparkerad
            else if (parkingSpots[vehiclePosition] != null && parkingSpots[vehiclePosition].Contains("|"))
            {
                string[] temp = parkingSpots[vehiclePosition].Split('|');

                //kolla om platsen vi vill stå på är tom
                if (EmptySpot(pos - 1) == true)
                {
                    parkingSpots[pos - 1] = identifier;
                    //kollar vilket av elementen i den splittade arrayen som matchar värdet vi vill flytta på
                    //och skriv över platsen den tidigare stod på med de andra värdet
                    if (temp[0] == identifier)
                    {
                        parkingSpots[vehiclePosition] = temp[1];
                    }
                    else if (temp[1] == identifier)
                    {
                        parkingSpots[vehiclePosition] = temp[0];
                    }
                    Console.WriteLine("Moved your vehicle from parkingwindow {0} -> {1}", vehiclePosition + 1, pos);
                    return;
                }
                //kolla om en till mc får plats på platsen vi vill stå
                else if (CheckForDoubleParkingMc(pos - 1) == true)
                {
                    //lägger till ett skiljetecken och värdet(fordonet) på platsen vi vill flytta till
                    parkingSpots[pos - 1] += "|" + identifier;

                    //samma som ovan skriver jag över platsen där fordonet vi flyttade på stod innan, till endast det andra värdet som stod där innan
                    if (temp[0] == identifier)
                    {
                        parkingSpots[vehiclePosition] = temp[1];
                    }
                    else if (temp[1] == identifier)
                    {
                        parkingSpots[vehiclePosition] = temp[0];
                    }
                    Console.WriteLine("Moved your vehicle from parkingwindow {0} -> {1}", vehiclePosition + 1, pos);
                    return;
                }
                Console.WriteLine("No room to park your vehicle there.");
            }
        }

        
        // search - user interaction
        public static void Search()
        {
            Console.Clear();
            Art();
            Console.Write("Give me a registration number to look for: ");
            string regNumber = Console.ReadLine().ToUpper();
            int temp = SearchVehicle(regNumber);

            if (temp == -1)
            {
                Console.WriteLine("{0} was not found", regNumber);
                return;
            }
            Console.WriteLine("Vehicle with registration number {0} was found at spot: {1}", regNumber, temp + 1);
        }
        
        
        // search logic, returnerar index där ett visst fordon står
        public static int SearchVehicle(string regNumber)
        {
            for (int i = 0; i < parkingSpots.Length; i++)
            {
                if (parkingSpots[i] == "MC#" + regNumber || parkingSpots[i] == "CAR#" + regNumber)
                {
                    return i;
                }
                // dubbelparkering = söker efter en substring av regnummer
                //hittar jag substrängen av regnummer så måste jag splitta båda motorcyklarna från skiljetecknet, i en temporär array 
                //för att kunna kolla hela "värdet"
                else if (parkingSpots[i] != null && parkingSpots[i].Contains("MC#" + regNumber) && parkingSpots[i].Contains("|"))
                {
                    string[] temp = parkingSpots[i].Split('|');

                    if (temp[0] == "MC#" + regNumber)
                    {
                        return i;
                    }
                    else if (temp[1] == "MC#" + regNumber)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
        
        
        // kollar ifall angiven parkeringsplats är tom
        public static bool EmptySpot(int pos)//, string regNumber)
        {
            if (parkingSpots[pos] == null) 
            {
                return true;
            }
            //else if()
            return false;
        }
        
        //kollar ifall en till motorcykel får plats på angiven parkeringsplats
        public static bool CheckForDoubleParkingMc(int index)
        {
            if (parkingSpots[index] != null && !parkingSpots[index].Contains("|") && !parkingSpots[index].Contains("CAR#"))
            {
                return true;
            }
            return false;
        }
        
        
        //returnerar hela regnumret med fordonstyp ex. "CAR#ABC123" eller "MC#ABC321" som står på ett visst index
        public static string IdentifyVehicleType(string regNumber, int index)
        {
            if (parkingSpots[index] == "CAR#" + regNumber || parkingSpots[index] == "MC#" + regNumber)
            {
                return parkingSpots[index];
            }
            else if (parkingSpots[index] != null && parkingSpots[index].Contains("|"))
            {
                string[] temp = parkingSpots[index].Split('|');
                if (temp[0] == "MC#" + regNumber)
                {
                    return temp[0];
                }
                return temp[1];
            }
            return "";
        }

       
        //ena "brute force" metoden för att optimera motorcyklar, som både söker efter ensamma MC, och slänger ihop dem(använder ej)
        public static void OptimizeMotorcycles(string[] array)
        {
            //Vi går igenom arrayen med en low och high pointer, som båda fortsätter gå inåt i ledet tills de "möts"
            int low = 0;
            int high = array.Length - 1;
            Console.Clear();
            Art();

            while (low < high)
            {
                //letar efter en ensamstående mc från början av arrayen, hittar vi inte det så flyttar jag pointern inåt(se else)
                if (array[low] != null && array[low].Contains("MC#") && !array[low].Contains("|"))
                {
                    //letar efter en ensamstående mc från slutet av arrayen, hittar jag en ensamstående mc här så tar jag det "värdet",
                    //och "lägger på" det med ett skiljetecken där vi hittade en ensam mc på low-pointern
                    //hittar vi inte det så flyttar jag pointern inåt(se else)
                    if (array[high] != null && array[high].Contains("MC#") && !array[high].Contains("|"))
                    {
                        string tempHigh = array[high];
                        //string tempLow = array[low];
                        Console.WriteLine("Moved vehicle {0}, from window {1} to window: {2}", array[high], high + 1, low + 1);
                        array[high] = null;
                        array[low] += "|" + tempHigh;
                    }
                    else
                    {
                        high--;
                    }
                }
                else
                {
                    low++;
                }
            }
            Console.WriteLine("Optimization is completed. Press any key to get back to the menu.");
            Console.ReadKey();
        }
        
        
        //andra och bättre metoden för att dubbelparkera motorcyklar, där vi också har en low och high pointer, och rör oss inåt på vardera sida
        static void DoubleParkAllMotorcycles()
        {
            //arrayen temp bär alla index ifrån parkeringsarrayen där ensamstående motorcyklar står
            int[] temp = FindIndexOfEmptyMc(parkingSpots);
            int low = 0;
            int high = temp.Length - 1;
            if(temp.Length <= 1)
            {
                Console.WriteLine("Motorcycles are already optimized!");
                Console.ReadKey();
                return;
            }
            while(low < high)
            {
                //flyttar över index från the high end ifrån temp array till low end(sedan ökar vi low pointer, och minskar high pointer) 
                Console.WriteLine("{0} gets joined by {1} at position {2}", parkingSpots[temp[low]], parkingSpots[temp[high]]);
                parkingSpots[temp[low]] += "|" + parkingSpots[temp[high]];
                parkingSpots[temp[high]] = null;
                low++;
                high--;
            }
            Console.WriteLine("Double parked as many motorcycles as possible!");
            Console.ReadKey();
        }

        //tar fram index där ensamma motorcyklar står, och returnerar deras index i en int array av rätt storlek
        public static int[] FindIndexOfEmptyMc(string[] array)
        {
            int size = SizeOfArrayForHoldingEmptyMcIndex(parkingSpots);
            int[] temp = new int[size];
            int index = 0;

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] != null && array[i].Contains("MC#") && !array[i].Contains("|"))
                {
                    temp[index++] = i;
                }
            }
            return temp;
        }
        
        
        //en hjälpmetod till FindIndexOfEmptyMc som bestämmer storleken på arrayen jag ska returnera i just FindIndexOfEmptyMc
        //onödigt att ha en temporär array i samma storlek som min parkeringsarray(blev problem då)
        static int SizeOfArrayForHoldingEmptyMcIndex(string[] array)
        {
            int counter = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] != null && array[i].Contains("MC#") && !array[i].Contains("|"))
                {
                    counter++;
                }
            }
            return counter;
        }

       
        // tar bort specifikt fordon
        public static void RemoveVehicle()
        {
            Console.Clear();
            Art();
            Console.WriteLine("Type in registration number to remove: ");
            string regNumber = Console.ReadLine().ToUpper();
            int regPos = SearchVehicle(regNumber);
            
            if (regPos == -1) 
            { 
                Console.WriteLine("Vehicle with registraingnumber: \"{0}\" is not present.\n" +
                    "Therefor you cannot remove it from the parkinglot.", regNumber); 
                Console.ReadKey(); 
                return; 
            }
            string vehicleToRemove = IdentifyVehicleType(regNumber, regPos);
            //kollar först om fordonet vi vill hämta ut är ensamstående
            if(parkingSpots[regPos] == vehicleToRemove)
            {
                Console.WriteLine("Vehicle {0} has succeccfully been checked out, thanks for visiting!", vehicleToRemove);
                parkingSpots[regPos] = null;
                Console.ReadKey();
                return;
            }
            //kommer vi hit måste det vara en dubbelparkering
            string[] temp = parkingSpots[regPos].Split("|");

            if (temp[0] == vehicleToRemove)
            {
                parkingSpots[regPos] = temp[1];
            }
            else if (temp[1] == vehicleToRemove)
            {
                parkingSpots[regPos] = temp[0];
            }
            Console.WriteLine("Vehicle {0} has succeccfully been checked out, thanks for visiting!", vehicleToRemove);
            Console.ReadKey();
        }

       
        // skriver ut fordon som står parkerade
        public static void Print()
        {
            Console.Clear();
            LoadingPercentage();
            Console.Clear();

            int counter = 0;
            for (int i = 0; i < parkingSpots.Length; i++)
            {
                if (string.IsNullOrEmpty(parkingSpots[i]))
                {
                    counter++;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Parking window: {0}\n" +
                        "Vehicle(s) parked: {1}", i + 1, parkingSpots[i]);
                    Console.WriteLine("------------------------------");
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nAvailable slots {0}\n", counter);
            Console.WriteLine("------------------------------------------------");
            string choice = "";
            do
            {
                Console.WriteLine("Would you like to see a list of the empty slots?\n" +
                "Type y for yes, n to get back to the menu\n" +
                "[Y]\n" +
                "[N]");

                choice = Console.ReadLine().ToUpper();
                if (choice == "Y")
                {
                    PrintEmptySlots();
                    Console.ResetColor();
                    break;
                }
                else if (choice == "N")
                {
                    Console.WriteLine("As you please, processing backtracking...");
                    Loading();
                    Console.Clear();
                    Console.WriteLine("Press any key to get back to the menu.");
                    Console.ResetColor();
                    return;
                }
                else
                {
                    Console.WriteLine("Please type 'y' or 'n'");
                    Console.Clear();
                }
            } while (choice != "Y" || choice != "N");
            
        }
        static void PrintEmptySlots()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            for (int i = 0; i < parkingSpots.Length; i++)
            {
                if(parkingSpots[i] == null)
                {
                    Console.WriteLine("Empty slot: {0}", i + 1);
                }
            }
            Console.WriteLine("Press any key to get back.");
            Console.ResetColor();
        }
        
        static void Art()
        {
            
            Console.WriteLine(@"
  _____                              _____           _    _              
 |  __ \                            |  __ \         | |  (_)             
 | |__) | __ __ _  __ _ _   _  ___  | |__) |_ _ _ __| | ___ _ __   __ _  
 |  ___/ '__/ _` |/ _` | | | |/ _ \ |  ___/ _` | '__| |/ / | '_ \ / _` | 
 | |   | | | (_| | (_| | |_| |  __/ | |  | (_| | |  |   <| | | | | (_| | 
 |_|   |_|  \__,_|\__, |\__,_|\___| |_|   \__,_|_|  |_|\_\_|_| |_|\__, | 
                   __/ |                                           __/ | 
                  |___/                                           |___/");
            Console.WriteLine("--------------------------------------------------------------------------------");
        }

        public static void Loading()
        {
            for (int i = 10; i >= 0; i--)
            {
                Console.Write($"\rLoading: {i}%   ");
                Thread.Sleep(200);
            }
        }

        // för "dramtisk laddning", för alla sånnna här system "laggar" 
        public static void LoadingPercentage()
        {
            for (int i = 0; i <= 100; i++)
            {
                Console.Write($"\rLoading: {i}%   ");
                Thread.Sleep(10);
            }
        }
    }
}
