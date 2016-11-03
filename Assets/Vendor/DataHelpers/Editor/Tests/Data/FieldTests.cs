using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using System.IO;

using DataHelpers;

[TestFixture]
public class FieldTests {

    [Test]
    public void AsInt() {
        var f = new Field() { value = "1" };

        Assert.AreEqual(1, f.AsInt, "it should provide the field value as an int");
    }

    [Test]
    public void AsBool() {
        var f = new Field { value = "true" };

        Assert.True(f.AsBool, "it should provide the field value as a bool");
    }
}
