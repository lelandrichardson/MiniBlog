using System;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Web.Security;
using MiniBlog.Includes.Data;

namespace MiniBlog.Framework
{
    public class MiniBlogMembershipProvider : MembershipProvider
    {
        const int SaltLength = 15;
        #region Unused Methods

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }


        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
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

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotImplementedException();
        }

        #endregion

        public string GenerateSalt(int length)
        {
            byte[] randomArray = new byte[length];

            string randomString;

            //Create random salt and convert to string
            Random rnd = new Random();

            rnd.NextBytes(randomArray);

            randomString = Convert.ToBase64String(randomArray);

            return randomString;
        }

        public string CreateUser(string fullName, string email, string password)
        {
            
            int result;
            using (var db = Db.GetOpenConnection())
            {
                result = db.Query<int>("site.spu_Author_Create",
                new
                    {
                        FullName = fullName,
                        Email = email, 
                        Password = password,
                        salt = GenerateSalt(SaltLength)
                    },
                commandType: CommandType.StoredProcedure).First();
            }

            if (result > 0) return string.Empty;
            return "There was an error during registration";
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            int result;
            using(var db = Db.GetOpenConnection())
            {
                result = db.Query<int>("dbo.spu_Author_UpdatePassword",
                new
                    {
                        email = username,
                        oldPassword,
                        newPassword,
                        newSalt = GenerateSalt(SaltLength)
                    },
                commandType: CommandType.StoredProcedure).First();
            }
            return result > 0;
        }

        public override string ResetPassword(string username, string answer)
        {
            //TODO: Create our own version of this
            //i don't think this is the right format for this?
            throw new NotImplementedException();
        }


        public override bool ValidateUser(string username, string password)
        {
            int result;
            using(var db = Db.GetOpenConnection())
            {
                result = db.Query<int>("dbo.spu_Author_Authorize",
                    new {Email = username, Password = password}).SingleOrDefault();
            }
            return result>0;
        }

        #region Unused Methods

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }


        public override bool EnablePasswordRetrieval
        {
            get { return false; }
        }

        public override bool EnablePasswordReset
        {
            get { return true; }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return false; }
        }

        public override string ApplicationName { get; set; }

        public override int MaxInvalidPasswordAttempts
        {
            get { return 5; }
        }

        public override int PasswordAttemptWindow
        {
            get { return 5; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return true; }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredPasswordLength
        {
            get { return 8; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return 1; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }

}
