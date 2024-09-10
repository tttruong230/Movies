/* Program regarding ordering movie tickets
* Class: MIS411 - Business Programming Applications I
* Author: Travis Truong
*/

namespace HW1
{
    internal class Program
    {
        // Creates an enum for MPAA content ratings to gauge how appropriate a movie is for certain age groups
        enum MPAA
        {
            G,
            PG,
            PG_13,
            R,
            NC_17
        }

        // Struct to represent the characteristics of each movie
        struct Movie
        { // Sets the access modifier for all variables below to public so that it can be used in other parts of the program
            public string Title; // The variable for the movie title
            public double Price; // The variable for the movie price
            public MPAA Rating; // The variable for the movie content rating
            public string Genre; // The variable for the movie genre
            public int[] Showtimes; // The variable for movie showtimes. Data type is int[] since the showtimes are stored in an array
            public int NumTickets; // The variable for the number of movie tickets a user orders
            public int ScreenNumber; // The variable to store the screen number
            public int SelectedShowtime; // The variable for the user selection of movie showtime
        }
        // Create a new class to represent a screen with its own capacity
        class Screen
        {
            public int Capacity { get; set; } // The variable for the capacity of the movie minus the number of tickets a user has bought for a particular screen
            public int TotalTicketsSold { get; set; } // The variable for the total number of tickets sold
        }
        static void Main(string[] args)
        {
            const int MaxCapacityPerScreen = 200; // The variable for the maximum seating capacity per screen. THe user can only order up to 200 movie tickets per screen

            // Welcome message when the user first runs the program
            Console.WriteLine("Welcome to Bovinavia Theatre! You can choose a movie by typing a numerical value from 1 to 5 (0 to finish).");

            List<Movie> selectedMovies = new List<Movie>(); // List to store selected movies

            // Create screens with their own capacity independent of the whole movie theater
            Dictionary<int, Screen> screens = new Dictionary<int, Screen>();

            // Create a dictionary to map movie and showtime to screen number
            Dictionary<(string, int), int> movieShowtimeToScreenMap = new Dictionary<(string, int), int>();

            // Creates a while loop for movie selection
            while (true)
            {
                Movie[] movies = new Movie[] // Creates a list of movies the user can choose from
                {
                    new Movie { Title = "The Barbie Movie", Price = 10.50, Rating = MPAA.PG_13, Genre = "Comedy", Showtimes = new int[] { 3, 5, 8 } },
                    new Movie { Title = "Oppenheimer", Price = 12.00, Rating = MPAA.R, Genre = "Drama", Showtimes = new int[] { 4, 7, 10 } },
                    new Movie { Title = "The Super Mario Bros. Movie", Price = 9.00, Rating = MPAA.PG, Genre = "Adventure", Showtimes = new int[] { 1, 3, 6 } },
                    new Movie { Title = "Ratatouille", Price = 8.00, Rating = MPAA.G, Genre = "Drama", Showtimes = new int[] { 2, 5, 7 } },
                    new Movie { Title = "Super Size Me", Price = 11.00, Rating = MPAA.PG_13, Genre = "Documentary", Showtimes = new int[] { 1, 4, 9 } }
                };
                // Creates a for loop for the program to display the movie with its price
                for (int i = 0; i < movies.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {movies[i].Title}: {movies[i].Price:C}");
                }

                int movieChoice;
                do
                {
                    movieChoice = GetValidIntegerInput("Type your number of the movie you want to watch here: ", 0, movies.Length);
                } while (movieChoice < 0 || movieChoice > movies.Length);

                if (movieChoice == 0) // If the user types in 0, the program ends
                    break;
                Movie selectedMovie = movies[movieChoice - 1];

                Console.WriteLine($"You have selected to see {selectedMovie.Title}.\nMPAA Rating: {selectedMovie.Rating}\nGenre: {selectedMovie.Genre}");

                // For loop to add :00 to each showtime
                Console.WriteLine("Available Showtimes:");
                for (int i = 0; i < selectedMovie.Showtimes.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {selectedMovie.Showtimes[i]}:00"); // Adds :00 to each showtime hour so that it formats like 1:00 for example
                }

                int showtimeChoice;
                // Dowhile loop if the user selects a showtime for their movie selection
                do
                {
                    showtimeChoice = GetValidIntegerInput($"Select a showtime (1-{selectedMovie.Showtimes.Length}): ", 1, selectedMovie.Showtimes.Length);
                } while (showtimeChoice < 1 || showtimeChoice > selectedMovie.Showtimes.Length);

                int selectedShowtime = selectedMovie.Showtimes[showtimeChoice - 1];
                double ticketPrice = selectedMovie.Price;

                // Check if the movie and showtime combination the user chooses already has a screen assigned
                var movieShowtimeKey = (selectedMovie.Title, selectedShowtime);
                int screenNumberForShowtime;

                if (movieShowtimeToScreenMap.ContainsKey(movieShowtimeKey))
                {
                    screenNumberForShowtime = movieShowtimeToScreenMap[movieShowtimeKey];
                }
                else
                {
                    screenNumberForShowtime = AllocateScreenNumber(screens);
                    movieShowtimeToScreenMap[movieShowtimeKey] = screenNumberForShowtime;
                }
                // Message when the user finishes selecting a movie with its showtime and the cost of the tickets the user has bought.
                Console.WriteLine($"You have selected to see the movie {selectedMovie.Title} on screen {screenNumberForShowtime} at {selectedShowtime}:00.");
                Console.WriteLine($"The cost for each ticket will be {ticketPrice:C}.");

                // If statement to see if a certain screen number has already been assigned to a specific combination of movie and showtime.
                if (!screens.ContainsKey(screenNumberForShowtime))
                {
                    screens[screenNumberForShowtime] = new Screen { Capacity = MaxCapacityPerScreen, TotalTicketsSold = 0 };
                }

                int maxTickets = screens[screenNumberForShowtime].Capacity - screens[screenNumberForShowtime].TotalTicketsSold; // Max tickets considering remaining capacity

                // If statement to see if a particular movie screen has reached capacity or not.
                if (maxTickets <= 0)
                {
                    // Sends the message to the user if the user tries to order more tickets from a screen that is full
                    Console.WriteLine("You have reached the maximum ticket capacity reached for this movie theater screen. Please choose a different movie to buy tickets for.");
                    continue;
                }

                // Dowhile loop if the user chooses a movie in the application.
                int numTickets;
                do
                {
                    numTickets = GetValidIntegerInput($"Enter the number of tickets to purchase (1-{maxTickets}): ", 1, maxTickets);
                } while (numTickets < 1 || numTickets > maxTickets);

                double subtotal = numTickets * ticketPrice;

                Console.WriteLine($"You are purchasing {numTickets} ticket(s) for a total of {subtotal:C}.");

                // Update the total tickets sold for the selected screen
                screens[screenNumberForShowtime].TotalTicketsSold += numTickets;

                // Store the selected showtime along with movie information
                selectedMovie.ScreenNumber = screenNumberForShowtime;
                selectedMovie.NumTickets = numTickets;
                selectedMovie.SelectedShowtime = selectedShowtime;

                selectedMovies.Add(selectedMovie);

                // Dowhile loop for the user to choose another movie or not, if the users buys at least one movie ticket
                string anotherMovieChoice;
                do
                {
                    Console.WriteLine("Do you want to choose another movie? (Answer with 'No' or 'Yes'): ");
                    anotherMovieChoice = Console.ReadLine().Trim().ToLower();
                    if (anotherMovieChoice != "yes" && anotherMovieChoice != "no")
                    {
                        Console.WriteLine("Invalid input. Please enter 'No' or 'Yes'.");
                    }
                } while (anotherMovieChoice != "yes" && anotherMovieChoice != "no");

                if (anotherMovieChoice != "yes")
                    break;
            }
            // Uses an if loop to show a receipt of the user's selection of movies if the user selects at least one movie
            if (selectedMovies.Count > 0)
            {

                // Receipt header formatting
                Console.WriteLine("\nReceipt:");
                Console.WriteLine("====================================================================================================================");
                Console.WriteLine("{0,-30} {1,-10} {2,-10} {3,-10} {4,-10} {5,-10:C}", "Movie", "Screen", "Time", "Price", "Quantity", "Subtotal");
                Console.WriteLine("====================================================================================================================");

                double grandTotal = 0; // Sets the initial value for grand total of ticket bought to 0 since the user doesn't buy any tickets by default

                foreach (Movie selectedMovie in selectedMovies)
                {
                    double ticketPrice = selectedMovie.Price;
                    double subtotal = selectedMovie.NumTickets * ticketPrice;

                    Console.WriteLine($"{selectedMovie.Title,-30} {selectedMovie.ScreenNumber,-10} {selectedMovie.SelectedShowtime}:00       {ticketPrice,-10:C} {selectedMovie.NumTickets,-10} {subtotal,-10:C}");

                    grandTotal += subtotal;
                }

                // Display the grand total of the total dollar value of movie tickets the user has bought.
                Console.WriteLine("====================================================================================================================");
                Console.WriteLine($"Grand Total: {grandTotal:C}");
            }

            Console.WriteLine("\nThank you for choosing Bovinavia Theatre!\nHave a great day and I hope to see you again soon!"); // Shows a message to thank the user for their time using the program
        }

        // Helper method to get a valid integer input within a specified range
        static int GetValidIntegerInput(string prompt, int minValue, int maxValue)
        {
            int result;
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out result) && result >= minValue && result <= maxValue)
                {
                    return result;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid whole numerical value that is not negative within the specified range.");
                }
            }
        }

        // Helper method to allocate a dynamic screen number for each movie and showtime
        static int AllocateScreenNumber(Dictionary<int, Screen> screens)
        {
            int nextScreenNumber = 1;
            while (true)
            {
                if (!screens.ContainsKey(nextScreenNumber))
                {
                    return nextScreenNumber;
                }
                nextScreenNumber++;
            }
        }
    }
}