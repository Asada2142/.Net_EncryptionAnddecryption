using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace KeyGen
{
    public class LicenseGenerate
    {
        public LicenseGenerate(string systemInfo, string fromDate, string toDate)
        {
            this._fromDateTime = fromDate;
            this._toDateTime = toDate;
            this._systemInfo = systemInfo;
        }

        #region 许可参数
        //机器序列号
        private string _systemInfo = "";

        //许可时间信息
        private string _fromDateTime = "";
        private string _toDateTime = "";

        //许可文件内容
        private string keyLicenseCode = "";
        private string timeLicenseCode = "";

        //存储要返回的许可信息
        private List<string> licenseCodeList;

        /// <summary>
        /// 机器序列号
        /// </summary>
        private string systemInfo
        {
            get { return _systemInfo; }
        }

        /// <summary>
        /// 许可开始时间
        /// </summary>
        private string fromDateTime
        {
            get { return _fromDateTime; }
        }

        /// <summary>
        /// 许可结束时间
        /// </summary>
        private string toDateTime
        {
            get { return _toDateTime; }
        }

        /// <summary>
        /// 私钥
        /// </summary>
        private const string privateKey = "<RSAKeyValue><Modulus>r+Fknz/UEsPyB+rOJpRt6BLPUnPvYFEAuXFtL6x23NI7LrUf+Xvcfo1iPMMJruOdNPs+wYNpxNy1166n5mRuSLn7EPbt1aumfCmtgq150NDWWdmx1y5rHNGFu9sUSUm+EgNEgVNbrjTTq8ru7ZJU2X5CLuLMYevuXwBaLZoS8kk=</Modulus><Exponent>AQAB</Exponent><P>6UVOdO86q+jMO8pdQGivk1EW2AnO3LRzkULvKcsXhqXXlQAjDhjPEF192osfmmnLXiqMkYd9DxcVb9oa5sLxXw==</P><Q>wQSLzXpqLsdb2lExGRx2iNEGZvxgjQxIkgb21DLrzgKSu6dTUzhxnZ2Iag4XcB5AWedseqRVvcGEkPmchQ31Vw==</Q><DP>kS/X0yQKqnCsnRIo1CvUC6bOxwvjuq59t42neaW0MNQLx+tb5iw+xHrMGDe7JcpvD18AOpvPlJLTftiLIdF3lQ==</DP><DQ>C5YYRkdY5GH3M424IsfAncneVoRDz3OzT4C3hFliKkWhRT5wFAjJWSrBq4wZABPwzPTFYD9JHlDlgkZZjOsflQ==</DQ><InverseQ>hnXlKRysPRroqHNd8SlzAcEPnWpgExIxLU2McE6slDO0DrABEyb/LHG6BTc8eBvzO6G+JiaKddTVKhHCLVU/rA==</InverseQ><D>Vo6JU6o494dBTM4s6GWx9T2UlJKD4xXaUmlU/9pToPdBswnmk4R2jj2MdDTURiK0kod3agr/eafZQi0takBQ2V4Kq86JI/Ei2VvMP781sPE/pnfENMufwfbdcbHdrU4heyd3DBT0NtTfOA71jeAlVbvMrCJVYDo/H3VVukGntUU=</D></RSAKeyValue>";

        /// <summary>
        /// 第一行的许可信息
        /// </summary>
        private string KeyLicenseCode
        {
            get { return keyLicenseCode; }
        }

        /// <summary>
        /// 第二行的许可信息
        /// </summary>
        private string TimeLicenseCode
        {
            get { return timeLicenseCode; }
        }

        /// <summary>
        /// SHA1(Secure Hash Algorithm)安全哈希算法的哈希值，主要用于数字签名验证
        /// </summary>
        private static HashAlgorithm DefaultHashAlgorithm
        {
            get
            {
                return (HashAlgorithm)new SHA1CryptoServiceProvider();
            }
        }

        /// <summary>
        /// 默认字节顺序编码为UTF-16编码
        /// </summary>
        private static Encoding ByteConvertor
        {
            get
            {
                return Encoding.Unicode;
            }
        }
        #endregion

        #region 注册码生成
        /// <summary>
        /// 时间许可
        /// </summary>
        /// <returns>返回加密的时间许可代码</returns>
        private string timeLicense(string fromDateTime, string toDateTime)
        {
            try
            {
                DateTime fromDate;
                DateTime toDate;

                fromDate = DateTime.Parse(fromDateTime);
                toDate = DateTime.Parse(toDateTime);

                DateTime nowDate = DateTime.Now;

                string str = Convert.ToBase64String(Encoding.Default.GetBytes(fromDate.ToString() + (object)"$" + toDate.ToShortDateString() + "$" + (object)nowDate.ToString()));
                return str;
            }
            catch
            {
                MessageBox.Show("时间许可生成失败！");
                return null;
            }

        }

        /// <summary>
        /// 私钥签名
        /// </summary>
        /// <param name="licenseFilePath">许可路径</param>
        /// <param name="signatureData">需要签名的数据（机器码）</param>
        /// <param name="privateKey">私钥</param>
        /// <returns></returns>
        private string privateKeySignature(byte[] signatureData, string privateKey)
        {
            try
            {
                //私钥签名
                RSACryptoServiceProvider cryptoServiceProvider = new RSACryptoServiceProvider();
                cryptoServiceProvider.FromXmlString(privateKey);

                //用SHA1的哈希算法指定字节数组的哈希值，并对计算所得的哈希值进行签名，得到加密后的字节数组
                byte[] signature = cryptoServiceProvider.SignData(signatureData, (object)DefaultHashAlgorithm);

                //签名数据用Base64算法进行加密
                string keyCode = Convert.ToBase64String(signature);
                return keyCode;
            }
            catch
            {
                MessageBox.Show("许可签名失败！");
                return null;
            }
        }
        #endregion

        #region 执行生成许可
        /// <summary>
        /// 生成注册码
        /// </summary>
        public List<string> hackExcute(out string message)
        {
            message = "许可生成成功!";

            /* 许可验证步骤：
             *  1.许可文件过滤
             *  2.许可文件读取
             *  3.注册表验证（许可时间）
             *  4.加密算法验证许可文件
             */

            licenseCodeList = new List<string>();

            //反向生成注册码
            // 1.许可过滤（存在、合法[.lic结尾]、不为空[文本内容]）
            if (fromDateTime == "" || toDateTime == "" || (fromDateTime == "" && toDateTime == ""))
            {
                message = "时间许可不能为空！";
                return null;
            }

            if (systemInfo == "")
            {
                message = "机器码不能为空！";
                return null;
            }

            // 2.许可写入
            //需要进行私钥签名的机器序列号数据
            byte[] signature = ByteConvertor.GetBytes(systemInfo);
            //获得第一行的许可注册码（经过RSA不对称算法私钥签名和Base64加密的机器码信息）
            keyLicenseCode = privateKeySignature(signature, privateKey);
            //获得第二行的许可注册吗（经过Base64加密的时间组合信息）
            timeLicenseCode = timeLicense(fromDateTime, toDateTime);

            licenseCodeList.Add(keyLicenseCode);
            licenseCodeList.Add(timeLicenseCode);
            
            return licenseCodeList;
        }
        #endregion
    }
}
