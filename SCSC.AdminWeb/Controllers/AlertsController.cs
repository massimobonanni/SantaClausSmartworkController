using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SCSC.AdminWeb.Models.Alerts;
using SCSC.APIClient;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SCSC.AdminWeb.Controllers
{
    public class AlertsController : Controller
    {
        private readonly AlertsRestClient alertsRestClient;
        private readonly ElvesRestClient elvesRestClient;

        public AlertsController(AlertsRestClient alertsRestClient, ElvesRestClient elvesRestClient)
        {
            this.alertsRestClient = alertsRestClient;
            this.elvesRestClient = elvesRestClient;
        }

        public async Task<ActionResult> Index([FromQuery(Name = "elfId")] string elfId,
           CancellationToken token)
        {
            var model = new IndexViewModel() { FilterElfId = elfId };

            var alerts = await this.alertsRestClient.GetAlertsAsync(elfId, token);

            model.Alerts = alerts
                .Select(e => new AlertInfoViewModel(e))
                .OrderByDescending(e => e.CreationTimeStamp);

            return View(model);
        }

        // GET: AlertsController/Details/5
        public async Task<ActionResult> Details(string id, CancellationToken token)
        {
            var alert = await this.alertsRestClient.GetAlertAsync(id, token);
            if (alert == null)
                return NotFound();
            var model = new AlertInfoViewModel(alert);
            return View(model);
        }


        public async Task<ActionResult> Create(string elfId, CancellationToken token)
        {
            var model = new CreateViewModel();
            var elves = await this.elvesRestClient.GetElvesAsync(null, token);
            model.ElfIds = new List<SelectListItem>();
            model.ElfIds.Add(new SelectListItem("Select elf", string.Empty));
            model.ElfIds.AddRange(elves.Select(e => new SelectListItem(e.Name, e.Id)).OrderBy(i => i.Text));
            if (elves.Any(e => e.Id == elfId))
                model.ElfId = elfId;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateViewModel alertModel, CancellationToken token)
        {
            if (ModelState.IsValid)
            {
                if (alertModel.IsValid())
                {
                    var alert = alertModel.ToCoreAlert();

                    var result = await this.alertsRestClient.CreateAlertAsync(alert, token);

                    if (result)
                        return RedirectToAction(nameof(Index));

                    ModelState.AddModelError(string.Empty, "An error occurs during insert operation");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Check alert configuration");
                }

            }

            var elves = await this.elvesRestClient.GetElvesAsync(null, token);

            alertModel.ElfIds = new List<SelectListItem>();
            alertModel.ElfIds.Add(new SelectListItem("Select elf", string.Empty));
            alertModel.ElfIds.AddRange(elves.Select(e => new SelectListItem(e.Name, e.Id)).OrderBy(i => i.Text));
            return View(alertModel);
        }


        public async Task<ActionResult> Delete(string id, CancellationToken token)
        {
            var result = await this.alertsRestClient.CancelAlertAsync(id, token);
            if (!result)
                return RedirectToAction("Details", new { id = id });
            return RedirectToAction(nameof(Index));
        }
    }
}
