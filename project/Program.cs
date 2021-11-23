using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace project
{
    internal class Program
    {
        enum LoginStatus
        {
            Yes,
            Error,
            No
        }

        private const string stored_password = "SHARPSOUND";

        private static List<MP3Player> mp3players;

        private static LoginStatus login(string password)
        {
            if (password == stored_password)
            {
                return LoginStatus.Yes;
            }
            return LoginStatus.No;
        }

        private static LoginStatus loginTries()
        {
            const int max_attempts = 3;
            int count = 0;
            
            while (count < max_attempts)
            {
                if (count > 0)
                {
                    // 3 attempts made
                    Console.WriteLine("Attempt {0} out of {1}", count, max_attempts);
                }

                Console.WriteLine("Type in your password:");
                string password = Console.ReadLine();
                LoginStatus login = Program.login(password);

                switch (login)
                {
                    case LoginStatus.No:
                        Console.WriteLine("This password is incorrect");
                        break;
                    case LoginStatus.Yes:
                        Console.WriteLine("You are now logged in.");
                        break;
                    case LoginStatus.Error:
                        Console.WriteLine("Something went wrong, please type your password again.");
                        break;
                }

                if (login == LoginStatus.Yes)
                {
                    return LoginStatus.Yes;
                }
                count++;
            }

            return LoginStatus.No;
        }

        private static void showMenu()
        {
            Console.WriteLine("1. Overview mp3players\n2. Overview stock\n3. Mutate Stock\n4. Statistics\n5. Add mp3player\n9. Exit");

            // true makes it go on forever and ever and ever
            while (true)
            {
                // Read one character
                ConsoleKeyInfo menu = Console.ReadKey(true);

                switch (menu.KeyChar)
                {
                    case '1':
                        menu1();
                        break;
                    case '2':
                        menu2();
                        break;
                    case '3':
                        menu3();
                        break;
                    case '4':
                        menu4_maar_dan_anders();
                        break;
                    case '5':
                        menu5();
                        break;
                    case '9':
                        Environment.Exit(1);
                        break;
                }
            }
        }

        public static void menu1()
        {
            foreach(MP3Player mp3player in mp3players)
            {
                Console.WriteLine(
                    "ID: {0}, Make: {1}, Model: {2}, MB size: {3}, Price: €{4}", 
                    mp3player.id, 
                    mp3player.make,
                    mp3player.model,
                    mp3player.mbsize,
                    mp3player.price
                );
            }
        }

        public static void menu2()
        {
            foreach(MP3Player mp3player in mp3players)
            {
                Console.WriteLine(
                    "ID: {0}, Stock: {1}",
                    mp3player.id, 
                    mp3player.stock
                );
            }
        }
        public static void menu3()
        {
            int id;
            int amount_of_stock;
            
            Console.WriteLine("Please enter the ID of the mp3player");
            try
            {
                id = int.Parse(Console.ReadLine());
            }
            catch (Exception e)
            {
                Console.WriteLine("ID is invalid!");
                menu3();
                return;
            }

            Console.WriteLine("Please enter the new stock of the mp3player");
            try
            {
                amount_of_stock = int.Parse(Console.ReadLine());
            }
            catch (Exception e)
            {
                Console.WriteLine("Stock is invalid!");
                menu3();
                return;
            }

            // First Find looks for a match in the list mp3players, THEN it ends up in mp3match
            MP3Player selected_mp3player = mp3players.Find(mp3player => mp3player.id == id);

            // null = when it's empty, not found
            if (selected_mp3player == null)
            {
                Console.WriteLine("ID does not exist!");
                menu3();
                return;
            }
            
            Console.WriteLine("Current stock is {0}", selected_mp3player.stock);
            
            // add amount of stock to a copy of the stock
            // example: 505 = 500 + 5
            // You create a copy (new_calculated_stock) first to check if the stock is above 0
            // otherwise it already is maybe under 0
            int new_calculated_stock = selected_mp3player.stock + amount_of_stock;
            if (new_calculated_stock < 0)
            {
                Console.WriteLine("Stock cannot be under 0!");
                menu3();
                return; 
            }
            
            // Now the stock is approved, so we update the mp3player stock
            selected_mp3player.stock = new_calculated_stock;

            Console.WriteLine("New stock is {0}", selected_mp3player.stock);
        }

        public static void menu4()
        {
            int amount_of_mp3_players = 0;
            double total_amount_of_worth = 0;
            double total_price = 0;
            double best_price_mb_all = 0;
            // This needs to be set otherwise i get an error while accessing it because it has no value
            MP3Player best_mp3player = new MP3Player(0, "", "", 0, 0, 0);

            foreach (MP3Player mp3player in mp3players)
            {
                amount_of_mp3_players++;
                double worth = mp3player.price * mp3player.stock;
                // total_amount_of_worth = total_amount_of_worth + worth
                total_amount_of_worth += worth;
                total_price += mp3player.price;

                // calculate price per MB
                double price_mb_mp3player = mp3player.price / mp3player.mbsize;

                // if there is no best price yet (best_price_mb_all equals 0), update it
                // if this price per MB is better than the previous best price, update it
                if (best_price_mb_all == 0 || price_mb_mp3player < best_price_mb_all)
                {
                    best_price_mb_all = price_mb_mp3player;
                    best_mp3player = mp3player;
                }
            }
            Console.WriteLine("We have {0} types of mp3 players in our stock", amount_of_mp3_players);
            Console.WriteLine("The total worth of the stock is €{0}", total_amount_of_worth);
            Console.WriteLine("The average price of an mp3player is €{0}", total_price / amount_of_mp3_players);
            Console.WriteLine("The mp3player with the best price per MB is €{0} with ID {1}",
                best_price_mb_all,
                best_mp3player.id
            );
        }

        public static void menu4_maar_dan_anders()
        {
            // .Count on the whole list to get the amount of mp3players
            Console.WriteLine("We have {0} types of mp3 players in our stock", mp3players.Count());
            
            // .Sum adds price * stock
            Console.WriteLine("The total worth of the stock is €{0}", mp3players.Sum(mp3player => mp3player.price * mp3player.stock));
            
            // .Average gets the average of price
            Console.WriteLine("The average price of an mp3player is €{0}", mp3players.Average(mp3player => mp3player.price));
            
            // Get the lowest price per mb
            double best_price_mb = mp3players.Min(mp3player => mp3player.price / mp3player.mbsize);
            
            // Get the mp3 player with the lowest price per mb
            MP3Player best_price_mp3player = mp3players.MinBy(mp3player => mp3player.price / mp3player.mbsize).First();
            
            Console.WriteLine("The mp3player with the best price per MB is €{0} with ID {1}",
                best_price_mb,
                best_price_mp3player.id
            );
        }

        public static void menu5()
        {
            Console.WriteLine("Please enter the make of the mp3player:");
            string input_make = Console.ReadLine();

            Console.WriteLine("Please enter the model of the mp3player:");
            string input_model = Console.ReadLine();

            Console.WriteLine("Please enter the MB size:");
            int input_mbsize;
            try
            {
                input_mbsize = int.Parse(Console.ReadLine());
            }
            catch (Exception e)
            {
                Console.WriteLine("Please enter a valid MB size");
                menu5();
                return;
            }

            Console.WriteLine("Please enter the price mp3player:");
            double input_price;
            try
            {
                input_price = double.Parse(Console.ReadLine());
            }
            catch (Exception e)
            {
                Console.WriteLine("Please enter a valid price (only numbers):");
                menu5();
                return;
            }
            
            // find the highest number in our ids and plus one, to get our new ID
            int new_id = mp3players.Max(mp3player => mp3player.id) + 1;
            
            // add all inserted data and put it in the list
            mp3players.Add(new MP3Player(new_id, input_make, input_model, input_mbsize, input_price, 0));
        }

        public static void Main(string[] args)
        {
            LoginStatus login;
            
            // If we have two arguments, we have a username and password to log in with
            if (args.Length == 2)
            {
                string name = args[0];
                string password = args[1];
                Console.WriteLine("Welcome {0}!", name);
                login = Program.login(password);
            }
            // If we don't, we log in the traditional way
            else
            {
                Console.WriteLine("Please enter your name:");
                string name = Console.ReadLine();
                Console.WriteLine("Welcome {0}!", name);
                login = loginTries();
            }

            if (login == LoginStatus.No || login == LoginStatus.Error)
            {
                Console.WriteLine("Login failed!");
                return;
            }
            
            // Nu zijn we ingelogd, anders stopt ie hierboven
            
            mp3players = new List<MP3Player>();
            mp3players.Add(new MP3Player(1, "GET technologies .inc", "HF 410", 4096, 129.95, 500));
            mp3players.Add(new MP3Player(2, "Far & Loud", "XM 600", 8192, 224.95, 500));
            mp3players.Add(new MP3Player(3, "Innovative", "Z3", 512, 79.95, 500));
            mp3players.Add(new MP3Player(4, "Resistance S.A.", "3001", 4096, 124.95, 500));
            mp3players.Add(new MP3Player(5, "CBA", "NXT volume", 2048, 159.05, 500));

            // Are we logged in automatically, only show specific functions
            if (args.Length == 2)
            {
                Console.WriteLine("\nOverview mp3players:");
                menu1();
                Console.WriteLine("\nOverview stock:");
                menu2();
                Console.WriteLine("\nStatistics:");
                menu4_maar_dan_anders();
            }
            // Logged in traditionally, so we show everything
            else
            {
                showMenu();
            }
        }
    }
}