using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace DotNetSiemensPLCToolBoxLibrary.Communication.Discovery
{
    /// <summary>
    /// can scan an given ipadress/port conmbinations if they are available
    /// </summary>
    /// <remarks></remarks>
    public class IPScanner
    {
        public event EventHandler<AdressFoundEventArgs> NewAdressFound;
        public event EventHandler<EventArgs> ScanComplete;
        public event EventHandler<EventArgs> RunningThreadCountChanged;
        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;

        private List<ScannerWorker> _Threads = new List<ScannerWorker>();
        private List<IPEndPoint> _DiscoveredAdresses;
        private List<IPEndPoint> _AdressesToCheck;
        private int _CheckIndex = -1;
        private TimeSpan _Timeout = new TimeSpan(0, 0, 0, 0, 500);
        private int _RunningThreads = 0;
        //Object so that the workers can synclock durning their End worker operation
        private object _ThreadCountLock = new object();

        private int _ThreadCount = 5;
        #region "Properties"
        /// <summary>
        /// specifies the timeout to wait before canceling an adress probing
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public TimeSpan CheckTimeOut
        {
            get { return _Timeout; }
            set { _Timeout = value; }
        }

        /// <summary>
        /// contains all adresses, that have been found. the value is only valid after the scan is completed
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual List<System.Net.IPEndPoint> DiscoveredAdresses
        {
            get
            {
                lock (_DiscoveredAdresses)
                {
                    return _DiscoveredAdresses;
                }
            }
        }

        /// <summary>
        /// specifies if the scanner is stll runing
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool isRunning
        {
            get { return _RunningThreads > 0; }
        }

        /// <summary>
        /// specifies the amount of thread, currently running
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int RunningThreadCount
        {
            get { return _RunningThreads; }
        }

        /// <summary>
        /// Specifies the maximum amount of threads to use for the scanning
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int ThreadCount
        {
            get { return _ThreadCount; }
            set { _ThreadCount = value; }
        }
        #endregion


        public IPScanner()
        {
        }

        /// <summary>
        /// Starts an asynchronous scanning of the given adresses
        /// </summary>
        /// <param name="AdressesToCheck"></param>
        /// <remarks></remarks>
        public virtual void BeginScan(List<System.Net.IPEndPoint> AdressesToCheck)
        {
            if (AdressesToCheck == null)
            {
                throw new ArgumentException("AdressesToCheck can not be NULL");
            }

            if (AdressesToCheck.Count == 0)
            {
                throw new ArgumentException("AdressesToCheck can not be empty");
            }

            if (isRunning)
            {
                throw new InvalidOperationException("Can only start when not running");
            }
            else
            {
                //Create Worker Threads
                int ThreadToCreate = Math.Min(AdressesToCheck.Count, _ThreadCount);
                for (int i = 1; i <= ThreadToCreate; i++)
                {
                    _Threads.Add(new ScannerWorker(this));
                }

                //Start Operation
                _CheckIndex = -1;
                _AdressesToCheck = AdressesToCheck;
                _DiscoveredAdresses = new List<IPEndPoint>();

                foreach (ScannerWorker Thread in _Threads)
                {
                    Thread.BeginWork();
                    lock (_ThreadCountLock)
                    {
                        _RunningThreads += 1;
                        if (RunningThreadCountChanged != null)
                        {
                            RunningThreadCountChanged(this, EventArgs.Empty);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// blocks until the scan has completed
        /// </summary>
        /// <remarks></remarks>
        public virtual void EndScan()
        {
            foreach (ScannerWorker Thread in _Threads)
            {
                Thread.EndWork();
            }
        }

        /// <summary>
        /// tries to abort the adress scanning
        /// </summary>
        /// <remarks></remarks>
        public virtual void AbortScan()
        {
            foreach (ScannerWorker Thread in _Threads)
            {
                Thread.Abort();
            }
        }

        /// <summary>
        /// Starts an synchronous scan for the Adress range
        /// </summary>
        /// <param name="AdressesToCheck"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual List<System.Net.IPEndPoint> Scan(List<System.Net.IPEndPoint> AdressesToCheck)
        {
            BeginScan(AdressesToCheck);
            EndScan();
            return _DiscoveredAdresses;
        }

        #region "Callbacks form Threads"
        private System.Net.IPEndPoint GetNextAdress()
        {
            lock (_AdressesToCheck)
            {
                if (_CheckIndex + 1 >= _AdressesToCheck.Count)
                    return null;
                //End here

                _CheckIndex += 1;

                if (ProgressChanged != null)
                {
                    ProgressChanged(this, new ProgressChangedEventArgs
                    {
                        Progress = _CheckIndex,
                        ProgressMax = _AdressesToCheck.Count
                    });
                }

                return _AdressesToCheck[_CheckIndex];
            }
        }

        private void AdressFound(IPEndPoint Endpoint)
        {
            lock (_DiscoveredAdresses)
            {
                _DiscoveredAdresses.Add(Endpoint);
            }
            OnNewAdressFound(new AdressFoundEventArgs(Endpoint));
        }

        private void WorkerEnded()
        {
            lock (_ThreadCountLock)
            {
                _RunningThreads -= 1;
                OnRunningThreadCountChanged();
                if (_RunningThreads == 0)
                    OnScanCompleted();
            }
        }
        #endregion

        #region "Event Handlers"
        protected virtual void OnScanCompleted()
        {
            if (ScanComplete != null)
            {
                ScanComplete(this, EventArgs.Empty);
            }
        }

        protected virtual void OnRunningThreadCountChanged()
        {
            if (RunningThreadCountChanged != null)
            {
                RunningThreadCountChanged(this, EventArgs.Empty);
            }
        }

        protected virtual void OnNewAdressFound(AdressFoundEventArgs e)
        {
            if (NewAdressFound != null)
            {
                NewAdressFound(this, e);
            }
        }
        #endregion

        /// <summary>
        /// Thread wrapper to wrap the thread and its local variables
        /// </summary>
        /// <remarks></remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
        private class ScannerWorker
        {
            private System.Threading.Thread _Thread;
            private TcpClient _TCPClient;
            private IPScanner _IPPortScanner;
            private bool _IsRunning = false;
            private bool _isAborting = false;

            private EventWaitHandle _WaitHandle = new EventWaitHandle(true, EventResetMode.ManualReset);
            public bool isRunning
            {
                get { return _IsRunning; }
            }

            public ScannerWorker(IPScanner IpPortScanner)
            {
                _IPPortScanner = IpPortScanner;
            }

            public void BeginWork()
            {
                if (!_IsRunning)
                {
                    _WaitHandle.Reset();
                    _Thread = new Thread(DoWork);
                    _Thread.Start();
                    _isAborting = false;
                    _IsRunning = true;
                }
            }

            public void EndWork()
            {
                _WaitHandle.WaitOne();
            }

            public void Abort()
            {
                _isAborting = true;
            }

            private void DoWork()
            {
                try
                {
                    System.Net.IPEndPoint Adress = _IPPortScanner.GetNextAdress();
                    while ((Adress != null))
                    {
                        try
                        {
                            _TCPClient = new TcpClient();
                            _TCPClient.ReceiveTimeout = (int)_IPPortScanner.CheckTimeOut.TotalMilliseconds;
                            _TCPClient.SendTimeout = (int)_IPPortScanner.CheckTimeOut.TotalMilliseconds;


                            //using Async connection, because the standard .net socket ignores the timeout for Socket connects.
                            IAsyncResult result = _TCPClient.BeginConnect(Adress.Address.ToString(), Adress.Port, null, null);
                            bool success = result.AsyncWaitHandle.WaitOne((int)_IPPortScanner.CheckTimeOut.TotalMilliseconds);

                            if (_TCPClient.Connected)
                            {
                                _TCPClient.Close();
                                _IPPortScanner.AdressFound(Adress);
                            }

                            if (_isAborting)
                            {
                                break; // TODO: might not be correct. Was : Exit While
                            }

                        }
                        catch
                        {
                        }
                        Adress = _IPPortScanner.GetNextAdress();
                    }

                }
                catch
                {
                }
                finally
                {
                    _isAborting = false;
                    _IsRunning = false;
                    _IPPortScanner.WorkerEnded();
                    _WaitHandle.Set();
                }
            }

        }

        public class AdressFoundEventArgs : EventArgs
        {

            private IPEndPoint _Adress;
            public IPEndPoint Adress
            {
                get { return _Adress; }
            }

            public AdressFoundEventArgs(IPEndPoint Adress)
            {
                _Adress = Adress;
            }
        }

        public class ProgressChangedEventArgs : EventArgs
        {
            public int Progress { get; set; }
            public int ProgressMax { get; set; }
        }
    }
}