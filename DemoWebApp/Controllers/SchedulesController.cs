using CompositionOverInheritance.Composition;
using FactoryPattern.Factories;
using FactoryPattern.Factories.FullyOpenClosed;
using Microsoft.AspNetCore.Mvc;

namespace DemoWebApp.Controllers {
    [Route("api/[controller]")]
    public class SchedulesController : ControllerBase {
        private readonly ScheduleFactoryProvider provider;

        public SchedulesController(ScheduleFactoryProvider provider) {
            this.provider = provider;
        }


        [HttpGet]
        public IActionResult AvailableTypes() {
            return Ok(provider.RegisteredFactories);
        }

        [HttpPost]
        public IActionResult CreateDaily() {
            Schedule schedule = provider.Create("daily", ScheduleParameters.Empty);
            
            return Ok(schedule);
        }
    }
}