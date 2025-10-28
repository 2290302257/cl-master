using System.Collections.Generic;
using System.Text;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using Alurax;

namespace Alurax
{
    public class UILock
    {
        private static EventSystem s_EventSystem;
        private static InputSystemUIInputModule s_InputModule;
        private static Dictionary<string, int> s_LockDict = new Dictionary<string, int>();

        static UILock()
        {
            s_EventSystem = EventSystem.current;
            s_InputModule = (InputSystemUIInputModule)s_EventSystem.currentInputModule;
        }

        public static void Lock()
        {
            Lock(string.Empty);
        }

        public static void UnLock()
        {
            UnLock(string.Empty);
        }

        public static void UnLockAll()
        {
            //Log.I($"xys UnLockAll");
            s_LockDict.Clear();
          
            s_EventSystem.enabled = true;
            s_InputModule?.actionsAsset?.Enable();
        }

        public static void Lock(string reason)
        {
            //Log.I($"xys Lock:{reason}");
            if (!s_LockDict.TryAdd(reason, 1))
                s_LockDict[reason]++;
            s_EventSystem.enabled = false;
            s_InputModule?.actionsAsset?.Disable();
            TaskManager.Remove(OnTimeOut);
            TaskManager.Timeout(OnTimeOut, 5f);
        }

        public static void UnLock(string reason)
        {
            //Log.I($"UnLock:{reason}");
            if (s_LockDict.ContainsKey(reason))
            {
                s_LockDict[reason]--;
                if (s_LockDict[reason] <= 0)
                    s_LockDict.Remove(reason);
            }

            if (s_LockDict.Count <= 0)
            {
                s_EventSystem.enabled = true;
                s_InputModule?.actionsAsset?.Enable();
            }
        }

        private static void OnTimeOut()
        {   
            //Log.I($"UnLock:OnTimeOut");
            UnLockAll();
        }

        //only debug
        public static void Debug()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"LockKeyCount:{s_LockDict.Count}");
            foreach (var @lock in s_LockDict)
            {
                sb.AppendLine($" reason:{@lock.Key} lockCount:{@lock.Value}");
            }

            Log.I(sb.ToString());
        }
    }
}