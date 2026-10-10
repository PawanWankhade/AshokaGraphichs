using PrintLogger.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Data;
using System.Reflection;
using System.Drawing;
using System.Text.RegularExpressions;

namespace PrintLogger.ViewModels
{
    class BillViewModel : INotifyPropertyChanged
    {
        string baseFilePath = ConfigurationManager.AppSettings.Get("jobFilepath");
        private Job _selectedJob;
        private Visibility _showGrid;
        public Visibility ShowGrid
        {
            get
            {
                return this._showGrid;
            }
            set
            {
                this._showGrid = value;
                OnPropertyRaised("ShowGrid");
            }
        }

        private Visibility _showError;
        public Visibility ShowError
        {
            get
            {
                return this._showError;
            }
            set
            {
                this._showError = value;
                OnPropertyRaised("ShowError");
            }
        }

        private Visibility _showDetails;
        public Visibility ShowDetails
        {
            get
            {
                return this._showDetails;
            }
            set
            {
                this._showDetails = value;
                OnPropertyRaised("ShowDetails");
            }
        }

        private string _errorText;
        public string ErrorText
        {
            get
            {
                return this._errorText;
            }
            set
            {
                this._errorText = value;
                OnPropertyRaised("ErrorText");
            }
        }
        private Bill _bill;
        public Bill Bill
        {
            get
            {
                return this._bill;
            }
            set
            {
                this._bill = value;
                OnPropertyRaised("Bill");
            }
        }
        public Job SelectedJob
        {
            get { return _selectedJob; }
            set
            {
                this._selectedJob = value;
                OnPropertyRaised("SelectedJob");
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


        public BillViewModel()
        {
            this.Bill = Bill.GetInstance();
            this.ShowGrid = Visibility.Hidden;
            this.ShowError = Visibility.Hidden;
            this.ShowDetails = Visibility.Hidden;
            //Bill.CustomerName = "Taori";
            ////Bill.FromDate = Convert.ToString(DateTime.Now.AddDays(-39));
            ////Bill.ToDate = Convert.ToString(DateTime.Now.AddDays(-38));
            //Bill.FromDate = "09/24/2020";
            //Bill.ToDate = "09/25/2020";
            //Bill.LstErrors = new List<string>();
        }

        private ICommand _clickCommand;
        public ICommand ClickCommand
        {
            get
            {
                return _clickCommand ?? (_clickCommand = new CommandHandler((obj) => MyAction(), () => CanExecute));
            }
        }

        private ICommand _printCommand;
        public ICommand PrintCommand
        {
            get
            {
                return _printCommand ?? (_printCommand = new CommandHandler((obj) => PrintBill(obj), () => CanExecute));
            }
        }


        private ICommand _updateTotalCommand;
        public ICommand UpdateTotalCommand
        {
            get
            {
                return _updateTotalCommand ?? (_updateTotalCommand = new CommandHandler((obj) => UpdateTotal(), () => CanExecute));
            }
        }

        private void UpdateTotal()
        {
            if (this.Bill.Jobs != null && this.Bill.Jobs.Count > 0)
            {
                this.Bill.TotalAmount = this.Bill.Jobs.Sum(x => x.Amount);
            }
        }

        private ICommand _clearCommand;
        public ICommand ClearCommand
        {
            get
            {
                return _clearCommand ?? (_clearCommand = new CommandHandler((obj) => ClearObject(), () => CanExecute));
            }
        }

        private void ClearObject()
        {
            this.Bill = Bill.DestroyInstance();
            this.Bill = Bill.GetInstance();
            this.ShowGrid = Visibility.Hidden;
            this.ShowError = Visibility.Hidden;
            this.ShowDetails = Visibility.Hidden;
        }

        private ICommand _leftMouseButtonDown;
        public ICommand LeftMouseButtonDown
        {
            get
            {
                return _leftMouseButtonDown ?? (_leftMouseButtonDown = new CommandHandler((obj) => RowSelected(obj), () => CanExecute));
            }
        }
        private void RowSelected(object obj)
        {
            this.ShowDetails = Visibility.Visible;
            this.SelectedJob = obj as Job;
        }

        public bool CanExecute
        {
            get
            {
                // check if executing is allowed, i.e., validate, check if a process is running, etc. 
                return true;
            }
        }

        public void MyAction()
        {
            try
            {
                DateTime strFromDate = Convert.ToDateTime(this.Bill.FromDate);
                DateTime strToDate = Convert.ToDateTime(this.Bill.ToDate);
                string strCustomerName = this.Bill.CustomerName;
                if (this.Bill.Jobs != null)
                {
                    this.Bill.Jobs = new ObservableCollection<Job>();
                    Bill.TotalAmount = 0;
                    Job.TotalAmount = 0;
                }
                GetBills(strFromDate, strToDate, strCustomerName);
                this.ShowGrid = this.Bill.Jobs?.Count > 0 ? Visibility.Visible : Visibility.Hidden;
                this.ShowError = this.Bill.Jobs?.Count > 0 ? Visibility.Hidden : Visibility.Visible;
                //this.Bill.ErrorObjects = this.Bill.Jobs?.FindAll(t => t.ErrorMessage != null);
                //this.Bill.CustomerName = "Indu";
            }
            catch (Exception ex)
            {
                this.ErrorText = "Error occured, Please contact support team.";
                this.ShowGrid = Visibility.Hidden;
                this.ShowError = Visibility.Visible;
            }

        }
        public void GetBills(DateTime fromDate, DateTime toDate, string customerName)
        {
            List<Job> jobs = null;
            string startMonthFolder = fromDate.ToString("MMM").ToLower() + " " + fromDate.Year;
            string endMonthFolder = toDate.ToString("MMM").ToLower() + " " + toDate.Year;

            var monthWiseFolderList = Enumerable.Range(0, 1 + ((toDate.Year - fromDate.Year) * 12) + toDate.Month - fromDate.Month)
                                   .Select(offset => fromDate.AddMonths(offset).ToString("MMM yyyy"))
                                   .ToArray();


            //ObservableCollection<Job> Jobs = new ObservableCollection<Job>();
            if (monthWiseFolderList != null)
            {
                int i = 1;
                foreach (var monthFolder in monthWiseFolderList)
                {
                    if (Directory.Exists(baseFilePath + "\\" + monthFolder))
                    {
                        
                        var dateWiseFolderList = Enumerable.Range(0, 1 + toDate.Subtract(fromDate).Days)
                                        .Select(offset => fromDate.AddDays(offset).ToString("d.M.yyyy"))
                                        .ToArray();
                        if (dateWiseFolderList != null)
                        {
                            foreach (var item in dateWiseFolderList)
                            {
                                if (Directory.Exists(baseFilePath + "\\" +monthFolder+"\\"+ item))
                                {
                                    string[] files = GetFiles(baseFilePath, monthFolder, item, customerName);

                                    if (files != null)
                                    {
                                        if (files.Length > 0)
                                        {
                                            try
                                            {
                                                jobs = CreateJobObject(files, customerName, item);
                                                foreach (var job in jobs)
                                                {
                                                    job.Id = i++;
                                                    this.Bill.Jobs.Add(job);
                                                    Bill.TotalAmount += job.Amount;
                                                    Bill.TotalDimensions += job.Dimension;
                                                    Bill.TotalQuantity += job.Quantity;
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                MessageBox.Show(ex.Message);
                                            }

                                        }
                                    }
                                    else
                                    {
                                        //MessageBox.Show("Files not found");
                                    }
                                }

                            }
                        }
                    }
                    else
                    {
                        // this.Bill.Audit.Status.Add("Folder " + monthFolder + " not found");
                        MessageBox.Show("Folder " + monthFolder + " not found."); 
                    }
                }

                //this.Bill.Jobs.Add(new Job
                //{
                //     Amount = Bill.TotalAmount,
                //    //Amount = Job.TotalAmount,
                //    //Amount = this.Bill.Jobs.Sum(x => x.Amount),
                //    Id = null
                //}) ;
            }
            else
            {
                this.ErrorText = "No Folders found. Please update date and try again.";
                this.ShowGrid = Visibility.Hidden;
                this.ShowError = Visibility.Visible;
            }

           // return Jobs;
        }

        public void SetTotalAmount(List<Job> jobs)
        { 
            
        }

        private string[] GetFiles(string baseFilePath, string monthFolder, string item, string customerName)
        {

            string[] files = null;
            if (Directory.Exists(baseFilePath + "\\" + monthFolder + "\\" + item + "\\" + customerName + "\\done"))
            {
                files = Directory.GetFiles(baseFilePath + "\\" + monthFolder + "\\" + item + "\\" + customerName + "\\done");
            }
            else
            {
                if (Directory.Exists(baseFilePath + "\\" + monthFolder + "\\" + item + "\\" + customerName))
                {
                    files = Directory.GetFiles(baseFilePath + "\\" + monthFolder + "\\" + item + "\\" + customerName);
                }
                else
                {
                    files = null;
                }
            }
            return files;
        }

        public List<Job> CreateJobObject(string[] files, string customerName, string jobDate)
        {
            List<Job> lstjob = new List<Job>();
            Job job = null;
            foreach (var item in files)
            {
                try
                {
                    job = new Job();
                    job.CustomerName = customerName;
                    job.JobDate = jobDate;
                    job.FilePath = item;
                    job.JobName = item.Substring(item.LastIndexOf('\\') + 1);

                    if (!String.IsNullOrEmpty(job.JobName))
                    {
                        double[] dimensions = GetJobDemensions(job.JobName);
                        if (dimensions != null)
                        {
                            job.Width = dimensions[0];
                            job.Height = dimensions[1];
                            job.Quantity = Convert.ToInt32(dimensions[3]);
                            job.Dimension = (dimensions[2] * dimensions[3]);
                        }
                        job.Quality = GetPrintQuality(job.JobName);
                        job.Rate = job.Quality.Rate;
                        job.Amount = job.Quality.Rate * job.Dimension;
                        //if (job.Quality != null)
                        //{
                        //    //job.Amount = job.Quality.Rate * job.Dimension;
                        //    //Job.TotalAmount += job.Quality.Rate * job.Dimension;
                        //}
                    }
                    else
                    {
                        job.Width = 0;
                        job.Height = 0;
                        job.Dimension = 0;
                        job.Quantity = 0;
                        job.Quality = null;
                    }
                    
                    lstjob.Add(job);

                }
                catch (Exception ex)
                {
                    if (job != null)
                    {
                        //job.ErrorMessage = item;
                        //this.Bill.Audit.Status.Add(item.ToString());
                        this.Bill.LstErrors.Add(item.ToString());
                    }
                    lstjob.Add(job);
                }
            }
            return lstjob;

        }



        public double[] GetJobDemensions(string filename)
        {
            double[] dimension = new double[4];
            try
            {
                //if (!String.IsNullOrEmpty(filename))
                //{
                //    //width
                //    dimension[0] = Convert.ToDouble((filename.Split(' ')[1]).Split('x')[0]);
                //    //height
                //    dimension[1] = Convert.ToDouble(filename.Split(' ')[1].Split('x')[1]);
                //    //dimension
                //    if (dimension[0] != 0 && dimension[1] != 0)
                //    { dimension[2] = dimension[0] * dimension[1]; }
                //    //quantity
                //    if (filename.Split(' ').Length > 3)
                //        dimension[3] = Convert.ToInt32(filename.Split(' ')[2]);
                //    else
                //        dimension[3] = Convert.ToInt32(filename.Split(' ')[2].Split('.')[0]);
                //}
                if (!String.IsNullOrEmpty(filename))
                {
                    var test = Regex.Match(filename, @"\s*\d+([.]?[\d])*(\s)*(x|X)\s*\d+([.]?[\d])*\s*").ToString();
                    if (test.Contains("x") || test.Contains("X"))
                    {
                        dimension[0] = test.Contains("x") ? Convert.ToDouble(test.Split('x')[0].Trim()) : Convert.ToDouble(test.Split('X')[0].Trim());
                        dimension[1] = test.Contains("x") ? Convert.ToDouble(test.Split('x')[1].Trim()) : Convert.ToDouble(test.Split('X')[1].Trim());
                    }
                    string newInput = filename.Replace(test, " ");
                    var test2 = Regex.Match(Regex.Match(newInput, @"\s\W*\d\s*\w*\W*(.)*\s*$").Value, @"\d+").ToString();
                    dimension[3] = !String.IsNullOrEmpty(test2.ToString()) ?  Convert.ToDouble(test2) : 1;
                    
                    if (dimension[0] != 0 && dimension[1] != 0)
                    { dimension[2] = dimension[0] * dimension[1]; }

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dimension;
        }

        public PrintQualities GetPrintQuality(string filename)
        {
            string quality = filename.Split(' ')[0];
            string[] splitFilepath = filename.Split(' ');
            int i = 0;
            string temp = "";
            foreach (var item in splitFilepath)
            {
                if (i != 0)
                    temp += item;
                i++;
            }

            if (temp.ToLower().Contains("vinil"))
            {
                return new PrintQualities("vinil");
            }
            else
            {
                if (temp.ToLower().Contains("backlite") || temp.ToLower() == "baklite" || temp.ToLower() == "backlit" || temp.ToLower() == "baklit")
                {
                    return new PrintQualities("backlite");
                }
                else
                {
                    if (temp.ToLower().Contains("star"))
                    {
                        return new PrintQualities("star");
                    }
                    else
                    {
                        return new PrintQualities("local");
                    }
                }
            }
        }


        private bool PrintBill(object parameter)
        {
            return true;
        }

        private FlowDocument CreateFlowDocument(object parameter)
        {
            // Create a FlowDocument  
            FlowDocument flowDoc = new FlowDocument();
            // Set PageHeight and PageWidth to "Auto".
            flowDoc.PageHeight = Double.NaN;
            flowDoc.PageWidth = Double.NaN;
            // Specify minimum page sizes.
            flowDoc.MinPageWidth = 680.0;
            flowDoc.MinPageHeight = 480.0;
            //Specify maximum page sizes.
            flowDoc.MaxPageWidth = 1024.0;
            
            flowDoc.MaxPageHeight = 768.0;
            Bill bill = parameter as Bill;
            if (bill != null)
            {
                Table jobsTable = BillViewModel.ToDataTable<Job>(bill);
                
                System.Windows.Controls.Image image = new System.Windows.Controls.Image();
                BitmapImage bimg = new BitmapImage();
                bimg.BeginInit();
                bimg.UriSource = new Uri(@"E:\Study\AshokaGraphics\AshokaGraphics_v1.0\WpfApp1\images\ashoka1.jpg", UriKind.Absolute);
                bimg.EndInit();
                image.Source = bimg;
                //flowDoc.Blocks.Add(new BlockUIContainer(image));
                Section sec = new Section();
                Paragraph p1 = new Paragraph();
                p1.Inlines.Add("Name : " + bill.CustomerName+"\n\n");
                flowDoc.Blocks.Add(p1);
                flowDoc.Blocks.Add(jobsTable);
            }
            return flowDoc;
        }

        public static Table ToDataTable<Job>(Bill bill)
        {
            DataTable dataTable = new DataTable(typeof(Job).Name);
            //dataTable.Columns.Add("Sr.No.");
            //dataTable.Columns.Add("Date");
            //dataTable.Columns.Add("Purticular");
            //dataTable.Columns.Add("Qty");
            //dataTable.Columns.Add("Sq.Ft.");
            //dataTable.Columns.Add("Rate");
            //dataTable.Columns.Add("Amount");
            //int i = 1;
            //foreach (var item in bill.Jobs)
            //{
            //    DataRow row = dataTable.NewRow();
            //    row["Sr.No."] = i++;
            //    row["Date"] = item.JobDate;
            //    row["Purticular"] = item.Width+"x"+item.Height;
            //    row["Qty"] = item.Quantity;
            //    row["Sq.Ft."] = item.Dimension;
            //    row["Rate"] = item.Rate;
            //    row["Amount"] = item.Amount;
            //    dataTable.Rows.Add(row);
            //}
            ////put a breakpoint here and check datatable
            //return dataTable;
            
            Table table1 = new Table();
            int numberOfColumns = 6;
            for (int x = 0; x < numberOfColumns; x++)
            {
                table1.Columns.Add(new TableColumn());
            }
            // Create and add an empty TableRowGroup to hold the table's Rows.
            table1.RowGroups.Add(new TableRowGroup());

            // Add the first (title) row.
            table1.RowGroups[0].Rows.Add(new TableRow());

            // Alias the current working row for easy reference.
            TableRow currentRow = table1.RowGroups[0].Rows[0];
            
            // Global formatting for the title row.
            //currentRow.Background = Brushes.Silver;
            //currentRow.FontSize = 40;
            
            // Add the header row with content,
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Sr.Id."))));
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Date"))));
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Purticular"))));
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Qty"))));
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Sq.Ft."))));
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Rate"))));
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Amount"))));
            int i = 1;
            foreach (var item in bill.Jobs)
            {
                table1.RowGroups[0].Rows.Add(new TableRow());
                TableRow row = table1.RowGroups[0].Rows[i-1];
                
                row.Cells.Add(new TableCell(new Paragraph(new Run(i++.ToString()))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.JobDate))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.Width+"x"+item.Height))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.Quantity.ToString()))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.Dimension.ToString()))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.Rate.ToString()))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.Amount.ToString()))));
            }

            // and set the row to span all 6 columns.
            //currentRow.Cells[0].ColumnSpan = 6;

            // Add the second (header) row.
           // table1.RowGroups[0].Rows.Add(new TableRow());
           // currentRow = table1.RowGroups[0].Rows[1];

           // // Global formatting for the header row.
           // currentRow.FontSize = 18;
           // currentRow.FontWeight = FontWeights.Bold;

           // // Add the third row.
           // table1.RowGroups[0].Rows.Add(new TableRow());
           // currentRow = table1.RowGroups[0].Rows[2];

           // // Global formatting for the row.
           // currentRow.FontSize = 12;
           // currentRow.FontWeight = FontWeights.Normal;

           // // Add cells with content to the third row.
           // currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Widgets"))));
           // currentRow.Cells.Add(new TableCell(new Paragraph(new Run("$50,000"))));
           // currentRow.Cells.Add(new TableCell(new Paragraph(new Run("$55,000"))));
           // currentRow.Cells.Add(new TableCell(new Paragraph(new Run("$60,000"))));
           // currentRow.Cells.Add(new TableCell(new Paragraph(new Run("$65,000"))));
           // currentRow.Cells.Add(new TableCell(new Paragraph(new Run("$230,000"))));

           // // Bold the first cell.
           // currentRow.Cells[0].FontWeight = FontWeights.Bold;

           // table1.RowGroups[0].Rows.Add(new TableRow());
           // currentRow = table1.RowGroups[0].Rows[3];

           // // Global formatting for the footer row.
           //// currentRow.Background = Brushes.LightGray;
           // currentRow.FontSize = 18;
           // currentRow.FontWeight = System.Windows.FontWeights.Normal;

           // // Add the header row with content,
           // currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Projected 2004 Revenue: $810,000"))));
           // // and set the row to span all 6 columns.
           // currentRow.Cells[0].ColumnSpan = 6;
            return table1;
        }

    }
}
