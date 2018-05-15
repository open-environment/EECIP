using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EECIP.App_Logic.BusinessLogicLayer;

namespace EECIP.App_Logic.DataAccessLayer
{
    public class db_Accounts
    {
        //*****************USERS **********************************
        public static int CreateT_OE_USERS(string uSER_ID, string pWD_HASH, string pWD_SALT, string fNAME, string lNAME, string eMAIL, bool aCT_IND, bool iNITAL_PWD_FLAG,
            DateTime? lASTLOGIN_DT, string pHONE, string pHONE_EXT, int? cREATE_USERIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    T_OE_USERS u = new T_OE_USERS();
                    u.USER_ID = uSER_ID;
                    u.PWD_HASH = pWD_HASH;
                    u.PWD_SALT = pWD_SALT;
                    u.FNAME = fNAME;
                    u.LNAME = lNAME;
                    u.EMAIL = eMAIL;
                    u.ACT_IND = aCT_IND;
                    u.INITAL_PWD_FLAG = iNITAL_PWD_FLAG;
                    u.EFFECTIVE_DT = System.DateTime.Now;
                    u.LASTLOGIN_DT = lASTLOGIN_DT;
                    u.PHONE = pHONE;
                    u.PHONE_EXT = pHONE_EXT;
                    u.CREATE_DT = System.DateTime.Now;
                    u.CREATE_USERIDX = cREATE_USERIDX;

                    ctx.T_OE_USERS.Add(u);
                    ctx.SaveChanges();
                    return u.USER_IDX;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    throw ex;
                }
            }
        }

        public static List<T_OE_USERS> GetT_OE_USERS()
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return ctx.T_OE_USERS.ToList();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    throw ex;
                }
            }
        }

        public static T_OE_USERS GetT_OE_USERSByIDX(int idx)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return ctx.T_OE_USERS.FirstOrDefault(usr => usr.USER_IDX == idx);
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    throw ex;
                }
            }
        }

        public static T_OE_USERS GetT_OE_USERSByID(string id)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return ctx.T_OE_USERS.FirstOrDefault(usr => usr.USER_ID.ToUpper() == id.ToUpper());
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    throw ex;
                }
            }
        }

        public static List<T_OE_USERS> GetT_OE_USERSByAgency(Guid OrgID)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.T_OE_USERS
                            where a.ORG_IDX == OrgID
                            orderby a.LNAME
                            select a).ToList();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static bool UserCanEditOrgIDX(int idx, Guid orgIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    //if governance, anyone can edit
                    T_OE_ORGANIZATION _org = db_Ref.GetT_OE_ORGANIZATION_ByID(orgIDX);
                    if (_org != null)
                        if (_org.ORG_TYPE == "Governance")
                            return true;


                    //otherwise, check if user belongs
                    int c =  (from a in ctx.T_OE_USERS
                              where a.ORG_IDX == orgIDX
                              && a.USER_IDX == idx
                              select a).Count();

                    return (c > 0);
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return false;
                }
            }
        }

        public static int UpdateT_OE_USERS(int idx, string pWD_HASH, string pWD_SALT, string fNAME, string lNAME, string eMAIL, bool? aCT_IND, bool? iNIT_PWD_FLG, 
            DateTime? eFF_DATE, DateTime? lAST_LOGIN_DT, string pHONE, string pHONE_EXT, int? mODIFY_USR, int? LogAtmpt, Guid? oRG_IDX, string jOB_TITLE, 
            string lINKED_IN, bool? NodeAdmin, bool? ExcludeBadges)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    T_OE_USERS row = new T_OE_USERS();
                    row = (from c in ctx.T_OE_USERS where c.USER_IDX == idx select c).First();

                    if (pWD_HASH != null) row.PWD_HASH = pWD_HASH;
                    if (pWD_SALT != null) row.PWD_SALT = pWD_SALT;
                    if (fNAME != null) row.FNAME = fNAME;
                    if (lNAME != null) row.LNAME = lNAME;
                    if (eMAIL != null) row.EMAIL = eMAIL;
                    if (aCT_IND != null) row.ACT_IND = (bool)aCT_IND;
                    if (iNIT_PWD_FLG != null) row.INITAL_PWD_FLAG = (bool)iNIT_PWD_FLG;
                    if (eFF_DATE != null) row.EFFECTIVE_DT = (DateTime)eFF_DATE;
                    if (lAST_LOGIN_DT != null) row.LASTLOGIN_DT = (DateTime)lAST_LOGIN_DT;
                    if (pHONE != null) row.PHONE = pHONE;
                    if (pHONE_EXT != null) row.PHONE_EXT = pHONE_EXT;
                    if (mODIFY_USR != null) row.MODIFY_USERIDX = mODIFY_USR;
                    if (LogAtmpt != null) row.LOG_ATMPT = LogAtmpt;
                    if (oRG_IDX != null) row.ORG_IDX = oRG_IDX;
                    if (jOB_TITLE != null) row.JOB_TITLE = jOB_TITLE;
                    if (lINKED_IN != null) row.LINKEDIN = lINKED_IN;
                    if (NodeAdmin != null) row.NODE_ADMIN = NodeAdmin ?? false;
                    if (ExcludeBadges != null) row.EXCLUDE_POINTS_IND = (bool)ExcludeBadges;

                    row.MODIFY_DT = System.DateTime.Now;

                    ctx.SaveChanges();
                    return 1;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }

        public static int DeleteT_OE_USERS(int idx)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    T_OE_USERS row = ctx.T_OE_USERS.First(i => i.USER_IDX == idx);
                    ctx.T_OE_USERS.Remove(row);
                    ctx.SaveChanges();
                    return 1;
                }
                catch (Exception ex)
                {
                    //set to inactive if cannot delete
                    UpdateT_OE_USERS(idx, null, null, null, null, null, false, null, null, null, null, null, null, null, null, null, null, null,false);
                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }

        public static bool ResetT_OE_USERS_Unsynced()
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_OE_USERS
                               where a.SYNC_IND == true
                               select a).ToList();

                    xxx.ForEach(a => a.SYNC_IND = false);
                    ctx.SaveChanges();

                    return true;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return false;
                }
            }
        }

        public static List<EECIP_Index> GetT_OE_USERS_ReadyToSync(int? UserIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_OE_USERS
                               join o in ctx.T_OE_ORGANIZATION on a.ORG_IDX equals o.ORG_IDX
                               join s in ctx.T_OE_REF_STATE on o.STATE_CD equals s.STATE_CD into sr1 from x1 in sr1.DefaultIfEmpty() //left join
                               where a.SYNC_IND == false
                               && (UserIDX == null ? true : a.USER_IDX == UserIDX)
                               select new EECIP_Index
                               {
                                   Agency = o.ORG_NAME,
                                   AgencyAbbreviation = o.ORG_ABBR,
                                   State_or_Tribal = (o.ORG_TYPE == "State" ? x1.STATE_NAME : o.ORG_TYPE),
                                   KeyID = a.USER_IDX.ToString(),
                                   DataType = "User",
                                   Record_Source = "Agency supplied",
                                   Name = a.LNAME + ", " + a.FNAME,
                                   Description = a.JOB_TITLE,
                                   PersonPhone = a.PHONE,
                                   PersonEmail = a.EMAIL,
                                   PersonLinkedIn = a.LINKEDIN,
                                   Population_Density = x1.POP_DENSITY,
                                   EPA_Region = o.EPA_REGION.ToString(),
                                   LastUpdated = a.MODIFY_DT ?? a.CREATE_DT
                               }).ToList();

                    foreach (EECIP_Index e in xxx)
                        e.Tags = db_EECIP.GetT_OE_USER_EXPERTISE_ByUserIDX(e.KeyID.ConvertOrDefault<int>()).ToArray();


                    return xxx;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int UpdateT_OE_USERS_Avatar(int idx, Byte[] UserAvatar)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    T_OE_USERS row = (from c in ctx.T_OE_USERS where c.USER_IDX == idx select c).First();
                    if (UserAvatar != null) row.USER_AVATAR = UserAvatar;
                    ctx.SaveChanges();
                    return row.USER_IDX;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }

        public static int GetUserIDX()
        {
            try
            {
                return (int)System.Web.Security.Membership.GetUser().ProviderUserKey;
            }
            catch
            {
                //if fails, we don't care why, but need to return 0 to indicate not authenticated
                return 0;
            }
        }

        public static int UpdateT_OE_USERS_UnlockGovernance(int idx)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    T_OE_USERS row = (from c in ctx.T_OE_USERS where c.USER_IDX == idx select c).First();
                    row.ALLOW_GOVERNANCE = true;
                    ctx.SaveChanges();
                    return row.USER_IDX;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }


        //*****************ROLES **********************************
        public static int CreateT_OE_ROLES(global::System.String rOLE_NAME, global::System.String rOLE_DESC, int? cREATE_USER = 0)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    T_OE_ROLES r = new T_OE_ROLES();
                    r.ROLE_NAME = rOLE_NAME;
                    r.ROLE_DESC = rOLE_DESC;
                    r.CREATE_DT = System.DateTime.Now;
                    r.CREATE_USERIDX = cREATE_USER;

                    ctx.T_OE_ROLES.Add(r);
                    ctx.SaveChanges();
                    return 1;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }

        public static List<T_OE_ROLES> GetT_OE_ROLES()
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return ctx.T_OE_ROLES.ToList();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    throw ex;
                }
            }
        }

        public static T_OE_ROLES GetT_OE_ROLEByIDX(int idx)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return ctx.T_OE_ROLES.FirstOrDefault(role => role.ROLE_IDX == idx);
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    throw ex;
                }
            }
        }

        public static int UpdateT_OE_ROLE(int idx, string newROLE_NAME, string newROLE_DESC, int? newMODIFY_USR)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    T_OE_ROLES row = new T_OE_ROLES();
                    row = (from c in ctx.T_OE_ROLES where c.ROLE_IDX == idx select c).First();

                    if (newROLE_NAME != null)
                        row.ROLE_NAME = newROLE_NAME;

                    if (newROLE_DESC != null)
                        row.ROLE_DESC = newROLE_DESC;

                    if (newMODIFY_USR != null)
                        row.MODIFY_USERIDX = newMODIFY_USR;

                    row.MODIFY_DT = System.DateTime.Now;

                    ctx.SaveChanges();
                    return row.ROLE_IDX;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }

        public static int DeleteT_OE_ROLE(int idx)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    T_OE_ROLES row = new T_OE_ROLES();
                    row = (from c in ctx.T_OE_ROLES where c.ROLE_IDX == idx select c).First();
                    ctx.T_OE_ROLES.Remove(row);
                    ctx.SaveChanges();
                    return 1;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }



        //*****************ROLE / USER RELATIONSHIP **********************************
        public static List<T_OE_USERS> GetT_OE_USERSInRole(int roleID)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var users = from itemA in ctx.T_OE_USERS
                                join itemB in ctx.T_OE_USER_ROLES on itemA.USER_IDX equals itemB.USER_IDX
                                where itemB.ROLE_IDX == roleID
                                orderby itemA.USER_ID
                                select itemA;

                    return users.ToList();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<T_OE_USERS> GetT_OE_USERSNotInRole(int roleID)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    //first get all users 
                    var allUsers = (from itemA in ctx.T_OE_USERS select itemA);

                    //next get all users in role
                    var UsersInRole = (from itemA in ctx.T_OE_USERS
                                       join itemB in ctx.T_OE_USER_ROLES on itemA.USER_IDX equals itemB.USER_IDX
                                       where itemB.ROLE_IDX == roleID
                                       select itemA);

                    //then get exclusions
                    var usersNotInRole = allUsers.Except(UsersInRole);

                    return usersNotInRole.OrderBy(a => a.USER_ID).ToList();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<T_OE_ROLES> GetT_OE_ROLESInUserName(string userName)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var roles = from itemA in ctx.T_OE_ROLES
                                join itemB in ctx.T_OE_USER_ROLES on itemA.ROLE_IDX equals itemB.ROLE_IDX
                                join itemC in ctx.T_OE_USERS on itemB.USER_IDX equals itemC.USER_IDX
                                where itemC.USER_ID.ToUpper() == userName.ToUpper()
                                select itemA;

                    return roles.ToList();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }


        public static int CreateT_OE_USER_ROLE(global::System.Int32 rOLE_IDX, global::System.Int32 uSER_IDX, int? cREATE_USER = 0)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    T_OE_USER_ROLES ur = new T_OE_USER_ROLES();
                    ur.ROLE_IDX = rOLE_IDX;
                    ur.USER_IDX = uSER_IDX;
                    ur.CREATE_DT = System.DateTime.Now;
                    ur.CREATE_USERIDX = cREATE_USER;
                    ctx.T_OE_USER_ROLES.Add(ur);
                    ctx.SaveChanges();
                    return 1;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }

        public static int DeleteT_OE_USER_ROLE(int UserIDX, int RoleIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    T_OE_USER_ROLES row = (from c in ctx.T_OE_USER_ROLES
                                           where c.ROLE_IDX == RoleIDX && c.USER_IDX == UserIDX
                                           select c).FirstOrDefault();
                    ctx.T_OE_USER_ROLES.Remove(row);
                    ctx.SaveChanges();
                    return 1;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }

    }
}