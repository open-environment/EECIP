using EECIP.App_Logic.BusinessLogicLayer;
using EECIP.App_Logic.DataAccessLayer;
using EECIP.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace EECIP.Controllers
{
    [Authorize]
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


        public ActionResult Index(int? p, string tag)
        {
            var viewModel = new vmForumLatestTopicsView
            {
                _topics = db_Forum.GetTopicsByCategory(null, tag, p ?? 1),
                currentPage = p ?? 1,
                numRecs = db_Forum.GetTopicCount(tag)
            };

            return View(viewModel);
        }


        public ActionResult PostedIn(int? p, string tag)
        {
            int UserIDX = db_Accounts.GetUserIDX();

            List<Guid> PostedIn = db_Forum.GetTopicIDs_InWhichUserPosted(UserIDX);

            var viewModel = new vmForumLatestTopicsView
            {
                _topics = db_Forum.GetTopicsByCategoryPostedIn(null, tag, p ?? 1, PostedIn),
                currentPage = p ?? 1,
                numRecs = db_Forum.GetTopicCountPostedIn(tag, PostedIn)
            };

            return View(viewModel);
        }


        public ActionResult Following(int? p, string tag)
        {
            int UserIDX = db_Accounts.GetUserIDX();

            var viewModel = new vmForumLatestTopicsView
            {
                _topics = db_Forum.GetTopicsByCategoryFollowing(null, tag, p ?? 1, UserIDX),
                currentPage = p ?? 1,
                numRecs = db_Forum.GetTopicCountFollowing(tag, UserIDX)
            };

            return View(viewModel);
        }


        public ActionResult Create(Guid? id)
        {
            var viewModel = new vmForumTopicCreate
            {
                SubscribeToTopic = true,
                Categories = ddlForumHelpers.get_ddl_categories_tree(),
                Category = id.ConvertOrDefault<Guid>(),
                OptionalPermissions = GetOptionalPermissions(),
                IsTopicStarter = true,
                SelectedTags = new List<string>(),
                AllTags = db_Forum.GetTopicTags_ByAttributeAll(Guid.NewGuid(), "Tags").Select(x => new SelectListItem { Value = x, Text = x })
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(vmForumTopicCreate topicViewModel)
        {
            int UserIDX = db_Accounts.GetUserIDX();

            // If viewModel is returned back to the view, repopulate view model fist
            topicViewModel.OptionalPermissions = GetOptionalPermissions();
            topicViewModel.Categories = ddlForumHelpers.get_ddl_categories_tree();
            topicViewModel.IsTopicStarter = true;
            topicViewModel.PollAnswers = topicViewModel.PollAnswers ?? new List<PollAnswer>();
            topicViewModel.SelectedTags = topicViewModel.SelectedTags ?? new List<string>();
            topicViewModel.AllTags = db_Forum.GetTopicTags_ByAttributeAll(Guid.NewGuid(), "Tags").Select(x => new SelectListItem { Value = x, Text = x });
            /*---- End Re-populate ViewModel ----*/

            if (ModelState.IsValid)
            {
                // ************************ VALIDATION **********************************************
                // See if the user has actually added some content to the topic
                if (string.IsNullOrEmpty(topicViewModel.Content))
                {
                    if (topicViewModel.PollAnswers.Count > 0)
                    {
                        topicViewModel.Content = topicViewModel.Name;
                    }
                    else
                    { 
                        TempData["Error"] = "You must supply content for the post.";
                        return RedirectToAction("Create");
                    }
                }

                // Check posting flood control
                if (!db_Forum.PassedTopicFloodTest(topicViewModel.Name, UserIDX))
                {
                    TempData["Error"] = "Can only post once per minute";
                    return RedirectToAction("Create");
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

                        //3b. Set last update datetime
                        db_Forum.UpdateTopic_SetLastPostDate(_Topic.Id, null);


                        // 4. Update the users points score for posting
                        db_Forum.InsertUpdateMembershipUserPoints(null, 1, System.DateTime.UtcNow, 0, _postID, null, UserIDX);


                        // 5. Poll handling
                        if (topicViewModel.PollAnswers.Count(x => x != null) > 1)
                        {
                            // Create a new Poll
                            Guid? PollID = db_Forum.InsertUpdatePoll(null, false, System.DateTime.Now, topicViewModel.PollCloseAfterDays, UserIDX);

                            // Now sort the answers
                            foreach (PollAnswer answer in topicViewModel.PollAnswers)
                            {
                                if (answer.Answer != null)
                                {
                                    db_Forum.InsertUpdatePollAnswer(null, answer.Answer, PollID);
                                }
                            }

                            // Add the poll to the topic
                            db_Forum.SetTopicPollID(_Topic.Id, PollID.ConvertOrDefault<Guid>());
                        }


                        // 6. File handling
                        if (topicViewModel.Files != null && topicViewModel.Files.Any())
                        {
                            UPloadPostFiles(topicViewModel.Files, UserIDX, _postID.ConvertOrDefault<Guid>());
                        }


                        // 7. Tag handling
                        db_Forum.DeleteTopicTags(_Topic.Id, "Topic Tag");
                        foreach (string feature in topicViewModel.SelectedTags ?? new List<string>())
                            db_Forum.InsertUpdateTopicTags(_Topic.Id, "Topic Tag", feature);


                        // 8. Subscribe the user to the topic if they have checked the checkbox
                        if (topicViewModel.SubscribeToTopic)
                            db_Forum.InsertUpdateTopicNotification(null, _Topic.Id, UserIDX);

                        // 9. Success so now send the emails
                        //NotifyNewTopics(category, topic, unitOfWork);


                        // 10. Badge Checking
                        db_Forum.EarnBadgePostTopicEvent(UserIDX);

                        // 11. Update Azure search
                        AzureSearch.PopulateSearchIndexForumPost(_postID);

                        // Redirect to the newly created topic
                        return RedirectToAction("ShowTopic", "Forum", new { slug = _Topic.Slug });
                    }
                }


            }

            if (TempData["Error"] == null)
                TempData["Error"] = "Unable to save data";
            return View(topicViewModel);
        }


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
                    Categories = ddlForumHelpers.get_ddl_categories_tree(),
                    OptionalPermissions = GetOptionalPermissions(),
                    IsTopicStarter = _post.IsTopicStarter ?? false,
                    PollAnswers = new List<PollAnswer>(),
                    PollCloseAfterDays = 0,
                    AllTags = db_Forum.GetTopicTags_ByAttributeAll(_topic.Id, "Tags").Select(x => new SelectListItem { Value = x, Text = x })
                };

                //add polling information
                if (_topic.Poll_Id != null)
                {
                    Poll _poll = db_Forum.GetPoll_ByID(_topic.Poll_Id.ConvertOrDefault<Guid>());
                    if (_poll != null)
                    {
                        viewModel.PollAnswers = db_Forum.GetPollAnswers_ByPollID(_topic.Poll_Id.ConvertOrDefault<Guid>());
                        viewModel.PollCloseAfterDays = _poll.ClosePollAfterDays ?? 0;
                        viewModel.IsPollClosed = _poll.IsClosed;
                    }
                }

                return View(viewModel);
            }
            else
            {
                TempData["Error"] = "Post not found";
                return RedirectToAction("Index", "Forum");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(vmForumTopicCreate model) {

            //get topic being edited
            Topic _topic = db_Forum.GetTopic_ByID(model.TopicId);
            if (_topic != null)
            {
                int UserIDX = db_Accounts.GetUserIDX();

                // Update the Topic
                db_Forum.InsertUpdateTopic(model, UserIDX);

                Guid? newPostID = db_Forum.InsertUpdatePost(model.Id, model.Content, null, null, null, null, null, null, null, null, null, true);

                //3b. Set last update datetime
                db_Forum.UpdateTopic_SetLastPostDate(model.TopicId, null);

                // 5. Poll handling
                if (_topic.Poll_Id != null) {
                    if (model.PollAnswers.Count(x => x != null) > 1)
                    {
                        // Update Poll
                        Guid? PollID = db_Forum.InsertUpdatePoll(_topic.Poll_Id, model.IsPollClosed, System.DateTime.Now, model.PollCloseAfterDays, UserIDX);

                        // Update poll answers
                        foreach (PollAnswer answer in model.PollAnswers)
                        {
                            if (answer.Answer != null)
                            {
                                db_Forum.InsertUpdatePollAnswer(answer.Id, answer.Answer, PollID);
                            }
                        }
                    }
                }

                // 7. Tag handling
                db_Forum.DeleteTopicTags(model.TopicId, "Topic Tag");
                foreach (string feature in model.SelectedTags ?? new List<string>())
                    db_Forum.InsertUpdateTopicTags(model.TopicId, "Topic Tag", feature);

                // 11. Update Azure search
                AzureSearch.PopulateSearchIndexForumPost(newPostID);
            }

            return RedirectToAction("ShowTopic", "Forum", new { slug = _topic.Slug });
        }
        

        public ActionResult ShowTopic(string slug, int? p, string order, Guid? id, Guid? p_id)
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
            else if (p_id != null)
                _topic = db_Forum.GetTopic_ByPostID(p_id.ConvertOrDefault<Guid>());


            if (_topic != null)
            {
                //add view count (only if not the person who created topic) and only if not a bot
                if (_topic.MembershipUser_Id != UserIDX && !BotUtils.UserIsBot())
                {
                    db_Forum.TopicAddView(_topic.Id);
                    _topic.Views = _topic.Views + 1;
                }

                var model = new vmForumTopicView
                {
                    Topic = _topic,
                    TopicTags = db_Forum.GetTopicTags_ByAttributeSelected(_topic.Id, "Topic Tag"),
                    StarterPost = db_Forum.GetPost_StarterForTopic(_topic.Id, UserIDX),
                    Posts = db_Forum.GetPost_NonStarterForTopic(_topic.Id, UserIDX, order),
                    IsSubscribed = db_Forum.NotificationIsUserSubscribed(_topic.Id, UserIDX),
                    LastPostDatePretty = db_Forum.GetTopic_LastPostPrettyDate(_topic.Id),
                    //model.DisablePosting = false;
                    NewPostContent = ""
                };

                //poll handling
                if (_topic.Poll_Id != null) {
                    Guid p1 = _topic.Poll_Id.ConvertOrDefault<Guid>();
                    model.Poll = new vmForumPoll {
                        Poll = db_Forum.GetPoll_ByID(p1),
                        TotalVotesInPoll = db_Forum.GetPollVotes_ByPollID(p1),
                        UserHasAlreadyVoted = db_Forum.HasUserVotedInPoll(p1, UserIDX),
                        PollAnswers = db_Forum.GetPollAnswersWithVotes_ByPollID(p1)
                    };
                }

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


        public ActionResult ShowCategory(string slug, int? p)
        {
            // Get the category
            var category = db_Forum.GetCategory_fromSlug(slug);
            if (category != null)
            {
                //get topics for the category
                var topics = db_Forum.GetTopicsByCategory(category.Category.Id, null, p ?? 1);

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
            var viewModel = new vmForumPopularTags
            {
                popularTags = db_Forum.GetPopularTags()
            };
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

            //required fields
            if (model.NewPostContent == null)
            {
                TempData["Error"] = "You must supply post content.";
                return RedirectToAction("ShowTopic", new { slug = model.Topic.Slug });
            }
            // ************************ END VALIDATION **********************************************


            Guid? _postID = db_Forum.InsertUpdatePost(null, model.NewPostContent, null, null, false, false, false, null, null, model.Topic.Id, UserIDX, false);
            if (_postID != null)
            {
                //set topic last post date
                db_Forum.UpdateTopic_SetLastPostDate(model.Topic.Id, null);

                // Success send any notifications
                NotifyTopics(model.Topic.Id, UserIDX, "Post");

                // 4. Update the users points score for posting
                db_Forum.InsertUpdateMembershipUserPoints(null, 1, System.DateTime.UtcNow, 0, _postID, null, UserIDX);

                // Update Azure search
                AzureSearch.PopulateSearchIndexForumPost(_postID);
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

                        //update the topic to reflect correct "LastEdited Date"
                        Post _lastPost = db_Forum.GetPost_LatestOfTopic(post.Topic_Id);
                        if (_lastPost != null)
                            db_Forum.UpdateTopic_SetLastPostDate(post.Topic_Id, _lastPost.DateEdited);

                        //now sync with Azure
                        AzureSearch.DeleteAzureGuid(id);

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

                            //now sync with Azure
                            AzureSearch.DeleteAzureGuid(p.Id);
                        }

                        //then delete the topic
                        int SuccID = db_Forum.DeleteTopic(post.Topic_Id);

                        if (SuccID > 0)
                        {
                            //now sync with Azure
                            //AzureSearch.DeleteAzureGuid(id);

                            TempData["Success"] = "Topic deleted";
                        }
                        return RedirectToAction("Index", "Forum");

                    }
                }
            }

            TempData["Error"] = "Unable to delete";
            return RedirectToAction("LatestTopics", "Forum", new { slug = post.Topic_Id });
        }


        [HttpPost]
        public JsonResult PostVote(Guid? id, string typ)
        {
            int UserIDX = db_Accounts.GetUserIDX();
            bool SuccInd = false;

            if (typ == "up" || typ == "down")
            {
                Guid? VoteID = db_Forum.InsertVote(id.ConvertOrDefault<Guid>(), UserIDX, (typ == "up" ? 1 : -1));
                SuccInd = (VoteID != null);

                //
                if (typ == "up")
                {
                    // Badge Checking
                    db_Forum.EarnBadgeUpVotePost(UserIDX);

                    //send notifications
                    Post _post = db_Forum.GetPost_ByID(id.ConvertOrDefault<Guid>());
                    if (_post != null)
                        NotifyTopics(_post.Topic_Id, UserIDX, "Upvote");
                }
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


        [HttpPost]
        public JsonResult SubscribeTopic(Guid? topic_id)
        {
            int UserIDX = db_Accounts.GetUserIDX();

            Guid? SuccId = db_Forum.InsertUpdateTopicNotification(null, topic_id.ConvertOrDefault<Guid>(), UserIDX);

            //return response
            if (SuccId != null)
            {
                return Json(new { msg = "Success" });
            }
            else
            {
                return Json(new { msg = "Unable to subscribe." });
            }

        }


        public JsonResult UnsubscribeTopic(Guid? topic_id)
        {
            int UserIDX = db_Accounts.GetUserIDX();

            int SuccId = db_Forum.DeleteTopicNotification(topic_id.ConvertOrDefault<Guid>(), UserIDX);

            //return response
            if (SuccId == 1)
            {
                return Json(new { msg = "Success" });
            }
            else
            {
                return Json(new { msg = "Unable to unsubscribe." });
            }

        }


        [HttpPost]
        public JsonResult PollVote(Guid? poll_id, Guid? answer_id, Guid? topic_id)
        {
            int UserIDX = db_Accounts.GetUserIDX();

            Guid? SuccID = db_Forum.InsertUpdatePollVote(null, answer_id.ConvertOrDefault<Guid>(), UserIDX);

            //return response
            if (SuccID != null)
            {
                return Json(new {
                    msg = "Success",
                    redirectUrl = Url.Action("ShowTopic", "Forum", new { id = topic_id})
                });
            }
            else
                return Json(new { msg = "Unable to record vote." });
        }


        private void NotifyTopics(Guid topicID, int PosterUserIDX, string notifyType)
        {
            Topic _topic = db_Forum.GetTopic_ByID(topicID);
            if (_topic != null)
            {
                List<TopicNotification> ts = db_Forum.GetTopicNotification_ByTopic(topicID);
                if (ts != null)
                {
                    foreach (TopicNotification t in ts)
                    {
                        if (t.MembershipUser_Id != PosterUserIDX)  //don't send notification to the person making the post
                        {
                            db_EECIP.InsertUpdateT_OE_USER_NOTIFICATION(null, t.MembershipUser_Id, System.DateTime.UtcNow, "Topic", ("New " + notifyType + " in: " + _topic.Name).SubStringPlus(0,50), "A new " + notifyType + " has been made to the discussion topic " + _topic.Name + " that you are subscribed to. Click <a href='" + db_Ref.GetT_OE_APP_SETTING("PUBLIC_APP_PATH") + "/Forum/ShowTopic?id=" + topicID + "'>here</a> to view.", false, PosterUserIDX, true, 0, true);
                        }
                    }
                }
            }

        }



        /****************POST FILES **********************/
        [HttpPost]
        public ActionResult PostFileUpload(vmForumAttachFilesToPost attachFileToPostViewModel)
        {
            try
            {
                int UserIDX = db_Accounts.GetUserIDX();

                // First this to do is get the post
                var post = db_Forum.GetPost_ByID(attachFileToPostViewModel.UploadPostId);
                if (post != null)
                {
                    // Check we get a valid post back and have some file
                    if (attachFileToPostViewModel.Files != null && attachFileToPostViewModel.Files.Any())
                    {
                        UPloadPostFiles(attachFileToPostViewModel.Files, UserIDX, post.Id);

                        TempData["Success"] = "Upload success";
                        return RedirectToAction("ShowTopic", "Forum", new { id = post.Topic_Id });

                    }
                    else
                        TempData["Error"] = "Please select a file";
                }
                else
                    TempData["Error"] = "Invalid post";

                return RedirectToAction("ShowTopic", "Forum", new { id = post.Topic_Id });
            }
            catch
            {
                TempData["Error"] = "Invalid post";
                return RedirectToAction("Index", "Forum");
            }

        }


        private static void UPloadPostFiles(HttpPostedFileBase[] files, int UserIDX, Guid postID)
        {
            // Loop through each file and get the file info and save to the users folder and Db
            foreach (var file in files)
            {
                byte[] fileBytes = null;

                if (file != null)
                {
                    using (Stream inputStream = file.InputStream)
                    {
                        MemoryStream memoryStream = inputStream as MemoryStream;
                        if (memoryStream == null)
                        {
                            memoryStream = new MemoryStream();
                            inputStream.CopyTo(memoryStream);
                        }
                        fileBytes = memoryStream.ToArray();

                        //insert to database
                        db_Forum.InsertUpdatePostFile(null, file.FileName, postID, fileBytes, file.FileName, file.ContentType, UserIDX);
                    }
                }
            }
        }


        public ActionResult PostFileDownload(Guid? id)
        {
            try
            {
                PostFile doc = db_Forum.GetPostFile_ByID(id.ConvertOrDefault<Guid>());
                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = doc.Filename,
                    Inline = false
                };

                Response.AppendHeader("Content-Disposition", cd.ToString());
                if (doc.FileContent != null)
                    return File(doc.FileContent, doc.FileContentType ?? "application/octet-stream");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            TempData["Error"] = "Unable to download document.";
            return RedirectToAction("Index", "Forum");
        }


        public ActionResult PostFileDelete(Guid? id)
        {

            PostFile doc = db_Forum.GetPostFile_ByID(id.ConvertOrDefault<Guid>());
            if (doc != null)
            {
                int UserIDX = db_Accounts.GetUserIDX();

                //permission check
                if (UserIDX == doc.MembershipUser_Id || User.IsInRole("Admins"))
                {
                    int SuccID = db_Forum.DeletePostFile(id.ConvertOrDefault<Guid>());
                    if (SuccID > 0)
                    {
                        //get the topic from post
                        Post _post = db_Forum.GetPost_ByID(doc.Post_Id.ConvertOrDefault<Guid>());

                        TempData["Success"] = "File removed.";
                        return RedirectToAction("ShowTopic", "Forum", new { id = _post.Topic_Id });
                    }
                }
            }


            TempData["Error"] = "Unable to delete document.";
            return RedirectToAction("Index", "Forum");
        }

    }
}