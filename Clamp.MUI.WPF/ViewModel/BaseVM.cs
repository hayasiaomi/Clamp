using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Clamp.MUI.WPF.ViewModel
{
    public abstract class BaseVM : INotifyPropertyChanged
    {
       
        public event PropertyChangedEventHandler PropertyChanged;

       
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
