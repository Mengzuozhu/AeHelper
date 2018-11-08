using System.Collections.Generic;
using System.IO;
using System.Text;
using ESRI.ArcGIS.Geoprocessor;

namespace AeHelper.LayerProcess.AllLayerProcess
{
    /// <summary>
    /// 几何处理
    /// </summary>
    public class MyGeoprocessor
    {
        /// <summary>
        /// 获取GP处理所需的输入列表
        /// </summary>
        /// <param name="fileList">输入列表</param>
        /// <returns></returns>
        public static string GetGpInFiles(List<string> fileList)
        {
            //移除不存在文件
            RemoveNotExistFile(fileList);
            if (fileList.Count == 0)
            {
                return null;
            }
            StringBuilder inFileBuilder = new StringBuilder();
            inFileBuilder.Append(fileList[0]);
            for (int i = 1; i < fileList.Count; i++)
            {
                if (!File.Exists(fileList[i]))
                {
                    continue;
                }
                inFileBuilder.Append(@"; " + fileList[i]);
            }
            return inFileBuilder.ToString();
        }

        /// <summary>
        /// 移除不存在文件
        /// </summary>
        /// <param name="fileList"></param>
        private static void RemoveNotExistFile(List<string> fileList)
        {
            //移除不存在文件
            for (int i = fileList.Count - 1; i >= 0; i--)
            {
                if (!File.Exists(fileList[i]))
                {
                    fileList.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 获取GP工具
        /// </summary>
        /// <returns></returns>
        private static Geoprocessor GetGeoprocessor()
        {
            return new Geoprocessor { OverwriteOutput = true };
        }

        /// <summary>
        /// 异步执行GP处理
        /// </summary>
        /// <param name="gpProcess">GP处理</param>
        public static void GpExecuteAsync(IGPProcess gpProcess)
        {
            Geoprocessor geoprocessor = GetGeoprocessor();
            geoprocessor.ExecuteAsync(gpProcess);
        }

        /// <summary>
        /// 同步执行GP处理（注意许可证要满足调用的工具）
        /// </summary>
        /// <param name="gpProcess">GP处理</param>
        public static void GpExecute(IGPProcess gpProcess)
        {
            Geoprocessor geoprocessor = GetGeoprocessor();
            geoprocessor.Execute(gpProcess, null);
        }

    }
}
