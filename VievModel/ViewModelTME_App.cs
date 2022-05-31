using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace TME_App.VievModel
{
    using Model;
    using System.Windows.Input;

    public class ViewModelTME_App : INotifyPropertyChanged
    {
        private ModelTME_App model = new ModelTME_App();

        public string inputText //text wejściowy
        {
            get
            { 
                return model.inputText;
            }
            set
            {
                model.inputText = value; 
                onPropertyChanged(nameof(inputText));
            }
        }

        public Boolean correctNumber
        {
            get { return model.correctNumber; }
        }

        public int quantityOfNumbers
        {
            get { return model.quantityOfNumbers; }
        }

        public double progressBarValue1;
        public double progressBarValue //wartość probressbara
        {
            get
            {
                return progressBarValue1;
            }
            set { progressBarValue1 = value; }
        }

        public string quantityOfNumbersYouCanRandMessage //wiadomość skierowana do użytkownika przekazywana do widoku
        {
            get { return model.quantityOfNumbersYouCanRandMessage; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void onPropertyChanged(string nameOfProperty) //pozwala poinformować widok o zmianie wartości danej zmiennej aby ją zaktualizować w widoku
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(nameOfProperty));
            }
        }

        private ICommand inputTextToInt = null;

        public ICommand InputTextToInt //komenda drugiego przycisku po której następuje walidacja inputText a następnie losowanie liczb
        {
            get
            {
                if(inputTextToInt == null)
                {
                    inputTextToInt = new RelayCommand(
                        (object o) =>
                        {
                            model.inputTextToInt(inputText);
                            onPropertyChanged(nameof(quantityOfNumbersYouCanRandMessage)); //informuje widok o zmianie wiadomosci którą wyświetla text block
                            work(); //rozpoczyna proces losowania
                        },
                        (object o) =>
                        {
                            return progressBarValue >= 0; //warunek kiedy button będzie pozwalał się wcisnąć
                        });
                }
                return inputTextToInt;
            }
        }

        private ICommand loadDatabase = null;

        public ICommand LoadDatabase //komenda pierwszego przycisku pozwalająca na wczytanie danych z bazy
        {
            get
            {
                if (loadDatabase == null)
                {
                    loadDatabase = new RelayCommand(
                        (object o) =>
                        {
                            model.loadDatabase();
                            onPropertyChanged(nameof(quantityOfNumbersYouCanRandMessage));
                        },
                        (object o) =>
                        {
                            return progressBarValue >= 0; //warunek kiedy button będzie pozwalał się wcisnąć
                        });
                }
                return loadDatabase;
            }
        }

        public async void work()
        {
            RandNumbers randNumbers = new RandNumbers(quantityOfNumbers);
            var progress = new Progress<double>(value =>
            {
                progressBarValue = value;
                onPropertyChanged(nameof(progressBarValue));
            });
            if(correctNumber == true) //jeśli liczba jest poprawna rozpoczynam losowanie
            {
                await Task.Run(() => randNumbers.RandAllNumbers(progress));
            }
        }

        class RandNumbers //klasa odpowiedzialna za losowanie unikatowych liczb i zapis do bazy
        {
            private int quantityOfNumbers;
            public RandNumbers(int quantityOfNumbers)
            {
                this.quantityOfNumbers = quantityOfNumbers;
            }

            Database database = new Database();
            System.Random random = new System.Random();

            public void RandAllNumbers(IProgress<double> progress)
            {
                for (int i = 0; i < quantityOfNumbers; i++)
                {
                    int number = random.Next(1000000, 9999999);
                    if (!database.selectedFromDatabase.Contains(number))
                    {
                        database.insertIntoDatabase(number);
                        database.selectedFromDatabase.Add(number);
                        double progres = ((double)i / quantityOfNumbers) * 100;
                        progress.Report(progres);
                    }
                }
                progress.Report(100);
            }
        }
    }
}
