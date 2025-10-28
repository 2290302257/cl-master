using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Alurax
{
    public class Download
    {
        public static string DOWNLOAD_PATH = $"{Application.persistentDataPath}/Download";
        public ResultData Result { get; private set; }
        public Action OnFinish;
        public Action<ErrorData> OnError;
        public Action<ProgressData> OnProgress;

        #region 下载内容
        public class ResultData
        {
            public ResultCode code;
            public long httpCode;
            public List<ResultFile> files = new List<ResultFile>();
            public long size; //需要下载的文件大小
            public int buildId;
        }
        public enum ResultCode
        {
            OK = 0, //成功
            ERROR = 1, //访问下载文件失败
            NO_EXIST = 2, //访问下载文件不存在
        }
        
        public class ResultFile
        {
            public string path;
            public string name;
            public string md5;
            public string ext;
        }
        #endregion

        #region 下载进度
        public struct ProgressData
        {
            public long downloadSize; //已下载的文件大小
            public long downloadSizeAll;//总需要下载的文件大小
            public long downloadSpeed;//每秒下载速度
        }
        #endregion

        #region 下载错误
        public enum DownloadCode
        {
            NET_ERROR = 0, //下载的文件出错误，需要重新下载
            FILE_ERROR, //文件损坏，需要重新下载
        }
        public struct ErrorData
        {
            public DownloadCode code;
            public string file; //错误文件
        }

        #endregion
        
        private int DOWNLOAD_ERROR_COUNT = 3;
        private long DOWNLOAD_MAX_SIZE = 1024 * 1024 * 5;
        private long DOWNLOAD_MAX_COUNT = 10;
        private long _downloadSizeAll = 0;
        private long _currentdownloadSize = 0;
        private string _headUrl;
        private string _bundlemap = string.Empty;
        private float _lastSpeedCheckTime = 0f;
        private long _lastSpeedCheckSize = 0;
        private long _lastSpeedPerSecond = 0;
        private List<DownloadData> _downloadUrls = new List<DownloadData>();
        private class DownloadData
        {
            public string url;
            public string name;
            public string md5;
            public string ext;
            public int fileSize;
            public string savePath;
            public UnityWebRequest request;
            public int errorCount;
        }

        public Download()
        {
            if(!Directory.Exists(DOWNLOAD_PATH))
                Directory.CreateDirectory(DOWNLOAD_PATH);
        }
        
        public void GetDownloadData(string bundleMapUrl, string bundleHeadUrl,Action callback)
        {
            _headUrl = bundleHeadUrl;
            Alurax.Inst.StartCoroutine(GetBundleMap(bundleMapUrl, (request) =>
            {
                Result = new ResultData();
                if (request.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        _bundlemap = request.downloadHandler.text;
                        var map = new BundleMap();
                        map.Load(_bundlemap);
                        Result.buildId= map.Content.Build;
                        SetFormatDownloadDir(map.Lines);
                        foreach (var url in _downloadUrls)
                        {
                            Result.size += url.fileSize;
                        }
                        _downloadSizeAll = Result.size;
                        foreach (var line in map.Lines)
                        {
                            var lineData = line.Value;
                            Result.files.Add(new ResultFile()
                            {
                                path = Path.Combine(DOWNLOAD_PATH,$"{lineData.md5}.{lineData.ext}"),
                                md5 = lineData.md5,
                                name = lineData.name,
                                ext = lineData.ext,
                            });
                        }
                        Result.code = ResultCode.OK;
                    }
                    catch (Exception e)
                    {
                        Log.E(e);
                        Result.code = ResultCode.ERROR;
                    }
                }
                else if (request.result != UnityWebRequest.Result.ConnectionError &&
                    request.responseCode != 200)
                {
                    Result.code = ResultCode.NO_EXIST;
                }
                else
                {
                    Result.code = ResultCode.ERROR;   
                }
                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                    Log.E($"[Download] GetDownloadData:{request.error}");
                callback?.Invoke();
            }));
        }

        public void StartDownload()
        {
            _currentdownloadSize = 0;
            _lastSpeedCheckTime = Time.time;
            TaskManager.Remove(Update);
            TaskManager.Update(Update);
        }

        public void StopDownload()
        {
            TaskManager.Remove(Update);
            foreach (var download in _downloadUrls)
            {
                download.request?.downloadHandler?.Dispose();
                download.request?.Dispose();
                download.request = null;
            }
            _downloadUrls.Clear();
        }
        
        void Update()
        {
            int tryDownloadCount = 0;
            int tryDownloadSize = 0;
            int realdownloadSize = 0;
            for (int i = 0; i < _downloadUrls.Count; i++)
            {
                var data = _downloadUrls[i];
                if (tryDownloadSize < DOWNLOAD_MAX_SIZE &&
                    tryDownloadCount < DOWNLOAD_MAX_COUNT)
                {
                    if (data.request == null)
                    {
                        data.request = UnityWebRequest.Get(data.url);
                        data.request.downloadHandler = new DownloadHandlerFile(data.savePath);
                        data.request.SendWebRequest();
                    }
                    if (data.request.isDone)
                    {
                        bool downloadVerify = data.request.result == UnityWebRequest.Result.Success;
                        bool fileVerify = File.Exists(data.savePath) && FileUtils.GetFileSize(data.savePath) == data.fileSize;
                        data.request.downloadHandler.Dispose();
                        data.request.Dispose();
                        data.request = null;
                        if (downloadVerify && fileVerify)
                        {
                            //file ok
                            tryDownloadSize = Mathf.Max(0, tryDownloadSize - data.fileSize);
                            _currentdownloadSize += data.fileSize;
                            _downloadUrls.RemoveAt(i);
                            i--;
                        }
                        else
                        {
                            //file error
                            if (File.Exists(data.savePath))
                                File.Delete(data.savePath);

                            data.errorCount++;
                            if (data.errorCount >= DOWNLOAD_ERROR_COUNT)
                            {
                                TaskManager.Remove(Update);
                                ErrorData errorData = default(ErrorData);
                                if (!downloadVerify)
                                {
                                    errorData.code = DownloadCode.NET_ERROR;
                                    errorData.file = data.url;

                                }
                                else if (!fileVerify)
                                {
                                    errorData.code = DownloadCode.FILE_ERROR;
                                    errorData.file = data.savePath;
                                }
                                OnError?.Invoke(errorData);
                                return;
                            }
                        }
                    }
                    else
                    {
                        tryDownloadCount++;
                        tryDownloadSize += data.fileSize;
                        realdownloadSize+=(int)data.request.downloadedBytes;
                    }
                }
                else
                    break;
            }
            var currentSize = realdownloadSize + _currentdownloadSize;
            if (_downloadSizeAll > 0)
            {
                if (Time.time - _lastSpeedCheckTime >= 1f)
                {
                    _lastSpeedPerSecond =Math.Max(1024,(currentSize - _lastSpeedCheckSize));
                    _lastSpeedCheckSize = currentSize;
                    _lastSpeedCheckTime = Time.time;
                }
                OnProgress?.Invoke(new ProgressData()
                {
                    downloadSpeed = _lastSpeedPerSecond,
                    downloadSize = currentSize,
                    downloadSizeAll = _downloadSizeAll
                });
            }
            if (_downloadUrls.Count == 0)
            {
                TaskManager.Remove(Update);
                Assets.UpdateBundleMap(_bundlemap);
                OnFinish?.Invoke();
            }
        }
        
        IEnumerator GetBundleMap(string url, Action<UnityWebRequest> callback)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.timeout = 3;
                yield return request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                    Log.E($"[Download] GetBundleMap:{request.error}");
                callback?.Invoke(request);
            }
        }

        void SetFormatDownloadDir(Dictionary<string,BundleMap.LineData> lines)
        {
            var dirs = Directory.GetDirectories(DOWNLOAD_PATH);
            foreach (var dir in dirs)
                Directory.Delete(dir,true);
            var reserved = new HashSet<string>();
            var files = Directory.GetFiles(DOWNLOAD_PATH);
            foreach (var file in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var fileSize = FileUtils.GetFileSize(file);
                if (!lines.TryGetValue(fileName,out var line)||
                    fileSize != line.size)
                    File.Delete(file); 
                else
                    reserved.Add(fileName);
            }

            _downloadUrls = new List<DownloadData>();
            foreach (var line in lines)
            {
                var lineData = line.Value;
                if (!reserved.Contains(lineData.md5))
                {
                    _downloadUrls.Add(new DownloadData()
                    {
                        url = Path.Combine(_headUrl,$"{lineData.md5}.{lineData.ext}"),
                        savePath = Path.Combine(DOWNLOAD_PATH,$"{lineData.md5}.{lineData.ext}"),
                        md5 = lineData.md5,
                        name = lineData.name,
                        ext = lineData.ext,
                        fileSize = lineData.size,
                    });
                }
            }
        }
    }
}
