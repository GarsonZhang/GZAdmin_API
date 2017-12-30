using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vueAdminAPI.Models
{
    public enum EnumResponseCode
    {
        /// <summary>
        /// 请求失败，自定义错误信息
        /// </summary>
        err = -1,
        /// <summary>
        /// 模型验证失败
        /// </summary>
        errModelState = -2,
        /// <summary>
        /// 保存失败
        /// </summary>
        errValidation = -3,
        /// <summary>
        /// 成功
        /// </summary>
        success = 0,

        /// <summary>
        /// 请求参数错误
        /// </summary>
        errParm = 1,
        /// <summary>
        /// 异常信息
        /// </summary>
        errException = 2,
        /// <summary>
        /// 签名错误
        /// </summary>
        errSign = 3,
        /// <summary>
        /// 用户名或密码错误
        /// </summary>
        errPwd = 4,
        /// <summary>
        /// 缺少必要信息 client
        /// </summary>
        errClient = 5,
        /// <summary>
        /// 没有权限
        /// </summary>
        Unauthorized = 6,
        /// <summary>
        /// Token错误
        /// </summary>
        tokenErr =7,
        /// <summary>
        /// token过期
        /// </summary>
        tokenExpired =8,
    }
}