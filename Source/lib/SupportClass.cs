//
// In order to convert some functionality to Visual C#, the Java Language Conversion Assistant
// creates "support classes" that duplicate the original functionality.  
//
// Support classes replicate the functionality of the original code, but in some cases they are 
// substantially different architecturally. Although every effort is made to preserve the 
// original architecture of the application in the converted project, the user should be aware that 
// the primary goal of these support classes is to replicate functionality, and that at times 
// the architecture of the resulting solution may differ somewhat.
//

using System;
using System.Collections.Generic;
using System.Text;

namespace ZXing
{
   /// <summary>
   /// Contains conversion support elements such as classes, interfaces and static methods.
   /// </summary>
   public static class SupportClass
   {
      /*******************************/
      /// <summary>
      /// Copies an array of chars obtained from a String into a specified array of chars
      /// </summary>
      /// <param name="sourceString">The String to get the chars from</param>
      /// <param name="sourceStart">Position of the String to start getting the chars</param>
      /// <param name="sourceEnd">Position of the String to end getting the chars</param>
      /// <param name="destinationArray">Array to return the chars</param>
      /// <param name="destinationStart">Position of the destination array of chars to start storing the chars</param>
      /// <returns>An array of chars</returns>
      public static void GetCharsFromString(System.String sourceString, int sourceStart, int sourceEnd, char[] destinationArray, int destinationStart)
      {
         int sourceCounter = sourceStart;
         int destinationCounter = destinationStart;
         while (sourceCounter < sourceEnd)
         {
            destinationArray[destinationCounter] = (char)sourceString[sourceCounter];
            sourceCounter++;
            destinationCounter++;
         }
      }

      /*******************************/
      /// <summary>
      /// Sets the capacity for the specified List
      /// </summary>
      /// <param name="vector">The List which capacity will be set</param>
      /// <param name="newCapacity">The new capacity value</param>
      public static void SetCapacity<T>(System.Collections.Generic.IList<T> vector, int newCapacity) where T : new()
      {
         while (newCapacity > vector.Count)
            vector.Add(new T());
         while (newCapacity < vector.Count)
            vector.RemoveAt(vector.Count - 1);
      }

      /// <summary>
      /// Converts a string-Collection to an array
      /// </summary>
      /// <param name="strings">The strings.</param>
      /// <returns></returns>
      public static String[] toStringArray(ICollection<string> strings)
      {
         var result = new String[strings.Count];
         strings.CopyTo(result, 0);
         return result;
      }

      /// <summary>
      /// Joins all elements to one string.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="separator">The separator.</param>
      /// <param name="values">The values.</param>
      /// <returns></returns>
      public static string Join<T>(string separator, IEnumerable<T> values)
      {
         var builder = new StringBuilder();
         separator = separator ?? String.Empty;
         if (values != null)
         {
            foreach (var value in values)
            {
               builder.Append(value);
               builder.Append(separator);
            }
            if (builder.Length > 0)
               builder.Length -= separator.Length;
         }

         return builder.ToString();
      }
   }
}