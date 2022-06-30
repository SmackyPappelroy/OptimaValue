using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Geared;
using LiveCharts.Helpers;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OptimaValue.Config;
using System.Windows.Threading;
using LiveCharts.Wpf;
using System.Windows.Controls.Primitives;
using ClosedXML.Excel;
using System.Diagnostics;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using GongSolutions.Wpf.DragDrop;
using System.Collections;
using System.Collections.Specialized;
using LiveCharts.Definitions.Series;
using System.Globalization;

namespace OptimaValue.Wpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class GraphWindow : Window
{
    public event Action<bool> OnLoaded;
    public GraphWindow()
    {
        InitializeComponent();
        CultureInfo ci = new CultureInfo("sv-SE");
        Thread.CurrentThread.CurrentCulture = ci;
        Thread.CurrentThread.CurrentUICulture = ci;

        Loaded += GraphWindow_Loaded;
    }

    private void GraphWindow_Loaded(object sender, RoutedEventArgs e)
    {
        DataContext = new GraphViewModel();
        OnLoaded?.Invoke(true);
    }
}





