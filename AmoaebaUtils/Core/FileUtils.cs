using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public class FileUtils
{
    public static void GenerateFoldersForPath(string path)
    {
        string[] paths = path.Split('/');
        string subPath ="";
        for(int i = 0; i < paths.Length; i++)
        {
            subPath += paths[i];
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }

    public static void GenerateClass(string className,
                                     string classNamespace,
                                     string folderPath,
                                     Action<StreamWriter> writeBody)
    {
        string filename = folderPath;
        filename += folderPath.EndsWith("/")? "" : "/";
        filename += className + ".cs";

        if(File.Exists(filename))
        {
            File.Delete(filename);
        }

        StreamWriter outfile = new StreamWriter(filename);
        if(outfile == null)
        {
            return;
        }
        
        WriteClassHeader(outfile, className, classNamespace);
        writeBody?.Invoke(outfile);
        WriteClassFooter(outfile);
        outfile.Close();
        
        return;
    }

    private static void WriteClassHeader(StreamWriter outfile, string className, string classNamespace)
    {
        outfile.WriteLine("using UnityEngine;");
        outfile.WriteLine("using System;");
        outfile.WriteLine("using System.Collections;");
        outfile.WriteLine("");
        outfile.WriteLine("");
        outfile.WriteLine("// !!!!! WARNING !!!!!");
        outfile.WriteLine("// Auto Generated Class --- DO NOT CHANGE ---");
        outfile.WriteLine("");
        outfile.WriteLine("");
        outfile.WriteLine("namespace "+ classNamespace + " {");
        outfile.WriteLine("public class "+ className +" : ScriptableObject {");
        outfile.WriteLine(" ");
    }

    private static void WriteClassFooter(StreamWriter outfile)
    {
        outfile.WriteLine(" }");
        outfile.WriteLine("}");
    }
}
}