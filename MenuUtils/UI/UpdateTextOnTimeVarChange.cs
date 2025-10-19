using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AmoaebaUtils
{
    
using TimeFunc = Func<bool, TimeSpan, double>;
using TimeTuple = Tuple<BooledString, Func<TimeSpan, double>>;

public class UpdateTextOnTimeVarChange : UpdateTextOnFloatVarChange
{    
    [SerializeField]
    private bool simplifyHighestValue = true;

    [SerializeField]
    private BooledString daysFormat = new BooledString(false, "{0:00}");

    [SerializeField]
    private BooledString hoursFormat = new BooledString(false, "{0:00}");

    [SerializeField]
    private BooledString minutesFormat = new BooledString(false, "{0:00}");

    [SerializeField]
    private BooledString secondsFormat = new BooledString(false, "{0:00}");

    [SerializeField]
    private BooledString millisecondsFormat = new BooledString(false, "{0:00}");

    private TimeTuple[] timeTupleAux;
    private List<string> timeComponents;

    protected override void Start()
    {
        timeTupleAux = new TimeTuple[]
                    {
                        new TimeTuple(daysFormat, (TimeSpan span) => span.Days),
                        new TimeTuple(hoursFormat, (TimeSpan span) => span.Hours),
                        new TimeTuple(minutesFormat, (TimeSpan span) => span.Minutes),
                        new TimeTuple(secondsFormat, (TimeSpan span) => span.Seconds),
                        new TimeTuple(millisecondsFormat, (TimeSpan span) => span.Milliseconds)
                    };
        timeComponents = new List<string>();
        base.Start();
    }
    protected override string GetText(float oldVal, float newVal)
    {
        string ret ="";
        
        TimeSpan timeSpan = TimeSpan.FromSeconds(var.Value);

        bool usedAny = false;
        timeComponents.Clear();
        
        for(int i = 0; i < timeTupleAux.Length; i++)
        {
            TimeTuple currentComponent = timeTupleAux[i];
            
            if(!currentComponent.Item1.check)
            {
                continue;
            }

            double timeValue = currentComponent.Item2(timeSpan);

            string actualFormat = simplifyHighestValue && !usedAny && timeValue < 10? "{0:0}" : currentComponent.Item1.value;
            timeComponents.Add(string.Format(actualFormat, timeValue));
            usedAny = true;
        }

        for(int i = 0; i < timeComponents.Count; i++)
        {
            ret += timeComponents[i];
            if(i < timeComponents.Count-1)
            {
                ret += format;
            }
        }
        
        return ret;
    }
}
}
