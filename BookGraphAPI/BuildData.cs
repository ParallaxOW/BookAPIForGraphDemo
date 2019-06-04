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
using System.Collections.Generic;

namespace BookGraphAPI
{
    public class PostBookData
    {
        private static DocumentClient _client;

        public PostBookData(DocumentClient client)
        {
            _client = client;
        }

        [FunctionName("BuildBooks")]
        public async Task<IActionResult> BuildBooks(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "book")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("BuildBooks processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            List<Book> data = JsonConvert.DeserializeObject<List<Book>>(requestBody);

            string dbName = Environment.GetEnvironmentVariable("dbName");
            string dbCollection = Environment.GetEnvironmentVariable("dbBooksCollection");

            List<Book> outList = new List<Book>();
            foreach(Book b in data)
            {
                var doc = await _client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(dbName, dbCollection), b);
                var createdBook = JsonConvert.DeserializeObject<Book>(doc.Resource.ToString());
                outList.Add(createdBook);
            }

            return new CreatedResult("Books Collection", outList);
        }

        [FunctionName("BuildAuthors")]
        public async Task<IActionResult> BuildAuthors(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "author")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("BuildAuthors processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Author data = JsonConvert.DeserializeObject<Author>(requestBody);

            string dbName = Environment.GetEnvironmentVariable("dbName");
            string dbCollection = Environment.GetEnvironmentVariable("dbAuthorsCollection");

            var doc = await _client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(dbName, dbCollection), data);
            //var doc = await _client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(dbName, dbCollection), data);
            data = JsonConvert.DeserializeObject<Author>(doc.Resource.ToString());

            return new CreatedResult("Authors Collection", data);
        }
    }
}
