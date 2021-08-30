using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using AmoaebaUtils;
public class CommandTestSuite : MonoBehaviour
{
    private CommandPool commandPool = new CommandPool();

    [SetUp]
    public void Setup()
    {
        commandPool = new CommandPool();
    } 

    [TearDown]
    public void Teardown()
    {
       commandPool = new CommandPool();
    }

    [UnityTest]
    public IEnumerator TestInitConditions()
    {
        Assert.IsTrue(commandPool.RequestedCommandCount() == 0, "Had unexpected requested commands");
        Assert.IsTrue(commandPool.AvailableCommandCount() == 0, "Had unexpected available commands");

        yield return null;
    }
}
