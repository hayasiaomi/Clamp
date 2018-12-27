using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Clamp.MUI.WPF.ViewModel
{
    public class AuthorityVM : BaseVM
    {
        private string password;
        private string username;
        private string mistake;
        private bool isFault = false;
        private bool isLogining = false;

        public string Mistake
        {
            get { return this.mistake; }
            set
            {
                if (this.mistake != value)
                {
                    this.mistake = value;

                    if (!string.IsNullOrWhiteSpace(this.mistake))
                    {
                        this.IsFault = true;
                    }
                    OnPropertyChanged("Mistake");
                }
            }
        }

        public bool IsFault
        {
            get
            {
                return this.isFault;
            }
            set
            {
                if (this.isFault != value)
                {
                    this.isFault = value;
                    OnPropertyChanged("IsFault");
                }
            }
        }

        public bool IsLogining
        {
            get
            {
                return this.isLogining;
            }
            set
            {
                if (this.isLogining != value)
                {
                    this.isLogining = value;

                    OnPropertyChanged("IsLogining");
                }
            }
        }

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

        public void Login()
        {
            this.ClearFault();

            this.IsLogining = true;

            if (string.IsNullOrWhiteSpace(this.Username))
            {
                this.Mistake = "用户名不能为空";
                this.IsLogining = false;
                return;
            }
            else if (string.IsNullOrWhiteSpace(this.Password))
            {
                this.Mistake = "密码不能为空";
                this.IsLogining = false;
                return;
            }

            Thread thread = new Thread(new ThreadStart(() =>
            {
                Thread.Sleep(3000);
                this.Mistake = "用户或密码不正确";
                this.IsLogining = false;

            }));

            thread.IsBackground = true;
            thread.Start();
        }

        public void ClearFault()
        {
            if (!string.IsNullOrWhiteSpace(this.Mistake))
                this.Mistake = string.Empty;
        }
    }
}
