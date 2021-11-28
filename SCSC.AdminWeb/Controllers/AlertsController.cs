using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SCSC.AdminWeb.Models.Alerts;
using SCSC.APIClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SCSC.AdminWeb.Controllers
{
    public class AlertsController : Controller
    {
        private readonly AlertsRestClient alertsRestClient;

        public AlertsController(AlertsRestClient alertsRestClient)
        {
            this.alertsRestClient = alertsRestClient;
        }

        public async Task<ActionResult> Index([FromQuery(Name = "elfId")] string elfId,
           CancellationToken token)
        {
            var model = new IndexViewModel() { FilterElfId = elfId };

            var alerts = await this.alertsRestClient.GetAlertsAsync(elfId,token);

            model.Alerts = alerts.Select(e => new AlertInfoViewModel(e));

            return View(model);
        }

        // GET: AlertsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AlertsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AlertsController/Create
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
