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

namespace HireToRetire.Controllers
{
    [Authorize]
    public class CandidateController : Controller
    {
        string domain = "candidateregistration";

        public IActionResult Index()
        {
            List<CandidateViewModel> candidates = new ApiService(new Uri($"http://{domain}"))
                .GetAsync<List<CandidateViewModel>>(new Uri($"http://{domain}/api/candidates")).Result;

            return View(candidates);
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult CreateSave(CandidateViewModel candidate)
        {
            // call api to save
            new ApiService(new Uri($"http://{domain}")).PostAsync<CandidateViewModel>(new Uri($"http://{domain}/api/candidates"), candidate);

            // try
            // {
            //     KPub(JsonConvert.SerializeObject(candidate));
            // }
            // catch (Exception)
            // {
            //     throw;
            //     //return View("Home/Error");
            // }

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
            new ApiService(new Uri($"http://{domain}")).PutAsync<CandidateViewModel>(new Uri($"http://{domain}/api/candidates/{candidate.Id}"), candidate);
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
            new ApiService(new Uri($"http://{domain}")).DeleteAsync<CandidateViewModel>(new Uri($"http://{domain}/api/candidates/{candidate.Id}"));
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
                    { "bootstrap.servers", "kafka-cp-kafka:9092" },
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
                { "bootstrap.servers", "kafka-cp-kafka:9092" }
            };

            using (var producer = new Producer<Null, string>(config, null, new StringSerializer(Encoding.UTF8)))
            {
                var deliveryReport = producer.ProduceAsync(topicName, null, data).Result;
            }
        }
    }
}