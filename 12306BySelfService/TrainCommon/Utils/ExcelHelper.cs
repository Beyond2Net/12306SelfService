using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainCommon
{
    public class ExcelHelper
    {
        public static string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0;HDR=YES;""";

        /// <summary>
        /// 读取 Excel 返回 DataSet
        /// </summary>
        /// <param name="connectionString">Excel 连接字符串</param>
        /// <param name="commandString">查询语句, for example:"SELECT ID,userName,userAddress FROM [Sheet1$]" </param>
        /// <returns></returns>
        public static DataSet GetExcelDataSet(string connectionString, string commandString)
        {
            DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.OleDb");
            DbDataAdapter adapter = factory.CreateDataAdapter();
            DbCommand selectCommand = factory.CreateCommand();

            selectCommand.CommandText = commandString;  //commandString例如:"SELECT ID,userName,userAddress FROM [Sheet1$]"
            DbConnection connection = factory.CreateConnection();
            connection.ConnectionString = connectionString;
            selectCommand.Connection = connection;
            adapter.SelectCommand = selectCommand;
            DataSet cities = new DataSet();
            adapter.Fill(cities);
            connection.Close();

            return cities;
        }

        //public static void SaveDataToExcel(System.Data.DataTable table, string savExcelFilePath)
        //{
        //    Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();

        //    Microsoft.Office.Interop.Excel.Workbook book = excel.Workbooks.Add(Missing.Value);
        //    Microsoft.Office.Interop.Excel.Worksheet sheet = (Microsoft.Office.Interop.Excel.Worksheet)book.ActiveSheet;

        //    for (int col = 0; col < table.Columns.Count; col++)
        //    {
        //        sheet.Cells[1, col + 1] = table.Columns[col].ColumnName;
        //    }

        //    Microsoft.Office.Interop.Excel.Range range = excel.get_Range(excel.Cells[1, 1], excel.Cells[1, table.Columns.Count]);
        //    range.Interior.ColorIndex = 43;

        //    for (int row = 0; row < table.Rows.Count; row++)
        //    {
        //        for (int col = 0; col < table.Columns.Count; col++)
        //        {
        //            sheet.Cells[row + 2, col + 1] = table.Rows[row][col].ToString();
        //        }
        //    }

        //    // 准备把工作溥保存到哪个位置 (保存为一个.xls文件)
        //    book.Close(true, savExcelFilePath, Missing.Value);
        //    excel.Quit();
        //    GC.Collect();
        //}

        public static void SaveDataToExcel(System.Data.DataTable table, string tableName, string savExcelFilePath)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet = workbook.Worksheets[0];
            Cells cells = sheet.Cells;

            //为标题设置样式     
            Style styleTitle = workbook.Styles[workbook.Styles.Add()];//新增样式 
            styleTitle.HorizontalAlignment = TextAlignmentType.Center;//文字居中 
            styleTitle.Font.Name = "宋体";//文字字体 
            styleTitle.Font.Size = 12;//文字大小 
            styleTitle.Font.IsBold = true;//粗体

            //样式2 
            Style style2 = workbook.Styles[workbook.Styles.Add()];//新增样式 
            style2.HorizontalAlignment = TextAlignmentType.Center;//文字居中 
            style2.VerticalAlignment = TextAlignmentType.Center;
            style2.Font.Name = "宋体";//文字字体 
            style2.Font.Size = 10;//文字大小 
            style2.Font.IsBold = true;//粗体 
            style2.IsTextWrapped = true;//单元格内容自动换行 
            style2.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            style2.ForegroundColor = System.Drawing.Color.FromArgb(147, 209, 80);
            style2.Pattern = BackgroundType.Solid;

            //样式3 
            Style style3 = workbook.Styles[workbook.Styles.Add()];//新增样式 
            style3.HorizontalAlignment = TextAlignmentType.Center;//文字居中 
            style3.Font.Name = "宋体";//文字字体 
            style3.Font.Size = 10;//文字大小 
            style3.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            style3.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style3.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style3.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

            int Colnum = table.Columns.Count;//表格列数 
            int Rownum = table.Rows.Count;//表格行数 

            //生成行1 标题行    
            cells.Merge(0, 0, 1, Colnum);//合并单元格 
            cells[0, 0].PutValue(tableName);//填写内容 
            cells[0, 0].SetStyle(styleTitle);
            //cells.SetRowHeight(0, 38);

            //生成行2 列名行 
            for (int i = 0; i < Colnum; i++)
            {
                cells[1, i].PutValue(table.Columns[i].ColumnName);
                cells[1, i].SetStyle(style2);
                cells.SetRowHeight(1, 25);
            }

            //生成数据行 
            for (int i = 0; i < Rownum; i++)
            {
                for (int k = 0; k < Colnum; k++)
                {
                    cells[2 + i, k].PutValue(table.Rows[i][k].ToString());
                    cells[2 + i, k].SetStyle(style3);
                }
                //cells.SetRowHeight(2 + i, 24);
            }

            workbook.Save(savExcelFilePath);
        }

        /// <summary>
        /// 无须电脑安装Excel，读取Excel文件
        /// </summary>
        /// <param name="excelPath"></param>
        /// <returns></returns>
        public static System.Data.DataTable GetExcelDataTableByAspose(string excelPath)
        {
            Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(excelPath);
            Aspose.Cells.Cells cells = workbook.Worksheets[0].Cells;
            System.Data.DataTable dt = cells.ExportDataTableAsString(0, 0, cells.MaxRow + 1, cells.MaxColumn + 1, true);
            return dt;
        }

        public static void AutoFitExcel(string sourceExcelPath, string destinateExcelPath)
        {
            FileStream fstream = new FileStream(sourceExcelPath, FileMode.Open);
            Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(fstream);
            Aspose.Cells.Worksheet worksheet = workbook.Worksheets[0];
            worksheet.AutoFitRows();
            worksheet.AutoFitColumns();
            workbook.Save(destinateExcelPath);
            fstream.Close();
        }

        public static void EncryptExcelFile(string sourceExcelPath, string destinateExcelPath, string password)
        {
            Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(sourceExcelPath);
            workbook.SetEncryptionOptions(Aspose.Cells.EncryptionType.XOR, 40);
            workbook.SetEncryptionOptions(Aspose.Cells.EncryptionType.StrongCryptographicProvider, 128);
            workbook.Settings.Password = password;
            workbook.Save(destinateExcelPath);
        }

    }
}
