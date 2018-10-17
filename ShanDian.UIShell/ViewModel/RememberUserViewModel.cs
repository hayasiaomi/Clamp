using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.UIShell.ViewModel
{
    public class RememberUserViewModel : NotifyPropertyChanged
    {
        private string username;
        private int userId;
        private string password;
        private bool isMemberkMe;

        public string Username
        {
            get { return this.username; }
            set
            {
                if (this.username != value)
                {
                    this.username = value;
                    OnPropertyChanged("Username");
                }
            }
        }
        public int UserId
        {
            get { return this.userId; }
            set
            {
                if (this.userId != value)
                {
                    this.userId = value;
                    OnPropertyChanged("UserId");
                }
            }
        }
        public string Password
        {
            get { return this.password; }
            set
            {
                if (this.password != value)
                {
                    this.password = value;
                    OnPropertyChanged("Password");
                }
            }
        }

        public bool IsMemberkMe
        {
            get { return this.isMemberkMe; }
            set
            {
                if (this.isMemberkMe != value)
                {
                    this.isMemberkMe = value;
                    OnPropertyChanged("IsMemberkMe");
                }
            }
        }

    }
}
