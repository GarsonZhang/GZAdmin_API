using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using vueAdminAPI.Common;

namespace vueAdminAPI.Models
{
    public class UserInfo
    {
        public string Token { get; set; }

        //解决修改密钥后并发的请求也返回签名错误问题

        long cacheID { get; set; }
        string cacheSecretKey { get; set; }

        internal long newID { get; set; }
        internal string newSecretKey { get; set; }

        public string getSecretKey(long ID)
        {
            if (ID == newID)
            {
                cacheID = 0;
                cacheSecretKey = "";
                return newSecretKey;
            }
            else if (ID == cacheID)
                return cacheSecretKey;
            else
                return newSecretKey;
        }

        public string currentSecretKey
        {
            get
            {
                return newSecretKey;
            }
        }

        public void updateTokenSecret(long ID, string key)
        {
            cacheID = newID;
            cacheSecretKey = newSecretKey;
            newID = ID;
            newSecretKey = key;
        }


        public string Client { get; set; }
        public string IPAdrress { get; set; }

        public DateTime SecretTime { get; set; }

        public bool isSecreKeyUpdate { get; set; }

        public DateTime? LastTime { get; set; }

        public string UserID { get; set; }
        public string Account { get; set; }
        public string UserName { get; set; }

        public int Status { get; set; }
    }

    public class UserHandle
    {
        Dictionary<string, UserInfo> dictionary;
        public UserHandle()
        {
            dictionary = new Dictionary<string, UserInfo>();
            syncToken();
        }
        //从数据库同步Token信息
        void syncToken()
        {
            using (DbContextSystem dbContext = new DbContextSystem())
            {
                var time = DateTime.Now.AddHours(-1);
                var users = dbContext.base_token.Where(p => p.updateTime > time && p.status == 0)
                     .Join(dbContext.base_user, token => token.userID, user => user.rowID, (token, user) =>
                            new UserInfo()
                            {
                                UserID = user.rowID,
                                Account = user.account,
                                UserName = user.userName,
                                Status = user.status,
                                Token = token.token,
                                newSecretKey = token.secretKey,
                                newID = token.secretID,
                                IPAdrress = token.IPAddress,
                                Client = token.Client,
                                LastTime = token.updateTime,
                                SecretTime = DateTime.Now,
                            }
                 );

                dictionary.Clear();
                foreach (var user in users)
                {
                    dictionary.Add(user.Token, user);
                }
            }
        }

        /// <summary>
        /// 删除用户所有Token信息，用户状态改变的时候使用
        /// </summary>
        /// <param name="userID"></param>
        public void deleteUser(string userID)
        {
            lock (this)
            {

                var data = dictionary.Where(p => p.Value.UserID == userID);

                foreach (var d in data)
                {
                    dictionary.Remove(d.Key);
                }
                //数据库删除token信息
                using (DbContextSystem dbContext = new DbContextSystem())
                {
                    var v = dbContext.base_token.Where(p => p.userID == userID);
                    foreach (var item in v)
                    {
                        item.updateTime = DateTime.Now;
                        item.status = -1;
                        dbContext.Entry(item).State = System.Data.EntityState.Modified;
                    }
                    dbContext.SaveChanges();
                }
            }
        }

        /// <summary>
        /// 删除token，安全退出
        /// </summary>
        /// <param name="token"></param>
        public void deleteToken(string token)
        {
            lock (this)
            {

                var data = dictionary.Where(p => p.Value.Token == token);

                foreach (var d in data)
                {
                    dictionary.Remove(d.Key);
                }
                //数据库删除token信息
                using (DbContextSystem dbContext = new DbContextSystem())
                {
                    var v = dbContext.base_token.Where(p => p.token == token);
                    foreach (var item in v)
                    {
                        item.updateTime = DateTime.Now;
                        item.status = -1;
                        dbContext.Entry(item).State = System.Data.EntityState.Modified;
                    }
                    dbContext.SaveChanges();
                }
            }
        }

        /// <summary>
        /// 更新Token，用户登录时使用
        /// </summary>
        /// <param name="userModel"></param>
        /// <param name="Client"></param>
        /// <param name="IPAddress"></param>
        public UserInfo updateToken(base_user userModel, string Client, string IPAddress)
        {
            lock (this)
            {
                UserInfo user = dictionary.FirstOrDefault(p => p.Value.UserID == userModel.rowID && p.Value.Client == Client).Value;
                using (DbContextSystem db = new DbContextSystem())
                {

                    if (user != null)
                    {
                        var d1 = db.base_token.Where(p => p.token == user.Token).FirstOrDefault();
                        if (d1 != null)
                        {
                            d1.status = -1;
                            d1.updateTime = DateTime.Now;
                            db.Entry(d1).State = System.Data.EntityState.Modified;
                        }
                    }

                    base_token token = new base_token();
                    token.userID = userModel.rowID;
                    token.token = Guid.NewGuid().ToString().Replace("-", "");
                    token.secretKey = Guid.NewGuid().ToString().Replace("-", "");
                    token.secretID = Tools.getTS();
                    token.IPAddress = IPAddress;
                    token.Client = Client;
                    token.status = 0;
                    db.Entry(token).State = System.Data.EntityState.Added;
                    db.SaveChanges();

                    //删除旧的数据
                    if (user != null)
                    {
                        dictionary.Remove(user.Token);
                    }
                    var result = new UserInfo()//同步新的token数据
                    {
                        UserID = userModel.rowID,
                        Account = userModel.account,
                        UserName = userModel.userName,
                        Status = userModel.status,
                        Token = token.token,
                        newSecretKey = token.secretKey,
                        newID = token.secretID,
                        IPAdrress = token.IPAddress,
                        Client = token.Client,
                        LastTime = token.updateTime,
                        SecretTime = DateTime.Now
                    };
                    dictionary.Add(token.token, result);
                    return result;
                }
            }
        }

        internal void updateLastTime(UserInfo user)
        {
            lock (this)
            {
                if (user == null) return;
                user.LastTime = DateTime.Now;
                if (user.SecretTime < DateTime.Now.AddMinutes(-5))//私钥5分钟更新一次
                {
                    user.isSecreKeyUpdate = true;
                    user.updateTokenSecret(Tools.getTS(), Guid.NewGuid().ToString().Replace("-", ""));
                    user.SecretTime = DateTime.Now;
                }
                using (DbContextSystem db = new DbContextSystem())
                {
                    var d1 = db.base_token.Where(p => p.token == user.Token).FirstOrDefault();
                    if (d1 != null)
                    {
                        d1.updateTime = user.LastTime;
                        d1.secretKey = user.newSecretKey;
                        d1.secretID = user.newID;
                    }
                    db.Entry(d1).State = System.Data.EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }
        //根据token，和客户端信息获得用户信息
        public UserInfo getUserInfo(string token)
        {
            lock (this)
            {
                if (dictionary.ContainsKey(token))
                    return dictionary[token];
                return null;
            }
        }
    }

    public static class APIDataCache
    {
        public static UserHandle UserHandle = new UserHandle();
        static APIDataCache()
        {
        }

    }
}