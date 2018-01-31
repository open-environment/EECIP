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
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;

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
        #region TYPE CONVERSION

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


        private static string GetDayNumberSuffix(DateTime date)
        {
            switch (date.Day)
            {
                case 1:
                case 0x15:
                case 0x1f:
                    return @"\s\t";

                case 2:
                case 0x16:
                    return @"\n\d";

                case 3:
                case 0x17:
                    return @"\r\d";
            }
            return @"\t\h";
        }

        public static string FormatDateTime(string date, string format)
        {
            DateTime time;
            if (DateTime.TryParse(date, out time) && !string.IsNullOrEmpty(format))
            {
                format = Regex.Replace(format, @"(?<!\\)((\\\\)*)(S)", "$1" + GetDayNumberSuffix(time));
                return time.ToString(format);
            }
            return string.Empty;
        }


        /// <summary>
        /// Returns a pretty date like Facebook
        /// </summary>
        /// <param name="date"></param>
        /// <returns>28 Days Ago</returns>
        public static string GetPrettyDate(string date)
        {
            DateTime time;
            if (DateTime.TryParse(date, out time))
            {
                var span = DateTime.UtcNow.Subtract(time);
                var totalDays = (int)span.TotalDays;
                var totalSeconds = (int)span.TotalSeconds;
                if ((totalDays < 0) || (totalDays >= 0x1f))
                {
                    return FormatDateTime(date, "dd MMMM yyyy");
                }
                if (totalDays == 0)
                {
                    if (totalSeconds < 60)
                        return "Just now";
                    if (totalSeconds < 120)
                        return "1 minute ago";
                    if (totalSeconds < 0xe10)
                        return string.Format("{0} minutes ago", Math.Floor((double)(((double)totalSeconds) / 60.0)));
                    if (totalSeconds < 0x1c20)
                        return "1 hour ago";
                    if (totalSeconds < 0x15180)
                        return string.Format("{0} hours ago", Math.Floor((double)(((double)totalSeconds) / 3600.0)));
                }
                if (totalDays == 1)
                    return "yesterday";
                if (totalDays < 7)
                    return string.Format("{0} days ago", totalDays);
                if (totalDays < 0x1f)
                    return string.Format("{0} weeks ago", Math.Ceiling((double)(((double)totalDays) / 7.0)));
            }
            return date;
        }

        #endregion


        #region EMAIL HELPERS

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

        #endregion


        #region STRING CONTENT HELPERS

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
        /// Strips all non alpha/numeric charators from a string
        /// </summary>
        public static string StripNonAlphaNumeric(string strInput, string replaceWith)
        {
            strInput = Regex.Replace(strInput, "[^\\w]", replaceWith);
            strInput = strInput.Replace(string.Concat(replaceWith, replaceWith, replaceWith), replaceWith)
                                .Replace(string.Concat(replaceWith, replaceWith), replaceWith)
                                .TrimStart(Convert.ToChar(replaceWith))
                                .TrimEnd(Convert.ToChar(replaceWith));
            return strInput;
        }

        /// <summary>
        /// Used to pass all string input in the system  - Strips all nasties from a string/html
        /// </summary>
        public static string GetSafeHtml(string html, bool useXssSantiser = false)
        {
            // Scrub html
            html = ScrubHtml(html, useXssSantiser);

            // remove unwanted html
            html = RemoveUnwantedTags(html);

            return html;
        }

        /// <summary>
        /// Takes in HTML and returns santized Html/string
        /// </summary>
        /// <param name="html"></param>
        /// <param name="useXssSantiser"></param>
        /// <returns></returns>
        public static string ScrubHtml(string html, bool useXssSantiser = false)
        {
            if (string.IsNullOrEmpty(html))
            {
                return html;
            }

            // clear the flags on P so unclosed elements in P will be auto closed.
            HtmlNode.ElementsFlags.Remove("p");

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var finishedHtml = html;

            // Embed Urls
            if (doc.DocumentNode != null)
            {
                // Get all the links we are going to 
                var tags = doc.DocumentNode.SelectNodes("//a[contains(@href, 'youtube.com')]|//a[contains(@href, 'youtu.be')]|//a[contains(@href, 'vimeo.com')]|//a[contains(@href, 'screenr.com')]|//a[contains(@href, 'instagram.com')]");

                if (tags != null)
                {
                    // find formatting tags
                    foreach (var item in tags)
                    {
                        if (item.PreviousSibling == null)
                        {
                            // Prepend children to parent node in reverse order
                            foreach (var node in item.ChildNodes.Reverse())
                            {
                                item.ParentNode.PrependChild(node);
                            }
                        }
                        else
                        {
                            // Insert children after previous sibling
                            foreach (var node in item.ChildNodes)
                            {
                                item.ParentNode.InsertAfter(node, item.PreviousSibling);
                            }
                        }

                        // remove from tree
                        item.Remove();
                    }
                }


                //Remove potentially harmful elements
                var nc = doc.DocumentNode.SelectNodes("//script|//link|//iframe|//frameset|//frame|//applet|//object|//embed");
                if (nc != null)
                {
                    foreach (var node in nc)
                    {
                        node.ParentNode.RemoveChild(node, false);

                    }
                }

                //remove hrefs to java/j/vbscript URLs
                nc = doc.DocumentNode.SelectNodes("//a[starts-with(translate(@href, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'javascript')]|//a[starts-with(translate(@href, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'jscript')]|//a[starts-with(translate(@href, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'vbscript')]");
                if (nc != null)
                {

                    foreach (var node in nc)
                    {
                        node.SetAttributeValue("href", "#");
                    }
                }

                //remove img with refs to java/j/vbscript URLs
                nc = doc.DocumentNode.SelectNodes("//img[starts-with(translate(@src, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'javascript')]|//img[starts-with(translate(@src, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'jscript')]|//img[starts-with(translate(@src, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'vbscript')]");
                if (nc != null)
                {
                    foreach (var node in nc)
                    {
                        node.SetAttributeValue("src", "#");
                    }
                }

                //remove on<Event> handlers from all tags
                nc = doc.DocumentNode.SelectNodes("//*[@onclick or @onmouseover or @onfocus or @onblur or @onmouseout or @ondblclick or @onload or @onunload or @onerror]");
                if (nc != null)
                {
                    foreach (var node in nc)
                    {
                        node.Attributes.Remove("onFocus");
                        node.Attributes.Remove("onBlur");
                        node.Attributes.Remove("onClick");
                        node.Attributes.Remove("onMouseOver");
                        node.Attributes.Remove("onMouseOut");
                        node.Attributes.Remove("onDblClick");
                        node.Attributes.Remove("onLoad");
                        node.Attributes.Remove("onUnload");
                        node.Attributes.Remove("onError");
                    }
                }

                // remove any style attributes that contain the word expression (IE evaluates this as script)
                nc = doc.DocumentNode.SelectNodes("//*[contains(translate(@style, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'expression')]");
                if (nc != null)
                {
                    foreach (var node in nc)
                    {
                        node.Attributes.Remove("stYle");
                    }
                }

                // build a list of nodes ordered by stream position
                var pos = new NodePositions(doc);

                // browse all tags detected as not opened
                foreach (var error in doc.ParseErrors.Where(e => e.Code == HtmlParseErrorCode.TagNotOpened))
                {
                    // find the text node just before this error
                    var last = pos.Nodes.OfType<HtmlTextNode>().LastOrDefault(n => n.StreamPosition < error.StreamPosition);
                    if (last != null)
                    {
                        // fix the text; reintroduce the broken tag
                        last.Text = error.SourceText.Replace("/", "") + last.Text + error.SourceText;
                    }
                }

                finishedHtml = doc.DocumentNode.WriteTo();
            }


            return finishedHtml;
        }

        public static string RemoveUnwantedTags(string html)
        {

            var unwantedTagNames = new List<string>
            {
                "div",
                "font",
                "table",
                "tbody",
                "tr",
                "td",
                "th",
                "thead"
            };

            return RemoveUnwantedTags(html, unwantedTagNames);
        }

        public static string RemoveUnwantedTags(string html, List<string> unwantedTagNames)
        {
            if (string.IsNullOrEmpty(html))
            {
                return html;
            }

            var htmlDoc = new HtmlDocument();

            // load html
            htmlDoc.LoadHtml(html);

            var tags = (from tag in htmlDoc.DocumentNode.Descendants()
                        where unwantedTagNames.Contains(tag.Name)
                        select tag).Reverse();


            // find formatting tags
            foreach (var item in tags)
            {
                if (item.PreviousSibling == null)
                {
                    // Prepend children to parent node in reverse order
                    foreach (var node in item.ChildNodes.Reverse())
                    {
                        item.ParentNode.PrependChild(node);
                    }
                }
                else
                {
                    // Insert children after previous sibling
                    foreach (var node in item.ChildNodes)
                    {
                        item.ParentNode.InsertAfter(node, item.PreviousSibling);
                    }
                }

                // remove from tree
                item.Remove();
            }

            // return transformed doc
            return htmlDoc.DocumentNode.WriteContentTo().Trim();
        }

        /// <summary>
        /// Uses regex to strip HTML from a string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string StripHtmlFromString(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                input = HttpUtility.HtmlDecode(input);
                input = Regex.Replace(input, @"</?\w+((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?>", string.Empty, RegexOptions.Singleline);
                input = Regex.Replace(input, @"\[[^]]+\]", "");
            }
            return input;
        }

        /// <summary>
        /// Returns safe plain text using XSS library
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string SafePlainText(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                input = StripHtmlFromString(input);
                input = GetSafeHtml(input, true);
            }
            return input;
        }

        /// <summary>
        /// Returns a specified amount of words from a string
        /// </summary>
        /// <param name="text"></param>
        /// <param name="wordAmount"></param>
        /// <returns></returns>
        public static string ReturnAmountWordsFromString(string text, int wordAmount)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            string tmpStr;
            string[] stringArray;
            var tmpStrReturn = "";
            tmpStr = text.Replace("\t", " ").Trim();
            tmpStr = tmpStr.Replace("\n", " ");
            tmpStr = tmpStr.Replace("\r", " ");

            while (tmpStr.IndexOf("  ") != -1)
                tmpStr = tmpStr.Replace("  ", " ");

            stringArray = tmpStr.Split(' ');

            if (stringArray.Length < wordAmount)
                    return text;
            else
            {
                for (int i = 0; i < wordAmount; i++)
                    tmpStrReturn += stringArray[i] + " ";
                return tmpStrReturn + "...";
            }
        }

        //public static string ConvertPostContent(string post)
        //{
        //    if (!string.IsNullOrEmpty(post))
        //    {
        //        // Convert any BBCode
        //        //NOTE: Decided to remove BB code
        //        //post = StringUtils.ConvertBbCodeToHtml(post, false);

        //        // If using the PageDown/MarkDown Editor uncomment this line
        //        post = StringUtils.ConvertMarkDown(post);

        //        // Allow video embeds
        //        post = StringUtils.EmbedVideosInPosts(post);

        //        // Add Google prettify code snippets
        //        post = post.Replace("<pre>", "<pre class='prettyprint'>");
        //    }

        //    return post;
        //}


        ///// <summary>
        ///// Converts markdown into HTML
        ///// </summary>
        ///// <param name="str"></param>
        ///// <returns></returns>
        //public static string ConvertMarkDown(string str)
        //{
        //    var md = new MarkdownSharp.Markdown { AutoHyperlink = true, LinkEmails = false };
        //    return md.Transform(str);
        //}

        #endregion


        #region URL / WEB HELPERS


        /// <summary>
        /// Creates a URL friendly string, good for SEO
        /// </summary>
        public static string CreateUrl(string strInput, string replaceWith)
        {
            // Doing this to stop the urls having amp from &amp;
            strInput = HttpUtility.HtmlDecode(strInput);
            // Doing this to stop the urls getting encoded
            var url = RemoveAccents(strInput);
            return StripNonAlphaNumeric(url, replaceWith).ToLower();
        }

        public static string RemoveAccents(string input)
        {
            // Replace accented characters for the closest ones:
            var stFormD = input.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var t in stFormD)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(t);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(t);
                }
            }

            return (sb.ToString().Normalize(NormalizationForm.FormC));

        }

        /// <summary>
        /// Get the current users IP address
        /// </summary>
        /// <returns></returns>
        public static string GetUsersIpAddress()
        {
            var context = HttpContext.Current;
            var serverName = context.Request.ServerVariables["SERVER_NAME"];
            if (serverName.ToLower().Contains("localhost"))
            {
                return serverName;
            }
            var ipList = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            return !string.IsNullOrEmpty(ipList) ? ipList.Split(',')[0] : context.Request.ServerVariables["REMOTE_ADDR"];
        }

        /// <summary>
        /// Returns true if the file is an image based on the file extension
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool FileIsImage(string file)
        {
            var imageFileTypes = new List<string>
            {
                ".jpg", ".jpeg",".gif",".bmp",".png"
            };
            return imageFileTypes.Any(file.Contains);
        }
        #endregion


        #region DATA IMPORT HELPERS

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

        #endregion





    }
}