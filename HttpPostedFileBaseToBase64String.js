using ExcelDataReader;

namespace RgSystems.Tools
{
    internal static class Tools
    {
        internal static string HttpPostedFileBaseToBase64String(HttpPostedFileBase file)
        {
            using (var reader = new BinaryReader(file.InputStream))
            {
                var fileBytes = reader.ReadBytes(file.ContentLength);
                return Convert.ToBase64String(fileBytes);
            }
        }
    }
}
