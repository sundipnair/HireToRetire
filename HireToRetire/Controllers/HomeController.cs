using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HireToRetire.Models;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;

namespace HireToRetire.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            string forwardedFor = Request.Headers.ContainsKey("X-Forwarded-For") ? Request.Headers["X-Forwarded-For"].ToString() : "No Forwarded For";
            string forwardedProto = Request.Headers.ContainsKey("X-Forwarded-Proto") ? Request.Headers["X-Forwarded-Proto"].ToString() : "No Forwarded Proto";
            string forwardedHost = Request.Headers.ContainsKey("X-Forwarded-Host") ? Request.Headers["X-Forwarded-Host"].ToString() : "No Forwarded Host";
            string xcustom = Request.Headers.ContainsKey("X-Custom-String") ? Request.Headers["X-Custom-String"].ToString() : "No Forwarded Custom String";

            ViewData["Message"] = $"Your contact page. || For || {forwardedFor} || Proto || {forwardedProto} || Host || {forwardedHost} || Custom || {xcustom}";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public string TestEgress()
        {
            HttpClient client = new HttpClient();
            string res = "TEstEgress default result";

            HttpResponseMessage response = client.GetAsync("https://reqres.in/api/users/2").Result;
            if (response.IsSuccessStatusCode)
            {
                res = response.Content.ReadAsStringAsync().Result;
            }

            return res;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
