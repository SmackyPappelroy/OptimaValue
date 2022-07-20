using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaValue.Wpf
{
    public class TagListControlViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private List<GridItem> gridItems = new();
        public List<GridItem> GridItems
        {
            get
            {
                if (gridItems.Count == 0)
                {
                    gridItems = (model ??= new()).GetGridItems();
                }
                return gridItems;
            }
            set
            {
                gridItems = value;
            }
        }

        private TagListControlModel model;


        public TagListControlViewModel()
        {
        }
    }
}
