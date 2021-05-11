using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MeetupAPI.Filters
{
    public class TimeTrackFilter : IActionFilter
    {
        private readonly ILogger<TimeTrackFilter> logger;

        public TimeTrackFilter(ILogger<TimeTrackFilter> logger)
        {
            this.logger = logger;
        }
        private Stopwatch _stopwatch;

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _stopwatch.Stop();

            var miliseconds = _stopwatch.ElapsedMilliseconds;
            var action = context.ActionDescriptor.DisplayName; // pobranie nazwy akcji z kontekstu

            logger.LogInformation($"Action [{action}], executed in: {miliseconds} miliseconds.");
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _stopwatch = new Stopwatch();
            _stopwatch.Start(); //rozpoczęcie mierzenia czasu
        }
    }
}
