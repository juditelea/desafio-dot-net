using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppTmDb.Data;
using AppTmDb.Models;
using Microsoft.Data.SqlClient;
using RestSharp;
using Nancy.Json;

namespace AppTmDb.Controllers
{
    public class FilmesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FilmesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Filmes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Filmes.ToListAsync());
        }

        // GET: Filmes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filme = await _context.Filmes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (filme == null)
            {
                return NotFound();
            }

            return View(filme);
        }

        // GET: Filmes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Filmes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NomeFilme,LinkImg")] Filme filme)
        {
            if (ModelState.IsValid)
            {
                _context.Add(filme);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(filme);
        }

        // GET: Filmes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filme = await _context.Filmes.FindAsync(id);
            if (filme == null)
            {
                return NotFound();
            }
            return View(filme);
        }

        // POST: Filmes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NomeFilme,LinkImg")] Filme filme)
        {
            if (id != filme.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(filme);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilmeExists(filme.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(filme);
        }

        // GET: Filmes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filme = await _context.Filmes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (filme == null)
            {
                return NotFound();
            }

            return View(filme);
        }

        // POST: Filmes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var filme = await _context.Filmes.FindAsync(id);
            _context.Filmes.Remove(filme);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FilmeExists(int id)
        {
            return _context.Filmes.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Executar()
        {
            //List<Filme> filmes = null;
            Dictionary<string, object> dict = new Dictionary<string, object>();
            //List<int> ids = null;
            //List<string> nomesFilmes = null;
            //List<string> linkImagens = null;

            var client = new RestClient("https://api.themoviedb.org/3/movie/popular");

            var request = new RestRequest("?api_key=563104c7ddd6f319ec8bfd33cf19ad2f", Method.GET);

            request.AddParameter("api_key", "563104c7ddd6f319ec8bfd33cf19ad2f", RestSharp.ParameterType.UrlSegment);

            var queryResult = client.Execute(request).Content;

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            dynamic result1 = serializer.DeserializeObject(queryResult);

            List<Dictionary<string, object>> MyList = new List<Dictionary<string, object>>();
            foreach (KeyValuePair<string, object> entry in result1)
            {
                var key = entry.Key;
                var value = entry.Value;
                int id = 0;
                string link = "";
                string nome = "";
                if (entry.Key == "results")
                {
                    MyList.Add(new Dictionary<string, object>());
                    MyList[0].Add("Dictionary 1", value);

                    dict = MyList[0];
                    string f = "";
                    foreach (KeyValuePair<string, object> par in dict)
                    {
                        f = par.Value.ToString();
                    }
                    var todo = f.Split('{');
                    foreach (string parte in todo)
                    {

                        int iLink = parte.IndexOf("backdrop_path", 0) + 16;
                        if (iLink > 20)
                        {
                            link = "http://image.tmdb.org/t/p/w92" + parte.Substring(iLink, 32);
                            int jLink = parte.IndexOf("original_title", iLink + link.Length) + 17;
                            int kLink = parte.IndexOf(",", jLink) - 1;
                            nome = parte.Substring(jLink, kLink - jLink);
                            //filmes.Add(new Filme(id, nome, link));
                            Salvar(nome, link);
                        }
                    }
                }

            }


            return RedirectToAction(nameof(Index));
        }

        private static string sqlConn()
        {
            string Conn = "Server = (localdb)\\mssqllocaldb; Database =AppTmDb; Trusted_Connection = True; MultipleActiveResultSets = true";
            return Conn;
        }

        private static void Salvar(string nomefilme, string linkimg)
        {
            using (var connection = new SqlConnection(sqlConn()))
            {
                string queryString = "Insert into Filmes (NomeFilme, LinkImg) " +
                    "Values('" + nomefilme + "', '" + linkimg + "')";

                SqlCommand command = new SqlCommand(queryString, connection);
                command.Connection.Open();
                command.ExecuteNonQuery();
               
            }
        }
    }
}
