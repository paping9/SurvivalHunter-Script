using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Utils
{
    public abstract class Enumeration : IComparable
    {
        public int Id { get; private set; }
        public string Name { get; private set; }

        protected Enumeration(int id, string name) => (Id, Name) = (id, name);

        public static IEnumerable<T> GetAll<T>() where T : Enumeration =>
            typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Select(f => f.GetValue(null))
                .Cast<T>();
    
        public static T FromId<T>(int id) where T : Enumeration
        {
            return GetAll<T>().FirstOrDefault(e => e.Id == id)
                   ?? throw new InvalidOperationException($"No {typeof(T).Name} with Id {id} found.");
        }
    
        public static T FromName<T>(string name) where T : Enumeration
        {
            return GetAll<T>().FirstOrDefault(e => e.Name == name)
                   ?? throw new InvalidOperationException($"No {typeof(T).Name} with Name {name} found.");
        }
    
        public override bool Equals(object obj)
        {
            if (obj is not Enumeration otherValue) return false;

            return GetType() == obj.GetType() && Id.Equals(otherValue.Id);
        }

        public override int GetHashCode() => Id.GetHashCode();

        public int CompareTo(object other)
        {
            if (other is not Enumeration otherValue) return 0;

            return Id.CompareTo(otherValue.Id);
        }
    }
}
