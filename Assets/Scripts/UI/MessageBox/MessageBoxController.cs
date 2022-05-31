using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MessageBoxController : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset _messageBoxTemplate;
    [SerializeField] private string _captionText;
    [SerializeField] private string _text;
    [SerializeField] public string _okButtonText = "Ok";
    [SerializeField] public string _cancelButtonText = "Cancel";
    [SerializeField] private MessageBoxButtons _messageBoxButtons;
    [SerializeField] private MessageBoxIcon _messageBoxIcon;
    [SerializeField] private Sprite _informationSprite;
    [SerializeField] private Sprite _warningSprite;
    [SerializeField] private Sprite _errorSprite;
    [SerializeField] private Sprite _questionSprite;
    [SerializeField] private float _width;
    [SerializeField] private float _height;
    [SerializeField] private bool _useScreenSize;

    private readonly string BackgroundPanelName = "Background";
    private readonly string MessageBoxPanelName = "MessageBox";
    private readonly string OkButtonName = "OkButton";
    private readonly string CancelButtonName = "CancelButton";
    private readonly string CaptionLabelName = "CaptionLabel";
    private readonly string TextLabelName = "TextLabel";
    private readonly string CloseButtonName = "CloseButton";
    private readonly string IconName = "Icon";
    private readonly string ButtonClassName = "messageBoxButton";
    private readonly string HideButtonClassName = "hideButton";

    private UIDocument _uiDocument;
    private VisualElement _root;
    private VisualElement _backgroundPanel;
    private VisualElement _messageBoxPanel;
    private Button _okButton;
    private Button _cancelButton;
    private Button _closeButton;
    private Label _captionLabel;
    private Label _textLabel;
    private VisualElement _icon;

    public event Action<bool> Closed;

    private void Awake()
    {
        _uiDocument = GetComponent<UIDocument>() ?? throw new ArgumentNullException(nameof(_uiDocument));
        _root = _messageBoxTemplate.Instantiate();
        _root.style.position = Position.Absolute;
        _root.style.left = 0;
        _root.style.top = 0;
        _root.style.visibility = Visibility.Hidden;

        _uiDocument.rootVisualElement.Add(_root);
        _backgroundPanel = _root.Q<VisualElement>(BackgroundPanelName) ?? throw new ArgumentNullException(nameof(_messageBoxPanel));
        _messageBoxPanel = _root.Q<VisualElement>(MessageBoxPanelName) ?? throw new ArgumentNullException(nameof(_messageBoxPanel));
        _okButton = _root.Q<Button>(OkButtonName) ?? throw new ArgumentNullException(nameof(_okButton));
        _cancelButton = _root.Q<Button>(CancelButtonName) ?? throw new ArgumentNullException(nameof(_cancelButton));
        _closeButton = _root.Q<Button>(CloseButtonName) ?? throw new ArgumentNullException(nameof(_closeButton));
        _captionLabel = _root.Q<Label>(CaptionLabelName) ?? throw new ArgumentNullException(nameof(_captionLabel));
        _textLabel = _root.Q<Label>(TextLabelName) ?? throw new ArgumentNullException(nameof(_textLabel));
        _icon = _root.Q<VisualElement>(IconName) ?? throw new ArgumentNullException(nameof(_icon));

        if (_width == 0f)
        {
            _width = 400f;
        }
        if (_height == 0f)
        {
            _height = 200f;
        }
        SetHidden();
    }

    public void Show(string caption = null, string text = null, MessageBoxButtons? buttons = null, MessageBoxIcon? icon = null)
    {
        _captionText = caption ?? _captionText;
        _text = text ?? _text;
        _captionLabel.text = _captionText;
        _textLabel.text = _text;
        _messageBoxButtons = buttons ?? _messageBoxButtons;
        _messageBoxIcon = icon ?? _messageBoxIcon;
        _okButton.text = _okButtonText;
        _cancelButton.text = _cancelButtonText;
        SetSizeAndLocation();
        SetButtonHandlers();
        SetIconSprite();
        SetVisible();
    }

    private void SetButtonHandlers()
    {
        _okButton.clicked += OnOkButtonClicked;

        if (_messageBoxButtons == MessageBoxButtons.OkCancel)
        {
            // Only subscribe if cancel  
            _cancelButton.clicked += OnCancelButtonClicked;
            _cancelButton.RemoveFromClassList(HideButtonClassName);
            _cancelButton.AddToClassList(ButtonClassName);
        }
        else
        {
            _cancelButton.RemoveFromClassList(ButtonClassName);
            _cancelButton.AddToClassList(HideButtonClassName);
        }
        _closeButton.clicked += OnCancelButtonClicked;
    }

    private void SetIconSprite()
    {
        Sprite iconSprite;
        switch (_messageBoxIcon)
        {
            case MessageBoxIcon.Question:
                iconSprite = _questionSprite;
                break;
            case MessageBoxIcon.Warning:
                iconSprite = _warningSprite;
                break;
            case MessageBoxIcon.Information:
                iconSprite = _informationSprite;
                break;
            case MessageBoxIcon.Error:
                iconSprite = _errorSprite;
                break;
            default:
                iconSprite = null;
                break;
        }
        if (iconSprite != null)
        {
            _icon.style.backgroundImage = new StyleBackground(iconSprite);
        }
        else
        {
            _icon.style.backgroundImage = new StyleBackground(StyleKeyword.None);
        }
    }

    public void SetVisible()
    {
        _backgroundPanel.style.visibility = Visibility.Visible;
        _messageBoxPanel.style.visibility = Visibility.Visible;
    }

    public void SetHidden()
    {
        _backgroundPanel.style.visibility = Visibility.Hidden;
        _messageBoxPanel.style.visibility = Visibility.Hidden;
    }

    public void SetSizeAndLocation(float? width = null, float? height = null)
    {
        _width = width ?? _width;
        _height = height ?? _height;
        _messageBoxPanel.style.width = new Length(_width, LengthUnit.Pixel);
        _messageBoxPanel.style.height = new Length(_height, LengthUnit.Pixel);
        
        float parentW = _useScreenSize ? Screen.width : _uiDocument.rootVisualElement.layout.width;
        float parentH = _useScreenSize ? Screen.height : _uiDocument.rootVisualElement.layout.height;
        _root.style.width = parentW;
        _root.style.height = parentH;
        _messageBoxPanel.style.left = (parentW - _width) / 2f;
        _messageBoxPanel.style.top = (parentH - _height) / 2f;
    }

    private void OnCancelButtonClicked()
    {
        CloseDialog(false);
    }

    private void OnOkButtonClicked()
    {
        CloseDialog(true);
    }

    private void CloseDialog(bool okClicked)
    {
        SetHidden();
        _okButton.clicked -= OnOkButtonClicked;
        _cancelButton.clicked -= OnCancelButtonClicked;
        _closeButton.clicked -= OnCancelButtonClicked;
        Closed?.Invoke(okClicked);
    }
}