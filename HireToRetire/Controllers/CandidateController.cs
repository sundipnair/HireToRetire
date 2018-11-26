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
    //[Authorize]
    public class CandidateController : Controller
    {
        //string domain = "azdemoapimgnt.azure-api.net/candidatereg";
        string domain = "candidateregistration";
        //string scheme = "https";
        string scheme = "http";
        string clientId = "0d36c971-15e4-4453-9e1f-2a44deb5b31e";
        string authority = "https://login.microsoftonline.com/tfp/capapps.onmicrosoft.com/B2C_1_SignUpIn/v2.0/.well-known/openid-configuration";
        //string redirectUri = "http://localhost:32768/signin-oidc";
        string redirectUri = "https://azdemoaks-dns.westeurope.cloudapp.azure.com/signin-oidc";
        string clientSecret = "M3.653[FaHr)E70Gx1D>w1E-";
        string[] scope = new string[] { "https://CapApps.onmicrosoft.com/cr-api/read" };

        public async Task<string> TestString()
        {
            try
            {
                AuthenticationResult result = await GetAuthResultAsync();

                string apiEndpoint = $"{scheme}://{domain}/api/Candidates/Test";
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

        public async Task<IActionResult> Index()
        {
            //AuthenticationResult result = await GetAuthResultAsync();

            //List<CandidateViewModel> candidates = await new ApiService(new Uri($"{scheme}://{domain}")).GetAsync<List<CandidateViewModel>>(new Uri($"{scheme}://{domain}/api/candidates"), result);
            List<CandidateViewModel> candidates = await new ApiService(new Uri($"{scheme}://{domain}")).GetAsync<List<CandidateViewModel>>(new Uri($"{scheme}://{domain}/api/candidates"), null);

            return View(candidates);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        public async Task<IActionResult> CreateSave(CandidateViewModel candidate)
        {
            // call api to save
            //new ApiService(new Uri($"{scheme}://{domain}")).PostAsync<CandidateViewModel>(new Uri($"{scheme}://{domain}/api/candidates"), candidate, result);
            new ApiService(new Uri($"{scheme}://{domain}")).PostAsync<CandidateViewModel>(new Uri($"{scheme}://{domain}/api/candidates"), candidate, null);

            //try
            //{
            //    KPub(JsonConvert.SerializeObject(candidate));
            //}
            //catch (Exception ex) 
            //{
            //    return View("Home/Error");
            //}

            ViewData["Message"] = "Candidate successfully registered";
            return View("Create");
        }

        public async Task<IActionResult> Edit(CandidateViewModel candidate)
        {
            return View(candidate);
        }

        public async Task<IActionResult> EditSave(CandidateViewModel candidate)
        {
            //AuthenticationResult result = await GetAuthResultAsync();

            // call api to update
            //new ApiService(new Uri($"{scheme}://{domain}")).PutAsync<CandidateViewModel>(new Uri($"{scheme}://{domain}/api/candidates/{candidate.Id}"), candidate, result);
            new ApiService(new Uri($"{scheme}://{domain}")).PutAsync<CandidateViewModel>(new Uri($"{scheme}://{domain}/api/candidates/{candidate.Id}"), candidate, null);
            ViewData["Message"] = "Candidate successfully updated";
            return View("Edit");
        }

        public async Task<IActionResult> Details(CandidateViewModel candidate)
        {
            return View(candidate);
        }

        public async Task<IActionResult> Delete(CandidateViewModel candidate)
        {
            return View(candidate);
        }

        public async Task<IActionResult> DeleteSave(CandidateViewModel candidate)
        {
            //AuthenticationResult result = await GetAuthResultAsync();

            // call api to delete
            //new ApiService(new Uri($"{scheme}://{domain}")).DeleteAsync<CandidateViewModel>(new Uri($"{scheme}://{domain}/api/candidates/{candidate.Id}"), result);
            new ApiService(new Uri($"{scheme}://{domain}")).DeleteAsync<CandidateViewModel>(new Uri($"{scheme}://{domain}/api/candidates/{candidate.Id}"), null);
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
            //string server = "confkafka-cp-kafka:9092";
            string server = "localhost:9092";

            var config = new Dictionary<string, object>
            {
                { "bootstrap.servers", server },
                //{ "acks", "all" },
                //{ "key.serializer", "org.apache.kafka.common.serialization.StringSerializer" },
                //{ "value.serializer", "org.apache.kafka.common.serialization.StringSerializer" },
            };

            using (var producer = new Producer<Null, string>(config, null, new StringSerializer(Encoding.UTF8)))
            {
                producer.OnError += (obj, error) =>
                {
                    //
                };

                var deliveryReport = producer.ProduceAsync(topicName, null, data).Result;

                //producer.Flush(500);
            }
        }

        private async Task<AuthenticationResult> GetAuthResultAsync()
        {
            // Retrieve the token with the specified scopes
            string signedInUserID = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            TokenCache userTokenCache = new MSALSessionCache(signedInUserID, this.HttpContext).GetMsalCacheInstance();
            ConfidentialClientApplication cca = new ConfidentialClientApplication(clientId, authority, redirectUri, new ClientCredential(clientSecret), userTokenCache, null);

            //var user = cca.Users.FirstOrDefault();
            var user = cca.GetAccountsAsync().Result.FirstOrDefault();
            if (user == null)
            {
                throw new Exception("The User is NULL.  Please clear your cookies and try again.");
            }

            return await cca.AcquireTokenSilentAsync(scope, user, authority, false);
        }
    }
}