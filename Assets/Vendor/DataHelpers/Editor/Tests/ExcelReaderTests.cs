using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System.IO;



[TestFixture]
public class ExcelReaderTests {

	[Test]
	public void ReadXlsxTest()
	{
        ExcelReader reader = new ExcelReader();
        ReadBundle readBundle = new ReadBundle();
        string testDataLocation = "/Assets/Vendor/DataHelpers/Editor/Tests/Fixtures/TestData.xlsx";
        
        ExcelReader.ReadXLSX(testDataLocation, ref readBundle);
        
        Assert.AreEqual(3, readBundle.fieldNames.Count, "it should load 3 field names");
        Assert.AreEqual(2, readBundle.rows.Count, "it should load 2 rows of data");
	}
}
