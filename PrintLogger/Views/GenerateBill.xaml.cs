
using Microsoft.Office.Interop.Word;
using PrintLogger.ViewModels;
using System;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;


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
                Microsoft.Office.Interop.Word.Application winword = new Microsoft.Office.Interop.Word.Application();
                winword.Visible = false;
                object missing = System.Reflection.Missing.Value;
                Microsoft.Office.Interop.Word.Document document = winword.Documents.Add();
                //document.Paragraphs.SpaceAfter = 0;
                foreach (Microsoft.Office.Interop.Word.Section wordSection in document.Sections)
                {
                    Microsoft.Office.Interop.Word.Range footerRange = wordSection.Footers[Microsoft.Office.Interop.Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;
                    footerRange.Font.ColorIndex = Microsoft.Office.Interop.Word.WdColorIndex.wdDarkRed;
                    footerRange.Font.Size = 10;
                    footerRange.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    footerRange.Text = "Ashoka Graphics";
                }

                Range docRange = document.Range();

                //string imagePath = @"E:\Study\AshokaGraphics\AshokaGraphics_v1.1\PrintLogger\PrintLogger\Images\ashoka1.jpg";
                string imagePath = ConfigurationManager.AppSettings.Get("billImageLogoPath");

                InlineShape autoScaledInlineShape = docRange.InlineShapes.AddPicture(imagePath);
                float scaledWidth = autoScaledInlineShape.Width;
                float scaledHeight = autoScaledInlineShape.Height;
                autoScaledInlineShape.Delete();

                Microsoft.Office.Interop.Word.Shape newShape = document.Shapes.AddShape(1, 0, 0, scaledWidth, scaledHeight);
                newShape.Fill.UserPicture(imagePath);
                InlineShape finalInlineShape = newShape.ConvertToInlineShape();
                finalInlineShape.Line.Visible = Microsoft.Office.Core.MsoTriState.msoFalse;
                finalInlineShape.Range.Cut();
                docRange.Paste();
                Microsoft.Office.Interop.Word.Paragraph para1 = document.Content.Paragraphs.Add();
                Microsoft.Office.Interop.Word.Paragraph para2 = document.Content.Paragraphs.Add(ref missing);
                para2.Range.Text = "Name: " + bill.CustomerName + "                                                                                               Date: " + bill.FromDate.Split(' ')[0].ToString() + " - " + bill.ToDate.Split(' ')[0].ToString(); 
                // para2.Range.InsertParagraphAfter();
                //para2.Range.Text = "Date: " + bill.FromDate.Split(' ')[0].ToString() + " - " + bill.ToDate.Split(' ')[0].ToString();
                //para2.SpaceAfter = 0.0f;
                para2.Range.InsertParagraphAfter();

                Microsoft.Office.Interop.Word.Table firstTable = document.Tables.Add(para2.Range, bill.Jobs.Count + 2, 8, WdDefaultTableBehavior.wdWord8TableBehavior, ref missing);

                firstTable.Borders.Enable = 1;
                firstTable.AllowAutoFit = true;

                foreach (Row row in firstTable.Rows)
                {
                    foreach (Cell cell in row.Cells)
                    {

                        //Header row  
                        if (cell.RowIndex == 1)
                        {
                            switch (cell.ColumnIndex)
                            {
                                case 1:
                                    cell.Range.Text = "Sr.Id";
                                    break;
                                case 2: cell.Range.Text = "Date"; break;
                                case 3: cell.Range.Text = "File"; break;
                                case 4: cell.Range.Text = "Sq.Ft"; break;
                                case 5: cell.Range.Text = "Quality"; break;
                                case 6: cell.Range.Text = "Rate"; break;
                                case 7: cell.Range.Text = "Qty"; break;
                                case 8: cell.Range.Text = "Amt"; break;

                                default:
                                    break;
                            }

                            cell.Range.Font.Bold = 1;
                            //cell.Width = 5;
                            //other format properties goes here  
                            cell.Range.Font.Name = "verdana";
                            cell.Range.Font.Size = 8;
                            //cell.Range.Font.ColorIndex = WdColorIndex.wdGray25;                              
                            cell.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
                            //Center alignment for the Header cells  
                            cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                            cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                            cell.Range.ParagraphFormat.SpaceAfter = 0.0f;
                            // cell.SetWidth(4, WdRulerStyle.wdAdjustProportional);
                            cell.BottomPadding = 0;
                        }
                        //Data row  
                        else
                        {
                            if (cell.RowIndex == bill.Jobs.Count + 2)
                            {
                                if (cell.ColumnIndex == 4)
                                {
                                    cell.Range.Text = bill.TotalDimensions.ToString();
                                }
                                if (cell.ColumnIndex == 7)
                                {
                                    cell.Range.Text = bill.TotalQuantity.ToString();
                                }
                                if (cell.ColumnIndex == 8)
                                {
                                    cell.Range.Text = bill.TotalAmount.ToString();
                                }
                                cell.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
                                cell.Range.Paragraphs.SpaceAfter = 0.0f;
                            }
                            else
                            {
                                switch (cell.ColumnIndex)
                                {
                                    case 1:
                                        cell.Range.Text = bill.Jobs[cell.RowIndex - 2].Id.ToString();
                                        break;
                                    case 2: cell.Range.Text = bill.Jobs[cell.RowIndex - 2].JobDate.ToString(); break;
                                    case 3: cell.Range.Text = bill.Jobs[cell.RowIndex - 2].JobName.ToString(); break;
                                    case 4: cell.Range.Text = bill.Jobs[cell.RowIndex - 2].Dimension.ToString(); break;
                                    case 5: cell.Range.Text = bill.Jobs[cell.RowIndex - 2].Quality.Quality.ToString(); break;
                                    case 6: cell.Range.Text = bill.Jobs[cell.RowIndex - 2].Rate.ToString(); break;
                                    case 7: cell.Range.Text = bill.Jobs[cell.RowIndex - 2].Quantity.ToString(); break;
                                    case 8: cell.Range.Text = bill.Jobs[cell.RowIndex - 2].Amount.ToString(); break;

                                    default:
                                        break;

                                }
                                cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                                cell.Range.Columns.AutoFit();
                                cell.BottomPadding = 0;
                                cell.Range.Rows.Alignment = WdRowAlignment.wdAlignRowCenter;
                                cell.Range.Paragraphs.SpaceAfter = 0.0f;
                                //cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;

                                //cell.SetWidth(4, WdRulerStyle.wdAdjustProportional);
                            }
                        }
                    }
                }

                //Save the document  
                object filename = ConfigurationManager.AppSettings.Get("billSavePath") + "\\" + bill.CustomerName + " " + Guid.NewGuid().ToString() + ".docx";

                document.SaveAs2(ref filename);

                document.Close(ref missing, ref missing, ref missing);
                document = null;
                winword.Quit(ref missing, ref missing, ref missing);
                winword = null;
                MessageBox.Show("Bill Created successfully !");
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
