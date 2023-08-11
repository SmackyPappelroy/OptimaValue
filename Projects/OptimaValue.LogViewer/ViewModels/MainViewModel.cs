using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Globalization;
using FileLogger;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OptimaValue.LogViewer;

public class MainViewModel : INotifyPropertyChanged
{

    public ICommand SortCommand { get; private set; }
    public ICommand OpenFileCommand { get; private set; }
    public List<string> LogLevels { get; } = new List<string> { "Alla", "Error", "Warning", "Information", "Debug" }; // Add all log levels that are present in your log files
    public ObservableCollection<LogFrequency> LogFrequencies { get; set; } = new ObservableCollection<LogFrequency>();

    private ObservableCollection<LogTemplate> _logEntries;
    public ObservableCollection<LogTemplate> LogEntries
    {
        get { return _logEntries; }
        set
        {
            _logEntries = value;
            OnPropertyChanged(nameof(LogEntries));
        }
    }

    private string fileName = string.Empty;
    public string FileName
    {
        get { return fileName; }
        set
        {
            fileName = value;
            OnPropertyChanged(nameof(FileName));
        }
    }

    private DateTime _startDate;
    public DateTime StartDate
    {
        get { return _startDate; }
        set
        {
            if (_startDate != value)
            {
                _startDate = value;
                OnPropertyChanged(nameof(StartDate));
                UpdateFilteredEntries();
            }
        }
    }

    private DateTime _endDate;
    public DateTime EndDate
    {
        get { return _endDate; }
        set
        {
            if (_endDate != value)
            {
                _endDate = value;
                OnPropertyChanged(nameof(EndDate));
                UpdateFilteredEntries();
            }
        }
    }


    private ObservableCollection<LogTemplate> _filteredEntries;
    public ObservableCollection<LogTemplate> FilteredEntries
    {
        get { return _filteredEntries; }
        set
        {
            _filteredEntries = value;
            OnPropertyChanged(nameof(FilteredEntries));
        }
    }

    private string _sortArrow;
    public string SortArrow
    {
        get { return _sortArrow; }
        set
        {
            _sortArrow = value;
            OnPropertyChanged(nameof(SortArrow));
        }
    }


    private string _searchTerm = "";
    public string SearchTerm
    {
        get { return _searchTerm; }
        set
        {
            if (value != _searchTerm)
            {
                _searchTerm = value;
                OnPropertyChanged(nameof(SearchTerm));
                UpdateFilteredEntries();
            }
        }
    }

    public MainViewModel()
    {
        LogEntries = new ObservableCollection<LogTemplate>();
        FilteredEntries = new ObservableCollection<LogTemplate>();
        OpenFileCommand = new RelayCommand(SelectFile);
        SortCommand = new RelayCommand(SortLogEntries);
        SelectedLogLevel = LogLevels.FirstOrDefault();
        FileName = Properties.Settings.Default.LastOpenedFile;
        FileName = @"C:\OptimaValue\Log.json";
        if (!string.IsNullOrWhiteSpace(FileName))
        {
            UpdateList();
        }
    }

    private void UpdateList()
    {
        LoadLogFile();
        UpdateLogFrequencies();
    }

    private void SelectFile()
    {
        var openFileDialog = new Microsoft.Win32.OpenFileDialog();
        openFileDialog.Filter = "Text files (*.txt)|*.txt|Log files (*.log)|*.log|All files (*.*)|*.*";
        openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        if (openFileDialog.ShowDialog() == true)
        {
            FileName = openFileDialog.FileName;
            Properties.Settings.Default.LastOpenedFile = FileName; // save the file name
            Properties.Settings.Default.Save(); // persist the settings
            UpdateList();
        }
    }

    private bool isOrderbyAscending = true;
    private void SortLogEntries()
    {
        if (isOrderbyAscending)
        {
            FilteredEntries = new ObservableCollection<LogTemplate>(FilteredEntries.OrderByDescending(entry => entry.DateTime)
                .Where(e => e.DateTime >= StartDate && e.DateTime <= EndDate));
            if (FilteredEntries.Count == 0)
                FilteredEntries = new ObservableCollection<LogTemplate>(FilteredEntries.OrderByDescending(entry => entry.DateTime));
            isOrderbyAscending = false;
            SortArrow = "↓";
        }
        else
        {
            FilteredEntries = new ObservableCollection<LogTemplate>(FilteredEntries.OrderBy(entry => entry.DateTime)
                .Where(e => e.DateTime >= StartDate && e.DateTime <= EndDate));
            if (FilteredEntries.Count == 0)
                FilteredEntries = new ObservableCollection<LogTemplate>(FilteredEntries.OrderBy(entry => entry.DateTime));
            isOrderbyAscending = true;
            SortArrow = "↑";
        }
        OnPropertyChanged(nameof(LogEntries));
    }



    private void UpdateLogFrequencies()
    {
        LogFrequencies.Clear();
        var groupedLogs = FilteredEntries.GroupBy(l => l.Message);
        groupedLogs = groupedLogs.OrderByDescending(g => g.Count());
        foreach (var group in groupedLogs)
        {
            LogFrequencies.Add(new LogFrequency { Message = group.Key, Frequency = group.Count() });
        }
        OnPropertyChanged(nameof(LogFrequencies));
    }

    private string _selectedLogLevel;
    public string SelectedLogLevel
    {
        get { return _selectedLogLevel; }
        set
        {
            if (_selectedLogLevel != value)
            {
                _selectedLogLevel = value;
                OnPropertyChanged(nameof(SelectedLogLevel));
                FilterLogEntries();
            }
        }
    }

    private void FilterLogEntries()
    {
        if (!string.IsNullOrEmpty(SelectedLogLevel))
        {
            if (SelectedLogLevel == "All")
                FilteredEntries = new ObservableCollection<LogTemplate>(LogEntries);
            else
                FilteredEntries = new ObservableCollection<LogTemplate>(LogEntries.Where(entry => entry.Level.ToString() == SelectedLogLevel
                && entry.DateTime >= StartDate && entry.DateTime <= EndDate));
            OnPropertyChanged(nameof(LogEntries));
        }
    }

    private void LoadLogFile()
    {
        var entries = FileLog.ReadLogs(FileName);

        foreach (var entry in entries)
        {
            LogEntries.Add(entry);
        }
        DateTime tempStartDate;
        DateTime tempEndDate;


        StartDate = LogEntries.Min(x => x.DateTime);
        EndDate = LogEntries.Max(x => x.DateTime);

        UpdateFilteredEntries();
    }



    private void UpdateFilteredEntries()
    {
        if (string.IsNullOrEmpty(SearchTerm))
            FilteredEntries = new ObservableCollection<LogTemplate>(LogEntries.Where(e => e.DateTime >= StartDate &&
                           e.DateTime <= EndDate));
        else
            FilteredEntries = new ObservableCollection<LogTemplate>(
                LogEntries.Where(e =>
                    e.Message.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) &&
                    e.DateTime >= StartDate &&
                    e.DateTime <= EndDate));

        if (isOrderbyAscending)
            FilteredEntries = new ObservableCollection<LogTemplate>(FilteredEntries.OrderBy(entry => entry.DateTime));
        else
            FilteredEntries = new ObservableCollection<LogTemplate>(FilteredEntries.OrderByDescending(entry => entry.DateTime));
        UpdateLogFrequencies();
    }



    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }



}


