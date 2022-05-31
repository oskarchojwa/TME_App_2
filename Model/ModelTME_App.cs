using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace TME_App.Model
{
    class ModelTME_App
    {
        public string inputText;//przechowuje liczbę podaną przez użytkownika (ile liczb chce wylosować)
        public int quantityOfNumbers; //ilość liczb jaką użytkownik chce wylosować czyli inputText po konwersji

        public string quantityOfNumbersYouCanRandMessage; //wiadomość wyświetlana w widoku
        private int quantityOfNumbersYouCanRand; //ilość liczb jaka jest maksymalnie możliwa do wylosowania

        public Boolean correctNumber = false;

        public void inputTextToInt(string inputText)  //pierwsza metoda w moim programie, która konwertuje inputText na int i w przypadku powodzenia rozpoczyna losowanie
        {
            if(validateText(inputText) == true)
            {
                quantityOfNumbers = Convert.ToInt32(inputText);
                if (quantityOfNumbers <= quantityOfNumbersYouCanRand && quantityOfNumbers > 0)
                {
                    correctNumber = true;
                }
                else
                {
                    if (quantityOfNumbersYouCanRand == 0)
                    {
                        quantityOfNumbersYouCanRandMessage = "Najpierw wczytaj baze!";
                    }
                    else
                    {
                        quantityOfNumbersYouCanRandMessage = "Podaj liczbę która mieści się w zakresie od 1 do " + quantityOfNumbersYouCanRand.ToString();
                    }
                }
            }
            else
            {
                if (quantityOfNumbersYouCanRand == 0)
                {
                    quantityOfNumbersYouCanRandMessage = "Najpierw wczytaj baze!";
                }
                else
                {
                    quantityOfNumbersYouCanRandMessage = "Podaj liczbę która mieści się w zakresie od 1 do " + quantityOfNumbersYouCanRand.ToString();
                }
            }
        }

        public bool validateText(string inputText) //metoda sprawdzająca czy podany ciąg znaków jest liczbą
        {
            if(!String.IsNullOrEmpty(inputText))
            {
                inputText = inputText.Trim();
                if (inputText.All(char.IsDigit) == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            
        }

        public void loadDatabase() //metoda wywołująca wczytywanie danych z bazy lub utworzenie nowej bazy jeśli takowa nie istnieje
        {
            Database database = new Database();
            if (database.selectedFromDatabase.Count != 0)
            {
                double percetOfUsageOfDatabse = (double)database.selectedFromDatabase.Count / (9999999 - 1000000); //obliczam procent wykorzystania bazy danych
                quantityOfNumbersYouCanRand = 9999999 - 1000000 - database.selectedFromDatabase.Count; //obliczam ile maksymalnie moge wylosować liczb
                quantityOfNumbersYouCanRandMessage = "W bazie wylosowanych numerów znajduje się " + database.selectedFromDatabase.Count.ToString() + " numerów\nProcent użycia numerów:\n" + percetOfUsageOfDatabse.ToString() + " %";
            }
            else
            {
                quantityOfNumbersYouCanRand = 8999999;
                quantityOfNumbersYouCanRandMessage = "Twoja baza danych jest pusta możesz zatem wylosować maksymalnie 8 999 999 numerów\nProcent użycia numerów:\n0%";
            }
            quantityOfNumbersYouCanRandMessage += "\n\nPoniżej wpisz ilość liczb jaką wchcesz wylosować i zapisać automatycznie do bazy";
        }
    }

    class Database //klasa odpowiedzialna za komunikację z bazą
    {
        public List<int> selectedFromDatabase
        {
            get;
            set;
        }

        public SQLiteConnection myConnection;

        public Database()
        {
            myConnection = new SQLiteConnection("Data Source=database.sqlite3");
            if (!File.Exists("./database.sqlite3"))
            {
                createDatabase(); //jeśli baza nie istnieje to utwórz nową
            }
            else
            {
                selectFromDatabase(); //jeśli baza istnieje rozpoczynam wczytywanie liczb
            }
        }

        public void OpenConnection() //otwieram połączenie z bazą danych
        {
            if (myConnection.State != System.Data.ConnectionState.Open)
            {
                myConnection.Open();
            }
        }

        public void CloseConnection() //zamykam połączenie z bazą danych
        {
            if (myConnection.State != System.Data.ConnectionState.Closed)
            {
                myConnection.Close();
            }
        }

        public void createDatabase() //tworzę plikową bazę danych
        {
            SQLiteConnection.CreateFile("database.sqlite3");
            string query = "CREATE TABLE Liczby (ID INTEGER PRIMARY KEY AUTOINCREMENT, Liczba int);";
            OpenConnection();
            SQLiteCommand myCommand = new SQLiteCommand(query, myConnection);
            myCommand.ExecuteNonQuery();
            CloseConnection();
        }

        public void insertIntoDatabase(int number) //dodawanie liczby do bazy danych
        {
            string query = "INSERT INTO Liczby (`Liczba`) VALUES (@Liczba);";
            OpenConnection();
            SQLiteCommand myCommand = new SQLiteCommand(query, myConnection);
            myCommand.Parameters.AddWithValue("@Liczba", number);
            myCommand.ExecuteNonQuery();
            CloseConnection();
        }

        public void selectFromDatabase() //odczytywanie liczb z bazy danych
        {
            string query = "SELECT Liczba FROM Liczby";
            OpenConnection();
            SQLiteCommand myCommand = new SQLiteCommand(query, myConnection);
            SQLiteDataReader myDataReader = myCommand.ExecuteReader();

            selectedFromDatabase = new List<int>();
            while (myDataReader.Read())
            {
                selectedFromDatabase.Add(myDataReader.GetInt32(0));
            }
        }
    }
}
