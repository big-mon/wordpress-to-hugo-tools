using System;
using MarkdownAdjustHugo;
using ToolConsole.Utility;

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
            Console.WriteLine("WordPressから取得した記事データをHugo向けに調整します。\n");

            // 調整対象の情報を取得
            SetUp();

            // Twitterの変換
            Twitter.Starter(TargetPostPath);

            // Rinkerの変換
            Rinker.Starter(TargetPostPath);
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
    }
}