using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using ru.antonriot.utils;

namespace ru.antonriot.change_dispathcher
{
    public class ChangeDispatcher<T> where T : ChangeObject, new()
    {
        private static EasyPool<T> pool = new EasyPool<T>();

        private Dictionary<ChangeType, List<Action<T>>> callbacksByChangeType = new Dictionary<ChangeType, List<Action<T>>>();


        public static T GetChange()
        {
            return pool.getInstance();
        }


        public void addChangeListener(ChangeType type, Action<T> callback)
        {
            if (callbacksByChangeType.ContainsKey(type))
            {
                callbacksByChangeType[type].Add(callback);
            }
            else
            {
                callbacksByChangeType[type] = new List<Action<T>>() { callback };
            }
        }

        public void removeChangeListener(ChangeType type, Action<T> callback)
        {
            if (callbacksByChangeType.ContainsKey(type))
            {
                callbacksByChangeType[type].Remove(callback);
            }
        }

        public void removeAllListeners()
        {
            callbacksByChangeType.Clear();
        }

        public void removeListeners(ChangeType type)
        {
            if (callbacksByChangeType.ContainsKey(type))
            {
                callbacksByChangeType[type].Clear();
            }
        }

        public void removeRedispatch(ChangeDispatcher<T> target, ChangeType type)
        {
            if (target != this)
            {
                target.removeChangeListener(type, onChange);
            }
        }

        public void addRedispatch(ChangeDispatcher<T> target, ChangeType type)
        {
            if (target != this)
            {
                target.addChangeListener(type, onChange);
            }
        }

        public void dispatchChange(T change, ChangeType type)
        {
            change.type = type;

            int lastDispatchCount = change.dispatchCount;
            change.dispatchCount += 1;

            if (callbacksByChangeType.ContainsKey(type))
            {
                List<Action<T>> list = callbacksByChangeType[type];
                int i;
                for (i = 0; i < list.Count; i += 1)
                {
                    list[i](change);
                    if (change.isPropagandationStopped)
                    {
                        break;
                    }
                }
            }

            if (lastDispatchCount == 0) //защита на случай если в процессе дёргания слушателей это же событие отдиспатчат ещё раз.
            {
                pool.release(change);
            }
        }
        //===================

        private void onChange(T change)
        {
            dispatchChange(change, change.type);
        }
        //

        //
    }

    public class ChangeType : StringEnum
    {
        public ChangeType(string val) : base(val)
        {
        }
    }
}