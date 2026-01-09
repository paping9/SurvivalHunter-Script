using System;
using System.Collections.Generic;
using Utils;

namespace Game
{
    /// <summary>
    /// In Game 에서 데이터 로직은 여기를 이용한다.
    /// </summary>
    public class GameServiceManager : IGameServiceManager
    {
        public GameServiceUnit Unit { get; private set; }
        public GameServiceMove Move { get; private set; }
        public GameServiceSkill Skill { get; private set; }
        public GameServiceStatus Status { get; private set; }

        public GameServiceManager()
        {

        }
    }
}
