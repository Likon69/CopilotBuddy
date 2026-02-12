# Styx.Common.WpfControls

Contains common WPF controls.

## Contents

- [MenuButton Class](#menubutton-class)
- [SplitButton Class](#splitbutton-class)

---

### MenuButton Class

**Inheritance:** System.Object → System.Windows.Threading.DispatcherObject → System.Windows.DependencyObject → System.Windows.Media.Visual → System.Windows.UIElement → System.Windows.FrameworkElement → System.Windows.Controls.Control → System.Windows.Controls.ContentControl → System.Windows.Controls.Primitives.ButtonBase → System.Windows.Controls.Button → Styx.Common.WpfControls.SplitButton → Styx.Common.WpfControls.MenuButton

```csharp
public class MenuButton : SplitButton
```

Implements a "menu button" for Silverlight and WPF.

#### MenuButton Methods

- **`OnClick Method`**
  ```csharp
  protected override void OnClick()
  ```

---

### SplitButton Class

**Inheritance:** System.Object → System.Windows.Threading.DispatcherObject → System.Windows.DependencyObject → System.Windows.Media.Visual → System.Windows.UIElement → System.Windows.FrameworkElement → System.Windows.Controls.Control → System.Windows.Controls.ContentControl → System.Windows.Controls.Primitives.ButtonBase → System.Windows.Controls.Button → Styx.Common.WpfControls.SplitButton → Styx.Common.WpfControls.MenuButton

```csharp
public class SplitButton : Button
```

Implements a "split button" for Silverlight and WPF.

#### SplitButton Properties

- **`AlwaysOpenMenu Property`**
  ```csharp
  public bool AlwaysOpenMenu { get; set; }
  ```
  Specifies whether or not the click event on button is routed to open context menu

- **`ButtonMenuItemsSource Property`**
  ```csharp
  public Collection&lt;Object&gt; ButtonMenuItemsSource { get; }
  ```
  Gets the collection of items for the split button's menu.

- **`IsMouseOverSplitElement Property`**
  ```csharp
  protected bool IsMouseOverSplitElement { get; }
  ```

#### SplitButton Methods

- **`OnApplyTemplate Method`**
  ```csharp
  public override void OnApplyTemplate()
  ```
  Called when the template is changed.

- **`OnClick Method`**
  ```csharp
  protected override void OnClick()
  ```

- **`OnKeyDown Method`**
  ```csharp
  protected override void OnKeyDown(
KeyEventArgs e
)
  ```
  - *e*: Type: System.Windows.InputAddLanguageSpecificTextSet("LSTE8E9B85E_1?cs=.|vb=.|cpp=::|nu=.|fs=.");KeyEventArgs

- **`OpenButtonMenu Method`**
  ```csharp
  protected void OpenButtonMenu()
  ```

#### SplitButton Fields

- **`AlwaysOpenMenuProperty Field`**
  ```csharp
  public static readonly DependencyProperty AlwaysOpenMenuProperty
  ```
  The DependencyProperty for the AlwaysOpenMenu property.
                Default Value:      false

---
