using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alurax
{
    public class UIBind : MonoBehaviour
    {
        public List<Component> Bind =new List<Component>();
    }
    public interface IExport{}

    public interface IExportProcess
    {
        void Process();
    }
}