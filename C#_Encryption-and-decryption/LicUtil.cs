using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace LicenseCommonUtil
{
  public static class LicenseUtil
  {
    private static string _systemInfo = string.Empty;

    public static Encoding ByteConvertor
    {
      get
      {
        return Encoding.Unicode;
      }
    }

    public static HashAlgorithm DefaultHashAlgorithm
    {
      get
      {
        return (HashAlgorithm) new SHA1CryptoServiceProvider();
      }
    }

    public static string DefaultLicenseFileExtension
    {
      get
      {
        return ".lic";
      }
    }

    public static string SystemInfo
    {
      get
      {
          //如果字符串系统信息为空值
          if (string.IsNullOrEmpty(LicenseUtil._systemInfo))
          {
              /*
               * 1.GetCpuId().ToUpper()：获取CPU的ID信息（序列号），并转成大写
               * 2.GetMotherBoardId().ToUpper()：
               * 
               * 
               */

              LicenseUtil._systemInfo = LicenseUtil.GetCpuId().ToUpper() + "_" + LicenseUtil.GetMotherBoardId().ToUpper() + "_" + LicenseUtil.GetHardDiskId().ToUpper();
          }
          return LicenseUtil._systemInfo;
      }
    }

    private static string GetCpuId()
    {
        //传入"Win32_Processor"可获得CPU处理器信息
        ManagementObjectCollection instances = new ManagementClass("Win32_Processor").GetInstances();
        
        string str = (string) null;

        //
        using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = instances.GetEnumerator())
        {
            if (enumerator.MoveNext())
                str = enumerator.Current.Properties["ProcessorId"].Value.ToString();
        }
        return str;
    }

    private static string GetHardDiskId()
    {
      ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");
      string str = (string) null;
      foreach (ManagementBaseObject managementBaseObject in managementObjectSearcher.Get())
        str = managementBaseObject["SerialNumber"].ToString().Trim();
      return str;
    }

    private static string GetMotherBoardId()
    {
      //
      ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BaseBoard");
      string str = string.Empty;
      using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = managementObjectSearcher.Get().GetEnumerator())
      {
        if (enumerator.MoveNext())
        {
          object obj = enumerator.Current["SerialNumber"];
          if (obj != null)
            str = obj.ToString();
        }
      }
      return str;
    }

    public static void SaveFile(string fileContent, string defaultExtension, string filter)
    {
      SaveFileDialog saveFileDialog = new SaveFileDialog();
      saveFileDialog.AddExtension = true;
      saveFileDialog.DefaultExt = defaultExtension;
      saveFileDialog.Filter = filter;
      saveFileDialog.FileName = "licenseInfo";
      if (saveFileDialog.ShowDialog() != DialogResult.OK)
        return;
      using (FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write))
      {
        using (StreamWriter streamWriter = new StreamWriter((Stream) fileStream, Encoding.Default))
        {
          streamWriter.Write(fileContent);
          int num = (int) MessageBox.Show("文件保存成功！", "提示");
        }
      }
    }

    public static void SaveFile(string fileContent, string nyr, string defaultExtension, string filter)
    {
      SaveFileDialog saveFileDialog = new SaveFileDialog();
      saveFileDialog.AddExtension = true;
      saveFileDialog.DefaultExt = defaultExtension;
      saveFileDialog.Filter = filter;
      saveFileDialog.FileName = "license.lic";
      if (saveFileDialog.ShowDialog() != DialogResult.OK)
        return;
      using (FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write))
      {
        using (StreamWriter streamWriter = new StreamWriter((Stream) fileStream, Encoding.Default))
        {
          streamWriter.WriteLine(fileContent);
          streamWriter.WriteLine(nyr);
          int num = (int) MessageBox.Show("文件保存成功！", "提示");
        }
      }
    }

    public static void SaveFile(string fileContent, string nyr, string todate, string defaultExtension, string filter)
    {
      SaveFileDialog saveFileDialog = new SaveFileDialog();
      saveFileDialog.AddExtension = true;
      saveFileDialog.DefaultExt = defaultExtension;
      saveFileDialog.Filter = filter;
      saveFileDialog.FileName = "license.lic";
      if (saveFileDialog.ShowDialog() != DialogResult.OK)
        return;
      using (FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write))
      {
        using (StreamWriter streamWriter = new StreamWriter((Stream) fileStream, Encoding.Default))
        {
          streamWriter.WriteLine(fileContent);
          string str = Convert.ToBase64String(Encoding.Default.GetBytes(nyr + "$" + todate));
          streamWriter.WriteLine(str);
          int num = (int) MessageBox.Show("文件保存成功！", "提示");
        }
      }
    }

    public static string ReadContentFromFile(string filePath)
    {
      using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
      {
        using (StreamReader streamReader = new StreamReader((Stream) fileStream, Encoding.Default))
          return streamReader.ReadToEnd();
      }
    }

    public static string ReadContentFromFile2(string filePath)
    {
      using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
      {
        using (StreamReader streamReader = new StreamReader((Stream) fileStream, Encoding.Default))
          //返回第一行的许可信息
          return streamReader.ReadLine();
      }
    }

    public static DateTime ReadDateTiFromFile(string filePath)
    {
      string s = string.Empty;
      string str1 = string.Empty;
      
      using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
      {
        using (StreamReader streamReader = new StreamReader((Stream) fileStream, Encoding.Default))
        {
          string str2;
          do
          {
            str2 = streamReader.ReadLine();
            s = str2 ?? s;
          }
          while (str2 != null);
        }
      }
      return DateTime.Parse(s);
    }

    /// <summary>
    /// 从许可中读取时间信息
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="fromDate"></param>
    /// <param name="toDate"></param>
    /// <param name="lastDate"></param>
    public static void ReadDateTiFromFile(string filePath, out DateTime fromDate, out DateTime toDate, out DateTime lastDate)
    {
      List<string> list = new List<string>();
      using (FileStream fileStream = new FileStream(filePath, FileMode.Open)) //打开文件流
      {
         using (StreamReader streamReader = new StreamReader((Stream) fileStream, Encoding.Default)) //打开文件流读取内容
         {
            //逐行遍历文件
            //如果行字符串不为空，则继续遍历
            //每次读取新的一行
            for (string str = streamReader.ReadLine(); !string.IsNullOrEmpty(str); str = streamReader.ReadLine())
            {
                //将每行的字符串信息添加到列表中存储
                list.Add(str);
            }
         }
      }
      
      //将当前计算机的本地时间赋值给三个变量
      fromDate = toDate = lastDate = DateTime.Now;

      //获取到列表的第二个索引对应的值
      string s = list[1];

      //如果第二个索引对应的值为空，则返回空值
      if (string.IsNullOrEmpty(s))
        return;
      
      //创建字符串数组
      /* 传入第二行的值
       * 从Base64加密算法解密再转换成字符串
       * 通过'$'符号对字符串进行分割，得到字符串数组，并且返回值不包含空字符串
       * 得到的三个值分别为 fromDate、toDate、lastDate(如果字符串数组的长度大于2)
       * 通过out参数传出
       */
      string[] strArray = Encoding.Default.GetString(Convert.FromBase64String(s)).Split(new char[1]{'$'}, StringSplitOptions.RemoveEmptyEntries);

      //分别对应索引值进行赋值
      fromDate = DateTime.Parse(strArray[0]);
      toDate = DateTime.Parse(strArray[1]);
      if (strArray.Length > 2)
        lastDate = DateTime.Parse(strArray[2]);
      else
        lastDate = fromDate;
    }

    public static void UpdateDateToFile(string filePath, DateTime nowDate)
    {
      List<string> list = new List<string>();
      //只读
      using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
      {
        using (StreamReader streamReader = new StreamReader((Stream) fileStream, Encoding.Default))
        {
            //如果文件行不为空，逐行遍历，并添加到列表中
            for (string str = streamReader.ReadLine(); !string.IsNullOrEmpty(str); str = streamReader.ReadLine())
            {
                list.Add(str);
            }
        }
      }

      
      DateTime dateTime1;
      DateTime dateTime2 = dateTime1 = DateTime.Now;
      
      //许可第二行的内容
      string s = list[1];

      //如果许可第二行不为空
      if (!string.IsNullOrEmpty(s))
      {
        //创建字符串数组
          /* 传入第二行的值
           * 从Base64加密算法解密再转换成字符串
           * 通过'$'符号对字符串进行分割，得到字符串数组，并且返回值不包含空字符串
           */
          string[] strArray = Encoding.Default.GetString(Convert.FromBase64String(s)).Split(new char[1]
        {
          '$'
        }, StringSplitOptions.RemoveEmptyEntries);
          
        //dataTime2是通过Base64解密的通过'$'进行分割得到的第一行时间数据
        dateTime2 = DateTime.Parse(strArray[0]);

        //dataTime1
        dateTime1 = DateTime.Parse(strArray[1]);
      }
    
      //支持写入
      using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Write))
      {
        using (StreamWriter streamWriter = new StreamWriter((Stream) fileStream, Encoding.Default))
        {
          //写入许可文件的第一行内容
          streamWriter.WriteLine(list[0]);

          /* 通过Base64加密算法进行加密
           * 传入参数类型：字节序列
           * 参数：
           *    1.fromDate
           *    2.toDate
           *    3.lastDate
           * 
           */
          string str = Convert.ToBase64String(Encoding.Default.GetBytes(dateTime2.ToString() + (object)"$" + dateTime1.ToShortDateString() + (object)"$" + (object)nowDate.ToString()));
          streamWriter.WriteLine(str);
        }
      }
    }

    public static string GetStringFromByte(byte[] datas)
    {
      if (datas == null)
        return string.Empty;
      string str = string.Empty;
      foreach (byte num in datas)
        str = str + (object) num + "_";
      return str;
    }
  }
}
