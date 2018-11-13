using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HireToRetire.Models;
using HireToRetire.Service;
using Microsoft.AspNetCore.Mvc;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Identity.Client;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;

namespace HireToRetire.Controllers
{
    [Authorize]
    public class CandidateController : Controller
    {
        string domain = "azdemoapimgnt.azure-api.net/candidatereg";
        string clientId = "0d36c971-15e4-4453-9e1f-2a44deb5b31e";
        string authority = "https://login.microsoftonline.com/tfp/capapps.onmicrosoft.com/B2C_1_SignUpIn/v2.0/.well-known/openid-configuration";
        string redirectUri = "http://localhost:32774/signin-oidc";
        string clientSecret = "M3.653[FaHr)E70Gx1D>w1E-";
        string apiEndpoint = "http://azdemoapimgnt.azure-api.net/candidatereg/api/Candidates/Test";

        public async Task<string> Index()
        {
            try
            {
                // Retrieve the token with the specified scopes
                var scope = new string[] { "https://CapApps.onmicrosoft.com/cr-api/read" };
                string signedInUserID = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                TokenCache userTokenCache = new MSALSessionCache(signedInUserID, this.HttpContext).GetMsalCacheInstance();
                ConfidentialClientApplication cca = new ConfidentialClientApplication(clientId, authority, redirectUri, new ClientCredential(clientSecret), userTokenCache, null);

                //var user = cca.Users.FirstOrDefault();
                var user = cca.GetAccountsAsync().Result.FirstOrDefault();
                if (user == null)
                {
                    throw new Exception("The User is NULL.  Please clear your cookies and try again.");
                }

                AuthenticationResult result = await cca.AcquireTokenSilentAsync(scope, user, authority, false);

                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, apiEndpoint);

                // Add token to the Authorization header and make the request
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
                request.Headers.Add("Ocp-Apim-Subscription-Key", "d8f0219eb112429ea89804114d4aff2a");
                request.Headers.Add("Ocp-Apim-Trace", "true");
                HttpResponseMessage response = await client.SendAsync(request);

                // Handle the response
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        String responseString = await response.Content.ReadAsStringAsync();
                        return responseString;
                    case HttpStatusCode.Unauthorized:
                        String errorString = await response.Content.ReadAsStringAsync();
                        return "Please sign in again. " + response.ReasonPhrase + " - " + errorString;
                    default:
                        return "Error. Status code = " + response.StatusCode + ": " + response.ReasonPhrase;
                }
            }
            catch (Exception ex)
            {
                return "Error reading: " + ex.Message;
            }
        }

        //public IActionResult Index()
        //{
        //    List<CandidateViewModel> candidates = new ApiService(new Uri($"https://{domain}"))
        //        .GetAsync<List<CandidateViewModel>>(new Uri($"https://{domain}/api/candidates")).Result;

        //    return View(candidates);
        //}

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult CreateSave(CandidateViewModel candidate)
        {
            // call api to save
            //new ApiService(new Uri($"http://{domain}")).PostAsync<CandidateViewModel>(new Uri($"http://{domain}/api/candidates"), candidate);

            try
            {
                KPub(JsonConvert.SerializeObject(candidate));
            }
            catch (Exception)
            {
                throw;
                //return View("Home/Error");
            }

            ViewData["Message"] = "Candidate successfully registered";
            return View("Create");
        }

        public IActionResult Edit(CandidateViewModel candidate)
        {
            return View(candidate);
        }

        public IActionResult EditSave(CandidateViewModel candidate)
        {
            // call api to update
            new ApiService(new Uri($"https://{domain}")).PutAsync<CandidateViewModel>(new Uri($"https://{domain}/api/candidates/{candidate.Id}"), candidate);
            ViewData["Message"] = "Candidate successfully updated";
            return View("Edit");
        }

        public IActionResult Details(CandidateViewModel candidate)
        {
            return View(candidate);
        }

        public IActionResult Delete(CandidateViewModel candidate)
        {
            return View(candidate);
        }

        public IActionResult DeleteSave(CandidateViewModel candidate)
        {
            // call api to delete
            new ApiService(new Uri($"https://{domain}")).DeleteAsync<CandidateViewModel>(new Uri($"https://{domain}/api/candidates/{candidate.Id}"));
            ViewData["Message"] = "Candidate successfully deleted";
            return View("Delete");
        }

        public List<string> Check()
        {
            List<string> ret = new List<string>();
            try
            {
                //List<string> ret = new List<string>();
                var config = new Dictionary<string, object>
                {
                    { "group.id", "hiretoretire" },
                    { "bootstrap.servers", "confkafka-cp-kafka:9092" },
                    { "enable.auto.commit", "false"}
                };

                using (var consumer = new Consumer<Null, string>(config, null, new StringDeserializer(Encoding.UTF8)))
                {
                    consumer.Subscribe(new string[] { "candidate-topic" });

                    //Message<Null, string> msg;

                    //consumer.Consume(out msg, 1000);

                    //ret.Add($"Topic: {msg.Topic} Partition: {msg.Partition} Offset: {msg.Offset} {msg.Value}");

                    consumer.OnMessage += (_, msg) =>
                    {
                        ret.Add($"Topic: {msg.Topic} Partition: {msg.Partition} Offset: {msg.Offset} {msg.Value}");
                        //Console.WriteLine($"Topic: {msg.Topic} Partition: {msg.Partition} Offset: {msg.Offset} {msg.Value}");
                        consumer.CommitAsync(msg);
                    };

                    for (int i = 1; i <= 500; i++)
                    {
                        consumer.Poll(100);
                    }
                }

                return ret;
            }
            catch (Exception ex)
            {
                ret.Add(ex.ToString());
                return ret;
            }
        }

        private void KPub(string data)
        {
            string topicName = "candidate-topic";

            var config = new Dictionary<string, object>
            {
                { "bootstrap.servers", "confkafka-cp-kafka:9092" }
            };

            using (var producer = new Producer<Null, string>(config, null, new StringSerializer(Encoding.UTF8)))
            {
                var deliveryReport = producer.ProduceAsync(topicName, null, data).Result;
            }
        }
    }
}