﻿using gameStore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net.Mail;
using gameStore;
using System.Collections.Generic;

namespace gameStore.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RegistryController : ControllerBase
    {
        /*[HttpGet("{id}")]

        public IActionResult UserWithId(int id)
        {
            List<Registry> list = new List<Registry>();
            using (var context = new jatekshopContext())
            {
                try
                {
                    return StatusCode(200, context.Registries.Where(v=>v.Id==id).ToList());
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }*/

        [HttpPost]

        public IActionResult Post(Registry registry)
        {
            using (var context = new jatekshopContext())
            {
                try
                {
                    if (context.Felhasznaloks.Where(f => f.FelhasznaloNev == registry.FelhasznaloNev).ToList().Count != 0)
                    {
                        return BadRequest("Létezik ilyen felhasználónév!");
                    }
                    if (context.Felhasznaloks.Where(f => f.Email == registry.Email).ToList().Count != 0)
                    {
                        return BadRequest("Erről az e-mail címről már regisztráltak!");
                    }
                    registry.Key = Program.GenerateSalt();
                    context.Add(registry);
                    context.SaveChanges();
                    Program.SendEmail(registry.Email, "Regisztráció", "A regisztrációhoz kattints a következő linkre: " + "https://localhost:5001/Registry/" + registry.Key);
                    return Ok("Sikeres regisztráció. Az e-mail címére küldött utasítások alapján befejezheti azt.");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpGet("{Key}")]

        public IActionResult GetKey(string Key)
        {
            using (var context = new jatekshopContext())
            {
                try
                {
                    var registryUser = context.Registries.Where(c => c.Key == Key).ToList();
                    if (registryUser.Count != 0)
                    {
                        Felhasznalok felhasznalo = new Felhasznalok();
                        felhasznalo.FelhasznaloNev = registryUser[0].FelhasznaloNev;
                        felhasznalo.TeljesNev = registryUser[0].TeljesNev;
                        felhasznalo.Salt = registryUser[0].Salt;
                        felhasznalo.Hash = registryUser[0].Hash;
                        felhasznalo.Email = registryUser[0].Email;
                        felhasznalo.Jogosultsag = 0;
                        felhasznalo.Aktiv = 1;
                        context.Felhasznaloks.Add(felhasznalo);
                        context.Registries.Remove(registryUser[0]);
                        context.SaveChanges();
                        return Ok("Regisztráció befejezve.");
                    }
                    else
                    {
                        return BadRequest("Sikertelen regisztráció!");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }
    }
}