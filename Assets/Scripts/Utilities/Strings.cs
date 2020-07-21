using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Strings
{

    public static bool ContainsAllCharacters(this string str, string list)
    {
        bool contains = true;

        for (int i = 0; i < list.Length; i++)
        {
            if (!str.Contains(list[i].ToString()))
            {
                contains = false;
                break;
            }
        }

        return contains;
    }

}