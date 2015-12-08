using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;

using log4net;

namespace DataServices.SimpleAzureIdentityDataService.Common
{
    public class Log4NetExceptionLogger : ExceptionLogger
    {
        private ILog log = LogManager.GetLogger(typeof(Log4NetExceptionLogger));

        public async override Task LogAsync(ExceptionLoggerContext context, System.Threading.CancellationToken cancellationToken)
        {
            log.Error("An unhandled exception occurred.", context.Exception);
            await base.LogAsync(context, cancellationToken);
        }

        public override void Log(ExceptionLoggerContext context)
        {
            log.Error("An unhandled exception occurred.", context.Exception);
            base.Log(context);
        }

        public override bool ShouldLog(ExceptionLoggerContext context)
        {
            return base.ShouldLog(context);
        }
    }
}