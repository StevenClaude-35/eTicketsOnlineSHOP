﻿using eTicketAPP.Controllers.Data;
using eTicketAPP.Data.Services;
using eTicketAPP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eTicketAPP.Controllers
{
    public class MoviesController : Controller
    {
        private readonly IMoviesService _service;

        public MoviesController(IMoviesService service)
        {
            _service=service;
        }
        public async Task<IActionResult> Index()
        {
            var allMovies = await _service.GetAll(n=>n.Cinema);
            return View(allMovies);
        }

        public async Task<IActionResult> Filter(string searchString)
        {
            var allMovies = await _service.GetAll(n => n.Cinema);

            if (!string.IsNullOrEmpty(searchString))
            {
                var filteredResult = allMovies.Where(n => n.Name.Contains(searchString) || n.Description.Contains(searchString)).ToList();

                return View("Index", filteredResult);
            }
            return View("Index",allMovies);
        }

        //GET:Movies/Details/1

        public async Task<IActionResult> Details(int id)
        {
            var movieDetails =await _service.GetMovieByIdAsync(id);

            return View(movieDetails);
        }

        //GET:Movies/Create

        public async Task<IActionResult> Create()
        {
            var movieDropDownData = await _service.GetNewMovieDropDownValues();
            ViewBag.Cinemas = new SelectList(movieDropDownData.Cinemas, "Id", "Name");
            ViewBag.Producers = new SelectList(movieDropDownData.Producers, "Id", "FullName");
            ViewBag.Actors = new SelectList(movieDropDownData.Actors, "Id", "FullName");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(NewMovieVM movie)
        {
            if (!ModelState.IsValid)
            {
                var movieDropDownData = await _service.GetNewMovieDropDownValues();

                ViewBag.Cinemas = new SelectList(movieDropDownData.Cinemas, "Id", "Name");
                ViewBag.Producers = new SelectList(movieDropDownData.Producers, "Id", "FullName");
                ViewBag.Actors = new SelectList(movieDropDownData.Actors, "Id", "FullName");

                return View(movie);
            }
            await _service.AddNewMovie(movie);
            return RedirectToAction(nameof(Index));
        }

        //GET:Movies/Edit/1

        public async Task<IActionResult> Edit(int id)
        {
            var movieDetails = await _service.GetMovieByIdAsync(id);
            if (movieDetails == null) return View("NotFound");

            var response = new NewMovieVM()
            {
                Id = movieDetails.Id,
                Name = movieDetails.Name,
                Description = movieDetails.Description,
                Price = movieDetails.Price,
                StartDate=movieDetails.StartDate,
                EndDate=movieDetails.EndDate,
                ImageUrl = movieDetails.ImageUrl,
                MovieCategory = movieDetails.MovieCategory,
                CinemaId = movieDetails.CinemaId,
                ProducerId = movieDetails.ProducerId,
                ActorIds = movieDetails.Actors_Movies.Select(n => n.ActorId).ToList()
            };
            var movieDropDownData = await _service.GetNewMovieDropDownValues();
            ViewBag.Cinemas = new SelectList(movieDropDownData.Cinemas, "Id", "Name");
            ViewBag.Producers = new SelectList(movieDropDownData.Producers, "Id", "FullName");
            ViewBag.Actors = new SelectList(movieDropDownData.Actors, "Id", "FullName");

            return View(response);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(int id, NewMovieVM movie)
        {
            if (id != movie.Id) return View("Not Found");
            if (!ModelState.IsValid)
            {
                var movieDropDownData = await _service.GetNewMovieDropDownValues();

                ViewBag.Cinemas = new SelectList(movieDropDownData.Cinemas, "Id", "Name");
                ViewBag.Producers = new SelectList(movieDropDownData.Producers, "Id", "FullName");
                ViewBag.Actors = new SelectList(movieDropDownData.Actors, "Id", "FullName");

                return View(movie);
            }
            await _service.UpdateNewMovie(movie);
            return RedirectToAction(nameof(Index));
        }
    }
}
