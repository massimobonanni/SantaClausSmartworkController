using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SCSC.AdminWeb.Models.Elfs;
using SCSC.APIClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SCSC.AdminWeb.Controllers
{
    public class ElfsController : Controller
    {
        private readonly ElfsRestClient elfsRestClient;

        public ElfsController(ElfsRestClient elfsRestClient)
        {
            this.elfsRestClient = elfsRestClient;
        }

        // GET: ElfsController
        public async Task<ActionResult> Index([FromQuery(Name = "filterName")] string filterName, CancellationToken token)
        {
            var model = new IndexViewModel() { FilterName = filterName };

            var elfs = await this.elfsRestClient.GetElfsAsync(filterName, token);

            model.Elfs = elfs.Select(e => new ElfInfoViewModel(e)).OrderBy(e => e.Name);

            return View(model);
        }

        // GET: ElfsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ElfsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ElfsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ElfsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ElfsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ElfsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ElfsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
