using MailChimp.Net;
using MailChimp.Net.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Threading.Tasks;
using EECIP.App_Logic.DataAccessLayer;

namespace EECIP.App_Logic.BusinessLogicLayer
{
    public class MailChimpHelper
    {
        public bool AddUpdateMailChimpContact(string userEmail, string fName, string lName) {

            string apiKey = db_Ref.GetT_OE_APP_SETTING("MAILCHIMP_API");
            string listId = db_Ref.GetT_OE_APP_SETTING("MAILCHIMP_LISTID");

            ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | SecurityProtocolType.Tls12;
            IMailChimpManager manager = new MailChimpManager(apiKey);

            try
            {
                //check if user exists
                var userExists = manager.Members.ExistsAsync(listId, userEmail).Result;
                if (!userExists)
                {
                    //create user
                    MailChimp.Net.Models.Member _newMember = new MailChimp.Net.Models.Member
                    {
                        EmailAddress = userEmail,
                        StatusIfNew = MailChimp.Net.Models.Status.Subscribed,
                    };
                    _newMember.MergeFields.Add("FNAME", fName);
                    _newMember.MergeFields.Add("LNAME", lName);
                    //_newMember.Status = MailChimp.Net.Models.Status.Subscribed;
                    MailChimp.Net.Models.MemberTag _tag = new MailChimp.Net.Models.MemberTag { Name = "EECIP User" };
                    _newMember.Tags.Add(_tag);

                    var xxx = manager.Members.AddOrUpdateAsync(listId, _newMember).Result;
                }
                else
                {
                    //update user
                    MailChimp.Net.Models.Member _newMember = new MailChimp.Net.Models.Member
                    {
                        EmailAddress = userEmail,
                        StatusIfNew = MailChimp.Net.Models.Status.Subscribed,
                    };
                    _newMember.MergeFields.Add("FNAME", fName);
                    _newMember.MergeFields.Add("LNAME", lName);
                    var xxx = manager.Members.AddOrUpdateAsync(listId, _newMember).Result;

                    //update EECIP User Tag only
                    MailChimp.Net.Models.Tags tags = new MailChimp.Net.Models.Tags();
                    tags.MemberTags.Add(new MailChimp.Net.Models.Tag() { Name = "EECIP User", Status = "active" });
                    manager.Members.AddTagsAsync(listId, userEmail, tags).GetAwaiter().GetResult();
                }



                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public bool RemoveMailChimpContant(string userEmail)
        {
            string apiKey = db_Ref.GetT_OE_APP_SETTING("MAILCHIMP_API");
            string listId = db_Ref.GetT_OE_APP_SETTING("MAILCHIMP_LISTID");

            ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | SecurityProtocolType.Tls12;
            IMailChimpManager manager = new MailChimpManager(apiKey);

            try
            {
                //check if user exists
                var userExists = manager.Members.ExistsAsync(listId, userEmail).Result;
                if (!userExists)
                {
                    //do nothing
                }
                else
                {
                    //remove EECIP User Tag only
                    MailChimp.Net.Models.Tags tags = new MailChimp.Net.Models.Tags();
                    tags.MemberTags.Add(new MailChimp.Net.Models.Tag() { Name = "EECIP User", Status = "inactive" });
                    manager.Members.AddTagsAsync(listId, userEmail, tags).GetAwaiter().GetResult();
                }



                return true;
            }
            catch (Exception ex)
            {
                return false;
            }


        }

    }
}