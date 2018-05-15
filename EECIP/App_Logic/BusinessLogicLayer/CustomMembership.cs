using System;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;
using EECIP.App_Logic.DataAccessLayer;

namespace EECIP.App_Logic.BusinessLogicLayer
{
    /// <summary>
    /// Implements a custom provider for the ASP.NET Membership Service. 
    /// </summary>
    public class CustomMembershipProvider : MembershipProvider
    {
        private bool _EnablePasswordRetrieval;
        private bool _EnablePasswordReset = true;
        private int _MaxInvalidPasswordAttempts;
        private int _MinRequiredPasswordLength;
        private int _MinRequiredNonalphanumericCharacters;
        private int _PasswordAttemptWindow;
        private MembershipPasswordFormat _PasswordFormat;
        private bool _RequiresQuestionAndAnswer = false;
        private bool _RequiresUniqueEmail;

        public override bool EnablePasswordRetrieval { get { return _EnablePasswordRetrieval; } }
        public override bool EnablePasswordReset { get { return _EnablePasswordReset; } }
        public override int MaxInvalidPasswordAttempts { get { return _MaxInvalidPasswordAttempts; } }
        public override int MinRequiredNonAlphanumericCharacters { get { return _MinRequiredNonalphanumericCharacters; } }
        public override int MinRequiredPasswordLength { get { return _MinRequiredPasswordLength; } }
        public override int PasswordAttemptWindow { get { return _PasswordAttemptWindow; } }
        public override MembershipPasswordFormat PasswordFormat { get { return _PasswordFormat; } }
        public override string PasswordStrengthRegularExpression { get { return null; } }
        public override bool RequiresUniqueEmail { get { return _RequiresUniqueEmail; } }
        public override bool RequiresQuestionAndAnswer { get { return _RequiresQuestionAndAnswer; } }
        public override string ApplicationName
        {
            get { return "EECIP"; }
            set { throw new NotImplementedException(); }
        }

        // Initialize methods
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);

            _EnablePasswordRetrieval = false;
            _EnablePasswordReset = true;
            _RequiresQuestionAndAnswer = false;
            _RequiresUniqueEmail = true;
            _MaxInvalidPasswordAttempts = 5;
            _PasswordAttemptWindow = 10;
            _MinRequiredPasswordLength = 8;
            _MinRequiredNonalphanumericCharacters = 0;
            _PasswordFormat = MembershipPasswordFormat.Hashed;
        }


        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            //validate new password length
            if (newPassword == null)
                return false;

            if (newPassword.Length < _MinRequiredPasswordLength)
                return false;

            //Validate Non-AlphaNumeric characters
            char[] charpwd = newPassword.ToCharArray();
            int pwdNonNumericCount = 0;
            for (int i = 1; i < newPassword.Length; i++)
            {
                if (!char.IsLetterOrDigit(charpwd[i]))
                    pwdNonNumericCount += 1;
            }

            if (pwdNonNumericCount < _MinRequiredNonalphanumericCharacters)
                return false;

            T_OE_USERS u = db_Accounts.GetT_OE_USERSByID(username);
            if (u != null)
            {
                //first check accuracy of old password
                if (!CheckPassword(oldPassword, u.PWD_HASH, u.PWD_SALT))
                    return false;

                //generate new password
                string salt = GenerateSalt();
                string hashpass = HashPassword(newPassword, _PasswordFormat, salt);
                //save updated information
                if (db_Accounts.UpdateT_OE_USERS(u.USER_IDX, hashpass, salt, null, null, null, null, false, null, null, null, null, 0, null, null, null, null, null,false) == 1)
                    return true;
                else
                    return false;
            }

            return true;
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            status = MembershipCreateStatus.Success;

            //******************************** BEGIN VALIDATION ********************************************************
            //Validate Username Length
            if (username.Length > 150)
            {
                status = MembershipCreateStatus.InvalidEmail;
                return null;
            }

            T_OE_USERS u = db_Accounts.GetT_OE_USERSByID(username);
            if (u != null)
            {
                //Duplicate username found -return error
                status = MembershipCreateStatus.DuplicateUserName;
                return null;
            }
            //******************************** END VALIDATION ***********************************************************

            try
            {
                //Generate password and hash it
                password = RandomString(10);
                string salt = GenerateSalt();
                string hashpass = HashPassword(password, _PasswordFormat, salt);

                //create user record
                int createUser = db_Accounts.CreateT_OE_USERS(username, hashpass, salt, "", "", email, true, true, null, null, null, 0);
                if (createUser > 0)  //Success
                {
                    //encrypt username for email
                    string encryptOauth = new SimpleAES().Encrypt(password + "||" + username);
                    encryptOauth = System.Web.HttpUtility.UrlEncode(encryptOauth);

                    //send verification email to user
                    string message = "Welcome to EECIP."
                        + "\r\n\r\n Your username is: " + username
                        + "\r\n\r\n You must activate your account by clicking the following link: "
                        + "\r\n\r\n " + db_Ref.GetT_OE_APP_SETTING("PUBLIC_APP_PATH") + "/Account/Verify?oauthcrd=" + encryptOauth
                        + "\r\n\r\n After verifying your account you will be prompted to enter a permanent password.";

                    //send verification email to user
                    string messageHTML = "<p><b>Welcome to EECIP.</b></p>"
                        + "<p>Your username is: " + username + "</p>"
                        + "<p>You must activate your account by clicking the following link: </p>"
                        + "<p><a href='" + db_Ref.GetT_OE_APP_SETTING("PUBLIC_APP_PATH") + "/Account/Verify?oauthcrd=" + encryptOauth + "'>Click Verification Link</a></p>"
                        + "<p>After verifying your account you will be prompted to enter a permanent password.</p>";


                    bool EmailStatus = Utils.SendEmail(null, email, null, null, "Confirm Your EECIP Account", message, null, "", messageHTML);

                    //delete user if the email sending failed
                    if (EmailStatus == false)
                    {
                        status = MembershipCreateStatus.InvalidEmail;
                        db_Accounts.DeleteT_OE_USERS(createUser);
                    }

                    return new MembershipUser(this.Name, username, createUser, email, passwordQuestion, null, isApproved, false, System.DateTime.Now, System.DateTime.Now, System.DateTime.Now, System.DateTime.Now, System.DateTime.Now);
                }
                else
                {
                    status = MembershipCreateStatus.ProviderError;
                    return null;
                }

            }
            catch (Exception exp)
            {
                throw exp;
            }

        }

        protected override byte[] DecryptPassword(byte[] encodedPassword)
        {
            return base.DecryptPassword(encodedPassword);
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            T_OE_USERS u = db_Accounts.GetT_OE_USERSByIDX((int)providerUserKey);

            var newCreateDate = u.CREATE_DT ?? System.DateTime.Now;
            var newModifyDate = u.MODIFY_DT ?? System.DateTime.Now;

            return new MembershipUser(this.Name, u.USER_ID, u.USER_IDX, u.EMAIL, null, null, true, false, newCreateDate, newModifyDate, newModifyDate, newModifyDate, newModifyDate);
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            T_OE_USERS u = db_Accounts.GetT_OE_USERSByID(username);
            if (u != null)
            {
                var newCreateDate = u.CREATE_DT ?? System.DateTime.Now;
                var newModifyDate = u.MODIFY_DT ?? System.DateTime.Now;

                return new MembershipUser(this.Name, u.USER_ID, u.USER_IDX, u.EMAIL, null, null, true, false, newCreateDate, newModifyDate, newModifyDate, newModifyDate, newModifyDate);
            }
            else
                return null;
        }

        public override string ResetPassword(string username, string answer)
        {
            T_OE_USERS u = db_Accounts.GetT_OE_USERSByID(username);
            if (u != null)
            {
                //generate new password
                string newPass = RandomString(10);
                string salt = GenerateSalt();
                string hashpass = HashPassword(newPass, _PasswordFormat, salt);

                //save updated hashed password and salt
                if (db_Accounts.UpdateT_OE_USERS(u.USER_IDX, hashpass, salt, null, null, null, null, true, null, null, null, null, null, 0, null, null, null, null,false) == 1)
                {
                    //encrypt username for email
                    string encryptOauth = new SimpleAES().Encrypt(newPass + "||" + username);
                    encryptOauth = System.Web.HttpUtility.UrlEncode(encryptOauth);

                    //send verification email to user
                    string msg = "Your EECIP password has been reset."
                        + "\r\n\r\n Your username is: " + username
                        + "\r\n\r\n You must click the following link to set your permanent password: "
                        + "\r\n\r\n " + db_Ref.GetT_OE_APP_SETTING("PUBLIC_APP_PATH") + "/Account/Verify?oauthcrd=" + encryptOauth;

                    //send verification email to user
                    string messageHTML = "<p><b>Your EECIP password has been reset.</b></p>"
                        + "<p>Your username is: " + username + "</p>"
                        + "<p>You must click the following link to set your permanent password: </p>"
                        + "<p><a href='" + db_Ref.GetT_OE_APP_SETTING("PUBLIC_APP_PATH") + "/Account/Verify?oauthcrd=" + encryptOauth + "'>Click Verification Link</a></p>";

                    if (Utils.SendEmail(null, u.EMAIL, null, null, "EECIP Password Reset", msg, null, "", messageHTML))
                        return "Success: Please check your email for password reset instructions.";
                    else
                        return "Error in sending email";

                }
                else
                {
                    return "Error resetting password";
                }
            }
            else
                return "Error: Email does not exist in the system.";
        }

        public override bool ValidateUser(string username, string password)
        {
            //check if password matches hashed/salted password
            T_OE_USERS u = db_Accounts.GetT_OE_USERSByID(username);
            if (u != null)
            {
                if (u.ACT_IND == false)
                    return false; //fail if user is inactive

                if (CheckPassword(password, u.PWD_HASH, u.PWD_SALT))
                    return true;
                else
                {
                    db_Accounts.UpdateT_OE_USERS(u.USER_IDX, null, null, null, null, null, u.LOG_ATMPT.ConvertOrDefault<int>() < MaxInvalidPasswordAttempts, null, null, null, null, null, null, u.LOG_ATMPT.ConvertOrDefault<int>() + 1, null, null, null, null,false);

                    //user account is locked due to too many invalid login attempts
                    if (u.LOG_ATMPT.ConvertOrDefault<int>() + 1 > MaxInvalidPasswordAttempts)
                        Utils.SendEmail(null, u.EMAIL, null, null, "EECIP account locked.", "Your EECIP user account has been locked due to too many incorrect login attempts. Please contact the system administrator to reset your user account.", null, "");

                    return false;
                }
            }
            else
                return false;

        }


        //CUSTOM ADDITIONS************************************************************
        private readonly Random _rng = new Random();
        private const string _chars = "ABCDEFGHJKLMNPQRSTUVWXYZbdefghjpry23456789";   //easily distinguishable characters

        private string GenerateSalt()
        {
            byte[] buf = new byte[32];
            (new RNGCryptoServiceProvider()).GetBytes(buf);
            return Convert.ToBase64String(buf);
        }

        private string HashPassword(string pass, MembershipPasswordFormat passwordFormat, string salt)
        {
            if (passwordFormat == MembershipPasswordFormat.Clear)
                return pass;
            else
            {
                SHA256Managed hash = new SHA256Managed();
                byte[] utf8 = UTF8Encoding.UTF8.GetBytes(pass + salt);
                StringBuilder s = new StringBuilder(hash.ComputeHash(utf8).Length * 2);
                foreach (byte b in hash.ComputeHash(utf8))
                    s.Append(b.ToString("x2"));
                return s.ToString();
            }
        }

        public bool CheckPassword(string password, string correctHash, string correctsalt)
        {
            string hashpass = "";
            if (correctsalt.Length < 1)  //if no salt value stored in DB, let users authenticate with clear password
                hashpass = HashPassword(password, MembershipPasswordFormat.Clear, correctsalt);
            else
                hashpass = HashPassword(password, MembershipPasswordFormat.Hashed, correctsalt);

            return (correctHash == hashpass);
        }

        private string RandomString(int size)
        {
            char[] buffer = new char[size];

            for (int i = 0; i < size; i++)
                buffer[i] = _chars[_rng.Next(_chars.Length)];

            return new string(buffer);
        }



        #region NotImplementedRegion

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}