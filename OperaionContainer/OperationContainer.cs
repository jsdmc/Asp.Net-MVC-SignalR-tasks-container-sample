using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading.Tasks;

using System.Collections.Concurrent;
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
        ConcurrentDictionary<int, Operation> _operations = new ConcurrentDictionary<int, Operation>();

        private readonly Timer _timer;

        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(1000);

        /// <summary>
        /// Id counter. Increments when new operation added
        /// </summary>
        private int idCounter;

        private OperationContainer(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;

            //RunNewOperation("Predefined operation 1", TaskDummy.DummyAction);
            //RunNewOperation("Predefined operation 2", TaskDummy.DummyAction);

            //_timer = new Timer(CreateOperation, null, _updateInterval, _updateInterval);
        }

        #region Client methods

        /// <summary>
        /// Client method - get all operations
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Operation> GetAllOperations()
        {
            return _operations.Values;
        }

        /// <summary>
        /// Client method - add new operation
        /// </summary>
        public async Task<Operation> RunNewOperation(string operationName, Action<Operation> action = null)
        {
            var operation = new Operation (++idCounter){ Name = operationName ?? string.Format("New operation") };
            
            _operations.TryAdd(operation.Id, operation);

            
            var task = Task.Run(() =>
            {
                if (action != null) action(operation);
                operation.ReportComplete();

                _operations.TryRemove(operation.Id, out operation);
            });

            //Notify clients
            BroadcastOperationStatus(operation);

            await task;

            return operation;
        }

        private void BroadcastOperationStatus(Operation job)
        {
            job.ProgressChanged += HandleJobProgressChanged;
            job.Completed += HandleJobCompleted;
        }

        private void HandleJobCompleted(object sender, EventArgs e)
        {
            var operation = (Operation)sender;

            //Notify clients
            Clients.All.operationCompleted(operation.Id);

            operation.ProgressChanged -= HandleJobProgressChanged;
            operation.Completed -= HandleJobCompleted;
        }

        private void HandleJobProgressChanged(object sender, EventArgs e)
        {
            var operation = (Operation)sender;

            //Notify clients
            Clients.All.progressChanged(operation);
        }

        /// <summary>
        /// Client method - remove operation
        /// </summary>
        /// <param name="id"></param>
        public void CancelOperation(int id)
        {
            var operationToRemove = GetOperation(id);

            if (operationToRemove != null)
            {
                operationToRemove.Cancel();
            }
        }

        public Operation GetOperation(int id)
        {
            Operation result;
            return _operations.TryGetValue(id, out result) ? result : null;
        }

        #endregion


        public void CreateOperation(object state)
        {
            var operation = RunNewOperation(null, TaskDummy.DummyAction);
        }

    }
}