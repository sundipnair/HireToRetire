using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HireToRetire.Models;
using HireToRetire.Service;
using Microsoft.AspNetCore.Mvc;

namespace HireToRetire.Controllers
{
    public class CandidateController : Controller
    {
        public IActionResult Index()
        {
            List<CandidateViewModel> candidates = new ApiService(new Uri("http://40.118.66.69"))
                .GetAsync<List<CandidateViewModel>>(new Uri("http://40.118.66.69/api/candidates")).Result;

            return View(candidates);
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult CreateSave(CandidateViewModel candidate)
        {
            // call api to save
            new ApiService(new Uri("http://40.118.66.69")).PostAsync<CandidateViewModel>(new Uri("http://40.118.66.69/api/candidates"), candidate);
            ViewData["Message"] = "Candidate successfully registered";
            return View("Create");
        }

        public IActionResult Edit(CandidateViewModel candidate)
        {
            return View(candidate);
        }

        public IActionResult EditSave(CandidateViewModel candidate)
        {
            // call api to save
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
            ViewData["Message"] = "Candidate successfully deleted";
            return View("Delete");
        }
    }
}