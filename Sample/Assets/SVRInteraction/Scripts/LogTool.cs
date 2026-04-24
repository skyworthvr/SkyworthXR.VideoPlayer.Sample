/*
 * @Author: xieminghui
 * @Date: 2020-12-01 13:55:27
 * @Description: log打印工具，只有release分支不打印，其他分支包括master分支都会打印
 * @LastEditors: xieminghui
 * @LastEditTime: 2020-12-02 14:18:51
 * @Copyright: Copyright 2020 Skyworth VR. All rights reserved.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class LogTool : MonoBehaviour
{
    private const string DebugKey = "ENABLE_LOG";


    [System.Diagnostics.Conditional(DebugKey)]
    public static void Log(string str)
    {

        Debug.Log(str);
    }
    [System.Diagnostics.Conditional(DebugKey)]
    public static void Log(string tag, string log)
    {

        Debug.Log(log);
    }
    [System.Diagnostics.Conditional(DebugKey)]
    public static void LogFormat(string format, params object[] args)
    {
        Debug.LogFormat(format, args);
    }
    [System.Diagnostics.Conditional(DebugKey)]
    public static void Error(string str)
    {
        Debug.LogError(str);
    }
    [System.Diagnostics.Conditional(DebugKey)]
    public static void ErrorFormat(string format, params object[] args)
    {
        Debug.LogErrorFormat(format,args);
    }
    [System.Diagnostics.Conditional(DebugKey)]
    public static void Error(string tag, string log)
    {

        Debug.LogError(log);
    }
}
