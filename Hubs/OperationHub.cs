using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using ASP.NET_MVC___SignalR_sample.OperaionContainer;
using ASP.NET_MVC___SignalR_sample.Models;
using Microsoft.AspNet.SignalR.Hubs;

namespace ASP.NET_MVC___SignalR_sample.Hubs
{
    [HubName("operationsHub")]
    public class OperationHub : Hub
    {
        private readonly OperationContainer _operationContainer;

        public OperationHub() : this(OperationContainer.Instance) { }

        public OperationHub(OperationContainer operationContainer)
        {
            _operationContainer = operationContainer;
        }

        //public methods accessible from client code
        public IEnumerable<Operation> GetAllOperations()
        {
            return _operationContainer.GetAllOperations();
        }

        public void RunNewOperation()
        {
            _operationContainer.RunNewOperation();
        }

        public void RemoveOperation(int id)
        {
            _operationContainer.RemoveOperation(id);
        }
    }
}