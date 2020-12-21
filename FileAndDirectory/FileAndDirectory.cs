using System.IO;
using System.IO.Compression;

namespace ToolConsole.Utility
{
    /// <summary>
    /// ディレクトリおよびファイル編集クラス
    /// </summary>
    public static class FileAndDirectory
    {
        /// <summary>
        /// 指定のディレクトリをコピー
        /// </summary>
        /// <param name="original">コピー元</param>
        /// <param name="copy">コピー先</param>
        public static void DirectoryCopy(string original, string copy)
        {
            DirectoryInfo dir = new DirectoryInfo(original);
            if (!dir.Exists) { throw new DirectoryNotFoundException("directory could not be found: " + original); }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // コピー先を生成
            Directory.CreateDirectory(copy);

            // 内部ファイルをコピー
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(copy, file.Name);
                _ = file.CopyTo(tempPath, false);
            }

            // サブディレクトリ処理のため再帰呼び出し
            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(copy, subdir.Name);
                DirectoryCopy(subdir.FullName, tempPath);
            }
        }

        /// <summary>
        /// 指定のディレクトリをZip圧縮
        /// </summary>
        /// <param name="targetPath">圧縮元</param>
        public static string CompressionDirectory(string targetPath)
        {
            var resultPath = targetPath + ".zip";

            // Zip圧縮
            ZipFile.CreateFromDirectory(targetPath, resultPath);

            // 元ディレクトリを削除
            Directory.Delete(targetPath, true);

            return resultPath;
        }

        /// <summary>
        /// 指定のディレクトリから一致する拡張子のファイル一覧を取得
        /// </summary>
        /// <param name="path">パス</param>
        /// <param name="type">拡張子(.md)</param>
        /// <returns></returns>
        public static string[] GetFiles(string path, string type)
        {
            return Directory.GetFiles(path, "*" + type);
        }
    }
}