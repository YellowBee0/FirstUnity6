using UnityEngine;
using UnityEngine.UIElements;

namespace YBFramework.MyEditor.Common
{
    public static class VisualElementStyle
    {
        public static void CommonBorder(this VisualElement element)
        {
            element.style.paddingTop = 4;
            element.style.paddingBottom = 4;
            element.style.borderTopWidth = 1;
            element.style.borderBottomWidth = 1;
            element.style.borderTopColor = new Color(0.2f, 0.2f, 0.2f);
            element.style.borderBottomColor = new Color(0.2f, 0.2f, 0.2f);
        }
    }
}