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

namespace OptimaValue.LogViewer;

public class MainViewModel : INotifyPropertyChanged
{
    private string fileName = string.Empty;
    public ICommand SortCommand { get; private set; }
    public ICommand OpenFileCommand { get; private set; }
    public List<string> LogLevels { get; } = new List<string> { "Alla", "Error", "Warning", "Information", "Debug"  }; // Add all log levels that are present in your log files
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


    private string _searchTerm;
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
        if (!string.IsNullOrWhiteSpace(fileName))
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
            fileName = openFileDialog.FileName;
            UpdateList();
        }
    }

    private bool isOrderbyAscending = true;
    private void SortLogEntries()
    {
        if (isOrderbyAscending)
        {
            FilteredEntries = new ObservableCollection<LogEntry>(FilteredEntries.OrderByDescending(entry => entry.LogDate));
            isOrderbyAscending = false;
            SortArrow = "↓";
        }
        else
        {
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
            FilteredEntries = new ObservableCollection<LogEntry>(LogEntries.Where(entry => entry.LogLevel == SelectedLogLevel));
            OnPropertyChanged(nameof(LogEntries));
        }
    }

    private void LoadLogFile()
    {
        string logFileContent = File.ReadAllText(fileName);

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

        UpdateFilteredEntries();
    }


    private void UpdateFilteredEntries()
    {
        if (string.IsNullOrEmpty(SearchTerm))
        {
            FilteredEntries = new ObservableCollection<LogEntry>(LogEntries);
        }
        else
        {
            FilteredEntries = new ObservableCollection<LogEntry>(
                LogEntries.Where(e => e.Message.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
            );
        }
        UpdateLogFrequencies();
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }



}


