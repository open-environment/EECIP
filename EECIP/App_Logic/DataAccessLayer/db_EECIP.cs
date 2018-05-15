using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using EECIP.App_Logic.BusinessLogicLayer;
using System.Data.Entity;

namespace EECIP.App_Logic.DataAccessLayer
{
    public class OrganizationEntServicesDisplayType
    {
        public int? ORG_ENT_SVCS_IDX { get; set; }
        public Guid? ORG_IDX { get; set; }
        public int ENT_PLATFORM_IDX { get; set; }
        public string ENT_PLATFORM_NAME { get; set; }
        public string RECORD_SOURCE { get; set; }
        public string ENT_PLATFORM_DESC { get; set; }
        public string PROJECT_NAME { get; set; }
        public string VENDOR { get; set; }
        public string IMPLEMENT_STATUS { get; set; }
        public string COMMENTS { get; set; }
        public string PROJECT_CONTACT { get; set; }
        public bool ACTIVE_INTEREST_IND { get; set; }
        public DateTime? CREATE_DT { get; set; }
        public int? CREATE_USERIDX { get; set; }
        public DateTime? MODIFY_DT { get; set; }
        public int? MODIFY_USERIDX { get; set; }
    }

    public class ProjectDisplayType
    {
        public Guid? PROJECT_IDX { get; set; }
        public Guid? ORG_IDX { get; set; }
        public string PROJ_NAME { get; set; }
        public string PROJ_DESC { get; set; }
        public int? MEDIA_TAG { get; set; }
        public string MEDIA_TAG_NAME { get; set; }
        public int? START_YEAR { get; set; }
        public string PROJ_STATUS { get; set; }
        public int? DATE_LAST_UPDATE { get; set; }
        public string RECORD_SOURCE { get; set; }
        public string PROJECT_URL { get; set; }
        public List<Tuple<int, string>> features { get; set; }
        public List<Tuple<int, string>> progam_areas { get; set; }
    }


    public class ProjectShortDisplayType
    {
        public Guid? PROJECT_IDX { get; set; }
        public Guid? ORG_IDX { get; set; }
        public string PROJ_NAME { get; set; }
        public string ORG_NAME { get; set; }
        public DateTime LAST_ACTIVITY_DATE { get; set; }
        public bool? TagMatch { get; set; }
    }


    public class ProjectImportType
    {
        public T_OE_PROJECTS T_OE_PROJECT { get; set; }
        public bool VALIDATE_CD { get; set; }
        public string VALIDATE_MSG { get; set; }
        public string ORG_NAME { get; set; }
        public string MEDIA_NAME { get; set; }
        public string MOBILE_IND_NAME { get; set; }
        public string ADV_MON_IND_NAME { get; set; }
        public string BP_MODERN_IND_NAME { get; set; }
        public string PROGRAM_AREAS { get; set; }
        public string FEATURES { get; set; }

        //INITIALIZE
        public ProjectImportType()
        {
            T_OE_PROJECT = new T_OE_PROJECTS();
            VALIDATE_CD = true;
        }
    }

    public class db_EECIP
    {

        //************************ USER_EXPERTISE ****************************************
        public static List<string> GetT_OE_USER_EXPERTISE_ByUserIDX(int id)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.T_OE_USER_EXPERTISE
                            where a.USER_IDX == id
                            select a.EXPERTISE_TAG).ToList();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<string> GetT_OE_USER_EXPERTISE_ByUserIDX_All(int id)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var xx1 = (from a in ctx.T_OE_USER_EXPERTISE
                               where a.USER_IDX == id
                               select a.EXPERTISE_TAG);

                    var xx2 = (from a in ctx.T_OE_REF_TAGS
                               //where a.TAG_CAT_NAME == "Expertise"
                               where a.TAG_CAT_NAME == "Project Feature"
                               select a.TAG_NAME);

                    return xx1.Union(xx2).ToList();

                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int InsertT_OE_USER_EXPERTISE(int uSER_IDX, string eXPERTISE_TAG, int? cREATE_USER = 0)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    T_OE_USER_EXPERTISE e = new T_OE_USER_EXPERTISE();
                    e.USER_IDX = uSER_IDX;
                    e.EXPERTISE_TAG = eXPERTISE_TAG;
                    e.CREATE_DT = System.DateTime.Now;
                    e.CREATE_USERIDX = cREATE_USER;

                    ctx.T_OE_USER_EXPERTISE.Add(e);

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

        public static int DeleteT_OE_USER_EXPERTISE(int UserIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    ctx.Database.ExecuteSqlCommand("DELETE FROM T_OE_USER_EXPERTISE where USER_IDX = {0}", UserIDX);

                    return 1;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }



        //************************** ORGANIZTION_ENTERPRISE_PLATFORM *************************************************
        public static List<OrganizationEntServicesDisplayType> GetT_OE_ORGANIZATION_ENTERPRISE_PLATFORM(Guid OrgID)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var yyy = (from a in ctx.T_OE_REF_ENTERPRISE_PLATFORM
                               join b in ctx.T_OE_ORGANIZATION_ENT_SVCS.Where(o => o.ORG_IDX == OrgID) on a.ENT_PLATFORM_IDX equals b.ENT_PLATFORM_IDX
                                   into sr
                               from x in sr.DefaultIfEmpty()  //left join
                               //where (b == null ? true : x.ORG_IDX == OrgID)
                               select new OrganizationEntServicesDisplayType
                               {
                                   ORG_ENT_SVCS_IDX = x.ORG_ENT_SVCS_IDX,
                                   ORG_IDX = x.ORG_IDX,
                                   ENT_PLATFORM_IDX = a.ENT_PLATFORM_IDX,
                                   ENT_PLATFORM_NAME = a.ENT_PLATFORM_NAME,
                                   PROJECT_NAME = x.PROJECT_NAME,
                                   VENDOR = x.VENDOR,
                                   IMPLEMENT_STATUS = x.IMPLEMENT_STATUS,
                                   COMMENTS = x.COMMENTS,
                                   PROJECT_CONTACT = x.PROJECT_CONTACT,
                                   ACTIVE_INTEREST_IND = x.ACTIVE_INTEREST_IND ?? false,
                                   CREATE_DT = x.CREATE_DT,
                                   MODIFY_DT = x.MODIFY_DT
                               }).ToList();

                    //var xxx = (from a in ctx.T_OE_REF_ENTERPRISE_PLATFORM
                    //        join b in ctx.T_OE_ORGANIZATION_ENT_SVCS on a.ENT_PLATFORM_IDX equals b.ENT_PLATFORM_IDX
                    //            into sr from x in sr.DefaultIfEmpty()  //left join
                    //        where (x == null ? true : x.ORG_IDX == OrgID)
                    //        select new OrganizationEntServicesDisplayType
                    //        {
                    //            ORG_ENT_SVCS_IDX = x.ORG_ENT_SVCS_IDX,
                    //            ORG_IDX = x.ORG_IDX,
                    //            ENT_PLATFORM_IDX = a.ENT_PLATFORM_IDX,
                    //            ENT_PLATFORM_NAME = a.ENT_PLATFORM_NAME,
                    //            PROJECT_NAME = x.PROJECT_NAME,
                    //            VENDOR = x.VENDOR,
                    //            IMPLEMENT_STATUS = x.IMPLEMENT_STATUS, 
                    //            COMMENTS = x.COMMENTS,
                    //            PROJECT_CONTACT = x.PROJECT_CONTACT,
                    //            ACTIVE_INTEREST_IND = x.ACTIVE_INTEREST_IND ?? false,
                    //            CREATE_DT = x.CREATE_DT, 
                    //            MODIFY_DT = x.MODIFY_DT
                    //        }).ToList();

                    return yyy;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<OrganizationEntServicesDisplayType> GetT_OE_ORGANIZATION_ENT_SVCS_NoLeftJoin(Guid OrgID)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.T_OE_REF_ENTERPRISE_PLATFORM
                            join b in ctx.T_OE_ORGANIZATION_ENT_SVCS on a.ENT_PLATFORM_IDX equals b.ENT_PLATFORM_IDX
                            where b.ORG_IDX == OrgID
                            select new OrganizationEntServicesDisplayType
                            {
                                ORG_ENT_SVCS_IDX = b.ORG_ENT_SVCS_IDX,
                                ORG_IDX = b.ORG_IDX,
                                ENT_PLATFORM_IDX = a.ENT_PLATFORM_IDX,
                                ENT_PLATFORM_NAME = a.ENT_PLATFORM_NAME,
                                RECORD_SOURCE = b.RECORD_SOURCE,
                                PROJECT_NAME = b.PROJECT_NAME,
                                VENDOR = b.VENDOR,
                                IMPLEMENT_STATUS = b.IMPLEMENT_STATUS,
                                COMMENTS = b.COMMENTS,
                                PROJECT_CONTACT = b.PROJECT_CONTACT,
                                ACTIVE_INTEREST_IND = b.ACTIVE_INTEREST_IND ?? false,
                                CREATE_DT = b.CREATE_DT,
                                MODIFY_DT = b.MODIFY_DT,
                                CREATE_USERIDX= b.CREATE_USERIDX,
                                MODIFY_USERIDX =b.MODIFY_USERIDX
                            }).ToList();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static OrganizationEntServicesDisplayType GetT_OE_ORGANIZATION_ENT_SVCS_ByID(int oRG_ENT_SVCS_IDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.T_OE_REF_ENTERPRISE_PLATFORM
                            join b in ctx.T_OE_ORGANIZATION_ENT_SVCS on a.ENT_PLATFORM_IDX equals b.ENT_PLATFORM_IDX
                            where b.ORG_ENT_SVCS_IDX == oRG_ENT_SVCS_IDX
                            select new OrganizationEntServicesDisplayType
                            {
                                ORG_ENT_SVCS_IDX = b.ORG_ENT_SVCS_IDX,
                                ORG_IDX = b.ORG_IDX,
                                ENT_PLATFORM_IDX = a.ENT_PLATFORM_IDX,
                                ENT_PLATFORM_NAME = a.ENT_PLATFORM_NAME,
                                ENT_PLATFORM_DESC = a.ENT_PLATFORM_DESC,
                                PROJECT_NAME = b.PROJECT_NAME,
                                VENDOR = b.VENDOR,
                                IMPLEMENT_STATUS = b.IMPLEMENT_STATUS,
                                COMMENTS = b.COMMENTS,
                                PROJECT_CONTACT = b.PROJECT_CONTACT,
                                ACTIVE_INTEREST_IND = b.ACTIVE_INTEREST_IND ?? false,
                                CREATE_USERIDX = b.CREATE_USERIDX,
                                CREATE_DT = b.CREATE_DT,
                                MODIFY_USERIDX = b.MODIFY_USERIDX,
                                MODIFY_DT = b.MODIFY_DT
                            }).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int InsertUpdatetT_OE_ORGANIZATION_ENT_SVCS(int? oRG_ENT_SVCS_IDX, Guid? oRG_IDX, int? eNT_PLATFORM_IDX, string pROJECT_NAME, 
            string vENDOR, string iMPLEMENT_STATUS, string cOMMENTS, string pROJECT_CONTACT, bool? aCTIVE_INTEREST_IND, bool? syncInd, int? cREATE_USER = 0, bool markUpdated = false)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    Boolean insInd = false;

                    T_OE_ORGANIZATION_ENT_SVCS e = (from c in ctx.T_OE_ORGANIZATION_ENT_SVCS
                                                      where c.ORG_ENT_SVCS_IDX == oRG_ENT_SVCS_IDX
                                                      select c).FirstOrDefault();

                    if (e == null)
                    {
                        e = (from c in ctx.T_OE_ORGANIZATION_ENT_SVCS
                             where c.ORG_IDX == oRG_IDX
                             && c.ENT_PLATFORM_IDX == eNT_PLATFORM_IDX
                             select c).FirstOrDefault();
                    }

                    if (e == null)
                    {
                        insInd = true;
                        e = new T_OE_ORGANIZATION_ENT_SVCS();
                        e.CREATE_DT = System.DateTime.Now;
                        e.CREATE_USERIDX = cREATE_USER;
                        e.SYNC_IND = false;
                    }
                    else
                    {
                        if (markUpdated) {
                            e.MODIFY_DT = System.DateTime.Now;
                            e.MODIFY_USERIDX = cREATE_USER;
                        }
                        if (syncInd != null) e.SYNC_IND = syncInd ?? false;
                    }
                  


                    if (oRG_IDX != null) e.ORG_IDX = oRG_IDX.ConvertOrDefault<Guid>();
                    if (eNT_PLATFORM_IDX != null) e.ENT_PLATFORM_IDX = eNT_PLATFORM_IDX ?? 0;
                    if (pROJECT_NAME != null) e.PROJECT_NAME = pROJECT_NAME;
                    if (vENDOR != null) e.VENDOR = vENDOR;
                    if (iMPLEMENT_STATUS != null) e.IMPLEMENT_STATUS = iMPLEMENT_STATUS;
                    if (cOMMENTS != null) e.COMMENTS = cOMMENTS;
                    if (pROJECT_CONTACT != null) e.PROJECT_CONTACT = pROJECT_CONTACT;
                    if (aCTIVE_INTEREST_IND != null) e.ACTIVE_INTEREST_IND = aCTIVE_INTEREST_IND;

                    if (insInd)
                        ctx.T_OE_ORGANIZATION_ENT_SVCS.Add(e);

                    ctx.SaveChanges();
                    return e.ORG_ENT_SVCS_IDX;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }

        public static int DeleteT_OE_ORGANIZATION_ENT_SVCS(int id)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    T_OE_ORGANIZATION_ENT_SVCS rec = new T_OE_ORGANIZATION_ENT_SVCS { ORG_ENT_SVCS_IDX = id };
                    ctx.Entry(rec).State = System.Data.Entity.EntityState.Deleted;
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

        public static bool ResetT_OE_ORGANIZATION_ENT_SVCS_Unsynced()
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_OE_ORGANIZATION_ENT_SVCS
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

        public static List<EECIP_Index> GetT_OE_ORGANIZATION_ENT_SVCS_ReadyToSync(int? EntSvcIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_OE_ORGANIZATION_ENT_SVCS
                               join o in ctx.T_OE_ORGANIZATION on a.ORG_IDX equals o.ORG_IDX
                               join s in ctx.T_OE_REF_STATE on o.STATE_CD equals s.STATE_CD into sr1 from x1 in sr1.DefaultIfEmpty() //left join
                               join e in ctx.T_OE_REF_ENTERPRISE_PLATFORM on a.ENT_PLATFORM_IDX equals e.ENT_PLATFORM_IDX
                               where a.SYNC_IND == false
                               && (EntSvcIDX == null ? true : a.ORG_ENT_SVCS_IDX == EntSvcIDX)
                               select new EECIP_Index
                               {
                                   Agency = o.ORG_NAME,
                                   AgencyAbbreviation = o.ORG_ABBR,
                                   State_or_Tribal = (o.ORG_TYPE == "State" ? x1.STATE_NAME : o.ORG_TYPE),
                                   KeyID = (a.ORG_ENT_SVCS_IDX + 100000).ToString(),
                                   DataType = "Enterprise Service",
                                   Record_Source = a.RECORD_SOURCE,
                                   Name = e.ENT_PLATFORM_NAME,
                                   Description = a.PROJECT_NAME,
                                   Population_Density = x1.POP_DENSITY,
                                   EPA_Region = o.EPA_REGION.ToString(),
                                   Status = a.IMPLEMENT_STATUS,
                                   LastUpdated = a.MODIFY_DT ?? a.CREATE_DT
                               }).ToList();

                    return xxx;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<SP_ENT_SVC_COUNT_DISPLAY_Result> GetT_OE_ORGANIZATION_ENT_SVCS_Overview()
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var x = (from a in ctx.SP_ENT_SVC_COUNT_DISPLAY()
                             orderby a.CNT descending
                             select a).ToList();

                    return x;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }



        //***************************PROJECTS****************************************
        public static List<T_OE_PROJECTS> GetT_OE_PROJECTS_ByOrgIDX(Guid OrgID)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_OE_PROJECTS
                               .Include(x => x.T_OE_REF_TAGS2) //media
                               .Include(x => x.T_OE_PROJECT_TAGS)//.Select(b => b.T_OE_REF_TAGS))
//                               .Include(x => x.T_OE_PROJECT_TAGS)
//                               .Where(a => a.T_OE_PROJECT_TAGS != null)
 //                           join b in ctx.T_OE_REF_TAGS on a.MEDIA_TAG equals b.TAG_IDX
 //                           into sr from x in sr.DefaultIfEmpty()  //left join
                            where a.ORG_IDX == OrgID
                            orderby a.CREATE_DT
                            select a).ToList();

                    return xxx;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }      

        public static T_OE_PROJECTS GetT_OE_PROJECTS_ByIDX(Guid? ProjectIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_OE_PROJECTS
                               .Include(x => x.T_OE_REF_TAGS)
                               .Include(x => x.T_OE_REF_TAGS1)
                               .Include(x => x.T_OE_REF_TAGS2)
                               .Include(x => x.T_OE_REF_TAGS3)
                               where a.PROJECT_IDX == ProjectIDX
                               select a).FirstOrDefault();

                    return xxx;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static T_OE_PROJECTS GetT_OE_PROJECTS_ByIMPORT_ID(string ImportID)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    if (string.IsNullOrEmpty(ImportID))
                        return null;
                    else
                        return (from a in ctx.T_OE_PROJECTS
                               where a.IMPORT_ID == ImportID
                               select a).FirstOrDefault();

                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int GetT_OE_PROJECTS_CountAddedByUserIDX(int UserIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_OE_PROJECTS
                               where a.CREATE_USERIDX == UserIDX
                               select a).Count();

                    return xxx;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }

        public static int GetT_OE_PROJECTS_CountNonGovernance()
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_OE_PROJECTS
                               join o in ctx.T_OE_ORGANIZATION on a.ORG_IDX equals o.ORG_IDX
                               where o.ORG_TYPE != "Governance"
                               select a).Count();

                    return xxx;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }

        public static int GetT_OE_PROJECTS_CountGovernance()
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_OE_PROJECTS
                               join o in ctx.T_OE_ORGANIZATION on a.ORG_IDX equals o.ORG_IDX
                               where o.ORG_TYPE == "Governance"
                               select a).Count();

                    return xxx;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }



        public static List<T_OE_PROJECTS> GetT_OE_PROJECTS_NeedingReview(int UserIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    DateTime staleDate = System.DateTime.Now.AddMonths(-6);
                    T_OE_USERS u = db_Accounts.GetT_OE_USERSByIDX(UserIDX);
                    if (u != null)
                    {
                        return (from a in ctx.T_OE_PROJECTS
                                where a.ORG_IDX == u.ORG_IDX
                                && ((a.MODIFY_DT != null ? a.MODIFY_DT < staleDate : a.CREATE_DT < staleDate)
                                || (a.PROJECT_CONTACT_IDX == null && (a.PROJECT_CONTACT == null || a.PROJECT_CONTACT == "")))
                                select a).ToList();
                    }
                    else
                        return null;

                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int GetT_OE_PROJECTS_NeedingReviewCount(int UserIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    DateTime staleDate = System.DateTime.Now.AddMonths(-6);
                    T_OE_USERS u = db_Accounts.GetT_OE_USERSByIDX(UserIDX);
                    if (u != null)
                    {
                        return (from a in ctx.T_OE_PROJECTS
                                where a.ORG_IDX == u.ORG_IDX
                                && ((a.MODIFY_DT != null ? a.MODIFY_DT < staleDate : a.CREATE_DT < staleDate)
                                || (a.PROJECT_CONTACT_IDX == null && (a.PROJECT_CONTACT == null || a.PROJECT_CONTACT == "")))
                                select a).ToList().Count();
                    }
                    else
                        return 0;

                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }


        public static List<ProjectShortDisplayType> GetT_OE_PROJECTS_RecentlyUpdatedMatchingInterest(int UserIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    List<ProjectShortDisplayType> xxx = null;

                    //get interest tags
                    List<string> user_tags = db_EECIP.GetT_OE_USER_EXPERTISE_ByUserIDX(UserIDX);

                    if (user_tags != null)
                    {
                        xxx = (from a in ctx.T_OE_PROJECTS
                               join b in ctx.T_OE_ORGANIZATION on a.ORG_IDX equals b.ORG_IDX
                               join c in ctx.T_OE_PROJECT_TAGS on a.PROJECT_IDX equals c.PROJECT_IDX
                               where user_tags.Contains(c.PROJECT_TAG_NAME)
                               orderby a.MODIFY_DT ?? a.CREATE_DT descending
                                select new ProjectShortDisplayType
                                {
                                    PROJECT_IDX = a.PROJECT_IDX,
                                    ORG_IDX = a.ORG_IDX,
                                    PROJ_NAME = a.PROJ_NAME,
                                    ORG_NAME = b.ORG_NAME,
                                    LAST_ACTIVITY_DATE = a.MODIFY_DT ?? a.CREATE_DT ?? System.DateTime.Now,
                                    TagMatch = true
                                }).Take(6).ToList();
                    }

                    if (xxx == null || xxx.Count() == 0)
                    {
                        xxx = (from a in ctx.T_OE_PROJECTS
                               join b in ctx.T_OE_ORGANIZATION on a.ORG_IDX equals b.ORG_IDX
                               orderby a.MODIFY_DT ?? a.CREATE_DT descending
                               select new ProjectShortDisplayType
                               {
                                   PROJECT_IDX = a.PROJECT_IDX,
                                   ORG_IDX = a.ORG_IDX,
                                   PROJ_NAME = a.PROJ_NAME,
                                   ORG_NAME = b.ORG_NAME,
                                   LAST_ACTIVITY_DATE = a.MODIFY_DT ?? a.CREATE_DT ?? System.DateTime.Now,
                                   TagMatch = false
                               }).Take(6).ToList();
                    }


                    return xxx;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<EECIP_Index> GetT_OE_PROJECTS_ReadyToSync(Guid? ProjectIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_OE_PROJECTS
                            join o in ctx.T_OE_ORGANIZATION on a.ORG_IDX equals o.ORG_IDX 
                            join s in ctx.T_OE_REF_STATE on o.STATE_CD equals s.STATE_CD into sr1 from x1 in sr1.DefaultIfEmpty() //left join
                            join b in ctx.T_OE_REF_TAGS on a.MEDIA_TAG equals b.TAG_IDX into sr from x in sr.DefaultIfEmpty()  //left join
                            where a.SYNC_IND == false
                            && a.ACT_IND == true
                            && (ProjectIDX == null ? true : a.PROJECT_IDX == ProjectIDX)
                            select new EECIP_Index {
                                Agency = o.ORG_NAME,
                                AgencyAbbreviation = o.ORG_ABBR,
                                State_or_Tribal = (o.ORG_TYPE == "State" ? x1.STATE_NAME : o.ORG_TYPE),
                                KeyID = a.PROJECT_IDX.ToString(),
                                DataType = (o.ORG_TYPE == "Governance" ? "Governance" : "Project"),
                                Record_Source = a.RECORD_SOURCE,
                                Name = a.PROJ_NAME,
                                Description = (a.PROJ_DESC.Length>2000) ? a.PROJ_DESC.Substring(0,2000) : a.PROJ_DESC,
                                Media = x.TAG_NAME,
                                Population_Density = x1.POP_DENSITY,
                                EPA_Region = o.EPA_REGION.ToString(),
                                Status = a.PROJ_STATUS,
                                LastUpdated = a.MODIFY_DT ?? a.CREATE_DT
                            }).Take(50).ToList();

                    foreach (EECIP_Index e in xxx)
                        e.Tags = GetT_OE_PROJECT_TAGS_ByTwoAttributeSelected(new Guid(e.KeyID), "Project Feature", "Program Area").ToArray();

                    return xxx;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static bool ResetT_OE_PROJECTS_Unsynced()
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_OE_PROJECTS
                               where a.ACT_IND == true
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

        public static Guid? InsertUpdatetT_OE_PROJECTS(Guid? pROJECT_IDX, Guid? oRG_IDX, string pROJ_NAME, string pROJ_DESC, int? mEDIA_TAG, int? sTART_YEAR,
            string pROJ_STATUS, int? dATE_LAST_UPDATE, string rECORD_SOURCE, string pROJECT_URL, int? mOBILE_IND, string mOBILE_DESC, int? aDV_MON_IND, 
            string aDV_MON_DESC, int? bP_MODERN_IND, string bP_MODERN_DESC, string cOTS, string vENDOR, string pROJECT_CONTACT, int? pROJECT_CONTACT_IDX, bool aCT_IND, bool? sYNC_IND, int? cREATE_USER = 0,
            string iMPORT_ID = null, bool markUpdated = false)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    Boolean insInd = false;

                    T_OE_PROJECTS e = (from c in ctx.T_OE_PROJECTS
                                       where c.PROJECT_IDX == pROJECT_IDX
                                       select c).FirstOrDefault();

                    if (e == null)
                    {
                        insInd = true;
                        e = new T_OE_PROJECTS();
                        e.PROJECT_IDX = Guid.NewGuid();
                        e.CREATE_DT = System.DateTime.Now;
                        e.CREATE_USERIDX = cREATE_USER;

                        //set project contact to person who created it
                        if (e.PROJECT_CONTACT_IDX == null)
                            e.PROJECT_CONTACT_IDX = cREATE_USER;
                    }
                    else
                    {
                        if (markUpdated)
                        {
                            e.MODIFY_DT = System.DateTime.Now;
                            e.MODIFY_USERIDX = cREATE_USER;
                        }
                    }

                    if (oRG_IDX != null) e.ORG_IDX = oRG_IDX.ConvertOrDefault<Guid>();
                    if (pROJ_NAME != null) e.PROJ_NAME = pROJ_NAME;
                    if (pROJ_DESC != null) e.PROJ_DESC = pROJ_DESC;
                    if (mEDIA_TAG != null) e.MEDIA_TAG = mEDIA_TAG;
                    if (sTART_YEAR != null) e.START_YEAR = sTART_YEAR;
                    if (sTART_YEAR == -1) e.START_YEAR = null;  //handling blanking out
                    if (pROJ_STATUS != null) e.PROJ_STATUS = pROJ_STATUS;
                    if (dATE_LAST_UPDATE != null) e.DATE_LAST_UPDATE = dATE_LAST_UPDATE;
                    if (dATE_LAST_UPDATE == -1) e.DATE_LAST_UPDATE = null; //handling blanking out
                    if (rECORD_SOURCE != null) e.RECORD_SOURCE = rECORD_SOURCE;
                    if (e.RECORD_SOURCE == null)
                        e.RECORD_SOURCE = "Agency supplied";
                    if (pROJECT_URL != null) e.PROJECT_URL = pROJECT_URL;
                    if (mOBILE_IND != null) e.MOBILE_IND = mOBILE_IND;
                    if (mOBILE_DESC != null) e.MOBILE_DESC = mOBILE_DESC;
                    if (aDV_MON_IND != null) e.ADV_MON_IND = aDV_MON_IND;
                    if (aDV_MON_DESC != null) e.ADV_MON_DESC = aDV_MON_DESC;
                    if (bP_MODERN_IND != null) e.BP_MODERN_IND = bP_MODERN_IND;
                    if (bP_MODERN_DESC != null) e.BP_MODERN_DESC = bP_MODERN_DESC;
                    if (cOTS != null) e.COTS = cOTS;
                    if (vENDOR != null) e.VENDOR = vENDOR;
                    if (pROJECT_CONTACT != null) e.PROJECT_CONTACT = pROJECT_CONTACT;
                    if (pROJECT_CONTACT_IDX != null) e.PROJECT_CONTACT_IDX = pROJECT_CONTACT_IDX;
                    if (pROJECT_CONTACT_IDX == -1) e.PROJECT_CONTACT_IDX = null; //handling blanking out

                    e.ACT_IND = aCT_IND;
                    if (sYNC_IND != null) e.SYNC_IND = sYNC_IND ?? false;
                    if (iMPORT_ID != null) e.IMPORT_ID = iMPORT_ID;

                    if (insInd)
                        ctx.T_OE_PROJECTS.Add(e);

                    ctx.SaveChanges();
                    return e.PROJECT_IDX;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int DeleteT_OE_PROJECTS(Guid id)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    T_OE_PROJECTS rec = new T_OE_PROJECTS { PROJECT_IDX = id };
                    ctx.Entry(rec).State = System.Data.Entity.EntityState.Deleted;
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

        public static List<T_OE_PROJECT_URLS> GetT_OE_PROJECTS_URL_ByProjIDX(Guid ProjectIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_OE_PROJECT_URLS
                               where a.PROJECT_IDX == ProjectIDX
                               select a).ToList();

                    return xxx;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int InsertT_OE_PROJECTS_URL(Guid pROJECT_IDX, string uRL, string pROJECT_URL_DESC)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    T_OE_PROJECT_URLS e = new T_OE_PROJECT_URLS();
                    e.PROJECT_URL_IDX = Guid.NewGuid();
                    e.PROJECT_IDX = pROJECT_IDX;
                    e.PROJECT_URL = uRL;
                    e.PROJ_URL_DESC = pROJECT_URL_DESC;                   

                    ctx.T_OE_PROJECT_URLS.Add(e);

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

        public static int DeleteT_OE_PROJECTS_URL(Guid ProjectID)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    ctx.Database.ExecuteSqlCommand("DELETE FROM T_OE_PROJECT_URLS where PROJECT_IDX = '" + ProjectID + "'");

                    return 1;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }

        //***************************project local (temp when importing)****************************************
        /// <summary>
        /// Creates a new local PROJECT record and validates it according to the validation rules contained in XML file
        /// </summary>
        /// <param name="UserIDX"></param>
        /// <param name="colVals">Name value pair for the different fields to import into the project record</param>
        /// <returns></returns>
        public static ProjectImportType InsertOrUpdate_T_OE_PROJECT_local(int UserIDX, Dictionary<string, string> colVals)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    ProjectImportType e = new ProjectImportType();

                    //determine if new Project record or updating existing one
                    Boolean insInd = true;

                    //try to get existing project based on PROJECT_IDX
                    Guid projIDX;
                    string ProjIDXStr = Utils.GetValueOrDefault(colVals, "PROJECT_IDX");
                    if (Guid.TryParse(ProjIDXStr, out projIDX))
                    {
                        T_OE_PROJECTS p = db_EECIP.GetT_OE_PROJECTS_ByIDX(projIDX);
                        if (p != null)
                        {
                            insInd = false;
                            e.T_OE_PROJECT.PROJECT_IDX = p.PROJECT_IDX;
                        }
                    }

                    //then try to get based on supplied IMPORT_ID 
                    T_OE_PROJECTS p2 = db_EECIP.GetT_OE_PROJECTS_ByIMPORT_ID(Utils.GetValueOrDefault(colVals, "IMPORT_ID"));
                    if (p2 != null)
                    {
                        insInd = false;
                        e.T_OE_PROJECT.PROJECT_IDX = p2.PROJECT_IDX;
                    }

                    if (insInd)
                    {
                        e.T_OE_PROJECT.PROJECT_IDX = Guid.NewGuid();
                        e.T_OE_PROJECT.CREATE_DT = System.DateTime.Now;
                        e.T_OE_PROJECT.CREATE_USERIDX = UserIDX;
                    }
                    else
                    {
                        e.T_OE_PROJECT.MODIFY_DT = System.DateTime.Now;
                        e.T_OE_PROJECT.MODIFY_USERIDX = UserIDX;
                    }


                    //get import config rules
                    List<ConfigInfoType> _allRules = Utils.GetAllColumnInfo("P");

                    //explicitly validate mandatory fields

                    foreach (string entry in Utils.GetMandatoryImportFieldList("P"))
                        T_OE_PROJECT_validate(ref e, _allRules, colVals, entry);

                    //then only validate optional fields if supplied (performance)
                    foreach (string entry in Utils.GetOptionalImportFieldList("P"))
                        T_OE_PROJECT_validate(ref e, _allRules, colVals, entry);

                    //********************** CUSTOM POST VALIDATION ********************************************
                    //SET ORG_IDX based on supplied ORG_NAME
                    e.ORG_NAME = Utils.GetValueOrDefault(colVals, "ORG_NAME");
                    T_OE_ORGANIZATION oo = db_Ref.GetT_OE_ORGANIZATION_ByName(e.ORG_NAME);
                    if (oo != null)
                        e.T_OE_PROJECT.ORG_IDX = oo.ORG_IDX;
                    else
                    {
                        e.VALIDATE_CD = false;
                        e.VALIDATE_MSG += "No matching agency found.";
                    }


                    //MEDIA
                    e.MEDIA_NAME = Utils.GetValueOrDefault(colVals, "MEDIA_TAG");
                    if (!string.IsNullOrEmpty(e.MEDIA_NAME))
                    {
                        T_OE_REF_TAGS media1 = db_Ref.GetT_OE_REF_TAGS_ByCategoryAndName("Project Media", e.MEDIA_NAME);
                        if (media1 != null)
                            e.T_OE_PROJECT.MEDIA_TAG = media1.TAG_IDX;
                        else
                        {
                            e.VALIDATE_CD = false;
                            e.VALIDATE_MSG += "Invalid Media name.";
                        }
                    }

                    //MOBILE
                    e.MOBILE_IND_NAME = Utils.GetValueOrDefault(colVals, "MOBILE_IND");
                    if (!string.IsNullOrEmpty(e.MOBILE_IND_NAME))
                    {
                        T_OE_REF_TAGS mobile1 = db_Ref.GetT_OE_REF_TAGS_ByCategoryAndName("Use Amount", e.MOBILE_IND_NAME);
                        if (mobile1 != null)
                            e.T_OE_PROJECT.MOBILE_IND = mobile1.TAG_IDX;
                        else
                        {
                            e.VALIDATE_CD = false;
                            e.VALIDATE_MSG += "Invalid Mobile Use. ";
                        }
                    }

                    //ADV MON
                    e.ADV_MON_IND_NAME = Utils.GetValueOrDefault(colVals, "ADV_MON_IND");
                    if (!string.IsNullOrEmpty(e.ADV_MON_IND_NAME))
                    {
                        T_OE_REF_TAGS adv1 = db_Ref.GetT_OE_REF_TAGS_ByCategoryAndName("Use Amount", e.ADV_MON_IND_NAME);
                        if (adv1 != null)
                            e.T_OE_PROJECT.ADV_MON_IND = adv1.TAG_IDX;
                        else
                        {
                            e.VALIDATE_CD = false;
                            e.VALIDATE_MSG += "Invalid Advanced Monitoring Use. ";
                        }
                    }

                    //BP MODERN
                    e.BP_MODERN_IND_NAME = Utils.GetValueOrDefault(colVals, "BP_MODERN_IND");
                    if (!string.IsNullOrEmpty(e.BP_MODERN_IND_NAME))
                    {
                        T_OE_REF_TAGS bp1 = db_Ref.GetT_OE_REF_TAGS_ByCategoryAndName("Use Amount", e.BP_MODERN_IND_NAME);
                        if (bp1 != null)
                            e.T_OE_PROJECT.BP_MODERN_IND = bp1.TAG_IDX;
                        else
                        {
                            e.VALIDATE_CD = false;
                            e.VALIDATE_MSG += "Invalid Business Process Improvement Use. ";
                        }
                    }


                    e.PROGRAM_AREAS = Utils.GetValueOrDefault(colVals, "PROGRAM_AREAS");
                    e.FEATURES = Utils.GetValueOrDefault(colVals, "FEATURES");
                    //********************** CUSTOM POST VALIDATION END ********************************************

                    return e;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static void T_OE_PROJECT_validate(ref ProjectImportType a, List<ConfigInfoType> t, Dictionary<string, string> colVals, string f)
        {
            var _rules = t.Find(item => item._name == f);   //import validation rules for this field
            if (_rules == null)
                return;

            string _value = Utils.GetValueOrDefault(colVals, f); //supplied value for this field

            if (!string.IsNullOrEmpty(_value)) //if value is supplied
            {
                _value = _value.Trim();

                //strings: field length validation and substring 
                if (_rules._datatype == "" && _rules._length != null)
                {
                    if (_value.Length > _rules._length)
                    {
                        a.VALIDATE_CD = false;
                        a.VALIDATE_MSG = (a.VALIDATE_MSG + f + " length (" + _rules._length + ") exceeded. ");
                        _value = _value.SubStringPlus(0, (int)_rules._length);
                    }
                }

                //integers: check type
                if (_rules._datatype == "int")
                {
                    int n;
                    if (int.TryParse(_value, out n) == false)
                    {
                        a.VALIDATE_CD = false;
                        a.VALIDATE_MSG = (a.VALIDATE_MSG + f + " not numeric. ");
                    }
                }

            }
            else
            {
                //required check
                if (_rules._req == "Y")
                {
                    if (_rules._datatype == "")
                        _value = "-";
                    a.VALIDATE_CD = false;
                    a.VALIDATE_MSG = (a.VALIDATE_MSG + "Required field " + f + " missing. ");
                }
            }

            //finally set the value before returning
            try
            {
                if (_rules._datatype == "")
                    typeof(T_OE_PROJECTS).GetProperty(f).SetValue(a.T_OE_PROJECT, _value);
                else if (_rules._datatype == "int")
                    typeof(T_OE_PROJECTS).GetProperty(f).SetValue(a.T_OE_PROJECT, _value.ConvertOrDefault<int?>());
            }
            catch {
                //let fail silently (non T_OE_PROJECTS fields will fail)
            }
        }



        //***************************PROJECT TAGS****************************************
        public static List<string> GetT_OE_PROJECT_TAGS_ByAttributeSelected(Guid ProjectIDX, string aTTRIBUTE)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.T_OE_PROJECT_TAGS
                            where a.PROJECT_IDX == ProjectIDX
                            && a.PROJECT_ATTRIBUTE == aTTRIBUTE
                            select a.PROJECT_TAG_NAME.ToString()).ToList();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<string> GetT_OE_PROJECT_TAGS_ByTwoAttributeSelected(Guid ProjectIDX, string aTTRIBUTE, string aTTRIBUTE2)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_OE_PROJECT_TAGS
                            where a.PROJECT_IDX == ProjectIDX
                            && (a.PROJECT_ATTRIBUTE == aTTRIBUTE || a.PROJECT_ATTRIBUTE == aTTRIBUTE2)
                            select a.PROJECT_TAG_NAME.ToString()).ToList();
                    return xxx;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<string> GetT_OE_PROJECT_TAGS_ByAttributeAll(Guid ProjectIDX, string aTTRIBUTE)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var xx1 = (from a in ctx.T_OE_PROJECT_TAGS
                            where a.PROJECT_IDX == ProjectIDX
                            && a.PROJECT_ATTRIBUTE == aTTRIBUTE
                            select a.PROJECT_TAG_NAME);

                    var xx2 = (from a in ctx.T_OE_REF_TAGS
                               where a.TAG_CAT_NAME == aTTRIBUTE
                               select a.TAG_NAME);

                    return xx1.Union(xx2).ToList();

                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int InsertT_OE_PROJECT_TAGS(Guid pROJECT_IDX, string aTTRIBUTE, string pROJECT_TAG_NAME, int? cREATE_USER = 0)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    T_OE_PROJECT_TAGS e = new T_OE_PROJECT_TAGS();
                    e.PROJECT_IDX = pROJECT_IDX;
                    e.PROJECT_ATTRIBUTE= aTTRIBUTE;
                    e.PROJECT_TAG_NAME = pROJECT_TAG_NAME;
                    e.CREATE_DT = System.DateTime.Now;
                    e.CREATE_USERIDX = cREATE_USER;

                    ctx.T_OE_PROJECT_TAGS.Add(e);

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

        public static int DeleteT_OE_PROJECT_TAGS(Guid ProjectID, string aTTRIBUTE)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    ctx.Database.ExecuteSqlCommand("DELETE FROM T_OE_PROJECT_TAGS where PROJECT_IDX = '" + ProjectID + "' and PROJECT_ATTRIBUTE = '" + aTTRIBUTE + "'");

                    return 1;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }



        //***************************PROJECT VOTES****************************************
        public static int GetT_OE_PROJECT_VOTES_TotalByProject(Guid ProjectIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.T_OE_PROJECT_VOTES
                            where a.PROJECT_IDX == ProjectIDX
                            select a.VOTE_AMOUNT).Sum();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }

        public static int GetT_OE_PROJECT_VOTES_TotalByUser(int UserIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.T_OE_PROJECT_VOTES
                            where a.VOTED_BY_USER_IDX == UserIDX
                            select a.VOTE_AMOUNT).Sum();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }

        public static bool GetT_OE_PROJECT_VOTES_HasVoted(Guid ProjectIDX, int UserIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.T_OE_PROJECT_VOTES
                            where a.PROJECT_IDX == ProjectIDX
                            && a.VOTED_BY_USER_IDX == UserIDX
                            select a).ToList().Count() > 0;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return true;
                }
            }
        }

        public static Guid? InsertT_OE_PROJECT_VOTES(Guid pROJECT_IDX, int vOTED_BY_USER_IDX, int vOTE_AMOUNT)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    bool insInd = false;

                    T_OE_PROJECT_VOTES e = (from c in ctx.T_OE_PROJECT_VOTES
                                       where c.PROJECT_IDX == pROJECT_IDX
                                       && c.VOTED_BY_USER_IDX == vOTED_BY_USER_IDX
                                       select c).FirstOrDefault();

                    if (e == null)
                    {
                        insInd = true;
                        e = new T_OE_PROJECT_VOTES();
                        e.PROJECT_VOTE_IDX = Guid.NewGuid();
                        e.PROJECT_IDX = pROJECT_IDX;
                        e.VOTED_BY_USER_IDX = vOTED_BY_USER_IDX;
                    }

                    e.DATE_VOTED = System.DateTime.UtcNow;
                    e.VOTE_AMOUNT = vOTE_AMOUNT;

                    if (insInd)
                        ctx.T_OE_PROJECT_VOTES.Add(e);
                    ctx.SaveChanges();

                    return e.PROJECT_VOTE_IDX;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int DeleteT_OE_PROJECT_VOTE(Guid pROJECT_IDX, int vOTED_BY_USER_IDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    T_OE_PROJECT_VOTES rec = (from a in ctx.T_OE_PROJECT_VOTES
                                              where a.PROJECT_IDX == pROJECT_IDX
                                              && a.VOTED_BY_USER_IDX == vOTED_BY_USER_IDX
                                              select a).FirstOrDefault();

                    ctx.Entry(rec).State = System.Data.Entity.EntityState.Deleted;
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


        //*******************************DOCUMENTS *********************************************
        public static Guid? InsertUpdateT_OE_DOCUMENTS(Guid? dOC_IDX, byte[] dOC_CONTENT, string dOC_NAME, string dOC_TYPE, string dOC_FILE_TYPE, int? dOC_SIZE, string dOC_COMMENT, 
            string dOC_AUTHOR, Guid? pROJECT_IDX, int? oRG_ENT_SVCS_IDX, int? UserIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    Boolean insInd = false;

                    T_OE_DOCUMENTS e = (from c in ctx.T_OE_DOCUMENTS
                                        where c.DOC_IDX == dOC_IDX
                                        select c).FirstOrDefault();

                    //insert case
                    if (e == null)
                    {
                        insInd = true;
                        e = new T_OE_DOCUMENTS();
                        e.DOC_IDX = Guid.NewGuid();
                        e.CREATE_DT = System.DateTime.UtcNow;
                        e.CREATE_USERIDX = UserIDX ?? 0;
                    }
                    else
                    {
                        e.MODIFY_DT = System.DateTime.UtcNow;
                        e.MODIFY_USERIDX = UserIDX ?? 0;
                    }

                    if (dOC_CONTENT != null) e.DOC_CONTENT = dOC_CONTENT;
                    if (dOC_NAME != null) e.DOC_NAME = dOC_NAME;
                    if (dOC_TYPE != null) e.DOC_TYPE = dOC_TYPE;
                    if (dOC_FILE_TYPE != null) e.DOC_FILE_TYPE = dOC_FILE_TYPE;
                    if (dOC_SIZE != null) e.DOC_SIZE = dOC_SIZE;
                    if (dOC_COMMENT != null) e.DOC_COMMENT = dOC_COMMENT;
                    if (dOC_AUTHOR != null) e.DOC_AUTHOR = dOC_AUTHOR;
                    if (pROJECT_IDX != null) e.PROJECT_IDX = pROJECT_IDX;
                    if (oRG_ENT_SVCS_IDX != null) e.ORG_ENT_SVCS_IDX = oRG_ENT_SVCS_IDX;

                    if (insInd)
                        ctx.T_OE_DOCUMENTS.Add(e);

                    ctx.SaveChanges();
                    return e.DOC_IDX;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<T_OE_DOCUMENTS> GetT_OE_DOCUMENTS_ByProjectID(Guid ProjectIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.T_OE_DOCUMENTS
                            where a.PROJECT_IDX == ProjectIDX
                            select a).ToList();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static T_OE_DOCUMENTS GetT_OE_DOCUMENTS_ByID(Guid DocIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.T_OE_DOCUMENTS
                            where a.DOC_IDX == DocIDX
                            select a).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int DeleteT_OE_DOCUMENTS(Guid DocIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    T_OE_DOCUMENTS rec = (from a in ctx.T_OE_DOCUMENTS
                                    where a.DOC_IDX == DocIDX
                                    select a).FirstOrDefault();

                    ctx.Entry(rec).State = System.Data.Entity.EntityState.Deleted;
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



        //******************************* NOTIFICATIONS ***************************************
        public static List<T_OE_USER_NOTIFICATION> GetT_OE_USER_NOTIFICATION_byUserIDX(int? UserIDX, bool OnlyUnread)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    if (UserIDX == null) return null;

                    return (from a in ctx.T_OE_USER_NOTIFICATION
                            where a.USER_IDX == UserIDX
                            && (OnlyUnread == true ? a.READ_IND == false : true)
                            orderby a.CREATE_DT descending
                            select a).ToList();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static T_OE_USER_NOTIFICATION GetT_OE_USER_NOTIFICATION_byID(Guid? Id)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.T_OE_USER_NOTIFICATION
                            where a.NOTIFICATION_IDX == Id
                            select a).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int DeleteT_OE_NOTIFICATION(Guid id)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    T_OE_USER_NOTIFICATION rec = new T_OE_USER_NOTIFICATION { NOTIFICATION_IDX = id };
                    ctx.Entry(rec).State = System.Data.Entity.EntityState.Deleted;
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

        public static Guid? InsertUpdateT_OE_USER_NOTIFICATION(Guid? nOTIFICATION_IDX, int uSER_IDX, DateTime? nOTIFY_DT, string nOTIFY_TYPE, string nOTIFY_TITLE,
            string nOTIFY_DESC, Boolean rEAD_IND, int? fROM_USER_IDX, Boolean aCT_IND, int? cREATE_USER, bool SendEmailInd)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    Boolean insInd = false;

                    T_OE_USER_NOTIFICATION e = (from c in ctx.T_OE_USER_NOTIFICATION
                                                where c.NOTIFICATION_IDX == nOTIFICATION_IDX
                                                select c).FirstOrDefault();

                    if (e == null)
                    {
                        insInd = true;
                        e = new T_OE_USER_NOTIFICATION();
                        e.NOTIFICATION_IDX = Guid.NewGuid();
                        e.CREATE_DT = System.DateTime.Now;
                        e.CREATE_USERIDX = cREATE_USER;
                    }
                    else
                    {
                        e.MODIFY_DT = System.DateTime.Now;
                        e.MODIFY_USERIDX = cREATE_USER;
                    }

                    e.USER_IDX = uSER_IDX;
                    if (nOTIFY_DT != null) e.NOTIFY_DT = nOTIFY_DT.ConvertOrDefault<DateTime>();
                    if (nOTIFY_TYPE != null) e.NOTIFY_TYPE = nOTIFY_TYPE;
                    if (nOTIFY_TITLE != null) e.NOTIFY_TITLE = nOTIFY_TITLE;
                    if (nOTIFY_DESC != null) e.NOTIFY_DESC = nOTIFY_DESC;
                    e.READ_IND = rEAD_IND;
                    if (fROM_USER_IDX != null) e.FROM_USER_IDX = fROM_USER_IDX;

                    if (insInd)
                        ctx.T_OE_USER_NOTIFICATION.Add(e);

                    ctx.SaveChanges();

                    //send email notification too
                    if (SendEmailInd)
                    {
                        T_OE_USERS u = db_Accounts.GetT_OE_USERSByIDX(uSER_IDX);
                        if (u != null)
                        {
                            Utils.SendEmail(db_Ref.GetT_OE_APP_SETTING("EMAIL_FROM"), u.EMAIL, null, null, nOTIFY_TITLE, nOTIFY_DESC, null, null, nOTIFY_DESC);
                        }
                    }

                    return e.NOTIFICATION_IDX;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

    }
}