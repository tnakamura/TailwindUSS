using System;
using UnityEngine;
using UnityEngine.UIElements;

public sealed class TailwindSampleNavigation : MonoBehaviour
{
    private static readonly string[] ScreenNames =
    {
        "HomeScreenContainer",
        "LessonScreenContainer",
        "ShopScreenContainer",
        "MenuScreenContainer"
    };

    private static readonly string[] TabButtonNames =
    {
        "HomeTabButton",
        "LessonTabButton",
        "ShopTabButton",
        "MenuTabButton"
    };

    private static readonly string[] Titles =
    {
        "Home",
        "Lessons",
        "Shop",
        "Menu"
    };

    private static readonly string[] Subtitles =
    {
        "Check today's highlights",
        "Resume your active courses",
        "Find useful items",
        "Access settings and support"
    };

    private VisualElement[] screens;
    private Button[] tabButtons;
    private Action[] clickHandlers;
    private Label appBarTitle;
    private Label appBarSubtitle;

    private void OnEnable()
    {
        if (!HasConsistentConfiguration())
        {
            return;
        }

        var document = GetComponent<UIDocument>();
        if (document == null)
        {
            return;
        }

        var root = document.rootVisualElement;
        if (root == null)
        {
            return;
        }

        appBarTitle = root.Q<Label>("AppBarTitle");
        appBarSubtitle = root.Q<Label>("AppBarSubtitle");
        screens = new VisualElement[ScreenNames.Length];
        tabButtons = new Button[TabButtonNames.Length];
        clickHandlers = new Action[TabButtonNames.Length];

        for (var i = 0; i < ScreenNames.Length; i++)
        {
            screens[i] = root.Q<VisualElement>(ScreenNames[i]);
            tabButtons[i] = root.Q<Button>(TabButtonNames[i]);

            if (screens[i] == null || tabButtons[i] == null)
            {
                continue;
            }

            var index = i;
            clickHandlers[i] = () => ShowScreen(index);
            tabButtons[i].clicked += clickHandlers[i];
        }

        ShowScreen(0);
    }

    private void OnDisable()
    {
        if (tabButtons == null || clickHandlers == null)
        {
            return;
        }

        for (var i = 0; i < tabButtons.Length; i++)
        {
            if (tabButtons[i] != null && clickHandlers[i] != null)
            {
                tabButtons[i].clicked -= clickHandlers[i];
            }
        }
    }

    private void ShowScreen(int selectedIndex)
    {
        if (!HasConsistentConfiguration() ||
            screens == null ||
            tabButtons == null ||
            selectedIndex < 0 ||
            selectedIndex >= ScreenNames.Length)
        {
            return;
        }

        for (var i = 0; i < ScreenNames.Length; i++)
        {
            var isSelected = i == selectedIndex;
            if (screens[i] != null)
            {
                screens[i].EnableInClassList("hidden", !isSelected);
            }

            if (tabButtons[i] != null)
            {
                UpdateTabButton(tabButtons[i], isSelected);
            }
        }

        if (appBarTitle != null)
        {
            appBarTitle.text = Titles[selectedIndex];
        }

        if (appBarSubtitle != null)
        {
            appBarSubtitle.text = Subtitles[selectedIndex];
        }
    }

    private static void UpdateTabButton(Button button, bool isSelected)
    {
        button.EnableInClassList("bg-brand", isSelected);
        button.EnableInClassList("border-brand", isSelected);
        button.EnableInClassList("text-white", isSelected);
        button.EnableInClassList("bg-white", !isSelected);
        button.EnableInClassList("border-slate-300", !isSelected);
        button.EnableInClassList("text-slate-500", !isSelected);

        foreach (var label in button.Query<Label>().ToList())
        {
            label.EnableInClassList("text-white", isSelected);
            label.EnableInClassList("text-slate-500", !isSelected);
        }
    }

    private static bool HasConsistentConfiguration()
    {
        return ScreenNames.Length == TabButtonNames.Length &&
               ScreenNames.Length == Titles.Length &&
               ScreenNames.Length == Subtitles.Length;
    }
}
