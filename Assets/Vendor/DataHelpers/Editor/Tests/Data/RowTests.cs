using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using System.IO;

using DataHelpers;

[TestFixture]
public class RowTests { 

    // this should be moved to a test of the row itself...
    [Test]
    public void SetErrorMessage() {
        Row row = new Row();
        row.lineNumber = 1;
        row.SetErrorMessage("foo bar baz");

        Assert.AreEqual(false, row.valid, "node should be false");
        Assert.AreEqual("error line: 1: foo bar baz", row.errorMessage, "node should have error set");
    }
}
