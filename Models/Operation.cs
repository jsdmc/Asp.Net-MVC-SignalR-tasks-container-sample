using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ASP.NET_MVC___SignalR_sample.Models
{
    public class Operation
    {
        public int Id { get; set; }

        public string Name { get; set; }


        private volatile int _progress;
        public int Progress
        {
            get { return _progress; }
        }

        private volatile bool _completed;
        public bool IsComplete
        {
            get { return _completed; }
        }

        private CancellationTokenSource _cancellationToken { get; set; }

        public event EventHandler<EventArgs> ProgressChanged;
        public event EventHandler<EventArgs> Completed;

        [JsonIgnore]
        public CancellationToken CancellationToken
        {
            get { return _cancellationToken.Token; }
        }

        [JsonIgnore]
        public Action OperaionAction { get; set; }

        public Operation()
        {

        }

        public Operation(int id)
        {
            Id = id;
            _cancellationToken = new CancellationTokenSource();
        }

        public void ReportProgress(int progress)
        {
            if (_progress != progress)
            {
                _progress = progress;
                OnProgressChanged();
            }
        }

        public void ReportComplete()
        {
            if (!IsComplete)
            {
                _completed = true;
                OnCompleted();
            }
        }

        protected virtual void OnCompleted()
        {
            if (Completed != null) Completed(this, EventArgs.Empty);
        }

        protected virtual void OnProgressChanged()
        {
            if (ProgressChanged != null) ProgressChanged(this, EventArgs.Empty);
        }

        public void Cancel()
        {
            _cancellationToken.Cancel();
        }
    }
}