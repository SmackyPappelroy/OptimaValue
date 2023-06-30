using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaValue.LogViewer;

public class LogFrequency : INotifyPropertyChanged
{
    private string message;
    public string Message
    {
        get { return message; }
        set
        {
            if (value != message)
            {
                message = value;
                OnPropertyChanged(nameof(Message));
            }
        }
    }

    private int frequency;
    public int Frequency
    {
        get { return frequency; }
        set
        {
            if (value != frequency)
            {
                frequency = value;
                OnPropertyChanged(nameof(Frequency));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

