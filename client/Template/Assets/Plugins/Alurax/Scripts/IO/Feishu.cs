using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Alurax
{
    public class Feishu
    {
        private const string REQUEST_TOKEN_URL =
            "https://open.feishu.cn/open-apis/auth/v3/tenant_access_token/internal";
        private const string REQUEST_UPLOADFILE_URL = "https://open.feishu.cn/open-apis/im/v1/files";
        private const string REQUEST_SENDCHAT_URL =
            "https://open.feishu.cn/open-apis/im/v1/messages?receive_id_type=chat_id";
        
        /// <summary>
        /// 飞书聊天群发文件
        /// </summary>
        /// <param name="app_id">飞书平台id</param>
        /// <param name="app_secret">飞书平台secret</param>
        /// <param name="groupid">聊天窗口id</param>
        /// <param name="path">文件路径</param>
        /// <param name="success">成功</param>
        /// <param name="error">失败</param>
        public static void Send(string app_id, string app_secret, string groupid, bool isEditor,string filePath, Action success,
            Action<string> error)
        {
            Alurax.Inst.StartCoroutine(UploadFile(app_id, app_secret, groupid,  isEditor, filePath, success, error));
        }
        
        static IEnumerator UploadFile(string app_id, string app_secret,string groupid,bool isEditor, string filePath,Action success, Action<string>error)
        {
            string tenant_access_token=null;
            string file_key = null;
            string json = $"{{\"app_id\":\"{app_id}\",\"app_secret\":\"{app_secret}\"}}";
            bool isFinish = false;
            using (UnityWebRequest www = UnityWebRequest.Post(REQUEST_TOKEN_URL, json, ""))
            {
                www.SetRequestHeader("Content-Type", "application/json; charset=utf-8");
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    error?.Invoke(www.error);
                }
                else
                {
                    var token = JsonUtility.FromJson<Token>(www.downloadHandler.text);
                    if (token.code == 0)
                    {
                        tenant_access_token = token.tenant_access_token;
                    }
                    else
                    {
                        error?.Invoke(www.downloadHandler.text);
                    }
                }
            }

            if (!string.IsNullOrEmpty(tenant_access_token))
            {
                WWWForm from = new WWWForm();
                from.AddField("file_type", "stream");
                from.AddField("file_name", Path.GetFileName(filePath)+".zip");
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var streamReader = new StreamReader(fileStream);
                string txt = streamReader.ReadToEnd();
                using (var outStream = new MemoryStream())
                {
                    using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
                    {
                        var fileInArchive = archive.CreateEntry(Path.GetFileName(filePath));
                        using (var entryStream = fileInArchive.Open())
                        using (var fileToCompressStream = new MemoryStream(Encoding.UTF8.GetBytes(txt)))
                        {
                            fileToCompressStream.CopyTo(entryStream);
                        }
                    }
                    from.AddBinaryData("file", outStream.ToArray());
                }

                using (UnityWebRequest www = UnityWebRequest.Post(REQUEST_UPLOADFILE_URL, from))
                {
                    www.SetRequestHeader("Authorization", $"Bearer {tenant_access_token}");
                    yield return www.SendWebRequest();


                    if (www.result != UnityWebRequest.Result.Success)
                    {
                        error?.Invoke(www.error);
                    }
                    else
                    {
                        var file = JsonUtility.FromJson<FileKey>(www.downloadHandler.text);
                        if (file.code == 0)
                        {
                            Debug.Log(www.downloadHandler.text);
                            file_key = file.data.file_key;
                        }
                        else
                        {
                            error?.Invoke(www.downloadHandler.text);
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(file_key))
            {
                json = $"{{\"receive_id\": \"{groupid}\",\"content\": \"{{\\\"file_key\\\":\\\"{file_key}\\\"}}\",\"msg_type\": \"file\"}}";

                using (UnityWebRequest www = UnityWebRequest.Post(REQUEST_SENDCHAT_URL, json,""))
                {
                   www.SetRequestHeader("Authorization", $"Bearer {tenant_access_token}");
                    www.SetRequestHeader("Content-Type", "application/json; charset=utf-8");
                    yield return www.SendWebRequest();


                    if (www.result != UnityWebRequest.Result.Success)
                    {
                        error?.Invoke(www.error);
                    }
                    else
                    {
                      var post = JsonUtility.FromJson<Post>(www.downloadHandler.text);
                      if(post.code == 0)
                      {
                            isFinish = true;
                          
                      }else
                            error?.Invoke(www.downloadHandler.text);
                    }
                }
            }

            if (isFinish)
            {
                success?.Invoke();
            }
            else
            {
                error?.Invoke("未知");
            }
        }

        [System.Serializable]
        class Token
        {
            //{"code":0,"expire":4613,"msg":"ok","tenant_access_token":"t-g10423kyZI6C6LDRDQMORC52C7C3D4M7BM2VG4JX"}
            public int code;
            public string tenant_access_token;
        }

        [System.Serializable]
        class Data
        {
            public string file_key;
        }
        [System.Serializable]
        class FileKey
        {
            //{"code":0,"data":{"file_key":"file_v3_007n_9038af1d-45e1-41ea-8487-99f84cd71ffg"},"msg":"success"}

            public int code;
            public Data data;
        }
        [System.Serializable]
        class Post
        {
            //{"code":0,"data":{"body":{"content":"{\"file_key\":\"file_v3_007n_63e934d5-5cf2-4588-a82a-fae21ce7540g\",\"file_name\":\"Bump_2024_02_02_client1.log\"}"},"chat_id":"oc_909bc5dc926e83f4c47b29790ce79b5c","create_time":"1706969549514","deleted":false,"message_id":"om_8e93d4c962e65d8ab2c640e14ed1ab73","msg_type":"file","sender":{"id":"cli_a24132d8a1bf900d","id_type":"app_id","sender_type":"app","tenant_key":"126edc8d994e9740"},"update_time":"1706969549514","updated":false},"msg":"success"}
            public int code;
        }
    }
}
