using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ValidityTakeHome.Models;
using CsvHelper;
using System.IO;
using System.ComponentModel;
using System.Linq;

namespace ValidityTakeHome.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Person> _PotentialDuplicates;
        public ObservableCollection<Person> PotentialDuplicates
        {
            get { return _PotentialDuplicates; }
            set
            {
                _PotentialDuplicates = value;
                OnPropertyChanged("PotentialDuplicates");
            }
        }

        private ObservableCollection<Person> _NonDuplicates;
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
            var openDialog = new OpenFileDialog();
            openDialog.Title = "Select a CSV file";
            openDialog.Filter = "CSV Files (*.csv)|*.csv";

            var result = openDialog.ShowDialog();

            if (result.HasValue && result.Value)
            {
                var csvFilePath = openDialog.FileName;

                using (var streamReader = new StreamReader(csvFilePath))
                {
                    using (var csv = new CsvReader(streamReader))
                    {
                        var records = csv.GetRecords<Person>().ToList();
                        NonDuplicates = new ObservableCollection<Person>(records);
                    }
                }
            }
        }
    }
}
