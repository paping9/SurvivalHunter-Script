using System.Collections;

namespace Game
{
    public class IDGenerator
    {
        private int _id = 0;

        public IDGenerator() 
        {
        }

        public void Free(int id)
        {
            
        }
        
        public int GetId()
        {
            return _id++;
        }
    }
}