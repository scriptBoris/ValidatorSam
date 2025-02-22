![icon of ValidatorSam.Fody library.](https://github.com/scriptBoris/ValidatorSam/blob/master/Nuget/icon.png)

# ValidatorSam
[![nuget](https://img.shields.io/nuget/v/ValidatorSam.Fody.svg)](https://www.nuget.org/packages/ValidatorSam.Fody/)

Simple MVVM-oriented library for validating user input. Works on the principle of lambda expressions and fluent style coding. <br/>
Works on Netstandard2.1 and is fully compatible with all UI platforms where there is support for netstandard2.1

## Fody
The library also uses the Fody postprocessor, which allows you to use a simple getter to create instance, instead of explicitly 
creating an instance in the constructor or anywhere else. Which will reduce routine amount of code in constructors
and will increase readability of code.

More info: [How it works?](https://github.com/scriptBoris/ValidatorSam/wiki/How-it-work-with-Fody%3F)

## Video demo
<div align="center">
  <video width="300" src="https://private-user-images.githubusercontent.com/29772096/415874389-7e8c8ac3-4cba-49fe-b244-d99f6e910ef2.mp4?jwt=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJnaXRodWIuY29tIiwiYXVkIjoicmF3LmdpdGh1YnVzZXJjb250ZW50LmNvbSIsImtleSI6ImtleTUiLCJleHAiOjE3NDAxOTU2MDUsIm5iZiI6MTc0MDE5NTMwNSwicGF0aCI6Ii8yOTc3MjA5Ni80MTU4NzQzODktN2U4YzhhYzMtNGNiYS00OWZlLWIyNDQtZDk5ZjZlOTEwZWYyLm1wND9YLUFtei1BbGdvcml0aG09QVdTNC1ITUFDLVNIQTI1NiZYLUFtei1DcmVkZW50aWFsPUFLSUFWQ09EWUxTQTUzUFFLNFpBJTJGMjAyNTAyMjIlMkZ1cy1lYXN0LTElMkZzMyUyRmF3czRfcmVxdWVzdCZYLUFtei1EYXRlPTIwMjUwMjIyVDAzMzUwNVomWC1BbXotRXhwaXJlcz0zMDAmWC1BbXotU2lnbmF0dXJlPTViNjRkMTE5YWYyMmEzODQxZjY4OTllNjY1ODFmYTBmOWQyNjI2M2ZjODIzMWE4MmY5YzI0NzEyNDY2ZjI0ZmYmWC1BbXotU2lnbmVkSGVhZGVycz1ob3N0In0.HUCAmEH4DdqLfuCRvLYTaPvXnCXWHFOaVv0ZjFixUfQ" 
      alt="maui" />
</div>

## UI recommendations
If your target type is string, int, long or other numbers types so i'm recommend use RawValue for binding UI<-->Data

```XAML
<!--for MAUI example-->
<Entry Text="{Binding UserAge.RawValue}" />
```
Because the library uses built-in preprocessors that can modify user input. 
And to work correctly, they need to separate the concept of displayed data (RawValue) and real data (Value). 
That is, the user can enter "0.5" in the input field, but the real data will be 0.5.

The same applies to strings. For example, when implementing telephone masks: for example, the user should see "+1 (123) - 456 - 78 - 90" in the input field, but in reality the value will be 11234567890

## C# example
```C#
using ValidatorSam;

public class RegisterViewModel
{
    public Validator<string> Email => Validator<string>.Build()
        .UsingRule(x => MailAddress.TryCreate(x, out _), "No valid email")
        .UsingRule(x => !x.Contains(' '), "Don't use spaces in email")
        .UsingRequired();

    public Validator<int?> UserAge => Validator<int?>.Build()
        .UsingRule(x => x >= 18, "User must be of legal age")
        .UsingLimitations(0, 100);

    public Validator<string> Login => Validator<string>.Build()
        .UsingRule(x=> !x.Contains(' '), "Don't use spaces in login")
        .UsingTextLimit(0, 20)
        .UsingRequired();

    public Validator<string> Password => Validator<string>.Build()
        .UsingRule(x => !x.Contains(' '), "Don't use spaces in password")
        .UsingRequired();

    public ICommand CommandRegister => new Command(() =>
    {
        // And you will invoke this code for get validator result and invalid fields
        if (!Validator.GetAll(this).TryCheckSuccess(out var invalids))
        {
            var errors = invalids.Select(x => $"Field \"{x.Name}\": {x.TextError}");
            string msg = string.Join('\n', errors);
            MessageBox.Show(msg, "Fail");
            return;
        }

        var model = new
        {
            Email = Email.Value,
            UserAge = UserAge.Value,
            Login = Login.Value,
            Password = Password.Value,
        };
        string json = JsonSerializer.Serialize(model);
        MessageBox.Show($"You have successfully register!\n" + json);
    });
}
```

## XAML/MAUI Example
```XAML
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="SampleApp.RegisterPage"
             Title="Registration">
  <VerticalStackLayout>
    <Entry Text="{Binding Email.RawValue}"/>
    <Label Text="{Binding Email.TextError}" />

    <Entry Text="{Binding UserAge.RawValue}"/>
    <Label Text="{Binding UserAge.TextError}" />

    <Entry Text="{Binding Login.RawValue}"/>
    <Label Text="{Binding Login.TextError}" />

    <Entry Text="{Binding Password.RawValue}"/>
    <Label Text="{Binding Password.TextError}" />

    <Button Text="Accept"
            Command="{Binding CommandRegister}" />
  </VerticalStackLayout>
</ContentPage>
```

## License
License MIT
(Happy coding!)

## Support me
If you want you make donate me :)
<div align="left">
    <a href="https://www.buymeacoffee.com/scriptboris">
        <img src="https://img.buymeacoffee.com/button-api/?text=Buy me a food&emoji=🍔&slug=scriptboris&button_colour=FFDD00&font_colour=000000&font_family=Comic&outline_colour=000000&coffee_colour=ffffff" />
    </a>
</div>
