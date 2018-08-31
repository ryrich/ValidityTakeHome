using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ValidityTakeHome.Models;
using CsvHelper;
using System.IO;
using System.ComponentModel;
using System.Linq;
using Fastenshtein;

namespace ValidityTakeHome.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private const int LEVENSHTEIN_THRESHOLD = 2;

        private ObservableCollection<Person> _PotentialDuplicates = new ObservableCollection<Person>();
        public ObservableCollection<Person> PotentialDuplicates
        {
            get { return _PotentialDuplicates; }
            set
            {
                _PotentialDuplicates = value;
                OnPropertyChanged("PotentialDuplicates");
            }
        }

        private ObservableCollection<Person> _NonDuplicates = new ObservableCollection<Person>();
        public ObservableCollection<Person> NonDuplicates
        {
            get { return _NonDuplicates; }
            set
            {
                _NonDuplicates = value;
                OnPropertyChanged("NonDuplicates");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        private ICommand _ImportCsvCommand;
        public ICommand ImportCsvCommand
        {
            get
            {
                if (_ImportCsvCommand == null)
                {
                    _ImportCsvCommand = new RelayCommand(ExecuteImportCsvCommand);
                }

                return _ImportCsvCommand;
            }
        }

        public void ExecuteImportCsvCommand(object args)
        {
            var openDialog = new OpenFileDialog
            {
                Title = "Select a CSV file",
                Filter = "CSV Files (*.csv)|*.csv"
            };

            var result = openDialog.ShowDialog();

            if (result.HasValue && result.Value)
            {
                PotentialDuplicates.Clear();
                NonDuplicates.Clear();

                var csvFilePath = openDialog.FileName;

                using (var streamReader = new StreamReader(csvFilePath))
                {
                    using (var csv = new CsvReader(streamReader))
                    {
                        // ToList loads whole file into memory
                        // for scalability, remove ToList to load one record into memory
                        // at a time.
                        var records = csv.GetRecords<Person>().ToList();

                        for (int i = 0; i < records.Count; i++)
                        {
                            var primaryRecord = records[i];

                            // if this is marked as a duplicate already, we don't need to check any further
                            if (primaryRecord.isDuplicate) continue;

                            for (int j = i + 1; j < records.Count; j++)
                            {
                                var secondaryRecord = records[j];

                                if (ArePotentialDuplicates(primaryRecord, secondaryRecord))
                                {
                                    // make sure we don't double dip
                                    if (!primaryRecord.isDuplicate)
                                    {
                                        primaryRecord.isDuplicate = true;
                                        PotentialDuplicates.Add(primaryRecord);
                                    }

                                    secondaryRecord.isDuplicate = true;
                                    PotentialDuplicates.Add(secondaryRecord);
                                }
                            }

                            // if the primary record has no duplicates, it is a non duplicate
                            if (!primaryRecord.isDuplicate)
                            {
                                NonDuplicates.Add(primaryRecord);
                            }
                        }
                    }
                }
            }
        }

        private bool ArePotentialDuplicates(Person p1, Person p2)
        {
            var firstNamesEqual = p1.first_name.Equals(p2.first_name, System.StringComparison.OrdinalIgnoreCase);
            var lastNamesEqual = p1.last_name.Equals(p2.last_name, System.StringComparison.OrdinalIgnoreCase);

            // if the full name is exactly equal, no need to compute levenshtein
            if (firstNamesEqual && lastNamesEqual) return true;

            // no need to compute levenshtein if first names are exactly equal
            if (firstNamesEqual)
            {
                var lastNameLevenshtein = Levenshtein.Distance(p1.last_name, p2.last_name);
                return lastNameLevenshtein <= LEVENSHTEIN_THRESHOLD;
            }
            // no need to compute levenshtein if last names are exactly equal
            else if (lastNamesEqual)
            {
                var firstNameLevenshtein = Levenshtein.Distance(p1.first_name, p2.first_name);
                return firstNameLevenshtein <= LEVENSHTEIN_THRESHOLD;
            }
            else
            {
                var firstNameLevenshtein = Levenshtein.Distance(p1.first_name, p2.first_name);
                var lastNameLevenshtein = Levenshtein.Distance(p1.last_name, p2.last_name);

                return firstNameLevenshtein <= LEVENSHTEIN_THRESHOLD && lastNameLevenshtein <= LEVENSHTEIN_THRESHOLD;
            }
        }
    }
}
