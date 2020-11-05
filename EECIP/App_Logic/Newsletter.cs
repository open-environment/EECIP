using System.Collections.Generic;
using EECIP.App_Logic.BusinessLogicLayer;
using EECIP.App_Logic.DataAccessLayer;


namespace EECIP.App_Logic
{
    public static class NewsletterClass
    {

        public static List<string> generateNewsletter(string emailOverride)
        {
            List<string> results = new List<string>();

            
            //get listing of all users 
            //only send newsletter for active users who have opted in to newsletter
            List<UserDisplayType> users = db_Accounts.GetT_OE_USERS_ForNewsLetter();

            foreach (UserDisplayType user in users)
            {
                //encrypt for email unsubscribe link
                string encryptOauth = new SimpleAES().Encrypt(user.users.PWD_HASH);
                encryptOauth = System.Web.HttpUtility.UrlEncode(encryptOauth);

                string emailHeader = @"<div style='width:100%; padding: 0; padding-top:10px; background-color: #393939; font-family: Helvetica, Arial, sans-serif; min-height: 42px;'>
                                                <a href='" + db_Ref.GetT_OE_APP_SETTING("PUBLIC_APP_PATH") + @"/Dashboard' style='font-size: 22px; color: #D9D9D9; padding: 14px 10px 12px; margin-left: 0; font-family: Helvetica, Arial, sans-serif; color: #d9d9d9; line-height: 20px; text-decoration: none;'>
                                                    <img src='https://www.eecip.net/Content/Images/seal.png' style='width:30px; padding-right:10px; vertical-align: middle'>
                                                    E-Enterprise Community Inventory Platform (EECIP)
                                                </a>
                                            </div>
                                        <br/><p style='font-family: Helvetica, Arial, sans-serif; margin-left:6px; '>Hello " + user.users.FNAME + @", here are recent updates from the E-Enterprise Community.</p>";
                
                string projectSnippet = "";
                string discussionSnippet = "";
                string emailFooter = @"<div style='width:100%; padding-top: 20px; padding-bottom: 20px; background-color: #393939; font-family: Helvetica, Arial, sans-serif; text-align:center; color: #D9D9D9'>
                                            <p>These news items can be personalized to match your interests. You can add or change subscriptions preferences on your <a href='" + db_Ref.GetT_OE_APP_SETTING("PUBLIC_APP_PATH") + @"/Account/UserProfile'>EECIP Profile</a></p>
                                            To learn more about EECIP, visit our <a href='" + db_Ref.GetT_OE_APP_SETTING("PUBLIC_APP_PATH") + @"/Dashboard' style='color: #D9D9D9; min-width: 150px;padding: 14px 10px 12px; margin-left: 0; font-family: Helvetica, Arial, sans-serif; color: #d9d9d9; line-height: 20px;'>website</a>
                                       <br/><br/><a href='" + db_Ref.GetT_OE_APP_SETTING("PUBLIC_APP_PATH") + @"/Home/Unsubscribe?ux=" + user.users.USER_IDX + "&key=" + encryptOauth + @"' style ='color: #D9D9D9; min-width: 150px;padding: 14px 10px 12px; margin-left: 0; font-family: Helvetica, Arial, sans-serif; color: #d9d9d9; line-height: 20px;'>Unsubscribe</a> from the EECIP newsletter.
                                        </div>";


                //get updated projects based on tags
                List<ProjectShortDisplayType> projs = db_EECIP.GetT_OE_PROJECTS_RecentlyUpdatedMatchingInterest(user.users.USER_IDX, 30, false, 10, null);
                if (projs != null && projs.Count > 0)
                    projectSnippet = popProjectSnippet(projs, true);

                //get updated discussion topics based on tags
                List<SP_RECENT_FORUM_BY_USER_TAG_Result_Expanded> topics = db_Forum.GetLatestTopicPostsMatchingInterestNew(user.users.USER_IDX, 30, 10, null);
                if (topics != null && topics.Count > 0)
                    discussionSnippet = popDiscussionSnippet(topics, true);

                //********************if no matches based on interest, then just grab those updated this month**********************************8
                if (projectSnippet.Length == 0 && discussionSnippet.Length == 0)
                {
                    List<ProjectShortDisplayType> projNoTags = db_EECIP.GetT_OE_PROJECTS_RecentlyUpdated(30, 10);
                    if (projNoTags != null && projNoTags.Count > 0)
                        projectSnippet = popProjectSnippet(projNoTags, false);

                    List<SP_RECENT_FORUM_BY_USER_TAG_Result_Expanded> topicsFallback = db_Forum.GetLatestTopicPostsFallback(30, 10);
                    if (topicsFallback != null && topicsFallback.Count > 0)
                        discussionSnippet = popDiscussionSnippet(topicsFallback, false);
                }

                //********* send email (if anything to send) ****************
                if (projectSnippet.Length > 0 || discussionSnippet.Length > 0)
                {
                    string htmlemail = emailHeader + projectSnippet + " <br/><br/>" + discussionSnippet + "<br/><br/>" + emailFooter;

                    Utils.SendEmail(null, emailOverride ?? user.users.EMAIL, null, null, "EECIP Newsletter", htmlemail, null, null, htmlemail);

                    db_Ref.InsertT_OE_SYS_EMAIL_LOG(null, user.users.EMAIL, null, "EECIP Newsletter", htmlemail, "Newsletter");

                    results.Add(user.users.EMAIL);
                }
                else
                {
                    //if no projects and discussions matching interests, then list 10 most recently updated

                }

            }

            return results;

        }

        private static string popProjectSnippet(List<ProjectShortDisplayType> projectResults, bool tagsInd)
        {
            string projectSnippet = @"<div style='font-family: Helvetica, Arial, sans-serif; font-size: 24px; margin-left:6px; margin-top:0px;'>Recent Projects " + (tagsInd ? "(matching your subscriptions)" : "") + @"</div>
                            <table style='margin: 6px; border: 1px solid #ddd; width: 90%; border-spacing: 2px; border-collapse: collapse; cellpadding=10px'>
                                <thead>
                                    <tr>
                                        <th style= 'color: #fff; background-color: #4f4f4f;'></th>
                                        <th style= 'color: #fff; background-color: #4f4f4f;'>Project Name</th>
                                        <th style= 'color: #fff; background-color: #4f4f4f;'>Agency</th>
                                        <th style= 'color: #fff; background-color: #4f4f4f;'>Last Updated</th>
                                    </tr>
                                </thead>
                                <tbody>";


            foreach (var item in projectResults)
            {
                projectSnippet += @"<tr style='background-color: #f9f9f9;'>
                                        <td style='border-top: 1px solid #ddd;'>
                                            <a href = '" + db_Ref.GetT_OE_APP_SETTING("PUBLIC_APP_PATH") + "/Dashboard/ProjectCard?strid=" + item.PROJECT_IDX + @"' style='color: #fff;background-color: #199c98; border-color: #1a8e8a; padding: 1px 5px; font-size: 12px; line-height: 1.5; border-radius: 3px; display: inline-block; border: 1px solid transparent; text-decoration: none;' >Info</a>
                                        </td>
                                        <td style='border-top: 1px solid #ddd;'>" + item.PROJ_NAME;

                //adding the tags for the matched topic
                if (tagsInd)
                {
                    foreach (string _tag in item.Tags)
                        projectSnippet += "<span style='background-color: #007AFF; font-size: 9.35px;padding: .4em .6em;font-weight: 700; color: #fff;border-radius: .25em;margin-left:10px '>" + _tag + @"</span>";
                }

                projectSnippet += @"</td>
                                        <td style='border-top: 1px solid #ddd;'>" + item.ORG_NAME + @"</td>
                                        <td style='border-top: 1px solid #ddd;'>" + item.LAST_ACTIVITY_DATE + @"</td>
                                    </tr>";
            }

            projectSnippet += @"</tbody>
                            </table>";

            return projectSnippet;
        }

        private static string popDiscussionSnippet(List<SP_RECENT_FORUM_BY_USER_TAG_Result_Expanded> topics, bool tagsInd)
        {
            string discussionSnippet = @"<div style='font-family: Helvetica, Arial, sans-serif; font-size: 24px; margin-left:6px; margin-top:0px;'>Recent Discussion Topics " + (tagsInd ? "(matching your subscriptions)" : "") + @"</div>
                            <table style='margin: 6px; border: 1px solid #ddd; width: 90%; border-spacing: 2px; border-collapse: collapse; cellpadding=10px'>
                                <thead>
                                    <tr >
                                        <th style= 'color: #fff; background-color: #4f4f4f;'></th>
                                        <th style= 'color: #fff; background-color: #4f4f4f;'>Topic</th>
                                        <th style= 'color: #fff; background-color: #4f4f4f;'>Post</th>
                                        <th style= 'color: #fff; background-color: #4f4f4f;'>Last Updated</th>
                                    </tr>
                                </thead>
                                <tbody>";


            foreach (var item in topics)
            {
                discussionSnippet += @"<tr style='background-color: #f9f9f9;'>
                                        <td style='border-top: 1px solid #ddd;'>
                                            <a href = '" + db_Ref.GetT_OE_APP_SETTING("PUBLIC_APP_PATH") + "/Forum/ShowTopic?slug=" + item.SP_RECENT_FORUM_BY_USER_TAG_Result.Slug + @"' style='color: #fff;background-color: #199c98; border-color: #1a8e8a; padding: 1px 5px; font-size: 12px; line-height: 1.5; border-radius: 3px; display: inline-block; border: 1px solid transparent; text-decoration: none;' >Info</a>
                                        </td>
                                        <td style='border-top: 1px solid #ddd;'>" + item.SP_RECENT_FORUM_BY_USER_TAG_Result.Name;


                //adding the tags for the matched topic
                if (tagsInd)
                {
                    foreach (string _tag in item.tags)
                        discussionSnippet += "<span style = 'background-color: #007AFF; font-size: 9.35px;padding: .4em .6em;font-weight: 700; color: #fff;border-radius: .25em;margin-left:10px'>" + _tag + "</span>";
                }

                discussionSnippet += @"</td>
                                        <td style='border-top: 1px solid #ddd;'>" + Utils.ReturnAmountWordsFromString(Utils.StripHtmlFromString(item.SP_RECENT_FORUM_BY_USER_TAG_Result.PostContent), 50) + @"</td>
                                        <td style='border-top: 1px solid #ddd;'>" + item.SP_RECENT_FORUM_BY_USER_TAG_Result.LatestPostDate + " by " + item.SP_RECENT_FORUM_BY_USER_TAG_Result.LatestPostUser + @"</td>
                                    </tr>";
            }

            discussionSnippet += @"</tbody>
                            </table>";

            return discussionSnippet;
        }
    }
}