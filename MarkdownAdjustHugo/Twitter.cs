using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using ToolConsole.Utility;

namespace MarkdownAdjustHugo
{
    /// <summary>
    /// Twitterの整形
    /// </summary>
    public static class Twitter
    {
        /// <summary>
        /// Start
        /// </summary>
        /// <param name="path">対象パス</param>
        public static void Starter(string path)
        {
            Console.WriteLine("\nTwitterタグを変換します。");

            // 実行可否を判定
            Console.Write("本機能を利用しますか？[y/n]：");
            bool isActive = Console.ReadLine().ToLower() == "y";

            if (!isActive)
            {
                // y以外を回答した場合、機能を終了
                Console.WriteLine("処理をスキップしました。\n");
                return;
            }

            // 処理実行
            string[] markdowns = FileAndDirectory.GetFiles(path, ".md");
            foreach (string markdown in markdowns)
            {
                ConvertTags(markdown);
            }
        }

        /// <summary>
        /// 指定のファイルを編集
        /// </summary>
        /// <param name="filePath">対象ファイルのパス</param>
        private static void ConvertTags(string filePath)
        {
            // 記事全文を取得
            string text;
            using (StreamReader sr = new StreamReader(filePath))
            {
                text = sr.ReadToEnd();
            }

            // 置換リストを取得
            Queue<string> queue = CreateReplaceTags(text);

            // 置換
            text = ReplaceText(text, queue);

            // ファイル上書き
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.Write(text);
            }
        }

        /// <summary>
        /// 置換するタグリストを生成
        /// </summary>
        /// <param name="text">記事全文</param>
        /// <returns>リスト</returns>
        private static Queue<string> CreateReplaceTags(string text)
        {
            var resultQueue = new Queue<string>();

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(text);

            // タグが存在しない場合、終了
            HtmlNodeCollection nodes = htmlDoc.DocumentNode.SelectNodes("//figure[contains(@class, 'wp-block-embed-twitter')]");
            if (null == nodes) return resultQueue;

            // 置換用のタグリストを生成
            for (int i = 0; i < nodes.Count; i++)
            {
                HtmlNode node = nodes[i];

                HtmlNode tweetText = node.SelectSingleNode("(//blockquote[@class='twitter-tweet'])[" + (i + 1) + "]");
                var body = tweetText.InnerHtml;
                body = body.Replace("\n", "");

                // 置換用のタグを作成
                var html = new StringBuilder()
                    .Append("<blockquote class='twitter-tweet'>")
                    .Append(body)
                    .Append("</blockquote>")
                    .Append("<script async src='https://platform.twitter.com/widgets.js' charset='utf-8'></script>")
                    .ToString();
                resultQueue.Enqueue(html);
            }

            return resultQueue;
        }

        /// <summary>
        /// タグを置換
        /// </summary>
        /// <param name="text">元記事</param>
        /// <param name="tags">置換タグキュー</param>
        /// <returns>置換後の記事</returns>
        private static string ReplaceText(string text, Queue<string> tags)
        {
            MatchCollection matches = Regex.Matches(text, "<figure class=\"wp-block-embed-twitter[\\s\\S]*?</figure>");
            foreach (Match match in matches)
            {
                var target = match.Value;
                var tag = tags.Dequeue();
                text = text.Replace(target, tag);
            }

            return text;
        }
    }
}