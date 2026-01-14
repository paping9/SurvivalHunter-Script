namespace UI
{
    public class ContentUIID : UIID
    {
        #region Title
        public static UIID TitleWindow = new ContentUIID(100, "UI/Title/TitleUI");

        #endregion

        protected ContentUIID(int id, string name) : base(id, name)
        {
        }
    }
}