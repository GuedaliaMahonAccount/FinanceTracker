using System.Collections.ObjectModel;
using System.ComponentModel;
using Finance.Models;

namespace Finance.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Project> _projects = new ObservableCollection<Project>();

        public ObservableCollection<Project> Projects
        {
            get => _projects;
            set
            {
                if (_projects != value)
                {
                    // Se desabonner de l'ancienne collection
                    if (_projects != null)
                    {
                        _projects.CollectionChanged -= Projects_CollectionChanged;
                        foreach (var project in _projects)
                        {
                            project.PropertyChanged -= Project_PropertyChanged;
                        }
                    }

                    _projects = value;

                    // S'abonner a la nouvelle collection
                    if (_projects != null)
                    {
                        _projects.CollectionChanged += Projects_CollectionChanged;
                        foreach (var project in _projects)
                        {
                            project.PropertyChanged += Project_PropertyChanged;
                        }
                    }

                    OnPropertyChanged(nameof(Projects));
                    UpdateTotals();
                }
            }
        }

        public decimal TotalIncome => CalculateTotal(true);
        public decimal TotalExpenses => CalculateTotal(false);
        public decimal Total => TotalIncome - TotalExpenses;

        public MainViewModel()
        {
            _projects.CollectionChanged += Projects_CollectionChanged;
        }

        private void Projects_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // S'abonner aux nouveaux projets
            if (e.NewItems != null)
            {
                foreach (Project project in e.NewItems)
                {
                    project.PropertyChanged += Project_PropertyChanged;
                }
            }

            // Se desabonner des anciens projets
            if (e.OldItems != null)
            {
                foreach (Project project in e.OldItems)
                {
                    project.PropertyChanged -= Project_PropertyChanged;
                }
            }

            UpdateTotals();
        }

        private void Project_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Mettre a jour les totaux quand les totaux d'un projet changent
            if (e.PropertyName == nameof(Project.TotalIncome) ||
                e.PropertyName == nameof(Project.TotalExpenses) ||
                e.PropertyName == nameof(Project.Total))
            {
                UpdateTotals();
            }
        }

        private void UpdateTotals()
        {
            OnPropertyChanged(nameof(TotalIncome));
            OnPropertyChanged(nameof(TotalExpenses));
            OnPropertyChanged(nameof(Total));
        }

        private decimal CalculateTotal(bool isIncome)
        {
            decimal total = 0;
            foreach (var project in Projects)
            {
                total += isIncome ? project.TotalIncome : project.TotalExpenses;
            }
            return total;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}