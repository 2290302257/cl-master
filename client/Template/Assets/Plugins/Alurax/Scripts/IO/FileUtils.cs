using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Alurax
{
    public class FileUtils 
    {
        public static string GetMD5Hash(string path)
        {
            if (!File.Exists(path))
                return string.Empty;

            return GetMD5Hash(File.ReadAllBytes(path));
        }

        private static MD5 s_MD5 = new MD5CryptoServiceProvider();
        public static string GetMD5Hash(byte[] buffer)
        {
            if (buffer == null)
                return string.Empty;
            return BitConverter.ToString(s_MD5.ComputeHash(buffer)).Replace("-", "").ToLower();
        }

        public static long GetFileSize(string file)
        {
            if(File.Exists(file))
                return new FileInfo(file).Length;
            return 0;
        }
        
        static string[] FormatSize = { "B", "KB", "MB", "GB", "TB" };
        public static string GetFileFormatSize(long bytes)
        {
            int order = 0;
            double dou = bytes;
            while (dou >= 1024f && order < FormatSize.Length - 1)
            {
                order++;
                dou/= 1024f;
            }
            return String.Format("{0:0.##} {1}", dou, FormatSize[order]);
        }

#region 读取
        
        public static string GetStreamingAssetsPath()
        {
#if UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_IOS
            return $"file://{Application.streamingAssetsPath}";
#else
            return $"{Application.streamingAssetsPath}";    
#endif
        }
        
        public static string GetDownloadPath()
        {
            return Download.DOWNLOAD_PATH;    
        }
        
        public static string LoadStreamingText(string name)
        {
            var bundleMapTxt = $"{GetStreamingAssetsPath()}/{name}";
            return InternalLoad(bundleMapTxt).text;
        }
        public static byte[] LoadStreamingByte(string name)
        {
            var bundleMapTxt = $"{GetStreamingAssetsPath()}/{name}";
            return InternalLoad(bundleMapTxt).data;
        }
        public static void LoadStreamingTextSync(string name,Action<string> callback)
        {
            var bundleMapTxt = $"{GetStreamingAssetsPath()}/{name}";

            Alurax.Inst.StartCoroutine(InternalLoadSync(bundleMapTxt, (h) =>
            {
                callback?.Invoke(h.text);
            }));
        }
        public static void LoadStreamingByteSync(string name,Action<byte[]> callback)
        {
            var bundleMapTxt = $"{GetStreamingAssetsPath()}/{name}";
            Alurax.Inst.StartCoroutine(InternalLoadSync(bundleMapTxt, (h) =>
            {
                callback?.Invoke(h.data);
            }));
        }
        
        public static byte[] LoadDownloadByte(string name)
        {
            return File.ReadAllBytes($"{GetDownloadPath()}/{name}");
        }
        
        static DownloadHandler InternalLoad(string path)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(path);
            webRequest.SendWebRequest();
            while (!webRequest.isDone)
            {
                if (webRequest.isNetworkError || webRequest.isHttpError)
                    break;
            }
            return webRequest.downloadHandler;
        }
        
        static IEnumerator InternalLoadSync(string path,Action<DownloadHandler> handler)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(path);
            yield return webRequest.SendWebRequest();
            handler?.Invoke(webRequest.downloadHandler);
        }
        #endregion    
#region 压缩
        public static byte[] Compress(byte[] bytes)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
                {
                    gzipStream.Write(bytes, 0, bytes.Length);
                }
                return memoryStream.ToArray();
            }
        }
        public static string Compress(string str)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(str);
            var compressBytes = Compress(bytes);
            return System.Convert.ToBase64String(compressBytes);
        }
        public static byte[] Decompress(byte[] bytes)
        {
            using (var memoryStream = new MemoryStream(bytes))
            {
                using (var outputStream = new MemoryStream())
                {
                    using (var decompressStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                    {
                        decompressStream.CopyTo(outputStream);
                    }
                    return outputStream.ToArray();
                }
            }
        }
        
        public static string Decompress(string str)
        {
            var bytes = System.Convert.FromBase64String(str);
            var decompressBytes = Decompress(bytes);
            return System.Text.Encoding.UTF8.GetString(decompressBytes);
        }
        public static void ZipFiles(string outputZipFilePath, params string[] inputFilePaths)
        {
            using (var fileStream = new FileStream(outputZipFilePath, FileMode.Create))
            {
                using (var archive = new ZipArchive(fileStream, ZipArchiveMode.Create, true))
                {
                    foreach (var filePath in inputFilePaths)
                    {
                        archive.CreateEntryFromFile(filePath, Path.GetFileName(filePath));
                    }
                }
            }
        }
        
        public static byte[] ZipFileInMemory(string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var streamReader = new StreamReader(fileStream))
            {
                var txt = streamReader.ReadToEnd();
                using (var outStream = new MemoryStream())
                {
                    using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
                    {
                        var fileInArchive = archive.CreateEntry(Path.GetFileName(path));
                        using (var entryStream = fileInArchive.Open())
                        using (var fileToCompressStream = new MemoryStream(Encoding.UTF8.GetBytes(txt)))
                        {
                            fileToCompressStream.CopyTo(entryStream);
                        }
                    }
                    return outStream.ToArray();
                }
            }
        }
#endregion 
        

        
    }
}
