using ExcelDataReader;

namespace RgSystems.Tools
{
    public static class Tools
    {
        public static string HttpPostedFileBaseToBase64String(HttpPostedFileBase file)
        {
            using (var reader = new BinaryReader(file.InputStream))
            {
                var fileBytes = reader.ReadBytes(file.ContentLength);
                return Convert.ToBase64String(fileBytes);
            }
        }
    }
}
