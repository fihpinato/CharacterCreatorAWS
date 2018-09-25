# Unity-Console
An in-game console for Unity games

[![Codacy Badge](https://api.codacy.com/project/badge/Grade/f8a05421a37846c2a7a2156284a773ad)](https://www.codacy.com/app/javierbullrich/Unity-Console?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=Bullrich/Unity-Console&amp;utm_campaign=Badge_Grade)

![rotation](Docs/rotation.png)

![custom-methods](Docs/custom-methods.png)

![log-detail](Docs/logs-detail.png)

## TOC

- [TOC](#TOC)
- [Setup](#Setup)
- [Adding methods](#adding-methods)
- [Sending a custom message](#sending-a-custom-message)

## Setup

[Download the latest release](../../releases)

To setup the Console, simply drag the prefab **Console Prefab** from Blue/Console directory to the scene.

To see the stack trace in a build be sure to have "Development build" toggled on.

 ## Adding methods

To add a method simply type: 
```csharp
Blue.GameConsole.AddAction(method, buttonDescription);
```

 Currently, the console accept four types of delegates:
 - Void
 - Bool
 - Int
 - Float

The following methods are used to add buttons to the console:

 ```csharp
 // Void
Blue.GameConsole.AddAction(voidMethod, "Description");

// Bool
Blue.GameConsole.AddAction(voidMethodThatReceivesABoolean, "Description", defaultValue = false);

// Float
Blue.GameConsole.AddAction(voidMethodThatReceivesFloat, "Description", defaultValue = 0f);

// Int
Blue.GameConsole.AddAction(voidMethodThatReceivesInt, "Description", defaultValue = 0);
 ```

 A simple example would be:

 ```csharp
 bool gateStatus = true;

 void Start(){
     Blue.GameConsole.AddAction(ChangeGateStatus, "Change the gate status", gateStatus);
 }

 void ChangeGateStatus(bool newStatus){
     gateStatus = newStatus;
 }
 ```

This will add a button with a boolean parameters set, by the default parameter, to true.

When that button is pressed the `ChangeGateStatus` method will be called with the new boolean value.

 You can find an example of adding methods in [Demo.cs](Assets/Blue/Console/Demo/Demo.cs)

 ## Sending a custom message

 The console allows to receive custom messages, this can easily be achieved with the following method
 ```csharp
 Blue.GameConsole.WriteMessage("Title", "Message");
 ```

 The message will appear as a information message in the console.
