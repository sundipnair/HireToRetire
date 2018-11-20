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
            string headerData = "";
            Request.Headers.Keys.ToList().ForEach(key =>
            {
                headerData += $"<br> {key} || {Request.Headers[key].ToString()} ";
            });

            //string xreqId = Request.Headers.ContainsKey("X-Request-ID") ? Request.Headers["X-Request-ID"].ToString() : "No X-Request-ID";
            //string xrealIp = Request.Headers.ContainsKey("X-Real-IP") ? Request.Headers["X-Real-IP"].ToString() : "No X-Real-IP";
            //string xfor = Request.Headers.ContainsKey("X-Forwarded-For") ? Request.Headers["X-Forwarded-For"].ToString() : "No X-Forwarded-For";
            //string xproto = Request.Headers.ContainsKey("X-Forwarded-Proto") ? Request.Headers["X-Forwarded-Proto"].ToString() : "No X-Forwarded-Proto";
            //string xport = Request.Headers.ContainsKey("X-Forwarded-Port") ? Request.Headers["X-Forwarded-Port"].ToString() : "No X-Forwarded-Port";
            //string xhost = Request.Headers.ContainsKey("X-Forwarded-Host") ? Request.Headers["X-Forwarded-Host"].ToString() : "No Forwarded Host";
            //string xoriguri = Request.Headers.ContainsKey("X-Original-URI") ? Request.Headers["X-Original-URI"].ToString() : "No X-Original-URI";
            //string xscheme = Request.Headers.ContainsKey("X-Scheme") ? Request.Headers["X-Scheme"].ToString() : "No X-Scheme";
            //string xorigfor = Request.Headers.ContainsKey("X-Original-Forwarded-For") ? Request.Headers["X-Original-Forwarded-For"].ToString() : "No X-Original-Forwarded-For";
            //string xcustom = Request.Headers.ContainsKey("X-Custom-String") ? Request.Headers["X-Custom-String"].ToString() : "No X-Custom-String";

            ViewData["Message"] = $"Your contact page. " + headerData;
            //$"<br> X-Request-ID || {xreqId} " +
            //$"<br> X-Real-IP || {xrealIp} " +
            //$"<br> X-Forwarded-For || {xfor} " +
            //$"<br> X-Forwarded-Proto || {xproto} " +
            //$"<br> X-Forwarded-Port || {xport} " +
            //$"<br> X-Forwarded-Host || {xhost} " +
            //$"<br> X-Original-URI || {xoriguri} " +
            //$"<br> X-Scheme || {xscheme} " +
            //$"<br> X-Original-Forwarded-For || {xorigfor}" +
            //$"<br> X-Custom-String || {xcustom}";

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
