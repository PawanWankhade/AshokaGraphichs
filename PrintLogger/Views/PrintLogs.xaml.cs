using PrintLogger.Models;
using PrintLogger.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PrintLogger.Views
{
    /// <summary>
    /// Interaction logic for PrintLogs.xaml
    /// </summary>
    public partial class PrintLogs : Page
    {
        List<Job> jobs = null;
        public PrintLogs()
        {
            InitializeComponent();
            this.BindGrid();
        }

        public void BindGrid()
        {
            try
            {
                jobs = new ViewModels.JobsViewModel().GetJobs();
                this.DataGrid1.ItemsSource = jobs;
            }
            catch (Exception ex)
            {
               // WriteLogs(ex.Message);
            }

        }
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (jobs != null)
                {
                    this.DataGrid1.ItemsSource = this.jobs.FindAll(x => x.JobDate.ToString().Contains(searchTxtBox.Text) || x.FilePath.ToLower().Contains(searchTxtBox.Text.ToLower()) || x.PercentComplete.Contains(searchTxtBox.Text));
                }
            }
            catch (Exception ex)
            {
                //WriteLogs(ex.Message);
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                searchTxtBox.Text = "";
                if (jobs != null)
                {
                    DataGrid1.ItemsSource = jobs;
                }
            }
            catch (Exception ex)
            {
               // WriteLogs(ex.Message);
            }

        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            this.BindGrid();
        }
    }
}
