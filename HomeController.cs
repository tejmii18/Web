using WebAppAtemMini.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace WebAppAtemMini.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAtemSwitcher _switcher;

        public HomeController(IAtemSwitcher switcher)
        {
            _switcher = switcher;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Control()
        {
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult Index(AtemSwitcherDataClass data)
        {
            if (data.ip == null || data.ip.Length == 0)
            {
                return View();
            }
            else
            {
                try
                {
                    _switcher.Connect(data);
                    return View("Control");
                }
                catch (Exception ex)
                {
                    return View();
                }
            }

        }

        [HttpPost]
        public IActionResult Control(AtemSwitcherDataClass data)
        {
            switch (data.button)
            {
                case "kamera":
                    _switcher.ChangeInput(data);
                    break;
                case "preview":
                    _switcher.ChangePreview(data);
                    break;
                case "auto":
                    _switcher.PerformAutoTransition();
                    break;
                case "cut":
                    _switcher.PerformCut();
                    break;
                case "ftb":
                    _switcher.PerformFTB();
                    break;
                case "transitionTime":
                    _switcher.ChangeTransitionTime(data);
                    break;
                case "macro":
                    _switcher.PerformMakro(data);
                    break;
                case "transition":
                    _switcher.ChangeTransition(data);
                    break;
                case "gain":
                    _switcher.SetAudioGain(data);
                    break;
            }
            return View();
        }

    }
}