using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WarframeMarketOverlay
{
    class QueryHandlerException : Exception
    {
        public QueryHandlerException() { }
        public QueryHandlerException(string message) : base(message) { }
        public QueryHandlerException(string message, Exception inner) : base(message, inner) { }
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
            cancellationTokenSource = new CancellationTokenSource();
            screenReader.EnableRaisingEvents = true;
            screenReader.OutputDataReceived += reader_OutputDataReceived;
            screenReader.Exited += reader_Exited;
            screenReader.ErrorDataReceived += reader_ErrorDataReceived;
            itemCount = 0;
        }

        private /*async*/ void reader_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                if (e.Data != "Done")
                {
                    itemCount++;
                    tasks.Add(Task.Run(() => HandleResult(e.Data), cancellationTokenSource.Token));
                    //await temp;
                    //tasks.Remove(temp);
                }
                else
                {
                    doneReceiving = true;
                    screenReader.CancelOutputRead();
                    if (itemCount == 0)
                        Cleanup();
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
                    response.Dispose();

                    cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    itemCount--;
                    if (itemCount == 0 && doneReceiving)
                    {
                        //System.Threading.Thread.Sleep(200);
                        //if (itemCount == 0)
                        Cleanup();
                    }
                    System.Windows.Forms.MessageBox.Show(s + ' ' + responseObject.GetLowestSellPrice().ToString());
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

        private void Cleanup()
        {
            KeyPressed = false;
            if (client != null)
            {
                client.Dispose();
                client = null;
            }
            System.Windows.Forms.MessageBox.Show("end");
        }

        public async void Dispose()
        {
            KeyPressed = false;
            cancellationTokenSource.Cancel();
            if (tasks != null)
            {
                await Task.WhenAll(tasks);
            }
            if (client != null)
            {
                client.Dispose();
                client = null;
            }
            if (screenReader != null)
                screenReader.Dispose();
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
            }
        }
    }
}
