using UnityEngine;
using System.Collections;
using ru.antonriot.utils;

namespace ru.antonriot.change_dispathcher
{
    public class ChangeObject : IClearable
    {
        public ChangeType type;
        internal int dispatchCount = 0;
        public bool isPropagandationStopped;


        public void clear()
        {
            type = null;
            dispatchCount = 0;
            isPropagandationStopped = false;
            executeClear();

        }

        virtual protected void executeClear()
        {
            Debug.LogError("executeClear is not overrided in " + this);

        }


        //

        //
    }
}