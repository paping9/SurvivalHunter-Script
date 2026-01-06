using System;
using UnityEngine.AddressableAssets;
using Utils.Collections;

namespace Defs
{
    [Serializable]
    public class MapThemeDoorTileDictionary : SerializedDictionary<WallType, string> {}
}