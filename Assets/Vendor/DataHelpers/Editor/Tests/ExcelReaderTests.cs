using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System.IO;

using DataHelpers;
using DataHelpers.Readers;

[TestFixture]
public class ExcelReaderTests {

	[Test]
	public void ReadXlsxTest()
	{
        ExcelReader reader = new ExcelReader();
        ImportData data = new ImportData();
        string testDataLocation = "/Assets/Vendor/DataHelpers/Editor/Tests/Fixtures/TestData.xlsx";
        
        reader.ReadAsset(testDataLocation, ref data);

        Assert.AreEqual(2, data.vars.Count, "it should load 2 variables");
        Assert.AreEqual("100", data.vars["foo1"]);
        Assert.AreEqual("200", data.vars["foo2"]);

        Assert.AreEqual(2, data.meta.Count, "it should load 2 meta values");
        Assert.AreEqual("foo author", data.meta["author"]);
        Assert.AreEqual("foo description", data.meta["description"]);

        Assert.AreEqual(3, data.fieldNames.Count, "it should load 3 field names");      
        Assert.AreEqual("Foo", data.fieldNames[0]);
        Assert.AreEqual("Bar", data.fieldNames[1]);
        Assert.AreEqual("Baz", data.fieldNames[2]);

        Assert.AreEqual(2, data.rows.Count, "it should load 2 rows of data");
        Assert.AreEqual("1", data.rows[0].fields["Foo"].value);
        Assert.AreEqual("2", data.rows[0].fields["Bar"].value);
        Assert.AreEqual("3", data.rows[0].fields["Baz"].value);
        Assert.AreEqual("4", data.rows[1].fields["Foo"].value);
        Assert.AreEqual("5", data.rows[1].fields["Bar"].value);
        Assert.AreEqual("6", data.rows[1].fields["Baz"].value);
    }
}
