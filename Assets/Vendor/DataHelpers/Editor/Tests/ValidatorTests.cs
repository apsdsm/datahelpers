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
        Validator validator = new Validator();

        ParsableRow row = new ParsableRow();
        row.cells = new string[2] { "foo", "bar" };

        ReadBundle rb = new ReadBundle();
        rb.fieldNames.Add("Foo");
        rb.fieldNames.Add("Bar");
        rb.rows.Add(row);

        validator.AddParsableRows(rb);

        Assert.AreEqual(1, validator.Nodes.Length, "should have added the row to the validation chain");
    }

    [Test]
    public void PassCheckableNodesToUserValidator()
    {
        Validator validator = new Validator();

        ParsableRow row = new ParsableRow();
        row.cells = new string[2] { "foo", "bar" };

        ReadBundle rb = new ReadBundle();
        rb.fieldNames.Add("Foo");
        rb.fieldNames.Add("Bar");
        rb.rows.Add(row);

        validator.AddParsableRows(rb);

        IValidator userValidator = Substitute.For<IValidator>();

        validator.IsValid(rb, userValidator);

        userValidator.Received().Validate(validator.Nodes[0], validator);
    }

    [Test]
    public void SetErrorMessage()
    {
        Validator validator = new Validator();

        ValidatorNode node = new ValidatorNode();
        node.lineNumber = 1;


        validator.SetErrorMessage(node, "foo bar baz");

        Assert.AreEqual(false, node.valid, "node should be false");
        Assert.AreEqual("error line: 1: foo bar baz", node.message, "node should have error set");
    }
}
