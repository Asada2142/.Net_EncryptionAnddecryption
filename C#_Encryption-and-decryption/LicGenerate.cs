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
    public class LicGenerate
    {
        private string _sysinfo = "";

        public LicGenerate(string sysInfo)
        {
            this._sysinfo = sysInfo;
        }

        #region 许可参数
        //许可时间信息
        private string _fromDateTime = "";
        private string _toDateTime = "";

        //机器序列号
        private static string _systemInfo = string.Empty;

        //许可文件内容
        private string keyLicenseCode = "";
        private string timeLicenseCode = "";

        /// <summary>
        /// 许可开始时间
        /// </summary>
        public string fromDateTime
        {
            set { _fromDateTime = value; }
            get { return _fromDateTime; }
        }

        /// <summary>
        /// 许可结束时间
        /// </summary>
        public string toDateTime
        {
            set { _toDateTime = value; }
            get { return _toDateTime; }
        }

        /// <summary>
        /// 私钥
        /// </summary>
        private const string privateKey = "<RSAKeyValue><Modulus>r+Fknz/UEsPyB+rOJpRt6BLPUnPvYFEAuXFtL6x23NI7LrUf+Xvcfo1iPMMJruOdNPs+wYNpxNy1166n5mRuSLn7EPbt1aumfCmtgq150NDWWdmx1y5rHNGFu9sUSUm+EgNEgVNbrjTTq8ru7ZJU2X5CLuLMYevuXwBaLZoS8kk=</Modulus><Exponent>AQAB</Exponent><P>6UVOdO86q+jMO8pdQGivk1EW2AnO3LRzkULvKcsXhqXXlQAjDhjPEF192osfmmnLXiqMkYd9DxcVb9oa5sLxXw==</P><Q>wQSLzXpqLsdb2lExGRx2iNEGZvxgjQxIkgb21DLrzgKSu6dTUzhxnZ2Iag4XcB5AWedseqRVvcGEkPmchQ31Vw==</Q><DP>kS/X0yQKqnCsnRIo1CvUC6bOxwvjuq59t42neaW0MNQLx+tb5iw+xHrMGDe7JcpvD18AOpvPlJLTftiLIdF3lQ==</DP><DQ>C5YYRkdY5GH3M424IsfAncneVoRDz3OzT4C3hFliKkWhRT5wFAjJWSrBq4wZABPwzPTFYD9JHlDlgkZZjOsflQ==</DQ><InverseQ>hnXlKRysPRroqHNd8SlzAcEPnWpgExIxLU2McE6slDO0DrABEyb/LHG6BTc8eBvzO6G+JiaKddTVKhHCLVU/rA==</InverseQ><D>Vo6JU6o494dBTM4s6GWx9T2UlJKD4xXaUmlU/9pToPdBswnmk4R2jj2MdDTURiK0kod3agr/eafZQi0takBQ2V4Kq86JI/Ei2VvMP781sPE/pnfENMufwfbdcbHdrU4heyd3DBT0NtTfOA71jeAlVbvMrCJVYDo/H3VVukGntUU=</D></RSAKeyValue>";

        /// <summary>
        /// 第一行的许可信息
        /// </summary>
        public string KeyLicenseCode
        {
            set{ keyLicenseCode = value; }
            get{ return keyLicenseCode; }
        }

        /// <summary>
        /// 第二行的许可信息
        /// </summary>
        public string TimeLicenseCode
        {
            set{ timeLicenseCode = value; }
            get{ return timeLicenseCode; }
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

        #region 逆向许可方法
        #region 生成许可文件
        /// <summary>
        /// 许可文件保存
        /// </summary>
        /// <returns>返回许可文件路径</returns>
        private string licenseFileSave()
        {
            try
            {
                string licenseFilePath;
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.InitialDirectory = "D:\\";
                saveFileDialog.Filter = "许可文件(*.lic)|*.lic";
                saveFileDialog.RestoreDirectory = true;
                DialogResult dr = saveFileDialog.ShowDialog();
                if (dr == DialogResult.OK && saveFileDialog.FileName.Length > 0)
                {
                    licenseFilePath = saveFileDialog.FileName;
                }
                else
                {
                    return null;
                }
                return licenseFilePath;
            }
            catch
            {
                MessageBox.Show("许可文件存储失败！");
                return null;
            }        
        }
        #endregion

        #region 注册码生成
        /// <summary>
        /// 时间许可
        /// </summary>
        /// <returns>返回加密的时间许可代码</returns>
        private string timeLicense()
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

        //#region 系统信息
        ///// <summary>
        ///// 获得机器码信息
        ///// </summary>
        //private static string SystemInfo
        //{
        //    get
        //    {
        //        try
        //        {
        //            //如果字符串系统信息为空值
        //            if (string.IsNullOrEmpty(_systemInfo))
        //            {
        //                /*
        //                 * 1.GetCpuId().ToUpper()：获取CPU的ID信息（序列号），并转成大写
        //                 * 2.GetMotherBoardId().ToUpper()：获取主板的ID信息（序列号），并转成大写
        //                 * 3.GetHarDiskId().ToUpper()：获取硬盘的ID信息（序列号），并转成大写
        //                 * 两个信息用"_"相连接，返回系统信息字符串。
        //                 */
        //                _systemInfo = GetCpuId().ToUpper() + "_" + GetMotherBoardId().ToUpper() + "_" + GetHardDiskId().ToUpper();
        //            }
        //            return _systemInfo;
        //        }
        //        catch
        //        {
        //            MessageBox.Show("机器码序列号获取失败！");
        //            return null;
        //        }
                
        //    }
        //}

        ///// <summary>
        ///// 获取CPU ID信息
        ///// </summary>
        ///// <returns>返回获取CPU ID</returns>
        //private static string GetCpuId()
        //{
        //    //传入"Win32_Processor"可获得CPU处理器信息
        //    ManagementObjectCollection instances = new ManagementClass("Win32_Processor").GetInstances();

        //    string str = (string)null;

        //    //
        //    using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = instances.GetEnumerator())
        //    {
        //        if (enumerator.MoveNext())
        //            str = enumerator.Current.Properties["ProcessorId"].Value.ToString();
        //    }
        //    return str;
        //}

        ///// <summary>
        ///// 获取硬盘ID信息
        ///// </summary>
        ///// <returns>返回硬盘ID信息</returns>
        //private static string GetHardDiskId()
        //{
        //    ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");
        //    string str = (string)null;
        //    foreach (ManagementBaseObject managementBaseObject in managementObjectSearcher.Get())
        //        str = managementBaseObject["SerialNumber"].ToString().Trim();
        //    return str;
        //}

        ///// <summary>
        ///// 获取主板信息
        ///// </summary>
        ///// <returns>返回主板信息</returns>
        //private static string GetMotherBoardId()
        //{
        //    //
        //    ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BaseBoard");
        //    string str = string.Empty;
        //    using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = managementObjectSearcher.Get().GetEnumerator())
        //    {
        //        if (enumerator.MoveNext())
        //        {
        //            object obj = enumerator.Current["SerialNumber"];
        //            if (obj != null)
        //                str = obj.ToString();
        //        }
        //    }
        //    return str;
        //}
        //#endregion

        #region 注册表清理
        /// <summary>
        /// 注册表清理
        /// </summary>
        /// <returns>返回注册表清理结果</returns>
        private bool regeditWrite()
        {
            DateTime fromDate;
            DateTime toDate;

            fromDate = DateTime.Parse(fromDateTime);
            toDate = DateTime.Parse(toDateTime);

            try
            {
                using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\PrivateCMW\\TimeLimitKey", true))
                {
                    //如果注册表存在，就删除注册表
                    if (registryKey != null)
                    {
                        //using (RegistryKey registryCreate = Registry.CurrentUser.CreateSubKey("SOFTWARE\\PrivateCMW\\TimeLimitKey"))
                        //{
                        //    //写入许可开始时间
                        //    registryCreate.SetValue("Start", (object)fromDate.ToString());
                        //    //写入许可结束时间
                        //    registryCreate.SetValue("End", (object)toDate.ToShortDateString());
                        //}
                        //return true;
                        try
                        {
                            RegistryKey registryDelete = Registry.CurrentUser;
                            registryDelete.DeleteSubKey("SOFTWARE\\PrivateCMW\\TimeLimitKey", true);
                            registryDelete.Close();
                        }
                        catch
                        {
                            MessageBox.Show("注册表清理失败！");
                            return false;
                        }
                        //using (RegistryKey registryCreate = Registry.CurrentUser.CreateSubKey("SOFTWARE\\PrivateCMW\\TimeLimitKey"))
                        //{
                        //    //写入许可开始时间
                        //    registryCreate.SetValue("Start", (object)fromDate.ToString());
                        //    //写入许可结束时间
                        //    registryCreate.SetValue("End", (object)toDate.ToShortDateString());
                        //}
                    }

                    ////如果存在就清理并创建新的注册表
                    //else
                    //{
                        
                    //}
                }
                return true;
            }
            catch
            {
                MessageBox.Show("注册表清理失败！");
                return false;
            }
        }
        #endregion

        #region 许可文件写入
        /// <summary>
        /// 许可文件写入
        /// </summary>
        /// <param name="licenseFilePath">许可文件路径</param>
        /// <param name="keyCode">密钥许可</param>
        /// <param name="timeCode">时间许可</param>
        /// <returns>返回许可文件写入结果</returns>
        private bool licenseWrite(string licenseFilePath, string keyCode, string timeCode)
        {
            try
            {
                if (!File.Exists(licenseFilePath))
                {
                    using (FileStream fileStream = new FileStream(licenseFilePath, FileMode.Create, FileAccess.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)fileStream, Encoding.Default))
                        {
                            streamWriter.WriteLine(keyCode);
                            streamWriter.WriteLine(timeCode);
                        }
                    }
                    return true;
                }
                else
                {
                    using (FileStream fileStream = new FileStream(licenseFilePath, FileMode.Open, FileAccess.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)fileStream, Encoding.Default))
                        {
                            streamWriter.WriteLine(keyCode);
                            streamWriter.WriteLine(timeCode);
                        }
                    }
                    return true;
                }
            }
            catch
            {
                MessageBox.Show("许可文件写入失败！");
                return false;
            }
        }
        #endregion
        #endregion

        #region 执行生成许可文件
        /// <summary>
        /// 生成注册码
        /// </summary>
        public void hackExcute()
        {
            /* 许可验证步骤：
             *  1.许可文件过滤
             *  2.许可文件读取
             *  3.注册表验证（许可时间）
             *  4.加密算法验证许可文件
             */

            //反向生成注册码
            // 1.许可过滤（存在、合法[.lic结尾]、不为空[文本内容]）
            if (fromDateTime == "" || toDateTime == "" || (fromDateTime == "" && toDateTime == ""))
            {
                MessageBox.Show("时间许可不能为空！");
                return;
            }
            // 2.许可写入
            //需要进行私钥签名的机器序列号数据
            //byte[] signature = ByteConvertor.GetBytes(SystemInfo);

            byte[] sys = Convert.FromBase64String(_sysinfo);

            //获得第一行的许可注册码（经过RSA不对称算法私钥签名和Base64加密的机器码信息）
            keyLicenseCode = privateKeySignature(sys, privateKey);
            //获得第二行的许可注册吗（经过Base64加密的时间组合信息）
            timeLicenseCode = timeLicense();

            // 3.注册许可
            //对注册表进行清理，并注册新的许可
            bool regeditControllder = regeditWrite();

            //返回结果
            //if (regeditControllder)
            //    MessageBox.Show("注册表清理成功！");
        }

        /// <summary>
        /// 生成许可文件
        /// </summary>
        /// <returns>返回生成结果</returns>
        public bool licenseFileGenerate()
        {
            try 
            {
                string licenseFilePath = licenseFileSave();
                //写入许可文件，返回结果
                bool licenseController = licenseWrite(licenseFilePath, keyLicenseCode, timeLicenseCode);
                return licenseController;
            }
            catch 
            {
                MessageBox.Show("写入文件失败！");
                return false;
            }    
        }
        #endregion
    }
}
