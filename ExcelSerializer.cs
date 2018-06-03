using ExcelDataReader;
using System;
using System.Data;

namespace RgSystems.Tools
{
    internal static class ExcelSerializer
    {
        internal static IEnumerable<T> SerializeExcelWorksheet<T>(HttpPostedFileBase file)
        {
            const string ExcelLegacyFileExtension = "xls";
            const string ExcelFileExtension = "xlsx";

            IExcelDataReader reader = null;
            var fileExtension = Path.GetExtension(file.FileName).Remove(0, 1).ToLower();
            if (fileExtension == ExcelLegacyFileExtension
                || fileExtension == ExcelFileExtension)
                reader = ExcelReaderFactory.CreateReader(file.InputStream);
            else
                reader = ExcelReaderFactory.CreateCsvReader(file.InputStream);

            var conf = new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration
                {
                    UseHeaderRow = true
                }
            };

            var collection = new List<T>();

            using (var dt = reader.AsDataSet(conf).Tables[0])
            {
                //Formatting column names
                System.Data.DataColumn[] dtColumnArray = new System.Data.DataColumn[dt.Columns.Count];
                dt.Columns.CopyTo(dtColumnArray, 0);
                var dataColumCollection = dtColumnArray.ToList();
                dtColumnArray = null;

                var propertyCollection = ((T)Activator.CreateInstance(typeof(T), null)).GetType().GetProperties().ToList();
                dataColumCollection.RemoveAll(dc => !propertyCollection.Any(prop => prop.Name.ToLower() == dc.ColumnName.Trim().ToLower()));

                if (dataColumCollection.Count < 1)
                    return collection;

                foreach (var dc in dataColumCollection)
                    for (var i = propertyCollection.Count - 1; i >= 0; i--)
                        if (propertyCollection[i].Name.ToLower() == dc.ColumnName.ToLower())
                        {
                            dc.ColumnName = propertyCollection[i].Name;
                            propertyCollection.RemoveAt(i);
                            break;
                        }

                //Creating model and populating collection
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    var model = (T)Activator.CreateInstance(typeof(T), null);
                    foreach (var dc in dataColumCollection)
                    {
                        var propertyInfo = model.GetType().GetProperty(dc.ColumnName);
                        var converter = System.ComponentModel.TypeDescriptor.GetConverter(propertyInfo.PropertyType);
                        var value = converter.ConvertFrom(dr[dc.ColumnName].ToString());

                        if (propertyInfo.PropertyType == typeof(string)
                            && string.Empty.Equals((string)value))
                            value = null;

                        propertyInfo.SetValue(model, value);
                    }

                    collection.Add(model);
                }
            }

            return collection;
        }
    }
}
