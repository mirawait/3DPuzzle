using UnityEngine;
using System.Collections;

public class TextTable
{

    public static string[] GetLines(string id)
    {
        ArrayList lines = new ArrayList();
        string line;
        TextAsset textFile = Resources.Load("UI/PlanetInfo/PlanetInfo") as TextAsset;
        System.IO.StringReader textStream = new System.IO.StringReader(textFile.text);
        string lineID = "[" + id + "]";
        bool match = false;
        while ((line = textStream.ReadLine()) != null)
        {
            if (match)
            {
                if (line.StartsWith("["))
                {
                    break;
                }
                if (line.Length > 0)
                {
                    lines.Add(line);
                }
            }
            else if (line.StartsWith(lineID))
            {
                match = true;
                lines.Add(id);
            }
        }
        textStream.Close();
        if (lines.Count > 0)
        {
            return (string[])lines.ToArray(typeof(string));
        }
        return new string[0];
    }
}