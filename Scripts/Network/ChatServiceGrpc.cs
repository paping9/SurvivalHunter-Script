//using Cysharp.Threading.Tasks;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using System.Threading;

//namespace Gravity.ServerApi.gRPC
//{
//    using Grpc.Core;
//    using Chat.Grpc;
//    using Gravity.Entity;
//    using Error.Types;
//    using Google.Protobuf;
//    using deVoid.Utils;

//    public enum ConnectionState
//    {
//        None,
//        Connecting,
//        Connected,
//        Closed,
//        Broken,
//        ReConnecting,
//    }

//    public class ChatServiceGrpc : IChatService
//    {
//        private Channel _chatChannel;
//        private ChatService.ChatServiceClient _chatServiceClient;
//        private RepositoryContainer _repositoryContainer;

//        private IClientStreamWriter<ChatStreamData> _streamWriter;
//        private IAsyncStreamReader<ChatStreamData> _streamReader;
//        private CancellationTokenSource _cancellationTokenSource;

//        private HashSet<IChatReceiver> _chatReceivers = new HashSet<IChatReceiver>();
//        private HashSet<INotiReceiver> _notiReceivers = new HashSet<INotiReceiver>();

//        private Dictionary<ChatMsgType, Action<IMessage>> _handler = new Dictionary<ChatMsgType, Action<IMessage>>();
//        private Dictionary<ChatMsgType, Action<ChatMsgType, ChatStreamData>> _onRecv = new Dictionary<ChatMsgType, Action<ChatMsgType, ChatStreamData>>();

//        private ConnectionState _connectionState;

//        public ConnectionState ConnectionState
//        {
//            get { return _connectionState; }
//            set
//            {
//                _connectionState = value;
//                UnityEngine.Debug.Log($"<color=red>[ChatServer]</color> Change Connection State : {_connectionState}");
//            }
//        }


//        public ChatServiceGrpc(RepositoryContainer repositoryContainer, Channel channel)
//        {
//            _repositoryContainer = repositoryContainer;
//            _chatChannel = channel;
//            _chatServiceClient = new ChatService.ChatServiceClient(channel);
//            ConnectionState = ConnectionState.None;

//            Init();
//        }

//        private void Init()
//        {
//            // 수신시 강제 종료
//            _onRecv.Add(ChatMsgType.Disconnect, MakePacket<ChatMsg_Disconnect>);
//            _handler.Add(ChatMsgType.Disconnect, OnRecvDisConnect);

//            // 채팅 서버 수신 요청(default 값 Close. Close시 채팅 수신 안됨.
//            _onRecv.Add(ChatMsgType.ChatOpenRes, MakePacket<ChatMsg_ChatOpenRes>);
//            _handler.Add(ChatMsgType.ChatOpenRes, OnRecvPrevChatMsgList);

//            // 채팅 메시지 수신
//            _onRecv.Add(ChatMsgType.ChatMsgRecv, MakePacket<ChatMsg_ChatMsgRecv>);
//            _handler.Add(ChatMsgType.ChatMsgRecv, OnRecvChatMsg);

//            // 노티 수신
//            _onRecv.Add(ChatMsgType.Notify, MakePacket<ChatMsg_Nofity>);
//            _handler.Add(ChatMsgType.Notify, OnRecvNotify);

//            // 노티 수신
//            _onRecv.Add(ChatMsgType.NotifyTest, MakePacket<ChatMsg_NotifyTest>);
//            _handler.Add(ChatMsgType.NotifyTest, OnRecvNotifyTest);
//        }

//        private void MakePacket<T>(ChatMsgType chatMsgType, ChatStreamData streamData) where T : IMessage, new()
//        {
//            T pkt = new T();
//            pkt.MergeFrom(streamData.Data);

//            if (_handler.TryGetValue(chatMsgType, out var action))
//                action?.Invoke(pkt);
//        }

//        public void AddChatListener(IChatReceiver chatReceiver)
//        {
//            if (_chatReceivers.Contains(chatReceiver))
//                return;

//            _chatReceivers.Add(chatReceiver);
//        }

//        public void RemoveChatListener(IChatReceiver chatReceiver)
//        {
//            if (!_chatReceivers.Contains(chatReceiver))
//                return;

//            _chatReceivers.Remove(chatReceiver);
//        }

//        public void AddNotiListener(INotiReceiver receiver)
//        {
//            if (_notiReceivers.Contains(receiver))
//                return;

//            _notiReceivers.Add(receiver);
//        }

//        public void RemoveNotiListener(INotiReceiver receiver)
//        {
//            if (!_notiReceivers.Contains(receiver))
//                return;

//            _notiReceivers.Remove(receiver);
//        }

//        public async UniTask<(ResultType resultType, ErrorCodeType errCode)> Connect()
//        {
//            // overlapped call
//            if (_connectionState == ConnectionState.Connecting)
//                return (ResultType.GeneralError, ErrorCodeType.Unknown);

//            _repositoryContainer.Chat.SetConnected(false);

//            ConnectionState = ConnectionState.Connecting;
//            AsyncDuplexStreamingCall<ChatStreamData, ChatStreamData> serverRes = _chatServiceClient.StreamOpen(Metadata.Empty);

//            _streamWriter = serverRes.RequestStream;
//            _streamReader = serverRes.ResponseStream;

//            if (await _streamReader.MoveNext())
//            {
//                var streamData = _streamReader.Current as ChatStreamData;
//                if (streamData == null || streamData.MsgType != ChatMsgType.ConnectRes)
//                {
//                    ConnectionState = ConnectionState.Broken;
//                    return (ResultType.GeneralError, ErrorCodeType.Unknown);
//                }

//                ChatMsg_ConnectRes res = new ChatMsg_ConnectRes();
//                res.MergeFrom(streamData.Data);

//                if (res.Error != null)
//                {
//                    ConnectionState = ConnectionState.Broken;
//                    OnReconnect();
//                    UnityEngine.Debug.LogError($"chat server connect error : {res.Error.DebugMsg}");
//                    return (ResultType.CriticalError, res.Error.Code);
//                }

//                ConnectionState = ConnectionState.Connected;
//                _repositoryContainer.Chat.SetConnected(true);

//                StartRecvTask();

//                await UniTask.Delay(100);

//                return (ResultType.Success, ErrorCodeType.Ok);
//            }

//            return (ResultType.GeneralError, ErrorCodeType.Unknown);
//        }

//        public async UniTask<(ResultType resultType, ErrorCodeType errCode)> ReConnect()
//        {
//            AsyncDuplexStreamingCall<ChatStreamData, ChatStreamData> serverRes = _chatServiceClient.StreamOpen(Metadata.Empty);

//            _streamWriter = serverRes.RequestStream;
//            _streamReader = serverRes.ResponseStream;

//            if (await _streamReader?.MoveNext())
//            {
//                var streamData = _streamReader?.Current as ChatStreamData;
//                if (streamData == null || streamData.MsgType != ChatMsgType.ConnectRes)
//                    return (ResultType.GeneralError, ErrorCodeType.Unknown);

//                ChatMsg_ConnectRes res = new ChatMsg_ConnectRes();
//                res.MergeFrom(streamData.Data);

//                if (res.Error != null)
//                {
//                    UnityEngine.Debug.LogError($"chat server reconnect error : {res.Error.DebugMsg}");
//                    return (ResultType.CriticalError, res.Error.Code);
//                }

//                ConnectionState = ConnectionState.Connected;
//                _repositoryContainer.Chat.SetConnected(true);

//                StartRecvTask();

//                await UniTask.Delay(100);

//                return (ResultType.Success, ErrorCodeType.Ok);
//            }

//            return (ResultType.GeneralError, ErrorCodeType.Unknown);
//        }


//        public void DisConnect()
//        {
//            Send(ChatMsgType.Disconnect, new ChatMsg_Disconnect()).Forget();

//            ConnectionState = ConnectionState.Closed;
//            _cancellationTokenSource?.Cancel();
//            _cancellationTokenSource = null;

//            _repositoryContainer.Chat.SetConnected(false);
//        }


//        #region Send
//        public async UniTask Send(ChatMsgType msgType, IMessage packet)
//        {
//            if (_streamWriter == null || _connectionState != ConnectionState.Connected)
//                return;
//            Signals.Get<UIController.SetNetBlockSignal>().Dispatch(true);

//            var stream = new ChatStreamData();
//            stream.GuildNo = _repositoryContainer.Guild?.GetMine()?.No ?? 0;
//            stream.MberNo = _repositoryContainer.Account.Account.No;
//            stream.MsgType = msgType;
//            stream.Data = packet.ToByteString();

//            try
//            {
//                await _streamWriter.WriteAsync(stream);
//            }
//            catch (RpcException e)
//            {
//                OnRpcException(e);
//            }
//            catch (NullReferenceException e)
//            {
//                UnityEngine.Debug.LogError(e);
//            }
//            catch (Exception e)
//            {
//                UnityEngine.Debug.LogError(e);

//                if (e.Source == "Grpc.Core.Api")
//                {
//                    OnReconnect();
//                }
//            }

//            finally
//            {
//                Signals.Get<UIController.SetNetBlockSignal>().Dispatch(false);
//            }
//        }

//        #endregion


//        #region Recv

//        private void StartRecvTask()
//        {
//            _cancellationTokenSource = new CancellationTokenSource();
//            OnRecv().Forget();
//        }

//        public async UniTask OnRecv()
//        {
//            try
//            {
//                while (await _streamReader.MoveNext(_cancellationTokenSource.Token))
//                {
//                    ChatStreamData chatStreamData = _streamReader.Current;
//                    if (chatStreamData == null) continue;

//                    if (_onRecv.TryGetValue(chatStreamData.MsgType, out var action))
//                        action?.Invoke(chatStreamData.MsgType, chatStreamData);
//                }

//                await _streamWriter.CompleteAsync();

//                await UniTask.Delay(100);
//            }
//            catch (RpcException e)
//            {
//                OnRpcException(e);
//            }
//            catch (Exception e)
//            {
//                UnityEngine.Debug.LogError(e);
//            }
//        }

//        private void OnRecvDisConnect(IMessage packet)
//        {
//            DisConnect();
//        }

//        /// <summary>
//        /// 지난 채팅 메시지 목록 수신
//        /// </summary>
//        /// <param name="streamData"></param>
//        private void OnRecvPrevChatMsgList(IMessage packet)
//        {
//            _repositoryContainer.Chat.SetOpenRecv(true);

//            var msgPacket = packet as ChatMsg_ChatOpenRes;
//            if (msgPacket == null)
//                return;

//            if (msgPacket == null || msgPacket.SendMsgs == null || msgPacket.SendMsgs.Count <= 0)
//                return;

//            _repositoryContainer.Chat.Update(msgPacket.SendMsgs);

//            foreach (var receiver in _chatReceivers)
//                receiver.ReceiveChatMessage();
//        }

//        /// <summary>
//        /// 채팅 메시지 수신
//        /// </summary>
//        /// <param name="streamData"></param>
//        private void OnRecvChatMsg(IMessage packet)
//        {
//            var msgPacket = packet as ChatMsg_ChatMsgRecv;
//            if (msgPacket == null)
//                return;

//            _repositoryContainer.Chat.Update(msgPacket);

//            foreach (var receiver in _chatReceivers)
//                receiver.ReceiveChatMessage();
//        }

//        private void OnRecvNotify(IMessage packet)
//        {
//            var notify = packet as ChatMsg_Nofity;
//            if (notify == null)
//                return;

//            if (notify.Error != ErrorCodeType.Ok)
//            {
//                UnityEngine.Debug.LogError($"recv notify error code : {notify.Error}");
//                return;
//            }

//            UnityEngine.Debug.Log($"<color=yellow>[ChatServer]</color> recv noti : {notify.Notify}");

//            foreach (var receiver in _notiReceivers)
//                receiver.ReceiveNoti(notify.Notify);
//        }

//        private void OnRecvNotifyTest(IMessage packet)
//        {
//            var notify = packet as ChatMsg_NotifyTest;
//            if (notify == null)
//                return;

//            UnityEngine.Debug.Log($"<color=yellow>[ChatServer]</color> recv noti message : {notify.Message}");
//        }

//        private void OnRpcException(RpcException exception)
//        {
//            // 정상 종료
//            if (exception.StatusCode == StatusCode.Cancelled || _connectionState == ConnectionState.Closed)
//            {
//                _streamWriter = null;
//                _streamReader = null;
//                UnityEngine.Debug.Log($"<color=green>[ChatServer]</color>rpc exception. code: {exception.StatusCode} detail: {exception.Status.Detail} message: {exception}");
//                return;
//            }

//            // 잘못된 서버 정보(url or token)
//            if (exception.StatusCode == StatusCode.Unavailable)
//            {
//                _streamWriter = null;
//                _streamReader = null;
//                UnityEngine.Debug.Log($"<color=red>[ChatServer]</color>rpc exception. code: {exception.StatusCode} detail: {exception.Status.Detail} message: {exception}");
//                return;
//            }

//            if (_connectionState == ConnectionState.Broken)
//                return;

//            ConnectionState = ConnectionState.Broken;
//            UnityEngine.Debug.LogError($"rpc exception. code: {exception.StatusCode} detail: {exception.Status.Detail} message: {exception}");

//            OnReconnect();
//        }

//        public void OnReconnect()
//        {
//            if (_connectionState == ConnectionState.ReConnecting)
//                return;

//            ConnectionState = ConnectionState.ReConnecting;
//            OnReconnectAsync().Forget();
//        }

//        private async UniTask OnReconnectAsync()
//        {
//            if (_cancellationTokenSource != null)
//            {
//                _cancellationTokenSource?.Cancel();
//                _cancellationTokenSource = null;
//            }

//            await UniTask.Delay(100);

//            int remainRequestCount = 3;
//            while (remainRequestCount-- > 0)
//            {
//                await UniTask.Delay(100);

//                var serverRes = await ReConnect();
//                if (serverRes.resultType != ResultType.Success)
//                    continue;

//                UnityEngine.Debug.Log($"<color=green>[ChatServer]</color> success reconnect chat server");
//                foreach (var receiver in _chatReceivers)
//                    receiver.ReceiveConnected();

//                break;
//            }
//        }
//    }
//    #endregion
//}
