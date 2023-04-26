using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;


using WotlkClient.Constants;
using WotlkClient.Terrain;
using WotlkClient.Shared;

namespace WotlkClient.Clients
{
    /// <summary>Keeps track of the world. Keeps track of all Objects, including the player as well as providing methods to move (or warp) the player and provides collision detection and pathing.</summary>
    public class ObjectMgr
    {
        private static ObjectMgr instance;
        private List<Object> mObjects;

        private ObjectMgr() 
        { 
            mObjects = new List<Object>(); 
        }

        public static ObjectMgr GetInstance()
        {
            if(instance == null)
                instance = new ObjectMgr();
            return instance;
        }

        public void addObject(Object obj)
        {
            int index = getObjectIndex(obj.Guid);
            if (index != -1)
            {
                updateObject(obj);
            }
            else
            {
                mObjects.Add(obj);
            }
        }

        public void updateObject(Object obj)
        {
            int index = getObjectIndex(obj.Guid);
            if (index != -1)
            {
                mObjects[index] = obj;
            }
            else
            {
                addObject(obj);
            }
        }

        public void delObject(WoWGuid guid)
        {
            int index = getObjectIndex(guid);
            if (index != -1)
            {
                mObjects.RemoveAt(index);
            }
        }

        public Object getObject(string name)
        {
            int index = getObjectIndex(name);
            if (index == -1)
            {
                return null;
            }
            else
                return mObjects[index];
        }

        public Object getObject(WoWGuid guid)
        {
            int index = getObjectIndex(guid);
            if (index == -1)
            {
                return null;
            }
            else
                return mObjects[index];

        }

        public Object getNearestObject(Object obj)
        {
            Object[] list = getObjectArray();
            Object closest = null;
            float dist;
            float mindist = 9999999999;

            if (list.Length < 1)
            {
                return null;
            }

            foreach (Object obj2 in list)
            {
                dist = TerrainMgr.CalculateDistance(obj.Position, obj2.Position);
                if (dist < mindist)
                {
                    mindist = dist;
                    closest = obj2;
                }
            }

            return closest;
        }

        public ObjectType getObjectType(WoWGuid guid)
        {
            int index = getObjectIndex(guid);
            if (index != -1)
            {
                return mObjects[index].Type;
            }
            else
                return new ObjectType();
        }

        public bool objectExists(WoWGuid guid)
        {

            int index = getObjectIndex(guid);
            if (index == -1)
            {
                return false;
            }
            else
                return true;
        }

        private int getObjectIndex(WoWGuid guid)
        {
            int index = mObjects.FindIndex(s => s.Guid.GetOldGuid() == guid.GetOldGuid());
            return index;
        }

        private int getObjectIndex(string name)
        {
            int index = mObjects.FindIndex(s => s.Name == name);
            return index;
        }

        public Object[] getObjectArray()
        {
            return mObjects.ToArray();
        }
    }
}