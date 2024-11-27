/*=============================================================================
**
** Purpose: Exception class for null arguments to a method.
** .NetFramework 4.6.2 ArgumentNullException does not implement the ThrowIfNull method.
**
=============================================================================*/

using System.Collections;
using System.Reflection;
using System.Runtime.Serialization;

namespace DataverseLogic;

public static class ArgumentNullExceptionHelper
{
    public static void ThrowIfNull(object? value, string paramName)
    {
        if (value == null)
        {
            throw new ArgumentNullException(paramName);
        }
    }

    public static void ThrowIfNull(object? value, string paramName, string message)
    {
        if (value == null)
        {
            throw new ArgumentNullException(paramName, message);
        }
    }
}