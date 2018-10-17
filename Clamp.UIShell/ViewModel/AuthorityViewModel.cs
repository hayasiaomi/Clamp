using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Clamp.UIShell.ViewModel
{
    public class AuthorityViewModel : NotifyPropertyChanged
    {
        private ObservableCollection<RememberUserViewModel> rememberUsers = new ObservableCollection<RememberUserViewModel>();

        public ObservableCollection<RememberUserViewModel> RememberUsers
        {
            get { return this.rememberUsers; }
            set
            {
                if (this.rememberUsers != value)
                {
                    this.rememberUsers = value;
                    OnPropertyChanged("RememberUsers");
                }
            }
        }
    }
}
