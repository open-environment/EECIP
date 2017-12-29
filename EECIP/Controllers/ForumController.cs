using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EECIP.Models;
using EECIP.App_Logic.DataAccessLayer;
using System.Web.Security;
using EECIP.App_Logic.BusinessLogicLayer;

namespace EECIP.Controllers
{
    public class ForumController : Controller
    {
        private CheckCreateTopicPermissions GetOptionalPermissions()
        {
            bool isAdmin = User.IsInRole("Admins");

            return new CheckCreateTopicPermissions
            {
                CanLockTopic = isAdmin,
                CanStickyTopic = isAdmin,
                CanUploadFiles = true,
                CanCreatePolls = true,
                CanInsertImages = false
            };
        }


        // GET: Forum
        public ActionResult Index()
        {
            return View();
        }    

        [Authorize]
        public ActionResult Create()
        {
            var viewModel = new vmForumTopicCreate
            {
                SubscribeToTopic = true,
                Categories = ddlForumHelpers.get_ddl_categories(),
                OptionalPermissions = GetOptionalPermissions(),
                IsTopicStarter = true,
                PollAnswers = new List<string>(),
                PollCloseAfterDays = 0,
                SelectedTags = new List<string>(),
                AllTags = db_Forum.GetTopicTags_ByAttributeAll(Guid.NewGuid(), "Project Feature").Select(x => new SelectListItem { Value = x, Text = x })
        };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(vmForumTopicCreate topicViewModel)
        {
            int UserIDX = db_Accounts.GetUserIDX();

            // If viewModel is returned back to the view, repopulate view model fist
            topicViewModel.OptionalPermissions = GetOptionalPermissions();
            topicViewModel.Categories = ddlForumHelpers.get_ddl_categories();
            topicViewModel.IsTopicStarter = true;
            topicViewModel.PollAnswers = topicViewModel.PollAnswers ?? new List<string>();
            /*---- End Re-populate ViewModel ----*/

            if (ModelState.IsValid)
            {
                // ************************ VALIDATION **********************************************
                // See if the user has actually added some content to the topic
                if (string.IsNullOrEmpty(topicViewModel.Content))
                {
                    TempData["Error"] = "You must supply content for the post.";
                    RedirectToAction("Create");
                }

                // Check posting flood control
                if (!db_Forum.PassedTopicFloodTest(topicViewModel.Name, UserIDX))
                {
                    TempData["Error"] = "Cannot repost the same topic";
                    RedirectToAction("Create");
                }

                // Log user out if they are LockedOut but still authenticated 
                T_OE_USERS u = db_Accounts.GetT_OE_USERSByIDX(UserIDX);
                if (u != null && u.LOCKOUT_ENABLED)
                {
                    FormsAuthentication.SignOut();
                    return RedirectToAction("Index", "Home");
                }

                // Check Permissions for topic opions
                if (!topicViewModel.OptionalPermissions.CanLockTopic)
                    topicViewModel.IsLocked = false;
                // ************************ END VALIDATION **********************************************

                // 1. Create the Topic
                Topic _Topic = db_Forum.InsertUpdateTopic(topicViewModel, UserIDX);
                if (_Topic != null)
                {
                    // 2. Create Post and add to Topic
                    Guid? _postID = db_Forum.InsertUpdatePost(null, topicViewModel.Content, null, null, true, null, null, null, null, _Topic.Id, UserIDX, false);

                    if (_postID != null)
                    {
                        // 3. Reupdate the topic with post ID
                        db_Forum.InsertUpdateTopic_withPost(_Topic.Id, _postID.ConvertOrDefault<Guid>());


                        // 4. Update the users points score for posting
                        db_Forum.InsertUpdateMembershipUserPoints(null, 1, System.DateTime.UtcNow, 0, _postID, null, UserIDX);


                        // 5. Poll handling
                        if (topicViewModel.PollAnswers.Count(x => x != null) > 1)
                        {
                            // Create a new Poll
                            //        var newPoll = new Poll
                            //        {
                            //            User = loggedOnUser,
                            //            ClosePollAfterDays = topicViewModel.PollCloseAfterDays
                            //        };

                            //        // Create the poll
                            //        _pollService.Add(newPoll);

                            //        // Now sort the answers
                            //        var newPollAnswers = new List<PollAnswer>();
                            //        foreach (var pollAnswer in topicViewModel.PollAnswers)
                            //        {
                            //            if (pollAnswer.Answer != null)
                            //            {
                            //                // Attach newly created poll to each answer
                            //                pollAnswer.Poll = newPoll;
                            //                _pollAnswerService.Add(pollAnswer);
                            //                newPollAnswers.Add(pollAnswer);
                            //            }
                            //        }
                            //        // Attach answers to poll
                            //        newPoll.PollAnswers = newPollAnswers;

                            //        // Add the poll to the topic
                            //        topic.Poll = newPoll;
                        }


                        // 6. File handling
                        if (topicViewModel.Files != null)
                        {
                            //    // Before we save anything, check the user already has an upload folder and if not create one
                            //    var uploadFolderPath =
                            //        HostingEnvironment.MapPath(string.Concat(SiteConstants.Instance.UploadFolderPath,
                            //            LoggedOnReadOnlyUser.Id));
                            //    if (!Directory.Exists(uploadFolderPath))
                            //    {
                            //        Directory.CreateDirectory(uploadFolderPath);
                            //    }

                            //    // Loop through each file and get the file info and save to the users folder and Db
                            //    foreach (var file in topicViewModel.Files)
                            //    {
                            //        if (file != null)
                            //        {
                            //            // If successful then upload the file
                            //            var uploadResult = AppHelpers.UploadFile(file, uploadFolderPath,
                            //                LocalizationService);
                            //            if (!uploadResult.UploadSuccessful)
                            //            {
                            //                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                            //                {
                            //                    Message = uploadResult.ErrorMessage,
                            //                    MessageType = GenericMessages.danger
                            //                };
                            //                unitOfWork.Rollback();
                            //                return View(topicViewModel);
                            //            }

                            //            // Add the filename to the database
                            //            var uploadedFile = new UploadedFile
                            //            {
                            //                Filename = uploadResult.UploadedFileName,
                            //                Post = topicPost,
                            //                MembershipUser = loggedOnUser
                            //            };
                            //            _uploadedFileService.Add(uploadedFile);
                            //        }
                            //    }
                        }

                        // 8. Tag handling
                        db_Forum.DeleteTopicTags(_Topic.Id, "Topic Tag");
                        foreach (string feature in topicViewModel.SelectedTags ?? new List<string>())
                            db_Forum.InsertUpdateTopicTags(_Topic.Id, "Topic Tag", feature);



                        // 9. Subscribe the user to the topic if they have checked the checkbox
                        if (topicViewModel.SubscribeToTopic)
                            db_Forum.InsertUpdateTopicNotification(null, _Topic.Id, UserIDX);

                        // 10. Success so now send the emails
                        //NotifyNewTopics(category, topic, unitOfWork);

                        //11  now update the Azure search
                        AzureSearch.PopulateSearchIndexForumTopic(_Topic.Id);

                        // Redirect to the newly created topic
                        //return Redirect($"{db_Forum.TopicURL(_Topic.Slug)}?postbadges=true");
                        return RedirectToAction("ShowTopic", "Forum", new { slug = _Topic.Slug });
                    }
                }


            }

            if (TempData["Error"].ToString() == "")
                ModelState.AddModelError(string.Empty, "Unable to save post");
            return View(topicViewModel);
        }

        [Authorize]
        public ActionResult EditPost(Guid? id)
        {
            Post _post = db_Forum.GetPost_ByID(id.ConvertOrDefault<Guid>());
            if (_post != null)
            {
                Topic _topic = db_Forum.GetTopic_ByID(_post.Topic_Id);

                var viewModel = new vmForumTopicCreate
                {
                    //stuff from post
                    Name = _topic.Name,
                    Content = _post.PostContent,
                    IsSticky = _topic.IsSticky,
                    IsLocked = _topic.IsLocked,
                    Category = _topic.Category_Id,
                    SelectedTags = db_Forum.GetTopicTags_ByAttributeSelected(_topic.Id, "Topic Tag"),
                    Id = _post.Id,
                    TopicId = _topic.Id,
                    Categories = ddlForumHelpers.get_ddl_categories(),
                    OptionalPermissions = GetOptionalPermissions(),
                    IsTopicStarter = _post.IsTopicStarter ?? false,
                    PollAnswers = new List<string>(),
                    PollCloseAfterDays = 0,
                    AllTags = db_Forum.GetTopicTags_ByAttributeAll(Guid.NewGuid(), "Project Feature").Select(x => new SelectListItem { Value = x, Text = x })
                };

                return View(viewModel);
            }
            else
            {
                TempData["Error"] = "Post not found";
                return RedirectToAction("Index", "Forum");
            }
        }


        public ActionResult ShowTopic(string slug, int? p, string order, Guid? id)
        {
            // Set initial stuff
            //var pageIndex = p ?? 1;
            int UserIDX = db_Accounts.GetUserIDX();

            // Get the topic
            var _topic = new Topic();
            if (slug != null)
                _topic = db_Forum.GetTopic_fromSlug(slug);
            else if (id != null)
                _topic = db_Forum.GetTopic_ByID(id.ConvertOrDefault<Guid>());


            if (_topic != null)
            {
                //add view count (only if not the person who created topic) and only if not a bot
                if (_topic.MembershipUser_Id != UserIDX && !BotUtils.UserIsBot())
                {
                    db_Forum.TopicAddView(_topic.Id);
                    _topic.Views = _topic.Views + 1;
                }

                var model = new vmForumTopicView();
                model.Topic = _topic;
                model.TopicTags = db_Forum.GetTopicTags_ByAttributeSelected(_topic.Id, "Topic Tag");
                model.StarterPost = db_Forum.GetPost_StarterForTopic(_topic.Id, UserIDX);
                model.Posts = db_Forum.GetPost_NonStarterForTopic(_topic.Id, UserIDX, order);
                model.IsSubscribed = db_Forum.NotificationIsUserSubscribed(_topic.Id, UserIDX);
                model.LastPostDatePretty = db_Forum.GetTopic_LastPostPrettyDate(_topic.Id);
                //model.DisablePosting = false;
                model.NewPostContent = "";

                return View(model);
            }
            else
            {
                TempData["Error"] = "No topic found";
                return RedirectToAction("Index", "Forum");
            }
        }


        [ChildActionOnly]
        public PartialViewResult ListCategorySideMenu()
        {
            var model = new vmForumAdminCategories
            {
                Categories = db_Forum.GetCategory()
            };
            return PartialView(model);
        }

        [ChildActionOnly]
        public PartialViewResult ListMainCategories()
        {
            var model = new vmForumMainCategoriesList
            {
                Categories = db_Forum.GetCategoriesMain()
            };

            return PartialView(model);
        }


        [ChildActionOnly]
        public ActionResult LatestTopics(int? p, string tag)
        {
            var viewModel = new vmForumLatestTopicsView
            {
                _topics = db_Forum.GetTopicsByCategory(null, tag),
                currentPage = p ?? 1,
                numRecs = db_Forum.GetTopicCount()
            };

            return PartialView(viewModel);
        }

        public ActionResult ShowCategory(string slug, int? p)
        {
            // Get the category
            var category = db_Forum.GetCategory_fromSlug(slug);
            if (category != null)
            {
                //get topics for the category
                var topics = db_Forum.GetTopicsByCategory(category.Category.Id, null);

                // Create the main view model for the category
                var viewModel = new vmForumCategoryView
                {
                    CategoryWithSub = category,
                    CategoryTopics = topics
                };

                return View(viewModel);
            }
            else
                return RedirectToAction("Index");
        }

        [ChildActionOnly]
        public PartialViewResult PopularTags()
        {
            var viewModel = new vmForumPopularTags();
            viewModel.popularTags = db_Forum.GetPopularTags();
            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult CreatePost(vmForumTopicView model)
        {
            int UserIDX = db_Accounts.GetUserIDX();

            // ************************ VALIDATION **********************************************
            // Check posting flood control
            if (!db_Forum.PassedPostFloodTest(UserIDX))
            {
                TempData["Error"] = "Please wait at least 30 seconds between posts";
                return RedirectToAction("ShowTopic", new { slug = model.Topic.Slug });
            }

            // Log user out if they are LockedOut but still authenticated 
            T_OE_USERS u = db_Accounts.GetT_OE_USERSByIDX(UserIDX);
            if (u != null && u.LOCKOUT_ENABLED)
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Index", "Home");
            }
            // ************************ END VALIDATION **********************************************


            Guid? _postID = db_Forum.InsertUpdatePost(null, model.NewPostContent, null, null, false, false, false, null, null, model.Topic.Id, UserIDX, false);
            if (_postID != null)
            {
                // Success send any notifications
                //NotifyNewTopics(topic, unitOfWork);
            }


            // Return view
            return RedirectToAction("ShowTopic", "Forum", new { slug = model.Topic.Slug  }); 
        }


        public ActionResult DeletePost(Guid id)
        {
            int UserIDX = db_Accounts.GetUserIDX();

            var post = db_Forum.GetPost_ByID(id);
            if (post != null)
            {
                //only allow delete if post by user or user is admin
                if (User.IsInRole("Admins") || UserIDX == post.MembershipUser_Id)
                {
                    //if not topic starter, just delete post
                    if (post.IsTopicStarter == false)
                    {
                        db_Forum.DeletePost(id);
                        TempData["Success"] = "Post deleted";
                        return RedirectToAction("ShowTopic", "Forum", new { id = post.Topic_Id });
                    }
                    else
                    {
                        //remove post id from topic
                        db_Forum.OrphanTopic(post.Topic_Id);

                        //delete all posts for the topic
                        List<Post> _postList = db_Forum.GetPost_ByTopicID(post.Topic_Id);
                        foreach (Post p in _postList)
                        {
                            db_Forum.DeletePost(p.Id);
                        }

                        //then delete the topic
                        db_Forum.DeleteTopic(post.Topic_Id);

                        TempData["Success"] = "Post deleted";
                        return RedirectToAction("Index", "Forum");

                    }
                }
            }

            TempData["Error"] = "Unable to delete";
            return RedirectToAction("LatestTopics", "Forum", new { slug = post.Topic_Id });
        }


        // POST: /Forum/PostVote
        [HttpPost]
        public JsonResult PostVote(Guid? id, string typ)
        {
            int UserIDX = db_Accounts.GetUserIDX();
            bool SuccInd = false;

            if (typ == "up" || typ == "down")
            {
                Guid? VoteID = db_Forum.InsertVote(id.ConvertOrDefault<Guid>(), UserIDX, (typ == "up" ? 1 : -1));
                SuccInd = (VoteID != null);
            }
            else if (typ == "removeup" || typ == "removedown")
            {
                int SuccID = db_Forum.DeleteVote(id.ConvertOrDefault<Guid>(), UserIDX);
                SuccInd = (SuccID == 1);
            }
            else
                return Json(new { msg = "Unable to record vote." });


            //return response
            if (SuccInd)
            {
                //get latest vote count
                string votes = db_Forum.GetVotes_TotalByPost(id.ConvertOrDefault<Guid>(), (typ=="up" || typ=="removeup"), (typ=="down" || typ=="removedown")).ToString();
                return Json(new { msg = "Success", val = votes });
            }
            else
                return Json(new { msg = "Unable to record vote." });

        }



        // POST: /Forum/PostAnswer
        [HttpPost]
        public JsonResult PostAnswer(Guid? id, string typ)
        {
            int UserIDX = db_Accounts.GetUserIDX();
            bool SuccInd = false;
            bool Answered = false;

            Post _p = db_Forum.GetPost_ByID(id.ConvertOrDefault<Guid>());
            if (_p != null)
            {
                //update Topic
                Guid? TopicID = db_Forum.SetTopicAnswer(_p.Topic_Id, (typ == "answer"));

                //update Post 
                Guid? VoteID = db_Forum.InsertUpdatePost(id, null, null, (typ == "answer"), null, null, null, null, null, null, null, false);
                SuccInd = (VoteID != null);
                Answered = (typ == "answer");
            }

            //return response
            if (SuccInd)
                return Json(new { msg = "Success", val = Answered });
            else
                return Json(new { msg = "Unable to record vote." });

        }



    }
}