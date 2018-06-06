[HttpPost]
        public ActionResult DeleteFile(string filePath)
        {
            try
            {
                if (System.IO.File.Exists(filePath))
                    try
                    {
                        System.IO.File.Delete(filePath);
                    }
                    catch { };

                var jsonResult = new JsonReturn
                {
                    Message = SuccessDeleteMessage
                };
                return new JsonResult { Data = jsonResult };
            }
            catch (Exception ex)
            {

                return new JsonResult { Data = new JsonReturn { IsException = true, Message = ex.Message } };
            }
        }
