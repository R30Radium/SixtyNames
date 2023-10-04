using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace SixtyNames
{
    /// <summary>
    /// Класс для работы и формирования Excel таблиц 
    /// </summary>
    class ExcelHelper : IDisposable
    {
        #region Constructor

        public ExcelHelper()
        {
            _excel = new Excel.Application();
        }

        #endregion

        #region Fields

        private Application _excel;
        private Workbook _workBook;
        private string _filePath;

        #endregion

        #region Methods

        public (bool isSuccess, Exception exception) Open(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    _workBook = _excel.Workbooks.Open(filePath);
                }
                else
                {
                    _workBook = _excel.Workbooks.Add();
                    _filePath = filePath;
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex);
            }
        }

        public void Dispose()
        {
            _workBook.Close();
        }

        public bool Set(string column, int row, string date)
        {
            try
            {
                ((Excel.Worksheet)_excel.ActiveSheet).Cells[row, column] = date;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Save()
        {
            try
            {
                if (!string.IsNullOrEmpty(_filePath))
                {
                    _workBook.SaveAs(_filePath);
                    _filePath = null;
                }
                else
                {
                    _workBook.Save();
                }

                return true;

            }
            catch
            {
                return false;
            }
        }

        #endregion


    }
}
