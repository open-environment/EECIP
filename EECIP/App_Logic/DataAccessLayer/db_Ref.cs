using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using EECIP.App_Logic.BusinessLogicLayer;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace EECIP.App_Logic.DataAccessLayer
{
    public class db_Ref
    {

        //*****************APP SETTINGS**********************************
        public static string GetT_OE_APP_SETTING(string settingName)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.T_OE_APP_SETTINGS
                            where a.SETTING_NAME == settingName
                            select a).FirstOrDefault().SETTING_VALUE;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return "";
                }
            }
        }

        public static List<T_OE_APP_SETTINGS> GetT_OE_APP_SETTING_List()
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.T_OE_APP_SETTINGS
                            orderby a.SETTING_IDX
                            select a).ToList();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int InsertUpdateT_OE_APP_SETTING(int sETTING_IDX, string sETTING_NAME, string sETTING_VALUE, bool? eNCRYPT_IND, string sETTING_VALUE_SALT, int? cREATE_USER = 0)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    Boolean insInd = false;

                    T_OE_APP_SETTINGS e = (from c in ctx.T_OE_APP_SETTINGS
                                           where c.SETTING_IDX == sETTING_IDX
                                           select c).FirstOrDefault();

                    if (e == null)
                    {
                        insInd = true;
                        e = new T_OE_APP_SETTINGS();
                    }

                    if (sETTING_NAME != null) e.SETTING_NAME = sETTING_NAME;
                    if (sETTING_VALUE != null) e.SETTING_VALUE = sETTING_VALUE;

                    e.MODIFY_DT = System.DateTime.Now;
                    e.MODIFY_USERIDX = cREATE_USER;

                    if (insInd)
                        ctx.T_OE_APP_SETTINGS.Add(e);

                    ctx.SaveChanges();
                    return e.SETTING_IDX;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }


        //*****************APP SETTINGS_CUSTOM**********************************
        public static T_OE_APP_SETTINGS_CUSTOM GetT_OE_APP_SETTING_CUSTOM()
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.T_OE_APP_SETTINGS_CUSTOM
                            select a).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int InsertUpdateT_OE_APP_SETTING_CUSTOM(string tERMS_AND_CONDITIONS, string aNNOUNCEMENTS)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    Boolean insInd = false;

                    T_OE_APP_SETTINGS_CUSTOM e = (from c in ctx.T_OE_APP_SETTINGS_CUSTOM
                                           select c).FirstOrDefault();

                    if (e == null)
                    {
                        insInd = true;
                        e = new T_OE_APP_SETTINGS_CUSTOM();
                    }

                    if (tERMS_AND_CONDITIONS != null) e.TERMS_AND_CONDITIONS = Utils.GetSafeHtml(tERMS_AND_CONDITIONS);
                    if (aNNOUNCEMENTS != null) e.ANNOUNCEMENTS = Utils.GetSafeHtml(aNNOUNCEMENTS);

                    if (insInd)
                        ctx.T_OE_APP_SETTINGS_CUSTOM.Add(e);

                    ctx.SaveChanges();
                    return e.SETTING_CUSTOM_IDX;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }



        //***************** ORGANZIATIONS ******************************
        public static List<T_OE_ORGANIZATION> GetT_OE_ORGANIZATION(bool actInd, bool ExcludeGovernanceInd)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.T_OE_ORGANIZATION
                            where (actInd == true ? a.ACT_IND == true : true)
                            && (ExcludeGovernanceInd == true ? a.ORG_TYPE != "Governance" : true)
                            orderby a.ORG_NAME
                            select a).ToList();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<T_OE_ORGANIZATION> GetT_OE_ORGANIZATIONS_ByType(string oRG_TYPE)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.T_OE_ORGANIZATION
                            where a.ORG_TYPE == oRG_TYPE
                            && a.ACT_IND == true
                            orderby a.ORG_NAME
                            select a).ToList();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int GetT_OE_ORGANIZATION_Count()
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.T_OE_ORGANIZATION
                            where a.ORG_TYPE != "Governance"
                            select a).Count();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }



        public static T_OE_ORGANIZATION GetT_OE_ORGANIZATION_ByID(Guid id)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.T_OE_ORGANIZATION
                            where a.ORG_IDX == id
                            select a).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static T_OE_ORGANIZATION GetT_OE_ORGANIZATION_ByName(string name)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.T_OE_ORGANIZATION
                            where a.ORG_NAME.ToUpper() == name.ToUpper()
                            select a).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<T_OE_ORGANIZATION> GetT_OE_ORGANIZATION_ByEmail(string email)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var domain = Regex.Match(email, "@(.*)").Groups[1].Value;

                    return (from a in ctx.T_OE_ORGANIZATION
                               join b in ctx.T_OE_ORGANIZATION_EMAIL_RULE on a.ORG_IDX equals b.ORG_IDX
                               where b.EMAIL_STRING.ToUpper() == domain.ToUpper()
                               select a).ToList();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    throw ex;
                }
            }
        }

        public static Guid? InsertUpdatetT_OE_ORGANIZATION(Guid? oRG_IDX, string oRG_ABBR, string oRG_NAME, string sTATE_CD, int? ePA_REGION, string oRG_TYPE,
            string cLOUD, string aPI, bool? aCT_IND, int? cREATE_USER = 0)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    Boolean insInd = false;

                    T_OE_ORGANIZATION e = (from c in ctx.T_OE_ORGANIZATION
                                           where c.ORG_IDX == oRG_IDX
                                           select c).FirstOrDefault();

                    if (e == null)
                    {
                        insInd = true;
                        e = new T_OE_ORGANIZATION();
                        e.ORG_IDX = Guid.NewGuid();
                        e.CREATE_DT = System.DateTime.Now;
                        e.CREATE_USERIDX = cREATE_USER;
                    }
                    else
                    {
                        e.MODIFY_DT = System.DateTime.Now;
                        e.MODIFY_USERIDX = cREATE_USER;
                    }

                    if (oRG_ABBR != null) e.ORG_ABBR = oRG_ABBR;
                    if (e.ORG_ABBR == null) e.ORG_ABBR = "";
                    if (oRG_NAME != null) e.ORG_NAME = oRG_NAME;
                    if (sTATE_CD != null) e.STATE_CD = sTATE_CD;
                    if (ePA_REGION != null) e.EPA_REGION = ePA_REGION;
                    if (oRG_TYPE != null) e.ORG_TYPE = oRG_TYPE;
                    if (cLOUD != null) e.CLOUD = cLOUD;
                    if (aPI != null) e.API = aPI;
                    if (aCT_IND != null) e.ACT_IND = aCT_IND ?? true;


                    if (insInd)
                        ctx.T_OE_ORGANIZATION.Add(e);

                    ctx.SaveChanges();
                    return e.ORG_IDX;
                }
                catch (Exception ex)
                {
                    LogEFException(ex);
                    return null;
                }
            }
        }

        public static bool ResetT_OE_ORGANIZATION_Unsynced()
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_OE_ORGANIZATION
                               where a.ACT_IND == true
                               && a.SYNC_IND == true
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

        public static List<EECIP_Index> GetT_OE_ORGANIZATION_ReadyToSync(Guid? OrgIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_OE_ORGANIZATION
                               join s in ctx.T_OE_REF_STATE on a.STATE_CD equals s.STATE_CD into sr1 from x1 in sr1.DefaultIfEmpty() //left join
                               where a.SYNC_IND == false
                               && a.ORG_TYPE != "Governance"
                               && a.ACT_IND == true
                               && (OrgIDX == null ? true : a.ORG_IDX == OrgIDX)
                               select new EECIP_Index
                               {
                                   Agency = a.ORG_NAME,
                                   AgencyAbbreviation = a.ORG_ABBR,
                                   State_or_Tribal = (a.ORG_TYPE == "State" ? x1.STATE_NAME : a.ORG_TYPE),
                                   KeyID = a.ORG_IDX.ToString(),
                                   DataType = "Agency",
                                   Record_Source = "EECIP Supplied",
                                   Name = a.ORG_NAME,
                                   Description = a.ORG_ABBR,
                                   Population_Density = x1.POP_DENSITY,
                                   EPA_Region = a.EPA_REGION.ToString(),
                                   LastUpdated = a.MODIFY_DT ?? a.CREATE_DT
                               }).ToList();

                    foreach (EECIP_Index e in xxx)
                        e.Tags = GetT_OE_ORGANIZATION_TAGS_ByOrg(new Guid(e.KeyID)).ToArray();

                    return xxx;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int DeleteT_OE_ORGANIZATION(Guid id)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    //***************** VALIDATION (DONT DELETE ORG IF ANY USERS) **********************
                    List<T_OE_USERS> u = db_Accounts.GetT_OE_USERSByAgency(id);
                    if (u != null && u.Count > 0)
                        return -1;

                    List<T_OE_PROJECTS> p = db_EECIP.GetT_OE_PROJECTS_ByOrgIDX(id);
                    if (p != null && p.Count > 0)
                        return -2;

                    List<OrganizationEntServicesDisplayType> es = db_EECIP.GetT_OE_ORGANIZATION_ENT_SVCS_NoLeftJoin(id);
                    if (es != null && es.Count > 0)
                        return -3;

                    T_OE_ORGANIZATION rec = new T_OE_ORGANIZATION { ORG_IDX = id };
                    ctx.Entry(rec).State = System.Data.Entity.EntityState.Deleted;
                    ctx.SaveChanges();

                    return 1;
                }
                catch (Exception ex)
                {
                    LogEFException(ex);
                    return 0;
                }
            }
        }


        //***************** ORGANZIATION TAGS ******************************
        public static List<string> GetT_OE_ORGANIZATION_TAGS_ByOrgAttribute(Guid OrgIDX, string aTTRIBUTE)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.T_OE_ORGANIZATION_TAGS
                            where a.ORG_IDX == OrgIDX
                            && a.ORG_ATTRIBUTE== aTTRIBUTE
                            select a.ORG_TAG).ToList();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<string> GetT_OE_ORGANIZATION_TAGS_ByOrg(Guid OrgIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.T_OE_ORGANIZATION_TAGS
                            where a.ORG_IDX == OrgIDX
                            select a.ORG_TAG).ToList();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<string> GetT_OE_ORGANIZATION_TAGS_ByAttributeAll(Guid OrgIDX, string aTTRIBUTE)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var xx1 = (from a in ctx.T_OE_ORGANIZATION_TAGS
                               where a.ORG_IDX == OrgIDX
                               && a.ORG_ATTRIBUTE == aTTRIBUTE
                               select a.ORG_TAG);

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

        public static int InsertT_OE_ORGANIZATION_TAGS(Guid oRG_IDX, string aTTRIBUTE, string oRG_TAG, int? cREATE_USER = 0)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    T_OE_ORGANIZATION_TAGS e = new T_OE_ORGANIZATION_TAGS();
                    e.ORG_IDX = oRG_IDX;
                    e.ORG_ATTRIBUTE = aTTRIBUTE;
                    e.ORG_TAG = oRG_TAG;
                    e.CREATE_DT = System.DateTime.Now;
                    e.CREATE_USERIDX = cREATE_USER;

                    ctx.T_OE_ORGANIZATION_TAGS.Add(e);

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

        public static int DeleteT_OE_ORGANIZATION_TAGS(Guid OrgID, string aTTRIBUTE)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    ctx.Database.ExecuteSqlCommand("DELETE FROM T_OE_ORGANIZATION_TAGS where ORG_IDX = '" + OrgID + "' and ORG_ATTRIBUTE = '" + aTTRIBUTE + "'");

                    return 1;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }



        //***************** ORGANZIATION EMAIL RULE ******************************
        public static List<T_OE_ORGANIZATION_EMAIL_RULE> GetT_OE_ORGANIZATION_EMAIL_RULES_ByID(Guid id)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.T_OE_ORGANIZATION_EMAIL_RULE
                            where a.ORG_IDX == id
                            select a).ToList();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int InsertT_OE_ORGANIZATION_EMAIL_RULE(Guid oRG_IDX, string eMAIL_STRING, int? cREATE_USER = 0)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {

                    T_OE_ORGANIZATION_EMAIL_RULE e = new T_OE_ORGANIZATION_EMAIL_RULE();
                    e.ORG_IDX = oRG_IDX;
                    e.EMAIL_STRING = eMAIL_STRING;
                    e.MODIFY_DT = System.DateTime.Now;
                    e.MODIFY_USERIDX = cREATE_USER;

                    ctx.T_OE_ORGANIZATION_EMAIL_RULE.Add(e);
                    ctx.SaveChanges();
                    return 1;
                }
                catch (Exception ex)
                {
                    LogEFException(ex);
                    return 0;
                }
            }
        }

        public static int DeleteT_OE_ORGANIZATION_EMAIL_RULE(Guid id, string email)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    T_OE_ORGANIZATION_EMAIL_RULE rec = new T_OE_ORGANIZATION_EMAIL_RULE { ORG_IDX = id, EMAIL_STRING = email };
                    ctx.Entry(rec).State = System.Data.Entity.EntityState.Deleted;
                    ctx.SaveChanges();

                    return 1;
                }
                catch (Exception ex)
                {
                    LogEFException(ex);
                    return 0;
                }
            }
        }



        //*****************ENTERPRISE_SERVICES**********************************
        public static List<T_OE_REF_ENTERPRISE_PLATFORM> GetT_OE_REF_ENTERPRISE_PLATFORM(bool actInd)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.T_OE_REF_ENTERPRISE_PLATFORM
                            where (actInd == true ? a.ACT_IND == true : true)
                            orderby a.SEQ_NO
                            select a).ToList();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int GetT_OE_REF_ENTERPRISE_PLATFORM_MaxSeq()
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return ctx.T_OE_REF_ENTERPRISE_PLATFORM.Max(r => r.SEQ_NO).ConvertOrDefault<int>();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }

        public static int InsertUpdatetT_OE_REF_ENTERPRISE_PLATFORM(int eNT_PLATFORM_IDX, string eNT_PLATFORM_NAME, string eNT_PLATFORM_DESC, string eNT_PLATFORM_EXAMPLE,
            int? sEQ_NO, bool? aCT_IND, int? cREATE_USER = 0)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    Boolean insInd = false;

                    T_OE_REF_ENTERPRISE_PLATFORM e = (from c in ctx.T_OE_REF_ENTERPRISE_PLATFORM
                                                      where c.ENT_PLATFORM_IDX == eNT_PLATFORM_IDX
                                                      select c).FirstOrDefault();

                    if (e == null)
                    {
                        insInd = true;
                        e = new T_OE_REF_ENTERPRISE_PLATFORM();
                    }


                    if (eNT_PLATFORM_NAME != null) e.ENT_PLATFORM_NAME = eNT_PLATFORM_NAME;
                    if (eNT_PLATFORM_DESC != null) e.ENT_PLATFORM_DESC = eNT_PLATFORM_DESC;
                    if (eNT_PLATFORM_EXAMPLE != null) e.ENT_PLATFORM_EXAMPLE = eNT_PLATFORM_EXAMPLE;
                    if (sEQ_NO != null) e.SEQ_NO = sEQ_NO;
                    if (e.SEQ_NO == null) e.SEQ_NO = GetT_OE_REF_ENTERPRISE_PLATFORM_MaxSeq() + 1;
                    if (aCT_IND != null) e.ACT_IND = aCT_IND ?? true;

                    e.MODIFY_DT = System.DateTime.Now;
                    e.MODIFY_USERIDX = cREATE_USER;

                    if (insInd)
                        ctx.T_OE_REF_ENTERPRISE_PLATFORM.Add(e);

                    ctx.SaveChanges();
                    return e.ENT_PLATFORM_IDX;
                }
                catch (Exception ex)
                {
                    LogEFException(ex);
                    return 0;
                }
            }
        }

        public static int DeleteT_OE_REF_ENTERPRISE_PLATFORM(int id)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    T_OE_REF_ENTERPRISE_PLATFORM rec = new T_OE_REF_ENTERPRISE_PLATFORM { ENT_PLATFORM_IDX = id };
                    ctx.Entry(rec).State = System.Data.Entity.EntityState.Deleted;
                    ctx.SaveChanges();

                    return 1;
                }
                catch (Exception ex)
                {
                    LogEFException(ex);
                    return 0;
                }
            }
        }



        //******************REF_REGION*********************************
        public static List<T_OE_REF_REGION> GetT_OE_REF_REGION()
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.T_OE_REF_REGION
                            orderby a.EPA_REGION
                            select a).ToList();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }


        //******************REF_STATE*********************************
        public static List<T_OE_REF_STATE> GetT_OE_REF_STATE()
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.T_OE_REF_STATE
                            orderby a.STATE_NAME
                            select a).ToList();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }


        //******************REF_SYNONYMS*********************************
        public static List<T_OE_REF_SYNONYMS> GetT_OE_REF_SYNONYMS()
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.T_OE_REF_SYNONYMS
                            orderby a.SYNONYM_IDX
                            select a).ToList();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int DeleteT_OE_REF_SYNONYMS(int id)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    T_OE_REF_SYNONYMS rec = new T_OE_REF_SYNONYMS { SYNONYM_IDX = id };
                    ctx.Entry(rec).State = System.Data.Entity.EntityState.Deleted;
                    ctx.SaveChanges();

                    return 1;
                }
                catch (Exception ex)
                {
                    LogEFException(ex);
                    return 0;
                }
            }
        }

        public static int InsertUpdatetT_OE_REF_SYNONYMS(int? sYNONYM_IDX, string sYNONYM_TEXT, int? cREATE_USER = 0)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    Boolean insInd = false;

                    T_OE_REF_SYNONYMS e = (from c in ctx.T_OE_REF_SYNONYMS
                                           where c.SYNONYM_IDX == sYNONYM_IDX
                                           select c).FirstOrDefault();

                    if (e == null)
                    {
                        insInd = true;
                        e = new T_OE_REF_SYNONYMS();
                        e.CREATE_DT = System.DateTime.Now;
                        e.CREATE_USERIDX = cREATE_USER;
                    }
                    else
                    {
                        e.MODIFY_DT = System.DateTime.Now;
                        e.MODIFY_USERIDX = cREATE_USER;
                    }


                    if (sYNONYM_TEXT != null) e.SYNONYM_TEXT = sYNONYM_TEXT.ToLower();


                    if (insInd)
                        ctx.T_OE_REF_SYNONYMS.Add(e);

                    ctx.SaveChanges();
                    return e.SYNONYM_IDX;
                }
                catch (Exception ex)
                {
                    LogEFException(ex);
                    return 0;
                }
            }
        }


        //*******************REF_ORG_TYPES ********************************
        public static List<T_OE_REF_ORG_TYPE> GetT_OE_REF_ORG_TYPE()
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.T_OE_REF_ORG_TYPE
                            orderby a.ORG_TYPE
                            select a).ToList();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }



        //******************REF_TAGS*********************************
        public static List<T_OE_REF_TAGS> GetT_OE_REF_TAGS_ByCategory(string CatName)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    if (CatName == "") CatName = null;

                    return (from a in ctx.T_OE_REF_TAGS
                            where (CatName == null ? true : a.TAG_CAT_NAME == CatName)
                            orderby a.TAG_CAT_NAME, a.TAG_NAME
                            select a).ToList();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static IEnumerable<SelectListItem> GetT_OE_REF_TAGS_ByCategory_ProjStatus(bool ExcludeNotUnderConsider)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_OE_REF_TAGS
                            where (ExcludeNotUnderConsider ? a.TAG_NAME != "Not under consideration" : true)
                            && a.TAG_CAT_NAME == "Project Status"
                            orderby a.TAG_IDX
                            select a).ToList();

                    return xxx.Select(x => new SelectListItem
                    {
                        Value = x.TAG_NAME,
                        Text = x.TAG_NAME
                    });
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static T_OE_REF_TAGS GetT_OE_REF_TAGS_ByCategoryAndName(string CatName, string Name)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.T_OE_REF_TAGS
                            where (CatName == null ? true : a.TAG_CAT_NAME == CatName)
                            && a.TAG_NAME == Name
                            select a).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<T_OE_REF_TAG_CATEGORIES> GetT_OE_REF_TAG_CATEGORIES()
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.T_OE_REF_TAG_CATEGORIES
                            orderby a.TAG_CAT_NAME
                            select a).ToList();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int DeleteT_OE_REF_TAGS(int id)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    T_OE_REF_TAGS rec = new T_OE_REF_TAGS { TAG_IDX = id };
                    ctx.Entry(rec).State = System.Data.Entity.EntityState.Deleted;
                    ctx.SaveChanges();

                    return 1;
                }
                catch (Exception ex)
                {
                    LogEFException(ex);
                    return 0;
                }
            }
        }

        public static int InsertUpdatetT_OE_REF_TAGS(int? tAG_IDX, string tAG_NAME, string tAG_CAT_NAME, int? cREATE_USER = 0)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    Boolean insInd = false;

                    T_OE_REF_TAGS e = (from c in ctx.T_OE_REF_TAGS
                                       where c.TAG_IDX == tAG_IDX
                                           select c).FirstOrDefault();

                    if (e == null)
                    {
                        insInd = true;
                        e = new T_OE_REF_TAGS();
                        e.CREATE_DT = System.DateTime.Now;
                        e.CREATE_USERIDX = cREATE_USER;
                    }


                    if (tAG_NAME != null) e.TAG_NAME = tAG_NAME;
                    if (tAG_CAT_NAME != null) e.TAG_CAT_NAME = tAG_CAT_NAME;


                    if (insInd)
                        ctx.T_OE_REF_TAGS.Add(e);

                    ctx.SaveChanges();
                    return e.TAG_IDX;
                }
                catch (Exception ex)
                {
                    LogEFException(ex);
                    return 0;
                }
            }
        }






        //*****************SYS_LOG**********************************
        public static int InsertT_OE_SYS_LOG(string logType, string logMsg)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    T_OE_SYS_LOG e = new T_OE_SYS_LOG();
                    e.LOG_TYPE = logType;
                    e.LOG_MSG = logMsg;
                    e.LOG_DT = System.DateTime.Now;

                    ctx.T_OE_SYS_LOG.Add(e);
                    ctx.SaveChanges();
                    return e.SYS_LOG_ID;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static void LogEFException(Exception ex)
        {
            string err = "";

            if (ex is DbEntityValidationException)
            {
                DbEntityValidationException dbex = (DbEntityValidationException)ex;
                foreach (var eve in dbex.EntityValidationErrors)
                {
                    err += "Entity error type" + eve.Entry.Entity.GetType().Name;  //maybe add eve.Entry.State too
                    foreach (var ve in eve.ValidationErrors)
                        err += " property: " + ve.PropertyName + " error: " + ve.ErrorMessage;
                }
            }
            else
            {
                if (ex.InnerException != null) {
                    if (ex.InnerException.Message == "An error occurred while updating the entries. See the inner exception for details.")
                    {
                        err = ex.InnerException.InnerException.ToString();
                    }
                }
                else
                    err = (ex.InnerException != null ? ex.InnerException.Message : "");
            }

            db_Ref.InsertT_OE_SYS_LOG("ERROR", err.SubStringPlus(0, 2000));

        }
    }
}