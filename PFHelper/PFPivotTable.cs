using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perfect
{
    public class PFPivotTable
    {
        private DataTable _dt;
        private List<string> _pivotLeft = new List<string>();//left必需有值(其实也可以为空,显示结果为1行)
        private List<string> _pivotTop = new List<string>();//top可以为空,因为最下一层是value
        private List<string> _pivotValue = new List<string>();
        public PFPivotTable(DataTable dt)
        {
            _dt = dt;
        }
        public PFPivotTable(DataTable dt, List<string> left, List<string> top)
        {
            _pivotLeft = left;
            _pivotTop = top;
            _dt = dt;
            GroupDataTable();
        }
        public PFPivotTable SetLeft(params string[] left)
        {
            _pivotLeft = left.ToList();
            return this;
        }
        public PFPivotTable SetTop(params string[] top)
        {
            _pivotTop = top.ToList();
            return this;
        }
        public PFPivotTable SetValue(params string[] value)
        {
            _pivotValue = value.ToList();
            return this;
        }
        /// <summary>
        /// 其实最后用到的只是汇总后的_dt,如果原dt很大的话,这样可以节省性能
        /// </summary>
        public PFPivotTable GroupDataTable()
        {
            _dt = PFDataHelper.DataTableGroupBy(_dt,
                PFDataHelper.MergeList(new List<string>(_pivotLeft), _pivotTop).ToArray(),
                new PFKeyValueCollection<SummaryType>(_pivotValue.Select(a => new PFKeyValue<SummaryType> { Key = a, Value = SummaryType.Sum }))
                );
            return this;
        }
        [Obsolete]
        public void SaveToFileTest(string path)
        {

            var fileName = Path.GetFileName(path);

            //var dt = _dt;

            var dt = PFDataHelper.DataTableGroupBy(_dt,
                PFDataHelper.MergeList(_pivotLeft, _pivotTop).ToArray(),
                new PFKeyValueCollection<SummaryType>(_pivotValue.Select(a => new PFKeyValue<SummaryType> { Key = a, Value = SummaryType.Sum }))
                );

            //var dt = PFDataHelper.DataTableGroupBy(_dt,
            //    _pivotTop.ToArray(),
            //    new PFKeyValueCollection<SummaryType>(_pivotTop.Select(a => new PFKeyValue<SummaryType> { Key = a, Value = SummaryType.Sum }))
            //    );

            StoreColumnCollection columns = null;

            var pagingResult = PFDataHelper.PagingStore(dt, new PagingParameters { },
                columns,
                false, null);
            var exporter = Exporter.Instance(pagingResult ?? new PagingResult(), new ExporterOption
            {
                FileType = "xlsx",//benjamin todo
                Scheme = Exporter.FinancialScheme
                ,
                SheetTitle = fileName
                //,
                //SheetTitle = GetWordCMonth(cmonthff) + hr + fgsname
            }).FileName("总表");//这里的下载名没用到
            var export = (exporter.GetExport() as XlsxExport);

            //var path = Path.Combine(PFDataHelper.BaseDirectory, "output", "excelPo.xlsx");
            var directoryName = Path.GetDirectoryName(path);
            PFDataHelper.DeleteFile(path);
            PFDataHelper.CreateDirectory(directoryName);
            export.workbook.Save(path);
        }
        public Workbook SaveToExcel(string path)
        {
            var scheme = new PrintPageScheme
            {
                TopMargin = 0.4,
                RightMargin = 0.3,
                BottomMargin = 0.4,
                LeftMargin = 0.3,

                DataRowHeight = 13.5,
                DataFontSize = 9,

                HeadRowHeight = 13.5,
                HeadForegroundColor = System.Drawing.Color.LightBlue, //默认是rgb(0,0,0),但显示为无色，原因未明
                HeadFontSize = 9,

                TitleRowHeight = 25.5,//20,
                TitleFontSize = 16,
                TitleFontIsBold = true,
                TitleHorizontalAlignment = PFTextAlignmentType.Center,

                FootRowHeight = 36.75,
                FootFontSize = 11
            };
            var leftHeadScheme = new PrintPageScheme
            {
                TopMargin = 0.4,
                RightMargin = 0.3,
                BottomMargin = 0.4,
                LeftMargin = 0.3,

                DataRowHeight = 13.5,
                DataFontSize = 9,

                HeadRowHeight = 13.5,
                HeadForegroundColor = System.Drawing.Color.LightGreen, //默认是rgb(0,0,0),但显示为无色，原因未明
                HeadFontSize = 9,

                TitleRowHeight = 25.5,//20,
                TitleFontSize = 16,
                TitleFontIsBold = true,
                TitleHorizontalAlignment = PFTextAlignmentType.Center,

                FootRowHeight = 36.75,
                FootFontSize = 11
            };

            var fileName = Path.GetFileName(path);
            var _sheetTitle = Path.GetFileNameWithoutExtension(path);

            IExport export = new XlsxExport();
            export.Init(null, scheme);// Exporter.FinancialScheme);

            Dictionary<int, int> currentHeadRow = new Dictionary<int, int>();
            Dictionary<int, int> currentHeadCell = new Dictionary<int, int>();
            Dictionary<string, List<int>> fieldIndex = new Dictionary<string, List<int>>();
            Dictionary<string, List<int>> fieldIndexLeft = new Dictionary<string, List<int>>();

            int titleRowCount = 0;
            int titleCellCount = 0;
            if (!string.IsNullOrWhiteSpace(_sheetTitle))
            {
                titleRowCount++;
            }
            Func<int, int> GetCurrentHeadRow = cell => currentHeadRow.ContainsKey(cell) ? currentHeadRow[cell] : titleRowCount;
            Func<int, int> GetCurrentHeadCell = row => currentHeadCell.ContainsKey(row) ? currentHeadCell[row] : titleCellCount;
            var currentRow = 0;
            var currentCell = 0;


            //提前计算leftTitle是为了可以知道top要向右移多少格
            var columns = GetTreeTop();

            var leftTitle = new List<List<StoreColumn>>();
            var leftColumns = GetTreeLeft();
            leftColumns.ForEach(a =>
            {
                if (a.Children.Any() && a.GetAllLeafCount(b => b.visible) < 1)//如果不是叶节点且所有子叶节点都隐藏,那么本父节点也隐藏
                {
                    a.visible = false;
                }
            });
            var maxDepth = new StoreColumn { Children = leftColumns }.GetDepth() - 1;
            var topBeginCell = maxDepth;
            StoreColumnCollection.StoreColumnTo2DArray(ref leftTitle, leftColumns, ref maxDepth);
            //标题--wxj20181011

            StoreColumn temp = new StoreColumn { Children = columns };
            int columnCount = temp.GetAllLeafCount(a => a.visible);
            var firstData = temp.FirstLeaf(a => true).data;
            if (!string.IsNullOrWhiteSpace(_sheetTitle))
            {
                export.FillData(0, 0, firstData, _sheetTitle);

                //export.SetTitleStyle(0, 0, columnCount - 1+ leftTitle.Count, 0);
                export.SetTitleStyle(0, 0, columnCount - 1 + (HasLeft() ? leftTitle.Count : 0), 0);
                currentRow++;
            }
            #region 生成top 3秒(后来发现只有调试时特别慢,原因未明)
            ////生成多行题头
            var title = new List<List<StoreColumn>>();
            columns.ForEach(a =>
            {
                if (a.Children.Any() && a.GetAllLeafCount(b => b.visible) < 1)//如果不是叶节点且所有子叶节点都隐藏,那么本父节点也隐藏
                {
                    a.visible = false;
                }
            });
            maxDepth = new StoreColumn { Children = columns }.GetDepth() - 1;
            StoreColumnCollection.StoreColumnTo2DArray(ref title, columns, ref maxDepth);


            var _title = title;
            //var topBeginCell = leftTitle.Count;
            for (var i = 0; i < _title.Count; i++)
            {
                currentCell = topBeginCell;

                for (var j = 0; j < _title[i].Count; j++)
                {
                    var item = _title[i][j];
                    if (item.visible == false) { continue; }//隐藏列不导出--wxj20181009

                    while (currentRow < GetCurrentHeadRow(currentCell))
                        currentCell++;

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

            //设置题头样式
            //export.SetHeadStyle(0, 0, currentCell - 1, currentRow - 1);
            //export.SetHeadStyle(0, titleRowCount, currentHeadRow.Count - 1, currentRow - 1);//上面那样,当后面的列不是多表头时,背景色只填到最后一个多表头为止
            export.SetHeadStyle(topBeginCell, titleRowCount, currentHeadRow.Count + topBeginCell - 1, currentRow - 1);

            #region 生成left
            ////生成left
            var _leftTitle = leftTitle;
            var leftBeginRow = currentRow;
            var dataBeginRow = currentRow;

            currentCell = 0;
            try
            {
                for (var i = 0; i < _leftTitle.Count; i++)
                {
                    currentRow = leftBeginRow;

                    if (currentRow == 7)
                    {
                        var aa = "aa";
                    }
                    for (var j = 0; j < _leftTitle[i].Count; j++)
                    {
                        var item = _leftTitle[i][j];
                        if (item.visible == false) { continue; }//隐藏列不导出--wxj20181009

                        while (currentCell < GetCurrentHeadCell(currentRow))
                        {
                            currentRow++;
                        }

                        export.FillData(currentCell, currentRow, "leftTitle_" + item.data, item.title ?? item.data);//e:\svn\businesssys2018\yjquery.web\areas\bonus\views\reportquery05\treegrid.cshtml里的leftTitle是null

                        if (item.rowspan + item.colspan > 2)
                        {
                            //export.MergeCell(currentCell, currentRow, currentCell + item.colspan - 1, currentRow + item.rowspan - 1);
                            export.MergeCell(currentCell, currentRow, currentCell + item.rowspan - 1, currentRow + item.colspan - 1);//这里应该要改(和top比是应该反过来)--benjamin todo
                        }

                        if (!string.IsNullOrEmpty(item.data))
                        {
                            if (!fieldIndexLeft.ContainsKey(item.data))
                            {
                                fieldIndexLeft[item.data] = new List<int>();
                            }
                            fieldIndexLeft[item.data].Add(currentRow);
                        }

                        for (var k = 0; k < item.colspan; k++)
                        {
                            currentHeadCell[currentRow] = GetCurrentHeadCell(currentRow++) + item.rowspan;
                        }
                        //resultCount++;
                    }
                    currentCell++;
                }
            }
            catch (Exception e)
            {
                throw e;
                //var aa = "a";
            }
            #endregion

            //设置left题头样式
            //export.SetHeadStyle(0, 0, currentCell - 1, currentRow - 1);
            //export.SetHeadStyle(0, titleRowCount, currentHeadRow.Count - 1, currentRow - 1);//上面那样,当后面的列不是多表头时,背景色只填到最后一个多表头为止
            //export.SetHeadStyle(topBeginCell, titleRowCount, currentHeadRow.Count + topBeginCell - 1, currentRow - 1);
            (export as XlsxExport).SetHeadStyle(0, leftBeginRow, currentCell - 1, currentHeadCell.Count + leftBeginRow - 1, leftHeadScheme);

            #region 填充数据

            var sColumn = new StoreColumn { Children = columns };
            var sLeftColumn = new StoreColumn { Children = leftColumns };
            var x = topBeginCell;

            //var dataXCount = _pivotTop.Count;
            var dataYCount = HasLeft() ? _leftTitle[_leftTitle.Count - 1].Count : 1;//就算没有左,也要有一行
            _title[_title.Count - 1].ForEach(c =>
            {
                for (int y = 0; y < dataYCount; y++)
                {
                    var r = HasLeft() ? _leftTitle[_leftTitle.Count - 1][y] : new StoreColumn();

                    //var xGroupValues = new List<string>();
                    //var yGroupValues = new List<string>();
                    //try
                    //{
                    //    for (int m = 0; m < _pivotTop.Count; m++)
                    //    {
                    //        xGroupValues.Add(_title[m][x - topBeginCell].title.ToString());//这样有问题的,_title并不是每行的第二维长度都一样--benjamin todo
                    //    }
                    //    for (int m = 0; m < _pivotLeft.Count; m++)
                    //    {
                    //        yGroupValues.Add(_leftTitle[m][y].title.ToString());
                    //    }

                    //    //xGroupValues.Add(_title[0][x - topBeginCell].title.ToString());//data??title--benjamin 
                    //    //yGroupValues.Add(_leftTitle[0][y- leftBeginRow].title.ToString());
                    //}
                    //catch (Exception e)
                    //{
                    //    throw e;
                    //    //var a = "a";
                    //}

                    var xGroupValues = c.data.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    var yGroupValues = HasLeft() ? r.data.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries).ToList() : null;
                    var valueField = xGroupValues[xGroupValues.Count - 1];
                    xGroupValues.Remove(valueField);

                    var whereList = new List<string>();
                    for (int m = 0; m < _pivotTop.Count; m++)
                    {
                        whereList.Add(string.Format("{0}='{1}'", _pivotTop[m], xGroupValues[m]));
                    }
                    if (yGroupValues != null)
                    {
                        for (int m = 0; m < _pivotLeft.Count; m++)
                        {
                            try
                            {
                                whereList.Add(string.Format("{0}='{1}'", _pivotLeft[m], yGroupValues[m]));
                            }
                            catch (Exception e)
                            {
                                throw e;
                                //var aa = e;
                            }
                        }
                    }
                    var valueRows = _dt.Select(string.Join(" and ", whereList));
                    if (valueRows != null && valueRows.Length > 0)
                    {
                        //export.FillData(x, y+ leftBeginRow, r.data + "_" + c.data, valueRows[0][c.data]);
                        export.FillData(x, y + leftBeginRow, r.data + "_" + c.data, valueRows[0][valueField]);
                    }
                }
                //var y = leftBeginRow;
                //_leftTitle[_leftTitle.Count - 1].ForEach(r =>
                //{
                //    var xGroupValues = new List<string>();
                //    var yGroupValues = new List<string>();
                //    try
                //    {
                //        for (int m = 0; m < _pivotTop.Count; m++)
                //        {
                //            xGroupValues.Add(_title[m][x - topBeginCell].title.ToString());
                //        }
                //        for (int m = 0; m < _pivotLeft.Count; m++)
                //        {
                //            yGroupValues.Add(_leftTitle[m][y - leftBeginRow].title.ToString());
                //        }

                //        //xGroupValues.Add(_title[0][x - topBeginCell].title.ToString());//data??title--benjamin 
                //        //yGroupValues.Add(_leftTitle[0][y- leftBeginRow].title.ToString());
                //    }catch(Exception e)
                //    {
                //        throw e;
                //        //var a = "a";
                //    }

                //    //var xGroupFields = c.data.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                //    ////var yGroupFields = r.data.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                //    //var valueField = xGroupFields[xGroupFields.Count - 1];
                //    //xGroupFields.Remove(valueField);

                //    var whereList =new List<string>();
                //    for(int m = 0; m < _pivotTop.Count; m++)
                //    {
                //        whereList.Add(string.Format("{0}='{1}'", _pivotTop[m], xGroupValues[m]));
                //    }
                //    for (int m = 0; m < _pivotLeft.Count; m++)
                //    {
                //        whereList.Add(string.Format("{0}='{1}'", _pivotLeft[m], yGroupValues[m]));
                //    }
                //    var valueRows = _dt.Select(string.Join(" and ", whereList));
                //    if (valueRows != null && valueRows.Length > 0)
                //    {
                //        export.FillData(x, y, r.data + "_" + c.data, valueRows[0][c.data]);
                //    }
                //    //var value = _dt.Select(string.Join(" and ", whereList))[0][c.data];

                //    ////var value =_dt.Select(string.Format("{0}='{1}'",_pivotTop[0], xGroupValues[0]))[0][c.data];

                //    ////var value = PFDataHelper.DataTableGroupBy(
                //    ////    _dt,
                //    ////    PFDataHelper.MergeList(xGroupFields, yGroupFields).ToArray(),
                //    ////    new PFKeyValueCollection<SummaryType> { { valueField, SummaryType.Sum } }
                //    ////    );
                //    //export.FillData(x, y, r.data + "_" + c.data, value);
                //    y++;
                //});
                x++;
            });


            #endregion

            export.SetRowsStyle(topBeginCell, leftBeginRow, currentHeadRow.Count + topBeginCell - 1, HasLeft() ? (currentHeadCell.Count + leftBeginRow - 1) : leftBeginRow);

            var tmpEx = export as XlsxExport;
            if (tmpEx != null)
            {
                return tmpEx.workbook;
                //tmpEx.workbook.Save(path);
            }
            return null;
        }
        public void SaveToFile(string path)
        {
            SaveToExcel(path).Save(path);
        }

        /// <summary>
        /// 根据dt,获得列头的结构
        /// </summary>
        private StoreColumnCollection GetTreeTop()
        {
            if (_pivotTop == null) { return null; }

            var dt = _dt;
            var rowList = dt.Rows.Cast<DataRow>().ToList();

            var result = DoGetTree(_pivotTop, rowList);
            if (result != null && result.Any())
            {
                new StoreColumn { Children = result }.EachLeaf(a =>
                {
                    a.Children = _pivotValue.Select(b => new StoreColumn(b)).ToList();
                    a.Children.ForEach(b =>
                    {
                        b.data = a.data + "_" + b.data;
                    });
                });
            }
            else//当没有top时
            {
                result = new StoreColumnCollection();
                result.AddRange(_pivotValue.Select(b => new StoreColumn(b)).ToList());
            }
            return result;
        }
        private StoreColumnCollection GetTreeLeft()
        {
            if (_pivotLeft == null) { return null; }

            var dt = _dt;
            var rowList = dt.Rows.Cast<DataRow>().ToList();

            return DoGetTree(_pivotLeft, rowList);
        }
        private StoreColumnCollection DoGetTree(List<string> pivot, List<DataRow> rowList, StoreColumn parent = null)
        {
            //if (pivot == null) { return null; }
            if (pivot == null || pivot.Count == 0) { return new StoreColumnCollection(); }

            var dt = _dt;
            //var rowList = dt.Rows.Cast<DataRow>().ToList();
            var result = new StoreColumnCollection();
            List<IGrouping<string, DataRow>> group = rowList
                .GroupBy<DataRow, string>(dr =>
                {
                    var g = "";
                    g += (dr[pivot[0]] ?? "").ToString();
                    return g;
                }).ToList();//按A分组  
            List<string> next = null;
            if (pivot.Count > 1)
            {
                next = new List<string>(pivot);
                next.RemoveAt(0);
            }
            foreach (IGrouping<string, DataRow> ig in group)
            {
                //var item = new StoreColumn { };
                result.Add(ig.Key, a =>
                {
                    if (parent != null) { a.data = parent.data + "_" + a.data; }
                    if (next != null)
                    {
                        a.Children = DoGetTree(next, ig.ToList(), a);
                        //a.Children.ForEach(b =>//这样有问题,因为3层+=2层,然后2层+=1层,这样的话不能连起来--benjamin todo
                        //{
                        //    b.data = a.data + "_" + b.data;
                        //});
                    }
                });
            }
            return result;
        }
        private bool HasTop()
        {
            return _pivotTop != null && _pivotTop.Count > 0;
        }
        private bool HasLeft()
        {
            return _pivotLeft != null && _pivotLeft.Count > 0;
        }
    }
}
