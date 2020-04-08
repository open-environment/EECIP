using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using EECIP.App_Logic.BusinessLogicLayer;
using EECIP.App_Logic.DataAccessLayer;

namespace EECIP_Service
{
    public partial class EECIPService : ServiceBase
    {
        private Timer timer = new Timer();


        public EECIPService()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Startup code - this method runs when the service starts up for the first time.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            General.WriteToFile("EECIP Task Service started");

            try
            {
                // Set up a timer that triggers every minute.
                timer.Interval = 600000; // 1 minutes
                timer.Elapsed += new ElapsedEventHandler(OnTimer);
                timer.AutoReset = true;
                timer.Enabled = true;
                timer.Start();
                General.WriteToFile("*************************************************");
                General.WriteToFile("EECIP Task Service timer successfully initialized");
                General.WriteToFile("*************************************************");
                General.WriteToFile("EECIP Task Service timer set to run every " + timer.Interval + " ms");
            }
            catch (Exception ex)
            {
                General.WriteToFile("Failed to start EECIP - Unspecified error. " + ex.Message);
            }
        }

        protected override void OnStop()
        {
            General.WriteToFile("EECIP Task has stopped");
        }


        public void OnTimer(object sender, ElapsedEventArgs args)
        {
            try
            {
                string emailTemplate = "";
                List<UserDisplayType> _users = db_Accounts.GetT_OE_USERS_ForWelcomeEmail();
                if (_users != null)
                {
                    //get template text
                    T_OE_APP_SETTINGS_CUSTOM _cust = db_Ref.GetT_OE_APP_SETTING_CUSTOM();
                    if (_cust != null && _cust.WELCOME_EMAIL != null)
                    {

                        emailTemplate = _cust.WELCOME_EMAIL;

                        //get cc email addressee
                        string cc = db_Ref.GetT_OE_APP_SETTING("EMAIL_WELCOME_CC");
                        List<string> cclist = new List<string>() { cc };


                        //loop through all people to send emails to
                        foreach (UserDisplayType _user in _users)
                        {
                            string emailBody = emailTemplate.Replace("[Name]", _user.users.FNAME + " " + _user.users.LNAME);

                            //send email
                            Utils.SendEmail(null, _user.users.EMAIL, cclist, null, "Welcome to EECIP!", emailBody, null, null, emailBody);
                            db_Ref.InsertT_OE_SYS_EMAIL_LOG(null, _user.users.EMAIL, null, "Welcome to EECIP!", emailBody, "Welcome");

                            General.WriteToFile("Welcome email sent to " + _user.users.EMAIL);

                            //remove user from list
                            db_Accounts.UpdateT_OE_USERS_WelcomeEmailSent(_user.users.USER_IDX);
                        }
                    }
                    else
                        General.WriteToFile("Template NOT found");
                }
                else
                    General.WriteToFile("No New Users");

                General.WriteToFile("EECIP task ran.");
            }
            catch (Exception ex)
            {
                General.WriteToFile("ERROR invoking Task: " + ex.Message.ToString());
                General.WriteToFile("ERROR invoking Task 2: " + ex.InnerException.ToString());
            }

        }
    }
}
