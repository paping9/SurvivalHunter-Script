using System;

namespace UI
{
    public class UiData
    {
        public UIID                 UiId;
        public UIParam              Param;
        public Action<UIBase>       CallBackOpenStart;
        public Action<UIBase>       CallBackOpenEnd;
        public Action<UIBase>       CallBackCloseStart;
        public Action<UIBase>       CallBackCloseEnd;

        public UiData(UIID uiId)
        {
            UiId = uiId;
        }

        public UiData(UIID uiId, UIParam param) : this(uiId)
        {
            Param = param;
        }

        public UiData(UIID uiId
            , Action<UIBase> callbackOpenStart
            , Action<UIBase> callbackOpenEnd = null
            , Action<UIBase> callbackCloseStart = null
            , Action<UIBase> callbackCloseEnd = null) : this(uiId)
        {
            CallBackOpenStart = callbackOpenStart;
            CallBackOpenEnd = callbackOpenEnd;
            CallBackCloseStart = callbackCloseStart;
            CallBackCloseEnd = callbackCloseEnd;
        }

        public UiData(UIID uiId
            , UIParam param
            , Action<UIBase> callbackOpenStart
            , Action<UIBase> callbackOpenEnd = null
            , Action<UIBase> callbackCloseStart = null
            , Action<UIBase> callbackCloseEnd = null) : this(uiId, param)
        {
            CallBackOpenStart = callbackOpenStart;
            CallBackOpenEnd = callbackOpenEnd;
            CallBackCloseStart = callbackCloseStart;
            CallBackCloseEnd = callbackCloseEnd;
        }


        public void Remove()
        {
            Param = null;
            CallBackOpenStart = null;
            CallBackOpenEnd = null;
            CallBackCloseStart = null;
            CallBackCloseEnd = null;
        }

    }

    public enum OpenType
    {
        Menu,
        Popup,
    }

    public enum DepthType
    {
        Low = 0,
        Middle,
        High,
    }

}
