using PrintLogger.ViewModels;
using System;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Text;
using System.Drawing;
using Image = System.Drawing.Image;


namespace PrintLogger.Views
{
    /// <summary>
    /// Interaction logic for GenerateBill.xaml
    /// </summary>
    public partial class GenerateBill : System.Windows.Controls.Page
    {

        BillViewModel objBillViewModel = null;
        public bool PrintEnabled { get; set; }
        public GenerateBill()
        {
            InitializeComponent();
            this.PrintEnabled = false;
            this.objBillViewModel = new BillViewModel();
            this.DataContext = this.objBillViewModel;
            if (this.objBillViewModel.Bill.Jobs.Count > 0)
            {
                this.PrintEnabled = true;
            }
        }



        private void CreateDocument(PrintLogger.Models.Bill bill)
        {
            try
            {
                this.PrintEnabled = false;

                string imagePath = ConfigurationManager.AppSettings.Get("billImageLogoPath");
                string saveFolder = ConfigurationManager.AppSettings.Get("billSavePath");
                if (string.IsNullOrWhiteSpace(saveFolder))
                {
                    MessageBox.Show("Save path for bills is not configured.");
                    return;
                }

                Directory.CreateDirectory(saveFolder);
                string fileName = Path.Combine(saveFolder, bill.CustomerName + " " + Guid.NewGuid().ToString() + ".doc");

                var sb = new StringBuilder();
                sb.AppendLine("<!DOCTYPE html>");
                sb.AppendLine("<html>");
                sb.AppendLine("<head>");
                sb.AppendLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />");
                sb.AppendLine("<style>body{font-family:Verdana, Arial, Helvetica, sans-serif; font-size:10pt;} table{border-collapse:collapse;} td, th{border:1px solid #000;padding:4px;} .header{background:#ddd;font-weight:bold;text-align:center;} .total{background:#ddd;font-weight:bold;} img.logo{display:block;margin:0 auto;}</style>");
                sb.AppendLine("</head>");
                sb.AppendLine("<body>");

                // Image (if exists) - use relative/absolute path so Word can load it
                if (!string.IsNullOrWhiteSpace(imagePath) && File.Exists(imagePath))
                {
                    // We will ask Word to render the image centered and scaled to a percentage of the page
                    int maxWidthPx = 200; // maximum pixel width allowed (reduced)
                    int desiredPercent = 40; // percent of page width to request (reduced from 60)

                    try
                    {
                        using (var img = Image.FromFile(imagePath))
                        {
                            int origW = img.Width;
                            // Never request a max-width larger than the original image to avoid upscaling
                            if (origW < maxWidthPx)
                                maxWidthPx = origW;
                        }
                    }
                    catch
                    {
                        // ignore, keep defaults
                    }

                    // Use file URI so Word can resolve it. Use CSS to center and fit the image.
                    string imgUri = System.Uri.EscapeUriString(imagePath.Replace('\\', '/'));
                    sb.AppendFormat("<div><img class=\"logo\" src=\"file:///{0}\" alt=\"Logo\" style=\"width:{1}%;max-width:{2}px;height:auto;\"/></div>", imgUri, desiredPercent, maxWidthPx);
                    sb.AppendLine();
                }

                string fromDate = bill.FromDate?.Split(' ')[0] ?? string.Empty;
                string toDate = bill.ToDate?.Split(' ')[0] ?? string.Empty;
                sb.AppendFormat("<p><strong>Name:</strong> {0} &nbsp;&nbsp; <strong>Date:</strong> {1} - {2}</p>", System.Net.WebUtility.HtmlEncode(bill.CustomerName), System.Net.WebUtility.HtmlEncode(fromDate), System.Net.WebUtility.HtmlEncode(toDate));

                // Table header
                sb.AppendLine("<table>");
                sb.AppendLine("<tr>");
                sb.AppendLine("<th class=\"header\">Sr.Id</th>");
                sb.AppendLine("<th class=\"header\">Date</th>");
                sb.AppendLine("<th class=\"header\">File</th>");
                sb.AppendLine("<th class=\"header\">Sq.Ft</th>");
                sb.AppendLine("<th class=\"header\">Quality</th>");
                sb.AppendLine("<th class=\"header\">Rate</th>");
                sb.AppendLine("<th class=\"header\">Qty</th>");
                sb.AppendLine("<th class=\"header\">Amt</th>");
                sb.AppendLine("</tr>");

                // Data rows
                if (bill.Jobs != null)
                {
                    for (int i = 0; i < bill.Jobs.Count; i++)
                    {
                        var job = bill.Jobs[i];
                        sb.AppendLine("<tr>");
                        sb.AppendFormat("<td>{0}</td>", System.Net.WebUtility.HtmlEncode(job.Id?.ToString() ?? string.Empty));
                        sb.AppendFormat("<td>{0}</td>", System.Net.WebUtility.HtmlEncode(job.JobDate ?? string.Empty));
                        sb.AppendFormat("<td>{0}</td>", System.Net.WebUtility.HtmlEncode(job.JobName ?? string.Empty));
                        sb.AppendFormat("<td>{0}</td>", job.Dimension.ToString());
                        sb.AppendFormat("<td>{0}</td>", System.Net.WebUtility.HtmlEncode(job.Quality?.Quality ?? string.Empty));
                        sb.AppendFormat("<td>{0}</td>", job.Rate.ToString());
                        sb.AppendFormat("<td>{0}</td>", job.Quantity.ToString());
                        sb.AppendFormat("<td>{0}</td>", job.Amount.ToString());
                        sb.AppendLine("</tr>");
                    }
                }

                // Total row
                sb.AppendLine("<tr>");
                sb.AppendLine("<td colspan=\"3\" class=\"total\"></td>");
                sb.AppendFormat("<td class=\"total\">{0}</td>", bill.TotalDimensions.ToString());
                sb.AppendLine("<td class=\"total\"></td>");
                sb.AppendLine("<td class=\"total\"></td>");
                sb.AppendFormat("<td class=\"total\">{0}</td>", bill.TotalQuantity.ToString());
                sb.AppendFormat("<td class=\"total\">{0}</td>", bill.TotalAmount.ToString());
                sb.AppendLine("</tr>");

                sb.AppendLine("</table>");

                // Footer text
                sb.AppendLine("<div style=\"text-align:center;margin-top:20px;color:#800;\">Ashoka Graphics</div>");

                sb.AppendLine("</body>");
                sb.AppendLine("</html>");

                File.WriteAllText(fileName, sb.ToString(), Encoding.UTF8);

                MessageBox.Show("Bill Created successfully !\nSaved to: " + fileName);
                this.PrintEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                this.PrintEnabled = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var myValue = ((Button)sender).Tag;
            if (myValue is PrintLogger.Models.Bill)
            {
                CreateDocument(myValue as PrintLogger.Models.Bill);
            }
        }
    }
}
