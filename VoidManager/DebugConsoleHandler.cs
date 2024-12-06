using HarmonyLib;
using IngameDebugConsole;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VoidManager
{
    internal class DebugConsoleHandler
    {
        private static MethodInfo addConsoleMethod = AccessTools.Method(typeof(DebugLogConsole), "AddCommand", new[] { typeof(string), typeof(string), typeof(MethodInfo), typeof(object) });
        internal static void DiscoverCommands(Assembly assembly, string ModName = "")
        {
            if (addConsoleMethod == null)
            {
                return;
            }
            int ConsoleMethodCount = 0;
            try
            {
                Type[] exportedTypes = assembly.GetExportedTypes();
                for (int k = 0; k < exportedTypes.Length; k++)
                {
                    foreach (MethodInfo methodInfo in exportedTypes[k].GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public))
                    {
                        object[] customAttributes = methodInfo.GetCustomAttributes(typeof(ConsoleMethodAttribute), false);
                        for (int m = 0; m < customAttributes.Length; m++)
                        {
                            ConsoleMethodAttribute consoleMethodAttribute = customAttributes[m] as ConsoleMethodAttribute;
                            if (consoleMethodAttribute != null)
                            {
                                ConsoleMethodCount++;
                                addConsoleMethod.Invoke(null, new object[] { consoleMethodAttribute.Command, consoleMethodAttribute.Description, methodInfo, new object() });
                            }
                        }
                    }
                }
            }
            catch (NotSupportedException)
            {
            }
            catch (FileNotFoundException)
            {
            }
            catch (Exception ex)
            {
                BepinPlugin.Log.LogInfo($"[{ModName}] Couldn't search assembly for console commands\n{ex.ToString()}");
            }
            if (ConsoleMethodCount > 0) BepinPlugin.Log.LogInfo($"[{ModName}] Detected {ConsoleMethodCount} console commands");
        }
    }
}
