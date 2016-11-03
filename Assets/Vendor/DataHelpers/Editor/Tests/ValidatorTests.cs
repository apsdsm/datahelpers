using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using System.IO;

using DataHelpers.Interfaces;

[TestFixture]
public class ValidatorTests
{
    [Test]
    public void AddParsableNodesToValidatorChain()
    {
        ValidationRunner validator = new ValidationRunner();

        ParsableRow row = new ParsableRow();
        row.cells = new string[2] { "foo", "bar" };

        ImportData rb = new ImportData();
        rb.fieldNames.Add("Foo");
        rb.fieldNames.Add("Bar");
        rb.rows.Add(row);

        validator.AddParsableRows(rb);

        Assert.AreEqual(1, validator.Nodes.Length, "should have added the row to the validation chain");
    }

    [Test]
    public void PassCheckableNodesToUserValidator()
    {
        ValidationRunner validator = new ValidationRunner();

        ParsableRow row = new ParsableRow();
        row.cells = new string[2] { "foo", "bar" };

        ImportData rb = new ImportData();
        rb.fieldNames.Add("Foo");
        rb.fieldNames.Add("Bar");
        rb.rows.Add(row);

        validator.AddParsableRows(rb);

        IValidator userValidator = Substitute.For<IValidator>();

        validator.IsValid(rb, userValidator);

        userValidator.Received().Validate(validator.Nodes[0], validator);
    }

    // this should be moved to a test of the row itself...
    [Test]
    public void SetErrorMessage()
    {
        ValidationRunner validator = new ValidationRunner();

        ValidatorRow node = new ValidatorRow();
        node.lineNumber = 1;
        node.SetErrorMessage("foo bar baz");

        Assert.AreEqual(false, node.valid, "node should be false");
        Assert.AreEqual("error line: 1: foo bar baz", node.errorMessage, "node should have error set");
    }
}
