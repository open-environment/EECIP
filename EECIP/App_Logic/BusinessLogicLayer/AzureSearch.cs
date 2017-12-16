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

        [IsSearchable, IsFilterable, IsFacetable]
        public string Agency { get; set; }

        [IsSearchable]
        public string Name { get; set; }

        [IsSearchable, IsFilterable]
        public string Description { get; set; }

        [IsFilterable, IsSortable, IsFacetable]
        public string Media { get; set; }

        [IsSearchable, IsFacetable, IsFilterable]
        public string[] Tags { get; set; }

        public string PersonPhone { get; set; }
        public string PersonEmail { get; set; }
        public string PersonLinkedIn { get; set; }
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
                Suggester sg = new Suggester();
                sg.Name = "eecip_suggest";
                //sg.SearchMode = SuggesterSearchMode.AnalyzingInfixMatching;
                sg.SourceFields = new List<string>() { "Name" };


                var definition = new Index()
                {
                    Name = "eecip",
                    Fields = FieldBuilder.BuildForType<EECIP_Index>(),
                    Suggesters = new List<Suggester> { sg }
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

                //get all projects needing to sync
                List<EECIP_Index> _ps = db_EECIP.GetT_OE_PROJECTS_ReadyToSync(ProjectIDX);
                if (_ps != null)
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
                if (_ps != null)
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
                throw ex;
            }
        }

        public static void PopulateSearchIndexEntServices(int? EntSvcIDX)
        {
            try
            {
                //connect to Azure Search
                SearchServiceClient serviceClient = CreateSearchServiceClient();

                //get all projects needing to sync
                List<EECIP_Index> _ps = db_EECIP.GetT_OE_ORGANIZATION_ENT_SVCS_ReadyToSync(EntSvcIDX);
                if (_ps != null)
                {
                    if (_ps.Count > 0)
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


            }
            catch (Exception ex)
            {
                throw ex;
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
                if (_ps != null)
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
                throw ex;
            }
        }



        //******************************** METHODS FOR QUERYING INDEX ******************************************
        public static DocumentSearchResult<EECIP_Index> QuerySearchIndex(string searchStr, string dataTypeFacet = "", string mediaFacet = "", 
            string recordSourceFacet = "", string agencyFacet = "", string tagsFacet = "", int? currentPage = 1)
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
                    Facets = new List<string> { "DataType", "Record_Source", "Agency", "Media", "Tags" },
                    Select = new[] { "KeyID", "DataType", "Record_Source", "Agency", "Name", "Description", "Media", "Tags", "PersonPhone", "PersonEmail", "PersonLinkedIn" },
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
                if ((tagsFacet ?? "").Length > 0)
                    parameters.Filter = (parameters.Filter ?? "") + (parameters.Filter != null ? " and " : "") + "Tags/any(t: t eq '" + tagsFacet + "') ";

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
                throw ex;
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