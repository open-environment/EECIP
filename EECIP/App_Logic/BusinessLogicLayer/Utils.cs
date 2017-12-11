using System;
using EECIP.App_Logic.DataAccessLayer;
using System.ComponentModel;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Web;
using System.Linq;
using System.Drawing.Imaging;
using System.Drawing;

namespace EECIP.App_Logic.BusinessLogicLayer
{
    public class ConfigInfoType
    {
        public string _name { get; set; }
        public string _req { get; set; }
        public int? _length { get; set; }
        public string _datatype { get; set; }
        public string _fkey { get; set; }
        public string _addfield { get; set; }
    }


    public static class Utils
    {

        /// <summary>
        ///  Generic data type converter 
        /// </summary>
        public static bool TryConvert<T>(object value, out T result)
        {
            result = default(T);
            if (value == null || value == DBNull.Value) return false;

            if (typeof(T) == value.GetType())
            {
                result = (T)value;
                return true;
            }

            string typeName = typeof(T).Name;

            try
            {
                if (typeName.IndexOf(typeof(System.Nullable).Name, StringComparison.Ordinal) > -1 ||
                    typeof(T).BaseType.Name.IndexOf(typeof(System.Enum).Name, StringComparison.Ordinal) > -1)
                {
                    TypeConverter tc = TypeDescriptor.GetConverter(typeof(T));
                    result = (T)tc.ConvertFrom(value);
                }
                else
                    result = (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return false;
            }

            return true;
        }


        /// <summary>
        ///  Converts to another datatype or returns default value
        /// </summary>
        public static T ConvertOrDefault<T>(this object value)
        {
            T result = default(T);
            TryConvert<T>(value, out result);
            return result;
        }


        public static Stream ConvertImage(this Stream originalStream, ImageFormat format)
        {
            var image = Image.FromStream(originalStream);

            var stream = new MemoryStream();
            image.Save(stream, format);
            stream.Position = 0;
            return stream;
        }


        public static byte[] ConvertGenericStreamToByteArray(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }


        /// <summary>
        ///  Better than built-in SubString by handling cases where string is too short
        /// </summary>
        public static string SubStringPlus(this string str, int index, int length)
        {
            if (index >= str.Length)
                return String.Empty;

            if (index + length > str.Length)
                return str.Substring(index);

            return str.Substring(index, length);
        }


        /// <summary>
        /// Sends out an email from the application. Returns true if successful. Supports multiple CC, BCC
        /// </summary>
        /// <param name="from">Email address of sender. Leave null to use default.</param>
        /// <param name="to">Email address sending to</param>
        /// <param name="subj">Email subject</param>
        /// <param name="body">Email body</param>
        /// <param name="attach">Attachment as byte array</param>
        /// <param name="attachFileName">Attachment file name including extension e.g. test.doc</param>
        /// <returns></returns>
        public static bool SendEmail(string from, string to, List<string> cc, List<string> bcc, string subj, string body, byte[] attach, string attachFileName, string bodyHTML = null)
        {
            try
            {
                //************* GET SMTP SERVER SETTINGS ****************************
                string mailServer = db_Ref.GetT_OE_APP_SETTING("EMAIL_SERVER");
                string Port = db_Ref.GetT_OE_APP_SETTING("EMAIL_PORT");
                string smtpUser = db_Ref.GetT_OE_APP_SETTING("EMAIL_SECURE_USER");
                string smtpUserPwd = db_Ref.GetT_OE_APP_SETTING("EMAIL_SECURE_PWD");


                //*************SET MESSAGE SENDER *********************
                if (from == null)
                    from = db_Ref.GetT_OE_APP_SETTING("EMAIL_FROM");

                //************** REROUTE TO SENDGRID HELPER IF SENDGRID ENABLED ******
                if (mailServer == "smtp.sendgrid.net")
                {
                    bool SendStatus = SendGridHelper.SendGridEmail(from, to, cc, bcc, subj, body, smtpUserPwd, bodyHTML).GetAwaiter().GetResult();
                    return SendStatus;
                }

                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
                message.From = new System.Net.Mail.MailAddress(from);
                message.To.Add(to);
                if (cc != null)
                {
                    foreach (string cc1 in cc)
                        message.CC.Add(cc1);
                }
                if (bcc != null)
                {
                    foreach (string bcc1 in bcc)
                        message.Bcc.Add(bcc1);
                }

                message.Subject = subj;
                message.Body = body;
                //*************ATTACHMENT START**************************
                if (attach != null)
                {
                    System.Net.Mail.Attachment att = new System.Net.Mail.Attachment(new MemoryStream(attach), attachFileName);
                    message.Attachments.Add(att);
                }
                //*************ATTACHMENT END****************************


                //***************SET SMTP SERVER *************************
                if (smtpUser.Length > 0)  //smtp server requires authentication
                {
                    var smtp = new System.Net.Mail.SmtpClient(mailServer, Port.ConvertOrDefault<int>())
                    {
                        Credentials = new System.Net.NetworkCredential(smtpUser, smtpUserPwd),
                        EnableSsl = true
                    };
                    smtp.Send(message);
                }
                else
                {
                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(mailServer);
                    smtp.Send(message);
                }

                return true;


            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    db_Ref.InsertT_OE_SYS_LOG("EMAIL ERR", ex.InnerException.ToString());
                else if (ex.Message != null)
                    db_Ref.InsertT_OE_SYS_LOG("EMAIL ERR", ex.Message.ToString());
                else
                    db_Ref.InsertT_OE_SYS_LOG("EMAIL ERR", "Unknown error");

                return false;
            }
        }


        //******************* DATA IMPORT HELPERS**********************************
        public static Dictionary<string, int> GetColumnMapping(string ImportType, string[] headerCols)
        {
            // Loading configuration file listing all data import columns
            var xml = XDocument.Load(HttpContext.Current.Server.MapPath("~/App_Docs/ImportColumnsConfig.xml"));

            // Query list of all columns for the type
            var allFields = (from c in xml.Root.Descendants("Alias")
                          .Where(i => i.Parent.Attribute("Level").Value == ImportType)
                             select new
                             {
                                 Name = c.Parent.Attribute("FieldName").Value,
                                 Alias = c.Value.ToUpper()
                             }).ToList();

            //list of fields supplied by user
            var headerColList = headerCols.Select((value, index) => new { value, index }).ToList();

            //return matches with index
            var colMapping = (from f in allFields
                              join h in headerColList
                              on f.Alias.Trim() equals h.value.ToUpper().Trim()
                              select new
                              {
                                  _Name = f.Name.Trim(),
                                  _Col = h.index
                              }).ToDictionary(x => x._Name, x => x._Col.ConvertOrDefault<int>());

            return colMapping;
        }

        public static List<ConfigInfoType> GetAllColumnInfo(string ImportType)
        {
            // Loading configuration file listing all data import columns
            var xml = XDocument.Load(HttpContext.Current.Server.MapPath("~/App_Docs/ImportColumnsConfig.xml"));

            // Query list of all columns for the type
            return (from c in xml.Root.Descendants("Alias")
                    .Where(i => i.Parent.Attribute("Level").Value == ImportType)
                    select new ConfigInfoType
                    {
                        _name = c.Parent.Attribute("FieldName").Value,
                        _req = c.Parent.Attribute("ReqInd").Value,
                        _length = c.Parent.Attribute("Length").Value.ConvertOrDefault<int?>(),
                        _datatype = c.Parent.Attribute("DataType").Value,
                        _fkey = c.Parent.Attribute("FKey").Value,
                        _addfield = c.Parent.Attribute("AddField") != null ? c.Parent.Attribute("AddField").Value : ""
                    }).ToList();
        }

        public static List<string> GetMandatoryImportFieldList(string ImportType)
        {
            // Loading configuration file listing all data import columns
            var xml = XDocument.Load(HttpContext.Current.Server.MapPath("~/App_Docs/ImportColumnsConfig.xml"));

            // Query list of all columns for the type
            List<string> mandFields = (from c in xml.Root.Descendants("Alias")
                                       .Where(i => i.Parent.Attribute("Level").Value == ImportType)
                                       .Where(j => j.Parent.Attribute("ReqInd").Value == "Y")
                                       select c.Parent.Attribute("FieldName").Value
                                       ).ToList();
            return mandFields;
        }

        public static List<string> GetOptionalImportFieldList(string ImportType)
        {
            // Loading configuration file listing all data import columns
            var xml = XDocument.Load(HttpContext.Current.Server.MapPath("~/App_Docs/ImportColumnsConfig.xml"));

            // Query list of all columns for the type
            List<string> optFields = (from c in xml.Root.Descendants("Alias")
                                       .Where(i => i.Parent.Attribute("Level").Value == ImportType)
                                       .Where(j => j.Parent.Attribute("ReqInd").Value == "N")
                                       select c.Parent.Attribute("FieldName").Value
                                       ).ToList();
            return optFields;
        }

        public static V GetValueOrDefault<K, V>(this Dictionary<K, V> dict, K key)
        {
            V ret;
            dict.TryGetValue(key, out ret);
            return ret;
        }


    }
}