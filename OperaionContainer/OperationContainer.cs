using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using ASP.NET_MVC___SignalR_sample.Hubs;
using ASP.NET_MVC___SignalR_sample.Models;

namespace ASP.NET_MVC___SignalR_sample.OperaionContainer
{
    public class OperationContainer
    {
        #region Singleton

        private readonly static Lazy<OperationContainer> _instance = new Lazy<OperationContainer>(() => 
            new OperationContainer(GlobalHost.ConnectionManager.GetHubContext<OperationHub>().Clients));

        public static OperationContainer Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        #endregion

        /// <summary>
        /// Hub clients reference - for broadcasting and calling client methods
        /// </summary>
        private IHubConnectionContext<dynamic> Clients { get; set; }

        private readonly object _syncObj = new object();

        /// <summary>
        /// Operations storage
        /// </summary>
        private List<Operation> _operations;

        private readonly Timer _timer;

        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(1000);

        /// <summary>
        /// Id counter. Increments when new operation added
        /// </summary>
        private int idCounter;

        private OperationContainer(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;

            _operations = new List<Operation>() {
                new Operation { Id = ++idCounter, Name = "Saving drafts"},
                new Operation { Id = ++idCounter, Name = "Pushing online"}
            };

            _timer = new Timer(UpdateOperations, null, _updateInterval, _updateInterval);
        }

        #region Client methods

        /// <summary>
        /// Client method - get all operations
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Operation> GetAllOperations()
        {
            return _operations;
        }

        /// <summary>
        /// Client method - add new operation
        /// </summary>
        public void RunNewOperation()
        {
            var newOperation = new Operation { Id = ++idCounter, Name = string.Format("New operation") };

            lock (_syncObj)
            {
                _operations.Add(newOperation);
            }

            //call client method to run operation
            Clients.All.addNewOperation(newOperation);
        }

        /// <summary>
        /// Client method - remove operation
        /// </summary>
        /// <param name="id"></param>
        public void RemoveOperation(int id)
        {
            var operationToRemove = _operations.FirstOrDefault(x => x.Id == id);

            RemoveOperationFromList(operationToRemove);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Remove operation from list
        /// </summary>
        /// <param name="operation"></param>
        private void RemoveOperationFromList(Operation operation)
        {
            lock (_syncObj)
            {
                _operations.Remove(operation);
            }
            //call client method to remove operation
            Clients.All.removeOperation(operation.Id);
        }

        /// <summary>
        /// Randomly Add or Remove operation
        /// </summary>
        /// <param name="state"></param>
        private void UpdateOperations(object state)
        {
            // Randomly choose whether to update this stock or not
            var random = new Random();

            lock (_syncObj)
            {
                //add or remove operation
                if (random.Next(100) < 50)
                {
                   RunNewOperation();

                } else if (_operations.Count > 0)
                {
                    //remove random element from list
                    var indexToRemove = random.Next(_operations.Count);
                    var operationToRemove = _operations.ElementAt(indexToRemove);

                    RemoveOperationFromList(operationToRemove);
                }
            }
        }

        #endregion
    }
}