using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Graphene.Demo
{
  // Atomic "Über" object for the view
  [System.Serializable, Draw(ControlType.Button)]
  public class BindableObject : IRoute
  {
    [field: SerializeField]
    [Bind("Label", BindingMode.OneWay)]
    public string Name { get; set; }

    [field: SerializeField]
    [Bind("Value")]
    public string Value { get; set; }

    [field: SerializeField]
    [Route]
    public string route;

    [field: SerializeField]
    [BindTooltip("Tooltip")]
    public string Description { get; set; }

    #region FoldoutAttribute
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.FoldoutGroup("Additionals")]
#elif NAUGHTY_ATTRIBUTES
    [NaughtyAttributes.Foldout("Additionals")]
#endif
    #endregion
    public string addClass;

    #region FoldoutAttribute
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.FoldoutGroup("Additionals")]
#elif NAUGHTY_ATTRIBUTES
    [NaughtyAttributes.Foldout("Additionals")]
#endif
    #endregion
    [Bind("")]
    public UnityEvent OnClick;
  }


  [System.Serializable]
  public class BindableBaseField<T>: INotifyValueChanged<T>
  {
    [SerializeField]
    protected T m_Value;

    [BindBaseField("Value")]
    public virtual T value { get => m_Value; set {
        SetValueWithoutNotify(value);
        ValueChangeCallback(value);
      } 
    }

    [field: SerializeField]
    [Bind("Label", BindingMode.OneWay)]
    public virtual string Label { get; set; }

    [BindValueChangeCallback("ValueChange")]
    public EventCallback<ChangeEvent<T>> ValueChange => (changeEvent) => { ValueChangeCallback(changeEvent.newValue); };

    public event System.EventHandler<T> OnValueChange;

    public virtual void SetValueWithoutNotify(T newValue)
    {
      m_Value = value;
    }

    protected virtual void ValueChangeCallback(T value)
    {
      OnValueChange?.Invoke(this, value);
    }
  }


  [System.Serializable, Draw(ControlType.Toggle)]
  public class BindableBool : BindableBaseField<bool>
  {
  }



  [System.Serializable, Draw(ControlType.Slider)]
  public class RangeBaseField<TValueType> : BindableBaseField<TValueType>
  {
    public virtual float normalizedValue { get => throw new System.NotImplementedException(); }

    public TValueType min;
    public TValueType max;

  }

  [System.Serializable, Draw(ControlType.Slider)]
  public class BindableFloat : RangeBaseField<float>
  {
    public override float normalizedValue => m_Value / (max - min);

    public BindableFloat()
    {
      min = 0;
      max = 1;
      m_Value = 0.5f;
    }
  }

  [System.Serializable, Draw(ControlType.SliderInt)]
  public class BindableInt : RangeBaseField<int>
  {
    public override float normalizedValue => (float)m_Value / ((float)max - (float)min);

    public BindableInt()
    {
      min = 0;
      max = 100;
      m_Value = 50;
    }
  }

  [System.Serializable, Draw(ControlType.SelectField)]
  public class BindableNamedInt : BindableBaseField<int>
  {
    [field: SerializeField]
    [Bind("Items")]
    public List<string> items { get; set; } = new List<string>();

    public float normalizedValue => (float)m_Value / items.Count;

    public void InitFromEnum<T>()
    {
      this.items.Clear();

      foreach (string s in System.Enum.GetNames(typeof(T)).ToList())
        this.items.Add(s);
    }
    public void InitFromList(IEnumerable<string> list)
    {
      this.items.Clear();
      this.items = list.ToList();
    }
  }

}
