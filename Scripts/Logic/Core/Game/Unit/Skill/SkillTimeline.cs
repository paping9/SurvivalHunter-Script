using System;

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

using Cysharp.Threading.Tasks;
using System.Threading;

namespace Game
{
    public class SkillTimeline : IDisposable
    {
        private PlayableDirector            _director = null;
        private INotificationReceiver       _timelineNotiReceiver;
        private CancellationTokenSource     _cancelToken;
        private bool                        _isDisposed = false;

        public SkillTimeline(INotificationReceiver timelineReceiver)
        {
            _timelineNotiReceiver = timelineReceiver;
        }

        public bool IsPlaying()
        {
            if (_director == null || _director.playableAsset == null) return false;

            return _director.enabled;
        }

        public void PlayTimeline(PlayableAsset asset)
        {
            BindingDirector(asset);
            PlayTimeline().Forget();
        }

        private void BindingDirector(PlayableAsset asset)
        {
            if (_director == null) return;

            _director.enabled = false;
            _director.playableAsset = asset;

            var timelineAsset = asset as TimelineAsset;

            if (timelineAsset == null) return;
            foreach (var trackAsset in timelineAsset.GetRootTracks())
            {
                // TODO : 스킬 연동
            }
        }

        private async UniTaskVoid PlayTimeline()
        {
            _director.enabled = true;
            _director.time = 0f;
            _director.Play();

            CancelToken();

            _cancelToken = new CancellationTokenSource();

            var outputCount = _director.playableGraph.GetOutputCount();

            for (var i = 0; i < outputCount; i++)
            {
                _director.playableGraph.GetOutput(i).AddNotificationReceiver(_timelineNotiReceiver);
            }

            while (_director != null && _director.state == PlayState.Playing)
            {
                await UniTask.DelayFrame(1, PlayerLoopTiming.Update, _cancelToken.Token);
            }

            if (_cancelToken != null)
            {
                _cancelToken.Dispose();
                _cancelToken = null;
            }

        }

        public void EndTimeline()
        {
            CancelToken();
        }

        public void StopTimeline()
        {
            EndTimeline();
        }

        public void PauseTimeline()
        {
            if (_director != null)
                _director.Pause();
        }

        public void ResumeTimeline()
        {
            if (_director != null)
                _director.Resume();
        }

        private void CancelToken()
        {
            if (_cancelToken == null) return;

            if (_cancelToken.IsCancellationRequested == false) _cancelToken.Cancel();
            _cancelToken.Dispose();
            _cancelToken = null;
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _isDisposed = true;
        }
    }
}
