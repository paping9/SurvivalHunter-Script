using System.Collections.Generic;
using UnityEngine;
using Defs;

namespace Game.Map
{
    public interface IMapRoomCreator
    {
        void Initialize(MapRoomDatabase roomDatabase);
        MapRoom CreateRoom(ContentType contentType, Room room, MapThemeInfo mapThemeInfo, Transform parent = null);
        MapRoom CreateRoom(MapRoomInfo roomInfo, MapThemeInfo themeInfo, List<DoorInfo> doorInfoMap, Transform parent = null);
    }
}