using System.Collections.Generic;
using ESRI.ArcGIS.DataManagementTools;
using ExternalProgram.FileAndDirectory;

namespace AeHelper.LayerProcess.GeoProcess
{
    /// <summary>
    /// GP工具-融合
    /// </summary>
    class GpMergeClass
    {
        /// <summary>
        /// 融合多个图层文件
        /// </summary>
        /// <param name="inFiles"></param>
        /// <param name="outFile"></param>
        public static void MergeMultFile(List<string> inFiles, string outFile)
        {
            string temDir = TempFile.CreateNewTempDirectory();
            if (inFiles.Count < 2) return;
            for (int i = inFiles.Count - 1; i >= 1; i--)
            {
                if (inFiles.Count < 3)
                {
                    break;
                }
                string firstFile = inFiles[i];
                string secondFile = inFiles[i - 1];
                string temOutFile = GetTemOutFile(temDir, "tem" + i);
                MergeTwoFile(firstFile, secondFile, temOutFile);
                inFiles.RemoveRange(i - 1, 2);
                inFiles.Add(temOutFile);
            }
            MergeTwoFile(inFiles[1], inFiles[0], outFile);
        }

        /// <summary>
        /// 融合2个图层文件
        /// </summary>
        /// <param name="firstFile"></param>
        /// <param name="secondFile"></param>
        /// <param name="outFile"></param>
        private static void MergeTwoFile(string firstFile, string secondFile, string outFile)
        {
            Merge gpMerge = new Merge
            {
                inputs = GetTwoInFileString(firstFile, secondFile),
                output = outFile
            };
            GeoprocessorHelper.GpExecute(gpMerge);
        }

        /// <summary>
        /// 获取2个输入文件字符串
        /// </summary>
        /// <param name="firstFile"></param>
        /// <param name="secondFile"></param>
        /// <returns></returns>
        private static string GetTwoInFileString(string firstFile, string secondFile)
        {
            return string.Format("{0} ; {1}", firstFile, secondFile);
        }

        /// <summary>
        /// 获取临时输出文件
        /// </summary>
        /// <returns></returns>
        private static string GetTemOutFile(string temDir, string fileName)
        {
            return string.Format(@"{0}\Eca{1}.shp", temDir, fileName);
        }

    }
}
