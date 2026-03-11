using UnityEngine;

namespace UI.Menus.Tabs
{
    public abstract class BaseDashboardTab
    {
        protected Vector2 scrollPos;
        
        public abstract void DrawTab(float x, float y, UITheme theme);

        protected void DrawSectionTitle(float x, ref float y, string title, UITheme theme)
        {
            GUI.Label(new Rect(x, y, 300, 20), title, theme.Value);
            y += 25;
        }

        protected void DrawScrollableArea(float x, ref float y, float width, float height, int itemCount, float rowHeight, System.Action<float> drawContent)
        {
            Rect scrollRect = new Rect(x, y, width, height);
            Rect viewRect = new Rect(0, 0, width - 20, itemCount * rowHeight);

            scrollPos = GUI.BeginScrollView(scrollRect, scrollPos, viewRect);
            drawContent?.Invoke(0);
            GUI.EndScrollView();
            
            y += height + 10;
        }
    }
}
