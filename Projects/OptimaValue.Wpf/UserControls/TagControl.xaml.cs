using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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

namespace OptimaValue.Wpf
{
    /// <summary>
    /// Interaction logic for TagControl.xaml
    /// </summary>
    public partial class TagControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        public static readonly DependencyProperty TagColorProperty = DependencyProperty.Register("TagColor",
        typeof(LinearGradientBrush), typeof(TagControl));

        public static readonly DependencyProperty TagNameProperty = DependencyProperty.Register("TagName",
        typeof(string), typeof(TagControl));

        public static readonly DependencyProperty TagUnitProperty = DependencyProperty.Register("TagUnit",
        typeof(string), typeof(TagControl));

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description",
        typeof(string), typeof(TagControl));

        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue",
        typeof(string), typeof(TagControl));


        public static readonly DependencyProperty AverageValueProperty = DependencyProperty.Register("AverageValue",
        typeof(string), typeof(TagControl));

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue",
        typeof(string), typeof(TagControl));


        public static readonly DependencyProperty IntegralProperty = DependencyProperty.Register("Integral",
        typeof(string), typeof(TagControl));

        public static readonly DependencyProperty IntegralPerTimmeProperty = DependencyProperty.Register("IntegralPerTimme",
        typeof(string), typeof(TagControl));

        public static readonly DependencyProperty OverZeroProperty = DependencyProperty.Register("OverZero",
        typeof(string), typeof(TagControl));

        public static readonly DependencyProperty OverZeroTimeProperty = DependencyProperty.Register("OverZeroTime",
        typeof(string), typeof(TagControl));

        public static readonly DependencyProperty DeviationProperty = DependencyProperty.Register("Deviation",
        typeof(string), typeof(TagControl));

        public static readonly DependencyProperty ActualValueProperty = DependencyProperty.Register("ActualValue",
        typeof(string), typeof(TagControl));



        public LinearGradientBrush TagColor
        {
            get => (LinearGradientBrush)GetValue(TagColorProperty);
            set
            {
                SetValue(TagColorProperty, value);
                OnPropertyChanged();
            }
        }

        public string TagName
        {
            get => (string)GetValue(TagNameProperty);
            set
            {
                SetValue(TagNameProperty, value);
                OnPropertyChanged();
            }
        }

        public string TagUnit
        {
            get => (string)GetValue(TagUnitProperty);
            set
            {
                SetValue(TagUnitProperty, value);
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set
            {
                SetValue(DescriptionProperty, value);
                if (string.IsNullOrEmpty(value))
                {
                    DescriptionText.Visibility = Visibility.Collapsed;
                    //this.UpdateLayout();
                }
                else
                {
                    DescriptionText.Visibility = Visibility.Visible;

                    //this.UpdateLayout();
                }
                OnPropertyChanged();
            }
        }

        public string MinValue
        {
            get => (string)GetValue(MinValueProperty);
            set
            {
                SetValue(MinValueProperty, value);
                OnPropertyChanged();
            }
        }


        public string AverageValue
        {
            get => (string)GetValue(AverageValueProperty);
            set
            {
                SetValue(AverageValueProperty, value);
                OnPropertyChanged();
            }
        }

        public string MaxValue
        {
            get => (string)GetValue(MaxValueProperty);
            set
            {
                SetValue(MaxValueProperty, value);
                OnPropertyChanged();
            }
        }

        public string Integral
        {
            get => (string)GetValue(IntegralProperty);
            set
            {
                SetValue(IntegralProperty, value);
                OnPropertyChanged();
            }
        }

        public string IntegralPerTimme
        {
            get => (string)GetValue(IntegralPerTimmeProperty);
            set
            {
                SetValue(IntegralPerTimmeProperty, value);
                OnPropertyChanged();
            }
        }

        public string OverZero
        {
            get => (string)GetValue(OverZeroProperty);
            set
            {
                SetValue(OverZeroProperty, value);
                OnPropertyChanged();
            }
        }

        public string OverZeroTime
        {
            get => (string)GetValue(OverZeroTimeProperty);
            set { SetValue(OverZeroTimeProperty, value); OnPropertyChanged(); }
        }

        public string Deviation
        {
            get => (string)GetValue(DeviationProperty);
            set { SetValue(DeviationProperty, value); OnPropertyChanged(); }
        }

        public string ActualValue
        {
            get => (string)GetValue(ActualValueProperty);
            set { SetValue(ActualValueProperty, value); OnPropertyChanged(); }
        }


        public TagControl()
        {
            InitializeComponent();
        }


    }
}
