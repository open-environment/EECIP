﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using EECIP.App_Logic.DataAccessLayer;
using Newtonsoft.Json;

namespace EECIP.App_Logic.BusinessLogicLayer
{
    // The SerializePropertyNamesAsCamelCase attribute is defined in the Azure Search .NET SDK.
    // It ensures that Pascal-case property names in the model class are mapped to camel-case
    // field names in the index.
    [SerializePropertyNamesAsCamelCase]
    public partial class EECIP_Index
    {
        [System.ComponentModel.DataAnnotations.Key]
        [IsFilterable]
        public string KeyID { get; set; }

        [IsFilterable, IsFacetable]
        public string DataType { get; set; }

        [IsFilterable, IsFacetable]
        public string RecordSource { get; set; }

        [IsSearchable, IsFacetable]
        public string Agency { get; set; }

        [IsSearchable]
        public string Name { get; set; }

        [IsSearchable, IsFilterable]
        public string Description { get; set; }

        [IsFilterable, IsSortable, IsFacetable]
        public string Media { get; set; }

        [IsSearchable, IsFacetable, IsFilterable]
        public string[] Tags { get; set; }
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
                sg.SearchMode = SuggesterSearchMode.AnalyzingInfixMatching;
                sg.SourceFields = new List<string>() { "name" };


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


        //******************************** METHODS FOR QUERYING INDEX ******************************************
        public static DocumentSearchResult<EECIP_Index> QuerySearchIndex(string searchStr)
        {
            try
            {
                //connect to Azure Search
                SearchServiceClient serviceClient = CreateSearchServiceClient();
                ISearchIndexClient indexClient = serviceClient.Indexes.GetClient("eecip");

                //Search the entire index 
                SearchParameters parameters = new SearchParameters()
                {
                    Facets = new List<string> { "dataType", "recordSource", "agency", "media", "tags" },
                    Select = new[] { "keyID", "dataType", "recordSource", "agency", "name", "description", "media", "tags" }
                };

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