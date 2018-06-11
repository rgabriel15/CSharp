using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RgSystems.Tools
{
    internal class ExcelSerializer
    {
        #region Properties
        internal string DateTimeFormat { get; set; } = null;
        internal string TimeSpanFormat { get; set; } = null;
        #endregion

        #region Functions
        internal IEnumerable<T> SerializeExcelWorksheet<T>(HttpPostedFileBase file)
        {
            const string ExcelLegacyFileExtension = "xls";
            const string ExcelFileExtension = "xlsx";

            IExcelDataReader reader = null;
            var fileExtension = System.IO.Path.GetExtension(file.FileName).Remove(0, 1).ToLower();
            if (fileExtension == ExcelLegacyFileExtension
                || fileExtension == ExcelFileExtension)
			{
                reader = ExcelReaderFactory.CreateReader(file.InputStream);
			}
            else
			{
                reader = ExcelReaderFactory.CreateCsvReader(file.InputStream);
			}

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
                var dataColumnCollection = dtColumnArray.ToList();
                dtColumnArray = null;
                var propertyCollection = ((T)Activator.CreateInstance(typeof(T), null)).GetType().GetProperties().ToList();
                dataColumnCollection.RemoveAll(dc => !propertyCollection.Any(prop => prop.Name.ToLower() == dc.ColumnName.Trim().ToLower()));

                if (dataColumnCollection.Count < 1)
					return collection;

				Func<string, DateTime> DateTimeParseFunc = null;
				if (string.IsNullOrWhiteSpace(DateTimeFormat))
					DateTimeParseFunc = (value) => return DateTime.Parse(value);
				else
					DateTimeParseFunc = (value) => return DateTime.ParseExact(value, DateTimeFormat, System.Globalization.CultureInfo.InvariantCulture);
				
				Func<string, TimeSpan> TimeSpanParseFunc = null;
				if (string.IsNullOrWhiteSpace(TimeSpanFormat))
					TimeSpanParseFunc = (value) => return TimeSpan.Parse(value);
				else
					TimeSpanParseFunc = (value) => return TimeSpan.ParseExact(value, TimeSpanFormat, System.Globalization.CultureInfo.InvariantCulture);
				
                foreach (var dc in dataColumnCollection)
				{
                    for (var i = propertyCollection.Count - 1; i >= 0; i--)
					{
                        if (propertyCollection[i].Name.ToLower() == dc.ColumnName.ToLower())
                        {
                            dc.ColumnName = propertyCollection[i].Name;
                            propertyCollection.RemoveAt(i);
                            break;
                        }
					}
				}

                //Creating models and populating collection
                foreach (DataRow dr in dt.Rows)
                {
                    var model = (T)Activator.CreateInstance(typeof(T), null);
                    foreach (var dc in dataColumnCollection)
                    {
                        var propertyInfo = model.GetType().GetProperty(dc.ColumnName);
                        
                        var valueString = dr[dc.ColumnName].ToString();
                        valueString = string.Empty.Equals(valueString) ? null : valueString;
                        object value = null;

                        if (string.Empty.Equals(valueString))
						{
                            value = null;
						}
                        else
                        {
                            if (propertyInfo.PropertyType == typeof(DateTime)
                                || propertyInfo.PropertyType == typeof(DateTime?))
                            {
                                value = DateTimeParseFunc(valueString);
                            }
                            else if (propertyInfo.PropertyType == typeof(TimeSpan)
                                || propertyInfo.PropertyType == typeof(TimeSpan?))
                            {
                                value = TimeSpanParseFunc(valueString);
                            }
                            else
							{
								var converter = System.ComponentModel.TypeDescriptor.GetConverter(propertyInfo.PropertyType);
                                value = converter.ConvertFrom(valueString);
							}
                        }

                        propertyInfo.SetValue(model, value);
                    }

                    collection.Add(model);
                }
            }

            return collection;
        }
        #endregion
    }
}
