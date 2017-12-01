using System;
using System.Collections.Generic;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Web;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using EECIP.App_Logic.DataAccessLayer;
using System.Threading.Tasks;
using System.Linq;

namespace EECIP.App_Logic.BusinessLogicLayer
{
    public class SendGridHelper
    {


        /// <summary>
        /// Sends out an email using SendGrid. 
        /// Note: Updated to work with SendGrid version 9.8
        /// </summary>
        /// <returns>true if successful</returns>
        public static async Task<bool> SendGridEmail(string from, string to, List<string> cc, List<string> bcc, string subj, string body, string apiKey, string bodyHTML = null)
        {
            try
            {
                var client = new SendGridClient(apiKey);

                //******************** CONSTRUCT EMAIL ********************************************
                // Create the email object first, then add the properties.
                var msg = new SendGridMessage();

                // Add message properties.
                msg.Subject = subj;
                msg.AddContent(MimeType.Text, body);
                if (bodyHTML != null)
                    msg.AddContent(MimeType.Html, bodyHTML);
                msg.From = new EmailAddress(from);
                msg.AddTo(to);

                foreach (string cc1 in cc ?? Enumerable.Empty<string>())
                    msg.AddCc(cc1);

                foreach (string bcc1 in bcc ?? Enumerable.Empty<string>())
                    msg.AddBcc(bcc1);


                //******************** SEND EMAIL ****************************************************
                var response = await client.SendEmailAsync(msg).ConfigureAwait(false);


                //******************** RETURN RESPONSE ***********************************************
                if (response.StatusCode == HttpStatusCode.Accepted)
                    return true;
                else
                    return false;
                //************************************************************************************

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


    }
}