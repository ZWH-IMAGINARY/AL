using AL.AL_Service;
using AL.model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace AL.FilesHelper
{
    public class FileHelper
    {
        public static void AppendOrCreateLog()
        {
            //获取文件操作路径
            string url = model.NeedValue.appurl;


            //日志内容
            string log = "";


            //创建文件流
            using (FileStream fs = new FileStream(url, FileMode.Append, FileAccess.Write))
            {
                //创建读写器
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    //写入日志内容
                    sw.Write(log);
                    sw.WriteLine("");

                    //刷新读写器
                    sw.Flush();

                    //刷新流
                    fs.Flush();

                    //关闭读写器
                    sw.Close();

                    //关闭流
                    fs.Close();
                }
            }
        }









        /// <summary>
        /// 添加聊天记录
        /// </summary>
        /// <param name="_df"></param>
        /// <param name="_u"></param>
        public static void AddFMessage(List<DTO_SMess> _df, UserInfo _u)
        {
            Dictionary<string, List<DTO_SMess>> lis = new Dictionary<string, List<DTO_SMess>>();


            //对聊天消息进行归类
            foreach (var item in _df)
            {
                if (item.Despatcher.UI_Number.Equals(_u.UI_Number))
                {
                    if (lis.ContainsKey(item.SendToNum._Number))
                    {
                        lis[item.SendToNum._Number].Add(item);
                    }
                    else
                    {
                        List<DTO_SMess> slis = new List<DTO_SMess>();
                        slis.Add(item);
                        lis.Add(item.SendToNum._Number, slis);
                    }
                }
                else if (item.SendToNum._Number.Equals(_u.UI_Number))
                {
                    if (lis.ContainsKey(item.Despatcher.UI_Number))
                    {
                        lis[item.Despatcher.UI_Number].Add(item);
                    }
                    else
                    {
                        List<DTO_SMess> slis = new List<DTO_SMess>();
                        slis.Add(item);
                        lis.Add(item.Despatcher.UI_Number, slis);
                    }
                }
            }

            //添加聊天文件
            AddFMessages(lis, _u.UI_Number);
        }







        /// <summary>
        /// 添加聊天记录
        /// </summary>
        /// <param name="_lis"></param>
        private static void AddFMessages(Dictionary<string, List<DTO_SMess>> _lis, string _number)
        {
            //获取项目路径
            string url = model.NeedValue.appurl + "FMessages/" + _number + "/";


            IsExistsUrl(url);

            foreach (var item in _lis)
            {
                //创建流
                using (FileStream fs = new FileStream(url + item.Key + ".txt", FileMode.CreateNew, FileAccess.Write))
                {
                    string s = JsonConvert.SerializeObject(item.Value);

                    //创建序列化对象
                    BinaryFormatter bf = new BinaryFormatter();

                    //序列化对象
                    bf.Serialize(fs, s);

                    //刷新缓冲区
                    fs.Flush();

                    //关闭流
                    fs.Close();
                }
            }
        }


        /// <summary>
        /// 获取存在历史聊天记录的用户number
        /// </summary>
        /// <param name="_number"></param>
        public static List<string> GetMessageFileName(string _number)
        {
            //获取项目路径
            string url = model.NeedValue.appurl + "FMessages/" + _number + "/";

            //验证路径是否存在
            IsExistsUrl(url);

            DirectoryInfo di = new DirectoryInfo(url);

            List<string> usernumber = new List<string>();

            foreach (var item in di.GetFiles())
            {
                if (item.Extension.ToLower().Equals(".txt"))
                {
                    usernumber.Add(item.Name.Substring(0, item.Name.LastIndexOf(".txt")));
                }
            }


            return usernumber;
        }



        /// <summary>
        /// 提取本地聊天记录
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, List<DTO_SMess>> ReadFMessages(string _number)
        {

            Dictionary<string, List<DTO_SMess>> dic = new Dictionary<string, List<DTO_SMess>>();

            //获取项目路径
            string url = model.NeedValue.appurl + "FMessages/" + _number + "/";

            IsExistsUrl(url);

            DirectoryInfo di = new DirectoryInfo(url);

            foreach (var item in di.GetFiles())
            {
                if (item.Extension.ToLower().Equals(".txt"))
                {
                    //创建流
                    using (FileStream fs = new FileStream(item.FullName, FileMode.Open, FileAccess.Read))
                    {
                        //创建二进制反序列化对象
                        BinaryFormatter bf = new BinaryFormatter();

                        //获取聊天消息
                        string s = bf.Deserialize(fs).ToString();

                        List<DTO_SMess> ds = JsonConvert.DeserializeObject<List<DTO_SMess>>(s);

                        //刷新缓冲区
                        fs.Flush();


                        //关闭流
                        fs.Close();

                        dic.Add(item.Name.Substring(0, item.Name.LastIndexOf(".txt")), ds);
                    }
                }


            }


            return dic;
        }






        /// <summary>
        /// 验证路径是否存在
        /// </summary>
        /// <param name="url"></param>
        private static void IsExistsUrl(string url)
        {
            if (!Directory.Exists(url))
                Directory.CreateDirectory(url);
        }
    }
}
