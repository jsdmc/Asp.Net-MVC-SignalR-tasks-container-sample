using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using ASP.NET_MVC___SignalR_sample.Models;

namespace ASP.NET_MVC___SignalR_sample.OperaionContainer
{
    public class TaskDummy
    {
        public static Action<Operation> DummyAction = (op) =>
            {
                for (var progress = 0; progress <= 100; progress++)
                {
                    if (op.CancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    Thread.Sleep(new Random().Next(200, 500));
                    op.ReportProgress(progress);
                }
            };
    }
}