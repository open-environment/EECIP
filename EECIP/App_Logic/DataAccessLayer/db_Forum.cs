using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using EECIP.App_Logic.BusinessLogicLayer;
using System.Data.Entity;
using EECIP.Models;

namespace EECIP.App_Logic.DataAccessLayer
{
    public class db_Forum
    {
        //****************************** CATEGORIES **********************************************
        public static List<Category> GetCategory()
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.Categories
                            orderby a.SortOrder
                            select a).ToList();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int InsertUpdateCategory(vmForumAdminCategory model)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    Boolean insInd = false;

                    Category e = (from c in ctx.Categories
                                  where c.Id == model.Id
                                  select c).FirstOrDefault();

                    //set ID
                    if (e == null)
                    {
                        e = new Category();
                        insInd = true;
                        e.Id = Guid.NewGuid();
                        e.DateCreated = System.DateTime.Now;
                    }

                    e.Name = HttpUtility.HtmlDecode(Utils.SafePlainText(model.Name));  //sanitize 
                    e.Description = Utils.GetSafeHtml(model.Description);   //sanitize
                    e.IsLocked = model.IsLocked;
                    e.ModerateTopics = model.ModerateTopics;
                    e.ModeratePosts = model.ModeratePosts;
                    e.SortOrder = model.SortOrder;
                    e.Slug = Utils.CreateUrl(model.Name, "-");    // url slug generator
                    e.PageTitle = model.PageTitle;
                    if (e.PageTitle == null)
                        e.PageTitle = e.Name;  //set page title to name if not specified
                    e.MetaDescription = model.MetaDesc;
                    e.Colour = model.CategoryColour;
                    e.Image = model.Image;
                    e.Category_Id = model.ParentCategory;

                    if (insInd)
                        ctx.Categories.Add(e);

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

        public static Category GetCategoryByID(Guid id)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.Categories.AsNoTracking()
                            where a.Id == id
                            select a).FirstOrDefault();
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