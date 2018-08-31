using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValidityTakeHome.Models;

namespace ValidityTakeHome.ViewModels
{
    public class MainWindowViewModel
    {
        public ObservableCollection<Person> PotentialDuplicates { get; set; }
        public ObservableCollection<Person> NonDuplicates { get; set; }
    }
}
