# Stackable Decorator
**Stackable Decorator** is a set of property drawers which allow you to stack multiple decorators on it, each decorator also modifies all stacked decorators and drawers.

For example, a box decorator stacked on a property field, it draws a padded box background on the field. If a color decorator stacked on, the color of the box will be changed. Then, a heading decorator stacked on, a heading is drawn above the boxed field.

```CSharp
[Heading(title = "Heading", order = 2)]
[Color(0.5f, 0.5f, 1f, 1, order = 1)]
[Box(2, 2, 2, 2)]
[StackableField]
public string field;
```
![](https://raw.githubusercontent.com/Kinwailo/Wiki-Images/master/Stackable%20Decorator/Sample.png)

StackableField is standard property drawer with stackable decorator support. The order of the decorator is the stack order from inside to outside.

Here is the code that create the logo of **Stackable Decorator** with  a bool field:
```CSharp
[Heading(top: 8, order = 7)]
[HorizontalGroup("Logo", 0, 98, order = 6)]
[Box(4, 4, 4, 4, style = "flow node 1 on", order = 5)]
[Heading(top: 2, bottom: 6, title = "Stackable", width = -1,
    style = "WarningOverlay", order = 4)]
[Color(1f, 1f, 0.5f, 1, order = 3)]
[Box(4, 4, 4, 4, style = "flow overlay box", order = 2)]
[Label(-1)]
[StackableField]
public bool Decorator = true;
```
![](https://raw.githubusercontent.com/Kinwailo/Wiki-Images/master/Stackable%20Decorator/Logo.png)

# How to use
Please read the [wiki](https://github.com/Kinwailo/Stackable-Decorator/wiki) for the document of property drawers and decorators.

# Link
\[ [Asset Store](https://www.assetstore.unity3d.com/#!/content/111270) \] - \[ [Forum Thread](https://forum.unity.com/threads/free-open-source-stackable-decorator-property-drawer-with-multiple-stackable-decorator.520856/) \] - \[ [WIP Thread](https://forum.unity.com/threads/free-stackable-decorator-beta-property-drawer-with-multiple-stackable-decorator.514943/) \]

# Update
Added [TextField](https://github.com/Kinwailo/Stackable-Decorator/wiki/StackableFieldAttribute#textfieldattribute), [DropdownValue](https://github.com/Kinwailo/Stackable-Decorator/wiki/EnumPopupAttribute#dropdownvalueattribute), [EnumPopup](https://github.com/Kinwailo/Stackable-Decorator/wiki/EnumPopupAttribute), [EnumMaskPopup](https://github.com/Kinwailo/Stackable-Decorator/wiki/EnumMaskPopupAttribute), [LayerMaskPopup](https://github.com/Kinwailo/Stackable-Decorator/wiki/EnumMaskPopupAttribute#layermaskpopupattribute), [DropdownMask](https://github.com/Kinwailo/Stackable-Decorator/wiki/EnumMaskPopupAttribute#dropdownmaskattribute)  
Added [ColorField](https://github.com/Kinwailo/Stackable-Decorator/wiki/StackableFieldAttribute#colorfieldattribute), [CurveField](https://github.com/Kinwailo/Stackable-Decorator/wiki/StackableFieldAttribute#curvefieldattribute), [ProgressBar](https://github.com/Kinwailo/Stackable-Decorator/wiki/StackableFieldAttribute#progressbarattribute), [TagPopup](https://github.com/Kinwailo/Stackable-Decorator/wiki/TagPopupAttribute), [LayerPopup](https://github.com/Kinwailo/Stackable-Decorator/wiki/TagPopupAttribute#layerpopupattribute), [SortingLayerPopup](https://github.com/Kinwailo/Stackable-Decorator/wiki/TagPopupAttribute#sortinglayerpopupattribute), [InputAxisPopup](https://github.com/Kinwailo/Stackable-Decorator/wiki/TagPopupAttribute#inputaxispopupattribute), [AnimatorParameterPopup](https://github.com/Kinwailo/Stackable-Decorator/wiki/TagPopupAttribute#animatorparameterpopupattribute)
