using AssetBundle;
using Game.Map;
using Message;
using Utils.Pool;

namespace System
{
    public static partial class SystemLocator
    {
#if UNITY_EDITOR
        public static void InitEditor()
        {
            Register<IAddressableManager>(()=>
            {
                return new AddressableManager();
            });
            Register<IMessageBus>(()=>
            {
                return new MessageBus();
            });
            Register<IGenericPoolManager>(()=>
            {
                return new GenericPoolManager();
            });
            Register<IMapRoomCreator>(()=>
            {
                return new MapRoomCreator(Get<IGenericPoolManager>());
            });
            
            var db = UnityEditor.AssetDatabase.LoadAssetAtPath<MapRoomDatabase>("Assets/AssetBundle/Maps/MapRoomDatabase.asset");
            Get<IMapRoomCreator>().Initialize(db);
        }
#endif
    }
}