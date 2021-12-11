using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SCSC.AdminWeb.Models.Elves;
using SCSC.APIClient;
using SCSC.Core.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SCSC.AdminWeb.Controllers
{
    public class ElvesController : Controller
    {
        private readonly ElvesRestClient elvesRestClient;

        public ElvesController(ElvesRestClient elvesRestClient)
        {
            this.elvesRestClient = elvesRestClient;
        }

        public async Task<ActionResult> Index([FromQuery(Name = "filterName")] string filterName,
           CancellationToken token)
        {
            var model = new IndexViewModel() { FilterName = filterName };

            var elves = await this.elvesRestClient.GetElvesAsync(filterName, token);

            model.Elves = elves.Select(e => new ElfInfoViewModel(e)).OrderBy(e => e.Name);

            return View(model);
        }

        public async Task<ActionResult> Details(string id,
         CancellationToken token)
        {
            var elf = await this.elvesRestClient.GetElfAsync(id, token);
            if (elf == null)
            {
                return this.NotFound();
            }
            var model = new ElfInfoViewModel(elf);
            return View(model);
        }

    }
}
