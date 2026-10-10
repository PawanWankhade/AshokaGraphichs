using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PrintLogger.Models
{
    class PrintQualities:INotifyPropertyChanged
    {
        public string QualityCode { get; set; }
        private double _rate;
        public double Rate 
        { 
            get 
            {
                return _rate;
            }
            set
            {
                this._rate = value;
                OnPropertyRaised("Rate");
                
            }    
        }
        public string Quality { get; set; }

        public PrintQualities(string quality)
        {
            NameValueCollection configuration = null;
            switch (quality.ToLower())
            {
                case "local":
                    configuration = ConfigurationManager.GetSection("local") as NameValueCollection;
                    
                    break;
                case "star":
                    configuration = ConfigurationManager.GetSection("star") as NameValueCollection;
                    break;

                case "vinil":
                    configuration = ConfigurationManager.GetSection("vinil") as NameValueCollection;
                    break;

                case "backlite":
                    configuration = ConfigurationManager.GetSection("backlite") as NameValueCollection;
                    break;

                default:
                    break;
            }
            if (configuration != null)
            {
                this.Quality = configuration["name"];
                this.QualityCode = configuration["code"];
                this.Rate = Convert.ToDouble(configuration["rate"]);
            }
        }

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
