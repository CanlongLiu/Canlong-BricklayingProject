using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LovattoCountdown.TutorialWizard;
using UnityEditor;

public class CountdownDoc : TutorialWizard
{
    //required//////////////////////////////////////////////////////
    public string FolderPath = "countdown/editor/";
    public NetworkImages[] m_ServerImages = new NetworkImages[]
    {
        new NetworkImages{Name = "img-0.png", Image = null},
        new NetworkImages{Name = "img-1.png", Image = null},
        new NetworkImages{Name = "img-2.png", Image = null},
        new NetworkImages{Name = "img-3.png", Image = null},
        new NetworkImages{Name = "img-4.png", Image = null},
    };
    public Steps[] AllSteps = new Steps[] {
    new Steps { Name = "Get Started", StepsLenght = 0 , DrawFunctionName = nameof(GetStartedDoc)},    
    new Steps { Name = "Text Mesh Pro", StepsLenght = 0, DrawFunctionName = nameof(TextMeshProDoc) },
    new Steps { Name = "Events", StepsLenght = 0, DrawFunctionName = nameof(EventsDoc) },
    new Steps { Name = "bl_Countdown", StepsLenght = 0, DrawFunctionName = nameof(SettingsDoc) },
    new Steps { Name = "bl_CountdownUI", StepsLenght = 0, DrawFunctionName = nameof(CountdownUIDoc) },
    };
    private readonly GifData[] AnimatedImages = new GifData[]
   {
        new GifData{ Path = "name.gif" },
   };

    public override void OnEnable()
    {
        base.OnEnable();
        base.Initizalized(m_ServerImages, AllSteps, FolderPath, AnimatedImages);
        Style.highlightColor = ("#c9f17c").ToUnityColor();
        allowTextSuggestions = true;
    }

    public override void WindowArea(int window)
    {
        AutoDrawWindows();
    }

    //final required////////////////////////////////////////////////

    void GetStartedDoc()
    {
        DrawHyperlinkText("Use the Countdown system is really easy, you simply have to drag and drop one of the countdown prefabs in one of your scene Canvas, and then with one line of code, you start the countdown in runtime.\n \nHow would you integrate or implements depends on your project, but it can be as simple as adding a callback to call after countdown finish, no code needed, simply add the function to call in the inspector of the bl_Countdown script.\n \nHere a simple guide to integrating it:\n \n<b><size=14>1. INSTANCE THE COUNTDOWN PREFAB.</size></b>\n\nSelect one of the ready-made countdown prefabs located in the folder <i>Assets -> Countdown -> Prefabs -> Countdowns->*</i> or click <link=asset:Assets/Countdown/Prefabs/Countdowns/Countdown [Custom 1].prefab>here</link> to ping the folder, <b>if you want to use the Text Mesh Pro</b> prefabs use the ones in the folder \"<b>Countdowns TMP</b>\" instead.\n \nOnce you selected it, drag and drop it inside a <b>Canvas</b> in your scene <i>(if you haven't one yet, create one by right click in the hierarchy window -> UI -> Canvas)</i>.");
        DrawServerImage(0);
        DrawText("<b><size=14>2. Call to start the countdown</size></b>\n \nDepending on your project and needs, you to decide when and how you wanna start the countdown, here a few examples in common scenarios:\n \n<b>- At the start of a scene:</b>\nIf you wanna start the countdown at the start of the scene, you can simply add this line of code in a <b>Start()</b> function in any script attached in your scene:");
        DrawCodeText("//reference to the countdown assigned in the inspector\n        public bl_Countdown countdown;\n \n        private void Start()\n        {\n            //start a countdown from 10 to 0\n            countdown.StartCountdown(10);\n        }");
        Space(10);
        DrawText("<b>-After clicking a button</b>\nIf you wanna start the countdown after the player clicks a button e.g a \"<i>Start Game</i>\" button, you can simply add a listener to that button pointing to the <b>Countdown instance</b> -> bl_Countdown -> StartCountdown(int count) -> set the start value.");
        DrawServerImage(1);
    }

    void TextMeshProDoc()
    {
        DrawHyperlinkText("This package includes support for both Unity UI text solutions <b>(UGUI and Text Mesh Pro)</b>, the asset includes a countdown prefab for each system with exactly the same design and functionality with the only difference being the Text component.\n \n<b>BUT</b> <b>by default, you only will find the UGUI prefabs</b>, this in order to avoid errors after the importation due to missing the TMP package.\n \nSo in order to use the Text Mesh Pro prefabs, first make sure you have imported and set up the Text Mesh Pro package from the Unity Package Manager, if you don't know how to do this or verify, check this: <link=https://learn.unity.com/tutorial/textmesh-pro-importing-the-package>https://learn.unity.com/tutorial/textmesh-pro-importing-the-package</link>");
        DownArrow();
        DrawText("Once you have made sure to have the Text Mesh Pro imported and ready, you have to import the Countdown TMP assets, for this, simply go to the root folder of the asset <b>(Assets ➔ Countdown ➔ *)</b> ➔ there you will see a unitypackage called <b>Countdown TMP</b> ➔ double click on it and import all the content of it in the import wizard window.\n \nAfter that, you will see a new folder under the Prefabs folder <b>Countdowns TMP</b> where you will find all the Text Mesh Pro countdown prefabs ready to use.");
    }

    void EventsDoc()
    {
        DrawText("The Countdown system includes a series of events that you can use to easily integrate with your project in order to set the logic to what to do when these events are invoked.\n \nThe events are called when:  countdown start, the count value change, the countdown finish, and also a customizable event list that you can use to trigger a callback when the countdown reach a specific value/second.\n \nYou can set callbacks for all those events without code by adding the listener in the <b>bl_Coundown.cs inspector</b>:");
        DrawServerImage(4);
        DrawText("If you wanna add an event that is called when the countdown reaches a specific value e.g when the countdown reaches 3 > invoke the callback, you can do so in the inspector by adding a new field in the <b>Count Events</b> list ➔ Set the time in the <b>Trigger Time</b> box and add the callback.");
        DrawServerImage("img-5.png");
        DrawHorizontalSeparator();
        DrawText("You can also add events by script in runtime when you start the countdown like this:");
        DrawCodeText("public bl_Countdown countdown;\n \n    public void StartCount()\n    {\n        countdown.StartCountdown().OnStartCount(() => { /*code to execute*/})\n            .OnCountChange((count) => { /*code to execute*/ })\n            .OnFinishCount(() => { /*code to execute*/ })\n            .OnCountAt(3, () => { /*add a callback to be invoked when count reach 3*/ });\n    }");
    }

    void SettingsDoc()
    {
        DrawTitleText("bl_Countdown.cs");
        DrawText("This script contains all the logic of the countdown, the object where it's attached should be active before start the countdown.");
        DrawServerImage(2);
        DrawPropertieInfo("Start Time", "int", "The second where the countdown will start.");
        DrawPropertieInfo("Finish Time", "int", "The second where the countdown finish, usually 0 or 1");
        DrawPropertieInfo("Count Speed", "float", "The time scale, 1(normal) = 1 sec, 2 = 0.5 sec, 0.5 = 2 sec, etc...");
        DrawPropertieInfo("Start Delay", "float", "Delay time to start to the countdown once you invoke the start function.");
        DrawPropertieInfo("Finish Delay", "float", "Delay time to invoke all the finish events once the countdown is complete.");
    }

    void CountdownUIDoc()
    {
        DrawTitleText("bl_CountdownUI.cs");
        DrawText("This script manage all relating to the UI Behavior based on the countdown events.");
        DrawServerImage(3);
        DrawPropertieInfo("Count Display Number", "enum", "How to display the seconds, as integer or with decimals.");
        DrawPropertieInfo("Replace Last With", "string", "In case you wanna replace the last count number with another text e.g: instead of 0 > Start!, if not, just leave it empty.");
        DrawPropertieInfo("Auto Hide On Finish", "bool", "Automatically hide the UI after countdown finish?");
        DrawPropertieInfo("Decimal Text Size", "float", "In case you display decimal numbers, and you want to show the decimals text a little bit smaller that the integer second, set the size here.");
        DrawPropertieInfo("Custom Format", "string", "If you want to show the count time in a custom string format e.g: remain: {0} or <color=red>{0}<\\color>, etc...");
    }

    [MenuItem("Window/Documentation/Countdown")]
    static void Open()
    {
        GetWindow<CountdownDoc>();
    }
}