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
        private readonly ElfsRestClient elfsRestClient;

        public AlertsController(AlertsRestClient alertsRestClient, ElfsRestClient elfsRestClient)
        {
            this.alertsRestClient = alertsRestClient;
            this.elfsRestClient = elfsRestClient;
        }

        public async Task<ActionResult> Index([FromQuery(Name = "elfId")] string elfId,
           CancellationToken token)
        {
            var model = new IndexViewModel() { FilterElfId = elfId };

            var alerts = await this.alertsRestClient.GetAlertsAsync(elfId, token);

            model.Alerts = alerts.Select(e => new AlertInfoViewModel(e));

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
            var elfs = await this.elfsRestClient.GetElfsAsync(null, token);
            model.ElfIds = new List<SelectListItem>();
            model.ElfIds.Add(new SelectListItem("Select elf", null));
            model.ElfIds.AddRange(elfs.Select(e => new SelectListItem(e.Name, e.Id)).OrderBy(i=>i.Text));
            if (elfs.Any(e => e.Id == elfId))
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
            var elfs = await this.elfsRestClient.GetElfsAsync(null, token);
            alertModel.ElfIds = new List<SelectListItem>();
            alertModel.ElfIds.Add(new SelectListItem("Select elf", null));
            alertModel.ElfIds.AddRange(elfs.Select(e => new SelectListItem(e.Name, e.Id)).OrderBy(i => i.Text));
            return View(alertModel);
        }

        // GET: AlertsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AlertsController/Edit/5
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

        // GET: AlertsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AlertsController/Delete/5
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
