//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Grpc.Core;

//namespace Gravity.ServerApi.gRPC
//{
//    public class ServerChannel
//    {
//        public string Host { get; private set; }
//        public int Port { get; private set; }
//        public Grpc.Core.Channel Channel { get; private set; }
//        public string Token { get; private set; }
//        public string ConnectionID { get; private set; }

//        public ServerChannel(string host, int port)
//        {
//            this.Channel = CreateChannel(host, port);
//        }

//        public ServerChannel(string hostUrl)
//        {
//            var url = SeperateHostAndPort(hostUrl);

//            this.Channel = CreateChannel(url.Host, url.Port);
//            this.Channel.ConnectAsync();
//        }

//        private (string Host, int Port) SeperateHostAndPort(string hostUrl)
//        {
//            var splits = hostUrl.Split(":", StringSplitOptions.RemoveEmptyEntries);

//            return (splits[0], int.Parse(splits[1]));
//        }

//        public void SetToken(string token)
//        {
//            this.Token = "Bearer " + token;
//        }

//        public void Shutdown()
//        {
//            this.Channel.ShutdownAsync().Wait();
//            this.Channel = null;
//        }

//        public bool IsValidChannel()
//        {
//            if (Channel == null) return false;
//            if (Channel.State == ChannelState.Connecting) return true;
//            return false;
//        }

//        private Grpc.Core.Channel CreateChannel(string hostIP, int hostPort, string selfSignedCert = "",
//            string sslTargetNameOverride = "")
//        {
//            ChannelCredentials credentials = ChannelCredentials.SecureSsl;

//            List<ChannelOption> channelOptions = new List<ChannelOption>();

//            if (!string.IsNullOrWhiteSpace(selfSignedCert))
//            {
//                credentials = new SslCredentials(selfSignedCert);

//                if (!string.IsNullOrWhiteSpace(sslTargetNameOverride))
//                {
//                    channelOptions.Add(new ChannelOption(ChannelOptions.SslTargetNameOverride, sslTargetNameOverride));
//                }
//            }

//            var callCredentials = CallCredentials.FromInterceptor(AuthInterceptorTask);
//            var credentialsWithCall = Grpc.Core.ChannelCredentials.Create(credentials, callCredentials);

//            Grpc.Core.Channel channel = new Grpc.Core.Channel(hostIP, hostPort, credentialsWithCall, channelOptions);

//            return channel;
//        }

//        private Task AuthInterceptorTask(AuthInterceptorContext context, Metadata metadata)
//        {
//            if (!string.IsNullOrWhiteSpace(ConnectionID)) metadata.Add("x-connection", ConnectionID);
//            if (!string.IsNullOrWhiteSpace(Token)) metadata.Add("Authorization", Token);
//            //if (!string.IsNullOrWhiteSpace(xTID)) metadata.Add("xTID", xTID);

//            return Task.CompletedTask;
//        }
//    }
//}