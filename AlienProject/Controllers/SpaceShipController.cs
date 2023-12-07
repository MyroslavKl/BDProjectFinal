﻿using AlienProject.Additional;
using AlienProject.GenerateTables;
using AlienProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlienProject.Controllers
{
    public class SpaceShipController : Controller
    {
        private readonly AlienDbContext _context;

        public SpaceShipController(AlienDbContext context) {
            this._context = context;
        }
        public IActionResult SpaceShips()
        {
            var spaceShips = _context.Spaceshipps.ToList();
            return View(spaceShips);
        }
        [HttpGet]
        public IActionResult FindPeople() {
            return View();
        }
        [HttpPost]
        public IActionResult FindPeople(string humanName, DateTime fromDate, DateTime toDate) {
            Generate generate = new(_context);
            var data = generate.GenerateSpaceShipTable();
            var spaceshipsVisited = data
            .Where(visit => visit.HumanName == humanName && visit.SpaceshipVisitDate >= fromDate && visit.SpaceshipVisitDate <= toDate)
            .Select(visit => visit.SpaceshipName)
            .ToList();
            return View(spaceshipsVisited);
        }

        [HttpGet]
        public IActionResult ExperimentsPerShip()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ExperimentsPerShip(string alienName, DateTime fromDate, DateTime toDate)
        {
            List<ShipInfo> shipsInfo = GetShipsInfo(alienName, fromDate, toDate);
            return View(shipsInfo);
        }

        private List<ShipInfo> GetShipsInfo(string alienName, DateTime fromDate, DateTime toDate)
        {
            var shipsInfo = _context.SpaceshipExperiments
                .Where(se => se.Experiment.Alien.Name == alienName && se.Experiment.ExperimentDate >= fromDate && se.Experiment.ExperimentDate <= toDate)
                .GroupBy(se => se.SpaceshipId)
                .Select(g => new ShipInfo
                {
                    ShipId = g.Key,
                    ShipName = g.Max(se => se.Spaceship.SpaceshipName),
                    ExperimentCount = g.Count()
                })
                .OrderByDescending(info => info.ExperimentCount)
                .ToList();

            return shipsInfo;
        }

        [HttpGet]
        public IActionResult Add() {
            return View();
        }

        [HttpPost]
        public IActionResult Add(Spaceshipp spaceship)
        {
            if (spaceship == null)
            {
                return View();
            }
            _context.Spaceshipps.Add(spaceship);
            _context.SaveChanges(); 
            return RedirectToAction("SpaceShips");
        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            var ship = _context.Spaceshipps.Where(s => s.SpaceshipId == Id).FirstOrDefault();

            return View(ship);

        }
        [HttpPost]
        public IActionResult Edit(Spaceshipp spaceshipp)
        {
            if (spaceshipp == null)
            {
                return NotFound();
            }
            _context.Spaceshipps.Update(spaceshipp);
            _context.SaveChanges();
            return RedirectToAction("SpaceShips");
        }

    }
}
