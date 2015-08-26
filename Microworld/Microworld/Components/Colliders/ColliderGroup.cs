using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Components.Colliders
{
    public class ColliderGroup
    {
        private String name = "";
        internal List<ColliderGroup> blacklist = new List<ColliderGroup>();

        public String Name
        {
            get { return name; }
            internal set { name = value; }
        }

        internal ColliderGroup(String name)
        {
            this.name = name;
        }

        public bool DoesCollideWith(ColliderGroup g)
        {
            return !blacklist.Contains(g);
        }
    }



    public static class ColliderGroupManager
    {
        internal static List<ColliderGroup> infos = new List<ColliderGroup>();


        internal static void Initialize()
        {
            BlacklistCollisions("Laser", "Laser");
        }

        public static ColliderGroup GetGroup(String name)
        {
            for (int i = 0; i < infos.Count; i++)
            {
                if (infos[i].Name == name)
                    return infos[i];
            }

            ColliderGroup g = new ColliderGroup(name);
            infos.Add(g);
            return g;
        }

        public static void BlacklistCollisions(String group1, String group2)
        {
            var a = GetGroup(group1);
            var b = GetGroup(group2);

            a.blacklist.Add(b);
            b.blacklist.Add(a);
        }

        public static void BlacklistCollisions(ColliderGroup a, ColliderGroup b)
        {
            if (a == null || b == null)
                throw new NullReferenceException();
            a.blacklist.Add(b);
            b.blacklist.Add(a);
        }

        public static bool CanCollide(String group1, String group2)
        {
            var a = GetGroup(group1);
            var b = GetGroup(group2);

            return a.DoesCollideWith(b);
        }

        public static bool CanCollide(ColliderGroup a, ColliderGroup b)
        {
            return a.DoesCollideWith(b);
        }
    }
}
