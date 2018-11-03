using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace ru.antonriot.utils
{
    public class EasyPool<T> where T : new()
    {
        private static Dictionary<Type, bool> usedPools = new Dictionary<Type, bool>();


        private Stack<T> objects = new Stack<T>();
        private int active = 0;


        public T getInstance()
        {
            active += 1;
            T instance;
            int count = objects.Count;
            if (count > 0)
            {
                instance = objects.Pop();
            }
            else
            {
                instance = new T();
            }
            return instance;
        }

        public void release(T instance)
        {
            if (instance != null)
            {
                if (objects.Contains(instance))
                {
                    Debug.LogError("releasing object is already released " + instance);
                }
                else
                {
                    active -= 1;
                    objects.Push(instance);
                    if (instance is IClearable)
                    {
                        ((IClearable)instance).clear();
                    }
                }
            }
        }

        //

        //
    }
}