using System;
using System.Collections.Generic;
using System.Threading;

namespace Prague_Parking1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Prague Parking 1.0";
            ParkingHouse.Menu();
        }
    }
    class ParkingHouse
    {
        public static string[] parkingSpots = new string[100];
        public static DateTime CheckIn { get; set; }
        public static void Menu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Choose something in the menu below\n" +
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
                        OptimizeDoubleParking();
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
            Console.Write("Please type in the registration number of your vehicle: ");
            string regNumber = Console.ReadLine().ToUpper();
            if (string.IsNullOrWhiteSpace(regNumber) || regNumber.Length < 0 || regNumber.Length > 10)
            {
                Console.WriteLine("No valid input.");
                return;
            }
            // tar bort eventuella spaces emellan, ifall anvädaren är jävlig
            regNumber = regNumber.Replace(" ", "");
            string vehicleType = "";

            do
            {
                Console.Clear();
                Console.WriteLine("Now specify what type of vehilce by simply typing in" +
                 " \"car\" or \"mc\"");
                vehicleType = Console.ReadLine().ToUpper();

                if (vehicleType == "CAR")
                {
                    AddCar(regNumber);
                    break;
                }

                else if (vehicleType == "MC")
                {
                    //int mcPos = 
                    AddMc(regNumber);
                    //Console.WriteLine("Proceed to parkingspot {0}", mcPos + 1);
                    //AddMc(regNumber);
                    break;
                }

                else
                {
                    Console.WriteLine("Please type in \"car\" or \"mc\" to move on..");
                    Console.ReadKey();
                }

            } while (vehicleType != "CAR" || vehicleType != "MC");
        }
        // tanke-> kanske ha detta som en static int, och returnera indexpositonen ett fordon parkeras på
        // då kan jag istället ha den platsen sparad i en variabel och slipper ha utskrifter här inne? kan skriva ut parkeringsplats
        // i min AddVehicle metod då, och endast ha logik här
        public static void AddMc(string regNumber)
        {
            string mc = regNumber.Insert(0, "MC#");

            for (int i = 0; i < parkingSpots.Length; i++)
            {
                if (parkingSpots[i] == null)
                {
                    parkingSpots[i] = "MC#" + regNumber;
                    Console.WriteLine("Arrival {0}\n" +
                    "Please proceed to parking window {1}", CheckIn, i + 1);
                    return;
                }
                else if (!parkingSpots[i].Contains("|") && !parkingSpots[i].Contains("CAR#"))
                {
                    string temp = parkingSpots[i].Insert(parkingSpots[i].Length, "|");
                    parkingSpots[i] = temp + mc;
                    Console.WriteLine("You will be sharing the same spot as another MC at spot: {0}\n", i + 1);
                    return;
                }
            }
            Console.WriteLine("The parkinglot is full, unfortunately");
            //return -1;
        }
        public static void AddCar(string regNumber)
        {
            for (int i = 0; i < parkingSpots.Length; i++)
            {
                if (string.IsNullOrEmpty(parkingSpots[i]))
                {
                    parkingSpots[i] = "CAR#" + regNumber;
                    CheckIn = DateTime.Now;
                    Console.WriteLine("Arrival {0}\n" +
                        "Please proceed to parking window {1}", CheckIn, i + 1);
                    return;
                }
            }
            Console.WriteLine("Not spots available, sorry!");
        }

        // move och search är EXTREMT lika.. borde kanske göra om dem, och använda dem "tillsammans" för vissa operationer
        public static void Move()
        {
            Console.WriteLine("Reg number of vehicle to move");
            string regNumber = Console.ReadLine().ToUpper();

            for (int i = 0; i < parkingSpots.Length; i++)
            {
                // singleparked vehicles
                if (parkingSpots[i] == "CAR#" + regNumber || parkingSpots[i] == "MC#" + regNumber)
                {
                    int pos = NextAvailableSlot();
                    Console.WriteLine("Your vehicle is moved to parking slot: {0}", pos + 1);
                    string temp = parkingSpots[i];
                    parkingSpots[i] = null;
                    parkingSpots[pos] = temp;
                    return;
                }
                // double parked vehicles (mc)
                else if (parkingSpots[i] != null && parkingSpots[i].Contains(regNumber))
                {
                    int newPosMc = NextAvailableSlot();
                    string mcPlaceHolder = "MC#" + regNumber;
                    string replace = parkingSpots[i].Replace("MC#" + regNumber, null);
                    parkingSpots[i] = replace;
                    parkingSpots[i] = parkingSpots[i].Replace("|", null);
                    parkingSpots[newPosMc] = mcPlaceHolder;
                    Console.WriteLine("Your vehicle is moved to parking slot: {0}", newPosMc + 1);
                    return;
                }
            }
            Console.WriteLine("Vehicle was not found!");

        }
        // returns index of first empty slot
        public static int NextAvailableSlot()
        {
            for (int i = 0; i < parkingSpots.Length; i++)
            {
                if (parkingSpots[i] == null)
                {
                    return i;
                }
            }
            return -1;
        }
        // flyttar ihop alla singelparkerade mc
        public static void OptimizeDoubleParking()
        {
            // ska flytta ihop MC som står ensamma tillsammans. Skiss på hur jag tänker att det ska gå till

            // if(parkingspot[i] != null && parkingSpots[i].Contains("MC#") && !parkingSpots[i].Contains("|")
            //         int temp = AddCar(parkingspots[i]);
            //          eller typ i = AddCar(parkingspots[i]);
            //         parkingSpots[i] = null;

            string[] tempArray = new string[parkingSpots.Length];
            int index = 0;
            for (int i = 0; i < parkingSpots.Length; i++)
            {
                if(parkingSpots[i] != null && parkingSpots[i].Contains("MC#") && !parkingSpots[i].Contains("|"))
                {
                    string temp = parkingSpots[i].Replace("MC#", null); // ABC123
                    tempArray[index++] = temp;
                    parkingSpots[i] = null;
                }
            }

            for (int i = 0; i < tempArray.Length; i++)
            {
                if(tempArray[i] != null)
                {
                    AddMc(tempArray[i]);
                    Console.Clear(); // "fulfix".. men "tar bort" utskrifterna från addmc i denna metod
                }
            }
            Console.WriteLine("Optimization completed.");
            Console.ReadKey();
        }
        
        // search logic, returnerar index där ett visst fordon står
        public static int SearchVehicle(string regNumber)
        {
            for (int i = 0; i < parkingSpots.Length; i++)
            {
                if(parkingSpots[i] == "MC#" + regNumber || parkingSpots[i] == "CAR#" + regNumber)
                {
                    return i;
                }
                else if(parkingSpots[i] != null && parkingSpots[i].Contains(regNumber))
                {
                    return i;
                }
            }
            return -1;
        }
        
        // search - user interaction
        public static void Search()
        {
            Console.Write("Give me a registration number to look for: ");
            string regNumber = Console.ReadLine().ToUpper();
            int temp = SearchVehicle(regNumber);

            if(temp == -1)
            {
                Console.WriteLine("{0} was not found", regNumber);
                return;
            }
            Console.WriteLine("Vehicle with registration number {0} was found at spot: {1}", regNumber, temp + 1);
        }
        public static void RemoveVehicle()
        {
            Console.WriteLine("Type in registration number to remove: ");
            string regNumber = Console.ReadLine().ToUpper();
            int regPos = SearchVehicle(regNumber);

            if (regPos == -1) { Console.WriteLine("Vehicle not found"); return; }

            if(parkingSpots[regPos] == "CAR#" + regNumber || parkingSpots[regPos] == "MC#" + regNumber)
            {
                // checka ut/ta bort singelparkerat fordon
                parkingSpots[regPos] = null;
                Console.WriteLine("Vehicle {0} has been checked out", regNumber);
                Console.ReadKey();
                return;
            }
            else if(parkingSpots[regPos] != null && parkingSpots[regPos].Contains(regNumber))
            {
                // checka ut/ta bort dubbelparkerat fordon
                // Console.WriteLine("Vi tog oss hit för dubbelparking");
                string replace = parkingSpots[regPos].Replace("MC#" + regNumber, null);
                parkingSpots[regPos] = replace;
                parkingSpots[regPos] = parkingSpots[regPos].Replace("|", null);
                Console.ReadKey();
                return;
            }
        }
        public static void Print()
        {
            Console.Clear();
            LoadingPercentage();
            Console.WriteLine("\n");

            int counter = 0;
            for (int i = 0; i < parkingSpots.Length; i++)
            {
                if (string.IsNullOrEmpty(parkingSpots[i]))
                {
                    counter++;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Parking window: {0} is occupied!\n" +
                        "Vehicle(s) parked: {1}", i + 1, parkingSpots[i]);
                    Console.WriteLine("-----------------------------------");
                }

            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n\nSpots available: {0}", counter);
            Console.ResetColor();
        }

        // för "dramtisk laddning", för vilket sånt här system "laggar" inte?
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
