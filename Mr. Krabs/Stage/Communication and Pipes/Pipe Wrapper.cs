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
    public class PipeWrapper {

        // pipin
        private NamedPipeClientStream clientPipe;
        private BinaryWriter writerStream;

        private string pipeName;

        public PipeWrapper(string pipe_name) {
            init(pipe_name);
        }

        private void init(string pipe_name) {

            pipeName = pipe_name;
            clientPipe = new NamedPipeClientStream(pipeName);
            writerStream = new BinaryWriter(clientPipe);
        }

        public event EventHandler Connected;
        public async Task Send(string message) {
            Debug.WriteLine($"Json Sent: {Environment.NewLine}{message}");
            await Send(Encoding.UTF8.GetBytes(message));
        }
        public async Task Send(byte[] bytes) {
            // await _server_pipe.WaitForConnectionAsync();
            await clientPipe.WriteAsync(bytes, 0, bytes.Length);
            writerStream.Flush();
            //   _client_pipe.WaitForPipeDrain();
        }
        public void Stop() {
            if (clientPipe != null) {
                clientPipe.Close();
                clientPipe = null;
            }
        }
        public void Start() {
            Task.Factory.StartNew(() => {
                if (clientPipe == null) {
                    init(pipeName);
                }
                if (!clientPipe.IsConnected)
                    clientPipe.Connect();

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
