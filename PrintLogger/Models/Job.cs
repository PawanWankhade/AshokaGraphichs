using PrintLogger.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintLogger.Models
{
    class Job : BillViewModel,INotifyPropertyChanged
    {

        public int? Id { get; set; }
        public string JobDate { get; set; }
        public string CustomerName { get; set; }
        public string JobName { get; set; }
        public int Quantity { get; set; }
        public string FilePath { get; set; }
        public string PercentComplete { get; set; }
        public string JobStartTime { get; set; }
        public string JobEndTime { get; set; }
        public string TimeRequired { get; set; }

        public static double PrevRate { get; set; }
        public static double PrevAmount { get; set; }

        private static double _totalAmount;
        public static double TotalAmount
        {
            get
            {
                return _totalAmount;
            }
            set
            {
                _totalAmount = value;

            }
        }


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
                OnPropertyRaised("Amount");
            }
        }

        private PrintQualities _quality;
        public PrintQualities Quality
        {
            get
            {
                return _quality;
            }
            set
            {
                _quality = value;
                //OnPropertyRaised("Quality
                OnPropertyRaised("Amount");
            }
        }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Dimension { get; set; }
        public double _amount { get; set; }
        public double Amount
        {
            get
            {
                if (this.Quality != null && this.Dimension != 0)
                {
                    this._amount = (this.Rate) * this.Dimension;
                    return (this.Rate) * this.Dimension;

                }
                else
                    return _amount;
            }
            set
            {
                this._amount = value;
                OnPropertyRaised("Amount");
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

        public bool CheckFileExists(string path)
        {
            return File.Exists(path);
        }

        private string[] ReadFileContent()
        {
            string path = ConfigurationManager.AppSettings.Get("filepath");
            if ((path != string.Empty || path != null) && File.Exists(path))
            {
                string[] fileContent = File.ReadAllLines(path);
                Array.Reverse(fileContent);
                return fileContent;
            }
            return null;
        }

        public bool ChangeFileAttribute(string path)
        {
            //try
            //{
            //    System.Diagnostics.Process process = new System.Diagnostics.Process();
            //    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            //    startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            //    startInfo.FileName = "cmd.exe";
            //    startInfo.Arguments = "/C copy /b Image1.jpg + Archive.rar Image2.jpg";
            //    process.StartInfo = startInfo;
            //    process.Start();
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            return false;
        }

        public List<Job> GetAllJobs()
        {
            List<Job> jobs = null;
            try
            {
                string path = ConfigurationManager.AppSettings.Get("filepath");
                if ((path != string.Empty || path != null) && File.Exists(path))
                {

                    var FileContent = this.ReadFileContent();
                    if (FileContent != null)
                    {
                        int count = 1;
                        jobs = new List<Job>();
                        foreach (var item in FileContent)
                        {
                            int id = count++;
                            string[] ar = item.Split(';');
                            //extract Filepath
                            string arrElementObj = ar[0];
                            //string filePath = arrElementObj.Substring(arrElementObj.IndexOf(":\\") - 1, arrElementObj.Length - 10);
                            string filePath = arrElementObj.Substring(arrElementObj.IndexOf(":\\") - 1);
                            string jobEndTime = arrElementObj.Substring(0, arrElementObj.IndexOf(":\\") - 2);


                            //extract date and time
                            arrElementObj = ar[1];
                            string[] temp = arrElementObj.Split('-');
                            //DateTime jobDate = Convert.ToDateTime(temp[0] + " " + temp[1]);
                            string jobDate = temp[0];
                            string jobTime = temp[1];

                            //extract time required 
                            string timeRequired = ar[2];

                            //extract percent complete
                            string percentComplete = ar[3];

                            jobs.Add(new Job { Id = id, JobDate = jobDate, JobStartTime = jobTime, JobEndTime = jobEndTime, FilePath = filePath, TimeRequired = timeRequired, PercentComplete = percentComplete });

                        }

                    }
                    else
                    {
                        return null;
                    }
                }
                return jobs;
            }
            catch (Exception)
            {
                throw;
            }

        }

    }

}
