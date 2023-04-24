using gameStore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace gameStore.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class kosarController : ControllerBase
    {

        /* [HttpGet("{uId}")]

         public async Task<IActionResult> GetKosar(string uId, int id)
         {
             if (Program.LoggedInUsers.ContainsKey(uId))
             {
             using (var context = new jatekshopContext())
             {
                 try
                 {
                     return Ok(await context.Kosars.Include(f => f.VasarloId).Include(f => f.JatekId).Include(f=>f.Darab).Where(f => f.VasarloId == id).ToListAsync());
                 }
                 catch (Exception ex)
                 {
                     return BadRequest(ex.Message);
                 }
             }

             }
             else
             {
                 return BadRequest("Nincs jogosultsága");
             }
         }*/

        [HttpGet("{vasid}")]

        public IActionResult KosarGet(int vasid)
        {
            using (var context = new jatekshopContext())
            {
                try
                {
                    return Ok(context.Kosars.Where(h=>h.VasarloId == vasid).ToList());
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpGet]

        public IActionResult KosarFid()
        {
            using (var context = new jatekshopContext())
            {
                try
                {
                    return Ok(context.Kosars.ToList());
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }


        [HttpPost]
        public IActionResult jatekPostkosarelott(Osszesjatek jatek)
        {
            using (var context = new jatekshopContext())
            {
                try
                {
                    jatek.EgyediId = Program.GenerateID();
                    context.Add(jatek);
                    context.SaveChanges();
                    return Ok(jatek.EgyediId);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message + "A játék nem került hozzáadásra.");
                }
            }
        }

       [HttpGet("{fId},{egyediId}")]

        public IActionResult KosarAtrakas(int fId, int egyediId, int darab)
        {
            /*if (Program.LoggedInUsers.ContainsKey(fId))
             {*/
            using (var context = new jatekshopContext())
            {
                try
                {
                    var jatekKosar = context.Osszesjateks.Where(c => c.EgyediId == egyediId).ToList();
                    var felhasznaloKosar = context.Felhasznaloks.Where(f => f.Id == fId).ToList();
                    if (jatekKosar[0].EgyediId == egyediId)
                    {
                        Kosar kosar = new Kosar();
                        kosar.VasarloId = felhasznaloKosar[0].Id;
                        kosar.JatekId = jatekKosar[0].Id;
                        kosar.Darab = darab;

                        context.Kosars.Add(kosar);
                        context.SaveChanges();
                        return Ok("Kosárba helyezés sikeres!");
                    }
                    else
                    {
                        return BadRequest("Kosárba helyezés sikertelen!");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            /*}
             else
             {
                 return BadRequest("Nincs jogosultsága");
             }*/
        }

        [HttpDelete("{id}")]

        public IActionResult kosarTorles(int id)
        {
            using (var context = new jatekshopContext())
            {
                try
                {
                    Kosar kosar = new Kosar();
                    kosar.Id = id;
                    context.Kosars.Remove(kosar);
                    context.SaveChanges();
                    return StatusCode(204, "A játék adatainak törlése sikeresen megtörtént.");
                }
                catch (Exception ex)
                {
                    return BadRequest("Sikertelen törlés");
                }
            }
        }
    }
}