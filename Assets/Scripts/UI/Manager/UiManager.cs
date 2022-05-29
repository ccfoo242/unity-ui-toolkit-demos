using UnityEngine;
using UnityEngine.UIElements;

public class UiManager: MonoBehaviour
{
    private static UiManager _instance;
    public static UiManager Instance
    {
        get
        {
            if (_instance == null)
            {
                throw new UnityException("UIManager instance is null");
            }
            return _instance;
        }
    }

    private UIDocument _uiDocument;
    private VisualElement _root;
    private MessageBoxController _messageBox;
    private MySpecialButton _specialButton;

    private void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
        _root = _uiDocument.rootVisualElement;
        _messageBox = GetComponent<MessageBoxController>();
        _specialButton = _root.Q<MySpecialButton>();
        _specialButton.clicked += OnSpecialButtonClicked;
    }

    private void OnSpecialButtonClicked()
    {
        Debug.Log($"{this.GetType().Name} - OnSpecialButtonClicked");
        string text = "First shalt thou take out the Holy Pin. "+
            "Then, shalt thou count to three. No more. No less. "+
            "Three shalt be the number thou shalt count, "+
            "and the number of the counting shall be three. "+
            "Four shalt thou not count, nor either count thou two, "+
            "excepting that thou then proceed to three. "+
            "Five is right out. Once the number three, "+
            "being the third number, be reached, then, "+
            "lobbest thou thy Holy Hand Grenade of Antioch "+
            "towards thy foe, who, being naughty in "+
            "My sight, shall snuff it.";

        _messageBox.Closed += OnMessageBoxClosed;
        _messageBox.Show("Help, help, I'm bring repressed!", text, MessageBoxButtons.OkCancel, MessageBoxIcon.Information);
        _specialButton.SetEnabled(false);
    }

    private void OnMessageBoxClosed(bool isOk)
    {
        _messageBox.Closed -= OnMessageBoxClosed;
        Debug.Log($"{this.GetType().Name} - OnMessageBoxClosed, isOk={isOk}");
        _specialButton.SetEnabled(true);
    }
}

