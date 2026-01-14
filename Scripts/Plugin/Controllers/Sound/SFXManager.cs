// using System;
// using UnityEngine;
//
// using AssetBundle;
// using Utils;
// using Utils.Extension;
// using Utils.Pool;
//
// namespace Sound
// {
//     public enum SFXType
//     {
//         _2D,
//         _3D
//     }
//
//     /// <summary>
//     /// Tier 별 풀을 따로 관리한다. SFX가 부족할 때 티어에 따라 재생산 방식이 나뉜다.    
//     /// </summary>
//     public enum SfxTier
//     {
//         Third,      //PoolBase.PopOptionForNotEnough.None //필요 시 사용
//         Second,     //PoolBase.PopOptionForNotEnough.Force
//         Top         //PoolBase.PopOptionForNotEnough.Instantiate
//     }
//
//
//     public class SfxManager : SingletonMB<SfxManager>
//     {
//         private PoolGameObject<SFXObject> _thirdTierSfxPool;
//         private PoolGameObject<SFXObject>  _secTierSfxPool;
//         private PoolGameObject<SFXObject>  _topTierSfxPool;
//         private Transform _listenerTrans;
//         private GameObject _prefSfxObject;
//         private GameObject _prefHitSfxObject;
//         
//         private IPoolingManager _poolingManager;
//
//         private bool _mute = false;
//         public bool Mute
//         {
//             get => _mute;
//             set
//             {
//                 if (_topTierSfxPool == null || _secTierSfxPool == null)
//                     return;
//
//                 if (_mute == value)
//                     return;
//
//                 _mute = value;
//
//                 var listSfxObj = _topTierSfxPool.GetAllItem();
//                 while (listSfxObj.MoveNext())
//                 {
//                     var sfxObj = listSfxObj.Current as SFXObject;
//                     if (sfxObj == null)
//                         continue;
//
//                     sfxObj.AudioSource.mute = value;
//                 }
//
//                 listSfxObj = _secTierSfxPool.GetAllItem();
//                 while (listSfxObj.MoveNext())
//                 {
//                     var sfxObj = listSfxObj.Current as SFXObject;
//                     if (sfxObj == null)
//                         continue;
//
//                     sfxObj.AudioSource.mute = value;
//                 }
//             }
//         }
//
//         private float _volume = 1f;
//
//         public float Volume
//         {
//             get => _volume;
//             set
//             {
//                 if (_volume == value)
//                     return;
//
//                 _volume = value;
//
//                 //if(SFXDeco.set_AudioSource != null)
//                 //{
//                 //    foreach(var audioSource in SFXDeco.set_AudioSource)
//                 //    {
//                 //        if(audioSource != null)
//                 //            audioSource.volume = _volume;
//                 //    }
//                 //}
//
//                 var listSfxObj = _topTierSfxPool.GetAllItem();
//                 while (listSfxObj.MoveNext())
//                 {
//                     var sfxObj = listSfxObj.Current as SFXObject;
//                     if (sfxObj == null)
//                         continue;
//
//                     sfxObj.AudioSource.volume = value;
//                 }
//                 
//                 listSfxObj = _secTierSfxPool.GetAllItem();
//                 while (listSfxObj.MoveNext())
//                 {
//                     var sfxObj = listSfxObj.Current as SFXObject;
//                     if (sfxObj == null)
//                         continue;
//
//                     sfxObj.AudioSource.volume = value;
//                 }
//
//                 listSfxObj = _thirdTierSfxPool.GetAllItem();
//                 while (listSfxObj.MoveNext())
//                 {
//                     var sfxObj = listSfxObj.Current as SFXObject;
//                     if (sfxObj == null)
//                         continue;
//
//                     sfxObj.AudioSource.volume = value;
//                 }
//             }
//         }
//
//         public GameObject LoadSfxObject(string name)
//         {
//             var handler = AddressableManager.Instance.LoadInstantiate($"Sfx/{name}", null);
//             handler.WaitForCompletion();
//
//             return handler.Result;
//         }
//
//         public void Init()
//         {
//             if (_topTierSfxPool == null || _secTierSfxPool == null)
//                 return;
//
//             var mainCamera = Camera.main;
//             _listenerTrans = mainCamera == null ? transform : mainCamera.transform;
//
//             _prefSfxObject =  LoadSfxObject("SFXObject");
//             _prefHitSfxObject = LoadSfxObject("HitSFXObject");
//
//             _topTierSfxPool
//             //합쳐서 32개가 되게 작업.
//             _topTierSfxPool.Init(_prefSfxObject, 4);
//             _secTierSfxPool.Init(_prefSfxObject, 14);
//             _thirdTierSfxPool.Init(_prefHitSfxObject, 14);
//
//             //현재 지스타 버전에서 SFX 플레이를 PoolBase.PopOptionForNotEnough.Instantiate로 처리하고 있어서 프리팹을 살림.
//             //추후에 해당 내용 PoolBase.PopOptionForNotEnough.Force로 변경할 때 바로 Destroy해준다.
//             _prefSfxObject.transform.SetParent(_topTierSfxPool.transform);
//             _prefSfxObject.SetActive(false);
//
//             _prefHitSfxObject.transform.SetParent(_topTierSfxPool.transform);
//             _prefHitSfxObject.SetActive(false);
//
//             //_pref.transform.SetParent(_topTierSFXPool.transform);
//             //_pref.SetActive(false);
//             //GameObject.Destroy(obj);
//
//             //var option = Grm.LocalData.GetOption();
//             //Mute = !option.TotalSoundOn || !option.SFXOn;
//             //Volume = option.TotalSoundVolume * option.SFXVolume;
//         }
//
//         /// <summary>
//         /// 나중에 초기화 필요할 때 호출해주자.
//         /// </summary>
//         public void Release()
//         {
//             if (_prefSfxObject != null)
//             {
//                 AddressableManager.Instance.ReleaseInstantiate(_prefSfxObject);
//             }
//         }
//
//         private void Awake()
//         {
//             var obj = new GameObject("TopTier");
//             _topTierSfxPool = obj.GetOrAddComponent<PoolBase>();
//             obj.transform.SetParent(this.transform);
//
//             obj = new GameObject("SecondTier");
//             _secTierSfxPool = obj.GetOrAddComponent<PoolBase>();
//             obj.transform.SetParent(this.transform);
//
//             obj = new GameObject("ThirdTier");
//             _thirdTierSfxPool = obj.GetOrAddComponent<PoolBase>();
//             obj.transform.SetParent(this.transform);
//         }
//
//         /// <summary>
//         /// 
//         /// </summary>
//         /// <param name="vLocalPos">로컬 좌표, tr_Parent == null이면 월드 좌표</param>
//         /// <param name="qLocalRot">로컬 회전, tr_Parent == null이면 월드 회전</param>
//         /// <param name="vLocalScale">로컬 스케일, tr_Parent == null이면 월드 스케일</param>
//         /// <param name="parent">부모 Transform</param>
//         /// <param name="clip">재생할 AudioClip</param>
//         /// <param name="isLoop">반복 재생 여부</param>
//         /// <param name="playTimeOffset">빨리감기 Time</param>
//         /// <param name="tier"></param>
//         /// <param name="volume"></param>
//         /// <returns></returns>
//         public SFXObject PlaySfx(
//             Vector3 vLocalPos,
//             Quaternion qLocalRot,
//             Vector3 vLocalScale,
//             Transform parent,
//             AudioClip clip,
//             SFXType sfxType,
//             bool isLoop,
//             float playTimeOffset = 0f,
//             SfxTier tier = SfxTier.Second,
//             float volume = 1f)
//         {
//             if (clip == null)
//                 return null;
//
//             SFXObject sfx = null;
//             switch (tier)
//             {
//                 case SfxTier.Second:
//                     sfx = _secTierSfxPool.Pop(PoolBase.PopOptionForNotEnough.Force) as SFXObject;
//                     break;
//                 case SfxTier.Top:
//                     sfx = _topTierSfxPool.Pop(PoolBase.PopOptionForNotEnough.Instantiate) as SFXObject;
//                     break;
//                 case SfxTier.Third:
//                     // sfx = _secTierSFXPool.Pop(PoolBase.PopOptionForNotEnough.None) as SFXObject;
//                     sfx = _thirdTierSfxPool.Pop(PoolBase.PopOptionForNotEnough.Instantiate) as SFXObject;
//                     break;
//                 default:
//                     throw new ArgumentOutOfRangeException(nameof(tier), tier, null);
//             }
//
//             if (sfx == null)
//                 return null;
//
//             sfx.GenerateObject(vLocalPos, qLocalRot, vLocalScale, parent);
//             sfx.PlaySFX(clip, sfxType, isLoop, playTimeOffset, null);
//             sfx.AudioSource.volume = this.Volume * volume;
//             return sfx;
//         }
//
//         public SFXObject PlaySfx(
//             Vector3 vLocalPos,
//             Quaternion qLocalRot,
//             Vector3 vLocalScale,
//             Transform parent,
//             AudioClip clip,
//             SFXType sfxType,
//             System.Action<SFXObject> endCallback,
//             float playTimeOffset = 0f,
//             SfxTier tier = SfxTier.Second)
//         {
//             if (clip == null)
//             {
//                 endCallback?.Invoke(null);
//                 return null;
//             }
//
//             SFXObject sfx = null;
//             switch (tier)
//             {
//                 case SfxTier.Second:
//                     sfx = _secTierSfxPool.Pop(PoolBase.PopOptionForNotEnough.Force) as SFXObject;
//                     break;
//                 case SfxTier.Top:
//                     sfx = _topTierSfxPool.Pop(PoolBase.PopOptionForNotEnough.Instantiate) as SFXObject;
//                     break;
//                 case SfxTier.Third:
//                     // sfx = _secTierSFXPool.Pop(PoolBase.PopOptionForNotEnough.None) as SFXObject;
//                     sfx = _thirdTierSfxPool.Pop(PoolBase.PopOptionForNotEnough.Instantiate) as SFXObject;
//                     break;
//                 default:
//                     throw new ArgumentOutOfRangeException(nameof(tier), tier, null);
//             }
//
//             if (sfx == null)
//                 return null;
//
//             //var sfx = _topTierSFXPool.Pop(PoolBase.PopOptionForNotEnough.Instantiate) as SFXObject;
//
//             sfx.GenerateObject(vLocalPos, qLocalRot, vLocalScale, parent);
//             sfx.PlaySFX(clip, sfxType, false, playTimeOffset, endCallback);
//             sfx.AudioSource.volume = this.Volume;
//             return sfx;
//         }
//
//         public SFXObject PlaySfx(
//             Transform parent,
//             AudioClip clip,
//             SFXType sfxType,
//             bool isLoop,
//             float playTimeOffset = 0f,
//             SfxTier tier = SfxTier.Second)
//         {
//             return PlaySfx(Vector3.zero, Quaternion.identity, Vector3.one, parent, clip, sfxType, isLoop, playTimeOffset, tier);
//         }
//
//         public SFXObject PlaySfx(
//             Transform parent,
//             AudioClip clip,
//             SFXType sfxType,
//             System.Action<SFXObject> endCallback,
//             float playTimeOffset = 0f,
//             SfxTier tier = SfxTier.Second)
//         {
//             return PlaySfx(Vector3.zero, Quaternion.identity, Vector3.one, parent, clip, sfxType, endCallback, playTimeOffset, tier);
//         }
//
//         /// <summary>
//         /// 부모없이 호출하는 경우는 부모는 SFX매니저에, 위치는 현재 메인카메라 위치로 설정한다.
//         /// </summary>
//         public SFXObject PlaySfx(AudioClip clip, SFXType sfxType)
//         {
//             return PlaySfx(_listenerTrans.position, Quaternion.identity, Vector3.one, this.transform, clip, sfxType, false, 0.0f);
//         }
//
//         /// <summary>
//         /// BGM Key에 할당되어있는 클립을 SFX로 재생할 때 사용
//         /// </summary>
//         public void PlaySfx(string bgmKey, SFXType sfxType, Action<SFXObject> result = null)
//         {
//             AddressableManager.Instance.Load<AudioClip>(bgmKey, (clip) =>
//             {
//                 var sfx = PlaySfx(clip, sfxType);
//                 result?.Invoke(sfx);
//             });
//         }
//
//         /// <summary>
//         /// 부모없이 호출하는 경우는 부모는 SFX매니저에, 위치는 현재 메인카메라 위치로 설정한다.
//         /// </summary>
//         public SFXObject PlaySfx(AudioClip clip, SFXType sfxType, System.Action<SFXObject> endCallback)
//         {
//             return PlaySfx(_listenerTrans.position, Quaternion.identity, Vector3.one, this.transform, clip, sfxType, endCallback, 0.0f);
//         }
//
//
//         public void RetrieveAllItems()
//         {
// #if DevClient
//             Debug.Log("SFXManager->RetrieveAllItems");
// #endif
//             _topTierSfxPool.RetrieveAllItems();
//             _secTierSfxPool.RetrieveAllItems();
//             _thirdTierSfxPool.RetrieveAllItems();
//         }
//     }
// }
