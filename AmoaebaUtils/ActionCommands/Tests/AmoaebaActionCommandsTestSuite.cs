using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace AmoaebaUtils
{

public class AmoaebaActionCommandsTestSuite
{
    public class TestCommandObject : ActionCommandObject
    {

        public int weight = 0;
        public Action onAction = null;
        public Action onActionInterrupt = null;
        public bool canPerform = true;

        public TestCommandObject() {}
        
        protected override void OnAction() { onAction(); }
        protected override void OnActionInterrupt()  { onActionInterrupt(); }
        public override float GetWeight() { return weight; }
        public override bool CanPerformAction() { return canPerform; }
    }

    private ActionCommandScheduler scheduler;

    [SetUp]
    public void Setup()
    {
        scheduler = new ActionCommandScheduler();
    }

    [TearDown]
    public void Teardown()
    {
       scheduler = new ActionCommandScheduler();
    }

    [UnityTest]
    public IEnumerator TestInitConditions()
    {
        Assert.IsTrue(scheduler.GetCommandCount() == 0, "Unexpected amount of commands");
        Assert.IsTrue(scheduler.GetCommands().Length == 0, "Unexpected amount of commands");
        yield return null;
    }

    
    [UnityTest]
    public IEnumerator TestAddActions()
    {
        yield return TestInitConditions();

        TestCommandObject obj1 = new TestCommandObject();
        obj1.weight = 2;
        TestCommandObject obj2 = new TestCommandObject();
        obj2.weight = 3;
        TestCommandObject obj3 = new TestCommandObject();
        obj3.weight = 1;
        TestCommandObject obj4 = new TestCommandObject();
        obj4.weight = 2;


        scheduler.AddActionCommand(obj1);
        scheduler.AddActionCommand(obj2);
        scheduler.AddActionCommand(obj3);
        scheduler.AddActionCommand(obj4);

        ActionCommandObject[] commands = scheduler.GetCommands();
        Assert.IsTrue(scheduler.GetCommandCount() == 4, " Unexpected command amount");
        Assert.IsTrue(commands.Length == 4, " Unexpected command amount");
        Assert.IsTrue(commands[0] == obj3, " Unexpected command at index 0");
        Assert.IsTrue(commands[1] == obj1, " Unexpected command at index 1");
        Assert.IsTrue(commands[2] == obj4, " Unexpected command at index 2");
        Assert.IsTrue(commands[3] == obj2, " Unexpected command at index 3");
    }
    
    [UnityTest]
    public IEnumerator TestClearActions()
    {
        yield return TestAddActions();
        Assert.IsTrue(scheduler.GetCommandCount() == 4, " Unexpected command amount");

        scheduler.ClearActions();

        ActionCommandObject[] commands = scheduler.GetCommands();
        Assert.IsTrue(scheduler.GetCommandCount() == 0, " Unexpected command amount");
        Assert.IsTrue(commands.Length == 0, " Unexpected command amount");
    }

    [UnityTest]
    public IEnumerator TestPerformActions()
    {
        yield return TestInitConditions();
        TestCommandObject obj1 = new TestCommandObject();
        obj1.weight = 2;
        obj1.canPerform = true;
        TestCommandObject obj2 = new TestCommandObject();
        obj2.weight = 3;
        obj2.canPerform = true;
        TestCommandObject obj3 = new TestCommandObject();
        obj3.weight = 1;
        obj3.canPerform = true;

        bool[] didInterrupt = new bool[3];
        int[] res = new int[3];
        for(int i = 0; i < 3; i ++)
        {
            didInterrupt[i] = false;
            res[i] = 0;
        }

        int index = 0;
        int change = 0;
        bool didEnd = false;
        int secondsDelay = 2;

        scheduler.AddActionCommand(obj1);
        scheduler.AddActionCommand(obj2);
        scheduler.AddActionCommand(obj3);

        obj1.onAction = () => {res[index-1] = 10; };
        obj2.onAction = () => { Task.Delay(secondsDelay*1000).Wait(); res[index-1] = -20; };
        obj3.onAction = () => { res[index-1] = 100; };

        obj1.onActionInterrupt = () => { didInterrupt[index-1] = true; };
        obj2.onActionInterrupt = () => { didInterrupt[index-1] = true; };
        obj3.onActionInterrupt = () => { didInterrupt[index-1] = true; };

        scheduler.OnCommandWillStartEvent += (ActionCommandObject command) => 
        {
            index++;
        };

        scheduler.OnCountChangeEvent += () => {
            change++;
        };

        scheduler.OnCommandDidFinishEvent += (ActionCommandObject command) => 
        {
            if(index == 1)
            {
                Assert.IsTrue(command == obj3, "Reached Conclusion with wrong index");
                Assert.IsTrue(res[index-1] == 100, "Wrong action result");
            } 
            else if (index == 2)
            {
                Assert.IsTrue(command == obj1, "Reached Conclusion with wrong index");
                Assert.IsTrue(res[index-1] == 10, "Wrong action result");
            }
            else
            {
                Assert.IsTrue(command == obj2, "Reached Conclusion with wrong index");
                Assert.IsTrue(res[index-1] == -20, "Wrong action result");
            }
        };

        scheduler.OnPerformEndEvent += () =>
        {
            didEnd = true;
        };
        
        scheduler.PerformAllActions();

        yield return TestUtils.WaitTime(secondsDelay*2);

        for(int i = 0; i < 3; i++)
        {
            Assert.IsFalse(didInterrupt[i], "Unexpected interrupt");
        }

        Assert.IsTrue(index == 3, "Reached Conclusion with wrong index");
        Assert.IsTrue(change == 3, "Reached Conclusion with wrong index");
        Assert.IsTrue(res[0] == 100, "Reached Conclusion with wrong index");
        Assert.IsTrue(res[1] == 10, "Reached Conclusion with wrong index");
        Assert.IsTrue(res[2] == -20, "Reached Conclusion with wrong index");
        Assert.IsTrue(didEnd, "Reached Conclusion without end callback");
        Assert.IsTrue(scheduler.GetCommandCount() == 0, "Commands to be executed");
    }

    
    [UnityTest]
    public IEnumerator TestInterruptActions()
    {
         yield return TestInitConditions();
        TestCommandObject obj1 = new TestCommandObject();
        obj1.weight = 1;
        obj1.canPerform = false;
        TestCommandObject obj2 = new TestCommandObject();
        obj2.weight = 2;
        obj2.canPerform = true;
        TestCommandObject obj3 = new TestCommandObject();
        obj3.weight = 3;
        obj3.canPerform = false;
        TestCommandObject obj4 = new TestCommandObject();
        obj4.weight = 4;
        obj4.canPerform = true;
        

        bool[] didInterrupt = new bool[4];
        int[] res = new int[4];
        for(int i = 0; i < 4; i ++)
        {
            didInterrupt[i] = false;
            res[i] = 0;
        }

        int index = 0;
        int change = 0;
        bool didEnd = false;
        int secondsDelay = 2;

        scheduler.AddActionCommand(obj1);
        scheduler.AddActionCommand(obj2);
        scheduler.AddActionCommand(obj3);
        scheduler.AddActionCommand(obj4);

        obj1.onAction = () => {res[index-1] = 10; };
        obj2.onAction = () => { Task.Delay(secondsDelay*1000).Wait(); res[index-1] = -20; };
        obj3.onAction = () => { res[index-1] = 100; };
        obj4.onAction = () => { res[index-1] = 880; };

        obj1.onActionInterrupt = () => { didInterrupt[index-1] = true; };
        obj2.onActionInterrupt = () => { didInterrupt[index-1] = true; };
        obj3.onActionInterrupt = () => { didInterrupt[index-1] = true; };
        obj4.onActionInterrupt = () => { didInterrupt[index-1] = true; };

        scheduler.OnCommandWillStartEvent += (ActionCommandObject command) => 
        {
            index++;
        };

        scheduler.OnCountChangeEvent += () => {
            change++;
        };

        scheduler.OnCommandDidFinishEvent += (ActionCommandObject command) => 
        {
            if (index == 2)
            {
                Assert.IsTrue(command == obj2, "Reached Conclusion with wrong index");
                Assert.IsTrue(res[index-1] == -20, "Wrong action result");
            }
            else if(index == 4)
            {
                Assert.IsTrue(command == obj4, "Reached Conclusion with wrong index");
                Assert.IsTrue(res[index-1] == 880, "Wrong action result");
            }
            else
            {
                Assert.IsTrue(res[index-1] == 0, "Wrong action result");
            }
        };

        scheduler.OnPerformEndEvent += () =>
        {
            didEnd = true;
        };
        
        scheduler.PerformAllActions();

        yield return TestUtils.WaitTime(secondsDelay*2);

        Assert.IsTrue(didInterrupt[0], "Unexpected interrupt");
        Assert.IsFalse(didInterrupt[1], "Unexpected interrupt");
        Assert.IsTrue(didInterrupt[2], "Unexpected interrupt");
        Assert.IsFalse(didInterrupt[3], "Unexpected interrupt");

        Assert.IsTrue(index == 4, "Reached Conclusion with wrong index");
        Assert.IsTrue(change == 4, "Reached Conclusion with wrong index");
        Assert.IsTrue(res[0] == 0, "Reached Conclusion with wrong index");
        Assert.IsTrue(res[1] == -20, "Reached Conclusion with wrong index");
        Assert.IsTrue(res[2] == 0, "Reached Conclusion with wrong index");
        Assert.IsTrue(res[3] == 880, "Reached Conclusion with wrong index");
        Assert.IsTrue(didEnd, "Reached Conclusion without end callback");
        Assert.IsTrue(scheduler.GetCommandCount() == 0, "Commands to be executed");   
    }
}
}

