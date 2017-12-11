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
        public string PROJECT_NAME { get; set; }
        public string VENDOR { get; set; }
        public string IMPLEMENT_STATUS { get; set; }
        public string COMMENTS { get; set; }
        public DateTime? CREATE_DT { get; set; }
        public DateTime? MODIFY_DT { get; set; }
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
                               where a.TAG_CAT_NAME == "Expertise"
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
                    return (from a in ctx.T_OE_REF_ENTERPRISE_PLATFORM
                            join b in ctx.T_OE_ORGANIZATION_ENT_SVCS on a.ENT_PLATFORM_IDX equals b.ENT_PLATFORM_IDX
                                into sr from x in sr.DefaultIfEmpty()  //left join
                            where (x == null ? true : x.ORG_IDX == OrgID)
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
                                CREATE_DT = x.CREATE_DT, 
                                MODIFY_DT = x.MODIFY_DT
                            }).ToList();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int InsertUpdatetT_OE_ORGANIZATION_ENT_SVCS(int? oRG_ENT_SVCS_IDX, Guid? oRG_IDX, int? eNT_PLATFORM_IDX, string pROJECT_NAME, 
            string vENDOR, string iMPLEMENT_STATUS, string cOMMENTS, int? cREATE_USER = 0)
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
                    }
                    else
                    {
                        e.MODIFY_DT = System.DateTime.Now;
                        e.MODIFY_USERIDX = cREATE_USER;
                    }
                  


                    if (oRG_IDX != null) e.ORG_IDX = oRG_IDX.ConvertOrDefault<Guid>();
                    if (eNT_PLATFORM_IDX != null) e.ENT_PLATFORM_IDX = eNT_PLATFORM_IDX ?? 0;
                    if (pROJECT_NAME != null) e.PROJECT_NAME = pROJECT_NAME;
                    if (vENDOR != null) e.VENDOR = vENDOR;
                    if (iMPLEMENT_STATUS != null) e.IMPLEMENT_STATUS = iMPLEMENT_STATUS;
                    if (cOMMENTS != null) e.COMMENTS = cOMMENTS;

                    if (insInd)
                        ctx.T_OE_ORGANIZATION_ENT_SVCS.Add(e);

                    ctx.SaveChanges();
                    return e.ENT_PLATFORM_IDX;
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
                               join e in ctx.T_OE_REF_ENTERPRISE_PLATFORM on a.ENT_PLATFORM_IDX equals e.ENT_PLATFORM_IDX
                               where a.SYNC_IND == false
                               && (EntSvcIDX == null ? true : a.ORG_ENT_SVCS_IDX == EntSvcIDX)
                               select new EECIP_Index
                               {
                                   Agency = o.ORG_NAME,
                                   KeyID = a.ORG_ENT_SVCS_IDX.ToString(),
                                   DataType = "Enterprise Service",
                                   Record_Source = a.RECORD_SOURCE,
                                   Name = e.ENT_PLATFORM_NAME,
                                   Description = a.PROJECT_NAME,
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


        //***************************PROJECTS****************************************
        public static List<T_OE_PROJECTS> GetT_OE_PROJECTS_ByOrgIDX(Guid OrgID)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_OE_PROJECTS
                               .Include(x => x.T_OE_REF_TAGS)
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


        public static List<EECIP_Index> GetT_OE_PROJECTS_ReadyToSync(Guid? ProjectIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var xxx = (from a in ctx.T_OE_PROJECTS
                            join o in ctx.T_OE_ORGANIZATION on a.ORG_IDX equals o.ORG_IDX
                            join b in ctx.T_OE_REF_TAGS on a.MEDIA_TAG equals b.TAG_IDX into sr from x in sr.DefaultIfEmpty()  //left join
                            where a.SYNC_IND == false
                            && a.ACT_IND == true
                            && (ProjectIDX == null ? true : a.PROJECT_IDX == ProjectIDX)
                            select new EECIP_Index {
                                Agency = o.ORG_NAME,
                                KeyID = a.PROJECT_IDX.ToString(),
                                DataType = "Project",
                                Record_Source = a.RECORD_SOURCE,
                                Name = a.PROJ_NAME,
                                Description = a.PROJ_DESC,
                                Media = x.TAG_NAME
                            }).ToList();

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
            string aDV_MON_DESC, int? bP_MODERN_IND, string bP_MODERN_DESC, string cOTS, string vENDOR, bool aCT_IND, bool? sYNC_IND, int? cREATE_USER = 0,
            bool? updateSearch = false, string iMPORT_ID = null)
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
                    }
                    else
                    {
                        e.MODIFY_DT = System.DateTime.Now;
                        e.MODIFY_USERIDX = cREATE_USER;
                    }

                    if (oRG_IDX != null) e.ORG_IDX = oRG_IDX.ConvertOrDefault<Guid>();
                    if (pROJ_NAME != null) e.PROJ_NAME = pROJ_NAME;
                    if (pROJ_DESC != null) e.PROJ_DESC = pROJ_DESC;
                    if (mEDIA_TAG != null) e.MEDIA_TAG = mEDIA_TAG;
                    if (sTART_YEAR != null) e.START_YEAR = sTART_YEAR;
                    if (pROJ_STATUS != null) e.PROJ_STATUS = pROJ_STATUS;
                    if (dATE_LAST_UPDATE != null) e.DATE_LAST_UPDATE = dATE_LAST_UPDATE;
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



        //***************************project local****************************************
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
            catch (Exception ex) {  }
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



        //******************************* NOTIFICATIONS ***************************************

        public static List<T_OE_USER_NOTIFICATION> GetT_OE_USER_NOTIFICATION_byUserIDX(int? UserIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    if (UserIDX == null) return null;

                    return (from a in ctx.T_OE_USER_NOTIFICATION
                            where a.USER_IDX == UserIDX
                            select a).ToList();
                }
                catch (Exception ex)
                { db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

    }
}