using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EECIP.Models;
using EECIP.App_Logic.DataAccessLayer;

namespace EECIP.Controllers
{
    [Authorize(Roles = "Admins")]
    public class ForumAdminController : Controller
    {
        // GET: ForumAdmin
        public ActionResult Index()
        {
            return RedirectToAction("Category");
        }

        // GET: Category
        public ActionResult Category()
        {
            return View();
        }

        [ChildActionOnly]
        public PartialViewResult GetMainCategories()
        {
            var viewModel = new vmForumAdminCategories
            {
                Categories = db_Forum.GetCategory()
            };
            return PartialView(viewModel);
        }


        public ActionResult CreateCategory()
        {
            var model = new vmForumAdminCategory();
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCategory(vmForumAdminCategory model)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    //if (categoryViewModel.ParentCategory != null)
                    //{
                    //    var parentCategory = _categoryService.Get(categoryViewModel.ParentCategory.Value);
                    //    category.ParentCategory = parentCategory;
                    //    SortPath(category, parentCategory);
                    //}

                    int SuccID = db_Forum.InsertUpdateCategory(model);

                    if (SuccID > 0)
                    {
                        TempData["Success"] = "Added successfully.";
                        return RedirectToAction("Category");
                    }
                }
                catch (Exception)
                {
                }

            }

            TempData["Error"] = "There was an error creating the category";
            return RedirectToAction("CreateCategory");
        }


        public ActionResult EditCategory(Guid id)
        {
            var category = db_Forum.GetCategoryByID(id);
            var vmForumAdminCategory = CreateEditCategoryViewModel(category);
            return View(vmForumAdminCategory);
        }

        [HttpPost]
        public ActionResult EditCategory(vmForumAdminCategory model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Check they are not trying to add a subcategory of this category as the parent or it will break
                    var category = db_Forum.GetCategoryByID(model.Id);
                    if (model.ParentCategory == category.Id)
                        model.ParentCategory = null;

                    //if (model.ParentCategory != null)
                    //{
                    //    // Set the parent category
                    //    var parentCategory = _categoryService.Get(model.ParentCategory.Value);
                    //    category.ParentCategory = parentCategory;

                    //    // Append the path from the parent category
                    //    SortPath(category, parentCategory);
                    //}
                    //else
                    //{
                    //    // Must access property (trigger lazy-loading) before we can set it to null (Entity Framework bug!!!)
                    //    var triggerEfLoad = category.ParentCategory;
                    //    category.ParentCategory = null;

                    //    // Also clear the path
                    //    category.Path = null;
                    //}

                    int SuccID = db_Forum.InsertUpdateCategory(model);

                    if (SuccID > 0)
                    {
                        TempData["Success"] = "Updated successfully.";
                        return RedirectToAction("Category");
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }

            TempData["Error"] = "There was an error editing the category";
            return RedirectToAction("EditCategory");
        }

        private vmForumAdminCategory CreateEditCategoryViewModel(Category category)
        {
            var vmForumAdminCategory = new vmForumAdminCategory
            {
                Name = category.Name,
                Description = category.Description,
                IsLocked = category.IsLocked,
                ModeratePosts = category.ModeratePosts,
                ModerateTopics = category.ModerateTopics,
                SortOrder = category.SortOrder,
                Id = category.Id,
                PageTitle = category.PageTitle,
                MetaDesc = category.MetaDescription,
                Image = category.Image,
                CategoryColour = category.Colour,
                ParentCategory = category.Category_Id ?? Guid.Empty
            };

            return vmForumAdminCategory;
        }

    }
}