using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using ToolConsole.Utility;

namespace MarkdownAdjustHugo
{
    /// <summary>
    /// RinkerプラグインHTMLの整形
    /// </summary>
    public static class Rinker
    {
        /// <summary>
        /// Start
        /// </summary>
        /// <param name="path">対象パス</param>
        public static void Starter(string path)
        {
            Console.WriteLine("\nプラグイン[Rinker]で生成されるHTMLを簡易的に変換します。");
            
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
            HtmlNodeCollection nodes = htmlDoc.DocumentNode.SelectNodes("//p[contains(@class, 'wp-block-rinkerg-gutenberg-rinker')]");
            if (null == nodes) return resultQueue;

            // 置換用のタグリストを生成
            for (int i = 0; i < nodes.Count; i++)
            {
                HtmlNode node = nodes[i];
                // 画像URLとリンク先を取得
                HtmlNode rinkerImage = node.SelectSingleNode("(//div[@class='yyi-rinker-image'])[" + (i + 1) + "]");
                var url = rinkerImage.SelectSingleNode(".//a").GetAttributeValue("href", "");
                var imgSrc = rinkerImage.SelectSingleNode(".//img").GetAttributeValue("src", "");

                // 商品名を取得
                HtmlNode rinkerTitle = node.SelectSingleNode("(//div[@class='yyi-rinker-title'])[" + (i + 1) + "]");
                var title = rinkerTitle.InnerText.Replace("\n", "").Trim();

                // 置換用のタグを作成
                var html = new StringBuilder()
                    .Append("<div class='hugo-rinker'>")
                    .Append("<a href='").Append(url).Append("' rel='noreferrer noopener external nofollow' targer='_blank'>")
                    .Append("<img src='").Append(imgSrc).Append("' loading='lazy'>")
                    .Append("<span>").Append(title).Append("</span>")
                    .Append("</a>")
                    .Append("</div>")
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
            MatchCollection matches = Regex.Matches(text, "<p class=\"wp-block-rinkerg-gutenberg-rinker[\\s\\S]*?</p>");
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