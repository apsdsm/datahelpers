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

    [Test]
    public void AsFloat() {
        var f = new Field { value = "1.0" };

        Assert.AreEqual(1.0f, f.AsFloat, "it should provide the field value as a float");
    }

    [Test]
    public void AsDouble() {
        var f = new Field { value = "1.0" };

        Assert.AreEqual(1.0, f.AsFloat, "it should provide the field value as a double");
    }

    [Test]
    public void IsEmpty() {
        var f = new Field();

        Assert.True(f.IsEmpty(), "it should return true when value is null");

        f.value = "";

        Assert.True(f.IsEmpty(), "it should return true when value is empty string.");

        f.value = "foo";

        Assert.False(f.IsEmpty(), "it should return false when there is value");
    }

    [Test]
    public void IsValidResource() {
        var f = new Field() { value = "__DataHelpers__TestAsset__" };

        Assert.True(f.IsValidResource(), "it should return true when there is a valid resource.");

        f.value = "__DataHelpers__NotARealAsset__";

        Assert.False(f.IsValidResource(), "it should return false when there is no valid resource.");
    }
}
