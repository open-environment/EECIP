using System.ComponentModel.DataAnnotations;
using EECIP.App_Logic.DataAccessLayer;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web;
using System;
using System.ComponentModel;
using System.Linq;

namespace EECIP.Models
{
    public static class ddlForumHelpers
    {
        public static IEnumerable<SelectListItem> get_ddl_categories()
        {
            return db_Forum.GetCategory().Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            });
        }
    }


    public class vmForumAdminCategories
    {
        public List<Category> Categories { get; set; }

    }

    public class vmForumAdminCategory
    {
        [HiddenInput]
        public Guid Id { get; set; }

        [DisplayName("Category Name")]
        //[Required]
        [StringLength(600)]
        public string Name { get; set; }

        [DisplayName("Category Description")]
        [DataType(DataType.MultilineText)]
        [UIHint("forumeditor"), AllowHtml]
        public string Description { get; set; }

        [DisplayName("Category Colour")]
        [UIHint("colourpicker"), AllowHtml]
        public string CategoryColour { get; set; }

        [DisplayName("Lock The Category")]
        public bool IsLocked { get; set; }

        [DisplayName("Moderate all topics in this Category")]
        public bool ModerateTopics { get; set; }

        [DisplayName("Moderate all posts in this Category")]
        public bool ModeratePosts { get; set; }

        [DisplayName("Sort Order")]
        [Range(0, int.MaxValue)]
        public int SortOrder { get; set; }

        [DisplayName("Parent Category")]
        public Guid? ParentCategory { get; set; }

        public IEnumerable<SelectListItem> AllCategories { get; set; }

        [DisplayName("Page Title")]
        [MaxLength(80)]
        public string PageTitle { get; set; }

        [DisplayName("Meta Desc")]
        [MaxLength(200)]
        public string MetaDesc { get; set; }

        [DisplayName("Category Image")]
        public HttpPostedFileBase[] Files { get; set; }
        public string Image { get; set; }


        //initialize
        public vmForumAdminCategory(){
            AllCategories = ddlForumHelpers.get_ddl_categories();
        }
    }
}