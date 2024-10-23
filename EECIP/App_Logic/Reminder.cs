using System;
using System.Collections.Generic;
using System.Linq;
using EECIP.App_Logic.BusinessLogicLayer;
using EECIP.App_Logic.DataAccessLayer;


namespace EECIP.App_Logic
{
    public static class ReminderClass
    {

        public static bool generateReminder(string emailOverride)
        {
            if ((emailOverride ?? "").Length < 3) emailOverride = null;

            //get listing of all projects that have reminders
            List<T_OE_PROJECTS> _proj = db_EECIP.GetProjectReminders();

            //get unique listing of people to which reminder is going out to
            List<int?> _users = _proj.Select(e => e.MODIFY_USERIDX ?? e.CREATE_USERIDX).Distinct().ToList();

            //get email header and footer
            string emailHeader = @"<div style='width:100%; padding: 0; padding-top:10px; background-color: #393939; font-family: Helvetica, Arial, sans-serif; min-height: 42px;'>
                                                <a href='" + db_Ref.GetT_OE_APP_SETTING("PUBLIC_APP_PATH") + @"/Dashboard' style='font-size: 22px; color: #D9D9D9; padding: 14px 10px 12px; margin-left: 0; font-family: Helvetica, Arial, sans-serif; color: #d9d9d9; line-height: 20px; text-decoration: none;'>
                                                    <img src='https://www.eecip.net/Content/Images/seal.png' style='width:30px; padding-right:10px; vertical-align: middle'>
                                                    E-Enterprise Community Inventory Platform (EECIP)
                                                </a>
                                            </div>
                                        <br/><p style='font-family: Helvetica, Arial, sans-serif; margin-left:6px; '></p>";
            string emailFooter = @"<div style='width:100%; padding-top: 20px; padding-bottom: 20px; background-color: #393939; font-family: Helvetica, Arial, sans-serif; text-align:center; color: #D9D9D9'>
                                            <p>These news items can be personalized to match your interests. You can add or change subscriptions preferences on your <a href='" + db_Ref.GetT_OE_APP_SETTING("PUBLIC_APP_PATH") + @"/Account/UserProfile'>EECIP Profile</a></p>
                                            To learn more about EECIP, visit our <a href='" + db_Ref.GetT_OE_APP_SETTING("PUBLIC_APP_PATH") + @"/Dashboard' style='color: #D9D9D9; min-width: 150px;padding: 14px 10px 12px; margin-left: 0; font-family: Helvetica, Arial, sans-serif; color: #d9d9d9; line-height: 20px;'>website</a>
                                        <br/>
                                        </div>";

            //get email body template
            string emailBody = db_Ref.GetT_OE_APP_SETTING_CUSTOM().REMINDER_EMAIL;


            foreach (int _user in _users)
            {
                T_OE_USERS _userRec = db_Accounts.GetT_OE_USERSByIDX(_user);

                emailBody = emailBody.Replace("[Name]", _userRec.FNAME + " " + _userRec.LNAME);

                //merge projects in
                string projSnippet = "";
                List<T_OE_PROJECTS> _projFiltered = _proj.Where(x => (x.MODIFY_USERIDX ?? x.CREATE_USERIDX) == _user).Select(x => x).ToList();
                foreach (T_OE_PROJECTS _itemFiltered in _projFiltered)
                    projSnippet = projSnippet + _itemFiltered.PROJ_NAME + "<br/>";

                emailBody = emailBody.Replace("[ProjectList]", projSnippet);


                //********* merge email parts ****************
                string htmlemail = emailHeader + emailBody + " <br/><br/>" + emailFooter;

                //********* send email (if anything to send) ****************
                Utils.SendEmail(null, emailOverride ?? _userRec.EMAIL, null, null, "EECIP Project Reminder", htmlemail, null, null, htmlemail);

                db_Ref.InsertT_OE_SYS_EMAIL_LOG(null, _userRec.EMAIL, null, "EECIP Reminder", htmlemail, "Reminder");

            }


            if (String.IsNullOrEmpty(emailOverride))
            {
                //update the next run time if no override email set
                string _newnext = System.DateTime.Now.AddMonths(1).ToString("MM/dd/yyyy HH:mm");
                db_Ref.InsertUpdateT_OE_APP_SETTING(null, "REMINDER_NEXT_RUN", _newnext, false, null);

                //update the reminder counter and remove reminder if past counter max (if no override email sent
                foreach (T_OE_PROJECTS _projCounterUpdate in _proj)
                    db_EECIP.UpdatetT_OE_PROJECTS_IncrementReminderCount(_projCounterUpdate.PROJECT_IDX);
            }


            return true;

        }

    }
}