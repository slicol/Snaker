////////////////////////////////////////////////////////////////////
//                            _ooOoo_                             //
//                           o8888888o                            //
//                           88" . "88                            //
//                           (| ^_^ |)                            //
//                           O\  =  /O                            //
//                        ____/`---'\____                         //
//                      .'  \\|     |//  `.                       //
//                     /  \\|||  :  |||//  \                      //
//                    /  _||||| -:- |||||-  \                     //
//                    |   | \\\  -  /// |   |                     //
//                    | \_|  ''\---/''  |   |                     //
//                    \  .-\__  `-`  ___/-. /                     //
//                  ___`. .'  /--.--\  `. . ___                   //
//                ."" '<  `.___\_<|>_/___.'  >'"".                //
//              | | :  `- \`.;`\ _ /`;.`/ - ` : | |               //
//              \  \ `-.   \_ __\ /__ _/   .-` /  /               //
//        ========`-.____`-.___\_____/___.-`____.-'========       //
//                             `=---='                            //
//        ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^      //
//            佛祖保佑       无BUG        不修改                   //
////////////////////////////////////////////////////////////////////
/*
 * 描述：
 * 作者：slicol
*/
using System;
using System.IO;
using System.Text;


namespace SGF
{
    public class FileUtils
    {
        private const string LOG_TAG = "FileUtils";

        public static byte[] ReadFile(string fullpath)
        {
            byte[] buffer = null;
            if (File.Exists(fullpath))
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(fullpath, FileMode.Open, FileAccess.Read);
                    buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                }
                catch (Exception e)
                {
                    Debuger.LogError(LOG_TAG, "ReadFile() Path:{0}, Error:{1}", fullpath, e.Message);
                }
                finally
                {
                    if (fs != null)
                    {
                        fs.Close();
                    }
                }
            }
            else
            {
                Debuger.LogError(LOG_TAG, "ReadFile() File is Not Exist: {0}", fullpath);
            }
            return buffer;
        }

        public static string ReadString(string fullpath)
        {
            byte[] buffer = ReadFile(fullpath);
            if (buffer != null)
            {
                return Encoding.UTF8.GetString(buffer);
            }
            return "";
        }

        public static string ReadStringASCII(string fullpath)
        {
            byte[] buffer = ReadFile(fullpath);
            if (buffer != null)
            {
                return Encoding.ASCII.GetString(buffer);
            }
            return "";
        }

        public static int SaveFile(string fullpath, byte[] content)
        {
            if (content == null)
            {
                content = new byte[0];
            }

            string dir = PathUtils.GetParentDir(fullpath);

            if (!Directory.Exists(dir))
            {
                try
                {
                    Directory.CreateDirectory(dir);
                }
                catch (Exception e)
                {
                    Debuger.LogError(LOG_TAG, "SaveFile() CreateDirectory Error! Dir:{0}, Error:{1}", dir, e.Message);
                    return -1;
                }

            }

            FileStream fs = null;
            try
            {
                fs = new FileStream(fullpath, FileMode.Create, FileAccess.Write);
                fs.Write(content, 0, content.Length);
            }
            catch (Exception e)
            {
                Debuger.LogError(LOG_TAG, "SaveFile() Path:{0}, Error:{1}", fullpath, e.Message);
                fs.Close();
                return -1;
            }

            fs.Close();
            return content.Length;
        }

        public static int SaveFile(string fullpath, string content)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(content);
            return SaveFile(fullpath, buffer);
        }


    }
}

