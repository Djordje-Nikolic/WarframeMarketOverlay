using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WarframeMarketOverlay
{
    class WarframeQueryHandlerException : Exception
    {
        public WarframeQueryHandlerException() { }
        public WarframeQueryHandlerException(string message) : base(message) { }
        public WarframeQueryHandlerException(string message, Exception inner) : base(message, inner) { }
    }

    class WarframeQueryHandler : IDisposable
    {
        private Process screenReader;
        private bool KeyPressed;
        private HttpClient client;
        private List<Task> tasks;
        private int itemCount;
        private bool doneReceiving;
        private CancellationTokenSource cancellationTokenSource;

        public WarframeQueryHandler()
        {
            RegisterReader();
            KeyPressed = false;
        }

        private void RegisterReader()
        {
            screenReader = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "OutputTest.exe",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            
            screenReader.EnableRaisingEvents = true;
            screenReader.OutputDataReceived += reader_OutputDataReceived;
            screenReader.Exited += reader_Exited;
            screenReader.ErrorDataReceived += reader_ErrorDataReceived;

            itemCount = 0;
        }

        private void reader_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                if (e.Data != "Done")
                {
                    Interlocked.Increment(ref itemCount);
                    tasks.Add(Task.Run(() => HandleResult(e.Data), cancellationTokenSource.Token));
                }
                else
                {
                    doneReceiving = true;
                    screenReader.CancelOutputRead();
                    if (itemCount == 0)
                        Dispose();
                }
            }  
        }

        private async void HandleResult(string s)
        {
            try
            {
                cancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (!s.StartsWith("forma"))
                {
                    var response = await client.GetAsync("https://api.warframe.market/v1/items/" + s + "/orders");
                    var responseObject = await response.Content.ReadAsAsync<Result>();
                    System.Windows.Forms.MessageBox.Show(s + ' ' + responseObject.GetLowestSellPrice().ToString());
                    response.Dispose();

                    cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    //itemCount--;
                    Interlocked.Decrement(ref itemCount);
                    if (itemCount == 0 && doneReceiving)
                    {                        
                        Dispose();
                    }
                    
                }
            }
            catch (TaskCanceledException e)
            {
                if (e.CancellationToken.IsCancellationRequested == false)
                {
                    System.Windows.Forms.MessageBox.Show("Source: Query Handler Timeout\r\n" + "Details: " + e.ToString(), "Error Message");
                }
            }
            catch (UnsupportedMediaTypeException e)
            {
                System.Windows.Forms.MessageBox.Show("Source: Query Handler\r\nDetails:Result for an item (" + s + ") was invalid: " + e.Message, "Error Message");        //change to log
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Source: Query Handler\r\n" + "Details: " + e.ToString(), "Error Message");     //change to log
            }
        }

        private void reader_ErrorDataReceived(object sender, EventArgs e)   //MODIFY
        {
            System.Windows.Forms.MessageBox.Show("Source: Screen Reader\r\n" + "Details: " + e.ToString(),"Error Message");
        }

        private void reader_Exited(object sender, EventArgs e)  //Is really needed?
        {
            
        }

        public async void Dispose()
        {
            KeyPressed = false;
            cancellationTokenSource.Cancel();
            cancellationTokenSource = null;
            client.CancelPendingRequests();
            if (tasks != null)
            {
                await Task.WhenAll(tasks);
                tasks = null;
            }
            if (client != null)
            {
                client.Dispose();
                client = null;
            }
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (screenReader != null) screenReader.Dispose();
            }
        }

        public void Execute()
        {
            if (!KeyPressed)
                try
                {
                    {
                        KeyPressed = true;
                        doneReceiving = false;
                        client = new HttpClient
                        {
                            BaseAddress = new Uri("https://warframe.market/")
                        };
                        tasks = new List<Task>();
                        screenReader.Start();
                        screenReader.BeginOutputReadLine();
                        cancellationTokenSource = new CancellationTokenSource();
                    }
                }
                catch (Exception e)
                {
                    screenReader.CancelOutputRead();
                    screenReader.Kill();
                    Dispose();
                    throw new WarframeQueryHandlerException("Error at startup",e);
                }
        }
    }
}
