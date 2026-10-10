using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintLogger.Models
{
    class Bill : INotifyPropertyChanged
    {
        private string _fromDate;
        private static Bill bill;

        private Bill()
        {
            this.LstErrors = new List<string>();
            this.CustomerName = "Taori";
            ////Bill.FromDate = Convert.ToString(DateTime.Now.AddDays(-39));
            ////Bill.ToDate = Convert.ToString(DateTime.Now.AddDays(-38));
            //this.FromDate = "09/24/2020";
            //this.ToDate = "09/25/2020";
            this.FromDate = Convert.ToString(DateTime.Now.AddDays(-7));
            this.ToDate = Convert.ToString(DateTime.Now);
            this.Jobs = new ObservableCollection<Job>();
        }
        public static Bill GetInstance()
        {
            if (bill != null)
            {
                return bill;
            }
            else
            {
                bill = new Bill();
                return bill;
            }
        }

        public static Bill DestroyInstance()
        {
            if (bill != null)
            {
                bill = null;
            }
            return null;
        }

        public string FromDate
        {
            get { return _fromDate; }
            set
            {
                _fromDate = value;
                OnPropertyRaised("FromDate");
            }
        }
        private string _toDate;
        public string ToDate
        {
            get { return _toDate; }
            set
            {
                _toDate = value;
                OnPropertyRaised("ToDate");
            }
        }

        private string _customerName;
        public string CustomerName
        {
            get
            {
                return this._customerName;
            }
            set
            {
                this._customerName = value;
                OnPropertyRaised("CustomerName");
            }
        }

        private List<string> lstErrors;
        public List<string> LstErrors
        {
            get
            {
                return this.lstErrors;
            }
            set
            {
                this.lstErrors = value;
                OnPropertyRaised("LstErrors");
            }
        }
        private ObservableCollection<Job> _jobs;
        public ObservableCollection<Job> Jobs
        {
            get { return _jobs; }
            set
            {
                //this.TotalAmount = Jobs.Sum(x => x.Amount);
                _jobs = value;
                OnPropertyRaised("Jobs");
            }
        }

        public int Quantity { get; set; }

        public double _totalAmount;
        public double TotalAmount
        {
            get
            {
                return _totalAmount;
                
            }
            set
            {
                _totalAmount = value;
                OnPropertyRaised("TotalAmount");
            }
        }

        public double TotalDimensions
        { get; set; }
        public int TotalQuantity { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }
    }

}
