using MarkdownAdjustHugo;
using System;
using ToolConsole.Utility;
using ImageCleaner;

namespace ToolConsole
{
    /// <summary>
    /// スタートアップクラス
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// 編集対象のディレクトリパス
        /// </summary>
        public static string TargetPostPath { get; private set; }

        /// <summary>
        /// スタートアップメソッド
        /// </summary>
        private static void Main()
        {
            // Markdownの編集
            CallAdjustMarkdown();

            // 画像の整理
            CallImageCleaner();
        }

        /// <summary>
        /// 処理対象の準備
        /// </summary>
        private static void SetUp()
        {
            var bkTime = DateTime.Now.ToString("yyyyMMHHmmss");

            Console.Write("変換対象とするファイルの保存されたディレクトリのパスを指定してください：");
            TargetPostPath = Console.ReadLine();
            string backupName = TargetPostPath + "_" + bkTime;

            Console.Write("バックアップを生成しますか？[y/n]：");
            bool isBackup = Console.ReadLine().ToLower() == "y";
            if (isBackup)
            {
                FileAndDirectory.DirectoryCopy(TargetPostPath, backupName);
                Console.WriteLine("-> " + FileAndDirectory.CompressionDirectory(backupName));
            }
            else
            {
                Console.WriteLine("処理をスキップしました。\n");
            }
        }

        /// <summary>
        /// Markdown編集機能の呼び出し
        /// </summary>
        private static void CallAdjustMarkdown()
        {
            Console.WriteLine("WordPressから取得した記事データをHugo向けに調整します。\n");

            // 調整対象の情報を取得
            SetUp();

            // Twitterの変換
            Twitter.Starter(TargetPostPath);

            // Rinkerの変換
            Rinker.Starter(TargetPostPath);
        }

        /// <summary>
        /// 画像整理機能の呼び出し
        /// </summary>
        private static void CallImageCleaner()
        {
            Console.WriteLine("WordPressが生成した様々な画像サイズ差分を整理します。\n");

            // 調整対象の情報を取得
            SetUp();

            // 差分画像の削除
            Images.Starter(TargetPostPath);
        }
    }
}