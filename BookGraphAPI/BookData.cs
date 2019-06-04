using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Documents.Client;
using System.Linq;
using BookGraphAPI.Domain;
using Microsoft.Azure.Documents.Linq;
using System.Collections.Generic;

namespace BookGraphAPI
{
    public class BookData
    {
        private readonly DocumentClient _client;
        private string dbName = Environment.GetEnvironmentVariable("dbName");
        private string dbCollection = Environment.GetEnvironmentVariable("dbBooksCollection");
        public BookData(DocumentClient client)
        {
            _client = client;
        }

        [FunctionName("GetBooksAsync")]
        public async Task<IActionResult> GetAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "books")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            FeedOptions queryOptions = new FeedOptions { EnableCrossPartitionQuery = true };

            var bookQuery = _client.CreateDocumentQuery<Book>(
                UriFactory.CreateDocumentCollectionUri(dbName, dbCollection), queryOptions)
                .AsDocumentQuery();

            List<Book> retVal = new List<Book>();

            while (bookQuery.HasMoreResults)
            {
                var results = await bookQuery.ExecuteNextAsync<Book>();
                retVal.AddRange(results);
            }

            return new JsonResult(retVal);
        }

        [FunctionName("GetBooksByTitleAsync")]
        public async Task<IActionResult> GetBooksByTitleAsyncGetAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "books/{title}")] HttpRequest req,
            string title,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            FeedOptions queryOptions = new FeedOptions { EnableCrossPartitionQuery = true };

            var bookQuery = _client.CreateDocumentQuery<Book>(
                UriFactory.CreateDocumentCollectionUri(dbName, dbCollection), queryOptions)
                .Where(b=> b.Title == title).AsDocumentQuery();

            List<Book> retVal = new List<Book>();

            while (bookQuery.HasMoreResults)
            {
                var results = await bookQuery.ExecuteNextAsync<Book>();
                retVal.AddRange(results);
            }

            return new JsonResult(retVal);
        }

        [FunctionName("GetBooksByAuthorIdAsync")]
        public async Task<IActionResult> GetBooksByAuthorAsyncGetAsync(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "bookbyauthor/{authorId}")] HttpRequest req,
           string authorId,
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            FeedOptions queryOptions = new FeedOptions { EnableCrossPartitionQuery = true };

            var bookQuery = _client.CreateDocumentQuery<Book>(
                UriFactory.CreateDocumentCollectionUri(dbName, dbCollection), queryOptions)
                .Where(b => b.AuthorId == Guid.Parse(authorId)).AsDocumentQuery();

            List<Book> retVal = new List<Book>();

            while (bookQuery.HasMoreResults)
            {
                var results = await bookQuery.ExecuteNextAsync<Book>();
                retVal.AddRange(results);
            }

            return new JsonResult(retVal);
        }

    }
}
