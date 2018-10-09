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
                                        <br/><p style='font-family: Helvetica, Arial, sans-serif; margin-left:6px; '>Hello " + user.users.FNAME + " " + user.users.LNAME + @", here are recent updates from the E-Enterprise Community</p>";
                string projectSnippet = "";
                string discussionSnippet = "";
                string emailFooter = @"<div style='width:100%; padding-top: 20px; padding-bottom: 20px; background-color: #393939; font-family: Helvetica, Arial, sans-serif; text-align:center; color: #D9D9D9'>
                                            To learn more about EECIP, visit our <a href='" + db_Ref.GetT_OE_APP_SETTING("PUBLIC_APP_PATH") + @"/Dashboard' style='color: #D9D9D9; min-width: 150px;padding: 14px 10px 12px; margin-left: 0; font-family: Helvetica, Arial, sans-serif; color: #d9d9d9; line-height: 20px;'>website</a>
                                       <br/><br/><a href='" + db_Ref.GetT_OE_APP_SETTING("PUBLIC_APP_PATH") + @"/Home/Unsubscribe?ux=" + user.users.USER_IDX + "&key=" + encryptOauth + @"' style ='color: #D9D9D9; min-width: 150px;padding: 14px 10px 12px; margin-left: 0; font-family: Helvetica, Arial, sans-serif; color: #d9d9d9; line-height: 20px;'>Unsubscribe</a> from the EECIP newsletter.
                                        </div>";

                //get updated projects based on tags
                List<ProjectShortDisplayType> projs = db_EECIP.GetT_OE_PROJECTS_RecentlyUpdatedMatchingInterest(user.users.USER_IDX, 30, false, 10, null);
                if (projs != null && projs.Count > 0)
                    projectSnippet = popProjectSnippet(projs);


                //get updated discussion topics based on tags
                List<TopicOverviewDisplay> topics = db_Forum.GetLatestTopicPostsMatchingInterestNewsletter(user.users.USER_IDX, 30, 10);
                if (topics != null && topics.Count > 0)
                    discussionSnippet = popDiscussionSnippet(topics);


                //********* send email (only if matches) ****************
                if (projectSnippet.Length > 0 || discussionSnippet.Length > 0)
                {
                    string htmlemail = emailHeader + projectSnippet + " <br/><br/>" + discussionSnippet + "<br/><br/>" + emailFooter;
                    Utils.SendEmail(null, emailOverride ?? user.users.EMAIL, null, null, "EECIP Newsletter", htmlemail, null, null, htmlemail);

                    db_Ref.InsertT_OE_SYS_EMAIL_LOG(null, user.users.EMAIL, null, "EECIP Newsletter", htmlemail, "Newsletter");

                    results.Add(user.users.EMAIL);
                }

            }

            return results;

        }

        private static string popProjectSnippet(List<ProjectShortDisplayType> projectResults)
        {
            string projectSnippet = @"<div style='font-family: Helvetica, Arial, sans-serif; font-size: 24px; margin-left:6px; margin-top:0px;'>Recent Projects (matching your subscriptions)</div>
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
                                        <td style='border-top: 1px solid #ddd;'>" + item.PROJ_NAME + @"<span style='background-color: #007AFF; font-size: 9.35px;padding: .4em .6em;font-weight: 700; color: #fff;border-radius: .25em;margin-left:10px '>" + item.Tag + @"</span></td>
                                        <td style='border-top: 1px solid #ddd;'>" + item.ORG_NAME + @"</td>
                                        <td style='border-top: 1px solid #ddd;'>" + item.LAST_ACTIVITY_DATE + @"</td>
                                    </tr>";
            }

            projectSnippet += @"</tbody>
                            </table>";

            return projectSnippet;
        }

        private static string popDiscussionSnippet(List<TopicOverviewDisplay> topics)
        {
            string discussionSnippet = @"<div style='font-family: Helvetica, Arial, sans-serif; font-size: 24px; margin-left:6px; margin-top:0px;'>Recent Discussion Topics (matching your subscriptions)</div>
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
                                            <a href = '" + db_Ref.GetT_OE_APP_SETTING("PUBLIC_APP_PATH") + "/Forum/ShowTopic?slug=" + item._topic.Slug + @"' style='color: #fff;background-color: #199c98; border-color: #1a8e8a; padding: 1px 5px; font-size: 12px; line-height: 1.5; border-radius: 3px; display: inline-block; border: 1px solid transparent; text-decoration: none;' >Info</a>
                                        </td>
                                        <td style='border-top: 1px solid #ddd;'>" + item._topic.Name + @"<span style='background-color: #007AFF; font-size: 9.35px;padding: .4em .6em;font-weight: 700; color: #fff;border-radius: .25em;margin-left:10px '>" + item.CategorySlug + @"</span></td>
                                        <td style='border-top: 1px solid #ddd;'>" + Utils.ReturnAmountWordsFromString(Utils.StripHtmlFromString(item._postStart.PostContent), 50) + @"</td>
                                        <td style='border-top: 1px solid #ddd;'>" + item._postLatest.Post.DateCreated + " by " + item._postLatest.PosterDisplayName + @"</td>
                                    </tr>";
            }

            discussionSnippet += @"</tbody>
                            </table>";

            return discussionSnippet;



            //string discussionSnippet = @"<div style='font-family: Helvetica, Arial, sans-serif; font-size: 24px; margin-top:0px;'>Recent Discussion Topics (matching your subscription)</div>
            //                             <section class='panel panel-default'>
            //                                <div class='panel-body'>";

            //foreach (var item in topics)
            //{
            //    discussionSnippet += @"<div class='topicrow' style='padding: 5px; border-bottom: solid #f2f2f2 2px; font-family: Helvetica, Arial, sans-serif; '>
            //                                <div class='rowdetails' style=''>
            //                                    <h3 style='margin: 6px 0; padding: 0;'>
            //                                        <a href='/Forum/ShowTopic?slug=should-the-eecip-be-a-relying-party-onto-the-ee-fim' style='font-size:18px;'>
            //                                            Should the EECIP be a Relying Party onto the EE-FIM?
            //                                        </a>
            //                                    </h3>
            //                                    If the Inventory were integrated into the EE-Federated Identity Management system a user logged onto the EE-FIM could traverse to the Inventory without logging in (after an initial one-time registration process).&nbsp;
            //                                        <span style='background-color: #007AFF; font-size: 9.35px;padding: .4em .6em;font-weight: 700; color: #fff;border-radius: .25em;'>Identity Management</span>
            //                                    <p class='topicrowfooterinfo'>
            //                                        Latest By Chris Simmers 17 April 2018
            //                                    </p>
            //                                </div>
            //                            </div>";
            //}

            //discussionSnippet += "</div></section>";

            //return discussionSnippet;
        }
    }
}