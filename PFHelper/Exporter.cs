using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Collections;
using System.Web.Mvc;
using System.Drawing;
using System.Diagnostics;

namespace Perfect
{
    /// <summary>
    /// 打印页样式方案（注意title是指标题，head是指表头.但Exporter里的title指表头）
    /// </summary>
    public class PrintPageScheme
    {
        #region 页边距
        public double TopMargin { get; set; }
        public double RightMargin { get; set; }
        public double BottomMargin { get; set; }
        public double LeftMargin { get; set; }

        #endregion
        #region 数据格式
        public double DataRowHeight { get; set; }
        public int DataFontSize { get; set; }
        #endregion
        #region 表头格式(列头)
        public double HeadRowHeight { get; set; }
        public Color HeadForegroundColor { get; set; }
        public int HeadFontSize { get; set; }
        #endregion
        #region 标题格式
        public double TitleRowHeight { get; set; }
        public int TitleFontSize { get; set; }
        public bool TitleFontIsBold { get; set; }
        public PFTextAlignmentType TitleHorizontalAlignment { get; set; }
        #endregion
        #region Foot格式
        public double FootRowHeight { get; set; }
        public int FootFontSize { get; set; }
        #endregion
    }
    public enum PFTextAlignmentType
    {
        Left = 1,
        Center = 2,
        Right = 3
    }
    public class ExporterOption
    {
        public string FileType { get; set; }
        public PrintPageScheme Scheme { get; set; }
        /// <summary>
        /// 正文的标题
        /// </summary>
        public string SheetTitle { get; set; }
        /// <summary>
        /// 正文的页脚
        /// </summary>
        public string SheetFoot { get; set; }
    }
    public class Exporter : IDisposable
    {
        const string DEFAULT_EXPORT = "xls";
        const string DEFAULT_DATAGETTER = "api";
        const string DEFAULT_COMPRESS = "none";

        private Dictionary<string, Type> _compress = new Dictionary<string, Type>() { 
            //{ "zip", typeof(ZipCompress)},
            {"none",typeof(NoneCompress)}
        };
        private Dictionary<string, Type> _dataGetter = new Dictionary<string, Type>() {
            { "api", typeof(ApiData) }
        };
        private Dictionary<string, Type> _export = new Dictionary<string, Type>() { 
            //{ "xls", typeof(XlsExport) },
            //{ "xls", typeof(XlsExport65535) },
            //{ "xls", typeof(XlsxExport) },
            { "xls", typeof(XlsxExport1048576) },
            { "doc", typeof(WordExport) }
            //, 
            //{ "xlsx", typeof(XlsxExport) } 
            //,
            //{ "doc", typeof(HtmlDocExport) },
            //{ "pdf", typeof(PdfExport) }
        };

        private Dictionary<string, IFormatter> _fieldFormatter = new Dictionary<string, IFormatter>();

        private object _data;
        private List<List<StoreColumn>> _title;
        private StoreColumnCollection _columns;//这个类型比_title实用--wxj20181010
        private IExport _exporter;//有这个属性比较容易做后期合并cell操作--wxj20181011
        private PrintPageScheme _printPageScheme;//打印页面样式方案--wxj20181011
        private string _sheetTitle;//打印页面样式方案--wxj20181011
        private string _sheetFoot;
        private Stream _fileStream = null;
        private string _fileName = string.Empty;
        private string _suffix = string.Empty;
        private IController _controller = null;

        public static PrintPageScheme FinancialScheme = new PrintPageScheme
        {
            TopMargin = 0.4,
            RightMargin = 0.3,
            BottomMargin = 0.4,
            LeftMargin = 0.3,

            DataRowHeight = 13.5,
            DataFontSize = 9,

            HeadRowHeight = 13.5,
            //HeadForegroundColor=null, //默认是rgb(0,0,0),但显示为无色，原因未明
            HeadFontSize = 9,

            TitleRowHeight = 25.5,//20,
            TitleFontSize = 16,
            TitleFontIsBold = true,
            TitleHorizontalAlignment = PFTextAlignmentType.Center,

            FootRowHeight = 36.75,
            FootFontSize = 11
        };

        public static Exporter Instance(IController controller)
        {
            var export = new Exporter();
            export._controller = controller;
            var context = HttpContext.Current;

            //if (context.Request.Form["titles"] != null)
            //    export.Title(JsonConvert.DeserializeObject<List<List<StoreColumn>>>(context.Request.Form["titles"]));//原本的title是从前端传过来,现改为统一用_data里的columns

            if (context.Request.Form["dataGetter"] != null)
            {
                export.Data(context.Request.Form["dataGetter"]);
                var columns = (export._data as PagingResult).columns as StoreColumnCollection;//这里可以扩展为多表头
                ////export.Title(new List<List<StoreColumn>> { columns });//树型必需转2维数组
                //var title = new List<List<StoreColumn>>();
                //var maxDepth = new StoreColumn { Children = columns }.GetDepth()-1;
                //StoreColumnCollection.StoreColumnTo2DArray(ref title, columns,ref maxDepth);
                //export.Title(title);
                export.Title(columns);
            }

            if (context.Request.Form["fileType"] != null)
                export.Export(context.Request.Form["fileType"]);

            if (context.Request.Form["compressType"] != null)
                export.Compress(context.Request.Form["compressType"]);

            return export;
        }
        public static Exporter Instance(PagingResult pagingResult, ExporterOption opts)
        {
            var export = new Exporter();
            //export._controller = controller;
            var context = HttpContext.Current;

            string fileType = opts.FileType; PrintPageScheme scheme = opts.Scheme; string sheetTitle = opts.SheetTitle; string sheetFoot = opts.SheetFoot;

            export._printPageScheme = scheme;
            export._sheetTitle = sheetTitle;
            export._sheetFoot = sheetFoot;

            //if (context.Request.Form["titles"] != null)
            //    export.Title(JsonConvert.DeserializeObject<List<List<StoreColumn>>>(context.Request.Form["titles"]));//原本的title是从前端传过来,现改为统一用_data里的columns

            //if (context.Request.Form["dataGetter"] != null)
            //{
            //export.Data(context.Request.Form["dataGetter"]);
            export.Data(pagingResult);
            var columns = (export._data as PagingResult).columns as StoreColumnCollection;//这里可以扩展为多表头
                                                                                          ////export.Title(new List<List<StoreColumn>> { columns });//树型必需转2维数组
                                                                                          //var title = new List<List<StoreColumn>>();
                                                                                          //var maxDepth = new StoreColumn { Children = columns }.GetDepth() - 1;
                                                                                          //StoreColumnCollection.StoreColumnTo2DArray(ref title, columns, ref maxDepth);
                                                                                          //export.Title(title);
            export.Title(columns);
            //}

            export.Export(fileType);

            if (context != null && context.Request.Form["compressType"] != null)
                export.Compress(context.Request.Form["compressType"]);

            return export;
        }
        //private static Exporter Instance()//旧方法,反射controller会导致方便调用时session为空
        //{
        //    var export = new Exporter();
        //    var context = HttpContext.Current;

        //    //if (context.Request.Form["titles"] != null)
        //    //    export.Title(JsonConvert.DeserializeObject<List<List<StoreColumn>>>(context.Request.Form["titles"]));//原本的title是从前端传过来,现改为统一用_data里的columns

        //    if (context.Request.Form["dataGetter"] != null)
        //    {
        //        export.Data(context.Request.Form["dataGetter"]);
        //        var columns = (export._data as PagingResult).columns as StoreColumnCollection;//这里可以扩展为多表头
        //        export.Title(new List<List<StoreColumn>> { columns });
        //    }

        //    if (context.Request.Form["fileType"] != null)
        //        export.Export(context.Request.Form["fileType"]);

        //    if (context.Request.Form["compressType"] != null)
        //        export.Compress(context.Request.Form["compressType"]);

        //    return export;
        //}


        public Exporter Data(IDataGetter data)
        {
            //_data = data.GetData(HttpContext.Current);
            _data = data.GetData(_controller, HttpContext.Current);
            return this;
        }

        public Exporter Data(string type)
        {
            var dataGetter = GetActor<IDataGetter>(_dataGetter, DEFAULT_DATAGETTER, type);
            return Data(dataGetter);
        }

        public Exporter Data(object data)
        {
            _data = data;
            return this;
        }

        public Exporter AddFormatter(string field, IFormatter formatter)
        {
            _fieldFormatter[field] = formatter;
            return this;
        }

        public Exporter Title(List<List<StoreColumn>> title)
        {
            _title = title;
            return this;
        }
        public Exporter Title(StoreColumnCollection columns)
        {
            var title = new List<List<StoreColumn>>();
            columns.ForEach(a =>
            {
                if (a.Children.Any() && a.GetAllLeafCount(b => b.visible) < 1)//如果不是叶节点且所有子叶节点都隐藏,那么本父节点也隐藏
                {
                    a.visible = false;
                }
            });
            var maxDepth = new StoreColumn { Children = columns }.GetDepth() - 1;
            StoreColumnCollection.StoreColumnTo2DArray(ref title, columns, ref maxDepth);
            _title = title;
            _columns = columns;
            return this;
        }

        /// <summary>
        /// 下载的文件名
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public Exporter FileName(string fileName)
        {
            _fileName = fileName;
            return this;
        }

        public Exporter Export(string type)
        {
            var export = GetActor<IExport>(_export, DEFAULT_EXPORT, type);
            return Export(export);
        }

        public Exporter Export(IExport export)
        {
            int i = 0;
            if (_title == null)
            {
                _title = new List<List<StoreColumn>>();
                _title.Add(new List<StoreColumn>());
                //PFDataHelper.EachListHeader(_data, (i, field, type) => _title[0].Add(new StoreColumn() { title = field, field = field, rowspan = 1, colspan = 1 }));
                PFDataHelper.EachListHeader(_data, (a, field, type) => _title[0].Add(new StoreColumn()
                {
                    title = field,
                    data = field
                    //, rowspan = 1, colspan = 1
                }));
            }

            Dictionary<int, int> currentHeadRow = new Dictionary<int, int>();
            Dictionary<string, List<int>> fieldIndex = new Dictionary<string, List<int>>();

            int titleRowCount = 0;
            if (!string.IsNullOrWhiteSpace(_sheetTitle))
            {
                titleRowCount++;
            }
            Func<int, int> GetCurrentHeadRow = cell => currentHeadRow.ContainsKey(cell) ? currentHeadRow[cell] : titleRowCount;
            var currentRow = 0;
            var currentCell = 0;

            export.Init(_data, _printPageScheme);

            //标题--wxj20181011
            var temp = new StoreColumn { Children = _columns };
            int columnCount = temp.GetAllLeafCount(a => a.visible);
            var firstData = temp.FirstLeaf(a => true).data;
            if (!string.IsNullOrWhiteSpace(_sheetTitle))
            {
                export.FillData(0, 0, firstData, _sheetTitle);

                export.SetTitleStyle(0, 0, columnCount - 1, 0);
                currentRow++;
            }

            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            //int resultCount = 0;
            #region 3秒(后来发现只有调试时特别慢,原因未明)
            ////生成多行题头
            for (i = 0; i < _title.Count; i++)
            {
                currentCell = 0;

                for (var j = 0; j < _title[i].Count; j++)
                {
                    var item = _title[i][j];
                    if (item.visible == false) { continue; }//隐藏列不导出--wxj20181009

                    while (currentRow < GetCurrentHeadRow(currentCell))
                        currentCell++;

                    //export.FillData(currentCell, currentRow, "title_" + item.data, item.title);
                    export.FillData(currentCell, currentRow, "title_" + item.data, item.title ?? item.data);//e:\svn\businesssys2018\yjquery.web\areas\bonus\views\reportquery05\treegrid.cshtml里的title是null

                    if (item.rowspan + item.colspan > 2)
                        export.MergeCell(currentCell, currentRow, currentCell + item.colspan - 1, currentRow + item.rowspan - 1);

                    if (!string.IsNullOrEmpty(item.data))
                    {
                        if (!fieldIndex.ContainsKey(item.data))
                            fieldIndex[item.data] = new List<int>();
                        fieldIndex[item.data].Add(currentCell);
                    }

                    for (var k = 0; k < item.colspan; k++)
                    {
                        currentHeadRow[currentCell] = GetCurrentHeadRow(currentCell++) + item.rowspan;
                    }
                    //resultCount++;
                }
                currentRow++;
            }
            #endregion
            #region 一样是3秒
            ////生成多行题头
            //foreach (var ii in _title)
            //{
            //    currentCell = 0;

            //    foreach (var j in ii)
            //    {
            //        //var item = _title[i][j];
            //        var item = j;
            //        if (item.visible == false) { continue; }//隐藏列不导出--wxj20181009
            //        //if (item.hidden) continue;

            //        while (currentRow < GetCurrentHeadRow(currentCell))
            //            currentCell++;

            //        //export.FillData(currentCell, currentRow, "title_" + item.data, item.title);
            //        export.FillData(currentCell, currentRow, "title_" + item.data, item.title ?? item.data);//e:\svn\businesssys2018\yjquery.web\areas\bonus\views\reportquery05\treegrid.cshtml里的title是null

            //        if (item.rowspan + item.colspan > 2)
            //            export.MergeCell(currentCell, currentRow, currentCell + item.colspan - 1, currentRow + item.rowspan - 1);

            //        if (!string.IsNullOrEmpty(item.data))
            //        {
            //            if (!fieldIndex.ContainsKey(item.data))
            //                fieldIndex[item.data] = new List<int>();
            //            fieldIndex[item.data].Add(currentCell);
            //        }

            //        for (var k = 0; k < item.colspan; k++)
            //            currentHeadRow[currentCell] = GetCurrentHeadRow(currentCell++) + item.rowspan;
            //    }
            //    currentRow++;
            //} 
            #endregion

            //sw.Stop();
            //var aa = string.Format("插入{0}条记录共花费{1}毫秒，{2}分钟", resultCount, sw.ElapsedMilliseconds, sw.ElapsedMilliseconds / 1000 / 60);

            //设置题头样式
            //export.SetHeadStyle(0, 0, currentCell - 1, currentRow - 1);
            export.SetHeadStyle(0, titleRowCount, currentHeadRow.Count - 1, currentRow - 1);//上面那样,当后面的列不是多表头时,背景色只填到最后一个多表头为止

            ////设置数据样式
            var dataCount = 0;
            if (_data is PagingResult)
            {
                var data = _data as PagingResult;
                var list = data.data as ArrayList;
                if (list != null)
                {
                    for (var rowIndex = 0; rowIndex < list.Count; rowIndex++)
                    {
                        dataCount++;
                    }
                }
                else
                {
                    var list1 = data.data as List<TreeListItem>;
                    if (list1 != null)
                    {
                        (new TreeListItem { Children = list1 }).EachChild(a => dataCount++);
                    }
                }
            }
            else
            {
                PFDataHelper.EachListRow(_data, (a, r) => dataCount++);//原版
            }
            ////export.SetRowsStyle(0, currentRow, currentCell - 1, currentRow + dataCount - 1);
            //此句内报错，要优化--benjamin todo
            if (!PFDataHelper.IsDebug)
            {
                export.SetRowsStyle(0, currentRow, currentHeadRow.Count - 1, currentRow + dataCount - 1);//上面那样,当后面的列不是多表头时,边框不见了
            }

            //填充数据
            if (_data is PagingResult)
            {
                var data = _data as PagingResult;
                if (data.data is List<TreeListItem>)
                {
                    export.SetFont(0, currentRow, 0, currentRow + dataCount - 1, "宋体");//默认的Arial字体中,树型的┝等符号对不齐--benjamin20190711
                    var tree = new TreeListItem();
                    tree.Children = data.data as List<TreeListItem>;
                    int rowIndex = 0;
                    int colIndex = 0;
                    var matrix = new TreeMatrix(tree.Children);
                    tree.EachChild((a, deep) =>
                    {
                        colIndex = 0;
                        PFDataHelper.EachObjectProperty(a.Data, (b, name, value) =>
                        {
                            if (fieldIndex.ContainsKey(name))
                            {
                                foreach (int cellIndex in fieldIndex[name])
                                {
                                    if (_fieldFormatter.ContainsKey(name))
                                    {
                                        value = _fieldFormatter[name].Format(value);
                                    }
                                    if (colIndex == 0)
                                    {
                                        var line = "";
                                        for (var j = 0; j < deep - 2; j++)
                                        {
                                            //line += string.Format("<div class='{0} {1}'></div>", "tree-tr-linearea ", GetClassByTreeMatrixNetLine(matrix.GetNetLine(j, rowIdx)));
                                            line += matrix.GetNetLineString(j, rowIndex);
                                        }
                                        value = line + PFDataHelper.ObjectToString(value);
                                        //var line = GetClassByTreeMatrixNetLine(matrix.GetNetLine(cellIndex, currentRow))
                                    }
                                    export.FillData(cellIndex, currentRow, name, value);
                                }
                                colIndex++;
                            }
                        });
                        rowIndex++;
                        currentRow++;
                    });
                }
                else
                {
                    var list = data.data as ArrayList;
                    for (var rowIndex = 0; rowIndex < list.Count; rowIndex++)
                    {
                        var rowData = list[rowIndex] as Dictionary<string, object>;

                        for (i = 0; i < rowData.Count; i++)
                        {
                            var name = rowData.ElementAt(i).Key;
                            var value = rowData.ElementAt(i).Value;

                            if (fieldIndex.ContainsKey(name))
                            {
                                foreach (int cellIndex in fieldIndex[name])
                                {
                                    if (_fieldFormatter.ContainsKey(name))
                                    {
                                        value = _fieldFormatter[name].Format(value);
                                    }
                                    export.FillData(cellIndex, currentRow, name, value);
                                }
                            }
                        }
                        currentRow++;
                    }
                }
            }
            else
            {
                //原版
                PFDataHelper.EachListRow(_data, (rowIndex, rowData) =>
                {
                    PFDataHelper.EachObjectProperty(rowData, (a, name, value) =>
                    {
                        if (fieldIndex.ContainsKey(name))
                        {
                            foreach (int cellIndex in fieldIndex[name])
                            {
                                if (_fieldFormatter.ContainsKey(name))
                                {
                                    value = _fieldFormatter[name].Format(value);
                                }
                                export.FillData(cellIndex, currentRow, name, value);
                            }
                        }
                    });
                    currentRow++;
                });
            }

            //汇总行
            bool hasSummary = false;
            i = 0;
            int firstSummary = 0;//第一个有汇总的格的位置
            string firstSummaryField = "";
            new StoreColumn { Children = _columns }.EachLeaf(a =>
            {
                //设置列宽--wxjtodo20190417
                if (!PFDataHelper.StringIsNullOrWhiteSpace(a.width))
                {
                    //export.SetColumnWidth(i, PFDataHelper.WebWidthToExcel(a.width).Value);
                    export.SetColumnWidth(i, a.width);
                }
                //if (a.excelWidth.HasValue)
                //{
                //    export.SetColumnWidth(i, double.Parse(a.width.Replace("px", "")));
                //}
                //var column = _title[_title.Count - 1][i];
                if (a.visible)
                {
                    var column = a;
                    if (!hasSummary) { firstSummary = i; firstSummaryField = column.data; }
                    if (column.summary != null)
                    {
                        hasSummary = true;
                        export.FillData(i, currentRow, column.data, column.summary);
                    }
                    i++;
                }
            });
            if (hasSummary)
            {
                export.FillData(firstSummary - 1, currentRow, firstSummaryField, "合计：");
                export.SetRowsStyle(0, currentRow, columnCount - 1, currentRow);//上面那样,当后面的列不是多表头时,边框不见了
                currentRow++;
            }

            //Foot--wxj20181011
            if (!string.IsNullOrWhiteSpace(_sheetFoot))
            {
                export.FillData(0, currentRow, firstData, _sheetFoot);

                export.SetFootStyle(0, currentRow, columnCount - 1, currentRow);

                //titleRowCount++;
                currentRow++;
            }

            _exporter = export;
            //_fileStream = export.SaveAsStream();

            _suffix = export.suffix;
            if (string.IsNullOrEmpty(_fileName))
                _fileName = DateTime.Now.ToString("yyyyMMddHHmmss");

            return this;
        }
        public IExport GetExport()//--wxj20181011
        {
            return _exporter;
        }
        //public Exporter SaveStream()//--wxj20181011
        //{
        //    _fileStream = _exporter.SaveAsStream();
        //    return this;
        //}

        //public Exporter PrintPageScheme()
        //{
        //    _fileStream = _exporter.SaveAsStream();
        //    return this;
        //}

        public Exporter Compress(string type)
        {
            var compress = GetActor<ICompress>(_compress, DEFAULT_COMPRESS, type);
            return Compress(compress);
        }

        public Exporter Compress(ICompress compress)
        {
            _fileStream = compress.Compress(_fileStream, string.Format("{0}.{1}", _fileName, _suffix));
            _suffix = compress.Suffix(_suffix);
            return this;
        }

        //private void SaveToLocal(string fileName)
        //{
        //    //测试xlsx下载后打不开的问题，暂保存到本地试试--benjamin 
        //    var path = Path.Combine(PFDataHelper.BaseDirectory, "output", fileName);
        //    var directoryName = Path.GetDirectoryName(path);
        //    PFDataHelper.DeleteFile(path);
        //    PFDataHelper.CreateDirectory(directoryName);
        //    var tmpEx = _exporter as XlsxExport;
        //    if (tmpEx != null)
        //    {
        //        tmpEx.workbook.Save(path);
        //    }
        //}
        public void Download()
        {
            var tmpEx = _exporter as XlsxExport;
            if (tmpEx != null)
            {
                PFDataHelper.DownloadExcel(HttpContext.Current, tmpEx.workbook, string.Format("{0}.{1}", _fileName, _suffix), PFDataHelper.GetConfigMapper().GetNetworkConfig().DownloadSpeed);
            }
            else
            {
                //SaveToLocal("excelPo.xlsx");

                if (_fileStream == null)
                {
                    _fileStream = _exporter.SaveAsStream();
                    //SaveToLocal("excelPoAfterSaveStream.xlsx");
                }
                if (_fileStream != null && _fileStream.Length > 0)
                {

                    //PFDataHelper.DownloadFile(HttpContext.Current, _fileStream, string.Format("{0}.{1}", _fileName, _suffix), 1024 * 1024 * 10);
                    PFDataHelper.DownloadFile(HttpContext.Current, _fileStream, string.Format("{0}.{1}", _fileName, _suffix), PFDataHelper.GetConfigMapper().GetNetworkConfig().DownloadSpeed);
                }
            }

            _exporter.Dispose();
        }

        private T GetActor<T>(Dictionary<string, Type> dict, string defaultKey, string key)
        {
            if (!dict.ContainsKey(key))
                key = defaultKey;

            return (T)Activator.CreateInstance(dict[key]);
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
