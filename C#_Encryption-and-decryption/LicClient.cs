using LicenseCommonUtil;
using Microsoft.Win32;
using System;
using System.IO;
using System.Security.Cryptography;

namespace LicenseClient
{
  public sealed class LicenseClient
  {
    public static bool CheckLicense(string licenseFilePath, out string message)
    {
        const string PublicKey = "<RSAKeyValue><Modulus>r+Fknz/UEsPyB+rOJpRt6BLPUnPvYFEAuXFtL6x23NI7LrUf+Xvcfo1iPMMJruOdNPs+wYNpxNy1166n5mRuSLn7EPbt1aumfCmtgq150NDWWdmx1y5rHNGFu9sUSUm+EgNEgVNbrjTTq8ru7ZJU2X5CLuLMYevuXwBaLZoS8kk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        try
        {
            message = "";

            #region 许可过滤（不存在、不合法、空）
            if (!File.Exists(licenseFilePath))
            {
            message = "许可文件不存在！";
            return false;
            }

            //.lic结尾的文件
            Path.GetExtension(licenseFilePath).Trim().ToUpper();
            if (!Path.GetExtension(licenseFilePath).Trim().ToUpper().Equals(LicenseUtil.DefaultLicenseFileExtension.ToUpper()))
            {
                message = "指定的许可文件不合法！";
                return false;
            }

            string s = LicenseUtil.ReadContentFromFile2(licenseFilePath);

            if (string.IsNullOrEmpty(s))
            {
                message = "许可内容为空！";
                return false;
            }
            #endregion

            #region 对许可解密获取时间信息（fromDate，toDate，lastDate）
            DateTime fromDate;
            DateTime toDate;
            DateTime lastDate;
            LicenseUtil.ReadDateTiFromFile(licenseFilePath, out fromDate, out toDate, out lastDate);
            #endregion

            #region 许可使用的注册表判断
            bool flag1 = false; //注册表信息控制器
            string str1 = "";
            string str2 = "";
            if (lastDate == fromDate) //如果起始时间等于最后时间
            {
                try
                {
                    //检索 SOFTWARE\\PrivateCMW\\TimeLimitKey，是否在当前用户的注册表中，不将写访问权限应用于该项。
                    using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\PrivateCMW\\TimeLimitKey", false))
                    {
                        //如果是空的话，注册表信息控制值为true
                        if (registryKey == null)
                        {
                            flag1 = true;
                        }
                        else
                        {
                            //如果不为空，则检索注册表中与指定名称关联的值
                            str1 = registryKey.GetValue("Start").ToString();
                            str2 = registryKey.GetValue("End").ToString();

                            //判断两个值中是否包含 fromDate 和 toDate 两个值
                            //如果包含了这两个值，则判断许可已经使用
                            if (str1.Contains(fromDate.ToString()) && str2.Contains(toDate.ToShortDateString()))
                            {
                                message = "许可已使用！";
                                return false;
                            }
                            //如果不包含 fromDate 和 toDate ，则返回true
                            flag1 = true;
                        }
                    }
                }
                catch (Exception)
                {
                    message = "验证注册表失败";
                }
            }
            #endregion

            #region 许可使用的时间期限判断
            /*
            * fromDate(授权许可开始的时间)
            * toDate(授权许可结束的时间)
            * lastDate(授权许可最后一次生效的时间)
            * 
            * DataTime.Now < lastData（现在的时间 小于 授权许可最后一次生效的时间）
            * lastDate < fromDate（授权许可最后一次生效的时间 小于 授权许可开始的时间）
            * lastDate > toDate（授权许可最后一次生效的时间 大于 授权许可结束的时间）
            * DateTime.Now < fromDate（现在的时间 小于 授权许可开始的时间）
            * DateTime.Now > toDate（现在的时间 大于 授权许可结束的时间）
            * 
            */
            if (DateTime.Now < lastDate || lastDate < fromDate || (lastDate > toDate || DateTime.Now < fromDate) || DateTime.Now > toDate)
            {
                message = "许可已经过期！";
                return false;
            }
            #endregion

            #region 加密算法验证许可文件
            //创建一个 RSA不对称加密解密算法 对象
            RSACryptoServiceProvider cryptoServiceProvider = new RSACryptoServiceProvider();

            //XML字符串中的密钥信息（包含公钥或者包含公钥和私钥）:
            //<RSAKeyValue><Modulus>uqYMs9AxuAfGsbVvo+ah8Z7c91qbYJ8ARbX/7585ZVH1Jl9V5ebnjUEv+cuMjDEYzMCbJujoKZbqSRD5X5f9I2b9lNhRBhvBNBJj6ntzYKYp7HxYGTOr5NQ+eqNUejPhv9+fGedNa1oe/KyyfvE//NshoUN/oxVvCMlBIgHS98s=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>

            /* PEM格式公钥
                            -----BEGIN PUBLIC KEY-----
            MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQC6pgyz0DG4B8axtW+j5qHxntz3
            WptgnwBFtf/vnzllUfUmX1Xl5ueNQS/5y4yMMRjMwJsm6OgplupJEPlfl/0jZv2U
            2FEGG8E0EmPqe3NgpinsfFgZM6vk1D56o1R6M+G/358Z501rWh78rLJ+8T/82yGh
            Q3+jFW8IyUEiAdL3ywIDAQAB
                            -----END PUBLIC KEY-----
            */
            //初始化 RSA不对称加密解密算法
            cryptoServiceProvider.FromXmlString(PublicKey);
        
            bool flag2;

            try
            {
                //将字符编码转为一个UTF-16格式的字节序列
                byte[] bytes = LicenseUtil.ByteConvertor.GetBytes(LicenseUtil.SystemInfo);

                //打开许可文件，读取第一行的秘钥信息（s），通过Base64算法解密
                byte[] signature = Convert.FromBase64String(s);

                //传入字符序列，SHA1加密算法的哈希值，Base64的解密信息进行验证
               /*
                * VerifyData(buffer, halg, signature)
                * 通过使用提供的公钥计算签名中的哈希值，然后将其与提供的数据的哈希值进行比较，从而验证数字签名是否有效。
                * 
                * 返回值：
                * 如果签名有效，则为 true；否则为 false。
                * 
                * 参数：
                * buffer
                *    Type: Byte[]
                *     已签名的数据。
                * 
                * halg
                *    Type: Object
                *    用于创建数据哈希值的哈希算法的名称。
                * 
                * signature
                *    Type: System.Byte[]
                *    要验证的签名数据。
                * 
                */
                flag2 = cryptoServiceProvider.VerifyData(bytes, (object)LicenseUtil.DefaultHashAlgorithm, signature);
            }
            catch
            {
                message = "许可文件验证错误！";
                return false;
            }
            #endregion

            #region 注册表和许可文件更新
            //加密解密算法验证通过
            if (flag2)
            {
                //注册表验证通过
                if (flag1)
                {
                    //检索注册表 SOFTWARE\\PrivateCMW\\TimeLimitKey，并支持写入操作
                    using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\PrivateCMW\\TimeLimitKey", true))
                    {
                        //如果注册表为空
                        if (registryKey == null)
                        {
                            //创建注册表信息
                            using (RegistryKey subKey = Registry.CurrentUser.CreateSubKey("SOFTWARE\\PrivateCMW\\TimeLimitKey"))
                            {
                                //写入许可开始时间
                                subKey.SetValue("Start", (object)fromDate.ToString());
                                //写入许可结束时间
                                subKey.SetValue("End", (object)toDate.ToShortDateString());
                            }
                        }

                        else
                        {
                            //如果不为空
                            //追加新的许可开始时间和结束时间
                            registryKey.SetValue("Start", (object)(str1 + ";" + fromDate.ToString()));
                            registryKey.SetValue("End", (object)(str2 + ";" + toDate.ToShortDateString()));
                        }
                    }
                }
                //
                LicenseUtil.UpdateDateToFile(licenseFilePath, DateTime.Now);
            }
            else
            {
                message = "许可文件验证不通过！";
            }
            return flag2;
            #endregion
        }  
        catch
        {
            message = "许可文件可能被篡改！";
            return false;
        }
        
    }
  }
}
