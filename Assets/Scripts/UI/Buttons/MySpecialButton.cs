using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[UnityEngine.Scripting.Preserve] 
public class MySpecialButton : Button
{
	[UnityEngine.Scripting.Preserve]
	public new class UxmlFactory : UxmlFactory<MySpecialButton, UxmlTraits> { }

	[UnityEngine.Scripting.Preserve]
	public new class UxmlTraits : VisualElement.UxmlTraits
	{
		readonly UxmlEnumAttributeDescription<MySpecialType> mySpecialType = new() { name = "my-special-type", defaultValue = MySpecialType.NotSet };
		readonly UxmlIntAttributeDescription rotation = new() { name = "rotation", defaultValue = 0, restriction = new UxmlValueBounds { min = "0", max = "3" } };

		public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
		{
			get { yield break; }
		}

		public override void Init(VisualElement visualElement, IUxmlAttributes attributes, CreationContext creationContext)
		{
			base.Init(visualElement, attributes, creationContext);
			var element = visualElement as MySpecialButton;

			if (element != null)
			{
				element.MySpecialType = mySpecialType.GetValueFromBag(attributes, creationContext);
				element.Rotation = rotation.GetValueFromBag(attributes, creationContext);
			}
		}
	}



	private readonly string TemplatePath = $"UI/MySpecialButton/MySpecialButton-template";
	private readonly string BaseUssClass = "my-special-button";
	private readonly string RotateButtonName = "RotateButton";
	
	private Button _rotateButton;
	private VisualElement _icon;
	private VisualElement _container;

	public override VisualElement contentContainer => this._container;
	public MySpecialType MySpecialType { get; private set; } = MySpecialType.NotSet;
	public int Rotation { get; private set; } = 0;
	public VisualElement RootElement { get; private set; }
	public MySpecialButton() : base()
	{
		var template = Resources.Load<VisualTreeAsset>(TemplatePath);
		if (template == null)
		{
			throw new Exception($"{this.GetType().Name} - Unable to load UXML template from {TemplatePath}");
		}
		template.CloneTree(this);
		this.AddToClassList(BaseUssClass);

		_rotateButton = this.Q<Button>(RotateButtonName) ?? throw new Exception($"Element with name '{RotateButtonName}' not defined.");
		_container = this.Q<VisualElement>("content-container") ?? throw new Exception("Element with name 'content-container' not defined.");
		
		RegisterCallback<AttachToPanelEvent>(OnAttachToPanelEvent);
		RegisterCallback<DetachFromPanelEvent>(OnDetatchFromPanelEvent);
	}

    void OnAttachToPanelEvent(AttachToPanelEvent e)
	{
		// How do I make the icon part of the base UXML layout AND let the user specify the sprite?
		_icon = this.Q("Icon");
		if(_icon == null)
        {
			Debug.LogWarning($"{GetType().Name} - Missing required child element 'Icon'");
		}
		
		clicked += OnButtonClicked;
		_rotateButton.clicked += OnRotateButtonClicked;
	}

	private void OnDetatchFromPanelEvent(DetachFromPanelEvent evt)
	{
		clicked -= OnButtonClicked;
		_rotateButton.clicked -= OnRotateButtonClicked;
	}

	private void OnRotateButtonClicked()
	{
		Rotation = (Rotation + 1) % 4;
		if (_icon != null)
		{
			_icon.transform.rotation = Quaternion.Euler(0, 0, Rotation * 90);
		}
		else
        {
			Debug.LogError($"{GetType().Name} - Missing required child element 'Icon'");
        }
	}

    private void OnButtonClicked()
    {
		Debug.Log($"{name} - OnButtonClick() - {MySpecialType}/{Rotation}");
    }
}
