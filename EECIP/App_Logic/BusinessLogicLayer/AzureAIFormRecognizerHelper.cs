using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using Azure.AI.TextAnalytics;
using System.Net;

namespace EECIP.App_Logic.BusinessLogicLayer
{
    class AzureAIFormRecognizerHelper
    {
        public async Task<string> SummarizeDocumentAsync(byte[] fileBytes)
        {
            //needed info (get from pwd2 file)
            string formRecognizerEndPoint = "";
            string formRecognizerApiKey = "";
            string languageEndPoint = "";
            string languageApiKey = "";
            int tokenLimit = 1024;

            // Step 1: Extract Text
            var extractedText = await ExtractTextAsync(fileBytes, formRecognizerEndPoint, formRecognizerApiKey);

            // Step 2: Split Text
            var chunks = SplitText(extractedText, tokenLimit);

            //// Step 3: Summarize Each Chunk
            var summaries = new List<string>();
            foreach (var chunk in chunks)
            {
                var summary = await SummarizeTextAsync(chunk, languageEndPoint, languageApiKey);
                summaries.Add(summary);
            }

            // Step 4: Combine Summaries
            //return CombineSummaries(summaries);

            return null;
        }



        public async Task<string> ExtractTextAsync(byte[] fileBytes, string endpoint, string apiKey)
        {
            var client = new DocumentAnalysisClient(new Uri(endpoint), new AzureKeyCredential(apiKey));

            Stream stream = new MemoryStream(fileBytes);

            AnalyzeDocumentOperation operation = await client.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-read", stream);
            var result = operation.Value;

            var extractedText = new StringBuilder();
            foreach (var page in result.Pages)
            {
                foreach (var line in page.Lines)
                {
                    extractedText.AppendLine(line.Content);
                }
            }

            foreach (var para in result.Paragraphs) { 
                extractedText.AppendLine(para.Content);
            }

            var xxx = extractedText.ToString();
            return xxx;
        }


        public List<string> SplitText(string text, int maxTokens)
        {
            var words = text.Split(' ');
            var chunks = new List<string>();
            var currentChunk = new StringBuilder();

            foreach (var word in words)
            {
                if ((currentChunk.Length + word.Length + 1) / 4 > maxTokens) // Approx. 4 chars per token
                {
                    chunks.Add(currentChunk.ToString().Trim());
                    currentChunk.Clear();
                }

                currentChunk.Append(word + " ");
            }

            if (currentChunk.Length > 0)
                chunks.Add(currentChunk.ToString().Trim());

            return chunks;
        }


        // Example method for summarizing text
        public async Task<string> SummarizeTextAsync(string text, string languageEndPoint, string languageApiKey)
        {
            string summaryText = "";
            var client = new TextAnalyticsClient(new Uri(languageEndPoint), new AzureKeyCredential(languageApiKey));

            // Prepare analyze operation input. You can add multiple documents to this list and perform the same operation to all of them.
            var batchInput = new List<string>
            {
                text
            };

            TextAnalyticsActions actions = new TextAnalyticsActions()
            {
                AbstractiveSummarizeActions = new List<AbstractiveSummarizeAction>() { new AbstractiveSummarizeAction( ) }
            };

            // Start analysis process.
            AnalyzeActionsOperation operation = await client.StartAnalyzeActionsAsync(batchInput, actions);
            await operation.WaitForCompletionAsync();


            // View operation results.
            //await foreach (AnalyzeActionsResult documentsInPage in operation.Value)
            //{
            //    IReadOnlyCollection<ExtractiveSummarizeActionResult> summaryResults = documentsInPage.ExtractiveSummarizeResults;

            //    foreach (ExtractiveSummarizeActionResult summaryActionResults in summaryResults)
            //    {
            //        if (summaryActionResults.HasError)
            //        {
            //            Console.WriteLine($"  Error!");
            //            Console.WriteLine($"  Action error code: {summaryActionResults.Error.ErrorCode}.");
            //            Console.WriteLine($"  Message: {summaryActionResults.Error.Message}");
            //            continue;
            //        }

            //        foreach (ExtractiveSummarizeResult documentResults in summaryActionResults.DocumentsResults)
            //        {
            //            if (documentResults.HasError)
            //            {
            //                Console.WriteLine($"  Error!");
            //                Console.WriteLine($"  Document error code: {documentResults.Error.ErrorCode}.");
            //                Console.WriteLine($"  Message: {documentResults.Error.Message}");
            //                continue;
            //            }

            //            Console.WriteLine($"  Extracted the following {documentResults.Sentences.Count} sentence(s):");
            //            Console.WriteLine();

            //            foreach (ExtractiveSummarySentence sentence in documentResults.Sentences)
            //            {
            //                summaryText += sentence.Text;
            //                Console.WriteLine($"  Sentence: {sentence.Text}");
            //                Console.WriteLine();
            //            }
            //        }
            //    }
            //}


            //await foreach (AnalyzeActionsResult documentsInPage in operation.Value)
            //{
            //    int xxx = 1;

            //    //IReadOnlyCollection<AbstractiveSummarizeResult> summaryResults = documentsInPage.AbstractiveSummarizeResult;
            //}
            return summaryText;
        }

        //async Task<string> SummarizeTextAsync(string text, string openAiApiKey)
        //{
        //    // Initialize the client
        //    var client = new OpenAIClient(openAiApiKey);

        //    var completionOptions = new options
        //    {
        //        Prompts = { $"Summarize the following text in a single paragraph:\n\n{text}" },
        //        MaxTokens = 150, // Adjust based on required summary length
        //        Temperature = 0.7,
        //    };

        //    var chatOptions = new ChatCompletionOptions()
        //    {
        //        Messages =
        //{
        //    new ChatMessage(ChatRole.System, "You are a helpful assistant."),
        //    new ChatMessage(ChatRole.User, $"Summarize the following text in a single paragraph:\n\n{text}")
        //},
        //        MaxTokens = 150, // Adjust summary length
        //        Temperature = 0.7,
        //    };

        //    var completionOptions = new ChatCompletionOptions
        //    {
        //        Prompt = $"Summarize the following text in a single paragraph:\n\n{text}",
        //        MaxTokens = 150, // Adjust based on required summary length
        //        Temperature = 0.7,
        //    };

        //    var completionsResponse = await client.GetCompletionsAsync("gpt-35-turbo", completionOptions);
        //    return completionsResponse.Value.Choices[0].Text.Trim();
        //}

    }


}