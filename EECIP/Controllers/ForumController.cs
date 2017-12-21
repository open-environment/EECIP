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
                PollCloseAfterDays = 0
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
                    Guid? _postID = db_Forum.InsertUpdatePost(null, topicViewModel.Content, null, null, true, null, null, null, null, _Topic.Id, UserIDX);

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
                        if (!string.IsNullOrEmpty(topicViewModel.Tags))
                        {
                            //// Sanitise the tags
                            //topicViewModel.Tags = _bannedWordService.SanitiseBannedWords(topicViewModel.Tags,
                            //    bannedWords);

                            //// Now add the tags
                            //_topicTagService.Add(topicViewModel.Tags.ToLower(), topic);
                        }


                        // 9. Subscribe the user to the topic if they have checked the checkbox
                        if (topicViewModel.SubscribeToTopic)
                            db_Forum.InsertUpdateTopicNotification(null, _Topic.Id, UserIDX);

                        // 10. Success so now send the emails
                        //NotifyNewTopics(category, topic, unitOfWork);

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



        public ActionResult ShowTopic(string slug, int? p)
        {
            // Set initial stuff
            //var pageIndex = p ?? 1;
            int UserIDX = db_Accounts.GetUserIDX();

            // Get the topic
            var _topic = db_Forum.GetTopic_fromSlug(slug);
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
                model.StarterPost = db_Forum.GetPost_StarterForTopic(_topic.Id, UserIDX);
                model.Posts = db_Forum.GetPost_NonStarterForTopic(_topic.Id, UserIDX);
                model.IsSubscribed = db_Forum.NotificationIsUserSubscribed(_topic.Id, UserIDX);
                model.LastPostDatePretty = db_Forum.GetTopic_LastPostPrettyDate(_topic.Id);


                //            var settings = SettingsService.GetSettings();
                //            var sortQuerystring = Request.QueryString[AppConstants.PostOrderBy];
                //            var orderBy = !string.IsNullOrEmpty(sortQuerystring) ?
                //                                      EnumUtils.ReturnEnumValueFromString<PostOrderBy>(sortQuerystring) : PostOrderBy.Standard;

                // Store the amount per page
                var amountPerPage = 50;


                //            // Get the posts
                //            var posts = _postService.GetPagedPostsByTopic(pageIndex,
                //                                                          amountPerPage,
                //                                                          int.MaxValue,
                //                                                          topic.Id,
                //                                                          orderBy);


                //            // Get the permissions for the category that this topic is in
                //            var permissions = RoleService.GetPermissions(topic.Category, UsersRole);


                //            // Set editor permissions
                //            ViewBag.ImageUploadType = permissions[SiteConstants.Instance.PermissionInsertEditorImages].IsTicked ? "forumimageinsert" : "image";

                //            var viewModel = ViewModelMapping.CreateTopicViewModel(topic, permissions, posts.ToList(), starterPost, posts.PageIndex, posts.TotalCount, posts.TotalPages, LoggedOnReadOnlyUser, settings, true);

                //            // If there is a quote querystring
                //            var quote = Request["quote"];
                //            if (!string.IsNullOrEmpty(quote))
                //            {
                //                try
                //                {
                //                    // Got a quote
                //                    var postToQuote = _postService.Get(new Guid(quote));
                //                    viewModel.QuotedPost = postToQuote.PostContent;
                //                    viewModel.ReplyTo = postToQuote.Id;
                //                    viewModel.ReplyToUsername = postToQuote.User.UserName;
                //                }
                //                catch (Exception ex)
                //                {
                //                    LoggingService.Error(ex);
                //                }
                //            }

                //            var reply = Request["reply"];
                //            if (!string.IsNullOrEmpty(reply))
                //            {
                //                try
                //                {
                //                    // Set the reply
                //                    var toReply = _postService.Get(new Guid(reply));
                //                    viewModel.ReplyTo = toReply.Id;
                //                    viewModel.ReplyToUsername = toReply.User.UserName;
                //                }
                //                catch (Exception ex)
                //                {
                //                    LoggingService.Error(ex);
                //                }
                //            }


                //            // Check the poll - To see if it has one, and whether it needs to be closed.
                //            if (viewModel.Poll?.Poll?.ClosePollAfterDays != null &&
                //                viewModel.Poll.Poll.ClosePollAfterDays > 0 &&
                //                !viewModel.Poll.Poll.IsClosed)
                //            {
                //                // Check the date the topic was created
                //                var endDate = viewModel.Poll.Poll.DateCreated.AddDays((int)viewModel.Poll.Poll.ClosePollAfterDays);
                //                if (DateTime.UtcNow > endDate)
                //                {
                //                    topic.Poll.IsClosed = true;
                //                    viewModel.Topic.Poll.IsClosed = true;
                //                    updateDatabase = true;
                //                }
                //            }

                return View(model);
            }
            else
            {
                TempData["Error"] = "No topic found";
                return RedirectToAction("Index", "Forum");
            }
            return null;
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
                var topics = db_Forum.GetTopicListOverviewByCategory(category.Category.Id);

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

    }
}