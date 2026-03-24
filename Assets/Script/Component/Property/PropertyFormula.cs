using System;
using System.Collections.Generic;

namespace YBFramework.Component
{
    public static class PropertyFormula
    {
        private static readonly unsafe Dictionary<string, IntPtr> s_Formulas = new()
        {
            { nameof(CommonDamage), (IntPtr)(delegate*<PropertyManager, bool, ref float, void>)&CommonDamage },
            { nameof(CommonDefense), (IntPtr)(delegate*<PropertyManager, bool, ref float, void>)&CommonDefense },
            { nameof(CommonIncreaseDamage), (IntPtr)(delegate*<PropertyManager, bool, ref float, void>)&CommonIncreaseDamage }
        };

        public static IntPtr GetFormula(string name)
        {
            return s_Formulas.GetValueOrDefault(name);
        }

        private static void CommonDamage(PropertyManager propertyManager, bool increase, ref float value)
        {
            Property property = propertyManager.GetProperty(PropertyType.Attack);
            if (property != null)
            {
                value += increase ? property.GetCurValue() : -property.GetCurValue();
            }
        }

        private static void CommonDefense(PropertyManager propertyManager, bool increase, ref float value)
        {
            Property property = propertyManager.GetProperty(PropertyType.Defense);
            if (property != null)
            {
                value += increase ? property.GetCurValue() : -property.GetCurValue();
            }
        }

        private static void CommonIncreaseDamage(PropertyManager propertyManager, bool increase, ref float value)
        {
            Property property = propertyManager.GetProperty(PropertyType.Attack);
            if (property != null)
            {
                value *= 1 + ((value > 0) ^ increase ? -property.GetCurValue() : property.GetCurValue());
            }
        }
    }
}