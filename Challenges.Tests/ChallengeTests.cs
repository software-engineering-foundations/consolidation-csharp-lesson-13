
namespace Challenges.Tests;

using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using NUnit.Framework;

[TestFixture]
public class ChallengeTests
{
    private readonly Challenges Challenges = new();

    [SetUp]
    public void BeforeEach()
    {
        DeleteFileIfItExists("test1.txt");
        DeleteFileIfItExists("test2.txt");
        DeleteFileIfItExists("test3.txt");
    }

    [TestCase("test1.txt", "Hi", "Hello", 0)]
    [TestCase("test2.txt", "The wheels on the bus go round and round", "round", 2)]
    [TestCase("test3.txt", "The Antikythera mechanism is believed to be the earliest known mechanical analog computer", "the", 2)]
    public void TestCountInstancesOfWordInFile(string filepath, string fileContent, string word, int expectedCount)
    {
        // Arrange
        File.WriteAllText(filepath, fileContent);
        
        // Act
        var count = Challenges.CountInstancesOfWordInFile(filepath, word);
        
        // Assert
        Assert.AreEqual(expectedCount, count);

    }

    [TestCase("test1.txt", "The wheels on the bus go round and round")]
    [TestCase("test2.txt",
        "The Antikythera mechanism is believed to be the earliest known mechanical analog computer")]
    public void TestWriteTextToFile(string filepath, string text)
    {
        // Arrange
        
        // Act
        Challenges.WriteTextToFile(filepath, text);
        var readText = File.ReadAllText(filepath);
        
        // Assert
        Assert.AreEqual(text, readText);
    }
    
    [TestCase("test1.txt", "None", "None")]
    [TestCase("test2.txt", "Apple", "Atpple")]
    [TestCase("test3.txt", "Congraulations", "Congratulations")]
    public void TestWriteTAfterFirstA(string filepath, string text, string expectedText)
    {
        // Arrange
        File.WriteAllText(filepath, text);
        
        // Act
        Challenges.WriteTAfterFirstA(filepath);
        var readText = File.ReadAllText(filepath);
        
        // Assert
        Assert.AreEqual(expectedText, readText);
    }
    
    [TestCaseSource(nameof(RecordDataTestCases))]
    public void TestRecordData(string filepath, List<Dictionary<string, object>> data, string expectedContent)
    {
        // Arrange
        
        // Act
        Challenges.RecordData(filepath, data);
        var readText = File.ReadAllText(filepath);
        
        // Assert
        Assert.AreEqual(expectedContent, readText);
    }
    
    [TestCaseSource(nameof(FindFastestRunnerTestCases))]
    public void TestFindFastestRunnerUsingCsvHelper(string filepath, List<Dictionary<string, object>> data, string expectedRunner)
    {
        // Arrange
        WriteToCsv(filepath, data);

        // Act
        var fastestRunner = Challenges.FindFastestRunnerUsingCsvHelper(filepath);

        // Assert
        Assert.AreEqual(expectedRunner, fastestRunner);
    }
    
    [TestCaseSource(nameof(FindFastestRunnerTestCases))]
    public void TestFindFastestRunnerUsingDeedle(string filepath, List<Dictionary<string, object>> data, string expectedRunner)
    {
        // Arrange
        WriteToCsv(filepath, data);
        
        // Act
        var fastestRunner = Challenges.FindFastestRunnerUsingDeedle(filepath);
        
        // Assert
        Assert.AreEqual(expectedRunner, fastestRunner);
    }
    
    [TestCaseSource(nameof(FindMeanFinishTimeTestCases))]
    public void TestFindMeanFinishTimeUsingCsvHelper(string filepath, List<Dictionary<string, object>> data, float expectedMeanFinishTime)
    {
        // Arrange
        WriteToCsv(filepath, data);
        
        // Act
        var meanTime = Challenges.FindMeanFinishTimeUsingCsvHelper(filepath);

        // Assert
        Assert.AreEqual(expectedMeanFinishTime, meanTime);
    }
    
    [TestCaseSource(nameof(FindMeanFinishTimeTestCases))]
    public void TestFindMeanFinishTimeUsingDeedle(string filepath, List<Dictionary<string, object>> data, float expectedMeanFinishTime)
    {
        // Arrange
        WriteToCsv(filepath, data);
        
        // Act
        var meanTime = Challenges.FindMeanFinishTimeUsingDeedle(filepath);

        // Assert
        Assert.AreEqual(expectedMeanFinishTime, meanTime);
    }
    
    public static IEnumerable<TestCaseData> RecordDataTestCases()
    {
        yield return new TestCaseData
        (
            "test1.txt", 
            CreateData
            (
                CreateDict(("name", "Alice"), ("city", "London")),
                CreateDict(("name", "Bob"), ("city", "Houston")),
                CreateDict(("name", "Charlie"), ("city", "Kuala Lumpur"))
            ), 
            "name,city\nAlice,London\nBob,Houston\nCharlie,Kuala Lumpur\n"
        );
        
        yield return new TestCaseData
        (
            "test2.txt", 
            CreateData
            (
                CreateDict(("complete", 1), ("in progress", 0), ("not started", 5)),
                CreateDict(("complete", 4), ("in progress", 1), ("not started", 1))
            ), 
            "complete,in progress,not started\n1,0,5\n4,1,1\n"
        );
        
    }

    public static IEnumerable<TestCaseData> FindFastestRunnerTestCases()
    {
        yield return new TestCaseData
        (
            "test1.txt", 
            CreateData
            (
                CreateDict(("name", "Alice"), ("finish_time", 11.56))
            ), 
            "Alice"
        );
        
        yield return new TestCaseData
        (
            "test2.txt", 
            CreateData
            (
                CreateDict(("name", "Alice"), ("finish_time", 11.56)),
                CreateDict(("name", "Bob"), ("finish_time", 10.99)),
                CreateDict(("name", "Charlie"), ("finish_time", 17.02))
            ), 
            "Alice"
        );
        
        yield return new TestCaseData
        (
            "test3.txt", 
            CreateData
            (
                CreateDict(("name", "David"), ("finish_time", 14.2)),
                CreateDict(("name", "Eve"), ("finish_time", 13.7)),
                CreateDict(("name", "Frank"), ("finish_time", 14.4))
            ), 
            "Alice"
        );
    }

    public static IEnumerable<TestCaseData> FindMeanFinishTimeTestCases()
    {
        yield return new TestCaseData
        (
            "test1.txt", 
            CreateData
            (
                CreateDict(("name", "Alice"), ("finish_time", 11.56f))
            ), 
            11.56f
        );
        
        yield return new TestCaseData
        (
            "test2.txt", 
            CreateData
            (
                CreateDict(("name", "Alice"), ("finish_time", 11.56f)),
                CreateDict(("name", "Bob"), ("finish_time", 10.99f)),
                CreateDict(("name", "Charlie"), ("finish_time", 17.02f))
            ), 
            13.19f
        );
    }
    
    private static void DeleteFileIfItExists(string filepath)
    {
        try
        {
            File.Delete(filepath);
        }
        catch (DirectoryNotFoundException)
        {
            // We don't need to do anything
        }
    }

    private static List<Dictionary<string, object>> CreateData(params Dictionary<string, object>[] values)
    {
        return new List<Dictionary<string, object>>(values);
    }

    private static Dictionary<string, object> CreateDict(params (string, object)[] values)
    {
        return values.ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2);
    }

    private static void WriteToCsv(string filepath, List<Dictionary<string, object>> data)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            NewLine = "\n"
        };
        using var streamWriter = new StreamWriter(filepath);
        using var csvWriter = new CsvWriter(streamWriter, config);
        var headings = data.First().Keys.ToList();
        headings.ForEach(csvWriter.WriteField);
        csvWriter.NextRecord();
        data.ForEach(dictionary => WriteDictToCsv(headings, csvWriter, dictionary));
    }

    private static void WriteDictToCsv(List<string> headings, CsvWriter csvWriter, Dictionary<string, object> dictionary)
    {
        headings.ForEach(heading =>
        {
            csvWriter.WriteField(dictionary[heading]);
        });
        csvWriter.NextRecord();
    }
}
