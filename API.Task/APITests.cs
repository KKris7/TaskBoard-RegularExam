using NUnit.Framework;
using RestSharp;
using System.Net;
using System.Text.Json;

namespace API.Task
{
    public class APITests
    {
        //private const string URL = "http://localhost:8080/api";
       private const string URL = "https://taskboard.nakov.repl.co/api";

        private RestClient client;
        private RestRequest request;


        [SetUp]
        public void Setup()
        {
            this.client = new RestClient(URL);
        }

        [Test]
        public void TestGetALLDoneTasks()
        {
            // arrange
            this.request = new RestRequest(URL + "/tasks/board/done");

            // act
            var response = this.client.Execute(request, Method.Get);
            var task = JsonSerializer.Deserialize<List<Tasks>>(response.Content);


            // assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(task[0].title, Is.EqualTo("Project skeleton"));
        }

        [Test]
        public void TestFindTaskKeyword()
        {
            // arrange
            this.request = new RestRequest(URL + "/tasks/search/home");

            // act
            var response = this.client.Execute(request, Method.Get);
            var tasks = JsonSerializer.Deserialize<List<Tasks>>(response.Content);


            // assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(tasks[0].title, Is.EqualTo("Home page"));
        }
        [Test]
        public void TestNewTaskInvalidData()
        {
            // arrange
            this.request = new RestRequest(URL + "/tasks");

            // act
            var response = this.client.Execute(request, Method.Post);

            // assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content, Is.EqualTo("{\"errMsg\":\"Title cannot be empty!\"}"));
        }

        [Test]
        public void TestFindTaskWithMissingKeyword()
        {
            // arrange
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            string randnumString = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray());

            this.request = new RestRequest(URL + "/tasks/search/" + randnumString.ToString());

            // act
            var response = this.client.Execute(request, Method.Get);
            var tasks = JsonSerializer.Deserialize<List<Tasks>>(response.Content);

            // assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.IsEmpty(tasks);
            Assert.That(tasks.Count, Is.EqualTo(0));
        }

        [Test]
        public void TestValidData()
        {
            // arrange
            this.request = new RestRequest(URL + "/tasks");

            var body = new
            {
                title = "Valid" + DateTime.Now.Ticks,
                
            };
            request.AddJsonBody(body);

            // act
            var response = this.client.Execute(request, Method.Post);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            response = this.client.Execute(request, Method.Get);

            var task = JsonSerializer.Deserialize<List<Tasks>>(response.Content);
            var lastTask = task.Last();

            // assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(lastTask.title, Is.EqualTo(body.title));
        }
    }
}