using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
//using UnityEngine.Build.Pipeline;
using UnityEngine.Serialization;

namespace Alurax
{
    public class BundleMap
    {
        public const string kSeparator = "#####\n";
        public Dictionary<string, (string,string[])> Map = null;
        public Dictionary<string,LineData> Lines = null;
        public ContentData Content;
        public void Load(string bundlemap)
        {
            var split = bundlemap.Split(kSeparator);
            var lines = split[0].Split("\n");
            Map = new Dictionary<string, (string,string[])>();
            Lines = new Dictionary<string, LineData>();
            Content = JsonUtility.FromJson<ContentData>(split[1]);
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (!string.IsNullOrEmpty(line))
                {
                    LineData lineData = new LineData(line);
                    Lines[lineData.md5]=new LineData(line);
                }
            }

            foreach (var reference in Content.References)
            {
                foreach (var assetName in reference.AssetNames)
                {
                    Map[assetName] = (reference.ABName, reference.DependAName);
                }
            }
        }

        public class LineData
        {
            private string line;
            public string name;
            public string md5;
            public string ext;
            public int size;
            public bool IsRaw;
            public string[] depend;
            public LineData(string line)
            {
                this.line = line;
                var lineSplit = line.Split(":");
                this.name = lineSplit[0];
                this.md5 = lineSplit[1];
                this.ext= lineSplit[2];
                this.size= int.Parse(lineSplit[3]);
                this.IsRaw = lineSplit[4] == "1";
            }
            public override string ToString()
            {
                return this.line;
            }
        }

        [System.Serializable]
        public class ContentData
        {
            public int Build;
            public int Fixed;
            public int HotUpdate;
            public List<Reference> References;
            
        }
        [System.Serializable]
        public class Reference
        {
            public string ABName;
            public string[] AssetNames;
            public string[] DependAName;
        }
    }
}