using Aspose.Cells;
using System.Drawing;
using System.IO;
using System.Web;
using System;
using System.Reflection;

namespace Perfect
{
    /// <summary>
    /// 注意：x为列,y为行。但在Aspose.Cells里习惯了把y放在x前面
    /// </summary>
    public class XlsxExport : IExport
    {
        public string suffix { get { return "xlsx"; } }

        public Workbook workbook;
        private Worksheet sheet;
        private Style dataStyle;
        private PrintPageScheme _printPageScheme;//打印页面样式方案--wxj20181011

        /// <summary>
        /// 报错:(初步怀疑是破解盗版时产生的问题)
        /// time:[2019/5/22 15:13:47]
        ///      System.NullReferenceException: Object reference not set to an instance of an object.
        /// at ح.ⴗ.⴦(Stream Ԡ)
        /// at ح.ⴗ.⴦(String ⴧ, Assembly ⴨)
        /// at Perfect.XlsExport.Init(Object data, PrintPageScheme scheme)
        /// at Perfect.Exporter.Export(IExport export)
        /// at Perfect.Exporter.Export(String type)
        /// at Perfect.Exporter.Instance(PagingResult pagingResult, ExporterOption opts)
        /// at YJQuery.Web.Areas.ProjOut.Controllers.ProjectRequirementController.GetExporter(String cmonthff, String fgsno, String fgsname, String hr, String fbatch, String SfName)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="scheme"></param>
        public void Init(object data, PrintPageScheme scheme)
        {
            //try
            //{
            //    XlsExport.InitializeAsposeCells();
            //    //Aspose.Cells.License l = new Aspose.Cells.License();
            //    ////l.SetLicense(Path.Combine(HttpRuntime.AppDomainAppPath, "lib/Aid/License.lic"));
            //    //l.SetLicense(Path.Combine(PFDataHelper.BaseDirectory, "lib/Aid/License.lic"));
            //}
            //catch (Exception e)
            //{
            //    PFDataHelper.WriteError(e);
            //}
            workbook = new Workbook(FileFormatType.Xlsx);
            sheet = workbook.Worksheets[0];

            if (scheme != null) { SetPrintPageScheme(scheme); }
        }
        #region Excel专用方法(非通用)
        public Worksheet GetSheet()
        {
            return sheet;
        }
        public void SetSheetTabName(string name)
        {
            sheet.Name = name;
        }
        #endregion

        public void SetColumnWidth(int x, string px)
        {
            double width = PFDataHelper.WebWidthToExcel(px).Value;
            sheet.Cells.SetColumnWidth(x, width);
            //sheet.Cells.SetColumnWidth(x, width);
        }
        public void SetRowHeight(int y, string px)
        {
            double height = PFDataHelper.WebWidthToExcel(px).Value;
            sheet.Cells.SetRowHeight(y, height);
        }
        /// <summary>
        /// 设置自动换行
        /// </summary>
        /// <param name="x1">列</param>
        /// <param name="y1">行</param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public void SetTextWrapped(int x1, int y1, int x2, int y2)
        {
            var style = sheet.Cells[y1, x1].GetStyle();
            style.IsTextWrapped = true;
            for (var y = y1; y <= y2; y++)
            {
                for (var x = x1; x <= x2; x++)
                {
                    var cell = sheet.Cells[y, x];
                    cell.SetStyle(style);
                }
            }
        }
        public void MergeCell(int x1, int y1, int x2, int y2)
        {
            //Range range = sheet.Cells.CreateRange(x1, y1, x2, y2);
            //Range range = sheet.Cells.CreateRange(x1, y1, x2-x1+1, y2-y1+1);
            Range range = sheet.Cells.CreateRange(y1, x1, y2 - y1 + 1, x2 - x1 + 1);
            range.Merge();

        }


        public virtual void FillData(int x, int y, string field, object value)
        {

            //if (!field.StartsWith("title_"))
            //    cell.SetStyle(GetDataStyle());

            switch ((value ?? string.Empty).GetType().Name.ToLower())
            {
                case "int32":
                case "int64":
                case "decimal":
                    sheet.Cells[y, x].PutValue(PFDataHelper.ObjectToType<double>(value, 0));
                    break;
                //case "System.String[]":
                //    var s = String.Join(",", (string[])value);
                //    sheet.Cells[y, x].PutValue(s);
                //    break;
                default:
                    if (value is string[])
                    {
                        var s = String.Join(",", value as string[]);
                        sheet.Cells[y, x].PutValue(s);
                    }
                    else
                    {
                        sheet.Cells[y, x].PutValue(PFDataHelper.ObjectToString(value));
                    }
                    break;
            }
        }

        public virtual void SetHeadStyle(int x1, int y1, int x2, int y2)
        {
            var style = GetHeadStyle();
            for (var y = y1; y <= y2; y++)
            {
                for (var x = x1; x <= x2; x++)
                {
                    var cell = sheet.Cells[y, x];
                    cell.SetStyle(style);
                }
                if (_printPageScheme != null) { sheet.Cells.SetRowHeight(y, _printPageScheme.HeadRowHeight); }
            }
        }

        public virtual void SetHeadStyle(int x1, int y1, int x2, int y2, PrintPageScheme printPageScheme)
        {
            var style = GetHeadStyle(printPageScheme);
            for (var y = y1; y <= y2; y++)
            {
                for (var x = x1; x <= x2; x++)
                {
                    var cell = sheet.Cells[y, x];
                    cell.SetStyle(style);
                }
                if (printPageScheme != null) { sheet.Cells.SetRowHeight(y, printPageScheme.HeadRowHeight); }
            }
        }

        public virtual void SetRowsStyle(int x1, int y1, int x2, int y2)
        {
            var style = GetDataStyle();
            for (var y = y1; y <= y2; y++)
            {
                for (var x = x1; x <= x2; x++)
                {
                    var cell = sheet.Cells[y, x];
                    cell.SetStyle(style);
                }
                if (_printPageScheme != null) { sheet.Cells.SetRowHeight(y, _printPageScheme.DataRowHeight); }
            }
        }

        public virtual void SetTitleStyle(int x1, int y1, int x2, int y2)
        {
            var style = GetTitleStyle();
            //var cell = sheet.Cells[x1, y1];
            var cell = sheet.Cells[y1, x1];
            cell.SetStyle(style);
            MergeCell(x1, y1, x2, y2);
            if (_printPageScheme != null)
            {
                for (int y = y1; y <= y2; y++)
                {
                    sheet.Cells.SetRowHeight(y1, _printPageScheme.TitleRowHeight);
                }
            }
        }
        public virtual void SetFootStyle(int x1, int y1, int x2, int y2)
        {
            var style = GetFootStyle();
            //var cell = sheet.Cells[x1, y1];
            var cell = sheet.Cells[y1, x1];
            cell.SetStyle(style);
            MergeCell(x1, y1, x2, y2);
            if (_printPageScheme != null)
            {
                for (int y = y1; y <= y2; y++)
                {
                    sheet.Cells.SetRowHeight(y1, _printPageScheme.FootRowHeight);
                }
            }
        }

        public void SetFont(int x1, int y1, int x2, int y2, string fontName)
        {
            for (var y = y1; y <= y2; y++)
            {
                for (var x = x1; x <= x2; x++)
                {
                    var cell = sheet.Cells[y, x];
                    var style = cell.GetStyle();
                    style.Font.Name = fontName;
                    cell.SetStyle(style);
                }
            }
        }

        public Stream SaveAsStream()
        {
            var ms = new MemoryStream();
            ms = workbook.SaveToStream();

            ms.Flush();
            ms.Position = 0;

            //workbook = null;//为了便于后期合并cell,先不清空--wxj20181011
            //sheet = null;
            return ms;
        }

        private void SetPrintPageScheme(PrintPageScheme scheme)
        {
            _printPageScheme = scheme;

            sheet.PageSetup.TopMargin = scheme.TopMargin;
            sheet.PageSetup.RightMargin = scheme.RightMargin;
            sheet.PageSetup.BottomMargin = scheme.BottomMargin;
            sheet.PageSetup.LeftMargin = scheme.LeftMargin;
            //sheet.Cells.SetRowHeight()     
        }

        private Style GetHeadStyle()
        {
            //表头样式
            var headStyle = workbook.CreateStyle();
            headStyle.HorizontalAlignment = TextAlignmentType.Center;//居中对齐
            headStyle.VerticalAlignment = TextAlignmentType.Center;
            headStyle.IsTextWrapped = true;

            ////表头单元格背景色
            headStyle.ForegroundColor = System.Drawing.Color.LightGreen;
            headStyle.Pattern = BackgroundType.Solid;
            ////表头单元格边框
            headStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            headStyle.Borders[BorderType.TopBorder].Color = Color.Black;
            headStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            headStyle.Borders[BorderType.RightBorder].Color = Color.Black;
            headStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            headStyle.Borders[BorderType.BottomBorder].Color = Color.Black;
            headStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            headStyle.Borders[BorderType.LeftBorder].Color = Color.Black;

            if (_printPageScheme != null)
            {
                headStyle.ForegroundColor = _printPageScheme.HeadForegroundColor;
                headStyle.Font.Size = _printPageScheme.HeadFontSize;// 10;
            }
            //headStyle.Borders.SetStyle(CellBorderType.Thin);//统一设置,会连对角线都加上的,暂不知道如何解决
            //headStyle.Borders.SetColor(Color.Black);

            //headStyle.Font.Size = 10;
            //headStyle.Font.IsBold = false;

            //////表头字体设置
            ////var font = workbook.CreateFont();
            ////font.FontHeightInPoints = 12;//字号
            ////font.Boldweight = 600;//加粗
            //////font.Color = HSSFColor.WHITE.index;//颜色
            ////headStyle.SetFont(font);

            return headStyle;
        }

        private Style GetHeadStyle(PrintPageScheme printPageScheme)
        {
            //表头样式
            var headStyle = workbook.CreateStyle();
            headStyle.HorizontalAlignment = TextAlignmentType.Center;//居中对齐
            headStyle.VerticalAlignment = TextAlignmentType.Center;
            headStyle.IsTextWrapped = true;

            ////表头单元格背景色
            headStyle.ForegroundColor = System.Drawing.Color.LightGreen;
            headStyle.Pattern = BackgroundType.Solid;
            ////表头单元格边框
            headStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            headStyle.Borders[BorderType.TopBorder].Color = Color.Black;
            headStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            headStyle.Borders[BorderType.RightBorder].Color = Color.Black;
            headStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            headStyle.Borders[BorderType.BottomBorder].Color = Color.Black;
            headStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            headStyle.Borders[BorderType.LeftBorder].Color = Color.Black;

            if (printPageScheme != null)
            {
                headStyle.ForegroundColor = printPageScheme.HeadForegroundColor;
                headStyle.Font.Size = printPageScheme.HeadFontSize;// 10;
            }
            //headStyle.Borders.SetStyle(CellBorderType.Thin);//统一设置,会连对角线都加上的,暂不知道如何解决
            //headStyle.Borders.SetColor(Color.Black);

            //headStyle.Font.Size = 10;
            //headStyle.Font.IsBold = false;

            //////表头字体设置
            ////var font = workbook.CreateFont();
            ////font.FontHeightInPoints = 12;//字号
            ////font.Boldweight = 600;//加粗
            //////font.Color = HSSFColor.WHITE.index;//颜色
            ////headStyle.SetFont(font);

            return headStyle;
        }

        private Style GetDataStyle()
        {
            if (dataStyle == null)
            {
                //数据样式
                dataStyle = workbook.CreateStyle();
                //dataStyle.Alignment = HorizontalAlignment.LEFT;//左对齐
                ////数据单元格的边框
                dataStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                dataStyle.Borders[BorderType.TopBorder].Color = Color.Black;
                dataStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                dataStyle.Borders[BorderType.RightBorder].Color = Color.Black;
                dataStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                dataStyle.Borders[BorderType.BottomBorder].Color = Color.Black;
                dataStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                dataStyle.Borders[BorderType.LeftBorder].Color = Color.Black;

                if (_printPageScheme != null)
                {
                    dataStyle.Font.Size = _printPageScheme.DataFontSize;// 10;
                }
                //dataStyle.Font.Name = "宋体";//默认的Arial字体中,树型的┝等符号对不齐--benjamin20190711
                //dataStyle.Font.IsBold = false;

                //////数据的字体
                ////var datafont = workbook.CreateFont();
                ////datafont.FontHeightInPoints = 11;//字号
                ////dataStyle.SetFont(datafont);
            }

            return dataStyle;
        }
        private Style GetTitleStyle()
        {
            //if (dataStyle == null)
            //{
            var titleStyle = workbook.CreateStyle();
            //数据样式
            titleStyle = workbook.CreateStyle();


            //////数据单元格的边框
            //titleStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            //titleStyle.Borders[BorderType.TopBorder].Color = Color.Black;
            //titleStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            //titleStyle.Borders[BorderType.RightBorder].Color = Color.Black;
            //titleStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            //titleStyle.Borders[BorderType.BottomBorder].Color = Color.Black;
            //titleStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            //titleStyle.Borders[BorderType.LeftBorder].Color = Color.Black;

            if (_printPageScheme != null)
            {
                //var f = new Aspose.Cells.Font(
                //{
                //    Size = _printPageScheme.TitleFontSize
                //};
                titleStyle.Font.Size = _printPageScheme.TitleFontSize;
                titleStyle.Font.IsBold = _printPageScheme.TitleFontIsBold;

                switch (_printPageScheme.TitleHorizontalAlignment)
                {
                    case PFTextAlignmentType.Center:
                        titleStyle.HorizontalAlignment = TextAlignmentType.Center;//左对齐
                        break;
                    default:
                        break;
                }
            }
            //////数据的字体
            ////var datafont = workbook.CreateFont();
            ////datafont.FontHeightInPoints = 11;//字号
            ////titleStyle.SetFont(datafont);
            //}

            return titleStyle;
        }
        private Style GetFootStyle()
        {
            //if (dataStyle == null)
            //{
            var footStyle = workbook.CreateStyle();
            //数据样式
            footStyle = workbook.CreateStyle();
            //footStyle.Alignment = HorizontalAlignment.LEFT;//左对齐
            //////数据单元格的边框
            //footStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            //footStyle.Borders[BorderType.TopBorder].Color = Color.Black;
            //footStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            //footStyle.Borders[BorderType.RightBorder].Color = Color.Black;
            //footStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            //footStyle.Borders[BorderType.BottomBorder].Color = Color.Black;
            //footStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            //footStyle.Borders[BorderType.LeftBorder].Color = Color.Black;

            if (_printPageScheme != null)
            {
                //var f = new Aspose.Cells.Font(
                //{
                //    Size = _printPageScheme.FootFontSize
                //};
                footStyle.Font.Size = _printPageScheme.FootFontSize;
                //footStyle.Font.IsBold = _printPageScheme.FootFontIsBold;
            }
            //////数据的字体
            ////var datafont = workbook.CreateFont();
            ////datafont.FontHeightInPoints = 11;//字号
            ////footStyle.SetFont(datafont);
            //}

            return footStyle;
        }

        public void Dispose()
        {
            workbook = null;//为了便于后期合并cell,先不清空--wxj20181011
            sheet = null;
        }

        public void AddExport(IExport export, string title = null
            )
        {
            var other = export as XlsExport;
            if (other != null)
            {
                var otherSheet = other.GetSheet();
                var s = workbook.Worksheets.Add(title ?? otherSheet.Name);
                s.Copy(otherSheet);
                //workbook.Worksheets[1] = other.GetSheet();
                //s = other.GetSheet();
            }
        }

        #region static
        public static void Crack()
        {

            try
            {
                //XlsExport.InitializeAsposeCells();
                Aspose.Cells.License l = new Aspose.Cells.License();
                //l.SetLicense(Path.Combine(HttpRuntime.AppDomainAppPath, "lib/Aid/License.lic"));
                l.SetLicense(Path.Combine(PFDataHelper.BaseDirectory, "lib/Aid/License.lic"));
            }
            catch (Exception e)
            {
                PFDataHelper.WriteError(e);
            }
        }
        public static void InitializeAsposeCells()
        {
            const BindingFlags BINDING_FLAGS_ALL = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

            const string CLASS_LICENSER = "\u0092\u0092\u0008.\u001C";
            const string CLASS_LICENSERHELPER = "\u0011\u0001\u0006.\u001A";
            const string ENUM_ISTRIAL = "\u0092\u0092\u0008.\u001B";

            const string FIELD_LICENSER_CREATED_LICENSE = "\u0001";     // static
            const string FIELD_LICENSER_EXPIRY_DATE = "\u0002";         // instance
            const string FIELD_LICENSER_ISTRIAL = "\u0001";             // instance

            const string FIELD_LICENSERHELPER_INT128 = "\u0001";        // static
            const string FIELD_LICENSERHELPER_BOOLFALSE = "\u0001";     // static

            const int CONST_LICENSER_ISTRIAL = 1;
            const int CONST_LICENSERHELPER_INT128 = 128;
            const bool CONST_LICENSERHELPER_BOOLFALSE = false;

            //- Field setter for convinient
            Action<FieldInfo, Type, string, object, object> setValue =
                delegate (FieldInfo field, Type chkType, string chkName, object obj, object value)
                {
                    if ((field.FieldType == chkType) && (field.Name == chkName))
                    {
                        field.SetValue(obj, value);
                    }
                };


            //- Get types
            Assembly assembly = Assembly.GetAssembly(typeof(Aspose.Cells.License));
            Type typeLic = null, typeIsTrial = null, typeHelper = null;
            foreach (Type type in assembly.GetTypes())
            {
                if ((typeLic == null) && (type.FullName == CLASS_LICENSER))
                {
                    typeLic = type;
                }
                else if ((typeIsTrial == null) && (type.FullName == ENUM_ISTRIAL))
                {
                    typeIsTrial = type;
                }
                else if ((typeHelper == null) && (type.FullName == CLASS_LICENSERHELPER))
                {
                    typeHelper = type;
                }
            }
            if (typeLic == null || typeIsTrial == null || typeHelper == null)
            {
                throw new Exception();
            }

            //- In class_Licenser
            object license = Activator.CreateInstance(typeLic);
            foreach (FieldInfo field in typeLic.GetFields(BINDING_FLAGS_ALL))
            {
                setValue(field, typeLic, FIELD_LICENSER_CREATED_LICENSE, null, license);
                setValue(field, typeof(DateTime), FIELD_LICENSER_EXPIRY_DATE, license, DateTime.MaxValue);
                setValue(field, typeIsTrial, FIELD_LICENSER_ISTRIAL, license, CONST_LICENSER_ISTRIAL);
            }

            //- In class_LicenserHelper
            foreach (FieldInfo field in typeHelper.GetFields(BINDING_FLAGS_ALL))
            {
                setValue(field, typeof(int), FIELD_LICENSERHELPER_INT128, null, CONST_LICENSERHELPER_INT128);
                setValue(field, typeof(bool), FIELD_LICENSERHELPER_BOOLFALSE, null, CONST_LICENSERHELPER_BOOLFALSE);
            }
        }
        #endregion static
    }
}
