/* Blackjack Project
 * 
 * This program sets up a multiplayer Blackjack game by getting player
 * information, choosing game mode, and deciding how many hands to play.
 * 
 * Author: Joel Bellows, 
 */

using System;
using System.Collections.Generic;

namespace Blackjack_Project
{
    //ENUM for card suits
    public enum Suit
    {
        Hearts = 1,
        Diamonds,
        Clubs,
        Spades
    }

    //ENUM for card ranks
    public enum Rank
    {
        Two = 2,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace
    }

    //STRUCT for a card
    public struct Card
    {
        public Suit suit;
        public Rank rank;

        //Return readable name of the card
        public string Name()
        {
            string card = rank.ToString();
            if (Convert.ToInt32(rank) >= 2 && Convert.ToInt32(rank) <= 10)
            {
                card = Convert.ToInt32(rank).ToString();
            }
            // For J, Q, K & A a string is fine
            return $"{card} of {suit}";
        }

        //Return Blackjack value of the card
        public int Value()
        {
            if (rank >= Rank.Two && rank <= Rank.Ten)
                return Convert.ToInt32(rank);
            if (rank == Rank.Jack || rank == Rank.Queen || rank == Rank.King)
                return 10;
            // Ace will default 11
            return 11;
        }
    }

    //ENUM for the game mode 
    public enum GameMode
    {
        HeadtoHead =1,
        VersusHouse
    }

    //CLASS to store player information 
    public class Player
    {
        public string FirstName;
        public string LastName;
        public string Username;
        public int Points;

        public string FullName()
        {
            return $"{FirstName} {LastName}";
        }
    }
    internal class Program
    {
        const int maxPlayers = 4;   //maximum number of players
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Wildcat Blackjack!");
            Console.WriteLine("Up to four players can play.\n");

            //ask for how many players
            int numPlayers = ValidIntInput("Enter the number of players (1-4): ", 1, maxPlayers);

            //list to store players
            List<Player> players = new List<Player>();

            //welcome messages 
            string[] welcomeMessages =
            {
                "Welcome to the table, ",
                "Good luck, ",
                "Thank you for playing, ",
                "We're glad to have you, "
            };

            //collecting player info
            for (int i = 0; i < numPlayers; i++)
            {
                Player p = new Player();
                Console.WriteLine($"\nPlayer {i + 1} information");

                Console.Write("Enter your first name: ");
                p.FirstName = Console.ReadLine();

                Console.Write("Enter your last name: ");
                p.LastName = Console.ReadLine();

                Console.Write("Enter your username (or create one): ");
                p.Username = Console.ReadLine();

                p.Points = 0;

                players.Add(p);

                //rotate through welcome messages 
                Console.WriteLine($"{welcomeMessages[i % welcomeMessages.Length]}{p.FirstName}!\n");
            }
            //choosing game mode
            Console.WriteLine("Choose a game mode:");
            Console.WriteLine("1. Head-to-Head (Player vs Player)");
            Console.WriteLine("2. Each Player vs the House");

            int modeChoice = ValidIntInput("Enter your choice (1-2): ", 1, 2);
            GameMode mode = (GameMode)modeChoice;

            Console.WriteLine($"\nYou selected: {mode}");

            //choose how many hands to play
            int numHands = ValidIntInput("Enter how many hands will be played (1-50): ", 1, 50);

            //display a game setup summary
            Console.WriteLine("\n===== GAME SETUP SUMMARY =====");
            Console.WriteLine($"Number of Players: {numPlayers}");
            Console.WriteLine($"Game Mode: {mode}");
            Console.WriteLine($"Number of Hands: {numHands}");

            Console.WriteLine("Players: ");
            for (int i = 0; i < players.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {players[i].FullName()} (Username: {players[i].Username})");
            }
        }
        //integer input validation
        static int ValidIntInput(string input, int min, int max)
        {
            Console.Write(input);
            int value = Convert.ToInt32(Console.ReadLine());

            while (value < min || value > max)
            {
                Console.WriteLine($"Please enter a value between {min} and {max}.");
                Console.Write(input);
                value = Convert.ToInt32(Console.ReadLine());
            }
            return value;
        }
    }
}
