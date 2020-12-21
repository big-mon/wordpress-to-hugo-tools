using System;
using ToolConsole.Utility;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace ImageCleaner
{
    /// <summary>
    /// 画像整理クラス
    /// </summary>
    public static class Images
    {
        /// <summary>
        /// 画像を整理
        /// </summary>
        /// <param name="path">対象パス</param>
        public static void Starter(string path)
        {
            Console.WriteLine("\n差分画像を一括削除します。");

            // 実行可否を判定
            Console.Write("本機能を利用しますか？[y/n]：");
            bool isActive = Console.ReadLine().ToLower() == "y";

            if (!isActive)
            {
                // y以外を回答した場合、機能を終了
                Console.WriteLine("処理をスキップしました。\n");
                return;
            }

            // 差分画像のパスを取得
            IReadOnlyList<string> imagesPath = GetImagePieces(path);

            // 画像を削除
            foreach (var f in imagesPath)
            {
                FileAndDirectory.DeleteFile(f);
            }
        }

        /// <summary>
        /// 差分画像の一覧を取得
        /// </summary>
        /// <param name="path">対象パス</param>
        /// <returns>差分画像パスのリスト</returns>
        private static List<string> GetImagePieces(string path)
        {
            // 正規表現のフィルタを作成
            Regex reg = new Regex(".+-[0-9]+x[0-9]+[.[a-z]*|a]+");

            // 指定拡張子の画像をフィルタしたうえで取得
            List<string> images = FileAndDirectory.GetFiles(path, ".*")
                .Where(x => x.ToLower().EndsWith("jpg") || x.ToLower().EndsWith("png") || x.ToLower().EndsWith("gif") || x.ToLower().EndsWith("webp"))
                .Where(x => reg.IsMatch(x))
                .ToList();

            return images;
        }
    }
}
