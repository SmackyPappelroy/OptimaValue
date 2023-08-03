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

namespace OptimaValue.LogViewer;

public class MainViewModel : INotifyPropertyChanged
{

    public ICommand SortCommand { get; private set; }
    public ICommand OpenFileCommand { get; private set; }
    public List<string> LogLevels { get; } = new List<string> { "Alla", "Error", "Warning", "Information", "Debug" }; // Add all log levels that are present in your log files
    public ObservableCollection<LogFrequency> LogFrequencies { get; set; } = new ObservableCollection<LogFrequency>();

    private ObservableCollection<LogEntry> _logEntries;
    public ObservableCollection<LogEntry> LogEntries
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


    private ObservableCollection<LogEntry> _filteredEntries;
    public ObservableCollection<LogEntry> FilteredEntries
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
        LogEntries = new ObservableCollection<LogEntry>();
        FilteredEntries = new ObservableCollection<LogEntry>();
        OpenFileCommand = new RelayCommand(SelectFile);
        SortCommand = new RelayCommand(SortLogEntries);
        SelectedLogLevel = LogLevels.FirstOrDefault();
        FileName = Properties.Settings.Default.LastOpenedFile;
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
            FilteredEntries = new ObservableCollection<LogEntry>(FilteredEntries.OrderByDescending(entry => entry.LogDate)
                .Where(e => DateTime.TryParseExact(e.LogDate, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.None, out _)
                && DateTime.TryParseExact(e.LogDate, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate)
                && parsedDate >= StartDate && parsedDate <= EndDate));
            if (FilteredEntries.Count == 0)
                FilteredEntries = new ObservableCollection<LogEntry>(FilteredEntries.OrderByDescending(entry => entry.LogDate));
            isOrderbyAscending = false;
            SortArrow = "↓";
        }
        else
        {
            FilteredEntries = new ObservableCollection<LogEntry>(FilteredEntries.OrderBy(entry => entry.LogDate)
                .Where(e => DateTime.TryParseExact(e.LogDate, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.None, out _)
                && DateTime.TryParseExact(e.LogDate, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate)
                && parsedDate >= StartDate && parsedDate <= EndDate));
            if (FilteredEntries.Count == 0)
                FilteredEntries = new ObservableCollection<LogEntry>(FilteredEntries.OrderBy(entry => entry.LogDate));
            isOrderbyAscending = true;
            SortArrow = "↑";
        }
        OnPropertyChanged(nameof(LogEntries));
    }



    private void UpdateLogFrequencies()
    {
        LogFrequencies.Clear();
        var replaceMessageWithExceptionIfMessageIsEmpty = FilteredEntries.Select(entry => new LogEntry
        {
            LogLevel = entry.LogLevel,
            LogDate = entry.LogDate,
            FilePath = entry.FilePath,
            Method = entry.Method,
            LineNumber = entry.LineNumber,
            Message = string.IsNullOrWhiteSpace(entry.Message) ? entry.Exception : entry.Message,
            Exception = entry.Exception
        });
        var groupedLogs = replaceMessageWithExceptionIfMessageIsEmpty.GroupBy(l => l.Message);
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
                FilteredEntries = new ObservableCollection<LogEntry>(LogEntries);
            else
                FilteredEntries = new ObservableCollection<LogEntry>(LogEntries.Where(entry => entry.LogLevel == SelectedLogLevel
                && entry.LogDateTime >= StartDate && entry.LogDateTime <= EndDate));
            OnPropertyChanged(nameof(LogEntries));
        }
    }

    private void LoadLogFile()
    {
        string logFileContent = File.ReadAllText(FileName);

        string pattern = @"\[(.*?)\]\s*(.*?)\r\n\[(.*?)\]\r\n(.*?)\r\nLine number: \[(.*?)\]\r\n((?:(?!\[Error\]|Meddelande:).)*)(Meddelande: (.*?))?\r\n\-{60}";

        var matches = Regex.Matches(logFileContent, pattern, RegexOptions.Singleline);

        foreach (Match match in matches)
        {
            var logEntry = new LogEntry
            {
                LogLevel = match.Groups[1].Value,
                LogDate = match.Groups[2].Value,
                FilePath = match.Groups[3].Value,
                Method = match.Groups[4].Value,
                LineNumber = match.Groups[5].Value,
                Message = match.Groups[8].Value,
                Exception = match.Groups[6].Value.Trim()
            };


            // If any of the extracted strings are null, skip adding to LogEntries collection
            if (string.IsNullOrEmpty(logEntry.LogLevel) || string.IsNullOrEmpty(logEntry.LogDate) || string.IsNullOrEmpty(logEntry.FilePath)
                || string.IsNullOrEmpty(logEntry.Method) || string.IsNullOrEmpty(logEntry.LineNumber) || (string.IsNullOrEmpty(logEntry.Message) && string.IsNullOrEmpty(logEntry.Exception)))
                continue;

            LogEntries.Add(logEntry);

        }
        DateTime tempStartDate;
        DateTime tempEndDate;

        DateTime.TryParseExact(LogEntries.Min(e => e.LogDate), "yyyy-MM-dd HH:mm:ss.fff",
            CultureInfo.InvariantCulture, DateTimeStyles.None, out tempStartDate);

        DateTime.TryParseExact(LogEntries.Max(e => e.LogDate), "yyyy-MM-dd HH:mm:ss.fff",
            CultureInfo.InvariantCulture, DateTimeStyles.None, out tempEndDate);

        StartDate = tempStartDate;
        EndDate = tempEndDate;

        UpdateFilteredEntries();
    }


    private void UpdateFilteredEntries()
    {
        if (string.IsNullOrEmpty(SearchTerm))
            FilteredEntries = new ObservableCollection<LogEntry>(LogEntries.Where(e =>
                           DateTime.ParseExact(e.LogDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) >= StartDate &&
                           DateTime.ParseExact(e.LogDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) <= EndDate));
        else
            FilteredEntries = new ObservableCollection<LogEntry>(
                LogEntries.Where(e =>
                    e.Message.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) &&
                    DateTime.ParseExact(e.LogDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) >= StartDate &&
                    DateTime.ParseExact(e.LogDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) <= EndDate));

        if (isOrderbyAscending)
            FilteredEntries = new ObservableCollection<LogEntry>(FilteredEntries.OrderBy(entry => entry.LogDate));
        else
            FilteredEntries = new ObservableCollection<LogEntry>(FilteredEntries.OrderByDescending(entry => entry.LogDate));
        UpdateLogFrequencies();
    }



    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }



}


