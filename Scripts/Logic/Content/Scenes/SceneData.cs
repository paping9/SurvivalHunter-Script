namespace Scene
{
    public static class SceneHelper
    {
        public static string GetScenePath(ContentSceneType contentsThemaType)
        {
            switch (contentsThemaType)
            {
                case ContentSceneType.Game: return "Game";
                case ContentSceneType.Lobby: return "Lobby";
            }
            return "";
        }

        public static string GetSceneName(ContentSceneType contentsThemaType)
        {
            switch (contentsThemaType)
            {
                case ContentSceneType.Title: return "Title";
                case ContentSceneType.Lobby: return "Lobby";
                case ContentSceneType.Game: return "Game";
                case ContentSceneType.Empty: return "Empty";
            }
            return "";
        }
    }

    public class SceneData
    {

    }

    public class GameSceneData : SceneData
    {
        public int MapId;
    }

}
