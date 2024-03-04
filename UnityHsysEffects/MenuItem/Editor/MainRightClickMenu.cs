using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/*public class MainRightClickMenu : EditorWindow
{
    [@MenuItem("Game/Open Window")]
    static void Init()
    {
        EditorWindow window = new EditorWindow();
        window.position = new Rect(50, 50, 250, 60);
        window.Show();
    }
    private void Callback(Object obj)
    {
        Debug.Log("Selected: " + obj);
    }
    private void OnGUI()
    {
        Event evt = Event.current;
        Rect contextRect = new Rect(10, 10, 100, 100);

        if (evt.type == EventType.ContextClick)
        {
            Vector2 mousePos = evt.mousePosition;
            if (contextRect.Contains(mousePos))
            {
                // Now create the menu, add items and show it
                GenericMenu menu = new GenericMenu();

                menu.AddItem(new GUIContent("MenuItem1"), false, null, "item 1");
                menu.AddItem(new GUIContent("MenuItem2"), false, null, "item 2");
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("SubMenu/MenuItem3"), false, null, "item 3");

                menu.ShowAsContext();

                evt.Use();
            }
        }

    }
}*/
