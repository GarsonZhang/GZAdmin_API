using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace vueAdminAPI.Common
{
    public class MD5
    {
        public static string MD5Encrypt32(string sDataIn)
        {
            var b = Encoding.UTF8.GetBytes(sDataIn);
            var _md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] output = _md5.ComputeHash(b);
            string md5Str = BitConverter.ToString(output).Replace("-", "");
            return md5Str;

            //string pwd = "";
            //System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create(); //实例化一个md5对像
            //                        // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            //byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(sDataIn));
            //// 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            //for (int i = 0; i < s.Length; i++)
            //{
            //    // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
            //    pwd = pwd + s[i].ToString("X");
            //}
            //return pwd;
        }
    }
}