using ASP.NET_MVC___SignalR_sample.OperaionContainer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASP.NET_MVC___SignalR_sample.Controllers
{
    public class TaskController : Controller
    {
        private OperationContainer _operationContainer = OperationContainer.Instance;

        // GET: Task
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult RunTask(string taskName)
        {
            var operation = _operationContainer.RunNewOperation(taskName, TaskDummy.DummyAction);

            return Json(new
            {
                taskId = operation.Id
            });
        }
    }
}