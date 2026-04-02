using System;
using System.IO;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SptItemCreator.Core.Services;

namespace SptItemCreator.Tests.Core.Services;

[TestClass]
[TestSubject(typeof(DataLoader))]
public class DataLoaderTest
{

    [TestMethod]
    public void TestStripJsoncComments()
    {
        // 本函数用于测试StripJsoncComments函数能否正确处理Jsonc格式

        // 测试移除注释能力
        string test1 = Path.Combine("data", "StripJsoncComments", "test_jsonc_1.jsonc");
        
        // 测试不改变原数据能力
        string test2 = Path.Combine("data", "StripJsoncComments", "test_jsonc_2.jsonc");

        foreach (string path in new[] { test1, test2 })
        {
            Console.WriteLine($"=========================={path}===========================");
            string json = File.ReadAllText(path);
            string jsonClean = DataLoader.StripJsoncComments(json);
            Console.WriteLine("原数据:");
            Console.WriteLine($"```jsonc\n{json}\n```");
            Console.WriteLine("处理后数据:");
            Console.WriteLine($"```json\n{jsonClean}\n```");
        }
        
    }
}