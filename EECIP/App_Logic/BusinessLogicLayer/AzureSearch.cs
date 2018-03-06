using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using EECIP.App_Logic.DataAccessLayer;
using Newtonsoft.Json;
using Microsoft.Rest.Azure;

namespace EECIP.App_Logic.BusinessLogicLayer
{
    // The SerializePropertyNamesAsCamelCase attribute is defined in the Azure Search .NET SDK.
    // It ensures that Pascal-case property names in the model class are mapped to camel-case
    // field names in the index.
   // [SerializePropertyNamesAsCamelCase]
    public partial class EECIP_Index
    {
        [System.ComponentModel.DataAnnotations.Key]
        [IsFilterable]
        public string KeyID { get; set; }

        [IsFilterable, IsFacetable]
        public string DataType { get; set; }

        [IsFilterable, IsFacetable]
        public string Record_Source { get; set; }

        [IsSearchable, IsFilterable]
        public string Agency { get; set; }

        [IsSearchable, IsFilterable]
        public string AgencyAbbreviation { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        public string State_or_Tribal { get; set; }

        [IsSearchable, IsSortable]
        [Analyzer(AnalyzerName.AsString.EnLucene)]
        public string Name { get; set; }

        [IsSearchable, IsFilterable]
        [Analyzer(AnalyzerName.AsString.EnLucene)]
        public string Description { get; set; }

        [IsFilterable, IsSortable, IsFacetable]
        public string Media { get; set; }

        [IsSearchable, IsFacetable, IsFilterable]
        [Analyzer(AnalyzerName.AsString.EnLucene)]
        public string[] Tags { get; set; }

        public string PersonPhone { get; set; }

        public string PersonEmail { get; set; }

        public string PersonLinkedIn { get; set; }

        [IsFilterable, IsFacetable]
        public string Population_Density { get; set; }

        [IsFilterable, IsFacetable]
        public string EPA_Region { get; set; }

        [IsFilterable, IsFacetable]
        public string Status { get; set; }

        [IsFilterable]
        public DateTime? LastUpdated { get; set; }
    }




    public class AzureSearch
    {

        public static void DeleteSearchIndex()
        {
            try
            {
                //connect to Azure Search
                SearchServiceClient serviceClient = CreateSearchServiceClient();

                if (serviceClient.Indexes.Exists("eecip"))
                    serviceClient.Indexes.Delete("eecip");

                if (serviceClient.SynonymMaps.Exists("desc-synonymmap"))
                    serviceClient.SynonymMaps.Delete("desc-synonymmap");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void CreateSearchIndex()
        {
            try
            {
                //connect to Azure Search
                SearchServiceClient serviceClient = CreateSearchServiceClient();

                //defining the suggester
                Suggester sg = new Suggester
                {
                    Name = "eecip_suggest",
                    SourceFields = new List<string>() { "Name" }
                };

                //defining the scoring profile //boosts items updated in the last 180 days
                ScoringProfile sp = new ScoringProfile {
                    Name = "date_scoring"
                };

                var freshnessFunction = new FreshnessScoringFunction() {
                    FieldName = "LastUpdated",
                    Boost = 20,
                    Parameters = new FreshnessScoringParameters(new TimeSpan(180, 0, 0, 0)),
                    Interpolation = ScoringFunctionInterpolation.Linear
                };
                // Assigns the freshness function to the scoring profile
                sp.Functions = new List<ScoringFunction>() { freshnessFunction };


                //define the index (the fields, and link in the suggester and scoring)
                var definition = new Index()
                {
                    Name = "eecip",
                    Fields = FieldBuilder.BuildForType<EECIP_Index>(),
                    Suggesters = new List<Suggester> { sg },
                    ScoringProfiles = new List<ScoringProfile>() { sp }
                };






                serviceClient.Indexes.Create(definition);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void UploadSynonyms()
        {
            try
            {
                //connect to Azure Search
                SearchServiceClient serviceClient = CreateSearchServiceClient();

                //grabbing synonyms
                string synstr = "";
                List<T_OE_REF_SYNONYMS> synlist = db_Ref.GetT_OE_REF_SYNONYMS();
                foreach (T_OE_REF_SYNONYMS syn in synlist)
                    synstr += syn.SYNONYM_TEXT + "\n";

                synstr = synstr.TrimEnd('\n');

                //adding synonyms
                var synonymMap = new SynonymMap()
                {
                    Name = "desc-synonymmap",
                    Format = "solr",
                    Synonyms = synstr
                };
                serviceClient.SynonymMaps.CreateOrUpdate(synonymMap);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static Index AddSynonymMapsToFields(Index index)
        {
            index.Fields.First(f => f.Name == "Name").SynonymMaps = new[] { "desc-synonymmap" };
            index.Fields.First(f => f.Name == "Description").SynonymMaps = new[] { "desc-synonymmap" };
            return index;
        }

        public static void EnableSynonyms()
        {
            try
            {
                int MaxNumTries = 3;

                for (int i = 0; i < MaxNumTries; ++i)
                {
                    try
                    {
                        //connect to Azure Search
                        SearchServiceClient serviceClient = CreateSearchServiceClient();

                        Index index = serviceClient.Indexes.Get("eecip");
                        index = AddSynonymMapsToFields(index);

                        // The IfNotChanged condition ensures that the index is updated only if the ETags match.
                        serviceClient.Indexes.CreateOrUpdate(index, accessCondition: AccessCondition.IfNotChanged(index));

                        Console.WriteLine("Updated the index successfully.\n");
                        break;
                    }
                    catch (CloudException e) when (e.IsAccessConditionFailed())
                    {
                        Console.WriteLine($"Index update failed : {e.Message}. Attempt({i}/{MaxNumTries}).\n");
                    }
                }



            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        private static SearchServiceClient CreateSearchServiceClient()
        {
            string searchServiceName = db_Ref.GetT_OE_APP_SETTING("AZURE_SEARCH_SVC_NAME");
            string adminApiKey = db_Ref.GetT_OE_APP_SETTING("AZURE_SEARCH_ADMIN_KEY");
            SearchServiceClient serviceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(adminApiKey));
            return serviceClient;
        }

        private static SearchServiceClient CreateSearchIndexClient()
        {
            string searchServiceName = db_Ref.GetT_OE_APP_SETTING("AZURE_SEARCH_SVC_NAME");
            string adminApiKey = db_Ref.GetT_OE_APP_SETTING("AZURE_SEARCH_QUERY_KEY");
            SearchServiceClient serviceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(adminApiKey));
            return serviceClient;
        }


        //******************************** METHODS FOR POPULATE INDEX ******************************************
        public static void PopulateSearchIndexProject(Guid? ProjectIDX)
        {
            try
            {
                //connect to Azure Search
                SearchServiceClient serviceClient = CreateSearchServiceClient();

                bool PendingRecs = true;

                while (PendingRecs) {
                    //get all projects needing to sync
                    List<EECIP_Index> _ps = db_EECIP.GetT_OE_PROJECTS_ReadyToSync(ProjectIDX);
                    if (_ps != null && _ps.Count > 0)
                    {
                        var batch = IndexBatch.Upload(_ps);

                        try
                        {
                            //send to azure
                            ISearchIndexClient indexClient = serviceClient.Indexes.GetClient("eecip");
                            indexClient.Documents.Index(batch);


                            //update local rec sync ind
                            foreach (EECIP_Index p in _ps)
                            {
                                Guid proj_idx = Guid.Parse(p.KeyID);
                                db_EECIP.InsertUpdatetT_OE_PROJECTS(proj_idx, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                                    null, null, null, null, null, true, true, null, null, false);
                            }

                        }
                        catch (IndexBatchException e)
                        {
                            // Sometimes when your Search service is under load, indexing will fail for some of the documents in
                            // the batch. Depending on your application, you can take compensating actions like delaying and
                            // retrying. For this simple demo, we just log the failed document keys and continue.
                            Console.WriteLine(
                                "Failed to index some of the documents: {0}",
                                String.Join(", ", e.IndexingResults.Where(r => !r.Succeeded).Select(r => r.Key)));
                            PendingRecs = false;
                            db_Ref.InsertT_OE_SYS_LOG("Search Pop1", e.Message.SubStringPlus(0,2000));
                            
                            return;
                        }
                    }
                    else
                        PendingRecs = false;
                }



            }
            catch (Exception ex)
            {
                db_Ref.InsertT_OE_SYS_LOG("Search Pop2", ex.Message.SubStringPlus(0, 2000));
                throw ex;
            }
        }

        public static void PopulateSearchIndexOrganization(Guid? OrgIDX)
        {
            try
            {
                //connect to Azure Search
                SearchServiceClient serviceClient = CreateSearchServiceClient();

                //get all projects needing to sync
                List<EECIP_Index> _ps = db_Ref.GetT_OE_ORGANIZATION_ReadyToSync(OrgIDX);
                if (_ps != null && _ps.Count > 0)
                {
                    var batch = IndexBatch.Upload(_ps);

                    try
                    {
                        ISearchIndexClient indexClient = serviceClient.Indexes.GetClient("eecip");
                        indexClient.Documents.Index(batch);
                    }
                    catch (IndexBatchException e)
                    {
                        // Sometimes when your Search service is under load, indexing will fail for some of the documents in
                        // the batch. Depending on your application, you can take compensating actions like delaying and
                        // retrying. For this simple demo, we just log the failed document keys and continue.
                        Console.WriteLine(
                            "Failed to index some of the documents: {0}",
                            String.Join(", ", e.IndexingResults.Where(r => !r.Succeeded).Select(r => r.Key)));
                    }
                }


            }
            catch (Exception ex)
            {
                db_Ref.InsertT_OE_SYS_LOG("AzureSearch", ex.InnerException.ToString().SubStringPlus(0, 2000));
            }
        }

        public static void PopulateSearchIndexEntServices(int? EntSvcIDX)
        {
            try
            {
                //connect to Azure Search
                SearchServiceClient serviceClient = CreateSearchServiceClient();

                //get all ent services needing to sync
                List<EECIP_Index> _ps = db_EECIP.GetT_OE_ORGANIZATION_ENT_SVCS_ReadyToSync(EntSvcIDX);
                if (_ps != null && _ps.Count > 0)
                {
                    var batch = IndexBatch.Upload(_ps);

                    try
                    {
                        //send to Azure
                        ISearchIndexClient indexClient = serviceClient.Indexes.GetClient("eecip");
                        indexClient.Documents.Index(batch);

                        //then update local rec sync ind
                        foreach (EECIP_Index p in _ps)
                        {
                            db_EECIP.InsertUpdatetT_OE_ORGANIZATION_ENT_SVCS(p.KeyID.ConvertOrDefault<int>()-100000, null, null, null, null, null, null, null, null, true, null, false);
                        }

                    }
                    catch (IndexBatchException e)
                    {
                        // Sometimes when your Search service is under load, indexing will fail for some of the documents in
                        // the batch. Depending on your application, you can take compensating actions like delaying and
                        // retrying. For this simple demo, we just log the failed document keys and continue.
                        Console.WriteLine(
                            "Failed to index some of the documents: {0}",
                            String.Join(", ", e.IndexingResults.Where(r => !r.Succeeded).Select(r => r.Key)));
                    }
                }


            }
            catch (Exception ex)
            {
                db_Ref.InsertT_OE_SYS_LOG("AzureSearch", ex.InnerException.ToString().SubStringPlus(0, 2000));
            }
        }

        public static void PopulateSearchIndexUsers(int? UserIDX)
        {
            try
            {
                //connect to Azure Search
                SearchServiceClient serviceClient = CreateSearchServiceClient();

                //get all projects needing to sync
                List<EECIP_Index> _ps = db_Accounts.GetT_OE_USERS_ReadyToSync(UserIDX);
                if (_ps != null && _ps.Count > 0)
                {
                    var batch = IndexBatch.Upload(_ps);

                    try
                    {
                        ISearchIndexClient indexClient = serviceClient.Indexes.GetClient("eecip");
                        indexClient.Documents.Index(batch);
                    }
                    catch (IndexBatchException e)
                    {
                        // Sometimes when your Search service is under load, indexing will fail for some of the documents in
                        // the batch. Depending on your application, you can take compensating actions like delaying and
                        // retrying. For this simple demo, we just log the failed document keys and continue.
                        Console.WriteLine(
                            "Failed to index some of the documents: {0}",
                            String.Join(", ", e.IndexingResults.Where(r => !r.Succeeded).Select(r => r.Key)));
                    }
                }


            }
            catch (Exception ex)
            {
                db_Ref.InsertT_OE_SYS_LOG("AzureSearch", ex.InnerException.ToString().SubStringPlus(0, 2000));
            }
        }

        public static void PopulateSearchIndexForumTopic(Guid? TopicIDX)
        {
            try
            {
                //connect to Azure Search
                SearchServiceClient serviceClient = CreateSearchServiceClient();

                bool PendingRecs = true;

                while (PendingRecs)
                {
                    //get all projects needing to sync
                    List<EECIP_Index> _ps = db_Forum.GetTopic_ReadyToSync(TopicIDX);
                    if (_ps != null && _ps.Count > 0)
                    {
                        var batch = IndexBatch.Upload(_ps);

                        try
                        {
                            //send to azure
                            ISearchIndexClient indexClient = serviceClient.Indexes.GetClient("eecip");
                            indexClient.Documents.Index(batch);

                            //update local rec sync ind
                            foreach (EECIP_Index p in _ps)
                            {
                                Guid topic_idx = Guid.Parse(p.KeyID);
                                db_Forum.UpdateTopic_SetSynced(topic_idx);
                            }
                        }
                        catch (IndexBatchException e)
                        {
                            // Sometimes when your Search service is under load, indexing will fail for some of the documents in
                            // the batch. Depending on your application, you can take compensating actions like delaying and
                            // retrying. For this simple demo, we just log the failed document keys and continue.
                            Console.WriteLine(
                                "Failed to index some of the documents: {0}",
                                String.Join(", ", e.IndexingResults.Where(r => !r.Succeeded).Select(r => r.Key)));
                            PendingRecs = false;
                        }
                    }
                    else
                        PendingRecs = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        //******************************** METHODS FOR DELETE ROW FROM INDEX ******************************************
        public static void DeleteSearchIndexProject(Guid? ProjectIDX)
        {
            try
            {
                //connect to Azure Search
                SearchServiceClient serviceClient = CreateSearchServiceClient();

                //get project needing to delete sync
                IEnumerable<string> ss = new List<string>() { ProjectIDX.ToString() };
                var batch = IndexBatch.Delete("KeyID", ss);

                try
                {
                    ISearchIndexClient indexClient = serviceClient.Indexes.GetClient("eecip");
                    indexClient.Documents.Index(batch);
                }
                catch (IndexBatchException e)
                {
                    // Sometimes when your Search service is under load, indexing will fail for some of the documents in
                    // the batch. Depending on your application, you can take compensating actions like delaying and
                    // retrying. For this simple demo, we just log the failed document keys and continue.
                    Console.WriteLine(
                        "Failed to index some of the documents: {0}",
                        String.Join(", ", e.IndexingResults.Where(r => !r.Succeeded).Select(r => r.Key)));
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void DeleteSearchIndexAgency(string OrgIDX)
        {
            try
            {
                //connect to Azure Search
                SearchServiceClient serviceClient = CreateSearchServiceClient();

                //get project needing to delete sync
                IEnumerable<string> ss = new List<string>() { OrgIDX };
                var batch = IndexBatch.Delete("KeyID", ss);

                try
                {
                    ISearchIndexClient indexClient = serviceClient.Indexes.GetClient("eecip");
                    indexClient.Documents.Index(batch);
                }
                catch (IndexBatchException e)
                {
                    // Sometimes when your Search service is under load, indexing will fail for some of the documents in
                    // the batch. Depending on your application, you can take compensating actions like delaying and
                    // retrying. For this simple demo, we just log the failed document keys and continue.
                    Console.WriteLine(
                        "Failed to index some of the documents: {0}",
                        String.Join(", ", e.IndexingResults.Where(r => !r.Succeeded).Select(r => r.Key)));
                }


            }
            catch (Exception ex)
            {
                db_Ref.InsertT_OE_SYS_LOG("AzureSearch", (ex.InnerException != null ? ex.InnerException.ToString() : ex.Message).SubStringPlus(0, 2000));
            }
        }

        public static void DeleteSearchIndexEntService(int OrgEntSvcIDX)
        {
            try
            {
                //connect to Azure Search
                SearchServiceClient serviceClient = CreateSearchServiceClient();

                //get ent service needing to delete sync
                IEnumerable<string> ss = new List<string>() { (OrgEntSvcIDX + 100000).ToString() };
                var batch = IndexBatch.Delete("KeyID", ss);

                try
                {
                    ISearchIndexClient indexClient = serviceClient.Indexes.GetClient("eecip");
                    indexClient.Documents.Index(batch);
                }
                catch (IndexBatchException e)
                {
                    // Sometimes when your Search service is under load, indexing will fail for some of the documents in
                    // the batch. Depending on your application, you can take compensating actions like delaying and
                    // retrying. For this simple demo, we just log the failed document keys and continue.
                    Console.WriteLine(
                        "Failed to index some of the documents: {0}",
                        String.Join(", ", e.IndexingResults.Where(r => !r.Succeeded).Select(r => r.Key)));
                }


            }
            catch (Exception ex)
            {
                db_Ref.InsertT_OE_SYS_LOG("AzureSearch", (ex.InnerException != null ? ex.InnerException.ToString() : ex.Message).SubStringPlus(0, 2000));
            }
        }

        public static void DeleteSearchIndexUsers(int UserIDX)
        {
            try
            {
                //connect to Azure Search
                SearchServiceClient serviceClient = CreateSearchServiceClient();

                //get ent service needing to delete sync
                IEnumerable<string> ss = new List<string>() { UserIDX.ToString() };
                var batch = IndexBatch.Delete("KeyID", ss);

                try
                {
                    ISearchIndexClient indexClient = serviceClient.Indexes.GetClient("eecip");
                    indexClient.Documents.Index(batch);
                }
                catch (IndexBatchException e)
                {
                    // Sometimes when your Search service is under load, indexing will fail for some of the documents in
                    // the batch. Depending on your application, you can take compensating actions like delaying and
                    // retrying. For this simple demo, we just log the failed document keys and continue.
                    Console.WriteLine(
                        "Failed to index some of the documents: {0}",
                        String.Join(", ", e.IndexingResults.Where(r => !r.Succeeded).Select(r => r.Key)));
                }


            }
            catch (Exception ex)
            {
                db_Ref.InsertT_OE_SYS_LOG("AzureSearch", (ex.InnerException != null ? ex.InnerException.ToString() : ex.Message).SubStringPlus(0, 2000));
            }
        }


        public static void DeleteForumTopic(Guid? TopicID)
        {
            try
            {
                //connect to Azure Search
                SearchServiceClient serviceClient = CreateSearchServiceClient();

                //get ent service needing to delete sync
                IEnumerable<string> ss = new List<string>() { TopicID.ToString() };
                var batch = IndexBatch.Delete("KeyID", ss);

                try
                {
                    ISearchIndexClient indexClient = serviceClient.Indexes.GetClient("eecip");
                    indexClient.Documents.Index(batch);
                }
                catch (IndexBatchException e)
                {
                    // Sometimes when your Search service is under load, indexing will fail for some of the documents in
                    // the batch. Depending on your application, you can take compensating actions like delaying and
                    // retrying. For this simple demo, we just log the failed document keys and continue.
                    Console.WriteLine(
                        "Failed to index some of the documents: {0}",
                        String.Join(", ", e.IndexingResults.Where(r => !r.Succeeded).Select(r => r.Key)));
                }


            }
            catch (Exception ex)
            {
                db_Ref.InsertT_OE_SYS_LOG("AzureSearch", (ex.InnerException != null ? ex.InnerException.ToString() : ex.Message).SubStringPlus(0, 2000));
            }
        }



        //******************************** METHODS FOR QUERYING INDEX ******************************************
        public static DocumentSearchResult<EECIP_Index> QuerySearchIndex(string searchStr, string dataTypeFacet = "", string mediaFacet = "", 
            string recordSourceFacet = "", string agencyFacet = "", string stateFacet = "", string tagsFacet = "", string popDensityFacet = "", 
            string regionFacet = "", string statusFacet = "", int? currentPage = 1, string sortType = "")
        {
            try
            {
                //connect to Azure Search
                SearchServiceClient serviceClient = CreateSearchServiceClient();
                ISearchIndexClient indexClient = serviceClient.Indexes.GetClient("eecip");

                //Search the entire index 
                SearchParameters parameters = new SearchParameters()
                {
                    Top = 50,
                    Skip = ((currentPage ?? 1) - 1) * 50,
                    Facets = new List<string> { "DataType", "State_or_Tribal,count:40", "Tags", "Status", "Record_Source", "Media", "EPA_Region", "Population_Density" },
                    Select = new[] { "KeyID", "DataType", "Record_Source", "Agency", "State_or_Tribal", "Name", "Description", "Media", "Tags", "Status", "PersonPhone", "PersonEmail", "PersonLinkedIn", "LastUpdated" },
                    IncludeTotalResultCount = true
                };

                //facet handling
                if ((dataTypeFacet ?? "").Length > 0)
                    parameters.Filter = "DataType eq '" + dataTypeFacet + "' ";
                if ((mediaFacet ?? "").Length > 0)
                    parameters.Filter = (parameters.Filter ?? "") + (parameters.Filter != null ? " and " : "") + "Media eq '" + mediaFacet + "' ";
                if ((recordSourceFacet ?? "").Length > 0)
                    parameters.Filter = (parameters.Filter ?? "") + (parameters.Filter != null ? " and " : "") + "Record_Source eq '" + recordSourceFacet + "' ";
                if ((agencyFacet ?? "").Length > 0)
                    parameters.Filter = (parameters.Filter ?? "") + (parameters.Filter != null ? " and " : "") + "Agency eq '" + agencyFacet + "' ";
                if ((stateFacet ?? "").Length > 0)
                    parameters.Filter = (parameters.Filter ?? "") + (parameters.Filter != null ? " and " : "") + "State_or_Tribal eq '" + stateFacet + "' ";
                if ((tagsFacet ?? "").Length > 0)
                    parameters.Filter = (parameters.Filter ?? "") + (parameters.Filter != null ? " and " : "") + "Tags/any(t: t eq '" + tagsFacet + "') ";
                if ((popDensityFacet ?? "").Length > 0)
                    parameters.Filter = (parameters.Filter ?? "") + (parameters.Filter != null ? " and " : "") + "Population_Density eq '" + popDensityFacet + "' ";
                if ((regionFacet ?? "").Length > 0)
                    parameters.Filter = (parameters.Filter ?? "") + (parameters.Filter != null ? " and " : "") + "EPA_Region eq '" + regionFacet + "' ";
                if ((statusFacet ?? "").Length > 0)
                    parameters.Filter = (parameters.Filter ?? "") + (parameters.Filter != null ? " and " : "") + "Status eq '" + statusFacet + "' ";

                //sort handling
                if (sortType == "alpha")
                    parameters.OrderBy = new List<string>() { "Name" };

                try
                {
                    DocumentSearchResult<EECIP_Index> results = indexClient.Documents.Search<EECIP_Index>(searchStr, parameters);
                    return results;
                }
                catch (IndexBatchException e) {
                    Console.WriteLine(
                        "Failed to index some of the documents: {0}",
                        String.Join(", ", e.IndexingResults.Where(r => !r.Succeeded).Select(r => r.Key)));
                    return null;
                }

            }
            catch (Exception ex)
            {
                db_Ref.InsertT_OE_SYS_LOG("AzureSearch", (ex.InnerException != null ? ex.InnerException.ToString() : ex.Message).SubStringPlus(0, 2000));
                return null;
            }
        }

        public static DocumentSuggestResult Suggest(string searchText, bool fuzzy)
        {
            // Execute search based on query string
            try
            {
                //connect to Azure Search
                SearchServiceClient serviceClient = CreateSearchServiceClient();
                ISearchIndexClient indexClient = serviceClient.Indexes.GetClient("eecip");

                SuggestParameters sp = new SuggestParameters()
                {
                    UseFuzzyMatching = fuzzy,
                    Top = 8
                };

                return indexClient.Documents.Suggest(searchText, "eecip_suggest", sp);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error querying index: {0}\r\n", ex.Message.ToString());
            }
            return null;
        }

    }
}