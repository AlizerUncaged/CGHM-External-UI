using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Mr.Krabs.Stage.Communication_and_Pipes {
    public class Pipe_Wrapper {

        // pipin
        private NamedPipeClientStream _client_pipe;
        private BinaryWriter _writer;

        private string _pipe_name;

        public Pipe_Wrapper(string pipe_name) {
            init(pipe_name);
        }

        private void init(string pipe_name) {

            _pipe_name = pipe_name;
            _client_pipe = new NamedPipeClientStream(_pipe_name);
            _writer = new BinaryWriter(_client_pipe);
        }

        public event EventHandler Connected;
        public async Task Send(string message) {
            Debug.WriteLine($"Json Sent: {Environment.NewLine}{message}");
            await Send(Encoding.UTF8.GetBytes(message));
        }
        public async Task Send(byte[] bytes) {
            // await _server_pipe.WaitForConnectionAsync();
            await _client_pipe.WriteAsync(bytes, 0, bytes.Length);
            _writer.Flush();
            //   _client_pipe.WaitForPipeDrain();
        }
        public void Stop() {
            if (_client_pipe != null) {
                _client_pipe.Close();
                _client_pipe = null;
            }
        }
        public void Start() {
            Task.Factory.StartNew(() => {
                if (_client_pipe == null) {
                    init(_pipe_name);
                }
                if (!_client_pipe.IsConnected)
                    _client_pipe.Connect();

                Connected?.Invoke(this, null);
            });

            //return;

            //Task.Factory.StartNew(() => {
            //    while (_keep_alive) {
            //        // watch the file every 1 second


            //        //        byte[] bytes = new byte[2048];
            //        //        var received = _client_pipe.Read(bytes, 0, bytes.Length);
            //        //        string stringed = Encoding.UTF8.GetString(bytes).Trim();

            //        //        MessageBox.Show("received: " + stringed);
            //        //        Received?.Invoke(this, stringed);
            //    }
            //});
        }
    }
}
