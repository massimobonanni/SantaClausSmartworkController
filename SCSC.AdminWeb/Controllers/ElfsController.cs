using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SCSC.AdminWeb.Models.Elfs;
using SCSC.APIClient;
using SCSC.Core.Models;
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

        public async Task<ActionResult> Index([FromQuery(Name = "filterName")] string filterName,
           CancellationToken token)
        {
            var model = new IndexViewModel() { FilterName = filterName };

            var elfs = await this.elfsRestClient.GetElfsAsync(filterName, token);

            model.Elfs = elfs.Select(e => new ElfInfoViewModel(e)).OrderBy(e => e.Name);

            return View(model);
        }

        public async Task<ActionResult> Details(string id,
         CancellationToken token)
        {
            var elf = await this.elfsRestClient.GetElfAsync(id, token);
            if (elf == null)
            {
                return this.NotFound();
            }
            var model = new ElfInfoViewModel(elf);
            return View(model);
        }

    }
}
