using System;
using System.IO;
using System.Text.Json;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SptItemCreator.Models.Abstracts;
using SptItemCreator.Models.Items;

namespace SptItemCreator.Tests.Models.Items;

[TestClass]
[TestSubject(typeof(NewItem))]
public class NewItemTest
{

    [TestMethod]
    public void TestVerify()
    {
        foreach (string file in Directory.GetFiles(Path.Combine("data", "NewItems")))
        {
            try
            {
                var newItem = JsonSerializer.Deserialize<NewItem>(File.ReadAllText(file));
                Console.WriteLine("=================================================");
                // Console.WriteLine(JsonSerializer.Serialize(newItem));
                
                newItem.ItemPath = file;

                (bool result, IErrorCollector errorCollector) = newItem.Verify();
                Console.WriteLine($"{newItem.ItemPath}:\n\t- result: {result}\n\t- errorCollector: \n{errorCollector.ErrorsToString()}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}