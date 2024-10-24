﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EECIP.App_Logic.DataAccessLayer;
using System.Web.Mvc;

namespace EECIP.Models
{

    public static class ddlHelpers
    {
        public static IEnumerable<SelectListItem> get_ddl_states()
        {

            return db_Ref.GetT_OE_REF_STATE().Select(x => new SelectListItem
            {
                Value = x.STATE_CD,
                Text = x.STATE_NAME
            });
        }

        public static IEnumerable<SelectListItem> get_ddl_regions()
        {

            return db_Ref.GetT_OE_REF_REGION().Select(x => new SelectListItem
            {
                Value = x.EPA_REGION.ToString(),
                Text = x.EPA_REGION_NAME
            });
        }

        public static IEnumerable<SelectListItem> get_ddl_organizations(bool activeInd, bool excludeGovernInd)
        {

            return db_Ref.GetT_OE_ORGANIZATION(activeInd, excludeGovernInd, null).Select(x => new SelectListItem
            {
                Value = x.ORG_IDX.ToString(),
                Text = x.ORG_NAME
            });
        }

        public static IEnumerable<SelectListItem> get_ddl_organizations_all_governance()
        {

            return db_Ref.GetT_OE_ORGANIZATIONS_ByType("Governance").Select(x => new SelectListItem
            {
                Value = x.ORG_IDX.ToString(),
                Text = x.ORG_NAME
            });
        }

        public static IEnumerable<SelectListItem> get_ddl_tags_by_category(string cat_name)
        {

            return db_Ref.GetT_OE_REF_TAGS_ByCategory(cat_name).Select(x => new SelectListItem
            {
                Value = x.TAG_IDX.ToString(),
                Text = x.TAG_NAME
            });
        }

        public static IEnumerable<SelectListItem> get_ddl_tags_by_category_stringy(string cat_name)
        {

            return db_Ref.GetT_OE_REF_TAGS_ByCategory(cat_name).Select(x => new SelectListItem
            {
                Value = x.TAG_NAME,
                Text = x.TAG_NAME
            });

        }

        public static IEnumerable<SelectListItem> get_ddl_tag_cats()
        {

            return db_Ref.GetT_OE_REF_TAG_CATEGORIES().Where(x => x.ACT_IND == true).Select(x => new SelectListItem
            {
                Value = x.TAG_CAT_NAME,
                Text = x.TAG_CAT_NAME
            });
        }

        public static IEnumerable<SelectListItem> get_ddl_orgtypes(bool excludeGovernance)
        {

            return db_Ref.GetT_OE_REF_ORG_TYPE(excludeGovernance).Select(x => new SelectListItem
            {
                Value = x.ORG_TYPE,
                Text = x.ORG_TYPE + " (" + x.ORG_TYPE_DESC + ")"
            });
        }


        public static IEnumerable<SelectListItem> get_ddl_users_by_organization(Guid orgID)
        {
            return db_Accounts.GetT_OE_USERSByAgency(orgID).Select(x => new SelectListItem
            {
                Value = x.USER_IDX.ToString(),
                Text = x.FNAME + " " + x.LNAME
            });
        }

        public static IEnumerable<SelectListItem> get_ddl_users_by_organizationList(List<T_OE_ORGANIZATION> orgs)
        {
            return db_Accounts.GetT_OE_USERSByAgencyList(orgs).Select(x => new SelectListItem
            {
                Value = x.USER_IDX.ToString(),
                Text = x.FNAME + " " + x.LNAME
            });
        }

        public static IEnumerable<SelectListItem> get_ddl_ProjectUpdateFreq()
        {
            List<SelectListItem> _list = new List<SelectListItem>();
            _list.Add(new SelectListItem() { Value = "N", Text = "None" });
            //_list.Add(new SelectListItem() { Value = "M", Text = "Monthly" });
            _list.Add(new SelectListItem() { Value = "Q", Text = "Quarterly" });
            _list.Add(new SelectListItem() { Value = "S", Text = "Semi-annual" });
            _list.Add(new SelectListItem() { Value = "Y", Text = "Yearly" });
            return _list;
        }


    }

    public class vmSharedNotificationHeader
    {
        public int NotifyCount { get; set; }
        public List<T_OE_USER_NOTIFICATION> Notifications { get; set; }
    }
}