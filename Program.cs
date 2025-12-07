/* Blackjack Project
* 
* This program sets up a multiplayer Blackjack game by getting player
* information, choosing game mode, and deciding how many hands to play.
* It then runs the game, tracks stats, and saves/loads a global high score.
* 
* Author: Joel Bellows, Hayden Stewart, Laly Hernandez
*/

using System;
using System.Collections.Generic;
using System.IO;

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
        HeadtoHead = 1,
        VersusHouse
    }

    //CLASS to store player information 
    public class Player
    {
        public string FirstName;
        public string LastName;
        public string Username;
        public int Points;

        //Hand + stats (per game)
        public List<Card> Hand = new List<Card>();
        public int HandsPlayed;
        public int Wins;
        public int Losses;
        public int Pushes;
        public int Blackjacks;
        public int Busts;
        public int HandsWith21;
        public int TotalCardsDrawn;
        public int MaxCardsInOneHand;
        public int CurrentWinStreak;
        public int BestWinStreak;

        public string FullName()
        {
            return $"{FirstName} {LastName}";
        }

        public void ResetForNewGame()
        {
            Points = 0;
            Hand.Clear();
            HandsPlayed = 0;
            Wins = 0;
            Losses = 0;
            Pushes = 0;
            Blackjacks = 0;
            Busts = 0;
            HandsWith21 = 0;
            TotalCardsDrawn = 0;
            MaxCardsInOneHand = 0;
            CurrentWinStreak = 0;
            BestWinStreak = 0;
        }

        public void ClearHand()
        {
            Hand.Clear();
        }

        public void AddCard(Card c)
        {
            Hand.Add(c);
            TotalCardsDrawn++;

            if (Hand.Count > MaxCardsInOneHand)
            {
                MaxCardsInOneHand = Hand.Count;
            }
        }
    }

    //CLASS for a deck of cards
    public class Deck
    {
        private List<Card> cards = new List<Card>();
        private static Random rng = new Random();

        public Deck()
        {
            Reset();
        }

        //Reset to a full 52-card deck
        public void Reset()
        {
            cards.Clear();

            foreach (Suit s in Enum.GetValues(typeof(Suit)))
            {
                foreach (Rank r in Enum.GetValues(typeof(Rank)))
                {
                    Card c = new Card();
                    c.suit = s;
                    c.rank = r;
                    cards.Add(c);
                }
            }
        }

        //Shuffle deck
        public void Shuffle()
        {
            for (int i = cards.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                Card temp = cards[i];
                cards[i] = cards[j];
                cards[j] = temp;
            }
        }

        //Draw top card
        public Card Draw()
        {
            if (cards.Count == 0)
            {
                Reset();
                Shuffle();
            }

            Card c = cards[0];
            cards.RemoveAt(0);
            return c;
        }
    }

    internal class Program
    {
        const int maxPlayers = 4;   //maximum number of players

        //Points rules (easy to change later)
        const int POINTS_WIN = 5;
        const int POINTS_SECOND = 3;
        const int POINTS_THIRD = 1;
        const int POINTS_21_BONUS = 15;

        //High score file (relative path)
        static readonly string DataFolder = "Data";
        static readonly string HighScorePath = Path.Combine(DataFolder, "HighScore.txt");

        static void Main(string[] args)
        {
            //Load global high score once
            string globalHighName = "";
            int globalHighScore = 0;
            LoadHighScore(ref globalHighName, ref globalHighScore);

            bool playAgain = true;
            while (playAgain)
            {
                Console.WriteLine("Welcome to Wildcat Blackjack!");
                Console.WriteLine("Up to four players can play.\n");

                if (globalHighScore > 0 && globalHighName != "")
                {
                    Console.WriteLine($"Global High Score to beat: {globalHighName} with {globalHighScore} points.\n");
                }

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

                    p.FirstName = ValidStringInput("Enter your first name: ");
                    p.LastName = ValidStringInput("Enter your last name: ");

                    //Username must be unique in this session
                    bool okUser = false;
                    while (!okUser)
                    {
                        string uname = ValidStringInput("Enter your username (or create one): ");
                        bool taken = false;
                        for (int k = 0; k < players.Count; k++)
                        {
                            if (players[k].Username != null && players[k].Username.Equals(uname, StringComparison.OrdinalIgnoreCase))
                            {
                                taken = true;
                            }
                        }

                        if (taken)
                        {
                            Console.WriteLine("That username is already taken by another player. Try a different one.");
                        }
                        else
                        {
                            p.Username = uname;
                            okUser = true;
                        }
                    }

                    p.ResetForNewGame();
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

                //Run the actual Blackjack game
                RunGame(players, mode, numHands, ref globalHighName, ref globalHighScore);

                //Ask to play again
                playAgain = ValidYesNoInput("\nWould you like to play another game? (yes/no): ");
                if (!playAgain)
                {
                    Console.WriteLine("\nThank you for playing Wildcat Blackjack! Goodbye.");
                }
                else
                {
                    Console.WriteLine();
                }
            }
        }

        //Runs the full game for the selected number of hands
        static void RunGame(List<Player> players, GameMode mode, int numHands, ref string globalHighName, ref int globalHighScore)
        {
            Deck deck = new Deck();
            Player dealer = new Player();

            dealer.FirstName = "Dealer";
            dealer.LastName = "";
            dealer.Username = "HOUSE";
            dealer.ResetForNewGame();

            //Game stats
            Dictionary<int, int> handDistribution = new Dictionary<int, int>();
            int totalBusts = 0;
            int highestHandTotal = 0;
            int highestRoundPoints = 0;
            string highestRoundPointsName = "";

            //For recognitions
            Player marathoner = null;
            int marathonCards = 0;

            //Reset players for this game
            for (int i = 0; i < players.Count; i++)
            {
                players[i].ResetForNewGame();
            }

            Console.WriteLine("\n===== GAME START =====");

            for (int handNum = 1; handNum <= numHands; handNum++)
            {
                Console.WriteLine($"\n----- HAND {handNum} -----");

                //Fresh deck each hand (simple + avoids running out of cards)
                deck.Reset();
                deck.Shuffle();

                //Clear hands
                for (int i = 0; i < players.Count; i++)
                {
                    players[i].ClearHand();
                }
                dealer.ClearHand();

                //Deal initial 2 cards
                for (int i = 0; i < players.Count; i++)
                {
                    players[i].AddCard(deck.Draw());
                    players[i].AddCard(deck.Draw());
                }

                if (mode == GameMode.VersusHouse)
                {
                    dealer.AddCard(deck.Draw());
                    dealer.AddCard(deck.Draw());
                    Console.WriteLine($"Dealer shows: {dealer.Hand[0].Name()} (one card hidden)");
                }

                //Player turns
                for (int i = 0; i < players.Count; i++)
                {
                    Player p = players[i];
                    Console.WriteLine($"\n{p.FirstName}'s turn:");

                    //Head-to-head variation: show one card as "hidden" to match the idea
                    if (mode == GameMode.HeadtoHead)
                    {
                        Console.WriteLine($"Your cards: {p.Hand[0].Name()} and [Hidden Card]");
                    }
                    else
                    {
                        Console.WriteLine($"Your cards: {p.Hand[0].Name()} and {p.Hand[1].Name()}");
                    }

                    Console.WriteLine($"Current total (with Ace rules): {HandValue(p.Hand)}");

                    bool doubledDown = false;

                    //Auto-stand on blackjack
                    if (IsBlackjack(p.Hand))
                    {
                        Console.WriteLine("BLACKJACK! (21 on the deal)");
                    }
                    else
                    {
                        bool done = false;
                        while (!done)
                        {
                            int total = HandValue(p.Hand);

                            if (total > 21)
                            {
                                done = true;
                            }
                            else
                            {
                                string prompt = "Choose: (H)it or (S)tand";
                                if (mode == GameMode.VersusHouse && p.Hand.Count == 2)
                                {
                                    prompt += " or (D)ouble Down";
                                }
                                prompt += ": ";

                                string choice = ValidActionInput(prompt, mode, p.Hand.Count);

                                if (choice == "H")
                                {
                                    Card c = deck.Draw();
                                    p.AddCard(c);
                                    Console.WriteLine($"You drew: {c.Name()}");
                                    Console.WriteLine($"New total: {HandValue(p.Hand)}");

                                    if (HandValue(p.Hand) > 21)
                                    {
                                        Console.WriteLine("BUST! (over 21)");
                                        done = true;
                                    }
                                }
                                else if (choice == "S")
                                {
                                    Console.WriteLine($"You stand at: {HandValue(p.Hand)}");
                                    done = true;
                                }
                                else if (choice == "D")
                                {
                                    Card c = deck.Draw();
                                    p.AddCard(c);
                                    Console.WriteLine($"Double Down! You drew: {c.Name()}");
                                    Console.WriteLine($"Final total: {HandValue(p.Hand)}");
                                    doubledDown = true;

                                    if (HandValue(p.Hand) > 21)
                                    {
                                        Console.WriteLine("BUST! (over 21)");
                                    }
                                    done = true;
                                }
                            }

                            if (doubledDown)
                            {
                                done = true;
                            }
                        }
                    }

                    //Marathoner tracking (most cards in one hand)
                    if (p.Hand.Count > marathonCards)
                    {
                        marathonCards = p.Hand.Count;
                        marathoner = p;
                    }
                }

                //Dealer turn if neded
                int dealerTotal = 0;
                bool dealerBust = false;

                if (mode == GameMode.VersusHouse)
                {
                    Console.WriteLine("\nDealer reveals hidden card:");
                    Console.WriteLine($"Dealer's hidden card was: {dealer.Hand[1].Name()}");
                    Console.WriteLine($"Dealer total: {HandValue(dealer.Hand)}");

                    while (HandValue(dealer.Hand) < 17)
                    {
                        Card c = deck.Draw();
                        dealer.AddCard(c);
                        Console.WriteLine($"Dealer hits and draws: {c.Name()}");
                        Console.WriteLine($"Dealer total: {HandValue(dealer.Hand)}");
                    }

                    dealerTotal = HandValue(dealer.Hand);

                    if (dealerTotal > 21)
                    {
                        dealerBust = true;
                        Console.WriteLine("Dealer BUSTS!");
                    }
                    else
                    {
                        Console.WriteLine($"Dealer stands at: {dealerTotal}");
                    }
                }

                //score this hand
                Console.WriteLine("\n===== HAND RESULTS =====");
                Console.WriteLine("Player\t\tHand Total\tRound Points\tTotal Points");

                for (int i = 0; i < players.Count; i++)
                {
                    Player p = players[i];
                    p.HandsPlayed++;

                    int total = HandValue(p.Hand);
                    bool bust = total > 21;

                    //Distribution stats
                    if (bust)
                    {
                        totalBusts++;
                    }
                    else
                    {
                        if (!handDistribution.ContainsKey(total))
                        {
                            handDistribution[total] = 0;
                        }
                        handDistribution[total]++;
                    }

                    if (!bust && total > highestHandTotal)
                    {
                        highestHandTotal = total;
                    }

                    if (IsBlackjack(p.Hand))
                    {
                        p.Blackjacks++;
                    }
                    if (!bust && total == 21)
                    {
                        p.HandsWith21++;
                    }
                    if (bust)
                    {
                        p.Busts++;
                    }

                    int roundPoints = 0;
                    string resultLabel = "";

                    if (mode == GameMode.VersusHouse)
                    {
                        //Vs dealer scoring
                        if (bust)
                        {
                            p.Losses++;
                            p.CurrentWinStreak = 0;
                            resultLabel = "LOSE (BUST)";
                        }
                        else if (dealerBust || total > dealerTotal)
                        {
                            p.Wins++;
                            p.CurrentWinStreak++;
                            if (p.CurrentWinStreak > p.BestWinStreak)
                            {
                                p.BestWinStreak = p.CurrentWinStreak;
                            }

                            roundPoints += POINTS_WIN;
                            resultLabel = "WIN";
                        }
                        else if (total == dealerTotal)
                        {
                            p.Pushes++;
                            p.CurrentWinStreak = 0;
                            resultLabel = "PUSH";
                        }
                        else
                        {
                            p.Losses++;
                            p.CurrentWinStreak = 0;
                            resultLabel = "LOSE";
                        }

                        //Bonus for 21 (any 21)
                        if (!bust && total == 21)
                        {
                            roundPoints += POINTS_21_BONUS;
                        }
                    }
                    else
                    {
                        //Head-to-head scoring
                        int rank = GetHeadToHeadRank(players, p);
                        if (bust)
                        {
                            p.Losses++;
                            p.CurrentWinStreak = 0;
                            resultLabel = "BUST";
                        }
                        else if (rank == 1)
                        {
                            p.Wins++;
                            p.CurrentWinStreak++;
                            if (p.CurrentWinStreak > p.BestWinStreak)
                            {
                                p.BestWinStreak = p.CurrentWinStreak;
                            }

                            roundPoints += POINTS_WIN;
                            resultLabel = "WIN";
                        }
                        else
                        {
                            p.Losses++;
                            p.CurrentWinStreak = 0;

                            resultLabel = $"PLACE {rank}";
                            if (rank == 2)
                            {
                                roundPoints += POINTS_SECOND;
                            }
                            else if (rank == 3)
                            {
                                roundPoints += POINTS_THIRD;
                            }
                        }

                        //Bonus for 21
                        if (!bust && total == 21)
                        {
                            roundPoints += POINTS_21_BONUS;
                        }
                    }

                    p.Points += roundPoints;

                    //Highest points in one hand
                    if (roundPoints > highestRoundPoints)
                    {
                        highestRoundPoints = roundPoints;
                        highestRoundPointsName = p.FullName();
                    }

                    string totalText = bust ? "BUST" : total.ToString();
                    Console.WriteLine($"{p.FirstName}\t\t{totalText}\t\t{roundPoints}\t\t{p.Points} ({resultLabel})");
                }

                //After each hand, show quick standings
                Console.WriteLine("\nCurrent Standings:");
                for (int i = 0; i < players.Count; i++)
                {
                    Console.WriteLine($"{players[i].FirstName}: {players[i].Points} points");
                }
            }

            //Final summary
            Console.WriteLine("\n===== FINAL GAME SUMMARY =====");

            //Winner
            int bestScore = -1;
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].Points > bestScore)
                {
                    bestScore = players[i].Points;
                }
            }

            Console.WriteLine("\nFinal Scores:");
            for (int i = 0; i < players.Count; i++)
            {
                Console.WriteLine($"{players[i].FullName()} ({players[i].Username}) - {players[i].Points} points");
            }

            //Winner message 
            List<Player> winners = new List<Player>();
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].Points == bestScore)
                {
                    winners.Add(players[i]);
                }
            }

            if (winners.Count == 1)
            {
                Console.WriteLine($"\nWinner: {winners[0].FullName()} with {bestScore} points!");
            }
            else
            {
                Console.WriteLine($"\nWinner: Tie at {bestScore} points between:");
                for (int i = 0; i < winners.Count; i++)
                {
                    Console.WriteLine($"- {winners[i].FullName()}");
                }
            }

            Console.WriteLine("\nPlayer Stats:");
            for (int i = 0; i < players.Count; i++)
            {
                Player p = players[i];
                Console.WriteLine($"\n{p.FullName()}");
                Console.WriteLine($"Hands Played: {p.HandsPlayed}");
                Console.WriteLine($"Wins: {p.Wins}");
                Console.WriteLine($"Losses: {p.Losses}");
                if (mode == GameMode.VersusHouse)
                {
                    Console.WriteLine($"Pushes: {p.Pushes}");
                }
                Console.WriteLine($"Blackjacks: {p.Blackjacks}");
                Console.WriteLine($"Busts: {p.Busts}");
                Console.WriteLine($"Hands with 21: {p.HandsWith21}");
                Console.WriteLine($"Best Win Streak: {p.BestWinStreak}");
                Console.WriteLine($"Most Cards in One Hand: {p.MaxCardsInOneHand}");
                Console.WriteLine($"Total Cards Drawn: {p.TotalCardsDrawn}");
            }

            //Distribution
            Console.WriteLine("\nHand Total Distribution (non-bust totals):");
            List<int> totals = new List<int>(handDistribution.Keys);
            totals.Sort();
            for (int i = 0; i < totals.Count; i++)
            {
                int t = totals[i];
                Console.WriteLine($"{t}: {handDistribution[t]} time(s)");
            }
            Console.WriteLine($"Busts: {totalBusts} time(s)");
            Console.WriteLine($"Highest Hand Total (non-bust) in this game: {highestHandTotal}");
            Console.WriteLine($"Highest Round Points Earned: {highestRoundPoints} by {highestRoundPointsName}");

            //Recognitions
            Console.WriteLine("\n===== SPECIAL RECOGNITIONS =====");

            if (marathoner != null)
            {
                Console.WriteLine($"Marathoner (most cards in a single hand): {marathoner.FullName()} with {marathonCards} cards");
            }

            //Short  Stop
            Player shortStop = null;
            int minCards = int.MaxValue;
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].TotalCardsDrawn < minCards)
                {
                    minCards = players[i].TotalCardsDrawn;
                    shortStop = players[i];
                }
            }
            if (shortStop != null)
            {
                Console.WriteLine($"Short Stop (fewest cards drawn overall): {shortStop.FullName()} with {shortStop.TotalCardsDrawn} cards");
            }

            //Lucky Son of a Gun 
            if (numHands == 5)
            {
                for (int i = 0; i < players.Count; i++)
                {
                    if (players[i].Busts == 0)
                    {
                        Console.WriteLine($"Lucky Son of a Gun (no busts in all 5 rounds): {players[i].FullName()}");
                    }
                }
            }

            //High score update
            Player bestPlayer = winners[0];
            if (winners.Count > 1)
            {
                bestPlayer = winners[0];
            }

            if (bestScore > globalHighScore)
            {
                globalHighScore = bestScore;
                globalHighName = bestPlayer.FullName();
                SaveHighScore(globalHighName, globalHighScore);

                Console.WriteLine($"\nNEW GLOBAL HIGH SCORE!");
                Console.WriteLine($"{globalHighName} set a new record with {globalHighScore} points!");
                Console.WriteLine($"Saved to: {HighScorePath}");
            }
            else if (globalHighScore > 0 && globalHighName != "")
            {
                Console.WriteLine($"\nNo new high score this time.");
                Console.WriteLine($"Record is still: {globalHighName} with {globalHighScore} points.");
            }
            else
            {
                Console.WriteLine($"\nNo global high score found. Next game can create one!");
            }

            Console.WriteLine("\n===== GAME COMPLETE =====");
        }

        //Calculates blackjack hand value with Ace adjustment
        static int HandValue(List<Card> hand)
        {
            int total = 0;
            int aces = 0;

            for (int i = 0; i < hand.Count; i++)
            {
                total += hand[i].Value();
                if (hand[i].rank == Rank.Ace)
                {
                    aces++;
                }
            }

            //If we are over 21 and have Aces, drop Ace from 11 to 1 by subtracting 10
            while (total > 21 && aces > 0)
            {
                total -= 10;
                aces--;
            }

            return total;
        }

        static bool IsBlackjack(List<Card> hand)
        {
            return hand.Count == 2 && HandValue(hand) == 21;
        }

        //Ranking for head-to-head mode
        static int GetHeadToHeadRank(List<Player> players, Player target)
        {
            int targetTotal = HandValue(target.Hand);
            bool targetBust = targetTotal > 21;

            if (targetBust)
            {
                return players.Count;
            }

            int rank = 1;
            for (int i = 0; i < players.Count; i++)
            {
                Player other = players[i];
                if (other == target) continue;

                int otherTotal = HandValue(other.Hand);
                if (otherTotal <= 21 && otherTotal > targetTotal)
                {
                    rank++;
                }
            }
            return rank;
        }

        //Load high score from file
        static void LoadHighScore(ref string name, ref int score)
        {
            try
            {
                if (!Directory.Exists(DataFolder))
                {
                    Directory.CreateDirectory(DataFolder);
                }

                if (File.Exists(HighScorePath))
                {
                    string line = File.ReadAllText(HighScorePath).Trim();
                    if (line.Contains("|"))
                    {
                        string[] parts = line.Split('|');
                        if (parts.Length == 2)
                        {
                            name = parts[0].Trim();
                            int.TryParse(parts[1].Trim(), out score);
                        }
                    }
                }
            }
            catch
            {
                name = "";
                score = 0;
            }
        }

        //Save high score to file
        static void SaveHighScore(string name, int score)
        {
            try
            {
                if (!Directory.Exists(DataFolder))
                {
                    Directory.CreateDirectory(DataFolder);
                }

                File.WriteAllText(HighScorePath, $"{name}|{score}");
            }
            catch
            {
                //If saving fails, do nothing 
            }
        }

        //integer input validation
       static int ValidIntInput(string input, int min, int max)
        {
            int value = 0;
            bool valid = false;

            while (!valid)
            {
                Console.Write(input);
                string raw = Console.ReadLine() ?? "";

                if (!int.TryParse(raw, out value))
                {
                    Console.WriteLine("Please enter a valid number.");
                }
                else if (value < min || value > max)
                {
                    Console.WriteLine($"Please enter a value between {min} and {max}.");
                }
                else
                {
                    valid = true;
                }
            }
            return value;
        }


        //string input validation (no empty)
        static string ValidStringInput(string prompt)
        {
            string value = "";
            while (value.Trim() == "")
            {
                Console.Write(prompt);
                value = Console.ReadLine();
                if (value == null) value = "";
                value = value.Trim();

                if (value == "")
                {
                    Console.WriteLine("Please enter a non-empty value.");
                }
            }
            return value;
        }

        //yes/no input validation
        static bool ValidYesNoInput(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (input == null) input = "";
                input = input.Trim().ToLower();

                if (input == "yes" || input == "y")
                {
                    return true;
                }
                if (input == "no" || input == "n")
                {
                    return false;
                }

                Console.WriteLine("Please enter yes or no.");
            }
        }

        //action input validation for H/S/D
        static string ValidActionInput(string prompt, GameMode mode, int cardsInHand)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (input == null) input = "";
                input = input.Trim().ToUpper();

                if (input == "H" || input == "HIT")
                {
                    return "H";
                }
                if (input == "S" || input == "STAND")
                {
                    return "S";
                }
                if ((input == "D" || input == "DOUBLE" || input == "DOUBLE DOWN") && mode == GameMode.VersusHouse && cardsInHand == 2)
                {
                    return "D";
                }

                Console.WriteLine("Invalid choice. Try again.");
            }
        }
    }
}
