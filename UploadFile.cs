[HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            try
            {
                if (file.ContentLength > MaxFileSizeBytes)
                    throw new FileLoadException("File size overflow.", "file");

                //string base64String = null;
                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory
                        , UploadedFileFolder, file.FileName);
  
                using (var ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);

                    using (var fs = new FileStream(filePath, FileMode.Create))
                    {
                        var byteArray = ms.ToArray();
                        fs.Write(byteArray, 0, byteArray.Count());
                        fs.Close();
                    }

                    //System.IO.File.WriteAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory
                    //    , UploadedFileFolder, file.FileName)
                    //    , ms.ToArray());

                    //base64String = Convert.ToBase64String(ms.ToArray());
                }

                var arquivoViewModel = new ArquivoViewModel()
                {
                    Arquivo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, UploadedFileFolder, file.FileName),
                    UrlPath = Request.ApplicationPath + '/' + UploadedFileFolder + '/' + file.FileName,
                    NomeArquivo = file.FileName,
                    //Base64String = base64String
                };

                var jsonResult = new JsonReturn
                {
                    Data = arquivoViewModel,
                    Message = SuccessUploadMessage
                };
                return new JsonResult { Data = jsonResult };
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = new JsonReturn { IsException = true, Message = ex.Message } };
            }
        }
