using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaValue.Wpf
{
    //public class BindingList<T> : ObservableCollection<T> where T : INotifyPropertyChanged
    //{
    //    public BindingList(IEnumerable<T> initialData) : base(initialData)
    //    {
    //        Init();
    //    }

    //    public BindingList()
    //    {
    //        Init();
    //    }

    //    private void Init()
    //    {
    //        foreach (T item in Items)
    //            item.PropertyChanged += ItemOnPropertyChanged;

    //        CollectionChanged += FullObservableCollectionCollectionChanged;
    //    }

    //    private void FullObservableCollectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    //    {
    //        if (e.NewItems != null)
    //        {
    //            foreach (T item in e.NewItems)
    //            {
    //                if (item != null)
    //                    item.PropertyChanged += ItemOnPropertyChanged;
    //            }
    //        }

    //        if (e.OldItems != null)
    //        {
    //            foreach (T item in e.OldItems)
    //            {
    //                if (item != null)
    //                    item.PropertyChanged -= ItemOnPropertyChanged;
    //            }
    //        }
    //    }

    //    private void ItemOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    //        => ItemChanged?.Invoke(sender, e);

    //    public event PropertyChangedEventHandler ItemChanged;
    //}
}
