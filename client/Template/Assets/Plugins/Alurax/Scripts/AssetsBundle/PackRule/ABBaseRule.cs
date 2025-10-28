using System.Collections.Generic;
using UnityEngine;
using Alurax;

public class ABBaseRule : ScriptableObject
{
    public virtual List<PackData> Pack(string packName,ABPack pack,List<Data> files)
    {
        return new List<PackData>();
    }

    public class Data
    {
        public string name;
        public string alias;
    }

    public class PackData
    {
        public string assetBundleName { get; private set; }
        public string[] assetNames{ get; private set; }
        public string[] aliasNames{ get; private set; }
        public PackData(string assetBundleName, string[] assetNames, string[] aliasNames)
        {
            this.assetBundleName = assetBundleName.ToLower();
            this.assetNames = assetNames;
            this.aliasNames = aliasNames;
        }
    }
}
