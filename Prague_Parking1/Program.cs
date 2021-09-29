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
                                  "[6] Quit");

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
                    int mcPos = AddMc(regNumber);
                    Console.WriteLine("Proceed to parkingspot {0}", mcPos + 1);
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
        public static int AddMc(string regNumber)
        {
            string mc = regNumber.Insert(0, "MC#");

            for (int i = 0; i < parkingSpots.Length; i++)
            {
                if (parkingSpots[i] == null)
                {
                    parkingSpots[i] = "MC#" + regNumber;
                    CheckIn = DateTime.Now;
                    //Console.WriteLine("Arrival {0}\n" +
                    //"Please proceed to parking window {1}", CheckIn, i + 1);
                    return i;
                }
                else if (!parkingSpots[i].Contains("|") && !parkingSpots[i].Contains("CAR#"))
                {
                    string q = parkingSpots[i].Insert(parkingSpots[i].Length, "|");
                    parkingSpots[i] = q + mc;
                    //Console.WriteLine("You will be sharing the same spot as another MC at spot: {0}\n", i + 1);
                    return i;
                }
            }
            return -1;
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
                // singelparkerade fordon
                if (parkingSpots[i] == "CAR#" + regNumber || parkingSpots[i] == "MC#" + regNumber)
                {
                    int pos = NextEmptySlot();
                    Console.WriteLine("Your vehicle is moved to parking slot: {0}", pos + 1);
                    string temp = parkingSpots[i];
                    parkingSpots[i] = null;
                    parkingSpots[pos] = temp;
                    return;
                }
                // dubbelparkerade fordon (MC)
                else if (parkingSpots[i] != null && parkingSpots[i].Contains(regNumber))
                {
                    int newPosMc = NextEmptySlot();
                    string mcPlaceHolder = "MC#"+ regNumber;
                    string replace = parkingSpots[i].Replace("MC#" + regNumber, "");
                    parkingSpots[i] = replace;
                    parkingSpots[i] = parkingSpots[i].Replace("|", "");
                    parkingSpots[newPosMc] = mcPlaceHolder;
                    Console.WriteLine("Your vehicle is moved to parking slot: {0}", newPosMc + 1);
                    return;
                    // logik för att flytta mc
                }
            }

        }
        // hittar första tomma plats, och retunerar dess index
        public static int NextEmptySlot()
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
        public static void OptimizeDoubleParking()
        {
            // ska flytta ihop MC som står ensamma tillsammans. Skiss på hur jag tänker att det ska gå till

            // if(parkingspot[i] != null && parkingSpots[i].Contains("MC#") && !parkingSpots[i].Contains("|")
            //         int temp = AddCar(parkingspots[i]);
            //          eller typ i = AddCar(parkingspots[i]);
            //         parkingSpots[i] = null;
        }

        public static void Search()
        {
            Console.WriteLine("Give me your registration number: ");
            string regNumber = Console.ReadLine().ToUpper();

            for (int i = 0; i < parkingSpots.Length; i++)
            {
                // går första if igenom vet vi att det är singelparkering
                if (parkingSpots[i] == "CAR#" + regNumber || parkingSpots[i] == "MC#" + regNumber)
                {
                    Console.WriteLine("Vehicle {0} Found at parkingspot: {1}", regNumber, i + 1);
                    return;
                }
                // annars kollar jag efter "sekvenser av fordon" aka om jag hittar sekvensen av regnumer, utan fordonsindentifiering
                // i så fall vet jag att det står mer än ett fordon på platsen
                else if (parkingSpots[i] != null && parkingSpots[i].Contains(regNumber))
                {
                    Console.WriteLine("Your vehicle is double-parked at spot {0}: {1}", i + 1, parkingSpots[i]);
                    return;
                }
            }
            Console.WriteLine("Not found");
        }
        public static void RemoveVehicle()
        {
            Console.WriteLine("Type in registration number to remove: ");
            string regNumber = Console.ReadLine().ToUpper();

            for (int i = 0; i < parkingSpots.Length; i++)
            {
                if (parkingSpots[i] == "CAR#" + regNumber || parkingSpots[i] == "MC#" + regNumber)
                {
                    Console.WriteLine("Vehicle {0} has checked out at {1}", regNumber, DateTime.Now);
                    parkingSpots[i] = null;
                    Console.WriteLine("Press any key to Get back to the menu");
                    Console.ReadKey();
                    return;
                }
            }
            Console.WriteLine("Vehicle was not found");
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
