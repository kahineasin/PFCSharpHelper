//using Aspose.Cells;
using System.Drawing;
using System.IO;
using System.Web;
using System;
using Aspose.Words;
using Aspose.Words.Tables;
using Aspose.Cells.Tables;
using System.Collections.Generic;

namespace Perfect
{
    public class WordExport : IExport
    {
        public string suffix { get { return "doc"; } }
        private Document document;
        private DocumentBuilder builder;
        private Table table;
        private Style dataStyle;
        private int rowCount = 0;
        private int colCount = 0;
        private int curRow = 0;
        private int curCol = 0;
        private PrintPageScheme _printPageScheme;//打印页面样式方案--wxj20181011

        public void Init(object data, PrintPageScheme scheme)
        {
            //Aspose.Cells.License l = new Aspose.Cells.License();
            //l.SetLicense(Path.Combine(HttpRuntime.AppDomainAppPath, "lib/Aid/License.lic"));
            document = new Document();
            builder = new Aspose.Words.DocumentBuilder(document);
            table = builder.StartTable();
            if (scheme != null) { SetPrintPageScheme(scheme); }
        }
        #region Excel专用方法(非通用)
        //public Worksheet GetSheet()
        //{
        //    return sheet;
        //}
        //public void SetSheetTabName(string name)
        //{
        //    sheet.Name = name;
        //}
        #endregion

        public void SetColumnWidth(int x, string px)
        {
            //double width = PFDataHelper.WebWidthToExcel(px).Value;
            //sheet.Cells.SetColumnWidth(x, width);
            ////sheet.Cells.SetColumnWidth(x, width);
        }
        public void SetRowHeight(int y, string px)
        {
        }
        public void SetTextWrapped(int x1, int y1, int x2, int y2)
        {
        }
        public void MergeCell(int x1, int y1, int x2, int y2)
        {
            ////Range range = sheet.Cells.CreateRange(x1, y1, x2, y2);
            ////Range range = sheet.Cells.CreateRange(x1, y1, x2-x1+1, y2-y1+1);
            //Range range = sheet.Cells.CreateRange(y1, x1, y2 - y1 + 1, x2 - x1 + 1);
            //range.Merge();

        }

        private void FixXY(int x, int y)
        {
            while (y > curRow)
            {
                builder.EndRow();
                curRow++;
            }
            while (x > curCol)
            {
                builder.InsertCell();
                curCol++;
            }
        }
        private void BuildCell(string text)
        {
            //table.Rows.Count
            builder.CellFormat.Borders.LineStyle = LineStyle.Single;
            builder.CellFormat.Borders.Color = System.Drawing.Color.Black;
            //builder.CellFormat.Width = widthList[j];
            builder.CellFormat.VerticalMerge = Aspose.Words.Tables.CellMerge.None;
            builder.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;//垂直居中对齐
            builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;//水平居中对齐
            builder.Write(text);
        }
        public virtual void FillData(int x, int y, string field, object value)//x列y行
        {
            FixXY(x, y);

            switch ((value ?? string.Empty).GetType().Name.ToLower())
            {
                case "int32":
                case "int64":
                case "decimal":
                    BuildCell(PFDataHelper.ObjectToType<double>(value, 0).ToString());
                    break;
                default:
                    BuildCell(PFDataHelper.ObjectToString(value) ?? "");
                    break;
            }

            //if (y > curRow) {

            //}
            //if (table.Rows.Count < y+1) {
            //    for (int i = table.Rows.Count - 1; i < y; i++) {
            //        if (i < 0) { continue; }
            //        builder.MoveToCell(0,i,table.Rows[i].Count-1,0);
            //        builder.EndRow();
            //    }
            //}
            //sheet.MoveToCell(y,x);

            ////if (!field.StartsWith("title_"))
            ////    cell.SetStyle(GetDataStyle());

            //switch ((value ?? string.Empty).GetType().Name.ToLower())
            //{
            //    case "int32":
            //    case "int64":
            //    case "decimal":
            //        sheet.Cells[y, x].PutValue(PFDataHelper.ObjectToType<double>(value, 0));
            //        break;
            //    default:
            //        sheet.Cells[y, x].PutValue(PFDataHelper.ObjectToString(value));
            //        break;
            //}
        }

        public virtual void SetHeadStyle(int x1, int y1, int x2, int y2)
        {
            //var style = GetHeadStyle();
            //for (var y = y1; y <= y2; y++)
            //{
            //    for (var x = x1; x <= x2; x++)
            //    {
            //        var cell = sheet.Cells[y, x];
            //        cell.SetStyle(style);
            //    }
            //    if (_printPageScheme != null) { sheet.Cells.SetRowHeight(y, _printPageScheme.HeadRowHeight); }
            //}
        }

        public virtual void SetRowsStyle(int x1, int y1, int x2, int y2)
        {
            //var style = GetDataStyle();
            //for (var y = y1; y <= y2; y++)
            //{
            //    for (var x = x1; x <= x2; x++)
            //    {
            //        var cell = sheet.Cells[y, x];
            //        cell.SetStyle(style);
            //    }
            //    if (_printPageScheme != null) { sheet.Cells.SetRowHeight(y, _printPageScheme.DataRowHeight); }
            //}
        }

        public virtual void SetTitleStyle(int x1, int y1, int x2, int y2)
        {
            //var style = GetTitleStyle();
            ////var cell = sheet.Cells[x1, y1];
            //var cell = sheet.Cells[y1, x1];
            //cell.SetStyle(style);
            //MergeCell(x1, y1, x2, y2);
            //if (_printPageScheme != null)
            //{
            //    for (int y = y1; y <= y2; y++)
            //    {
            //        sheet.Cells.SetRowHeight(y1, _printPageScheme.TitleRowHeight);
            //    }
            //}
        }
        public virtual void SetFootStyle(int x1, int y1, int x2, int y2)
        {
            //var style = GetFootStyle();
            ////var cell = sheet.Cells[x1, y1];
            //var cell = sheet.Cells[y1, x1];
            //cell.SetStyle(style);
            //MergeCell(x1, y1, x2, y2);
            //if (_printPageScheme != null)
            //{
            //    for (int y = y1; y <= y2; y++)
            //    {
            //        sheet.Cells.SetRowHeight(y1, _printPageScheme.FootRowHeight);
            //    }
            //}
        }

        public void SetFont(int x1, int y1, int x2, int y2, string fontName)
        {
            //throw new NotImplementedException();
        }

        public Stream SaveAsStream()
        {
            var ms = new MemoryStream();
            //ms = doc.SaveToStream();
            var path = Path.Combine(PFDataHelper.BaseDirectory, "TempFile");
            document.Save(path);
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            //Read all bytes into an array from the specified file.
            int nBytes = (int)fs.Length;//计算流的长度
            byte[] byteArray = new byte[nBytes];//初始化用于MemoryStream的Buffer
            int nBytesRead = fs.Read(byteArray, 0, nBytes);//将File里的内容一次性的全部读到byteArray中去

            ms = new MemoryStream(byteArray);
            //            using (MemoryStream br = new MemoryStream(byteArray))//初始化MemoryStream,并将Buffer指向FileStream的读取结果数组
            //            {

            //}
            ms.Flush();
            ms.Position = 0;

            ////workbook = null;//为了便于后期合并cell,先不清空--wxj20181011
            ////sheet = null;
            return ms;
        }

        private void SetPrintPageScheme(PrintPageScheme scheme)
        {
            //_printPageScheme = scheme;

            //sheet.PageSetup.TopMargin = scheme.TopMargin;
            //sheet.PageSetup.RightMargin = scheme.RightMargin;
            //sheet.PageSetup.BottomMargin = scheme.BottomMargin;
            //sheet.PageSetup.LeftMargin = scheme.LeftMargin;
            ////sheet.Cells.SetRowHeight()     
        }

        //private Style GetHeadStyle()
        //{
        //    ////表头样式
        //    //var headStyle = workbook.CreateStyle();
        //    //headStyle.HorizontalAlignment = TextAlignmentType.Center;//居中对齐
        //    //headStyle.VerticalAlignment = TextAlignmentType.Center;
        //    //headStyle.IsTextWrapped = true;

        //    //////表头单元格背景色
        //    //headStyle.ForegroundColor = System.Drawing.Color.LightGreen;
        //    //headStyle.Pattern = BackgroundType.Solid;
        //    //////表头单元格边框
        //    //headStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
        //    //headStyle.Borders[BorderType.TopBorder].Color = Color.Black;
        //    //headStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
        //    //headStyle.Borders[BorderType.RightBorder].Color = Color.Black;
        //    //headStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
        //    //headStyle.Borders[BorderType.BottomBorder].Color = Color.Black;
        //    //headStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
        //    //headStyle.Borders[BorderType.LeftBorder].Color = Color.Black;

        //    //if (_printPageScheme != null)
        //    //{
        //    //    headStyle.ForegroundColor = _printPageScheme.HeadForegroundColor;
        //    //    headStyle.Font.Size = _printPageScheme.HeadFontSize;// 10;
        //    //}
        //    ////headStyle.Borders.SetStyle(CellBorderType.Thin);//统一设置,会连对角线都加上的,暂不知道如何解决
        //    ////headStyle.Borders.SetColor(Color.Black);

        //    ////headStyle.Font.Size = 10;
        //    ////headStyle.Font.IsBold = false;

        //    ////////表头字体设置
        //    //////var font = workbook.CreateFont();
        //    //////font.FontHeightInPoints = 12;//字号
        //    //////font.Boldweight = 600;//加粗
        //    ////////font.Color = HSSFColor.WHITE.index;//颜色
        //    //////headStyle.SetFont(font);

        //    return headStyle;
        //}

        //private Style GetDataStyle()
        //{
        //    //if (dataStyle == null)
        //    //{
        //    //    //数据样式
        //    //    dataStyle = workbook.CreateStyle();
        //    //    //dataStyle.Alignment = HorizontalAlignment.LEFT;//左对齐
        //    //    ////数据单元格的边框
        //    //    dataStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
        //    //    dataStyle.Borders[BorderType.TopBorder].Color = Color.Black;
        //    //    dataStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
        //    //    dataStyle.Borders[BorderType.RightBorder].Color = Color.Black;
        //    //    dataStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
        //    //    dataStyle.Borders[BorderType.BottomBorder].Color = Color.Black;
        //    //    dataStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
        //    //    dataStyle.Borders[BorderType.LeftBorder].Color = Color.Black;

        //    //    if (_printPageScheme != null)
        //    //    {
        //    //        dataStyle.Font.Size = _printPageScheme.DataFontSize;// 10;
        //    //    }
        //    //    //dataStyle.Font.IsBold = false;

        //    //    //////数据的字体
        //    //    ////var datafont = workbook.CreateFont();
        //    //    ////datafont.FontHeightInPoints = 11;//字号
        //    //    ////dataStyle.SetFont(datafont);
        //    //}

        //    return dataStyle;
        //}
        //private Style GetTitleStyle()
        //{
        //    ////if (dataStyle == null)
        //    ////{
        //    //var titleStyle = workbook.CreateStyle();
        //    ////数据样式
        //    //titleStyle = workbook.CreateStyle();


        //    ////////数据单元格的边框
        //    ////titleStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
        //    ////titleStyle.Borders[BorderType.TopBorder].Color = Color.Black;
        //    ////titleStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
        //    ////titleStyle.Borders[BorderType.RightBorder].Color = Color.Black;
        //    ////titleStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
        //    ////titleStyle.Borders[BorderType.BottomBorder].Color = Color.Black;
        //    ////titleStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
        //    ////titleStyle.Borders[BorderType.LeftBorder].Color = Color.Black;

        //    //if (_printPageScheme != null)
        //    //{
        //    //    //var f = new Aspose.Cells.Font(
        //    //    //{
        //    //    //    Size = _printPageScheme.TitleFontSize
        //    //    //};
        //    //    titleStyle.Font.Size = _printPageScheme.TitleFontSize;
        //    //    titleStyle.Font.IsBold = _printPageScheme.TitleFontIsBold;

        //    //    switch (_printPageScheme.TitleHorizontalAlignment)
        //    //    {
        //    //        case PFTextAlignmentType.Center:
        //    //            titleStyle.HorizontalAlignment = TextAlignmentType.Center;//左对齐
        //    //            break;
        //    //        default:
        //    //            break;
        //    //    }
        //    //}
        //    ////////数据的字体
        //    //////var datafont = workbook.CreateFont();
        //    //////datafont.FontHeightInPoints = 11;//字号
        //    //////titleStyle.SetFont(datafont);
        //    ////}

        //    return titleStyle;
        //}
        //private Style GetFootStyle()
        //{
        //    ////if (dataStyle == null)
        //    ////{
        //    var footStyle =  TableStyle();
        //    ////数据样式
        //    //footStyle = workbook.CreateStyle();
        //    ////footStyle.Alignment = HorizontalAlignment.LEFT;//左对齐
        //    ////////数据单元格的边框
        //    ////footStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
        //    ////footStyle.Borders[BorderType.TopBorder].Color = Color.Black;
        //    ////footStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
        //    ////footStyle.Borders[BorderType.RightBorder].Color = Color.Black;
        //    ////footStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
        //    ////footStyle.Borders[BorderType.BottomBorder].Color = Color.Black;
        //    ////footStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
        //    ////footStyle.Borders[BorderType.LeftBorder].Color = Color.Black;

        //    //if (_printPageScheme != null)
        //    //{
        //    //    //var f = new Aspose.Cells.Font(
        //    //    //{
        //    //    //    Size = _printPageScheme.FootFontSize
        //    //    //};
        //    //    footStyle.Font.Size = _printPageScheme.FootFontSize;
        //    //    //footStyle.Font.IsBold = _printPageScheme.FootFontIsBold;
        //    //}
        //    ////////数据的字体
        //    //////var datafont = workbook.CreateFont();
        //    //////datafont.FontHeightInPoints = 11;//字号
        //    //////footStyle.SetFont(datafont);
        //    ////}

        //    return footStyle;
        //}

        public void Dispose()
        {
            //workbook = null;//为了便于后期合并cell,先不清空--wxj20181011
            //sheet = null;
        }

        public void AddExport(IExport export, string title
            )
        {
            //var other = export as XlsExport;
            //if (other != null)
            //{
            //    var otherSheet = other.GetSheet();
            //    var s = workbook.Worksheets.Add(title ?? otherSheet.Name);
            //    s.Copy(otherSheet);
            //    //workbook.Worksheets[1] = other.GetSheet();
            //    //s = other.GetSheet();
            //}
        }

    }

    public class WordTemplateExport
    {
        private Document document;
        private DocumentBuilder builder;
        private Stream _fileStream = null;
        public string suffix { get { return "doc"; } }
        public string _downloadFileName = "";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="templateName">带后缀.docx</param>
        /// <param name="downloadFileName">不带后缀</param>
        public WordTemplateExport(string templateName, string downloadFileName)//, List<Dictionary<string, object>> rows)
        {
            var sourcePath = Path.Combine(PFDataHelper.BaseDirectory, "ExportTemplate", "Word", templateName);
            var targetPath = PFDataHelper.BaseDirectory + "word.doc";

            //Aspose.Words.License l = new Aspose.Words.License();
            //l.SetLicense(Path.Combine(HttpRuntime.AppDomainAppPath, "lib/Aid/WordLicense.lic"));
            document = new Aspose.Words.Document(sourcePath);
            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(document);

            _downloadFileName = downloadFileName;
        }
        public Document GetDocument()
        {
            return document;
        }
        /// <summary>
        /// 替换Word里的词{xx}.现时没有为空时忽略的需求,所以当空串
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        public WordTemplateExport ReplaceWords(Dictionary<string, string> words)
        {
            foreach (var word in words)
            {
                document.Range.Replace("{" + word.Key + "}", word.Value ?? "", false, false);//3 4参数不知道什么用,传true反而不行
            }
            return this;
        }
        public Stream SaveAsStream()
        {
            var ms = new MemoryStream();

            var path = Path.Combine(PFDataHelper.BaseDirectory, "TempFile");

            document.Save(ms, SaveFormat.Doc);

            ms.Flush();
            ms.Position = 0;

            ////workbook = null;//为了便于后期合并cell,先不清空--wxj20181011
            ////sheet = null;
            return ms;
        }
        public void Download()
        {
            // var _fileStream = SaveAsStream();
            if (_fileStream == null)
            {
                _fileStream = SaveAsStream();
            }
            if (PFDataHelper.StringIsNullOrWhiteSpace(_downloadFileName))
            {
                _downloadFileName = DateTime.Now.ToString("yyyyMMddHHmmss");
            }
            if (_fileStream != null && _fileStream.Length > 0)
            {
                //PFDataHelper.DownloadFile(HttpContext.Current, _fileStream, string.Format("{0}.{1}", _fileName, _suffix), 1024 * 1024 * 10);
                PFDataHelper.DownloadFile(HttpContext.Current, _fileStream, string.Format("{0}.{1}", _downloadFileName, suffix), PFDataHelper.GetConfigMapper().GetNetworkConfig().DownloadSpeed);
            }
            Dispose();
        }
        public void Dispose()
        {
            if (_fileStream != null)
            {
                _fileStream.Dispose();
            }
        }
    }
}
