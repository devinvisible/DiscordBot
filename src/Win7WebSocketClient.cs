using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord.Net.WebSockets;
using SuperSocket.ClientEngine;
using WebSocket4Net;

namespace DiscordBot
{
    internal class Win7WebSocketClient : IWebSocketClient, IDisposable
    {
        private readonly SemaphoreSlim _lock;
        private readonly Dictionary<string, string> _headers;
        private WebSocket _client;
        private CancellationTokenSource _cancelTokenSource;
        private CancellationToken _cancelToken;
        private CancellationToken _parentToken;
        private ManualResetEventSlim _waitUntilConnect;
        private bool _isDisposed;

        public event Func<byte[], int, int, Task> BinaryMessage;
        public event Func<string, Task> TextMessage;
        public event Func<Exception, Task> Closed;

        public Win7WebSocketClient()
        {
            this._headers = new Dictionary<string, string>();
            this._lock = new SemaphoreSlim(1, 1);
            this._cancelTokenSource = new CancellationTokenSource();
            this._cancelToken = CancellationToken.None;
            this._parentToken = CancellationToken.None;
            this._waitUntilConnect = new ManualResetEventSlim();
        }

        private void Dispose(bool disposing)
        {
            if (!this._isDisposed)
            {
                if (disposing)
                {
                    this.DisconnectInternalAsync(true).GetAwaiter().GetResult();
                }
                this._isDisposed = true;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        public async Task ConnectAsync(string host)
        {
            await this._lock.WaitAsync().ConfigureAwait(false);
            try
            {
                await this.ConnectInternalAsync(host).ConfigureAwait(false);
            }
            finally
            {
                this._lock.Release();
            }
        }

        private async Task ConnectInternalAsync(string host)
        {
            await this.DisconnectInternalAsync(false).ConfigureAwait(false);
            this._cancelTokenSource = new CancellationTokenSource();
            this._cancelToken = CancellationTokenSource.CreateLinkedTokenSource(this._parentToken, this._cancelTokenSource.Token).Token;
            this._client = new WebSocket(host, "", null, this._headers.ToList<KeyValuePair<string, string>>(), "", "", WebSocketVersion.None, null)
            {
                EnableAutoSendPing = false,
                NoDelay = true,
                Proxy = null
            };
            this._client.MessageReceived += this.OnTextMessage;
            this._client.DataReceived += this.OnBinaryMessage;
            this._client.Opened += new EventHandler(this.OnConnected);
            this._client.Closed += new EventHandler(this.OnClosed);
            this._client.Open();
            this._waitUntilConnect.Wait(this._cancelToken);
        }

        public async Task DisconnectAsync()
        {
            await this._lock.WaitAsync().ConfigureAwait(false);
            try
            {
                await this.DisconnectInternalAsync(false).ConfigureAwait(false);
            }
            finally
            {
                this._lock.Release();
            }
        }

        private Task DisconnectInternalAsync(bool isDisposing = false)
        {
            this._cancelTokenSource.Cancel();
            if (this._client == null)
            {
                return Task.Delay(0);
            }
            if (this._client.State == WebSocketState.Open)
            {
                try
                {
                    this._client.Close(1000, "");
                }
                catch
                {
                }
            }
            this._client.MessageReceived -= this.OnTextMessage;
            this._client.DataReceived -= this.OnBinaryMessage;
            this._client.Opened -= new EventHandler(this.OnConnected);
            this._client.Closed -= new EventHandler(this.OnClosed);
            try
            {
                this._client.Dispose();
            }
            catch
            {
            }
            this._client = null;
            this._waitUntilConnect.Reset();
            return Task.Delay(0);
        }

        public void SetHeader(string key, string value)
        {
            this._headers[key] = value;
        }

        public void SetCancelToken(CancellationToken cancelToken)
        {
            this._parentToken = cancelToken;
            this._cancelToken = CancellationTokenSource.CreateLinkedTokenSource(this._parentToken, this._cancelTokenSource.Token).Token;
        }

        public async Task SendAsync(byte[] data, int index, int count, bool isText)
        {
            await this._lock.WaitAsync(this._cancelToken).ConfigureAwait(false);
            try
            {
                if (isText)
                {
                    this._client.Send(Encoding.UTF8.GetString(data, index, count));
                }
                else
                {
                    this._client.Send(data, index, count);
                }
            }
            finally
            {
                this._lock.Release();
            }
        }

        private void OnTextMessage(object sender, MessageReceivedEventArgs e)
        {
            this.TextMessage(e.Message).GetAwaiter().GetResult();
        }

        private void OnBinaryMessage(object sender, DataReceivedEventArgs e)
        {
            this.BinaryMessage(e.Data, 0, e.Data.Count<byte>()).GetAwaiter().GetResult();
        }

        private void OnConnected(object sender, object e)
        {
            this._waitUntilConnect.Set();
        }

        private void OnClosed(object sender, object e)
        {
            ErrorEventArgs errorEventArgs = e as ErrorEventArgs;
            Exception arg = ((errorEventArgs != null) ? errorEventArgs.Exception : null) ?? new Exception("Unexpected close");
            this.Closed(arg).GetAwaiter().GetResult();
        }
    }
}