using UnityEngine;
using System.Collections;
using System.Diagnostics;

public abstract class Trigger {
    public static string triggerType = (new StackFrame()).GetMethod().DeclaringType.ToString();

    public abstract string getType();
}
