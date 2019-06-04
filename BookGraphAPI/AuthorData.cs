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
using BookGraphAPI.Domain;
using Microsoft.Azure.Documents.Linq;
using System.Collections.Generic;
using System.Linq;

namespace BookGraphAPI
{
    public class AuthorData
    {
        private readonly DocumentClient _client;
        private string dbName = Environment.GetEnvironmentVariable("dbName");
        private string dbCollection = Environment.GetEnvironmentVariable("dbAuthorsCollection");
        public AuthorData(DocumentClient client)
        {
            _client = client;
        }

        [FunctionName("GetAuthorsAsync")]
        public async Task<IActionResult> GetAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "authors")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            FeedOptions queryOptions = new FeedOptions { EnableCrossPartitionQuery = true };

            var bookQuery = _client.CreateDocumentQuery<Author>(
                UriFactory.CreateDocumentCollectionUri(dbName, dbCollection), queryOptions)
                .AsDocumentQuery();

            List<Author> retVal = new List<Author>();

            while (bookQuery.HasMoreResults)
            {
                var results = await bookQuery.ExecuteNextAsync<Author>();
                retVal.AddRange(results);
            }

            return new JsonResult(retVal);
        }

        [FunctionName("GetAuthorsByLastNameAsync")]
        public async Task<IActionResult> GetAuthorsByLastNameAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "authors/{lastName}")] HttpRequest req,
            string lastName,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            FeedOptions queryOptions = new FeedOptions { EnableCrossPartitionQuery = true };

            var authorQuery = _client.CreateDocumentQuery<Author>(
                UriFactory.CreateDocumentCollectionUri(dbName, dbCollection), queryOptions)
                .Where(b => b.LastName == lastName).AsDocumentQuery();

            List<Author> retVal = new List<Author>();

            while (authorQuery.HasMoreResults)
            {
                var results = await authorQuery.ExecuteNextAsync<Author>();
                retVal.AddRange(results);
            }

            return new JsonResult(retVal);
        }

        [FunctionName("GetAuthorsByIdAsync")]
        public async Task<IActionResult> GetAuthorsByIdAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "authorbyid/{id}")] HttpRequest req,
            string id,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            RequestOptions queryOptions = new RequestOptions { PartitionKey = new Microsoft.Azure.Documents.PartitionKey(id) };

            var response = await _client.ReadDocumentAsync<Author>(UriFactory.CreateDocumentUri(dbName, dbCollection, id), queryOptions);
            var retVal = response.Document;

            log.LogInformation(retVal.LastName);

            return new JsonResult(retVal);
        }

    }
}
